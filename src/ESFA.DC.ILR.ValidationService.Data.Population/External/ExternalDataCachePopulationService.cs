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
        private readonly IEpaOrgDataMapper _epaOrgDataMapper;
        private readonly IFcsDataMapper _fcsDataMapper;
        private readonly IOrganisationsDataMapper _organisationsDataMapper;
        private readonly IPostcodesDataMapper _postcodesDataMapper;
        private readonly IUlnDataMapper _ulnDataMapper;

        public ExternalDataCachePopulationService(
            IExternalDataCache externalDataCache,
            ICache<ReferenceDataRoot> referenceDataCache,
            ILARSStandardDataRetrievalService larsStandardDataRetrievalService,
            ILARSStandardValidityDataRetrievalService larsStandardValidityDataRetrievalService,
            ILARSLearningDeliveryDataRetrievalService larsLearningDeliveryDataRetrievalService,
            ILARSFrameworkDataRetrievalService larsFrameworkDataRetrievalService,
            IEmployersDataMapper employersDataMapper,
            IEpaOrgDataMapper epaOrgDataMapper,
            IFcsDataMapper fcsDataMapper,
            IOrganisationsDataMapper organisationsDataMapper,
            IPostcodesDataMapper postcodesDataMapper,
            IUlnDataMapper ulnDataMapper)
        {
            _externalDataCache = externalDataCache;
            _referenceDataCache = referenceDataCache;
            _larsStandardDataRetrievalService = larsStandardDataRetrievalService;
            _larsStandardValidityDataRetrievalService = larsStandardValidityDataRetrievalService;
            _larsLearningDeliveryDataRetrievalService = larsLearningDeliveryDataRetrievalService;
            _larsFrameworkDataRetrievalService = larsFrameworkDataRetrievalService;
            _employersDataMapper = employersDataMapper;
            _epaOrgDataMapper = epaOrgDataMapper;
            _fcsDataMapper = fcsDataMapper;
            _organisationsDataMapper = organisationsDataMapper;
            _postcodesDataMapper = postcodesDataMapper;
            _ulnDataMapper = ulnDataMapper;
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

            externalDataCache.Postcodes = _postcodesDataMapper.MapPostcodes(referenceDataCache.Postcodes);
            externalDataCache.ONSPostcodes = _postcodesDataMapper.MapONSPostcodes(referenceDataCache.Postcodes);

            externalDataCache.Organisations = _organisationsDataMapper.MapOrganisations(referenceDataCache.Organisations);
            externalDataCache.CampusIdentifiers = _organisationsDataMapper.MapCampusIdentifiers(referenceDataCache.Organisations);

            externalDataCache.EPAOrganisations = _epaOrgDataMapper.MapEpaOrganisations(referenceDataCache.EPAOrganisations);

            externalDataCache.FCSContractAllocations = _fcsDataMapper.MapFcsContractAllocations(referenceDataCache.FCSContractAllocations);

            externalDataCache.ERNs = _employersDataMapper.MapEmployers(referenceDataCache.Employers);
        }
    }
}
