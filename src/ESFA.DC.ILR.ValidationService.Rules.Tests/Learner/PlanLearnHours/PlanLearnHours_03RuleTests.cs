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
    public class PlanLearnHours_03RuleTests : AbstractRuleTests<PlanLearnHours_03Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("PlanLearnHours_03");
        }

        [Theory]
        [InlineData(5, 10)]
        [InlineData(null, 5)]
        [InlineData(5, null)]
        public void LearnerConditionMet_False(int? planLearnHours, int? planEEPhours)
        {
            NewRule().LearnerConditionMet(planLearnHours, planEEPhours).Should().BeFalse();
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(null, 0)]
        [InlineData(0, null)]
        [InlineData(null, null)]
        public void LearnerConditionMet_True(int? planLearnHours, int? planEEPhours)
        {
            NewRule().LearnerConditionMet(planLearnHours, planEEPhours).Should().BeTrue();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017)]
        public void FundModelConditionMet_False(int fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeFalse();
        }

        [Theory]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships)]
        [InlineData(TypeOfFunding.Other16To19)]
        public void FundModelConditionMet_True(int fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeTrue();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017)]
        public void ConditionMet_False(int fundModel)
        {
            NewRule().ConditionMet(fundModel).Should().BeFalse();
        }

        [Theory]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships)]
        [InlineData(TypeOfFunding.Other16To19)]
        public void ConditionMet_True(int fundModel)
        {
            NewRule().ConditionMet(fundModel).Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner()
            {
                PlanLearnHoursNullable = 0,
                PlanEEPHoursNullable = 0,
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = TypeOfFunding.Other16To19
                    },
                    new TestLearningDelivery()
                    {
                        FundModel = TypeOfFunding.Age16To19ExcludingApprenticeships
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
                PlanLearnHoursNullable = 5,
                PlanEEPHoursNullable = 6,
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = TypeOfFunding.AdultSkills
                    },
                    new TestLearningDelivery()
                    {
                        FundModel = 0
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_NullPlanLearnHours()
        {
            var learner = new TestLearner()
            {
                PlanEEPHoursNullable = 6,
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = TypeOfFunding.AdultSkills
                    },
                    new TestLearningDelivery()
                    {
                        FundModel = 0
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
            int fundModel = 25;

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.PlanLearnHours, planLearnHours)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.PlanEEPHours, planEEPHours)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(planLearnHours, planEEPHours, fundModel);

            validationErrorHandlerMock.Verify();
        }

        private PlanLearnHours_03Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new PlanLearnHours_03Rule(validationErrorHandler);
        }
    }
}
