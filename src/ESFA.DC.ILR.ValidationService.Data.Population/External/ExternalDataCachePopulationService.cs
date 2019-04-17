using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ValidationService.Data.Population.External
{
    public class ExternalDataCachePopulationService : IExternalDataCachePopulationService
    {
        private readonly IExternalDataCache _externalDataCache;
        private readonly ICache<ReferenceDataRoot> _referenceDataCache;
        private readonly ILARSStandardDataRetrievalService _larsStandardDataRetrievalService;
        private readonly ILARSStandardValidityDataRetrievalService _larsStandardValidityDataRetrievalService;
        private readonly ILARSLearningDeliveryDataRetrievalService _larsLearningDeliveryDataRetrievalService;
        private readonly ILARSFrameworkDataRetrievalService _larsFrameworkDataRetrievalService;
        private readonly IEmployersDataMapper _employersDataMapper;
        private readonly IUlnDataMapper _ulnDataMapper;
        private readonly IPostcodesDataRetrievalService _postcodesDataRetrievalService;
        private readonly IOrganisationsDataRetrievalService _organisationsDataRetrievalService;
        private readonly IEPAOrganisationsDataRetrievalService _epaOrganisationsDataRetrievalService;
        private readonly ICampusIdentifierDataRetrievalService _campusIdentifierDataRetrievalService;
        private readonly IFCSDataRetrievalService _fcsDataRetrievalService;

        public ExternalDataCachePopulationService(
            IExternalDataCache externalDataCache,
            ICache<ReferenceDataRoot> referenceDataCache,
            ILARSStandardDataRetrievalService larsStandardDataRetrievalService,
            ILARSStandardValidityDataRetrievalService larsStandardValidityDataRetrievalService,
            ILARSLearningDeliveryDataRetrievalService larsLearningDeliveryDataRetrievalService,
            ILARSFrameworkDataRetrievalService larsFrameworkDataRetrievalService,
            IEmployersDataMapper employersDataMapper,
            IUlnDataMapper ulnDataMapper,
            IPostcodesDataRetrievalService postcodesDataRetrievalService,
            IOrganisationsDataRetrievalService organisationsDataRetrievalService,
            IEPAOrganisationsDataRetrievalService epaOrganisationsDataRetrievalService,
            ICampusIdentifierDataRetrievalService campusIdentifierDataRetrievalService,
            IFCSDataRetrievalService fcsDataRetrievalService)
        {
            _externalDataCache = externalDataCache;
            _referenceDataCache = referenceDataCache;
            _larsStandardDataRetrievalService = larsStandardDataRetrievalService;
            _larsStandardValidityDataRetrievalService = larsStandardValidityDataRetrievalService;
            _larsLearningDeliveryDataRetrievalService = larsLearningDeliveryDataRetrievalService;
            _larsFrameworkDataRetrievalService = larsFrameworkDataRetrievalService;
            _employersDataMapper = employersDataMapper;
            _ulnDataMapper = ulnDataMapper;
            _postcodesDataRetrievalService = postcodesDataRetrievalService;
            _organisationsDataRetrievalService = organisationsDataRetrievalService;
            _epaOrganisationsDataRetrievalService = epaOrganisationsDataRetrievalService;
            _campusIdentifierDataRetrievalService = campusIdentifierDataRetrievalService;
            _fcsDataRetrievalService = fcsDataRetrievalService;
        }

        public async Task PopulateAsync(CancellationToken cancellationToken)
        {
            var externalDataCache = (ExternalDataCache)_externalDataCache;
            var referenceDataCache = _referenceDataCache.Item;

            externalDataCache.Standards = await _larsStandardDataRetrievalService.RetrieveAsync(cancellationToken);
            externalDataCache.StandardValidities = await _larsStandardValidityDataRetrievalService.RetrieveAsync(cancellationToken);
            externalDataCache.LearningDeliveries = await _larsLearningDeliveryDataRetrievalService.RetrieveAsync(cancellationToken);
            externalDataCache.Frameworks = await _larsFrameworkDataRetrievalService.RetrieveAsync(cancellationToken);

            externalDataCache.ULNs = _ulnDataMapper.MapUlns(referenceDataCache.ULNs);

            externalDataCache.Postcodes = (await _postcodesDataRetrievalService.RetrieveAsync(cancellationToken)).ToCaseInsensitiveHashSet();
            externalDataCache.ONSPostcodes = await _postcodesDataRetrievalService.RetrieveONSPostcodesAsync(cancellationToken);

            externalDataCache.Organisations = await _organisationsDataRetrievalService.RetrieveAsync(cancellationToken);
            externalDataCache.CampusIdentifiers = await _campusIdentifierDataRetrievalService.RetrieveAsync(cancellationToken);

            externalDataCache.EPAOrganisations = await _epaOrganisationsDataRetrievalService.RetrieveAsync(cancellationToken);

            externalDataCache.FCSContractAllocations = await _fcsDataRetrievalService.RetrieveAsync(cancellationToken);

            externalDataCache.ERNs = _employersDataMapper.MapEmployers(referenceDataCache.Employers);
        }
    }
}
