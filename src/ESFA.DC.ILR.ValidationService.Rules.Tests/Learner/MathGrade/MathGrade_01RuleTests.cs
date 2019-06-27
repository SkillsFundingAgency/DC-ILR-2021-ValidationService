using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.MathGrade;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.MathGrade
{
    public class MathGrade_01RuleTests : AbstractRuleTests<MathGrade_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("MathGrade_01");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void MathGradeConditionMet_True(string mathGrade)
        {
            NewRule().MathGradeConditionMet(mathGrade).Should().BeTrue();
        }

        [Fact]
        public void MathGradeConditionMet_False()
        {
            var mathGrade = "A";

            NewRule().MathGradeConditionMet(mathGrade).Should().BeFalse();
        }

        [Theory]
        [InlineData(25)]
        public void FundModelConditionMet_True(int fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeTrue();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(100)]
        public void FundModelConditionMet_False(int fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeFalse();
        }

        [Theory]
        [InlineData(null, 25)]
        [InlineData("", 25)]
        [InlineData("  ", 25)]
        public void ValidateError(string mathGrade, int fundModel)
        {
            var learner = new TestLearner
            {
                MathGrade = mathGrade,
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = fundModel
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Theory]
        [InlineData("A", 25)]
        [InlineData("A", 82)]
        [InlineData(null, 1)]
        [InlineData("", 1)]
        [InlineData(" ", 1)]
        public void ValidateNoError(string mathGrade, int fundModel)
        {
            var learner = new TestLearner
            {
                MathGrade = mathGrade,
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = fundModel
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

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("FundModel", 25)).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(25);

            validationErrorHandlerMock.Verify();
        }

        private MathGrade_01Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new MathGrade_01Rule(validationErrorHandler);
        }
    }
}
