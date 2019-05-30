using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.RuleSet
{
    public class RuleSetOrchestrationService<T, U> : IRuleSetOrchestrationService<T, U>
        where T : class
    {
        private readonly IRuleSetResolutionService<T> _ruleSetResolutionService;
        private readonly IRuleSetExecutionService<T> _ruleSetExecutionService;
        private readonly IValidationErrorCache<U> _validationErrorCache;

        public RuleSetOrchestrationService(
            IRuleSetResolutionService<T> ruleSetResolutionService,
            IRuleSetExecutionService<T> ruleSetExecutionService,
            IValidationErrorCache<U> validationErrorCache)
        {
            _ruleSetResolutionService = ruleSetResolutionService;
            _ruleSetExecutionService = ruleSetExecutionService;
            _validationErrorCache = validationErrorCache;
        }

        public async Task<IEnumerable<U>> ExecuteAsync(IValidationContext validationContext, IEnumerable<T> validationItems, CancellationToken cancellationToken)
        {
            List<IRule<T>> ruleSet = _ruleSetResolutionService.Resolve(validationContext).ToList();

            foreach (T validationItem in validationItems)
            {
                cancellationToken.ThrowIfCancellationRequested();

                _ruleSetExecutionService.Execute(ruleSet, validationItem);
            }

            return _validationErrorCache.ValidationErrors;
        }

        public async Task<IEnumerable<U>> ExecuteAsync(IValidationContext validationContext, T validationItem, CancellationToken cancellationToken)
        {
            List<IRule<T>> ruleSet = _ruleSetResolutionService.Resolve(validationContext).ToList();

            cancellationToken.ThrowIfCancellationRequested();

            _ruleSetExecutionService.Execute(ruleSet, validationItem);

            return _validationErrorCache.ValidationErrors;
        }
    }
}
