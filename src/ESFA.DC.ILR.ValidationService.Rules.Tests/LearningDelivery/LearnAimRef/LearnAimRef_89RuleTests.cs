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
        public void LarsConditionMet_True()
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

            NewRule(larsDataServiceMock.Object).LarsConditionMet("ADULT_SKILLS", learnAimRef, new DateTime(2019, 7, 31)).Should().BeTrue();
        }

        [Fact]
        public void LarsConditionMet_False()
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

            NewRule(larsDataServiceMock.Object).LarsConditionMet("ADULT_SKILLS", learnAimRef, new DateTime(2019, 7, 31)).Should().BeFalse();
        }

        [Fact]
        public void LarsConditionMet_False_NullValidities()
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

            NewRule(larsDataServiceMock.Object).LarsConditionMet("ANY", learnAimRef, new DateTime(2019, 7, 31)).Should().BeFalse();
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
            var ddMock = new Mock<IDerivedData_ValidityCategory>();

            larsDataServiceMock.Setup(ds => ds.GetValiditiesFor(learnAimRef)).Returns(validities);
            ddMock.Setup(d => d.Derive(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>())).Returns("ADULT_SKILLS");

            NewRule(larsDataServiceMock.Object, ddMock.Object).ConditionMet(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>(), new DateTime(2019, 7, 31)).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_NoMatchingValidity()
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
            var ddMock = new Mock<IDerivedData_ValidityCategory>();

            larsDataServiceMock.Setup(ds => ds.GetValiditiesFor(learnAimRef)).Returns(validities);
            ddMock.Setup(d => d.Derive(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>())).Returns("ANY");

            NewRule(larsDataServiceMock.Object, ddMock.Object).ConditionMet(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>(), new DateTime(2019, 7, 31)).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_NullCategory()
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

            string ddMockResult = null;

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var ddMock = new Mock<IDerivedData_ValidityCategory>();

            larsDataServiceMock.Setup(ds => ds.GetValiditiesFor(learnAimRef)).Returns(validities);
            ddMock.Setup(d => d.Derive(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>())).Returns(ddMockResult);

            NewRule(larsDataServiceMock.Object, ddMock.Object).ConditionMet(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>(), new DateTime(2019, 7, 31)).Should().BeFalse();
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
            var ddMock = new Mock<IDerivedData_ValidityCategory>();

            larsDataServiceMock.Setup(ds => ds.GetValiditiesFor(learnAimRef)).Returns(validities);
            ddMock.Setup(d => d.Derive(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>())).Returns("ADULT_SKILLS");

            NewRule(larsDataServiceMock.Object, ddMock.Object).ConditionMet(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>(), new DateTime(2019, 7, 31)).Should().BeFalse();
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
            var ddMock = new Mock<IDerivedData_ValidityCategory>();

            larsDataServiceMock.Setup(ds => ds.GetValiditiesFor(learnAimRef)).Returns(validities);
            ddMock.Setup(d => d.Derive(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>())).Returns("ADULT_SKILLS");

            NewRule(larsDataServiceMock.Object, ddMock.Object).ConditionMet(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>(), new DateTime(2019, 7, 31)).Should().BeFalse();
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

            var learner = new TestLearner
            {
                LearningDeliveries = new TestLearningDelivery[] { learningDelivery }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var ddMock = new Mock<IDerivedData_ValidityCategory>();
            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();

            larsDataServiceMock.Setup(ds => ds.GetValiditiesFor(learnAimRef)).Returns(validities);
            ddMock.Setup(d => d.Derive(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>())).Returns("ADULT_SKILLS");
            academicYearDataServiceMock.Setup(p => p.PreviousYearEnd()).Returns(new DateTime(2019, 7, 31));

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(larsDataServiceMock.Object, ddMock.Object, academicYearDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
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
                    EndDate = new DateTime(2019, 8, 30),
                }
            };

            var learner = new TestLearner
            {
                LearningDeliveries = new TestLearningDelivery[] { learningDelivery }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var ddMock = new Mock<IDerivedData_ValidityCategory>();
            var academicYearDataServiceMock = new Mock<IAcademicYearDataService>();

            larsDataServiceMock.Setup(ds => ds.GetValiditiesFor(learnAimRef)).Returns(validities);
            ddMock.Setup(d => d.Derive(learningDelivery, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>())).Returns("ADULT_SKILLS");
            academicYearDataServiceMock.Setup(p => p.PreviousYearEnd()).Returns(new DateTime(2019, 7, 31));

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(larsDataServiceMock.Object, ddMock.Object, academicYearDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
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
            IAcademicYearDataService academicYearDataService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnAimRef_89Rule(larsDataService, ddValidityCategory, academicYearDataService, validationErrorHandler);
        }
    }
}
