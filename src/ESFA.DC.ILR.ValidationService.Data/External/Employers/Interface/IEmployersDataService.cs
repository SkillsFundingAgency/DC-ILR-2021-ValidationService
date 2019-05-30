using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.External.EDRS.Interface
{
    public interface IEmployersDataService : IDataService
    {
        bool IsValid(int? empId);
    }
}
