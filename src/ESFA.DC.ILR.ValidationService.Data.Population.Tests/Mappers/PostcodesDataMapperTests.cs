using System;
using System.Collections.Generic;
using ESFA.DC.ILR.ReferenceDataService.Model.Postcodes;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Mappers;
using FluentAssertions;
using Xunit;
using DevolvedPostcodeRDS = ESFA.DC.ILR.ReferenceDataService.Model.PostcodesDevolution.DevolvedPostcode;

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
        public void MapDevolvedPostcodes()
        {
            var postcodes = TestDevolvedPostcodes();

            var expectedDevolvedPostcodes = new Dictionary<string, List<DevolvedPostcode>>
            {
                {
                    "Postcode1", new List<DevolvedPostcode>
                    {
                        new DevolvedPostcode
                        {
                            Postcode = "Postcode1",
                            Area = "Area1",
                            SourceOfFunding = "150",
                            EffectiveFrom = new DateTime(2019, 8, 1)
                        },
                    }
                },
                {
                    "Postcode2", new List<DevolvedPostcode>
                    {
                        new DevolvedPostcode
                        {
                            Postcode = "Postcode2",
                            Area = "Area1",
                            SourceOfFunding = "150",
                            EffectiveFrom = new DateTime(2019, 8, 1),
                            EffectiveTo = new DateTime(2020, 8, 1),
                        },
                        new DevolvedPostcode
                        {
                            Postcode = "Postcode2",
                            Area = "Area2",
                            SourceOfFunding = "105",
                            EffectiveFrom = new DateTime(2018, 8, 1)
                        }
                    }
                }
            };

            NewMapper().MapDevolvedPostcodes(postcodes).Should().BeEquivalentTo(expectedDevolvedPostcodes);
        }

        [Fact]
        public void MapDevolvedPostcodes_Null()
        {
            NewMapper().MapDevolvedPostcodes(null).Should().BeEquivalentTo(new Dictionary<string, DevolvedPostcode>());
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

            return new List<Postcode>
            {
                new Postcode { PostCode = "Postcode1", ONSData = onsData },
                new Postcode { PostCode = "Postcode2" },
                new Postcode { PostCode = "Postcode3", ONSData = onsData },
                new Postcode { PostCode = "Postcode4" },
            };
        }

        private IReadOnlyCollection<DevolvedPostcodeRDS> TestDevolvedPostcodes()
        {
            return new List<DevolvedPostcodeRDS>
            {
                new DevolvedPostcodeRDS
                {
                    Postcode = "Postcode1",
                    Area = "Area1",
                    SourceOfFunding = "150",
                    EffectiveFrom = new DateTime(2019, 8, 1)
                },
                new DevolvedPostcodeRDS
                {
                    Postcode = "Postcode2",
                    Area = "Area1",
                    SourceOfFunding = "150",
                    EffectiveFrom = new DateTime(2019, 8, 1),
                    EffectiveTo = new DateTime(2020, 8, 1),
                },
                new DevolvedPostcodeRDS
                {
                    Postcode = "Postcode2",
                    Area = "Area2",
                    SourceOfFunding = "105",
                    EffectiveFrom = new DateTime(2018, 8, 1)
                }
            };
        }

        private PostcodesDataMapper NewMapper()
        {
            return new PostcodesDataMapper();
        }
    }
}
