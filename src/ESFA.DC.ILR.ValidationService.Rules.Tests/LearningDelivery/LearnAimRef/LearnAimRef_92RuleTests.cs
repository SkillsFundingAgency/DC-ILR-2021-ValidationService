using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Model;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.DateEmpStatApp;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_92RuleTests : AbstractRuleTests<LearnAimRef_92Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnAimRef_92");
        }

        [Fact]
        public void LearnActEndDateCondition_True_BeforeStart()
        {
            NewRule().LearnActEndDateCondition(new DateTime(2020, 7, 31)).Should().BeTrue();
        }

        [Fact]
        public void LearnActEndDateCondition_False()
        {
            NewRule().LearnActEndDateCondition(new DateTime(2020, 8, 31)).Should().BeFalse();
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
        public void LarsValidityConditionMet_True_NoMatchingValidityCategory()
        {
            var learnAimRef = "learnAimRef";
            var validities = new List<LARSValidity>();

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ds => ds.GetValiditiesFor(learnAimRef)).Returns(validities);

            NewRule(larsDataServiceMock.Object).LarsValidityConditionMet("ADULT_SKILLS", learnAimRef, new DateTime(2019, 12, 31)).Should().BeTrue();
        }

        [Fact]
        public void LarsValidityConditionMet_True_AfterEndDate()
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

            NewRule(larsDataServiceMock.Object).LarsValidityConditionMet("ADULT_SKILLS", learnAimRef, new DateTime(2020, 8, 1)).Should().BeTrue();
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

            NewRule(larsDataServiceMock.Object).LarsCategoryConditionMet(41, learnAimRef, new DateTime(2020, 8, 1)).Should().BeTrue();
        }

        [Fact]
        public void LarsCategoryConditionMet_False_PrepDateBefore()
        {
            var learnAimRef = "learnAimRef";
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

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(ds => ds.GetCategoriesFor(learnAimRef)).Returns(categories);

            NewRule(larsDataServiceMock.Object).LarsCategoryConditionMet(41, learnAimRef, new DateTime(2020, 7, 1)).Should().BeFalse();
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

        [Theory]
        [InlineData(null, false, false)]
        [InlineData("string", false, false)]
        [InlineData("string", true, true)]
        [InlineData(null, true, false)]
        public void TriggerOnValidityCategory(string validityCategory, bool validityCheck, bool expectation)
        {
            NewRule().TriggerOnValidityCategory(validityCategory, validityCheck).Should().Be(expectation);
        }

        [Theory]
        [InlineData(null, false, true)]
        [InlineData(null, true, true)]
        [InlineData(10, true, true)]
        [InlineData(10, false, false)]
        public void TriggerOnCategoryRef(int? categoryRef, bool categoryRefCheck, bool expectation)
        {
            NewRule().TriggerOnCategoryRef(categoryRef, categoryRefCheck).Should().Be(expectation);
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learnAimRef = "learnAimRef";
            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = learnAimRef,
                LearnActEndDateNullable = new DateTime(2019, 12, 31)
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
                    CategoryRef = 50,
                    EffectiveFrom = new DateTime(2020, 1, 1),
                    EffectiveTo = new DateTime(2020, 7, 31),
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var ddValidityMock = new Mock<IDerivedData_ValidityCategory_02>();
            var ddCategoryMock = new Mock<IDerivedData_CategoryRef_02>();
            var fileDataServiceMock = new Mock<IFileDataService>();

            fileDataServiceMock.Setup(ds => ds.FilePreparationDate()).Returns(new DateTime(2020, 8, 1));
            larsDataServiceMock.Setup(ds => ds.GetValiditiesFor(learnAimRef)).Returns(validities);
            ddValidityMock.Setup(d => d.Derive(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>())).Returns("ANY");
            ddCategoryMock.Setup(x => x.Derive(learningDelivery)).Returns(41);
            larsDataServiceMock.Setup(ds => ds.GetCategoriesFor(learnAimRef)).Returns(categories);

            NewRule(larsDataServiceMock.Object, fileDataServiceMock.Object, ddValidityMock.Object, ddCategoryMock.Object).ConditionMet(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>(), new DateTime(2019, 12, 31)).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_True_NoValidityMatch_CategoryRefDates()
        {
            var learnAimRef = "learnAimRef";
            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = learnAimRef,
                LearnActEndDateNullable = new DateTime(2020, 8, 1)
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
                    EffectiveFrom = new DateTime(2020, 1, 1),
                    EffectiveTo = new DateTime(2020, 7, 31),
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var ddValidityMock = new Mock<IDerivedData_ValidityCategory_02>();
            var ddCategoryMock = new Mock<IDerivedData_CategoryRef_02>();
            var fileDataServiceMock = new Mock<IFileDataService>();

            fileDataServiceMock.Setup(ds => ds.FilePreparationDate()).Returns(new DateTime(2020, 8, 1));
            larsDataServiceMock.Setup(ds => ds.GetValiditiesFor(learnAimRef)).Returns(validities);
            ddValidityMock.Setup(d => d.Derive(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>())).Returns("ADULT_SKILLS");
            ddCategoryMock.Setup(x => x.Derive(learningDelivery)).Returns(41);
            larsDataServiceMock.Setup(ds => ds.GetCategoriesFor(learnAimRef)).Returns(categories);

            NewRule(larsDataServiceMock.Object, fileDataServiceMock.Object, ddValidityMock.Object, ddCategoryMock.Object).ConditionMet(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>(), new DateTime(2020, 08, 1)).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_ValidityPasses()
        {
            var learnAimRef = "learnAimRef";
            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = learnAimRef,
                LearnActEndDateNullable = new DateTime(2019, 12, 31)
            };

            var validities = new List<LARSValidity>
            {
                new LARSValidity
                {
                    LearnAimRef = learnAimRef,
                    ValidityCategory = "ADULT_SKILLS",
                    StartDate = new DateTime(2019, 8, 1),
                    LastNewStartDate = new DateTime(2019, 12, 31),
                    EndDate = new DateTime(2020, 8, 31),
                }
            };

            var categories = new List<LearningDeliveryCategory>
            {
                new LearningDeliveryCategory
                {
                    LearnAimRef = learnAimRef,
                    CategoryRef = 41,
                    EffectiveFrom = new DateTime(2019, 1, 1),
                    EffectiveTo = new DateTime(2020, 7, 31),
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var ddValidityMock = new Mock<IDerivedData_ValidityCategory_02>();
            var ddCategoryMock = new Mock<IDerivedData_CategoryRef_02>();
            var fileDataServiceMock = new Mock<IFileDataService>();

            fileDataServiceMock.Setup(ds => ds.FilePreparationDate()).Returns(new DateTime(2020, 8, 1));
            larsDataServiceMock.Setup(ds => ds.GetValiditiesFor(learnAimRef)).Returns(validities);
            ddValidityMock.Setup(d => d.Derive(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>())).Returns("ADULT_SKILLS");
            ddCategoryMock.Setup(x => x.Derive(learningDelivery)).Returns(50);
            larsDataServiceMock.Setup(ds => ds.GetCategoriesFor(learnAimRef)).Returns(categories);

            NewRule(larsDataServiceMock.Object, fileDataServiceMock.Object, ddValidityMock.Object, ddCategoryMock.Object).ConditionMet(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>(), new DateTime(2019, 12, 31)).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var learnAimRef = "learnAimRef";
            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = learnAimRef,
                FundModel = 35,
                LearnActEndDateNullable = new DateTime(2019, 08, 31)
            };

            var validities = new List<LARSValidity>
            {
                new LARSValidity
                {
                    LearnAimRef = learnAimRef,
                    ValidityCategory = "ADULT_SKILLS",
                    StartDate = new DateTime(2019, 8, 1),
                    LastNewStartDate = new DateTime(2019, 10, 1),
                    EndDate = new DateTime(2020, 8, 31),
                }
            };
            var categories = new List<LearningDeliveryCategory>
            {
                new LearningDeliveryCategory
                {
                    LearnAimRef = learnAimRef,
                    CategoryRef = 41,
                    EffectiveFrom = new DateTime(2019, 8, 1),
                    EffectiveTo = new DateTime(2020, 8, 31),
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var ddValidityMock = new Mock<IDerivedData_ValidityCategory_02>();
            var ddCategoryMock = new Mock<IDerivedData_CategoryRef_02>();
            var fileDataServiceMock = new Mock<IFileDataService>();

            fileDataServiceMock.Setup(ds => ds.FilePreparationDate()).Returns(new DateTime(2020, 8, 1));
            larsDataServiceMock.Setup(ds => ds.GetValiditiesFor(learnAimRef)).Returns(validities);
            ddValidityMock.Setup(d => d.Derive(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>())).Returns("ADULT_SKILLS");
            ddCategoryMock.Setup(x => x.Derive(learningDelivery)).Returns(41);
            larsDataServiceMock.Setup(ds => ds.GetCategoriesFor(learnAimRef)).Returns(categories);

            NewRule(larsDataServiceMock.Object, fileDataServiceMock.Object, ddValidityMock.Object, ddCategoryMock.Object).ConditionMet(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>(), new DateTime(2019, 08, 31)).Should().BeFalse();
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnActEndDate", "01/08/2019")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnAimRef", "LearnAimRef")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2019, 8, 1), "LearnAimRef");

            validationErrorHandlerMock.Verify();
        }

        [Fact]
        public void Validate_NoError()
        {
            var learnAimRef = "learnAimRef";
            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = learnAimRef,
                FundModel = 35,
                LearnActEndDateNullable = new DateTime(2020, 08, 31)
            };

            var validities = new List<LARSValidity>
            {
                new LARSValidity
                {
                    LearnAimRef = learnAimRef,
                    ValidityCategory = "ADULT_SKILLS",
                    StartDate = new DateTime(2019, 8, 1),
                    LastNewStartDate = new DateTime(2019, 10, 1),
                    EndDate = new DateTime(2020, 8, 31),
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
            var ddValidityMock = new Mock<IDerivedData_ValidityCategory_02>();
            var ddCategoryMock = new Mock<IDerivedData_CategoryRef_02>();
            var fileDataServiceMock = new Mock<IFileDataService>();

            larsDataServiceMock.Setup(ds => ds.GetValiditiesFor(learnAimRef)).Returns(validities);
            larsDataServiceMock.Setup(ds => ds.GetCategoriesFor(learnAimRef)).Returns(larsCategories);
            ddValidityMock.Setup(d => d.Derive(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>())).Returns("ADULT_SKILLS");
            ddCategoryMock.Setup(x => x.Derive(learningDelivery)).Returns(41);
            fileDataServiceMock.Setup(ds => ds.FilePreparationDate()).Returns(new DateTime(2020, 8, 1));

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(larsDataServiceMock.Object, fileDataServiceMock.Object, ddValidityMock.Object, ddCategoryMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_NoLearnActEndDate()
        {
            var learnAimRef = "learnAimRef";
            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = learnAimRef,
                FundModel = 35,
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
            var ddValidityMock = new Mock<IDerivedData_ValidityCategory_02>();
            var ddCategoryMock = new Mock<IDerivedData_CategoryRef_02>();
            var fileDataServiceMock = new Mock<IFileDataService>();

            larsDataServiceMock.Setup(ds => ds.GetValiditiesFor(learnAimRef)).Returns(validities);
            larsDataServiceMock.Setup(ds => ds.GetCategoriesFor(learnAimRef)).Returns(larsCategories);
            ddValidityMock.Setup(d => d.Derive(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>())).Returns("ADULT_SKILLS");
            ddCategoryMock.Setup(x => x.Derive(learningDelivery)).Returns(41);
            fileDataServiceMock.Setup(ds => ds.FilePreparationDate()).Returns(new DateTime(2020, 7, 1));

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(larsDataServiceMock.Object, fileDataServiceMock.Object, ddValidityMock.Object, ddCategoryMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_NoLearningDeliveries()
        {
            var learner = new TestLearner
            {
                LearningDeliveries = null
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(null, null, null, null, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        private LearnAimRef_92Rule NewRule(
            ILARSDataService larsDataService = null,
            IFileDataService fileDataService = null,
            IDerivedData_ValidityCategory_02 ddValidityCategory = null,
            IDerivedData_CategoryRef_02 ddCategoryRef = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            var academicYearDataService = new Mock<IAcademicYearDataService>();
            academicYearDataService.Setup(x => x.Start()).Returns(new DateTime(2020, 8, 1));

            return new LearnAimRef_92Rule(academicYearDataService.Object, larsDataService, fileDataService, ddValidityCategory, ddCategoryRef, validationErrorHandler);
        }
    }
}
