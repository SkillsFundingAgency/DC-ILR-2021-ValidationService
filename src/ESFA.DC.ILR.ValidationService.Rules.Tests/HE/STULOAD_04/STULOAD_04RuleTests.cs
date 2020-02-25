using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.HE.STULOAD;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.STULOAD_04
{
    public class STULOAD_04RuleTests : AbstractRuleTests<STULOAD_04Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("STULOAD_04");
        }

        [Fact]
        public void LearnStartDateConditionMet_True()
        {
            var learnStartDate = new DateTime(2013, 08, 02);

            NewRule().LearnStartDateConditionMet(learnStartDate).Should().BeTrue();
        }

        [Fact]
        public void LearnStartDateConditionMet_False()
        {
            var learnStartDate = new DateTime(2013, 07, 31);

            NewRule().LearnStartDateConditionMet(learnStartDate).Should().BeFalse();
        }

        [Fact]
        public void STULOADConcitionMet_True()
        {
            var learningDeliveryHe = new TestLearningDeliveryHE();

            NewRule().STULOADConditionMet(learningDeliveryHe).Should().BeTrue();
        }

        [Fact]
        public void STULOADConditionMet_False()
        {
            var learningDeliveryHe = new TestLearningDeliveryHE()
            {
                STULOADNullable = 1
            };

            NewRule().STULOADConditionMet(learningDeliveryHe).Should().BeFalse();
        }

        [Fact]
        public void STULOADConditionMet_FalseNull()
        {
            NewRule().STULOADConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learnStartDate = new DateTime(2013, 08, 02);
            var learningDeliveryHe = new TestLearningDeliveryHE();

            NewRule().ConditionMet(learnStartDate, learningDeliveryHe).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_FalseLearnStartDate()
        {
            var learnStartDate = new DateTime(2013, 07, 02);
            var learningDeliveryHe = new TestLearningDeliveryHE();

            NewRule().ConditionMet(learnStartDate, learningDeliveryHe).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseSTULOAD()
        {
            var learnStartDate = new DateTime(2013, 08, 02);
            var learningDeliveryHe = new TestLearningDeliveryHE()
            {
                STULOADNullable = 1
            };

            NewRule().ConditionMet(learnStartDate, learningDeliveryHe).Should().BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = new DateTime(2018, 10, 01),
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
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
                        LearnStartDate = new DateTime(2018, 10, 01),
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                        {
                            STULOADNullable = 1
                        }
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

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnStartDate", "01/01/2017")).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2017, 1, 1));

            validationErrorHandlerMock.Verify();
        }

        private STULOAD_04Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new STULOAD_04Rule(validationErrorHandler);
        }
    }
}
