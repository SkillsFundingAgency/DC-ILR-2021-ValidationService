using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Interface
{
    public interface IExternalDataCachePopulationService
    {
        void Populate(ReferenceDataRoot referenceDataRoot, IValidationContext validationContext);
    }
}