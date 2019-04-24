using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ValidationService.Data.External;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population.External
{
    public class ExternalDataCachePopulationService : IExternalDataCachePopulationService
    {
        private readonly IExternalDataCache _externalDataCache;
        private readonly ICache<ReferenceDataRoot> _referenceDataCache;
        private readonly IEmployersDataMapper _employersDataMapper;
        private readonly IEpaOrgDataMapper _epaOrgDataMapper;
        private readonly IFcsDataMapper _fcsDataMapper;
        private readonly ILarsDataMapper _larsDataMapper;
        private readonly IOrganisationsDataMapper _organisationsDataMapper;
        private readonly IPostcodesDataMapper _postcodesDataMapper;
        private readonly IUlnDataMapper _ulnDataMapper;
        private readonly IValidationErrorsDataMapper _validationErrorsDataMapper;

        public ExternalDataCachePopulationService(
            IExternalDataCache externalDataCache,
            ICache<ReferenceDataRoot> referenceDataCache,
            IEmployersDataMapper employersDataMapper,
            IEpaOrgDataMapper epaOrgDataMapper,
            IFcsDataMapper fcsDataMapper,
            ILarsDataMapper larsDataMapper,
            IOrganisationsDataMapper organisationsDataMapper,
            IPostcodesDataMapper postcodesDataMapper,
            IUlnDataMapper ulnDataMapper,
            IValidationErrorsDataMapper validationErrorsDataMapper)
        {
            _externalDataCache = externalDataCache;
            _referenceDataCache = referenceDataCache;
            _employersDataMapper = employersDataMapper;
            _epaOrgDataMapper = epaOrgDataMapper;
            _fcsDataMapper = fcsDataMapper;
            _larsDataMapper = larsDataMapper;
            _organisationsDataMapper = organisationsDataMapper;
            _postcodesDataMapper = postcodesDataMapper;
            _ulnDataMapper = ulnDataMapper;
            _validationErrorsDataMapper = validationErrorsDataMapper;
        }

        public async Task PopulateAsync(CancellationToken cancellationToken)
        {
            var externalDataCache = (ExternalDataCache)_externalDataCache;
            var referenceDataCache = _referenceDataCache.Item;

            externalDataCache.Standards = _larsDataMapper.MapLarsStandards(referenceDataCache.LARSStandards);
            externalDataCache.StandardValidities = _larsDataMapper.MapLarsStandardValidities(referenceDataCache.LARSStandards);
            externalDataCache.LearningDeliveries = _larsDataMapper.MapLarsLearningDeliveries(referenceDataCache.LARSLearningDeliveries);

            externalDataCache.ULNs = _ulnDataMapper.MapUlns(referenceDataCache.ULNs);

            externalDataCache.Postcodes = _postcodesDataMapper.MapPostcodes(referenceDataCache.Postcodes);
            externalDataCache.ONSPostcodes = _postcodesDataMapper.MapONSPostcodes(referenceDataCache.Postcodes);

            externalDataCache.Organisations = _organisationsDataMapper.MapOrganisations(referenceDataCache.Organisations);
            externalDataCache.CampusIdentifiers = _organisationsDataMapper.MapCampusIdentifiers(referenceDataCache.Organisations);

            externalDataCache.EPAOrganisations = _epaOrgDataMapper.MapEpaOrganisations(referenceDataCache.EPAOrganisations);

            externalDataCache.FCSContractAllocations = _fcsDataMapper.MapFcsContractAllocations(referenceDataCache.FCSContractAllocations);

            externalDataCache.ERNs = _employersDataMapper.MapEmployers(referenceDataCache.Employers);

            externalDataCache.ValidationErrors = _validationErrorsDataMapper.MapValidationErrors(referenceDataCache.MetaDatas?.ValidationErrors);
        }
    }
}
