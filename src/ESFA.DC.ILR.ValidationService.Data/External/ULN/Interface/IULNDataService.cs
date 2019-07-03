using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.External.ULN.Interface
{
    public interface IULNDataService : IDataService
    {
        bool Exists(long? uln);
    }
}
