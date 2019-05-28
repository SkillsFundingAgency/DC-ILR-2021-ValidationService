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
        private readonly IValidationItemProviderService<IEnumerable<T>> _validationItemProviderService;
        private readonly IRuleSetExecutionService<T> _ruleSetExecutionService;
        private readonly IValidationErrorCache<U> _validationErrorCache;

        public RuleSetOrchestrationService(
            IRuleSetResolutionService<T> ruleSetResolutionService,
            IValidationItemProviderService<IEnumerable<T>> validationItemProviderService,
            IRuleSetExecutionService<T> ruleSetExecutionService,
            IValidationErrorCache<U> validationErrorCache)
        {
            _ruleSetResolutionService = ruleSetResolutionService;
            _validationItemProviderService = validationItemProviderService;
            _ruleSetExecutionService = ruleSetExecutionService;
            _validationErrorCache = validationErrorCache;
        }

        public async Task<IEnumerable<U>> ExecuteAsync(IValidationContext validationContext, CancellationToken cancellationToken)
        {
            List<IRule<T>> ruleSet = _ruleSetResolutionService.Resolve(validationContext).ToList();

            IEnumerable<T> items = await _validationItemProviderService.ProvideAsync(validationContext, cancellationToken);
            foreach (T validationItem in items)
            {
                cancellationToken.ThrowIfCancellationRequested();

                _ruleSetExecutionService.Execute(ruleSet, validationItem);
            }

            return _validationErrorCache.ValidationErrors;
        }
    }
}
