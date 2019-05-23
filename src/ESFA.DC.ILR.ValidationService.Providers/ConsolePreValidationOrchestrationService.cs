using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Providers
{
    /// <summary>
    /// This orchestration service will combine both Pre and actual validation orchestrations,
    /// this could be used for console app and FIS
    /// </summary>
    /// <typeparam name="U">The type.</typeparam>
    public class ConsolePreValidationOrchestrationService<U> : IPreValidationOrchestrationService<U>
    {
        private readonly IPopulationService _preValidationPopulationService;
        private readonly IRuleSetOrchestrationService<ILearner, U> _learnerRuleSetOrchestrationService;
        private readonly IRuleSetOrchestrationService<IMessage, U> _messageRuleSetOrchestrationService;
        private readonly IValidationOutputService _validationOutputService;
        private readonly IFileDataCache _fileDataCache;

        public ConsolePreValidationOrchestrationService(
            IPopulationService preValidationPopulationService,
            IRuleSetOrchestrationService<ILearner, U> learnerRuleSetOrchestrationService,
            IRuleSetOrchestrationService<IMessage, U> messageRuleSetOrchestrationService,
            IValidationOutputService validationOutputService,
            IFileDataCache fileDataCache)
        {
            _preValidationPopulationService = preValidationPopulationService;
            _learnerRuleSetOrchestrationService = learnerRuleSetOrchestrationService;
            _messageRuleSetOrchestrationService = messageRuleSetOrchestrationService;
            _validationOutputService = validationOutputService;
            _fileDataCache = fileDataCache;
        }

        public async Task ExecuteAsync(
            IValidationContext preValidationContext,
            CancellationToken cancellationToken)
        {
            // get the file name
            _fileDataCache.FileName = preValidationContext.Filename;

            // get ILR data from file
            await _preValidationPopulationService.PopulateAsync(cancellationToken);

            await _messageRuleSetOrchestrationService.ExecuteAsync(new List<string>(), cancellationToken);
            await _learnerRuleSetOrchestrationService.ExecuteAsync(new List<string>(), cancellationToken);

            await _validationOutputService.ProcessAsync(CancellationToken.None);
        }
    }
}
