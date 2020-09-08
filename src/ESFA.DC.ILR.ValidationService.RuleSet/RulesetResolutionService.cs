using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.RuleSet
{
    public class RuleSetResolutionService<TRule, T> : IRuleSetResolutionService<TRule, T>
        where TRule : IAbstractRule<T> 
        where T : class
    {
        private readonly ILifetimeScope _lifetimeScope;
        private readonly IEnabledRulesProvider _enabledRulesProvider;

        public RuleSetResolutionService(ILifetimeScope lifetimeScope, IEnabledRulesProvider enabledRulesProvider)
        {
            _lifetimeScope = lifetimeScope;
            _enabledRulesProvider = enabledRulesProvider;
        }

        public IEnumerable<TRule> Resolve()
        {
            return _lifetimeScope.Resolve<IEnumerable<TRule>>().Where(x => _enabledRulesProvider.Provide().Any(y => string.Equals(x.RuleName, y, StringComparison.OrdinalIgnoreCase)));
        }
    }
}
