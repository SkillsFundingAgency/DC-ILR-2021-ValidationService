using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population
{
    public class PreValidationPopulationService : IPopulationService
    {
        private readonly IMessageCachePopulationService _messageCachePopulationService;
        private readonly IReferenceDataCachePopulationService _referenceDataCachePopulationService;
        private readonly IFileDataCachePopulationService _fileDataCachePopulationService;
        private readonly IInternalDataCachePopulationService _internalDataCachePopulationService;
        private readonly IExternalDataCachePopulationService _externalDataCachePopulationService;

        public PreValidationPopulationService(
            IMessageCachePopulationService messageCachePopulationService,
            IReferenceDataCachePopulationService referenceDataCachePopulationService,
            IFileDataCachePopulationService fileDataCachePopulationService,
            IInternalDataCachePopulationService internalDataCachePopulationService,
            IExternalDataCachePopulationService externalDataCachePopulationService)
        {
            _messageCachePopulationService = messageCachePopulationService;
            _referenceDataCachePopulationService = referenceDataCachePopulationService;
            _fileDataCachePopulationService = fileDataCachePopulationService;
            _internalDataCachePopulationService = internalDataCachePopulationService;
            _externalDataCachePopulationService = externalDataCachePopulationService;
        }

        public async Task PopulateAsync(IValidationContext validationContext, CancellationToken cancellationToken)
        {
            await _messageCachePopulationService.PopulateAsync(validationContext, cancellationToken);
            await _referenceDataCachePopulationService.PopulateAsync(validationContext, cancellationToken);

            await Task.WhenAll(
                _fileDataCachePopulationService.PopulateAsync(validationContext, cancellationToken),
                _internalDataCachePopulationService.PopulateAsync(validationContext, cancellationToken),
                _externalDataCachePopulationService.PopulateAsync(validationContext, cancellationToken));
        }
    }
}
