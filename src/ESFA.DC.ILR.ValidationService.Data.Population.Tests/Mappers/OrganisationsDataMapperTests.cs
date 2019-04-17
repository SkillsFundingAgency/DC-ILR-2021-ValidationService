using System;
using System.Collections.Generic;
using ESFA.DC.ILR.ReferenceDataService.Model.Postcodes;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Model;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes;
using ESFA.DC.ILR.ValidationService.Data.Population.Mappers;
using FluentAssertions;
using Xunit;

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
                    }
                },
                {
                    2, new Organisation
                    {
                        UKPRN = 2,
                        PartnerUKPRN = false,
                        LegalOrgType = "LegalOrgType",
                    }
                }
            };

            NewMapper().MapOrganisations(organisations).Should().BeEquivalentTo(expectedOrganisations);
        }

        [Fact]
        public void MapONSPostcodes()
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
                    OrganisationFundings = new List<ReferenceDataService.Model.Organisations.OrganisationFunding>(),
                    CampusIdentifers = new List<string>
                    {
                        "Id1",
                        "Id2"
                    }
                },
                new ReferenceDataService.Model.Organisations.Organisation
                {
                    UKPRN = 2,
                    PartnerUKPRN = false,
                    LegalOrgType = "LegalOrgType",
                    OrganisationFundings = new List<ReferenceDataService.Model.Organisations.OrganisationFunding>(),
                    CampusIdentifers = new List<string>
                    {
                        "Id1",
                        "Id2"
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
