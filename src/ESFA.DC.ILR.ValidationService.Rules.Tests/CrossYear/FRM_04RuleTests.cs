using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Learner;
using ESFA.DC.ILR.ValidationService.Data.Learner.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.CrossYear;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossYear
{
    public class FRM_04RuleTests : AbstractRuleTests<FRM_04Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("FRM_04");
        }

        [Fact]
        public void LearnActEndDateCondition_True()
        {
            var learnActEndDate = new DateTime(2020, 8, 1);
            var prevLearnActEndDate = new DateTime(2020, 8, 2);

            NewRule().LearnActEndDateCondition(learnActEndDate, prevLearnActEndDate).Should().BeTrue();
        }

        [Fact]
        public void LearnActEndDateCondition_True_Null()
        {
            var prevLearnActEndDate = new DateTime(2020, 8, 2);

            NewRule().LearnActEndDateCondition(null, prevLearnActEndDate).Should().BeTrue();
        }

        [Fact]
        public void LearnActEndDateCondition_False_Matching()
        {
            var learnActEndDate = new DateTime(2020, 8, 1);
            var prevLearnActEndDate = new DateTime(2020, 8, 1);

            NewRule().LearnActEndDateCondition(learnActEndDate, prevLearnActEndDate).Should().BeFalse();
        }

        [Fact]
        public void LearnActEndDateCondition_False_NoPrevDate()
        {
            var learnActEndDate = new DateTime(2020, 8, 1);
            var prevLearnActEndDate = new DateTime(2020, 8, 1);

            NewRule().LearnActEndDateCondition(learnActEndDate, null).Should().BeFalse();
        }

        [Theory]
        [InlineData(25, 1, false)]
        [InlineData(35, 1, false)]
        [InlineData(36, 1, false)]
        [InlineData(81, 25, false)]
        [InlineData(99, 1, true)]
        public void FundedAimCondition_True(int fundModel, int? progType, bool learningDeliveryFamMock)
        {
            var learningDeliveryFams = new List<ILearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = "1",
                    LearnDelFAMType = "ADL"
                }
            };

            var learningDeliveryFamQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, "ADL", "1")).Returns(learningDeliveryFamMock);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFamQSMock.Object).FundedAimCondition(fundModel, progType, learningDeliveryFams).Should().BeTrue();
        }

        [Theory]
        [InlineData(10, 1, false)]
        [InlineData(81, 26, false)]
        [InlineData(81, null, false)]
        [InlineData(99, 25, false)]
        [InlineData(10, 1, true)]
        [InlineData(99, 1, false)]
        public void FundedAimCondition_False(int fundModel, int? progType, bool learningDeliveryFamMock)
        {
            var learningDeliveryFams = new List<ILearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = "1",
                    LearnDelFAMType = "ADL"
                }
            };

            var learningDeliveryFamQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, "ADL", "1")).Returns(learningDeliveryFamMock);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFamQSMock.Object).FundedAimCondition(fundModel, progType, learningDeliveryFams).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learnerReferenceData = new LearnerReferenceData
            {
                LearnAimRef = "LearnAimRef",
                LearnStartDate = new DateTime(2020, 8, 1),
                LearnActEndDate = new DateTime(2020, 7, 1),
                FundModel = 35,
                ProgTypeNullable = 1,
                FworkCodeNullable = 1,
                PwayCodeNullable = 1,
                StdCodeNullable = 1
            };

            var learningDeliveryFams = new List<ILearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = "1",
                    LearnDelFAMType = "ADL"
                }
            };

            var learningDelivery = new TestLearningDelivery
            {
                FundModel = 35,
                LearnActEndDateNullable = new DateTime(2020, 8, 1),
                LearningDeliveryFAMs = learningDeliveryFams
            };

            var learningDeliveries = new List<ILearningDelivery>
            {
                learningDelivery
            };

            var learner = new TestLearner
            {
                LearnRefNumber = "LearnRefNUmber",
                LearningDeliveries = learningDeliveries
            };

            var learningDeliveryFamQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, "ADL", "1")).Returns(false);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFamQSMock.Object).ConditionMet(learningDelivery, learnerReferenceData).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_ActEndDate()
        {
            var learnerReferenceData = new LearnerReferenceData
            {
                LearnAimRef = "LearnAimRef",
                LearnStartDate = new DateTime(2020, 8, 1),
                LearnActEndDate = new DateTime(2020, 7, 1),
                FundModel = 35,
                ProgTypeNullable = 1,
                FworkCodeNullable = 1,
                PwayCodeNullable = 1,
                StdCodeNullable = 1
            };

            var learningDeliveryFams = new List<ILearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = "1",
                    LearnDelFAMType = "ADL"
                }
            };

            var learningDelivery = new TestLearningDelivery
            {
                FundModel = 35,
                LearnActEndDateNullable = new DateTime(2020, 7, 1),
                LearningDeliveryFAMs = learningDeliveryFams
            };

            var learningDeliveries = new List<ILearningDelivery>
            {
                learningDelivery
            };

            var learner = new TestLearner
            {
                LearnRefNumber = "LearnRefNUmber",
                LearningDeliveries = learningDeliveries
            };

            var learningDeliveryFamQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, "ADL", "1")).Returns(false);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFamQSMock.Object).ConditionMet(learningDelivery, learnerReferenceData).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FundedAim()
        {
            var learnerReferenceData = new LearnerReferenceData
            {
                LearnAimRef = "LearnAimRef",
                LearnStartDate = new DateTime(2020, 8, 1),
                LearnActEndDate = new DateTime(2020, 7, 1),
                FundModel = 35,
                ProgTypeNullable = 1,
                FworkCodeNullable = 1,
                PwayCodeNullable = 1,
                StdCodeNullable = 1
            };

            var learningDeliveryFams = new List<ILearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = "1",
                    LearnDelFAMType = "ADL"
                }
            };

            var learningDelivery = new TestLearningDelivery
            {
                FundModel = 10,
                LearnActEndDateNullable = new DateTime(2020, 8, 1),
                LearningDeliveryFAMs = learningDeliveryFams
            };

            var learningDeliveries = new List<ILearningDelivery>
            {
                learningDelivery
            };

            var learner = new TestLearner
            {
                LearnRefNumber = "LearnRefNUmber",
                LearningDeliveries = learningDeliveries
            };

            var learningDeliveryFamQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, "ADL", "1")).Returns(false);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFamQSMock.Object).ConditionMet(learningDelivery, learnerReferenceData).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learnerReferenceData = new LearnerReferenceData
            {
                LearnAimRef = "LearnAimRef",
                LearnStartDate = new DateTime(2020, 8, 1),
                LearnActEndDate = new DateTime(2020, 7, 1),
                FundModel = 35,
                ProgTypeNullable = 1,
                FworkCodeNullable = 1,
                PwayCodeNullable = 1,
                StdCodeNullable = 1
            };

            var learningDeliveryFams = new List<ILearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = "1",
                    LearnDelFAMType = "ADL"
                }
            };

            var learningDelivery = new TestLearningDelivery
            {
                FundModel = 35,
                LearnActEndDateNullable = new DateTime(2020, 8, 1),
                LearningDeliveryFAMs = learningDeliveryFams
            };

            var learningDeliveries = new List<ILearningDelivery>
            {
                learningDelivery
            };

            var learner = new TestLearner
            {
                LearnRefNumber = "LearnRefNUmber",
                LearningDeliveries = learningDeliveries
            };

            var dd39 = new Mock<IDerivedData_39Rule>();
            dd39.Setup(x => x.GetMatchingLearningAimFromPreviousYear(learner, learningDelivery)).Returns(learnerReferenceData);

            var learningDeliveryFamQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, "ADL", "1")).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object, learningDeliveryFamQSMock.Object, dd39.Object).Validate(learner);
                validationErrorHandlerMock.Verify(h => h.BuildErrorMessageParameter(It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(2));
            }
        }

        [Fact]
        public void ValidateNo_NoError()
        {
            var learnerReferenceData = new LearnerReferenceData
            {
                LearnAimRef = "LearnAimRef",
                LearnStartDate = new DateTime(2020, 8, 1),
                LearnActEndDate = new DateTime(2020, 7, 1),
                FundModel = 35,
                ProgTypeNullable = 1,
                FworkCodeNullable = 1,
                PwayCodeNullable = 1,
                StdCodeNullable = 1
            };

            var learningDeliveryFams = new List<ILearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = "1",
                    LearnDelFAMType = "ADL"
                }
            };

            var learningDelivery = new TestLearningDelivery
            {
                FundModel = 35,
                LearnActEndDateNullable = new DateTime(2020, 7, 1),
                LearningDeliveryFAMs = learningDeliveryFams
            };

            var learningDeliveries = new List<ILearningDelivery>
            {
                learningDelivery
            };

            var learner = new TestLearner
            {
                LearnRefNumber = "LearnRefNUmber",
                LearningDeliveries = learningDeliveries
            };

            var dd39 = new Mock<IDerivedData_39Rule>();
            dd39.Setup(x => x.GetMatchingLearningAimFromPreviousYear(learner, learningDelivery)).Returns(learnerReferenceData);

            var learningDeliveryFamQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, "ADL", "1")).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, learningDeliveryFamQSMock.Object, dd39.Object).Validate(learner);
                validationErrorHandlerMock.Verify(h => h.BuildErrorMessageParameter(It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(0));
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("FundModel", 1)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnActEndDate", "01/08/2020")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(1, new DateTime(2020, 8, 1));

            validationErrorHandlerMock.Verify();
        }

        private FRM_04Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            ILearningDeliveryFAMQueryService learningDeliveryFamQueryService = null,
            IDerivedData_39Rule dd39 = null)
        {
            return new FRM_04Rule(validationErrorHandler, learningDeliveryFamQueryService, dd39);
        }
    }
}
