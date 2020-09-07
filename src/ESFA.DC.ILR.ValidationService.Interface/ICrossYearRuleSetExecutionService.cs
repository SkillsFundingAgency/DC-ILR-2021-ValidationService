using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface ICrossYearRuleSetExecutionService<T>
        where T : class
    {
        void Execute(IEnumerable<ICrossYearRule<T>> ruleSet, T objectToValidate);
    }
}
