using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.OrigLearnStartDate;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.OrigLearnStartDate
{
    public class OrigLearnStartDate_09RuleTests : AbstractRuleTests<OrigLearnStartDate_09Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("OrigLearnStartDate_09");
        }

        [Fact]
        public void OrigLearnStartDateConditionMet_Null_False()
        {
            NewRule().OrigLearnStartDateConditionMet(null).Should().BeFalse();
        }

        [Theory]
        [InlineData("2017-04-30")]
        [InlineData("2016-05-01")]
        public void OrigLearnStartDateConditionMet_True(string origStartDate)
        {
            var origLearnStartDate = DateTime.Parse(origStartDate);
            NewRule().OrigLearnStartDateConditionMet(origLearnStartDate).Should().BeTrue();
        }

        [Theory]
        [InlineData("2018-04-30")]
        [InlineData("2017-05-01")]
        public void OrigLearnStartDateConditionMet_False(string origStartDate)
        {
            var origLearnStartDate = DateTime.Parse(origStartDate);
            NewRule().OrigLearnStartDateConditionMet(origLearnStartDate).Should().BeFalse();
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            NewRule().FundModelConditionMet(36).Should().BeTrue();
        }

        [Theory]
        [InlineData(35)]
        [InlineData(99)]
        [InlineData(10)]
        [InlineData(88)]
        public void FundModelConditionMet_False(int fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var origLearnStartDate = new DateTime(2017, 04, 30);
            var fundModel = 36;

            NewRule().ConditionMet(origLearnStartDate, fundModel)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void ConditionMet_FalseOrigLearnStartDate()
        {
            var origLearnStartDate = new DateTime(2017, 05, 01);
            var fundModel = 36;

            NewRule().ConditionMet(origLearnStartDate, fundModel)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseFundModel()
        {
            var origLearnStartDate = new DateTime(2017, 05, 1);
            var fundModel = 35;

            NewRule().ConditionMet(origLearnStartDate, fundModel)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var origLearnStartDate = new DateTime(2017, 04, 30);
            var fundModel = 36;

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        OrigLearnStartDateNullable = origLearnStartDate,
                        FundModel = fundModel,
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
            var origLearnStartDate = new DateTime(2017, 05, 01);
            var fundModel = 36;

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        OrigLearnStartDateNullable = origLearnStartDate,
                        FundModel = fundModel,
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

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2017, 1, 1));

            validationErrorHandlerMock.Verify();
        }

        private OrigLearnStartDate_09Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new OrigLearnStartDate_09Rule(validationErrorHandler);
        }
    }
}
