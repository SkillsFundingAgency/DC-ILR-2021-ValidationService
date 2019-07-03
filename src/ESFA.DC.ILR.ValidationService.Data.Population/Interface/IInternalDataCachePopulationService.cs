using ESFA.DC.ILR.ReferenceDataService.Model;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Interface
{
    public interface IInternalDataCachePopulationService
    {
        void Populate(ReferenceDataRoot referenceDataRoot);
    }
}
