using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IRuleSetExecutionService<TRule, T>
        where TRule : IAbstractRule<T>
        where T : class
    {
        void Execute(IEnumerable<TRule> ruleSet, T objectToValidate);
    }
}
