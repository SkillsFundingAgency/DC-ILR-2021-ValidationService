using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AchDate;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.AchDate
{
    public class AchDate_09RuleTests : AbstractRuleTests<AchDate_09Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("AchDate_09");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var achDate = new DateTime(2017, 1, 1);
            var learnStartDate = new DateTime(2017, 1, 1);
            var aimType = 1;
            var progType = 1;
            var fundModel = 1;

            var rule = NewRuleMock();

            rule.Setup(r => r.AchDateConditionMet(achDate)).Returns(true);
            rule.Setup(r => r.LearnStartDateConditionMet(learnStartDate)).Returns(true);
            rule.Setup(r => r.ApprenticeshipConditionMet(aimType, progType, fundModel)).Returns(true);

            rule.Object.ConditionMet(achDate, learnStartDate, aimType, progType, fundModel).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_AchDate()
        {
            var achDate = new DateTime(2017, 1, 1);
            var learnStartDate = new DateTime(2017, 1, 1);
            var aimType = 1;
            var progType = 1;
            var fundModel = 1;

            var rule = NewRuleMock();

            rule.Setup(r => r.AchDateConditionMet(achDate)).Returns(false);

            rule.Object.ConditionMet(achDate, learnStartDate, aimType, progType, fundModel).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_LearnStartDate()
        {
            var achDate = new DateTime(2017, 1, 1);
            var learnStartDate = new DateTime(2017, 1, 1);
            var aimType = 1;
            var progType = 1;
            var fundModel = 1;

            var rule = NewRuleMock();

            rule.Setup(r => r.AchDateConditionMet(achDate)).Returns(true);
            rule.Setup(r => r.LearnStartDateConditionMet(learnStartDate)).Returns(false);

            rule.Object.ConditionMet(achDate, learnStartDate, aimType, progType, fundModel).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_Apprenticeship()
        {
            var achDate = new DateTime(2017, 1, 1);
            var learnStartDate = new DateTime(2017, 1, 1);
            var aimType = 1;
            var progType = 1;
            var fundModel = 1;

            var rule = NewRuleMock();

            rule.Setup(r => r.AchDateConditionMet(achDate)).Returns(true);
            rule.Setup(r => r.LearnStartDateConditionMet(learnStartDate)).Returns(true);
            rule.Setup(r => r.ApprenticeshipConditionMet(aimType, progType, fundModel)).Returns(false);

            rule.Object.ConditionMet(achDate, learnStartDate, aimType, progType, fundModel).Should().BeFalse();
        }

        [Fact]
        public void AchDateConditionMet_False()
        {
            NewRule().AchDateConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void LearnStartDateConditionMet_True()
        {
            NewRule().LearnStartDateConditionMet(new DateTime(2016, 1, 1)).Should().BeTrue();
        }

        [Fact]
        public void LearnStartDateConditionMet_False()
        {
            NewRule().LearnStartDateConditionMet(new DateTime(2014, 1, 1)).Should().BeFalse();
        }

        [Fact]
        public void ApprenticeshipConditionMet_True_AimType()
        {
            NewRule().ApprenticeshipConditionMet(2, 1, 1).Should().BeTrue();
        }

        [Fact]
        public void ApprenticeshipConditionMet_True_ProgType()
        {
            NewRule().ApprenticeshipConditionMet(1, 1, 1).Should().BeTrue();
        }

        [Fact]
        public void ApprenticeshipConditionMet_True_FundModel()
        {
            NewRule().ApprenticeshipConditionMet(1, 25, 1).Should().BeTrue();
        }

        [Fact]
        public void ApprenticeshipConditionMet_False_Traineeship()
        {
            NewRule().ApprenticeshipConditionMet(1, 24, 1).Should().BeFalse();
        }

        [Fact]
        public void ApprenticeshipConditionMet_False_Trailblazer()
        {
            NewRule().ApprenticeshipConditionMet(1, 25, 81).Should().BeFalse();
        }

        [Fact]
        public void ApprenticeshipConditionMet_False_AsFundModelEquals36()
        {
            NewRule().ApprenticeshipConditionMet(1, 25, 36).Should().BeFalse();
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
                        AchDateNullable = new DateTime(2019, 1, 1),
                        LearnStartDate = new DateTime(2016, 1, 1)
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

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("AimType", 1)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnStartDate", "01/01/2017")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("ProgType", 1)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("AchDate", "01/01/2016")).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(1, new DateTime(2017, 1, 1), 1, new DateTime(2016, 1, 1));

            validationErrorHandlerMock.Verify();
        }

        private AchDate_09Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new AchDate_09Rule(validationErrorHandler);
        }
    }
}
