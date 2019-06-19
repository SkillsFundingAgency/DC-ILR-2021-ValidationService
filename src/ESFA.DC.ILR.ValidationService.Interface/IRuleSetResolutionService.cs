using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IRuleSetResolutionService<in T>
        where T : class
    {
        IEnumerable<IRule<T>> Resolve();
    }
}
