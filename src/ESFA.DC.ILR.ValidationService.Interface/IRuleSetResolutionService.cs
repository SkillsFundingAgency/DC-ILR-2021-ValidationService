using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IRuleSetResolutionService<TRule, T>
        where TRule : IValidationRule<T>
        where T : class
    {
        IEnumerable<TRule> Resolve();
    }
}
