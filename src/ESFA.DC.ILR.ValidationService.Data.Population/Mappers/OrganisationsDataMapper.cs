using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Model;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Mappers
{
    public class OrganisationsDataMapper
    {
        public OrganisationsDataMapper()
        {
        }

        public IReadOnlyDictionary<long, Organisation> MapOrganisations(IReadOnlyCollection<ReferenceDataService.Model.Organisations.Organisation> organisations)
        {
            return organisations?
                .ToDictionary(
                u => (long)u.UKPRN,
                o => new Organisation
                {
                    UKPRN = o.UKPRN,
                    LegalOrgType = o.LegalOrgType,
                    PartnerUKPRN = o.PartnerUKPRN
                });
        }

        public IReadOnlyCollection<ICampusIdentifier> MapCampusIdentifiers(IReadOnlyCollection<ReferenceDataService.Model.Organisations.Organisation> organisations)
        {
            List<CampusIdentifier> campusIdentifiers = new List<CampusIdentifier>();

            foreach (var organisation in organisations.Where(o => o.CampusIdentifers != null))
            {
                campusIdentifiers.AddRange(organisation.CampusIdentifers.Select(campusId => new CampusIdentifier
                {
                    CampusIdentifer = campusId,
                    MasterUKPRN = organisation.UKPRN
                }));
            }

            return campusIdentifiers;
        }
    }
}
