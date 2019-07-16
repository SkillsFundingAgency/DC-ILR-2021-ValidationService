using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.Outcome;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.Outcome
{
    public class Outcome_09RuleTests : AbstractRuleTests<Outcome_09Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("Outcome_09");
        }

        [Fact]
        public void OutcomeConditionMet_True()
        {
            var outcome = 8;

            NewRule().OutcomeConditionMet(outcome).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        public void OutcomeConditionMet_False(int? outcome)
        {
            NewRule().OutcomeConditionMet(outcome).Should().BeFalse();
        }

        [Fact]
        public void CompStatusConditionMet_True()
        {
            var compStatus = 1;

            NewRule().CompStatusConditionMet(compStatus).Should().BeTrue();
        }

        [Fact]
        public void CompStatusConditionMet_False()
        {
            var compStatus = 2;

            NewRule().CompStatusConditionMet(compStatus).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var outcome = 8;
            var compStatus = 1;
            var fundModel = 81;
            var progType = 24;

            NewRule().ConditionMet(outcome, compStatus, fundModel, progType).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_FalseNullOutcome()
        {
            int? outcome = null;
            var compStatus = 1;
            var fundModel = 81;
            var progType = 24;

            NewRule().ConditionMet(outcome, compStatus, fundModel, progType).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseCompStatus()
        {
            var outcome = 8;
            var compStatus = 2;
            var fundModel = 81;
            var progType = 24;

            NewRule().ConditionMet(outcome, compStatus, fundModel, progType).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseFundModel()
        {
            var outcome = 8;
            var compStatus = 1;
            var fundModel = 36;
            var progType = 24;

            NewRule().ConditionMet(outcome, compStatus, fundModel, progType).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseProgType()
        {
            var outcome = 8;
            var compStatus = 1;
            var fundModel = 81;
            var progType = 25;

            NewRule().ConditionMet(outcome, compStatus, fundModel, progType).Should().BeFalse();
        }

        [Fact]
        public void FundModelConditionMet_Pass()
        {
            var fundModel = 81;
            NewRule().FundModelConditionMet(fundModel).Should().BeTrue();
        }

        [Fact]
        public void FundModelConditionMet_Fails()
        {
            var fundModel = 36;
            NewRule().FundModelConditionMet(fundModel).Should().BeFalse();
        }

        [Fact]
        public void ProgTypeConditionMet_Pass()
        {
            var progType = 24;
            NewRule().ProgTypeConditionMet(progType).Should().BeTrue();
        }

        [Fact]
        public void ProgTypeConditionMet_Fails()
        {
            var progType = 25;
            NewRule().ProgTypeConditionMet(progType).Should().BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        OutcomeNullable = 8,
                        CompStatus = 1,
                        ProgTypeNullable = 24,
                        FundModel = 81
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError()
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        OutcomeNullable = 8,
                        CompStatus = 2,
                        ProgTypeNullable = 25,
                        FundModel = 36
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("Outcome", 1)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("CompStatus", 2)).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(1, 2);

            validationErrorHandlerMock.Verify();
        }

        private Outcome_09Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new Outcome_09Rule(validationErrorHandler);
        }
    }
}
