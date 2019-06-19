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
        private readonly IEnabledRulesProvider _enabledRulesProvider;

        public AutoFacRuleSetResolutionService(ILifetimeScope lifetimeScope, IEnabledRulesProvider enabledRulesProvider)
        {
            _lifetimeScope = lifetimeScope;
            _enabledRulesProvider = enabledRulesProvider;
        }

        public IEnumerable<IRule<T>> Resolve()
        {
            return _lifetimeScope.Resolve<IEnumerable<IRule<T>>>().Where(x => _enabledRulesProvider.Provide().Any(y => string.Equals(x.RuleName, y, StringComparison.OrdinalIgnoreCase)));
        }
    }
}
