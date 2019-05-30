using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Interface.Enum;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.ILR.ValidationService.Providers
{
    public class PreValidationOrchestrationSfService<U> : IPreValidationOrchestrationService<U>
    {
        private readonly IPopulationService _preValidationPopulationService;
        private readonly IProvider<IMessage> _messageProvider;
        private readonly IProvider<ReferenceDataRoot> _referenceDataRootProvider;
        private readonly IValidationErrorCache<U> _validationErrorCache;
        private readonly IValidationOutputService _validationOutputService;
        private readonly IRuleSetOrchestrationService<IMessage, U> _ruleSetOrchestrationService;
        private readonly IValidationExecutionProvider<U> _validationExecutionProvider;
        private readonly ILogger _logger;

        public PreValidationOrchestrationSfService(
            IPopulationService preValidationPopulationService,
            IProvider<IMessage> messageProvider,
            IProvider<ReferenceDataRoot> referenceDataRootProvider,
            IValidationErrorCache<U> validationErrorCache,
            IValidationOutputService validationOutputService,
            IRuleSetOrchestrationService<IMessage, U> ruleSetOrchestrationService,
            IValidationExecutionProvider<U> validationExecutionProvider,
            ILogger logger)
        {
            _preValidationPopulationService = preValidationPopulationService;
            _messageProvider = messageProvider;
            _referenceDataRootProvider = referenceDataRootProvider;
            _validationErrorCache = validationErrorCache;
            _validationOutputService = validationOutputService;
            _ruleSetOrchestrationService = ruleSetOrchestrationService;
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

                _preValidationPopulationService.Populate(validationContext, message, referenceDataRoot);
                _logger.LogDebug($"Population service completed in: {stopWatch.ElapsedMilliseconds}");
                
                // File Validation
                await _ruleSetOrchestrationService.ExecuteAsync(validationContext, message, cancellationToken).ConfigureAwait(false);

                cancellationToken.ThrowIfCancellationRequested();

                if (_validationErrorCache.ValidationErrors.Any(IsFail))
                {
                    _logger.LogDebug(
                        $"File schema catastrophic error, so will not execute learner validation actors, error count: {_validationErrorCache.ValidationErrors.Count}");
                    return;
                }

                await _validationExecutionProvider.ExecuteAsync(validationContext, message, cancellationToken).ConfigureAwait(false);

                _logger.LogDebug(
                    $"Actors results collated {_validationErrorCache.ValidationErrors.Count} validation errors");
            }
            catch (Exception ex)
            {
                _logger.LogError("Validation Critical Error", ex);
                throw;
            }
            finally
            {
                cancellationToken.ThrowIfCancellationRequested();
                await _validationOutputService.ProcessAsync(validationContext, cancellationToken).ConfigureAwait(false);
                _logger.LogDebug($"Validation final results persisted in {stopWatch.ElapsedMilliseconds}");
            }
        }

        private bool IsFail(U item)
        {
            Severity severity = ((IValidationError)item).Severity ?? Severity.Error;
            return severity == Severity.Fail;
        }
    }
}
