using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.ProgType;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.ProgType
{
    public class ProgType_15RuleTests : AbstractRuleTests<ProgType_15Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("ProgType_15");
        }

        [Theory]
        [InlineData("ZWRKX003")]
        [InlineData("zwrkx003")]
        public void ConditionMet_True(string learnAimRef)
        {
            NewRule().ConditionMet(learnAimRef, 30).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_LearnAimRef()
        {
            NewRule().ConditionMet("11111111", 30).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_ProgType()
        {
            NewRule().ConditionMet("ZWRKX003", 31).Should().BeFalse();
        }

        [Theory]
        [InlineData("zwrkx003")]
        [InlineData("ZWRKX003")]
        public void Validate_Error(string learnAimRef)
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        ProgTypeNullable = 30
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "111111111",
                        ProgTypeNullable = 31
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
            var learnAimRef = "Z0007834";
            var progType = 31;

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnAimRef", learnAimRef)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("ProgType", progType)).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(learnAimRef, progType);

            validationErrorHandlerMock.Verify();
        }

        private ProgType_15Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new ProgType_15Rule(validationErrorHandler);
        }
    }
}
