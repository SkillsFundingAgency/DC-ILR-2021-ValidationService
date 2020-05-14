using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.PlanLearnHours;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.PlanLearnHours
{
    public class PlanLearnHours_02RuleTests : AbstractRuleTests<PlanLearnHours_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("PlanLearnHours_02");
        }

        [Fact]
        public void PlanLearnHoursConditionMet_True()
        {
            NewRule().PlanLearnHoursConditionMet(0).Should().BeTrue();
        }

        [Fact]
        public void PlanLearnHoursConditionMet_FalseNull()
        {
            NewRule().PlanLearnHoursConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void PlanLearnHoursConditionMet_False()
        {
            NewRule().PlanLearnHoursConditionMet(1).Should().BeFalse();
        }

        [Theory]
        [InlineData(10)]
        [InlineData(25)]
        [InlineData(82)]
        [InlineData(35)]
        [InlineData(36)]
        [InlineData(81)]
        [InlineData(99)]
        public void FundModelConditionMet_True(int fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeTrue();
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(100).Should().BeFalse();
        }

        [Theory]
        [InlineData(0, 10, 0)]
        [InlineData(0, 25, 0)]
        [InlineData(0, 82, 0)]
        [InlineData(0, 35, 0)]
        [InlineData(0, 36, 0)]
        [InlineData(0, 81, 0)]
        [InlineData(0, 99, 0)]
        public void ConditionMet_True(int? planLearnHours, int fundModel, int? progType)
        {
            NewRule().ConditionMet(planLearnHours, fundModel, progType).Should().BeTrue();
        }

        [Theory]
        [InlineData(null, 10, 0)]
        [InlineData(null, 25, 0)]
        [InlineData(null, 82, 0)]
        [InlineData(null, 35, 0)]
        [InlineData(null, 36, 0)]
        [InlineData(null, 81, 0)]
        [InlineData(null, 99, 0)]
        public void ConditionMet_False_NullPlanLearnHours(int? planLearnHours, int fundModel, int? progType)
        {
            NewRule().ConditionMet(planLearnHours, fundModel, progType).Should().BeFalse();
        }

        [Theory]
        [InlineData(1, 10, 0)]
        [InlineData(1, 25, 0)]
        [InlineData(1, 82, 0)]
        [InlineData(1, 35, 0)]
        [InlineData(1, 36, 0)]
        [InlineData(1, 81, 0)]
        [InlineData(1, 99, 0)]
        public void ConditionMet_False_PlanLearnHours(int? planLearnHours, int fundModel, int? progType)
        {
            NewRule().ConditionMet(planLearnHours, fundModel, progType).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_FundModel_False()
        {
            NewRule().ConditionMet(0, 100, 0).Should().BeFalse();
        }

        [Theory]
        [InlineData(0, 25, 31)]
        [InlineData(0, 25, 30)]
        public void ConditionMet_False_Excluded(int? planLearnHours, int fundModel, int? progType)
        {
            NewRule().ConditionMet(planLearnHours, fundModel, progType).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner()
            {
                PlanLearnHoursNullable = 0,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = 10
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learner = new TestLearner()
            {
                PlanLearnHoursNullable = 20,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = 10
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
            int? planLearnHours = 0;

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("PlanLearnHours", planLearnHours)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("FundModel", 10)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(planLearnHours, 10);

            validationErrorHandlerMock.Verify();
        }

        private PlanLearnHours_02Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new PlanLearnHours_02Rule(validationErrorHandler);
        }
    }
}
