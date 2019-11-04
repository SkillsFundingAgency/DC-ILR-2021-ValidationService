using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Data.External.Organisation
{
    public class OrganisationDataService : IOrganisationDataService
    {
        private readonly IExternalDataCache _referenceDataCache;

        public OrganisationDataService(IExternalDataCache referenceDataCache)
        {
            _referenceDataCache = referenceDataCache;
        }

        public bool LegalOrgTypeMatchForUkprn(long ukprn, string legalOrgType)
        {
            return legalOrgType == GetLegalOrgTypeForUkprn(ukprn);
        }

        public bool UkprnExists(long ukprn)
        {
            return _referenceDataCache.Organisations.ContainsKey(ukprn);
        }

        public bool IsPartnerUkprn(long ukprn)
        {
            _referenceDataCache.Organisations.TryGetValue(ukprn, out var organisation);

            return organisation != null && organisation.PartnerUKPRN == true;
        }

        public string GetLegalOrgTypeForUkprn(long ukprn)
        {
            _referenceDataCache.Organisations.TryGetValue(ukprn, out var organisation);

            return organisation?.LegalOrgType;
        }

        public bool CampIdExists(string campId)
        {
           return _referenceDataCache.CampusIdentifiers.Any(ci => ci.CampusIdentifer.CaseInsensitiveEquals(campId));
        }

        public bool CampIdMatchForUkprn(string campId, long ukprn)
        {
            return _referenceDataCache.CampusIdentifiers.Any(ci => ci.CampusIdentifer.CaseInsensitiveEquals(campId) && ci.MasterUKPRN == ukprn);
        }

        public bool IsLongTermResForUkprn(long ukprn)
        {
            _referenceDataCache.Organisations.TryGetValue(ukprn, out var organisation);

            return organisation.LongTermResid ?? false;
        }
    }
}
