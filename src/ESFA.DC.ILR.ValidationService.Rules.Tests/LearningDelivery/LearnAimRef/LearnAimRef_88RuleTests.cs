using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_88RuleTests : AbstractRuleTests<LearnAimRef_88Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnAimRef_88");
        }

        [Fact]
        public void LarsValidityConditionMet_False()
        {
            var learnAimRef = "learnAimRef";
            var validities = new List<LARSValidity>
            {
                new LARSValidity
                {
                    LearnAimRef = learnAimRef,
                    ValidityCategory = "ADULT_SKILLS",
                    StartDate = new DateTime(2018, 8, 1),
                    EndDate = new DateTime(2019, 7, 31),
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ds => ds.GetValiditiesFor(learnAimRef)).Returns(validities);

            NewRule(larsDataServiceMock.Object).LarsValidityConditionMet("ADULT_SKILLS", learnAimRef, new DateTime(2019, 7, 31)).Should().BeFalse();
        }

        [Fact]
        public void LarsValidityConditionMet_True_NoValiditiesMatch()
        {
            var learnAimRef = "learnAimRef";

            var validities = new List<LARSValidity>
            {
                new LARSValidity
                {
                    LearnAimRef = learnAimRef,
                    ValidityCategory = "ADULT_SKILLS",
                    StartDate = new DateTime(2018, 8, 1),
                    EndDate = new DateTime(2019, 7, 31),
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ds => ds.GetValiditiesFor(learnAimRef)).Returns(validities);

            NewRule(larsDataServiceMock.Object).LarsValidityConditionMet("ANY", learnAimRef, new DateTime(2019, 7, 31)).Should().BeTrue();
        }

        [Fact]
        public void LarsValidityConditionMet_True_BeforeStartDate()
        {
            var learnAimRef = "learnAimRef";
            var validities = new List<LARSValidity>
            {
                new LARSValidity
                {
                    LearnAimRef = learnAimRef,
                    ValidityCategory = "ADULT_SKILLS",
                    StartDate = new DateTime(2019, 8, 1),
                    EndDate = new DateTime(2020, 7, 31),
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ds => ds.GetValiditiesFor(learnAimRef)).Returns(validities);

            NewRule(larsDataServiceMock.Object).LarsValidityConditionMet("ADULT_SKILLS", learnAimRef, new DateTime(2019, 7, 31)).Should().BeTrue();
        }

        [Fact]
        public void LarsValidityConditionMet_True_AfterLastNewStartDate()
        {
            var learnAimRef = "learnAimRef";
            var validities = new List<LARSValidity>
            {
                new LARSValidity
                {
                    LearnAimRef = learnAimRef,
                    ValidityCategory = "ADULT_SKILLS",
                    StartDate = new DateTime(2019, 8, 1),
                    LastNewStartDate = new DateTime(2019, 10, 1),
                    EndDate = new DateTime(2020, 7, 31),
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ds => ds.GetValiditiesFor(learnAimRef)).Returns(validities);

            NewRule(larsDataServiceMock.Object).LarsValidityConditionMet("ADULT_SKILLS", learnAimRef, new DateTime(2019, 12, 31)).Should().BeTrue();
        }

        [Fact]
        public void LarsValidityConditionMet_True_NoMatchingValidityCategory()
        {
            var learnAimRef = "learnAimRef";
            var validities = new List<LARSValidity>();

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ds => ds.GetValiditiesFor(learnAimRef)).Returns(validities);

            NewRule(larsDataServiceMock.Object).LarsValidityConditionMet("ADULT_SKILLS", learnAimRef, new DateTime(2019, 12, 31)).Should().BeTrue();
        }

        [Fact]
        public void LarsCategoryConditionMet_False()
        {
            var learnAimRef = "learnAimRef";
            var categories = new List<LearningDeliveryCategory>
            {
                new LearningDeliveryCategory
                {
                    LearnAimRef = learnAimRef,
                    CategoryRef = 41,
                    EffectiveFrom = new DateTime(2018, 8, 1),
                    EffectiveTo = new DateTime(2019, 7, 31),
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(ds => ds.GetCategoriesFor(learnAimRef)).Returns(categories);

            NewRule(larsDataServiceMock.Object).LarsCategoryConditionMet(41, learnAimRef, new DateTime(2019, 7, 31)).Should().BeFalse();
        }

        [Fact]
        public void LarsCategoryConditionMet_True_NoCategoryMatch()
        {
            var learnAimRef = "learnAimRef";
            var categories = new List<LearningDeliveryCategory>
            {
                new LearningDeliveryCategory
                {
                    LearnAimRef = learnAimRef,
                    CategoryRef = 41,
                    EffectiveFrom = new DateTime(2018, 8, 1),
                    EffectiveTo = new DateTime(2019, 7, 31),
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(ds => ds.GetCategoriesFor(learnAimRef)).Returns(categories);

            NewRule(larsDataServiceMock.Object).LarsCategoryConditionMet(50, learnAimRef, new DateTime(2019, 7, 31)).Should().BeTrue();
        }

        [Fact]
        public void LarsCategoryConditionMet_True_BeforeStartDate()
        {
            var learnAimRef = "learnAimRef";
            var categories = new List<LearningDeliveryCategory>
            {
                new LearningDeliveryCategory
                {
                    LearnAimRef = learnAimRef,
                    CategoryRef = 41,
                    EffectiveFrom = new DateTime(2018, 8, 1),
                    EffectiveTo = new DateTime(2019, 7, 31),
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(ds => ds.GetCategoriesFor(learnAimRef)).Returns(categories);

            NewRule(larsDataServiceMock.Object).LarsCategoryConditionMet(41, learnAimRef, new DateTime(2018, 7, 31)).Should().BeTrue();
        }

        [Fact]
        public void LarsCategoryConditionMet_True_AfterEndDate()
        {
            var learnAimRef = "learnAimRef";
            var categories = new List<LearningDeliveryCategory>
            {
                new LearningDeliveryCategory
                {
                    LearnAimRef = learnAimRef,
                    CategoryRef = 41,
                    EffectiveFrom = new DateTime(2018, 8, 1),
                    EffectiveTo = new DateTime(2019, 7, 31),
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(ds => ds.GetCategoriesFor(learnAimRef)).Returns(categories);

            NewRule(larsDataServiceMock.Object).LarsCategoryConditionMet(41, learnAimRef, new DateTime(2020, 7, 31)).Should().BeTrue();
        }

        [Fact]
        public void LarsCategoryConditionMet_True_NoMatchingValidityCategory()
        {
            var learnAimRef = "learnAimRef";
            var categories = new List<LearningDeliveryCategory>();

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(ds => ds.GetCategoriesFor(learnAimRef)).Returns(categories);

            NewRule(larsDataServiceMock.Object).LarsCategoryConditionMet(41, learnAimRef, new DateTime(2019, 12, 31)).Should().BeTrue();
        }

        [Fact]
        public void CategoryRefConditionMet_False()
        {
            var learnAimRef = "learnAimRef";
            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = learnAimRef,
                FundModel = 35,
                LearnStartDate = new DateTime(2019, 08, 31)
            };

            var ddCategoryMock = new Mock<IDerivedData_CategoryRef_01>();
            ddCategoryMock.Setup(x => x.Derive(learningDelivery)).Returns((int?)null);

            NewRule(ddCategoryRef: ddCategoryMock.Object).CategoryRefConditionMet(learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void CategoryRefConditionMet_False_Lars()
        {
            var learnAimRef = "learnAimRef";
            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = learnAimRef,
                FundModel = 35,
                LearnStartDate = new DateTime(2019, 08, 31)
            };

            var categories = new List<LearningDeliveryCategory>
            {
                new LearningDeliveryCategory
                {
                    LearnAimRef = learnAimRef,
                    CategoryRef = 41,
                    EffectiveFrom = new DateTime(2018, 8, 1),
                    EffectiveTo = new DateTime(2020, 7, 31),
                }
            };

            var ddCategoryMock = new Mock<IDerivedData_CategoryRef_01>();
            ddCategoryMock.Setup(x => x.Derive(learningDelivery)).Returns(41);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(ds => ds.GetCategoriesFor(learnAimRef)).Returns(categories);

            NewRule(larsDataServiceMock.Object, ddCategoryRef: ddCategoryMock.Object).CategoryRefConditionMet(learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void CategoryRefConditionMet_True_LarsMismatch()
        {
            var learnAimRef = "learnAimRef";
            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = learnAimRef,
                FundModel = 35,
                LearnStartDate = new DateTime(2019, 08, 31)
            };

            var categories = new List<LearningDeliveryCategory>
            {
                new LearningDeliveryCategory
                {
                    LearnAimRef = learnAimRef,
                    CategoryRef = 50,
                    EffectiveFrom = new DateTime(2018, 8, 1),
                    EffectiveTo = new DateTime(2020, 7, 31),
                }
            };

            var ddCategoryMock = new Mock<IDerivedData_CategoryRef_01>();
            ddCategoryMock.Setup(x => x.Derive(learningDelivery)).Returns(41);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(ds => ds.GetCategoriesFor(learnAimRef)).Returns(categories);

            NewRule(larsDataServiceMock.Object, ddCategoryRef: ddCategoryMock.Object).CategoryRefConditionMet(learningDelivery).Should().BeTrue();
        }

        [Fact]
        public void CategoryRefConditionMet_True_LarsNoCategories()
        {
            var learnAimRef = "learnAimRef";
            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = learnAimRef,
                FundModel = 35,
                LearnStartDate = new DateTime(2019, 08, 31)
            };

            var categories = new List<LearningDeliveryCategory>();

            var ddCategoryMock = new Mock<IDerivedData_CategoryRef_01>();
            ddCategoryMock.Setup(x => x.Derive(learningDelivery)).Returns(41);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(ds => ds.GetCategoriesFor(learnAimRef)).Returns(categories);

            NewRule(larsDataServiceMock.Object, ddCategoryRef: ddCategoryMock.Object).CategoryRefConditionMet(learningDelivery).Should().BeTrue();
        }

        [Fact]
        public void ValidityCategoryConditionMet_False()
        {
            var learnAimRef = "learnAimRef";
            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = learnAimRef,
                FundModel = 35,
                LearnStartDate = new DateTime(2019, 08, 31)
            };

            var validities = new List<LARSValidity>
            {
                new LARSValidity
                {
                    LearnAimRef = learnAimRef,
                    ValidityCategory = "ADULT_SKILLS",
                    StartDate = new DateTime(2019, 8, 1),
                    LastNewStartDate = new DateTime(2019, 10, 1),
                    EndDate = new DateTime(2020, 7, 31),
                }
            };

            var ddValidityMock = new Mock<IDerivedData_ValidityCategory_01>();
            ddValidityMock.Setup(d => d.Derive(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>())).Returns((string)null);

            NewRule(ddValidityCategory: ddValidityMock.Object).ValidityCategoryConditionMet(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>()).Should().BeFalse();
        }

        [Fact]
        public void ValidityCategoryConditionMet_False_Lars()
        {
            var learnAimRef = "learnAimRef";
            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = learnAimRef,
                FundModel = 35,
                LearnStartDate = new DateTime(2019, 08, 31)
            };

            var validities = new List<LARSValidity>
            {
                new LARSValidity
                {
                    LearnAimRef = learnAimRef,
                    ValidityCategory = "ADULT_SKILLS",
                    StartDate = new DateTime(2019, 8, 1),
                    LastNewStartDate = new DateTime(2019, 10, 1),
                    EndDate = new DateTime(2020, 7, 31),
                }
            };

            var ddValidityMock = new Mock<IDerivedData_ValidityCategory_01>();
            ddValidityMock.Setup(d => d.Derive(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>())).Returns("ADULT_SKILLS");

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(ds => ds.GetValiditiesFor(learnAimRef)).Returns(validities);

            NewRule(larsDataServiceMock.Object, ddValidityMock.Object).ValidityCategoryConditionMet(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>()).Should().BeFalse();
        }

        [Fact]
        public void ValidityCategoryConditionMet_True_LarsMismatch()
        {
            var learnAimRef = "learnAimRef";
            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = learnAimRef,
                FundModel = 35,
                LearnStartDate = new DateTime(2019, 08, 31)
            };

            var validities = new List<LARSValidity>
            {
                new LARSValidity
                {
                    LearnAimRef = learnAimRef,
                    ValidityCategory = "ADULT_SKILLS",
                    StartDate = new DateTime(2019, 8, 1),
                    LastNewStartDate = new DateTime(2019, 10, 1),
                    EndDate = new DateTime(2020, 7, 31),
                }
            };

            var ddValidityMock = new Mock<IDerivedData_ValidityCategory_01>();
            ddValidityMock.Setup(d => d.Derive(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>())).Returns("ANY");

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(ds => ds.GetValiditiesFor(learnAimRef)).Returns(validities);

            NewRule(larsDataServiceMock.Object, ddValidityMock.Object).ValidityCategoryConditionMet(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>()).Should().BeTrue();
        }

        [Fact]
        public void ValidityCategoryConditionMet_True_LarsNoValidities()
        {
            var learnAimRef = "learnAimRef";
            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = learnAimRef,
                FundModel = 35,
                LearnStartDate = new DateTime(2019, 08, 31)
            };

            var validities = new List<LARSValidity>();

            var ddValidityMock = new Mock<IDerivedData_ValidityCategory_01>();
            ddValidityMock.Setup(d => d.Derive(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>())).Returns("ADULT_SKILLS");

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(ds => ds.GetValiditiesFor(learnAimRef)).Returns(validities);

            NewRule(ddValidityCategory: ddValidityMock.Object).ValidityCategoryConditionMet(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>()).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learnAimRef = "learnAimRef";
            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = learnAimRef,
                LearnStartDate = new DateTime(2019, 12, 31)
            };

            var validities = new List<LARSValidity>
            {
                new LARSValidity
                {
                    LearnAimRef = learnAimRef,
                    ValidityCategory = "ANY",
                    StartDate = new DateTime(2019, 8, 1),
                    LastNewStartDate = new DateTime(2019, 10, 1),
                    EndDate = new DateTime(2020, 7, 31),
                }
            };

            var categories = new List<LearningDeliveryCategory>
            {
                new LearningDeliveryCategory
                {
                    LearnAimRef = learnAimRef,
                    CategoryRef = 50,
                    EffectiveFrom = new DateTime(2020, 1, 1),
                    EffectiveTo = new DateTime(2020, 7, 31),
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var ddValidityMock = new Mock<IDerivedData_ValidityCategory_01>();
            var ddCategoryMock = new Mock<IDerivedData_CategoryRef_01>();

            larsDataServiceMock.Setup(ds => ds.GetValiditiesFor(learnAimRef)).Returns(validities);
            ddValidityMock.Setup(d => d.Derive(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>())).Returns("ADULT_SKILLS");
            ddCategoryMock.Setup(x => x.Derive(learningDelivery)).Returns(41);
            larsDataServiceMock.Setup(ds => ds.GetCategoriesFor(learnAimRef)).Returns(categories);

            NewRule(larsDataServiceMock.Object, ddValidityMock.Object, ddCategoryMock.Object).ConditionMet(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>()).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_True_NoValidityMatch_CategoryRefDates()
        {
            var learnAimRef = "learnAimRef";
            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = learnAimRef,
                LearnStartDate = new DateTime(2019, 12, 31)
            };

            var validities = new List<LARSValidity>
            {
                new LARSValidity
                {
                    LearnAimRef = learnAimRef,
                    ValidityCategory = "ANY",
                    StartDate = new DateTime(2019, 8, 1),
                    LastNewStartDate = new DateTime(2019, 10, 1),
                    EndDate = new DateTime(2020, 7, 31),
                }
            };

            var categories = new List<LearningDeliveryCategory>
            {
                new LearningDeliveryCategory
                {
                    LearnAimRef = learnAimRef,
                    CategoryRef = 41,
                    EffectiveFrom = new DateTime(2020, 1, 1),
                    EffectiveTo = new DateTime(2020, 7, 31),
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var ddValidityMock = new Mock<IDerivedData_ValidityCategory_01>();
            var ddCategoryMock = new Mock<IDerivedData_CategoryRef_01>();

            larsDataServiceMock.Setup(ds => ds.GetValiditiesFor(learnAimRef)).Returns(validities);
            ddValidityMock.Setup(d => d.Derive(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>())).Returns("ADULT_SKILLS");
            ddCategoryMock.Setup(x => x.Derive(learningDelivery)).Returns(41);
            larsDataServiceMock.Setup(ds => ds.GetCategoriesFor(learnAimRef)).Returns(categories);

            NewRule(larsDataServiceMock.Object, ddValidityMock.Object, ddCategoryMock.Object).ConditionMet(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>()).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_ValidityPasses()
        {
            var learnAimRef = "learnAimRef";
            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = learnAimRef,
                FundModel = 35,
                LearnStartDate = new DateTime(2019, 08, 31)
            };

            var validities = new List<LARSValidity>
            {
                new LARSValidity
                {
                    LearnAimRef = learnAimRef,
                    ValidityCategory = "ADULT_SKILLS",
                    StartDate = new DateTime(2019, 8, 1),
                    LastNewStartDate = new DateTime(2019, 10, 1),
                    EndDate = new DateTime(2020, 7, 31),
                }
            };
            var categories = new List<LearningDeliveryCategory>
            {
                new LearningDeliveryCategory
                {
                    LearnAimRef = learnAimRef,
                    CategoryRef = 41,
                    EffectiveFrom = new DateTime(2019, 8, 1),
                    EffectiveTo = new DateTime(2020, 7, 31),
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var ddValidityMock = new Mock<IDerivedData_ValidityCategory_01>();
            var ddCategoryMock = new Mock<IDerivedData_CategoryRef_01>();

            larsDataServiceMock.Setup(ds => ds.GetValiditiesFor(learnAimRef)).Returns(validities);
            ddValidityMock.Setup(d => d.Derive(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>())).Returns("ADULT_SKILLS");
            ddCategoryMock.Setup(x => x.Derive(learningDelivery)).Returns(41);
            larsDataServiceMock.Setup(ds => ds.GetCategoriesFor(learnAimRef)).Returns(categories);

            NewRule(larsDataServiceMock.Object, ddValidityMock.Object, ddCategoryMock.Object).ConditionMet(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>()).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_ValidityFail_CategoryPasses()
        {
            var learnAimRef = "learnAimRef";
            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = learnAimRef,
                FundModel = 35,
                LearnStartDate = new DateTime(2020, 08, 31)
            };

            var validities = new List<LARSValidity>
            {
                new LARSValidity
                {
                    LearnAimRef = learnAimRef,
                    ValidityCategory = "ADULT_SKILLS",
                    StartDate = new DateTime(2019, 8, 1),
                    LastNewStartDate = new DateTime(2019, 10, 1),
                    EndDate = new DateTime(2020, 7, 31),
                }
            };
            var categories = new List<LearningDeliveryCategory>
            {
                new LearningDeliveryCategory
                {
                    LearnAimRef = learnAimRef,
                    CategoryRef = 41,
                    EffectiveFrom = new DateTime(2020, 8, 1),
                    EffectiveTo = new DateTime(2021, 7, 31),
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var ddValidityMock = new Mock<IDerivedData_ValidityCategory_01>();
            var ddCategoryMock = new Mock<IDerivedData_CategoryRef_01>();

            larsDataServiceMock.Setup(ds => ds.GetValiditiesFor(learnAimRef)).Returns(validities);
            ddValidityMock.Setup(d => d.Derive(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>())).Returns("ADULT_SKILLS");
            ddCategoryMock.Setup(x => x.Derive(learningDelivery)).Returns(41);
            larsDataServiceMock.Setup(ds => ds.GetCategoriesFor(learnAimRef)).Returns(categories);

            NewRule(larsDataServiceMock.Object, ddValidityMock.Object, ddCategoryMock.Object).ConditionMet(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>()).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learnAimRef = "learnAimRef";
            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = learnAimRef,
                FundModel = 35,
                LearnStartDate = new DateTime(2019, 12, 31)
            };

            var validities = new List<LARSValidity>
            {
                new LARSValidity
                {
                    LearnAimRef = learnAimRef,
                    ValidityCategory = "ADULT_SKILLS",
                    StartDate = new DateTime(2019, 8, 1),
                    LastNewStartDate = new DateTime(2019, 10, 1),
                    EndDate = new DateTime(2020, 7, 31),
                }
            };

            var larsCategories = new List<LearningDeliveryCategory>
            {
                new LearningDeliveryCategory
                {
                    LearnAimRef = learnAimRef,
                    CategoryRef = 50,
                    EffectiveFrom = new DateTime(2019, 8, 1),
                    EffectiveTo = new DateTime(2020, 7, 31)
                }
            };

            var learner = new TestLearner
            {
                LearningDeliveries = new TestLearningDelivery[] { learningDelivery }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var ddValidityMock = new Mock<IDerivedData_ValidityCategory_01>();
            var ddCategoryMock = new Mock<IDerivedData_CategoryRef_01>();

            larsDataServiceMock.Setup(ds => ds.GetValiditiesFor(learnAimRef)).Returns(validities);
            larsDataServiceMock.Setup(ds => ds.GetCategoriesFor(learnAimRef)).Returns(larsCategories);
            ddValidityMock.Setup(d => d.Derive(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>())).Returns("ADULT_SKILLS");
            ddCategoryMock.Setup(x => x.Derive(learningDelivery)).Returns(41);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(larsDataServiceMock.Object, ddValidityMock.Object, ddCategoryMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_ValidityPasses()
        {
            var learnAimRef = "learnAimRef";
            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = learnAimRef,
                FundModel = 35,
                LearnStartDate = new DateTime(2019, 08, 31)
            };

            var validities = new List<LARSValidity>
            {
                new LARSValidity
                {
                    LearnAimRef = learnAimRef,
                    ValidityCategory = "ADULT_SKILLS",
                    StartDate = new DateTime(2019, 8, 1),
                    LastNewStartDate = new DateTime(2019, 10, 1),
                    EndDate = new DateTime(2020, 7, 31),
                }
            };

            var larsCategories = new List<LearningDeliveryCategory>
            {
                new LearningDeliveryCategory
                {
                    LearnAimRef = learnAimRef,
                    CategoryRef = 41,
                    EffectiveFrom = new DateTime(2019, 8, 1),
                    EffectiveTo = new DateTime(2020, 7, 31)
                }
            };

            var learner = new TestLearner
            {
                LearningDeliveries = new TestLearningDelivery[] { learningDelivery }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var ddValidityMock = new Mock<IDerivedData_ValidityCategory_01>();
            var ddCategoryMock = new Mock<IDerivedData_CategoryRef_01>();

            larsDataServiceMock.Setup(ds => ds.GetValiditiesFor(learnAimRef)).Returns(validities);
            larsDataServiceMock.Setup(ds => ds.GetCategoriesFor(learnAimRef)).Returns(larsCategories);
            ddValidityMock.Setup(d => d.Derive(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>())).Returns("ADULT_SKILLS");
            ddCategoryMock.Setup(x => x.Derive(learningDelivery)).Returns(41);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(larsDataServiceMock.Object, ddValidityMock.Object, ddCategoryMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_ValidityFail_CategoryPass()
        {
            var learnAimRef = "learnAimRef";
            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = learnAimRef,
                FundModel = 35,
                LearnStartDate = new DateTime(2019, 12, 31)
            };

            var validities = new List<LARSValidity>
            {
                new LARSValidity
                {
                    LearnAimRef = learnAimRef,
                    ValidityCategory = "ANY",
                    StartDate = new DateTime(2019, 8, 1),
                    LastNewStartDate = new DateTime(2019, 10, 1),
                    EndDate = new DateTime(2020, 7, 31),
                }
            };

            var larsCategories = new List<LearningDeliveryCategory>
            {
                new LearningDeliveryCategory
                {
                    LearnAimRef = learnAimRef,
                    CategoryRef = 41,
                    EffectiveFrom = new DateTime(2019, 8, 1),
                    EffectiveTo = new DateTime(2020, 7, 31)
                }
            };

            var learner = new TestLearner
            {
                LearningDeliveries = new TestLearningDelivery[] { learningDelivery }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var ddValidityMock = new Mock<IDerivedData_ValidityCategory_01>();
            var ddCategoryMock = new Mock<IDerivedData_CategoryRef_01>();

            larsDataServiceMock.Setup(ds => ds.GetValiditiesFor(learnAimRef)).Returns(validities);
            larsDataServiceMock.Setup(ds => ds.GetCategoriesFor(learnAimRef)).Returns(larsCategories);
            ddValidityMock.Setup(d => d.Derive(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>())).Returns("ADULT_SKILLS");
            ddCategoryMock.Setup(x => x.Derive(learningDelivery)).Returns(41);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(larsDataServiceMock.Object, ddValidityMock.Object, ddCategoryMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnStartDate", "01/08/2019")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnAimRef", "LearnAimRef")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2019, 8, 1), "LearnAimRef");

            validationErrorHandlerMock.Verify();
        }

        private LearnAimRef_88Rule NewRule(
            ILARSDataService larsDataService = null,
            IDerivedData_ValidityCategory_01 ddValidityCategory = null,
            IDerivedData_CategoryRef_01 ddCategoryRef = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnAimRef_88Rule(larsDataService, ddValidityCategory, ddCategoryRef, validationErrorHandler);
        }
    }
}
