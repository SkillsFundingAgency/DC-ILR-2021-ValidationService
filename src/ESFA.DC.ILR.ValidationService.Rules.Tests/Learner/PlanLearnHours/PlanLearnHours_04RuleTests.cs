using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.PlanLearnHours;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.PlanLearnHours
{
    public class PlanLearnHours_04RuleTests : AbstractRuleTests<PlanLearnHours_04Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("PlanLearnHours_04");
        }

        [Fact]
        public void PlanLearnHoursValue_Zero()
        {
            NewRule().PlanLearnHoursValue(0).Should().Be(0);
        }

        [Fact]
        public void PlanLearnHoursValue_Null()
        {
            NewRule().PlanLearnHoursValue(null).Should().Be(0);
        }

        [Fact]
        public void PlanLearnHoursValue_Value()
        {
            NewRule().PlanLearnHoursValue(10).Should().Be(10);
        }

        [Fact]
        public void PlanEEPHoursValue_Zero()
        {
            NewRule().PlanEEPHoursValue(0).Should().Be(0);
        }

        [Fact]
        public void PlanEEPHoursValue_Null()
        {
            NewRule().PlanEEPHoursValue(null).Should().Be(0);
        }

        [Fact]
        public void PlanEEPHoursValue_Value()
        {
            NewRule().PlanEEPHoursValue(10).Should().Be(10);
        }

        [Fact]
        public void ConditionMet_True()
        {
            NewRule().ConditionMet(500, 501).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_True_PlanLearnHours()
        {
            NewRule().ConditionMet(1001, 0).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_True_PlanEEPHours()
        {
            NewRule().ConditionMet(0, 1001).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            NewRule().ConditionMet(500, 500).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_NullPlanLearnHours()
        {
            NewRule().ConditionMet(null, 500).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_NullPlanEEPHours()
        {
            NewRule().ConditionMet(500, null).Should().BeFalse();
        }

        [Theory]
        [InlineData(25, 31)]
        [InlineData(82, null)]
        public void ConditionMet_False_Excluded(int fundModel, int? progType)
        {
            NewRule().Excluded(fundModel, progType).Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner()
            {
                PlanLearnHoursNullable = 500,
                PlanEEPHoursNullable = 501,
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = 1,
                        FundModel = 25
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
                PlanLearnHoursNullable = 500,
                PlanEEPHoursNullable = null,
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = 1,
                        FundModel = 25
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
            int? planEEPHours = 0;

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("PlanLearnHours", planLearnHours)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("PlanEEPHours", planEEPHours)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(planLearnHours, planEEPHours);

            validationErrorHandlerMock.Verify();
        }

        private PlanLearnHours_04Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new PlanLearnHours_04Rule(validationErrorHandler);
        }
    }
}
