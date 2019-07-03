using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived.Interface
{
    public interface IDerivedData_14Rule : IDerivedDataRule
    {
        char InvalidLengthChecksum { get; }

        char GetWorkPlaceEmpIdChecksum(int workPlaceEmpId);
    }
}
