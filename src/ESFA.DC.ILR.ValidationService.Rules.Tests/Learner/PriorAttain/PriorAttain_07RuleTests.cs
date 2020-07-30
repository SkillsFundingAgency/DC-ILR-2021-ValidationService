using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Learner.PriorAttain;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.PriorAttain
{
    public class PriorAttain_07RuleTests : AbstractRuleTests<PriorAttain_07Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("PriorAttain_07");
        }

        [Theory]
        [InlineData("2016/08/01")]
        [InlineData("2020/07/30")]
        public void LearnStartDateConditionMet_True(DateTime learnStartDate)
        {
            NewRule().LearnStartDateConditionMet(learnStartDate).Should().BeTrue();
        }

        [Theory]
        [InlineData("2016/07/31")]
        [InlineData("2020/07/31")]
        [InlineData("2016/07/01")]
        [InlineData("2020/08/01")]
        public void LearnStartDateConditionMet_False(DateTime learnStartDate)
        {
            NewRule().LearnStartDateConditionMet(learnStartDate).Should().BeFalse();
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            var fundModel = 35;

            NewRule().FundModelConditionMet(fundModel).Should().BeTrue();
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            var fundModel = 0;

            NewRule().FundModelConditionMet(fundModel).Should().BeFalse();
        }

        [Theory]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(11)]
        [InlineData(12)]
        [InlineData(13)]
        [InlineData(97)]
        [InlineData(98)]
        public void PriorAttainConditionMet_True(int? priorAttain)
        {
            NewRule().PriorAttainConditionMet(priorAttain).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(1)]
        public void PriorAttainConditionMet_False(int? priorAttain)
        {
            NewRule().PriorAttainConditionMet(priorAttain).Should().BeFalse();
        }

        [Fact]
        public void ProgTypeConditionMet_True()
        {
            var progType = 24;

            NewRule().ProgTypeConditionMet(progType).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        public void ProgTypeConditionMet_False(int? progType)
        {
            NewRule().ProgTypeConditionMet(progType).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learnStartDate = new DateTime(2017, 08, 01);
            var fundModel = 35;
            var priorAttain = 3;
            var progType = 24;

            NewRule().ConditionMet(learnStartDate, fundModel, priorAttain, progType).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_FalseLearnStartDate()
        {
            var learnStartDate = new DateTime(2016, 07, 01);
            var fundModel = 35;
            var priorAttain = 3;
            var progType = 24;

            NewRule().ConditionMet(learnStartDate, fundModel, priorAttain, progType).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseFundModel()
        {
            var learnStartDate = new DateTime(2017, 08, 01);
            var fundModel = 0;
            var priorAttain = 3;
            var progType = 24;

            NewRule().ConditionMet(learnStartDate, fundModel, priorAttain, progType).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_FalsePriorAttain()
        {
            var learnStartDate = new DateTime(2017, 08, 01);
            var fundModel = 35;
            var priorAttain = 0;
            var progType = 24;

            NewRule().ConditionMet(learnStartDate, fundModel, priorAttain, progType).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseProgType()
        {
            var learnStartDate = new DateTime(2017, 08, 01);
            var fundModel = 35;
            var priorAttain = 3;
            var progType = 0;

            NewRule().ConditionMet(learnStartDate, fundModel, priorAttain, progType).Should().BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var learner = new TestLearner()
            {
                PriorAttainNullable = 3,
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 35,
                        LearnStartDate = new DateTime(2016, 09, 01),
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
        public void ValidateNoError()
        {
            var learner = new TestLearner()
            {
                PriorAttainNullable = 0,
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 0,
                        LearnStartDate = new DateTime(2016, 06, 01),
                        ProgTypeNullable = 0
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

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, "01/01/2016")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.FundModel, 1)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.PriorAttain, 1)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.ProgType, 1)).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2016, 1, 1), 1, 1, 1);

            validationErrorHandlerMock.Verify();
        }

        private PriorAttain_07Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new PriorAttain_07Rule(validationErrorHandler);
        }
    }
}
