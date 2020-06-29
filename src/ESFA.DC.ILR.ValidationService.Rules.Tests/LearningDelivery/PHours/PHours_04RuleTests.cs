using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.PHours;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.PHours
{
    public class PHours_04RuleTests : AbstractRuleTests<PHours_04Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("PHours_04");
        }

        [Fact]
        public void ConditionMet_True()
        {
            int? pHours = 0;
            int fundModel = 25;
            int aimType = 1;
            int progType = 31;

            NewRule().ConditionMet(aimType, fundModel, progType, pHours).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_PHours()
        {
            int? pHours = 10;
            int fundModel = 25;
            int aimType = 1;
            int progType = 31;

            NewRule().ConditionMet(aimType, fundModel, progType, pHours).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FundModel()
        {
            int? pHours = 0;
            int fundModel = 10;
            int aimType = 1;
            int progType = 31;

            NewRule().ConditionMet(aimType, fundModel, progType, pHours).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_AimType()
        {
            int? pHours = 0;
            int fundModel = 25;
            int aimType = 10;
            int progType = 31;

            NewRule().ConditionMet(aimType, fundModel, progType, pHours).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_ProgType()
        {
            int? pHours = 0;
            int fundModel = 25;
            int aimType = 1;
            int progType = 30;

            NewRule().ConditionMet(aimType, fundModel, progType, pHours).Should().BeFalse();
        }

        public void Validate_Error()
        {
            int? pHours = 0;
            int fundModel = 25;
            int aimType = 1;
            int progType = 31;

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = aimType,
                        FundModel = fundModel,
                        PHoursNullable = pHours,
                        ProgTypeNullable = progType
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        public void Validate_NoError()
        {
            int? pHours = 0;
            int fundModel = 25;
            int aimType = 1;
            int progType = 31;

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = aimType,
                        FundModel = fundModel,
                        PHoursNullable = pHours,
                        ProgTypeNullable = progType
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
            int? pHours = 0;
            int fundModel = 25;
            int aimType = 1;
            int progType = 31;

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("AimType", aimType)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("FundModel", fundModel)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("ProgType", progType)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("PHours", pHours)).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(aimType, fundModel, progType, pHours);

            validationErrorHandlerMock.Verify();
        }

        public PHours_04Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new PHours_04Rule(validationErrorHandler);
        }
    }
}
