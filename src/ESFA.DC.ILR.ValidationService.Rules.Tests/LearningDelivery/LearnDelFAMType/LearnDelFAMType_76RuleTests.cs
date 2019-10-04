using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_76RuleTests : AbstractRuleTests<LearnDelFAMType_76Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMType_76");
        }

        [Theory]
        [InlineData("SOF", "034")]
        [InlineData("sof", "357")]
        [InlineData("Sof", "115")]
        public void ConditionMet_True(string famType, string famCode)
        {
            var testLearningDeliveryFam = new TestLearningDeliveryFAM()
            {
                LearnDelFAMType = famType,
                LearnDelFAMCode = famCode
            };

            NewRule().ConditionMet(testLearningDeliveryFam).Should().BeTrue();
        }

        [Theory]
        [InlineData("SOF", "105")]
        [InlineData("LDM", "357")]
        [InlineData("ldm", "034")]
        public void ConditionMet_false(string famType, string famCode)
        {
            var testLearningDeliveryFam = new TestLearningDeliveryFAM()
            {
                LearnDelFAMType = famType,
                LearnDelFAMCode = famCode
            };

            NewRule().ConditionMet(testLearningDeliveryFam).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.SOF,
                    LearnDelFAMCode = "034"
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearningDeliveryFAMs = learningDeliveryFAMs
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
            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.SOF,
                    LearnDelFAMCode = "105"
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_WithNoLearnerFAMS_Returns_NoError()
        {
            var learner = new TestLearner();
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.SOF)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, "034")).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(LearningDeliveryFAMTypeConstants.SOF, "034");

            validationErrorHandlerMock.Verify();
        }

        public LearnDelFAMType_76Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnDelFAMType_76Rule(validationErrorHandler: validationErrorHandler);
        }
    }
}
