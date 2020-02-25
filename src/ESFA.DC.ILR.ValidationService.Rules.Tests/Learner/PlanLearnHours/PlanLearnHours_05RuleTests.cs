using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Learner.PlanLearnHours;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.PlanLearnHours
{
    public class PlanLearnHours_05RuleTests : AbstractRuleTests<PlanLearnHours_05Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("PlanLearnHours_05");
        }

        [Theory]
        [InlineData(500, 500, false)]
        [InlineData(500, null, false)]
        [InlineData(null, 500, false)]
        [InlineData(2000, 2000, false)]
        [InlineData(2000, 2001, true)]
        [InlineData(4001, null, true)]
        [InlineData(null, 4001, true)]
        [InlineData(4001, 0, true)]
        public void PlanLearnHoursMeetsExpectation(int? planLearnHours, int? planEEPHours, bool expectation)
        {
            NewRule().ConditionMet(planLearnHours, planEEPHours).Should().Be(expectation);
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner()
            {
                PlanLearnHoursNullable = 2000,
                PlanEEPHoursNullable = 3000
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
                PlanEEPHoursNullable = 500,
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

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.PlanLearnHours, planLearnHours)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.PlanEEPHours, planEEPHours)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(planLearnHours, planEEPHours);

            validationErrorHandlerMock.Verify();
        }

        private PlanLearnHours_05Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new PlanLearnHours_05Rule(validationErrorHandler);
        }
    }
}
