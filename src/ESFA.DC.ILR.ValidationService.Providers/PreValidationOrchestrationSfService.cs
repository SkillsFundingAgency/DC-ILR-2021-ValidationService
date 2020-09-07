using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReferenceDataService.Model.Learner;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Interface.Enum;
using ESFA.DC.ILR.ValidationService.Providers.Utils;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.ILR.ValidationService.Providers
{
    public class PreValidationOrchestrationSfService : IPreValidationOrchestrationService
    {
        private readonly IPopulationService _preValidationPopulationService;
        private readonly IProvider<IMessage> _messageProvider;
        private readonly IProvider<ReferenceDataRoot> _referenceDataRootProvider;
        private readonly IProvider<LearnerReferenceData> _learnerReferenceDataProvider;
        private readonly IValidationErrorCache _validationErrorCache;
        private readonly IValidationOutputService _validationOutputService;
        private readonly IValidIlrFileOutputService _validIlrFileOutputService;
        private readonly IRuleSetOrchestrationService<IMessage> _messageRuleSetOrchestrationService;
        private readonly ICrossYearRuleSetOrchestrationService<ILearner> _crossYearRuleSetOrchestrationService;
        private readonly IValidationExecutionProvider _validationExecutionProvider;
        private readonly ILogger _logger;

        public PreValidationOrchestrationSfService(
            IPopulationService preValidationPopulationService,
            IProvider<IMessage> messageProvider,
            IProvider<ReferenceDataRoot> referenceDataRootProvider,
            IProvider<LearnerReferenceData> learnerReferenceDataProvider,
            IValidationErrorCache validationErrorCache,
            IValidationOutputService validationOutputService,
            IValidIlrFileOutputService validIlrFileOutputService,
            IRuleSetOrchestrationService<IMessage> messageRuleSetOrchestrationService,
            ICrossYearRuleSetOrchestrationService<ILearner> crossYearRuleSetOrchestrationService,
            IValidationExecutionProvider validationExecutionProvider,
            ILogger logger)
        {
            _preValidationPopulationService = preValidationPopulationService;
            _messageProvider = messageProvider;
            _referenceDataRootProvider = referenceDataRootProvider;
            _learnerReferenceDataProvider = learnerReferenceDataProvider;
            _validationErrorCache = validationErrorCache;
            _validationOutputService = validationOutputService;
            _validIlrFileOutputService = validIlrFileOutputService;
            _messageRuleSetOrchestrationService = messageRuleSetOrchestrationService;
            _crossYearRuleSetOrchestrationService = crossYearRuleSetOrchestrationService;
            _validationExecutionProvider = validationExecutionProvider;
            _logger = logger;
        }

        public async Task ExecuteAsync(IValidationContext validationContext, CancellationToken cancellationToken)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                if (_validationErrorCache.ValidationErrors.Any())
                {
                    return;
                }

                cancellationToken.ThrowIfCancellationRequested();

                var message = await _messageProvider.ProvideAsync(validationContext, cancellationToken);
                var referenceDataRoot = await _referenceDataRootProvider.ProvideAsync(validationContext, cancellationToken);
                var learnerReferenceData = await _learnerReferenceDataProvider.ProvideAsync(validationContext, cancellationToken);

                _preValidationPopulationService.Populate(validationContext, message, referenceDataRoot, learnerReferenceData);

                _logger.LogDebug($"Population service completed in: {stopWatch.ElapsedMilliseconds}");

                // Stateless Validation
                var statelessValidationTasks = new List<Task>
                {
                    _messageRuleSetOrchestrationService.ExecuteAsync(message, cancellationToken),
                    _crossYearRuleSetOrchestrationService.ExecuteAsync(message.Learners, cancellationToken)
                };

                await Task.WhenAll(statelessValidationTasks).ConfigureAwait(false);

                // FRM Validation
                await _messageRuleSetOrchestrationService.ExecuteAsync(message, cancellationToken).ConfigureAwait(false);

                cancellationToken.ThrowIfCancellationRequested();

                if (_validationErrorCache.ValidationErrors.Any(IsFail))
                {
                    _logger.LogDebug($"File schema catastrophic error, so will not execute learner validation actors, error count: {_validationErrorCache.ValidationErrors.Count}");
                    await _validationOutputService.ProcessAsync(validationContext, message, _validationErrorCache.ValidationErrors, cancellationToken).ConfigureAwait(false);

                    throw new ValidationSeverityFailException("File level errors (Severity F) caught. Handing back to Message Handler.");
                }

                await _validationExecutionProvider.ExecuteAsync(validationContext, message, cancellationToken).ConfigureAwait(false);

                _logger.LogDebug($"Actors results collated {_validationErrorCache.ValidationErrors.Count} validation errors");

                cancellationToken.ThrowIfCancellationRequested();

                await _validationOutputService.ProcessAsync(validationContext, message, _validationErrorCache.ValidationErrors, cancellationToken).ConfigureAwait(false);

                await _validIlrFileOutputService.ProcessAsync(validationContext, message, cancellationToken).ConfigureAwait(false);

                _logger.LogDebug($"Validation final results persisted in {stopWatch.ElapsedMilliseconds}");
            }
            catch (Exception ex)
            {
                _logger.LogError("Validation Critical Error", ex);
                throw;
            }
        }

        private bool IsFail(IValidationError  validationError)
        {
            Severity severity = validationError.Severity ?? Severity.Error;
            return severity == Severity.Fail;
        }
    }
}
