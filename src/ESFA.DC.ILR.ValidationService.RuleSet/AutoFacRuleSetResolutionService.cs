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

        public AutoFacRuleSetResolutionService(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public IEnumerable<IRule<T>> Resolve(IValidationContext validationContext)
        {
            return _lifetimeScope.Resolve<IEnumerable<IRule<T>>>().Where(x => !validationContext.IgnoredRules.Any(y => string.Equals(x.RuleName, y, StringComparison.OrdinalIgnoreCase)));
        }
    }
}
