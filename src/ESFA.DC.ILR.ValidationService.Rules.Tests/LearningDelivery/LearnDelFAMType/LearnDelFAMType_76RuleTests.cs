using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Query;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
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
        public void LearnDelFamTypeSOFConditionMet_True(string famType, string famCode)
        {
            var testLearningDeliveryFam = new TestLearningDeliveryFAM()
            {
                LearnDelFAMType = famType,
                LearnDelFAMCode = famCode
            };

            NewRule().LearnDelFamTypeSOFConditionMet(testLearningDeliveryFam).Should().BeTrue();
        }

        [Theory]
        [InlineData("SOF", "105")]
        [InlineData("LDM", "357")]
        [InlineData("ldm", "034")]
        public void LearnDelFamTypeSOFConditionMet_false(string famType, string famCode)
        {
            var testLearningDeliveryFam = new TestLearningDeliveryFAM()
            {
                LearnDelFAMType = famType,
                LearnDelFAMCode = famCode
            };

            NewRule().LearnDelFamTypeSOFConditionMet(testLearningDeliveryFam).Should().BeFalse();
        }

        [Fact]
        public void LearnDelFamTypeLDMConditionMet_True()
        {
            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                    LearnDelFAMCode = "034"
                },
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.SOF,
                    LearnDelFAMCode = "102"
                }
            };

            NewRule(new LearningDeliveryFAMQueryService()).LearnDelFamTypeLDMConditionMet(learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void LearnDelFamTypeLDMConditionMet_False()
        {
            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                    LearnDelFAMCode = "032"
                },
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.SOF,
                    LearnDelFAMCode = "102"
                }
            };

            NewRule(new LearningDeliveryFAMQueryService()).LearnDelFamTypeLDMConditionMet(learningDeliveryFAMs).Should().BeFalse();
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
                },
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.SOF,
                    LearnDelFAMCode = "102"
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

            var learningDeliveryFaMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFaMsQueryServiceMock.Setup(dd => dd.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, "034")).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learningDeliveryFaMsQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
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
                    LearnDelFAMCode = "034"
                },
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

            var learningDeliveryFaMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFaMsQueryServiceMock.Setup(dd => dd.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, "034")).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learningDeliveryFaMsQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_WithNoLearnerFAMS_Returns_NoError()
        {
            var learner = new TestLearner();
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(null, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.SOF)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, "102")).Verifiable();

            NewRule(null, validationErrorHandlerMock.Object).BuildErrorMessageParameters(LearningDeliveryFAMTypeConstants.SOF, "102");

            validationErrorHandlerMock.Verify();
        }

        public LearnDelFAMType_76Rule NewRule(
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnDelFAMType_76Rule(learningDeliveryFamQueryService: learningDeliveryFAMQueryService, validationErrorHandler: validationErrorHandler);
        }
    }
}
