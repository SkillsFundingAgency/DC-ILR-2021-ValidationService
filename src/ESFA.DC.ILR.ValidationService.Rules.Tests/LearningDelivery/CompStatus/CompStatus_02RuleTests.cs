using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.CompStatus;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.CompStatus
{
    public class CompStatus_02RuleTests : AbstractRuleTests<CompStatus_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("CompStatus_02");
        }

        [Fact]
        public void FundModelConditionMet_Pass()
        {
            var fundModel = 81;
            NewRule().FundModelConditionMet(fundModel).Should().BeTrue();
        }

        [Fact]
        public void FundModelConditionMet_Fails()
        {
            var fundModel = 36;
            NewRule().FundModelConditionMet(fundModel).Should().BeFalse();
        }

        [Fact]
        public void ProgTypeConditionMet_Pass_AsNot25()
        {
            int? progType = 24;
            NewRule().ProgTypeConditionMet(progType).Should().BeTrue();
        }

        [Fact]
        public void ProgTypeConditionMet_Pass_NullProgType()
        {
            int? progType = null;
            NewRule().ProgTypeConditionMet(progType).Should().BeTrue();
        }

        [Fact]
        public void ProgTypeConditionMet_Fails()
        {
            var progType = 25;
            NewRule().ProgTypeConditionMet(progType).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            NewRule().ConditionMet(new DateTime(2017, 8, 1), 1, 81, 24).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_LearnActEndDate()
        {
            NewRule().ConditionMet(null, 1, 81, 24).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_CompStatus()
        {
            NewRule().ConditionMet(new DateTime(2017, 8, 1), 2, 81, 24).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_ForFundModel()
        {
            NewRule().ConditionMet(new DateTime(2017, 8, 1), 1, 36, 24).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_ForProgType()
        {
            NewRule().ConditionMet(new DateTime(2017, 8, 1), 1, 81, 25).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnActEndDateNullable = new DateTime(2017, 1, 1),
                        CompStatus = 1,
                        FundModel = 81,
                        ProgTypeNullable = 24
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
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnActEndDateNullable = null
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

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("CompStatus", 1)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnActEndDate", "01/08/2017")).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(1, new DateTime(2017, 8, 1));

            validationErrorHandlerMock.Verify();
        }

        private CompStatus_02Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new CompStatus_02Rule(validationErrorHandler);
        }
    }
}
