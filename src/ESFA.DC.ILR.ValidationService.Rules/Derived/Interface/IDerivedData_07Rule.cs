using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived.Interface
{
    public interface IDerivedData_07Rule : IDerivedDataRule
    {
        bool IsApprenticeship(int? progType);
    }
}
