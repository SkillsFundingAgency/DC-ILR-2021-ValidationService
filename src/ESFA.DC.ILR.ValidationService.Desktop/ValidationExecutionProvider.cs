using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.ILR.ValidationService.Desktop
{
    public class ValidationExecutionProvider : IValidationExecutionProvider
    {
        private readonly ILogger _logger;
        private readonly IRuleSetOrchestrationService<ILearner> _learneRuleSetOrchestrationService;
        private readonly IRuleSetOrchestrationService<ILearnerDestinationAndProgression> _learnerDPRuleSetOrchestrationService;

        public ValidationExecutionProvider(
            IRuleSetOrchestrationService<ILearner> learnerRuleSetOrchestrationService,
            IRuleSetOrchestrationService<ILearnerDestinationAndProgression> learnerDPRuleSetOrchestrationService,
            ILogger logger)
        {
            _learneRuleSetOrchestrationService = learnerRuleSetOrchestrationService;
            _learnerDPRuleSetOrchestrationService = learnerDPRuleSetOrchestrationService;
            _logger = logger;
        }

        public async Task ExecuteAsync(IValidationContext validationContext, IMessage message, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Starting Learner RuleSet Execution");
            await _learneRuleSetOrchestrationService.ExecuteAsync(validationContext, message?.Learners, cancellationToken).ConfigureAwait(false);
            _logger.LogDebug("Finished Learner RuleSet Execution");

            _logger.LogDebug("Starting LearnerDP RuleSet Execution");
            await _learnerDPRuleSetOrchestrationService.ExecuteAsync(validationContext, message?.LearnerDestinationAndProgressions, cancellationToken).ConfigureAwait(false);
            _logger.LogDebug("Finished LearnerDP RuleSet Execution");
        }
    }
}
