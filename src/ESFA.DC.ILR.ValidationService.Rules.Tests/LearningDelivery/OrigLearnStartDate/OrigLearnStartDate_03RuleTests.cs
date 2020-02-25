using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.OrigLearnStartDate;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.OrigLearnStartDate
{
    public class OrigLearnStartDate_03RuleTests : AbstractRuleTests<OrigLearnStartDate_03Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("OrigLearnStartDate_03");
        }

        [Theory]
        [InlineData(25)]
        [InlineData(82)]
        [InlineData(10)]
        public void FundModelConditionMet_True(int fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeTrue();
        }

        [Theory]
        [InlineData(35)]
        [InlineData(36)]
        [InlineData(99)]
        [InlineData(100)]
        public void FundModelConditionMet_False(int fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeFalse();
        }

        [Fact]
        public void OrigLearnStartDateConditionMet_False()
        {
            NewRule().OriginalLearnStartDateConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void OrigLearnStartDateConditionMet_True()
        {
            var originalStartDate = new DateTime(2008, 10, 10);
            NewRule().OriginalLearnStartDateConditionMet(originalStartDate).Should().BeTrue();
        }

        [Theory]
        [InlineData(25)]
        [InlineData(82)]
        [InlineData(10)]
        public void ConditionMet_True(int fundModel)
        {
            var origLearnStartDate = new DateTime(2018, 10, 10);
            NewRule().ConditionMet(origLearnStartDate, fundModel).Should().BeTrue();
        }

        [Theory]
        [InlineData(25)]
        [InlineData(82)]
        [InlineData(10)]
        public void ConditionMet_NullOrigStartDate_False(int fundModel)
        {
            NewRule().ConditionMet(null, fundModel).Should().BeFalse();
        }

        [Theory]
        [InlineData(35)]
        [InlineData(99)]
        [InlineData(36)]
        public void ConditionMet_FundModel_False(int fundModel)
        {
            NewRule().ConditionMet(new DateTime(2018, 10, 10), fundModel).Should().BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var origLearnStartDate = new DateTime(2008, 10, 01);

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        OrigLearnStartDateNullable = origLearnStartDate,
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
        public void ValidateNoError()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        OrigLearnStartDateNullable = null,
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
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("OrigLearnStartDate", "01/01/2017")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("FundModel", 35)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2017, 1, 1), 35);

            validationErrorHandlerMock.Verify();
        }

        private OrigLearnStartDate_03Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new OrigLearnStartDate_03Rule(validationErrorHandler);
        }
    }
}
