using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface ICrossYearRuleSetResolutionService<in T>
        where T : class
    {
        IEnumerable<ICrossYearRule<T>> Resolve();
    }
}
