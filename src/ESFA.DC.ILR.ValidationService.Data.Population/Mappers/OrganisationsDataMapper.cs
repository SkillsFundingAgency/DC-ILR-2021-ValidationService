using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Model;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Mappers
{
    public class OrganisationsDataMapper : IOrganisationsDataMapper
    {
        public IReadOnlyDictionary<long, Organisation> MapOrganisations(IReadOnlyCollection<ReferenceDataService.Model.Organisations.Organisation> organisations)
        {
            return organisations?
                .ToDictionary(
                u => (long)u.UKPRN,
                o => new Organisation
                {
                    UKPRN = o.UKPRN,
                    LegalOrgType = o.LegalOrgType,
                    PartnerUKPRN = o.PartnerUKPRN,
                    LongTermResid = o.LongTermResid
                });
        }

        public IReadOnlyCollection<ICampusIdentifier> MapCampusIdentifiers(IReadOnlyCollection<ReferenceDataService.Model.Organisations.Organisation> organisations)
        {
            List<CampusIdentifier> campusIdentifiers = new List<CampusIdentifier>();

            foreach (var organisation in organisations.Where(o => o.CampusIdentifers != null))
            {
                campusIdentifiers.AddRange(organisation.CampusIdentifers.Select(campusId => new CampusIdentifier
                {
                    CampusIdentifer = campusId.CampusIdentifier,
                    MasterUKPRN = organisation.UKPRN
                }));
            }

            return campusIdentifiers;
        }
    }
}
