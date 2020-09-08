using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.RuleSet
{
    public class RuleSetOrchestrationService<TRule, T> : IRuleSetOrchestrationService<TRule, T>
        where TRule : IAbstractRule<T>
        where T : class
    {
        private readonly IRuleSetResolutionService<TRule, T> _ruleSetResolutionService;
        private readonly IRuleSetExecutionService<TRule, T> _ruleSetExecutionService;
        private readonly IValidationErrorCache _validationErrorCache;

        public RuleSetOrchestrationService(
            IRuleSetResolutionService<TRule, T> ruleSetResolutionService,
            IRuleSetExecutionService<TRule, T> ruleSetExecutionService,
            IValidationErrorCache validationErrorCache)
        {
            _ruleSetResolutionService = ruleSetResolutionService;
            _ruleSetExecutionService = ruleSetExecutionService;
            _validationErrorCache = validationErrorCache;
        }

        public async Task<IEnumerable<IValidationError>> ExecuteAsync(IEnumerable<T> validationItems, CancellationToken cancellationToken)
        {
            List<TRule> ruleSet = _ruleSetResolutionService.Resolve().ToList();

            foreach (T validationItem in validationItems)
            {
                cancellationToken.ThrowIfCancellationRequested();

                _ruleSetExecutionService.Execute(ruleSet, validationItem);
            }

            return _validationErrorCache.ValidationErrors;
        }

        public async Task<IEnumerable<IValidationError>> ExecuteAsync(T validationItem, CancellationToken cancellationToken)
        {
            List<TRule> ruleSet = _ruleSetResolutionService.Resolve().ToList();

            cancellationToken.ThrowIfCancellationRequested();

            _ruleSetExecutionService.Execute(ruleSet, validationItem);

            return _validationErrorCache.ValidationErrors;
        }
    }
}
