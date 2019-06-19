using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.RuleSet
{
    public class RuleSetOrchestrationService<T> : IRuleSetOrchestrationService<T>
        where T : class
    {
        private readonly IRuleSetResolutionService<T> _ruleSetResolutionService;
        private readonly IRuleSetExecutionService<T> _ruleSetExecutionService;
        private readonly IValidationErrorCache _validationErrorCache;

        public RuleSetOrchestrationService(
            IRuleSetResolutionService<T> ruleSetResolutionService,
            IRuleSetExecutionService<T> ruleSetExecutionService,
            IValidationErrorCache validationErrorCache)
        {
            _ruleSetResolutionService = ruleSetResolutionService;
            _ruleSetExecutionService = ruleSetExecutionService;
            _validationErrorCache = validationErrorCache;
        }

        public async Task<IEnumerable<IValidationError>> ExecuteAsync(IEnumerable<T> validationItems, CancellationToken cancellationToken)
        {
            List<IRule<T>> ruleSet = _ruleSetResolutionService.Resolve().ToList();

            foreach (T validationItem in validationItems)
            {
                cancellationToken.ThrowIfCancellationRequested();

                _ruleSetExecutionService.Execute(ruleSet, validationItem);
            }

            return _validationErrorCache.ValidationErrors;
        }

        public async Task<IEnumerable<IValidationError>> ExecuteAsync(T validationItem, CancellationToken cancellationToken)
        {
            List<IRule<T>> ruleSet = _ruleSetResolutionService.Resolve().ToList();

            cancellationToken.ThrowIfCancellationRequested();

            _ruleSetExecutionService.Execute(ruleSet, validationItem);

            return _validationErrorCache.ValidationErrors;
        }
    }
}
