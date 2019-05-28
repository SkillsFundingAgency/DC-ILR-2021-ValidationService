using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IRuleSetExecutionService<T>
        where T : class
    {
        void Execute(IEnumerable<IRule<T>> ruleSet, T objectToValidate);
    }
}
