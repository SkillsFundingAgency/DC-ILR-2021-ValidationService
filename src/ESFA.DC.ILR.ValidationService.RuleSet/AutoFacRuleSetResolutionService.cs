using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.RuleSet
{
    public class AutoFacRuleSetResolutionService<T> : IRuleSetResolutionService<T>
        where T : class
    {
        private readonly ILifetimeScope _lifetimeScope;
        private readonly IDisabledRulesProvider _disabledRulesProvider;

        public AutoFacRuleSetResolutionService(ILifetimeScope lifetimeScope, IDisabledRulesProvider disabledRulesProvider)
        {
            _lifetimeScope = lifetimeScope;
            _disabledRulesProvider = disabledRulesProvider;
        }

        public IEnumerable<IRule<T>> Resolve(IValidationContext validationContext)
        {
            var disabledRules = _disabledRulesProvider.Provide().Union(validationContext.IgnoredRules);

            return _lifetimeScope.Resolve<IEnumerable<IRule<T>>>().Where(x => !disabledRules.Any(y => string.Equals(x.RuleName, y, StringComparison.OrdinalIgnoreCase)));
        }
    }
}
