using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population
{
    public class PreValidationPopulationService : IPopulationService
    {
        private readonly IMessageCachePopulationService _messageCachePopulationService;
        private readonly IFileDataCachePopulationService _fileDataCachePopulationService;
        private readonly IInternalDataCachePopulationService _internalDataCachePopulationService;
        private readonly IExternalDataCachePopulationService _externalDataCachePopulationService;

        public PreValidationPopulationService(
            IMessageCachePopulationService messageCachePopulationService,
            IFileDataCachePopulationService fileDataCachePopulationService,
            IInternalDataCachePopulationService internalDataCachePopulationService,
            IExternalDataCachePopulationService externalDataCachePopulationService)
        {
            _messageCachePopulationService = messageCachePopulationService;
            _fileDataCachePopulationService = fileDataCachePopulationService;
            _internalDataCachePopulationService = internalDataCachePopulationService;
            _externalDataCachePopulationService = externalDataCachePopulationService;
        }

        public void Populate(IValidationContext validationContext, IMessage message, ReferenceDataRoot referenceDataRoot)
        {
            _messageCachePopulationService.Populate(message);
            _internalDataCachePopulationService.Populate(referenceDataRoot);
            _fileDataCachePopulationService.Populate(validationContext, message);
            _externalDataCachePopulationService.Populate(referenceDataRoot, validationContext);
        }
    }
}
