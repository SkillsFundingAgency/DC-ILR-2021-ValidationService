using System.Collections.Generic;
using ESFA.DC.ILR.ReferenceDataService.Model.Organisations;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Model;
using ESFA.DC.ILR.ValidationService.Data.Population.Mappers;
using FluentAssertions;
using Xunit;
using Organisation = ESFA.DC.ILR.ValidationService.Data.External.Organisation.Model.Organisation;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Tests.Mappers
{
    public class OrganisationsDataMapperTests
    {
        [Fact]
        public void MapOrganisations()
        {
            var organisations = TestOrganisations();

            var expectedOrganisations = new Dictionary<long, Organisation>
            {
                {
                    1, new Organisation
                    {
                        UKPRN = 1,
                        PartnerUKPRN = true,
                        LegalOrgType = "LegalOrgType",
                        LongTermResid = true
                    }
                },
                {
                    2, new Organisation
                    {
                        UKPRN = 2,
                        PartnerUKPRN = false,
                        LegalOrgType = "LegalOrgType",
                        LongTermResid = false
                    }
                }
            };

            NewMapper().MapOrganisations(organisations).Should().BeEquivalentTo(expectedOrganisations);
        }

        [Fact]
        public void MapCampusIdentifiers()
        {
            var organisations = TestOrganisations();

            var expectedCampusIdentifiers = new List<CampusIdentifier>
            {
                new CampusIdentifier { MasterUKPRN = 1, CampusIdentifer = "Id1" },
                new CampusIdentifier { MasterUKPRN = 1, CampusIdentifer = "Id2" },
                new CampusIdentifier { MasterUKPRN = 2, CampusIdentifer = "Id1" },
                new CampusIdentifier { MasterUKPRN = 2, CampusIdentifer = "Id2" }
            };

            NewMapper().MapCampusIdentifiers(organisations).Should().BeEquivalentTo(expectedCampusIdentifiers);
        }

        private IReadOnlyCollection<ReferenceDataService.Model.Organisations.Organisation> TestOrganisations()
        {
            return new List<ReferenceDataService.Model.Organisations.Organisation>
            {
                new ReferenceDataService.Model.Organisations.Organisation
                {
                    UKPRN = 1,
                    PartnerUKPRN = true,
                    LegalOrgType = "LegalOrgType",
                    LongTermResid = true,
                    OrganisationFundings = new List<OrganisationFunding>(),
                    CampusIdentifers = new List<OrganisationCampusIdentifier>
                    {
                        new OrganisationCampusIdentifier
                        {
                            CampusIdentifier = "Id1",
                            EffectiveFrom = new System.DateTime(2019, 8, 1)
                        },
                        new OrganisationCampusIdentifier
                        {
                            CampusIdentifier = "Id2",
                            EffectiveFrom = new System.DateTime(2019, 8, 1)
                        }
                    }
                },
                new ReferenceDataService.Model.Organisations.Organisation
                {
                    UKPRN = 2,
                    PartnerUKPRN = false,
                    LegalOrgType = "LegalOrgType",
                    LongTermResid = false,
                    OrganisationFundings = new List<OrganisationFunding>(),
                    CampusIdentifers = new List<OrganisationCampusIdentifier>
                    {
                        new OrganisationCampusIdentifier
                        {
                            CampusIdentifier = "Id1",
                            EffectiveFrom = new System.DateTime(2019, 8, 1)
                        },
                        new OrganisationCampusIdentifier
                        {
                            CampusIdentifier = "Id2",
                            EffectiveFrom = new System.DateTime(2019, 8, 1)
                        }
                    }
                }
            };
        }

        private OrganisationsDataMapper NewMapper()
        {
            return new OrganisationsDataMapper();
        }
    }
}
