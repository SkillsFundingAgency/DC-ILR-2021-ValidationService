using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
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
        private readonly IFileDataCache _fileDataCache;
        private readonly IValidationErrorCache<U> _validationErrorCache;
        private readonly IValidationOutputService _validationOutputService;
        private readonly IRuleSetOrchestrationService<IMessage, U> _ruleSetOrchestrationService;
        private readonly IValidationExecutionProvider<U> _validationExecutionProvider;
        private readonly ILogger _logger;

        public PreValidationOrchestrationSfService(
            IPopulationService preValidationPopulationService,
            IFileDataCache fileDataCache,
            IValidationErrorCache<U> validationErrorCache,
            IValidationOutputService validationOutputService,
            IRuleSetOrchestrationService<IMessage, U> ruleSetOrchestrationService,
            IValidationExecutionProvider<U> validationExecutionProvider,
            ILogger logger)
        {
            _preValidationPopulationService = preValidationPopulationService;
            _fileDataCache = fileDataCache;
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

                await _preValidationPopulationService.PopulateAsync(validationContext, cancellationToken).ConfigureAwait(false);
                _logger.LogDebug($"Population service completed in: {stopWatch.ElapsedMilliseconds}");

                // Set the filename
                _fileDataCache.FileName = validationContext.Filename;

                // File Validation
                await _ruleSetOrchestrationService.ExecuteAsync(validationContext, cancellationToken).ConfigureAwait(false);

                cancellationToken.ThrowIfCancellationRequested();

                if (_validationErrorCache.ValidationErrors.Any(IsFail))
                {
                    _logger.LogDebug(
                        $"File schema catastrophic error, so will not execute learner validation actors, error count: {_validationErrorCache.ValidationErrors.Count}");
                    return;
                }

                await _validationExecutionProvider.ExecuteAsync(validationContext, cancellationToken).ConfigureAwait(false);

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
