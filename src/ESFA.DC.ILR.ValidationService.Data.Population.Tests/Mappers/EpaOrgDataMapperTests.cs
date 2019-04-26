using System;
using System.Collections.Generic;
using ESFA.DC.ILR.ReferenceDataService.Model.EPAOrganisations;
using ESFA.DC.ILR.ValidationService.Data.External.EPAOrganisation.Model;
using ESFA.DC.ILR.ValidationService.Data.Population.Mappers;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Tests.Mappers
{
    public class EpaOrgDataMapperTests
    {
        [Fact]
        public void MapEpaOrganisations()
        {
            var epaOrganisations = TestEpaOrganisations();

            var expectedEpaOrganisations = new Dictionary<string, List<EPAOrganisations>>
            {
                {
                    "Epa1", new List<EPAOrganisations> { new EPAOrganisations { ID = "Epa1", Standard = "Standard1", EffectiveFrom = new DateTime(2018, 8, 1) } }
                },
                {
                    "Epa2", new List<EPAOrganisations> { new EPAOrganisations { ID = "Epa2", Standard = "Standard2", EffectiveFrom = new DateTime(2018, 8, 1) } }
                },
                {
                    "Epa3", new List<EPAOrganisations>
                    {
                        new EPAOrganisations { ID = "Epa3", Standard = "Standard3", EffectiveFrom = new DateTime(2018, 8, 1), EffectiveTo = new DateTime(2018, 8, 31) },
                        new EPAOrganisations { ID = "Epa3", Standard = "Standard30", EffectiveFrom = new DateTime(2018, 9, 1) }
                    }
                },
                {
                    "Epa4", new List<EPAOrganisations>
                    {
                        new EPAOrganisations { ID = "Epa4", Standard = "Standard4", EffectiveFrom = new DateTime(2018, 8, 1), EffectiveTo = new DateTime(2018, 8, 31) },
                        new EPAOrganisations { ID = "Epa4", Standard = "Standard40", EffectiveFrom = new DateTime(2018, 9, 1) }
                    }
                }
            };

            NewMapper().MapEpaOrganisations(epaOrganisations).Should().BeEquivalentTo(expectedEpaOrganisations);
        }

        private IReadOnlyCollection<EPAOrganisation> TestEpaOrganisations()
        {
            return new List<EPAOrganisation>
            {
                new EPAOrganisation { ID = "Epa1", Standard = "Standard1", EffectiveFrom = new DateTime(2018, 8, 1) },
                new EPAOrganisation { ID = "Epa2", Standard = "Standard2", EffectiveFrom = new DateTime(2018, 8, 1) },
                new EPAOrganisation { ID = "Epa3", Standard = "Standard3", EffectiveFrom = new DateTime(2018, 8, 1), EffectiveTo = new DateTime(2018, 8, 31) },
                new EPAOrganisation { ID = "Epa3", Standard = "Standard30", EffectiveFrom = new DateTime(2018, 9, 1) },
                new EPAOrganisation { ID = "Epa4", Standard = "Standard4", EffectiveFrom = new DateTime(2018, 8, 1), EffectiveTo = new DateTime(2018, 8, 31) },
                new EPAOrganisation { ID = "Epa4", Standard = "Standard40", EffectiveFrom = new DateTime(2018, 9, 1) }
            };
        }

        private EpaOrgDataMapper NewMapper()
        {
            return new EpaOrgDataMapper();
        }
    }
}
