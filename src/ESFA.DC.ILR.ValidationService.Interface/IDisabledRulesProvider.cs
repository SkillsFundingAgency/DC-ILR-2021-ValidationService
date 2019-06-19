using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IDisabledRulesProvider
    {
        ICollection<string> Provide();
    }
}
