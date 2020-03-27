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
    public class LearnDelFAMType_75RuleTests : AbstractRuleTests<LearnDelFAMType_75Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMType_75");
        }

        [Fact]
        public void DD35ConditionMet_True()
        {
            var learnDelivery = new TestLearningDelivery();

            var dd35Mock = new Mock<IDerivedData_35Rule>();

            dd35Mock.Setup(dd => dd.IsCombinedAuthorities(learnDelivery)).Returns(true);

            NewRule(dd35: dd35Mock.Object).DD35ConditionMet(learnDelivery).Should().BeTrue();
        }

        [Fact]
        public void DD35ConditionMet_False()
        {
            var learnDelivery = new TestLearningDelivery();

            var dd35Mock = new Mock<IDerivedData_35Rule>();

            dd35Mock.Setup(dd => dd.IsCombinedAuthorities(learnDelivery)).Returns(false);

            NewRule(dd35: dd35Mock.Object).DD35ConditionMet(learnDelivery).Should().BeFalse();
        }

        [Theory]
        [InlineData("034")]
        [InlineData("357")]
        [InlineData("363")]
        public void LearningDeliveryFAMsConditionMet_True(string famCode)
        {
            var testLearningDeliveryFam = new TestLearningDeliveryFAM()
            {
                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                LearnDelFAMCode = famCode
            };

            NewRule().LearningDeliveryFamConditionMet(testLearningDeliveryFam).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("abc")]
        [InlineData("123")]
        public void LearningDeliveryFAMsConditionMet_false(string famCode)
        {
            var testLearningDeliveryFam = new TestLearningDeliveryFAM()
            {
                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                LearnDelFAMCode = famCode
            };

            NewRule().LearningDeliveryFamConditionMet(testLearningDeliveryFam).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
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

            var dd35Mock = new Mock<IDerivedData_35Rule>();
            dd35Mock.Setup(dd => dd.IsCombinedAuthorities(It.IsAny<ILearningDelivery>())).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object, dd35Mock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                    LearnDelFAMCode = "011"
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

            var dd35Mock = new Mock<IDerivedData_35Rule>();
            dd35Mock.Setup(dd => dd.IsCombinedAuthorities(It.IsAny<ILearningDelivery>())).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, dd35Mock.Object).Validate(learner);
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
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.LDM)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, "034")).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(LearningDeliveryFAMTypeConstants.LDM, "034");

            validationErrorHandlerMock.Verify();
        }

        public LearnDelFAMType_75Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            IDerivedData_35Rule dd35 = null)
        {
            return new LearnDelFAMType_75Rule(validationErrorHandler: validationErrorHandler, dd35: dd35);
        }
    }
}
