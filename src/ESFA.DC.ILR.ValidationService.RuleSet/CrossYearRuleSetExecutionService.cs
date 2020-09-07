using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.RuleSet
{
    public class CrossYearRuleSetExecutionService<T> : ICrossYearRuleSetExecutionService<T>
        where T : class
    {
        public void Execute(IEnumerable<ICrossYearRule<T>> ruleSet, T objectToValidate)
        {
            foreach (var rule in ruleSet)
            {
                rule.Validate(objectToValidate);
            }
        }
    }
}
