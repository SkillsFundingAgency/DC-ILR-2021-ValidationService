using System;
using System.Collections.Generic;
using ESFA.DC.ILR.ReferenceDataService.Model.LARS;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Model;
using ESFA.DC.ILR.ValidationService.Data.Population.Mappers;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Tests.Mappers
{
    public class LarsDataMapperTests
    {
        [Fact]
        public void MapLarsLearningDeliveries()
        {
            var larsLearningDeliveries = TestLarsLearningDeliveries();

            var expectedLarsLearningDeliveries = new Dictionary<string, LearningDelivery>
            {
                {
                    "LearnAimRef1", new LearningDelivery
                    {
                        LearnAimRef = "LearnAimRef1",
                        EffectiveFrom = new DateTime(2018, 8, 1),
                        FrameworkCommonComponent = 1,
                        LearnAimRefType = "LearnAimRefType",
                        EnglPrscID = 2,
                        NotionalNVQLevel = "NotionalNVQLevel",
                        NotionalNVQLevelv2 = "NotionalNVQLevelv2",
                        LearnDirectClassSystemCode1 = new LearnDirectClassSystemCode("LearnDirectClassSystemCode1"),
                        LearnDirectClassSystemCode2 = new LearnDirectClassSystemCode("LearnDirectClassSystemCode2"),
                        LearnDirectClassSystemCode3 = new LearnDirectClassSystemCode("LearnDirectClassSystemCode3"),
                        SectorSubjectAreaTier1 = 1.0m,
                        SectorSubjectAreaTier2 = 2.0m,
                        AnnualValues = new List<AnnualValue>
                        {
                            new AnnualValue
                            {
                                LearnAimRef = "LearnAimRef1",
                                BasicSkills = 1,
                                BasicSkillsType = 2,
                                FullLevel2EntitlementCategory = 3,
                                FullLevel3EntitlementCategory = 4,
                                FullLevel3Percent = 5.0m,
                                EffectiveFrom = new DateTime(2018, 8, 1)
                            }
                        },
                        Categories = new List<LearningDeliveryCategory>
                        {
                            new LearningDeliveryCategory
                            {
                                LearnAimRef = "LearnAimRef1",
                                CategoryRef = 1,
                                EffectiveFrom = new DateTime(2018, 8, 1)
                            }
                        },
                        Validities = new List<Data.External.LARS.Model.LARSValidity>
                        {
                            new Data.External.LARS.Model.LARSValidity
                            {
                                LearnAimRef = "LearnAimRef1",
                                ValidityCategory = "ValidityCategory",
                                StartDate = new DateTime(2018, 8, 1),
                                EndDate = new DateTime(2019, 8, 1),
                                LastNewStartDate = new DateTime(2018, 8, 1)
                            }
                        }
                    }
                },
                {
                    "LearnAimRef2", new LearningDelivery
                    {
                        LearnAimRef = "LearnAimRef2",
                        EffectiveFrom = new DateTime(2018, 8, 1),
                        FrameworkCommonComponent = 1,
                        LearnAimRefType = "LearnAimRefType",
                        EnglPrscID = 2,
                        NotionalNVQLevel = "NotionalNVQLevel",
                        NotionalNVQLevelv2 = "NotionalNVQLevelv2",
                        LearnDirectClassSystemCode1 = new LearnDirectClassSystemCode("LearnDirectClassSystemCode1"),
                        LearnDirectClassSystemCode2 = new LearnDirectClassSystemCode("LearnDirectClassSystemCode2"),
                        LearnDirectClassSystemCode3 = new LearnDirectClassSystemCode("LearnDirectClassSystemCode3"),
                        SectorSubjectAreaTier1 = 1.0m,
                        SectorSubjectAreaTier2 = 2.0m,
                        Frameworks = new List<Framework>
                        {
                            new Framework
                            {
                                FworkCode = 1,
                                ProgType = 2,
                                PwayCode = 3,
                                FrameworkAim = new FrameworkAim
                                {
                                    LearnAimRef = "LearnAimRef2",
                                    FworkCode = 1,
                                    ProgType = 2,
                                    PwayCode = 3,
                                    FrameworkComponentType = 4,
                                    EffectiveFrom = new DateTime(2018, 8, 1),
                                },
                                FrameworkCommonComponents = new List<FrameworkCommonComponent>
                                {
                                    new FrameworkCommonComponent
                                    {
                                        FworkCode = 1,
                                        ProgType = 2,
                                        PwayCode = 3,
                                        CommonComponent = 1,
                                        EffectiveFrom = new DateTime(2018, 8, 1),
                                    }
                                }
                            }
                        }
                    }
                }
            };

            NewMapper().MapLarsLearningDeliveries(larsLearningDeliveries).Should().BeEquivalentTo(expectedLarsLearningDeliveries);
        }

        [Fact]
        public void MapLarsStandards()
        {
            var larsStandards = TestLarsStandards();

            var expectedLarsStandards = new List<Data.External.LARS.Model.LARSStandard>
            {
                new Data.External.LARS.Model.LARSStandard
                {
                    StandardCode = 1,
                    EffectiveFrom = new DateTime(2018, 8, 1),
                    NotionalEndLevel = "NotionalEndLevel",
                    StandardSectorCode = "StandardSectorCode",
                    StandardsFunding = new List<Data.External.LARS.Model.LARSStandardFunding>
                    {
                        new Data.External.LARS.Model.LARSStandardFunding
                        {
                            CoreGovContributionCap = 1.0m,
                            EffectiveFrom = new DateTime(2018, 8, 1)
                        }
                    }
                },
                new Data.External.LARS.Model.LARSStandard
                {
                    StandardCode = 2,
                    EffectiveFrom = new DateTime(2018, 8, 1),
                    NotionalEndLevel = "NotionalEndLevel",
                    StandardSectorCode = "StandardSectorCode"
                }
            };

            NewMapper().MapLarsStandards(larsStandards).Should().BeEquivalentTo(expectedLarsStandards);
        }

        [Fact]
        public void MapLarsStandardValidities()
        {
            var larsStandards = TestLarsStandards();

            var expectedLarsStandarValidities = new List<Data.External.LARS.Model.LARSStandardValidity>
            {
                new Data.External.LARS.Model.LARSStandardValidity
                {
                    StandardCode = 2,
                    StartDate = new DateTime(2018, 8, 1),
                    EndDate = new DateTime(2019, 8, 1),
                    ValidityCategory = "ValidityCategory",
                    LastNewStartDate = new DateTime(2018, 8, 1)
                }
            };

            NewMapper().MapLarsStandardValidities(larsStandards).Should().BeEquivalentTo(expectedLarsStandarValidities);
        }

        private IReadOnlyCollection<ReferenceDataService.Model.LARS.LARSLearningDelivery> TestLarsLearningDeliveries()
        {
            return new List<LARSLearningDelivery>
            {
                new LARSLearningDelivery
                {
                    LearnAimRef = "LearnAimRef1",
                    EffectiveFrom = new DateTime(2018, 8, 1),
                    FrameworkCommonComponent = 1,
                    LearnAimRefType = "LearnAimRefType",
                    EnglPrscID = 2,
                    NotionalNVQLevel = "NotionalNVQLevel",
                    NotionalNVQLevelv2 = "NotionalNVQLevelv2",
                    LearnDirectClassSystemCode1 = "LearnDirectClassSystemCode1",
                    LearnDirectClassSystemCode2 = "LearnDirectClassSystemCode2",
                    LearnDirectClassSystemCode3 = "LearnDirectClassSystemCode3",
                    SectorSubjectAreaTier1 = 1.0m,
                    SectorSubjectAreaTier2 = 2.0m,
                    LARSAnnualValues = new List<LARSAnnualValue>
                    {
                        new LARSAnnualValue
                        {
                            BasicSkills = 1,
                            BasicSkillsType = 2,
                            FullLevel2EntitlementCategory = 3,
                            FullLevel3EntitlementCategory = 4,
                            FullLevel3Percent = 5.0m,
                            EffectiveFrom = new DateTime(2018, 8, 1)
                        }
                    },
                    LARSLearningDeliveryCategories = new List<LARSLearningDeliveryCategory>
                    {
                        new LARSLearningDeliveryCategory
                        {
                            CategoryRef = 1,
                            EffectiveFrom = new DateTime(2018, 8, 1)
                        }
                    },
                    LARSValidities = new List<ReferenceDataService.Model.LARS.LARSValidity>
                    {
                        new ReferenceDataService.Model.LARS.LARSValidity
                        {
                            ValidityCategory = "ValidityCategory",
                            EffectiveFrom = new DateTime(2018, 8, 1),
                            EffectiveTo = new DateTime(2019, 8, 1),
                            LastNewStartDate = new DateTime(2018, 8, 1)
                        }
                    }
                },
                new LARSLearningDelivery
                {
                    LearnAimRef = "LearnAimRef2",
                    EffectiveFrom = new DateTime(2018, 8, 1),
                    FrameworkCommonComponent = 1,
                    LearnAimRefType = "LearnAimRefType",
                    EnglPrscID = 2,
                    NotionalNVQLevel = "NotionalNVQLevel",
                    NotionalNVQLevelv2 = "NotionalNVQLevelv2",
                    LearnDirectClassSystemCode1 = "LearnDirectClassSystemCode1",
                    LearnDirectClassSystemCode2 = "LearnDirectClassSystemCode2",
                    LearnDirectClassSystemCode3 = "LearnDirectClassSystemCode3",
                    SectorSubjectAreaTier1 = 1.0m,
                    SectorSubjectAreaTier2 = 2.0m,
                    LARSFrameworks = new List<LARSFramework>
                    {
                        new LARSFramework
                        {
                            FworkCode = 1,
                            ProgType = 2,
                            PwayCode = 3,
                            LARSFrameworkAim = new LARSFrameworkAim
                            {
                                FrameworkComponentType = 4,
                                EffectiveFrom = new DateTime(2018, 8, 1)
                            },
                            LARSFrameworkCommonComponents = new List<LARSFrameworkCommonComponent>
                            {
                                new LARSFrameworkCommonComponent
                                {
                                    CommonComponent = 1,
                                    EffectiveFrom = new DateTime(2018, 8, 1)
                                }
                            },
                            LARSFrameworkApprenticeshipFundings = new List<LARSFrameworkApprenticeshipFunding>
                            {
                                new LARSFrameworkApprenticeshipFunding
                                {
                                    BandNumber = 1,
                                    CareLeaverAdditionalPayment = 1.0m,
                                    EffectiveFrom = new DateTime(2018, 8, 1)
                                }
                            }
                        }
                    }
                }
            };
        }

        private IReadOnlyCollection<ReferenceDataService.Model.LARS.LARSStandard> TestLarsStandards()
        {
            return new List<ReferenceDataService.Model.LARS.LARSStandard>
            {
                new ReferenceDataService.Model.LARS.LARSStandard
                {
                    StandardCode = 1,
                    EffectiveFrom = new DateTime(2018, 8, 1),
                    NotionalEndLevel = "NotionalEndLevel",
                    StandardSectorCode = "StandardSectorCode",
                    LARSStandardApprenticeshipFundings = new List<LARSStandardApprenticeshipFunding>
                    {
                        new LARSStandardApprenticeshipFunding
                        {
                            BandNumber = 1
                        }
                    },
                    LARSStandardFundings = new List<ReferenceDataService.Model.LARS.LARSStandardFunding>
                    {
                        new ReferenceDataService.Model.LARS.LARSStandardFunding
                        {
                            BandNumber = 1,
                            CoreGovContributionCap = 1.0m,
                            EffectiveFrom = new DateTime(2018, 8, 1),
                        }
                    }
                },
                 new ReferenceDataService.Model.LARS.LARSStandard
                {
                    StandardCode = 2,
                    EffectiveFrom = new DateTime(2018, 8, 1),
                    NotionalEndLevel = "NotionalEndLevel",
                    StandardSectorCode = "StandardSectorCode",
                    LARSStandardCommonComponents = new List<LARSStandardCommonComponent>
                    {
                        new LARSStandardCommonComponent
                        {
                            CommonComponent = 1,
                            EffectiveFrom = new DateTime(2018, 8, 1)
                        }
                    },
                    LARSStandardValidities = new List<ReferenceDataService.Model.LARS.LARSStandardValidity>
                    {
                        new ReferenceDataService.Model.LARS.LARSStandardValidity
                        {
                            ValidityCategory = "ValidityCategory",
                            EffectiveFrom = new DateTime(2018, 8, 1),
                            EffectiveTo = new DateTime(2019, 8, 1),
                            LastNewStartDate = new DateTime(2018, 8, 1)
                        }
                    }
                }
            };
        }

        private LarsDataMapper NewMapper()
        {
            return new LarsDataMapper();
        }
    }
}
