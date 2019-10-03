using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.FundModel;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.FundModel
{
    public class FundModel_10RuleTests : AbstractRuleTests<FundModel_10Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("FundModel_10");
        }

        [Fact]
        public void ConditionMet_AdultSkills()
        {
            NewRule().ConditionMet(35).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_CommunityLearning()
        {
            NewRule().ConditionMet(10).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False()
        {
            NewRule().ConditionMet(36).Should().BeTrue();
        }

        [Fact]
        public void Validate_Error_When_Condition_Met()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = 36,
                LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                {
                    new TestLearningDeliveryFAM
                    {
                        LearnDelFAMType = "SOF",
                        LearnDelFAMCode = "112"
                    }
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    learningDelivery
                }
            };

            var dd35Mock = new Mock<IDerivedData_35Rule>();

            dd35Mock.Setup(dd => dd.IsCombinedAuthorities(learningDelivery)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(dd35Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Error_False_FundModel()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = 35,
                LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                {
                    new TestLearningDeliveryFAM
                    {
                        LearnDelFAMType = "SOF",
                        LearnDelFAMCode = "112"
                    }
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    learningDelivery
                }
            };

            var dd35Mock = new Mock<IDerivedData_35Rule>();

            dd35Mock.Setup(dd => dd.IsCombinedAuthorities(learningDelivery)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dd35Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Error_False_SOF()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = 35,
                LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                {
                    new TestLearningDeliveryFAM
                    {
                        LearnDelFAMType = "SOF"
                    },
                    new TestLearningDeliveryFAM
                    {
                        LearnDelFAMCode = "105"
                    }
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    learningDelivery
                }
            };

            var dd35Mock = new Mock<IDerivedData_35Rule>();

            dd35Mock.Setup(dd => dd.IsCombinedAuthorities(learningDelivery)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dd35Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        private FundModel_10Rule NewRule(
            IDerivedData_35Rule dd35 = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new FundModel_10Rule(dd35, validationErrorHandler);
        }
    }
}
