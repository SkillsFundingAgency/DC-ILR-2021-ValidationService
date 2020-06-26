using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_90RuleTests : AbstractRuleTests<LearnAimRef_90Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnAimRef_90");
        }

        [Theory]
        [InlineData("Z0007834")]
        [InlineData("Z0007835")]
        [InlineData("Z0007836")]
        [InlineData("Z0007837")]
        [InlineData("Z0007838")]
        [InlineData("Z0002347")]
        [InlineData("ZWRKX001")]
        [InlineData("ZWRKX002")]
        [InlineData("zwrkx002")]
        public void ConditionMet_True(string learnAimRef)
        {
            NewRule().ConditionMet(learnAimRef, 31).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_LearnAimRef()
        {
            NewRule().ConditionMet("11111111", 31).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_ProgType()
        {
            NewRule().ConditionMet("ZWRKX002", 30).Should().BeFalse();
        }

        [Theory]
        [InlineData("Z0007834")]
        [InlineData("Z0007835")]
        [InlineData("Z0007836")]
        [InlineData("Z0007837")]
        [InlineData("Z0007838")]
        [InlineData("Z0002347")]
        [InlineData("ZWRKX001")]
        [InlineData("ZWRKX002")]
        public void Validate_Error(string learnAimRef)
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        ProgTypeNullable = 31
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

        private LearnAimRef_90Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnAimRef_90Rule(validationErrorHandler);
        }
    }
}
