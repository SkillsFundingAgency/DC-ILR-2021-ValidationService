using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.Outcome;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.Outcome
{
    public class Outcome_05RuleTests : AbstractRuleTests<Outcome_05Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("Outcome_05");
        }

        [Fact]
        public void OutcomeConditionMet_True()
        {
            NewRule().OutcomeConditionMet(1).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        public void OutcomeConditionMet_False(int? outcome)
        {
            NewRule().OutcomeConditionMet(outcome).Should().BeFalse();
        }

        [Fact]
        public void LearnActEndDateConditionMet_True()
        {
            NewRule().LearnActEndDateConditionMet(null).Should().BeTrue();
        }

        [Fact]
        public void LearnActEndDateConditionMet_False()
        {
            NewRule().LearnActEndDateConditionMet(new DateTime(2018, 01, 01)).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            NewRule().ConditionMet(1, null).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_Outcome()
        {
            NewRule().ConditionMet(0, null).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_LearnActEndDate()
        {
            NewRule().ConditionMet(1, new DateTime(2018, 01, 01)).Should().BeFalse();
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
                        OutcomeNullable = 1,
                        LearnActEndDateNullable = null
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
                        OutcomeNullable = 1,
                        LearnActEndDateNullable = new DateTime(2018, 01, 01)
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

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(1);

            validationErrorHandlerMock.Verify();
        }

        private Outcome_05Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new Outcome_05Rule(validationErrorHandler);
        }
    }
}
