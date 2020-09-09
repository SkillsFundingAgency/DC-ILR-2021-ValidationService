using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.FundModel;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.FundModel
{
    public class FundModel_12RuleTests : AbstractRuleTests<FundModel_12Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("FundModel_12");
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            NewRule().FundModelConditionMet(1).Should().BeTrue();
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(35).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMConditionMet_True()
        {
            var learningDeliverFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMCode = "376",
                    LearnDelFAMType = "LDM"
                },
                new TestLearningDeliveryFAM()
            };

            var learningDeliveryFamQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(learningDeliverFams, "LDM", "376")).Returns(true);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFamQSMock.Object).LearningDeliveryFAMConditionMet(learningDeliverFams).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryFAMConditionMet_False()
        {
            var learningDeliverFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMCode = "100",
                    LearnDelFAMType = "LDM"
                },
                new TestLearningDeliveryFAM()
            };

            var learningDeliveryFamQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(learningDeliverFams, "LDM", "376")).Returns(false);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFamQSMock.Object).LearningDeliveryFAMConditionMet(learningDeliverFams).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var fundModel = 1;
            var learningDeliverFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMCode = "376",
                    LearnDelFAMType = "LDM"
                },
                new TestLearningDeliveryFAM()
            };

            var learningDeliveryFamQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(learningDeliverFams, "LDM", "376")).Returns(true);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFamQSMock.Object).ConditionMet(fundModel, learningDeliverFams).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_FAMs()
        {
            var fundModel = 1;
            var learningDeliverFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMCode = "100",
                    LearnDelFAMType = "LDM"
                },
                new TestLearningDeliveryFAM()
            };

            var learningDeliveryFamQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(learningDeliverFams, "LDM", "376")).Returns(false);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFamQSMock.Object).ConditionMet(fundModel, learningDeliverFams).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FundModel()
        {
            var fundModel = 35;
            var learningDeliverFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMCode = "376",
                    LearnDelFAMType = "LDM"
                },
                new TestLearningDeliveryFAM()
            };

            var learningDeliveryFamQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(learningDeliverFams, "LDM", "376")).Returns(true);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFamQSMock.Object).ConditionMet(fundModel, learningDeliverFams).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learningDeliverFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMCode = "376",
                    LearnDelFAMType = "LDM"
                },
                new TestLearningDeliveryFAM()
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 36,
                        LearningDeliveryFAMs = learningDeliverFams
                    }
                }
            };

            var learningDeliveryFamQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(learningDeliverFams, "LDM", "376")).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object, learningDeliveryFamQSMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var learningDeliverFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMCode = "376",
                    LearnDelFAMType = "LDM"
                },
                new TestLearningDeliveryFAM()
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 35,
                        LearningDeliveryFAMs = learningDeliverFams
                    }
                }
            };

            var learningDeliveryFamQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(learningDeliverFams, "LDM", "376")).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, learningDeliveryFamQSMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnDelFAMType", "LDM")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnDelFAMCode", "376")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("FundModel", 1)).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(1);

            validationErrorHandlerMock.Verify();
        }

        private FundModel_12Rule NewRule(IValidationErrorHandler validationErrorHandler = null, ILearningDeliveryFAMQueryService learningDeliveryFamQueryService = null)
        {
            return new FundModel_12Rule(validationErrorHandler, learningDeliveryFamQueryService);
        }
    }
}
