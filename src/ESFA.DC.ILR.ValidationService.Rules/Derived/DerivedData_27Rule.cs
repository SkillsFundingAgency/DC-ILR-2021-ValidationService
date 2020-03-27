using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    public class DerivedData_27Rule : IDerivedData_27Rule
    {
        private readonly IEnumerable<string> _legalOrgTypes = new HashSet<string>()
        {
            LegalOrgTypeConstants.USFC,
            LegalOrgTypeConstants.USDC,
            LegalOrgTypeConstants.USAH,
            LegalOrgTypeConstants.USAD,
            LegalOrgTypeConstants.UGFE,
            LegalOrgTypeConstants.UHEO,
            LegalOrgTypeConstants.ULEA,
            LegalOrgTypeConstants.ULAD,
            LegalOrgTypeConstants.USCL,
            LegalOrgTypeConstants.LAFB,
            LegalOrgTypeConstants.UFES,
            LegalOrgTypeConstants.SSPS
        };

        private readonly IOrganisationDataService _organisationDataService;

        public DerivedData_27Rule(IOrganisationDataService organisationDataService)
        {
            _organisationDataService = organisationDataService;
        }

        public bool IsUKPRNCollegeOrGrantFundedProvider(int ukprn) => _legalOrgTypes.Contains(_organisationDataService.GetLegalOrgTypeForUkprn(ukprn));
    }
}
