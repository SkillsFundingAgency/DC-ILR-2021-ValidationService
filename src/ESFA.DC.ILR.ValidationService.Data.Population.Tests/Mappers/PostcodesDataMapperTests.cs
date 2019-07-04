using System;
using System.Collections.Generic;
using ESFA.DC.ILR.ReferenceDataService.Model.Postcodes;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes;
using ESFA.DC.ILR.ValidationService.Data.Population.Mappers;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Tests.Mappers
{
    public class PostcodesDataMapperTests
    {
        [Fact]
        public void MapPostcodes()
        {
            var postcodes = TestPostcodes();

            var expectedPostcodes = new List<string>
            {
                "Postcode1",
                "Postcode2",
                "Postcode3",
                "Postcode4",
            };

            NewMapper().MapPostcodes(postcodes).Should().BeEquivalentTo(expectedPostcodes);
        }

        [Fact]
        public void MapONSPostcodes()
        {
            var postcodes = TestPostcodes();

            var expectedONSPostcodes = new List<ONSPostcode>
            {
                new ONSPostcode
                {
                    Postcode = "Postcode1",
                    EffectiveFrom = new DateTime(2018, 8, 1),
                    Lep1 = "Lep1",
                    Lep2 = "Lep2",
                    LocalAuthority = "LocalAuthority",
                    Nuts = "Nuts"
                },
                new ONSPostcode
                {
                    Postcode = "Postcode1",
                    EffectiveFrom = new DateTime(2018, 8, 1),
                    Lep1 = "Lep1",
                    Lep2 = "Lep2",
                    LocalAuthority = "LocalAuthority",
                    Nuts = "Nuts",
                    Termination = new DateTime(2018, 8, 1)
                },
                new ONSPostcode
                {
                    Postcode = "Postcode3",
                    EffectiveFrom = new DateTime(2018, 8, 1),
                    Lep1 = "Lep1",
                    Lep2 = "Lep2",
                    LocalAuthority = "LocalAuthority",
                    Nuts = "Nuts"
                },
                new ONSPostcode
                {
                    Postcode = "Postcode3",
                    EffectiveFrom = new DateTime(2018, 8, 1),
                    Lep1 = "Lep1",
                    Lep2 = "Lep2",
                    LocalAuthority = "LocalAuthority",
                    Nuts = "Nuts",
                    Termination = new DateTime(2018, 8, 1)
                }
            };

            NewMapper().MapONSPostcodes(postcodes).Should().BeEquivalentTo(expectedONSPostcodes);
        }

        [Fact]
        public void MapONMcaglaSOFPostcodes()
        {
            var postcodes = TestPostcodes();

            var expectedMcaglaSOFPostcodes = new List<McaglaSOFPostcode>
            {
                new McaglaSOFPostcode
                {
                    Postcode = "Postcode1",
                    SofCode = "SofCode1",
                    EffectiveFrom = new DateTime(2018, 8, 1)
                },
                new McaglaSOFPostcode
                {
                    Postcode = "Postcode1",
                    SofCode = "SofCode1",
                    EffectiveFrom = new DateTime(2018, 8, 1),
                    EffectiveTo = new DateTime(2019, 8, 1),
                },
                 new McaglaSOFPostcode
                {
                    Postcode = "Postcode3",
                    SofCode = "SofCode1",
                    EffectiveFrom = new DateTime(2018, 8, 1)
                },
                new McaglaSOFPostcode
                {
                    Postcode = "Postcode3",
                    SofCode = "SofCode1",
                    EffectiveFrom = new DateTime(2018, 8, 1),
                    EffectiveTo = new DateTime(2019, 8, 1),
                }
            };

            NewMapper().MapMcaglaSOFPostcodes(postcodes).Should().BeEquivalentTo(expectedMcaglaSOFPostcodes);
        }

        private IReadOnlyCollection<Postcode> TestPostcodes()
        {
            var onsData = new List<ONSData>
            {
                new ONSData
                {
                    EffectiveFrom = new DateTime(2018, 8, 1),
                    Lep1 = "Lep1",
                    Lep2 = "Lep2",
                    LocalAuthority = "LocalAuthority",
                    Nuts = "Nuts"
                },
                new ONSData
                {
                    EffectiveFrom = new DateTime(2018, 8, 1),
                    Lep1 = "Lep1",
                    Lep2 = "Lep2",
                    LocalAuthority = "LocalAuthority",
                    Nuts = "Nuts",
                    Termination = new DateTime(2018, 8, 1)
                }
            };

            var mcgalaSOFData = new List<McaglaSOF>
            {
                new McaglaSOF
                {
                    SofCode = "SofCode1",
                    EffectiveFrom = new DateTime(2018, 8, 1)
                },
                new McaglaSOF
                {
                    SofCode = "SofCode1",
                    EffectiveFrom = new DateTime(2018, 8, 1),
                    EffectiveTo = new DateTime(2019, 8, 1),
                }
            };

            return new List<Postcode>
            {
                new Postcode { PostCode = "Postcode1", ONSData = onsData, McaglaSOFs = mcgalaSOFData },
                new Postcode { PostCode = "Postcode2" },
                new Postcode { PostCode = "Postcode3", ONSData = onsData, McaglaSOFs = mcgalaSOFData },
                new Postcode { PostCode = "Postcode4" },
            };
        }

        private PostcodesDataMapper NewMapper()
        {
            return new PostcodesDataMapper();
        }
    }
}
