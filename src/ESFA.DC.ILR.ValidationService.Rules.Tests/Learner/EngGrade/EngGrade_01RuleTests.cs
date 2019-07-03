using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.EngGrade;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.EngGrade
{
    public class EngGrade_01RuleTests : AbstractRuleTests<EngGrade_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("EngGrade_01");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void EngGradeConditionMet_True(string engGrade)
        {
            NewRule().EngGradeConditionMet(engGrade).Should().BeTrue();
        }

        [Fact]
        public void EngGradeConditionMet_False()
        {
            var engGrade = "A";

            NewRule().EngGradeConditionMet(engGrade).Should().BeFalse();
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
        [InlineData(" ", 25)]
        public void ValidateError(string engGrade, int fundModel)
        {
            var learner = new TestLearner
            {
                EngGrade = engGrade,
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
        [InlineData(null, 1)]
        [InlineData("", 1)]
        [InlineData(" ", 1)]
        public void ValidateNoError(string engGrade, int fundModel)
        {
            var learner = new TestLearner
            {
                EngGrade = engGrade,
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

        private EngGrade_01Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new EngGrade_01Rule(validationErrorHandler);
        }
    }
}
