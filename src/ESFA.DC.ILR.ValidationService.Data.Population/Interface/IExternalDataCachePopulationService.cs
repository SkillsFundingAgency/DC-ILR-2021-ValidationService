using ESFA.DC.ILR.ReferenceDataService.Model;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Interface
{
    public interface IExternalDataCachePopulationService
    {
        void Populate(ReferenceDataRoot referenceDataRoot);
    }
}