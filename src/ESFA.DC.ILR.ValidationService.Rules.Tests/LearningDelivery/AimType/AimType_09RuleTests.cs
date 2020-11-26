using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AimType;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.AimType
{
    public class AimType_09RuleTests : AbstractRuleTests<AimType_09Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("AimType_09");
        }

        [Theory]
        [InlineData("", 1)]
        [InlineData(null, 1)]
        [InlineData("ABCD", 1)]
        [InlineData("ZTPR", 5)]
        public void ConditionMet_False(string learnAimRef, int aimType)
        {
            NewRule().ConditionMet(learnAimRef, aimType).Should().BeFalse();
        }

        [Theory]
        [InlineData("ZTPR")]
        [InlineData("ztpr")]
        [InlineData("ZTpr")]
        [InlineData("ztpr1234")]
        [InlineData("ZTPR1234")]
        public void ConditionMet_True(string learnAimRef)
        {
            NewRule().ConditionMet(learnAimRef, 1).Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        LearnAimRef = "ZTPR1234"
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
                        AimType = 5,
                        LearnAimRef = "ABCD1234"
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

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnAimRef", "ztpr1234")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("AimType", 5)).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters("ztpr1234", 5);

            validationErrorHandlerMock.Verify();
        }

        private AimType_09Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new AimType_09Rule(validationErrorHandler);
        }
    }
}
