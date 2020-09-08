using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.RuleSet
{
    public class RuleSetExecutionService<TRule, T> : IRuleSetExecutionService<TRule, T>
        where TRule : IValidationRule<T>
        where T : class
    {
        public void Execute(IEnumerable<TRule> ruleSet, T objectToValidate)
        {
            foreach (var rule in ruleSet)
            {
                rule.Validate(objectToValidate);
            }
        }
    }
}
