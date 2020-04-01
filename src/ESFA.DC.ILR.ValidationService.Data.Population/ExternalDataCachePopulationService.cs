using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ValidationService.Data.External;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population
{
    public class ExternalDataCachePopulationService : IExternalDataCachePopulationService
    {
        private readonly IExternalDataCache _externalDataCache;
        private readonly IEmployersDataMapper _employersDataMapper;
        private readonly IEpaOrgDataMapper _epaOrgDataMapper;
        private readonly IFcsDataMapper _fcsDataMapper;
        private readonly ILarsDataMapper _larsDataMapper;
        private readonly IOrganisationsDataMapper _organisationsDataMapper;
        private readonly IPostcodesDataMapper _postcodesDataMapper;
        private readonly IUlnDataMapper _ulnDataMapper;
        private readonly IValidationErrorsDataMapper _validationErrorsDataMapper;
        private readonly IValidationRulesDataMapper _validationRulesDataMapper;

        public ExternalDataCachePopulationService(
            IExternalDataCache externalDataCache,
            IEmployersDataMapper employersDataMapper,
            IEpaOrgDataMapper epaOrgDataMapper,
            IFcsDataMapper fcsDataMapper,
            ILarsDataMapper larsDataMapper,
            IOrganisationsDataMapper organisationsDataMapper,
            IPostcodesDataMapper postcodesDataMapper,
            IUlnDataMapper ulnDataMapper,
            IValidationErrorsDataMapper validationErrorsDataMapper,
            IValidationRulesDataMapper validationRulesDataMapper)
        {
            _externalDataCache = externalDataCache;
            _employersDataMapper = employersDataMapper;
            _epaOrgDataMapper = epaOrgDataMapper;
            _fcsDataMapper = fcsDataMapper;
            _larsDataMapper = larsDataMapper;
            _organisationsDataMapper = organisationsDataMapper;
            _postcodesDataMapper = postcodesDataMapper;
            _ulnDataMapper = ulnDataMapper;
            _validationErrorsDataMapper = validationErrorsDataMapper;
            _validationRulesDataMapper = validationRulesDataMapper;
        }

        public void Populate(ReferenceDataRoot referenceDataRoot, IValidationContext validationContext)
        {
            var externalDataCache = (ExternalDataCache)_externalDataCache;

            externalDataCache.Standards = _larsDataMapper.MapLarsStandards(referenceDataRoot.LARSStandards);
            externalDataCache.StandardValidities = _larsDataMapper.MapLarsStandardValidities(referenceDataRoot.LARSStandards);
            externalDataCache.LearningDeliveries = _larsDataMapper.MapLarsLearningDeliveries(referenceDataRoot.LARSLearningDeliveries);

            externalDataCache.ULNs = _ulnDataMapper.MapUlns(referenceDataRoot.ULNs);

            externalDataCache.Postcodes = _postcodesDataMapper.MapPostcodes(referenceDataRoot.Postcodes);
            externalDataCache.ONSPostcodes = _postcodesDataMapper.MapONSPostcodes(referenceDataRoot.Postcodes);
            externalDataCache.DevolvedPostcodes = _postcodesDataMapper.MapDevolvedPostcodes(referenceDataRoot.DevolvedPostcodes?.Postcodes);

            externalDataCache.Organisations = _organisationsDataMapper.MapOrganisations(referenceDataRoot.Organisations);
            externalDataCache.CampusIdentifiers = _organisationsDataMapper.MapCampusIdentifiers(referenceDataRoot.Organisations);

            externalDataCache.EPAOrganisations = _epaOrgDataMapper.MapEpaOrganisations(referenceDataRoot.EPAOrganisations);

            externalDataCache.FCSContractAllocations = _fcsDataMapper.MapFcsContractAllocations(referenceDataRoot.FCSContractAllocations);

            externalDataCache.ERNs = _employersDataMapper.MapEmployers(referenceDataRoot.Employers);

            externalDataCache.ValidationErrors = _validationErrorsDataMapper.MapValidationErrors(referenceDataRoot.MetaDatas?.ValidationErrors);
            externalDataCache.ValidationRules = _validationRulesDataMapper.MapValidationRules(referenceDataRoot.MetaDatas?.ValidationRules);

            externalDataCache.ReturnPeriod = validationContext.ReturnPeriod;
        }
    }
}
