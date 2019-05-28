using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ValidationService.Data.Cache;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population
{
    public class ReferenceDataCachePopulationService : IReferenceDataCachePopulationService
    {
        private readonly ICache<ReferenceDataRoot> _referenceDataCache;
        private readonly IValidationItemProviderService<ReferenceDataRoot> _validationItemProviderService;

        public ReferenceDataCachePopulationService(
            ICache<ReferenceDataRoot> referenceDataCache,
            IValidationItemProviderService<ReferenceDataRoot> validationItemProviderService)
        {
            _referenceDataCache = referenceDataCache;
            _validationItemProviderService = validationItemProviderService;
        }

        public async Task PopulateAsync(IValidationContext validationContext, CancellationToken cancellationToken)
        {
            var messageCache = (Cache<ReferenceDataRoot>)_referenceDataCache;
            messageCache.Item = await _validationItemProviderService.ProvideAsync(validationContext, cancellationToken);
        }
    }
}
