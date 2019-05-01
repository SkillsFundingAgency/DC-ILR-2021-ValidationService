using System;
using System.Collections.Generic;
using ESFA.DC.ILR.ReferenceDataService.Model.MetaData;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Model;
using ESFA.DC.ILR.ValidationService.Data.Internal.Model;
using ESFA.DC.ILR.ValidationService.Data.Population.Mappers;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Tests.Mappers
{
    public class LookupsDataMapperTests
    {
        [Fact]
        public void BuildLookups()
        {
            var lookups = TestLookups();
            var lookupsDictionary = LookupsDictionary();

            lookupsDictionary.Should().BeEquivalentTo(NewMapper().BuildLookups(lookups));
        }

        [Fact]
        public void MapAcademicYear()
        {
            var academicYearLookup = new AcademicYear
            {
                AugustThirtyFirst = new DateTime(2018, 8, 31),
                End = new DateTime(2019, 7, 31),
                JanuaryFirst = new DateTime(2019, 1, 1),
                JulyThirtyFirst = new DateTime(2019, 7, 31),
                Start = new DateTime(2018, 8, 1)
            };

            var expectedAademicYearLookup = new AcademicYear
            {
                AugustThirtyFirst = new DateTime(2018, 8, 31),
                End = new DateTime(2019, 7, 31),
                JanuaryFirst = new DateTime(2019, 1, 1),
                JulyThirtyFirst = new DateTime(2019, 7, 31),
                Start = new DateTime(2018, 8, 1)
            };

            expectedAademicYearLookup.Should().BeEquivalentTo(NewMapper().MapAcademicYear(academicYearLookup));
        }

        [Fact]
        public void MapIntegerLookups()
        {
            var lookup = LookupsDictionary();

            var expectedLookups = new Dictionary<TypeOfIntegerCodedLookup, List<int>>
            {
                {
                    TypeOfIntegerCodedLookup.AimType, new List<int> { 1, 3 }
                },
                {
                    TypeOfIntegerCodedLookup.LocType, new List<int> { 1, 2 }
                },
            };

            expectedLookups.Should().BeEquivalentTo(NewMapper().MapIntegerLookups(lookup));
        }

        [Fact]
        public void MapStringLookups()
        {
            var lookup = LookupsDictionary();

            var expectedLookups = new Dictionary<TypeOfStringCodedLookup, List<string>>
            {
                {
                    TypeOfStringCodedLookup.AppFinType, new List<string> { "PMR1", "PMR2", "PMR3", "TNP1", "TNP2", "TNP3", "TNP4" }
                },
                {
                    TypeOfStringCodedLookup.ContPrefType, new List<string> { "PMC1", "PMC2", "PMC3", "PMC4", "PMC5", "PMC6" }
                },
                {
                    TypeOfStringCodedLookup.OutGrade, new List<string> { "A", "A*" }
                },
            };

            expectedLookups.Should().BeEquivalentTo(NewMapper().MapStringLookups(lookup));
        }

        [Fact]
        public void MapLimitedLifeLookups()
        {
            var lookup = LookupsDictionary();

            var expectedLookups = new Dictionary<TypeOfLimitedLifeLookup, IReadOnlyDictionary<string, ValidityPeriods>>
            {
                {
                    TypeOfLimitedLifeLookup.LLDDCat, new Dictionary<string, ValidityPeriods>
                    {
                        {
                            "1", new ValidityPeriods(new DateTime(1900, 01, 01), new DateTime(2018, 05, 25))
                        },
                        {
                            "2", new ValidityPeriods(new DateTime(1900, 01, 01), new DateTime(2018, 05, 25))
                        }
                    }
                },
                {
                    TypeOfLimitedLifeLookup.ContPrefType, new Dictionary<string, ValidityPeriods>
                    {
                        {
                            "PMC1", new ValidityPeriods(new DateTime(1900, 01, 01), new DateTime(2018, 05, 25))
                        },
                        {
                            "PMC2", new ValidityPeriods(new DateTime(1900, 01, 01), new DateTime(2018, 05, 25))
                        },
                        {
                            "PMC3", new ValidityPeriods(new DateTime(1900, 01, 01), new DateTime(2018, 05, 25))
                        },
                        {
                            "PMC4", new ValidityPeriods(new DateTime(1900, 01, 01), new DateTime(2018, 05, 25))
                        },
                        {
                            "PMC5", new ValidityPeriods(new DateTime(1900, 01, 01), new DateTime(2018, 05, 25))
                        },
                        {
                            "PMC6", new ValidityPeriods(new DateTime(1900, 01, 01), new DateTime(2018, 05, 25))
                        }
                    }
                }
            };

            expectedLookups.Should().BeEquivalentTo(NewMapper().MapLimitedLifeLookups(lookup));
        }

        [Fact]
        public void MapListItemLookups()
        {
            var lookup = LookupsDictionary();

            var expectedLookups = new Dictionary<TypeOfListItemLookup, IReadOnlyDictionary<string, IReadOnlyCollection<string>>>
            {
                {
                    TypeOfListItemLookup.LearningAimType, new Dictionary<string, IReadOnlyCollection<string>>
                    {
                        {
                            "0001", new List<string> { "A", "B", "C", "D", "E", "F" }
                        },
                        {
                            "0002", new List<string> { "A", "B", "C", "D", "E", "F" }
                        },
                    }
                }
            };

            expectedLookups.Should().BeEquivalentTo(NewMapper().MapListItemLookups(lookup));
        }

        private List<Lookup> TestLookups()
        {
            return new List<Lookup>
            {
                new Lookup
                {
                    Name = "AimType",
                    Code = "1"
                },
                new Lookup
                {
                    Name = "AimType",
                    Code = "3"
                },
                new Lookup
                {
                    Name = "AppFinType",
                    Code = "PMR",
                    SubCategories = new List<LookupSubCategory>
                    {
                        new LookupSubCategory { Code = "1" },
                        new LookupSubCategory { Code = "2" },
                        new LookupSubCategory { Code = "3" }
                    }
                },
                new Lookup
                {
                    Name = "AppFinType",
                    Code = "TNP",
                    SubCategories = new List<LookupSubCategory>
                    {
                        new LookupSubCategory { Code = "1" },
                        new LookupSubCategory { Code = "2" },
                        new LookupSubCategory { Code = "3" },
                        new LookupSubCategory { Code = "4" }
                    }
                },
                new Lookup
                {
                    Name = "ContPrefType",
                    Code = "PMC",
                    SubCategories = new List<LookupSubCategory>
                    {
                        new LookupSubCategory { Code = "1", EffectiveFrom = new DateTime(1900, 01, 01), EffectiveTo = new DateTime(2018, 05, 25) },
                        new LookupSubCategory { Code = "2", EffectiveFrom = new DateTime(1900, 01, 01), EffectiveTo = new DateTime(2018, 05, 25) },
                        new LookupSubCategory { Code = "3", EffectiveFrom = new DateTime(1900, 01, 01), EffectiveTo = new DateTime(2018, 05, 25) },
                        new LookupSubCategory { Code = "4", EffectiveFrom = new DateTime(1900, 01, 01), EffectiveTo = new DateTime(2018, 05, 25) },
                        new LookupSubCategory { Code = "5", EffectiveFrom = new DateTime(1900, 01, 01), EffectiveTo = new DateTime(2018, 05, 25) },
                        new LookupSubCategory { Code = "6", EffectiveFrom = new DateTime(1900, 01, 01), EffectiveTo = new DateTime(2018, 05, 25) }
                    }
                },
                new Lookup
                {
                    Name = "LearningAimType",
                    Code = "0001",
                    SubCategories = new List<LookupSubCategory>
                    {
                        new LookupSubCategory { Code = "A" },
                        new LookupSubCategory { Code = "B" },
                        new LookupSubCategory { Code = "C" },
                        new LookupSubCategory { Code = "D" },
                        new LookupSubCategory { Code = "E" },
                        new LookupSubCategory { Code = "F" }
                    }
                },
                new Lookup
                {
                    Name = "LearningAimType",
                    Code = "0002",
                    SubCategories = new List<LookupSubCategory>
                    {
                        new LookupSubCategory { Code = "A" },
                        new LookupSubCategory { Code = "B" },
                        new LookupSubCategory { Code = "C" },
                        new LookupSubCategory { Code = "D" },
                        new LookupSubCategory { Code = "E" },
                        new LookupSubCategory { Code = "F" }
                    }
                },
                new Lookup
                {
                    Name = "LLDDCat",
                    Code = "1",
                    EffectiveFrom = new DateTime(1900, 01, 01),
                    EffectiveTo = new DateTime(2018, 05, 25)
                },
                new Lookup
                {
                    Name = "LLDDCat",
                    Code = "2",
                    EffectiveFrom = new DateTime(1900, 01, 01),
                    EffectiveTo = new DateTime(2018, 05, 25)
                },
                new Lookup
                {
                    Name = "LocType",
                    Code = "1"
                },
                new Lookup
                {
                    Name = "LocType",
                    Code = "2",
                },
                new Lookup
                {
                    Name = "OutGrade",
                    Code = "A"
                },
                new Lookup
                {
                    Name = "OutGrade",
                    Code = "A*"
                },
            };
        }

        private IReadOnlyDictionary<string, IReadOnlyDictionary<string, IlrLookup>> LookupsDictionary()
        {
            return new Dictionary<string, IReadOnlyDictionary<string, IlrLookup>>
            {
                {
                    "AimType", new Dictionary<string, IlrLookup>
                    {
                        {
                            "1",
                             new IlrLookup
                             {
                                 Name = "AimType",
                                 Code = "1"
                             }
                        },
                        {
                            "3",
                            new IlrLookup
                            {
                                Name = "AimType",
                                Code = "3",
                            }
                        }
                    }
                },
                {
                    "AppFinType", new Dictionary<string, IlrLookup>
                    {
                        {
                            "PMR",
                             new IlrLookup
                             {
                                 Name = "AppFinType",
                                 Code = "PMR",
                                 SubLookup = new List<IlrSubLookup>
                                 {
                                     new IlrSubLookup { Code = "1" },
                                     new IlrSubLookup { Code = "2" },
                                     new IlrSubLookup { Code = "3" }
                                 }
                             }
                        },
                        {
                            "TNP",
                            new IlrLookup
                             {
                                 Name = "AppFinType",
                                 Code = "TNP",
                                 SubLookup = new List<IlrSubLookup>
                                 {
                                     new IlrSubLookup { Code = "1" },
                                     new IlrSubLookup { Code = "2" },
                                     new IlrSubLookup { Code = "3" },
                                     new IlrSubLookup { Code = "4" }
                                 }
                             }
                        }
                    }
                },
                {
                    "ContPrefType", new Dictionary<string, IlrLookup>
                    {
                        {
                            "PMC",
                            new IlrLookup
                            {
                                Name = "ContPrefType",
                                Code = "PMC",
                                SubLookup = new List<IlrSubLookup>
                                {
                                    new IlrSubLookup { Code = "1", ValidityPeriods = new ValidityPeriods(new DateTime(1900, 01, 01), new DateTime(2018, 05, 25)) },
                                    new IlrSubLookup { Code = "2", ValidityPeriods = new ValidityPeriods(new DateTime(1900, 01, 01), new DateTime(2018, 05, 25)) },
                                    new IlrSubLookup { Code = "3", ValidityPeriods = new ValidityPeriods(new DateTime(1900, 01, 01), new DateTime(2018, 05, 25)) },
                                    new IlrSubLookup { Code = "4", ValidityPeriods = new ValidityPeriods(new DateTime(1900, 01, 01), new DateTime(2018, 05, 25)) },
                                    new IlrSubLookup { Code = "5", ValidityPeriods = new ValidityPeriods(new DateTime(1900, 01, 01), new DateTime(2018, 05, 25)) },
                                    new IlrSubLookup { Code = "6", ValidityPeriods = new ValidityPeriods(new DateTime(1900, 01, 01), new DateTime(2018, 05, 25)) }
                                }
                            }
                        }
                    }
                },
                {
                    "LearningAimType", new Dictionary<string, IlrLookup>
                    {
                        {
                            "0001",
                            new IlrLookup
                            {
                                Name = "LearningAimType",
                                Code = "0001",
                                SubLookup = new List<IlrSubLookup>
                                {
                                    new IlrSubLookup { Code = "A" },
                                    new IlrSubLookup { Code = "B" },
                                    new IlrSubLookup { Code = "C" },
                                    new IlrSubLookup { Code = "D" },
                                    new IlrSubLookup { Code = "E" },
                                    new IlrSubLookup { Code = "F" }
                                }
                            }
                        },
                        {
                            "0002",
                            new IlrLookup
                            {
                                Name = "LearningAimType",
                                Code = "0002",
                                SubLookup = new List<IlrSubLookup>
                                {
                                    new IlrSubLookup { Code = "A" },
                                    new IlrSubLookup { Code = "B" },
                                    new IlrSubLookup { Code = "C" },
                                    new IlrSubLookup { Code = "D" },
                                    new IlrSubLookup { Code = "E" },
                                    new IlrSubLookup { Code = "F" }
                                }
                            }
                        }
                    }
                },
                {
                    "LLDDCat", new Dictionary<string, IlrLookup>
                    {
                        {
                            "1",
                            new IlrLookup
                            {
                                Name = "LLDDCat",
                                Code = "1",
                                ValidityPeriods = new ValidityPeriods(new DateTime(1900, 01, 01), new DateTime(2018, 05, 25))
                            }
                        },
                        {
                            "2",
                            new IlrLookup
                            {
                                Name = "LLDDCat",
                                Code = "2",
                                ValidityPeriods = new ValidityPeriods(new DateTime(1900, 01, 01), new DateTime(2018, 05, 25))
                            }
                        }
                    }
                },
                {
                    "LocType", new Dictionary<string, IlrLookup>
                    {
                        {
                            "1",
                            new IlrLookup
                             {
                                 Name = "LocType",
                                 Code = "1"
                             }
                        },
                        {
                            "2",
                            new IlrLookup
                             {
                                 Name = "LocType",
                                 Code = "2",
                             }
                        }
                    }
                },
                {
                    "OutGrade", new Dictionary<string, IlrLookup>
                    {
                        {
                            "A",
                            new IlrLookup
                            {
                                Name = "OutGrade",
                                Code = "A"
                            }
                        },
                        {
                            "A*",
                            new IlrLookup
                            {
                                Name = "OutGrade",
                                Code = "A*"
                            }
                        }
                    }
                },
            };
        }

        private LookupsDataMapper NewMapper()
        {
            return new LookupsDataMapper();
        }
    }
}
