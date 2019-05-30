using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived.Interface
{
    public interface IDerivedData_01Rule : IDerivedDataRule
    {
        string Derive(long uln);
    }
}
