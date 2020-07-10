using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Model;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_89RuleTests : AbstractRuleTests<LearnAimRef_89Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnAimRef_89");
        }

        [Fact]
        public void LarsCategoryRefConditionMet_True()
        {
            var learnAimRef = "learnAimRef";
            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = learnAimRef,
                FundModel = 35,
            };

            var larsCategories = new List<ILARSLearningCategory>
            {
                new LearningDeliveryCategory
                {
                    LearnAimRef = learnAimRef,
                    CategoryRef = 41
                }
            };

            var ddCategoryMock = new Mock<IDerivedData_CategoryRef>();
            ddCategoryMock.Setup(x => x.Derive(learningDelivery)).Returns(41);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(ds => ds.GetCategoriesFor(learnAimRef)).Returns(larsCategories);

            NewRule(larsDataServiceMock.Object, ddCategoryRef: ddCategoryMock.Object).CategoryRefConditionMet(learningDelivery).Should().BeTrue();
        }

        [Fact]
        public void LarsCategoryRefConditionMet_False_FundModel()
        {
            var learnAimRef = "learnAimRef";
            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = learnAimRef,
                FundModel = 25,
            };

            var larsCategories = new List<ILARSLearningCategory>
            {
                new LearningDeliveryCategory
                {
                    LearnAimRef = learnAimRef,
                    CategoryRef = 41
                }
            };

            var ddCategoryMock = new Mock<IDerivedData_CategoryRef>();
            ddCategoryMock.Setup(x => x.Derive(learningDelivery)).Returns((int?)null);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(ds => ds.GetCategoriesFor(learnAimRef)).Returns(larsCategories);

            NewRule(larsDataServiceMock.Object, ddCategoryRef: ddCategoryMock.Object).CategoryRefConditionMet(learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void LarsCategoryRefConditionMet_False_NoLarsMatch()
        {
            var learnAimRef = "learnAimRef";
            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = learnAimRef,
                FundModel = 35,
            };

            var larsCategories = new List<ILARSLearningCategory>
            {
                new LearningDeliveryCategory
                {
                    LearnAimRef = learnAimRef,
                    CategoryRef = 50
                }
            };

            var ddCategoryMock = new Mock<IDerivedData_CategoryRef>();
            ddCategoryMock.Setup(x => x.Derive(learningDelivery)).Returns(41);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(ds => ds.GetCategoriesFor(learnAimRef)).Returns(larsCategories);

            NewRule(larsDataServiceMock.Object, ddCategoryRef: ddCategoryMock.Object).CategoryRefConditionMet(learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void LarsValidityCategoryConditionMet_True()
        {
            var learnAimRef = "learnAimRef";
            var validities = new List<LARSValidity>
            {
                new LARSValidity
                {
                    LearnAimRef = learnAimRef,
                    ValidityCategory = "ADULT_SKILLS",
                    StartDate = new DateTime(2018, 8, 1),
                    EndDate = new DateTime(2019, 6, 30),
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(ds => ds.GetValiditiesFor(learnAimRef)).Returns(validities);

            NewRule(larsDataServiceMock.Object).LarsValidityCategoryConditionMet("ADULT_SKILLS", learnAimRef, new DateTime(2019, 7, 31)).Should().BeTrue();
        }

        [Fact]
        public void LarsValidityCategoryConditionMet_False()
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
                },
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

            NewRule(larsDataServiceMock.Object).LarsValidityCategoryConditionMet("ADULT_SKILLS", learnAimRef, new DateTime(2019, 7, 31)).Should().BeFalse();
        }

        [Fact]
        public void LarsValidityCategoryConditionMet_True_NullValidities()
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
                },
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

            NewRule(larsDataServiceMock.Object).LarsValidityCategoryConditionMet("ANY", learnAimRef, new DateTime(2019, 7, 31)).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learnAimRef = "learnAimRef";
            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = learnAimRef
            };

            var validities = new List<LARSValidity>
            {
                new LARSValidity
                {
                    LearnAimRef = learnAimRef,
                    ValidityCategory = "ADULT_SKILLS",
                    StartDate = new DateTime(2018, 8, 1),
                    EndDate = new DateTime(2019, 6, 30),
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var ddValidityMock = new Mock<IDerivedData_ValidityCategory>();
            var ddCategoryMock = new Mock<IDerivedData_CategoryRef>();

            larsDataServiceMock.Setup(ds => ds.GetValiditiesFor(learnAimRef)).Returns(validities);
            larsDataServiceMock.Setup(ds => ds.GetCategoriesFor(learnAimRef)).Returns(new List<ILARSLearningCategory>());
            ddValidityMock.Setup(d => d.Derive(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>())).Returns("ADULT_SKILLS");
            ddCategoryMock.Setup(x => x.Derive(learningDelivery)).Returns(41);

            NewRule(larsDataServiceMock.Object, ddValidityMock.Object, ddCategoryMock.Object).ConditionMet(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>(), new DateTime(2019, 7, 31)).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_True_NoMatchingValidity()
        {
            var learnAimRef = "learnAimRef";
            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = learnAimRef
            };

            var validities = new List<LARSValidity>
            {
                new LARSValidity
                {
                    LearnAimRef = learnAimRef,
                    ValidityCategory = "ADULT_SKILLS",
                    StartDate = new DateTime(2018, 8, 1),
                    EndDate = new DateTime(2019, 6, 30),
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var ddValidityMock = new Mock<IDerivedData_ValidityCategory>();
            var ddCategoryMock = new Mock<IDerivedData_CategoryRef>();

            larsDataServiceMock.Setup(ds => ds.GetValiditiesFor(learnAimRef)).Returns(validities);
            larsDataServiceMock.Setup(ds => ds.GetCategoriesFor(learnAimRef)).Returns(new List<ILARSLearningCategory>());
            ddValidityMock.Setup(d => d.Derive(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>())).Returns("ANY");
            ddCategoryMock.Setup(x => x.Derive(learningDelivery)).Returns(41);

            NewRule(larsDataServiceMock.Object, ddValidityMock.Object, ddCategoryMock.Object).ConditionMet(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>(), new DateTime(2019, 7, 31)).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_CategoryRefMatch()
        {
            var learnAimRef = "learnAimRef";
            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = learnAimRef
            };

            var validities = new List<LARSValidity>
            {
                new LARSValidity
                {
                    LearnAimRef = learnAimRef,
                    ValidityCategory = "ADULT_SKILLS",
                    StartDate = new DateTime(2018, 8, 1),
                    EndDate = new DateTime(2019, 6, 30),
                }
            };

            var larsCategories = new List<LearningDeliveryCategory>
            {
                new LearningDeliveryCategory { LearnAimRef = learnAimRef, CategoryRef = 41 }
            };

            string ddValidityMockResult = null;

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var ddValidityMock = new Mock<IDerivedData_ValidityCategory>();
            var ddCategoryMock = new Mock<IDerivedData_CategoryRef>();

            larsDataServiceMock.Setup(ds => ds.GetValiditiesFor(learnAimRef)).Returns(validities);
            larsDataServiceMock.Setup(ds => ds.GetCategoriesFor(learnAimRef)).Returns(larsCategories);
            ddValidityMock.Setup(d => d.Derive(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>())).Returns(ddValidityMockResult);
            ddCategoryMock.Setup(x => x.Derive(learningDelivery)).Returns(41);

            NewRule(larsDataServiceMock.Object, ddValidityMock.Object, ddCategoryMock.Object).ConditionMet(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>(), new DateTime(2019, 7, 31)).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_NullValidityCategory()
        {
            var learnAimRef = "learnAimRef";
            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = learnAimRef
            };

            var validities = new List<LARSValidity>
            {
                new LARSValidity
                {
                    LearnAimRef = learnAimRef,
                    ValidityCategory = "ADULT_SKILLS",
                    StartDate = new DateTime(2018, 8, 1),
                    EndDate = new DateTime(2019, 6, 30),
                }
            };

            string ddValidityMockResult = null;

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var ddValidityMock = new Mock<IDerivedData_ValidityCategory>();
            var ddCategoryMock = new Mock<IDerivedData_CategoryRef>();

            larsDataServiceMock.Setup(ds => ds.GetValiditiesFor(learnAimRef)).Returns(validities);
            larsDataServiceMock.Setup(ds => ds.GetCategoriesFor(learnAimRef)).Returns(new List<ILARSLearningCategory>());
            ddValidityMock.Setup(d => d.Derive(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>())).Returns(ddValidityMockResult);
            ddCategoryMock.Setup(x => x.Derive(learningDelivery)).Returns(41);

            NewRule(larsDataServiceMock.Object, ddValidityMock.Object, ddCategoryMock.Object).ConditionMet(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>(), new DateTime(2019, 7, 31)).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_DatesWithinRange()
        {
            var learnAimRef = "learnAimRef";
            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = learnAimRef
            };

            var validities = new List<LARSValidity>
            {
                new LARSValidity
                {
                    LearnAimRef = learnAimRef,
                    ValidityCategory = "ADULT_SKILLS",
                    StartDate = new DateTime(2018, 8, 1),
                    EndDate = new DateTime(2019, 8, 1),
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var ddValidityMock = new Mock<IDerivedData_ValidityCategory>();
            var ddCategoryMock = new Mock<IDerivedData_CategoryRef>();

            larsDataServiceMock.Setup(ds => ds.GetValiditiesFor(learnAimRef)).Returns(validities);
            larsDataServiceMock.Setup(ds => ds.GetCategoriesFor(learnAimRef)).Returns(new List<ILARSLearningCategory>());
            ddValidityMock.Setup(d => d.Derive(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>())).Returns("ADULT_SKILLS");
            ddCategoryMock.Setup(x => x.Derive(learningDelivery)).Returns(41);

            NewRule(larsDataServiceMock.Object, ddValidityMock.Object, ddCategoryMock.Object).ConditionMet(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>(), new DateTime(2019, 7, 31)).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_NoEndDate()
        {
            var learnAimRef = "learnAimRef";
            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = learnAimRef
            };

            var validities = new List<LARSValidity>
            {
                new LARSValidity
                {
                    LearnAimRef = learnAimRef,
                    ValidityCategory = "ADULT_SKILLS",
                    StartDate = new DateTime(2018, 8, 1)
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var ddValidityMock = new Mock<IDerivedData_ValidityCategory>();
            var ddCategoryMock = new Mock<IDerivedData_CategoryRef>();

            larsDataServiceMock.Setup(ds => ds.GetValiditiesFor(learnAimRef)).Returns(validities);
            larsDataServiceMock.Setup(ds => ds.GetCategoriesFor(learnAimRef)).Returns(new List<ILARSLearningCategory>());
            ddValidityMock.Setup(d => d.Derive(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>())).Returns("ADULT_SKILLS");
            ddCategoryMock.Setup(x => x.Derive(learningDelivery)).Returns(41);

            NewRule(larsDataServiceMock.Object, ddValidityMock.Object, ddCategoryMock.Object).ConditionMet(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>(), new DateTime(2019, 7, 31)).Should().BeFalse();
        }

        [Fact]
        public void CategoryRefConditionMet_True()
        {
            var learnAimRef = "learnAimRef";
            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = learnAimRef,
                FundModel = 35
            };

            var larsCategories = new List<LearningDeliveryCategory>
            {
                new LearningDeliveryCategory { LearnAimRef = learnAimRef, CategoryRef = 41 }
            };

            var ddValidityMock = new Mock<IDerivedData_CategoryRef>();
            ddValidityMock.Setup(x => x.Derive(learningDelivery)).Returns(41);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(ds => ds.GetCategoriesFor(learnAimRef)).Returns(larsCategories);

            NewRule(larsDataServiceMock.Object, null, ddValidityMock.Object).CategoryRefConditionMet(learningDelivery).Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            var learnAimRef = "learnAimRef";
            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = learnAimRef
            };

            var validities = new List<LARSValidity>
            {
                new LARSValidity
                {
                    LearnAimRef = learnAimRef,
                    ValidityCategory = "ADULT_SKILLS",
                    StartDate = new DateTime(2018, 8, 1),
                    EndDate = new DateTime(2019, 6, 30),
                }
            };

            var larsCategories = new List<LearningDeliveryCategory>
            {
                new LearningDeliveryCategory { LearnAimRef = learnAimRef, CategoryRef = 50 }
            };

            var learner = new TestLearner
            {
                LearningDeliveries = new TestLearningDelivery[] { learningDelivery }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var ddValidityMock = new Mock<IDerivedData_ValidityCategory>();
            var ddCategoryMock = new Mock<IDerivedData_CategoryRef>();
            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();

            larsDataServiceMock.Setup(ds => ds.GetValiditiesFor(learnAimRef)).Returns(validities);
            larsDataServiceMock.Setup(ds => ds.GetCategoriesFor(learnAimRef)).Returns(larsCategories);
            ddValidityMock.Setup(d => d.Derive(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>())).Returns("ADULT_SKILLS");
            ddCategoryMock.Setup(x => x.Derive(learningDelivery)).Returns(41);
            academicYearDataServiceMock.Setup(p => p.PreviousYearEnd()).Returns(new DateTime(2019, 7, 31));

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(larsDataServiceMock.Object, ddValidityMock.Object, ddCategoryMock.Object, academicYearDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
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
                    StartDate = new DateTime(2018, 8, 1),
                    EndDate = new DateTime(2019, 8, 30),
                }
            };

            var larsCategories = new List<LearningDeliveryCategory>
            {
                new LearningDeliveryCategory { LearnAimRef = learnAimRef, CategoryRef = 50 }
            };

            var learner = new TestLearner
            {
                LearningDeliveries = new TestLearningDelivery[] { learningDelivery }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var ddValidityMock = new Mock<IDerivedData_ValidityCategory>();
            var ddCategoryMock = new Mock<IDerivedData_CategoryRef>();
            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();

            larsDataServiceMock.Setup(ds => ds.GetValiditiesFor(learnAimRef)).Returns(validities);
            larsDataServiceMock.Setup(ds => ds.GetCategoriesFor(learnAimRef)).Returns(larsCategories);
            ddValidityMock.Setup(d => d.Derive(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>())).Returns("ADULT_SKILLS");
            ddCategoryMock.Setup(x => x.Derive(learningDelivery)).Returns(41);
            academicYearDataServiceMock.Setup(p => p.PreviousYearEnd()).Returns(new DateTime(2019, 7, 31));

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(larsDataServiceMock.Object, ddValidityMock.Object, ddCategoryMock.Object, academicYearDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_CategoryRefMatch()
        {
            var learnAimRef = "learnAimRef";
            var learningDelivery = new TestLearningDelivery
            {
                LearnAimRef = learnAimRef,
                FundModel = 35
            };

            var validities = new List<LARSValidity>
            {
                new LARSValidity
                {
                    LearnAimRef = learnAimRef,
                    ValidityCategory = "ADULT_SKILLS",
                    StartDate = new DateTime(2018, 8, 1),
                    EndDate = new DateTime(2019, 8, 30),
                }
            };

            var larsCategories = new List<LearningDeliveryCategory>
            {
                new LearningDeliveryCategory { LearnAimRef = learnAimRef, CategoryRef = 41 }
            };

            var learner = new TestLearner
            {
                LearningDeliveries = new TestLearningDelivery[] { learningDelivery }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var ddValidityMock = new Mock<IDerivedData_ValidityCategory>();
            var ddCategoryMock = new Mock<IDerivedData_CategoryRef>();
            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();

            larsDataServiceMock.Setup(ds => ds.GetValiditiesFor(learnAimRef)).Returns(validities);
            larsDataServiceMock.Setup(ds => ds.GetCategoriesFor(learnAimRef)).Returns(larsCategories);
            ddValidityMock.Setup(d => d.Derive(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>())).Returns("ADULT_SKILLS");
            ddCategoryMock.Setup(x => x.Derive(learningDelivery)).Returns(41);
            academicYearDataServiceMock.Setup(p => p.PreviousYearEnd()).Returns(new DateTime(2019, 7, 31));

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(larsDataServiceMock.Object, ddValidityMock.Object, ddCategoryMock.Object, academicYearDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnAimRef", "LearnAimRef")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters("LearnAimRef");

            validationErrorHandlerMock.Verify();
        }

        private LearnAimRef_89Rule NewRule(
            ILARSDataService larsDataService = null,
            IDerivedData_ValidityCategory ddValidityCategory = null,
            IDerivedData_CategoryRef ddCategoryRef = null,
            IAcademicYearDataService academicYearDataService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnAimRef_89Rule(larsDataService, ddValidityCategory, ddCategoryRef, academicYearDataService, validationErrorHandler);
        }
    }
}
