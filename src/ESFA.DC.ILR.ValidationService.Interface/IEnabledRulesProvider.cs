using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IEnabledRulesProvider
    {
        ICollection<string> Provide();
    }
}
