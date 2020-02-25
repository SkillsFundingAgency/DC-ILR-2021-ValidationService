using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_71RuleTests : AbstractRuleTests<LearnDelFAMType_71Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMType_71");
        }

        [Theory]
        [InlineData(LearningDeliveryFAMTypeConstants.LDM, "358", LearningDeliveryFAMTypeConstants.ACT, "2")]
        [InlineData(LearningDeliveryFAMTypeConstants.HHS, "820", LearningDeliveryFAMTypeConstants.ACT, "2")]
        public void ConditionMet_False(string learnDelFAMTypeMain, string learnDelFAMCodeMain, string learnDelFAMTypeAct, string learnDelFAMCodeAct)
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = learnDelFAMTypeMain,
                        LearnDelFAMCode = learnDelFAMCodeMain
                    },
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = learnDelFAMTypeAct,
                        LearnDelFAMCode = learnDelFAMCodeAct
                    }
                };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(f => f.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, learnDelFAMTypeMain, learnDelFAMCodeMain)).Returns(false);
            learningDeliveryFAMQueryServiceMock.Setup(f => f.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, learnDelFAMTypeAct, learnDelFAMCodeAct)).Returns(true);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).ConditionMet(testLearningDeliveryFAMs).Should().BeFalse();
        }

        [Theory]
        [InlineData(LearningDeliveryFAMTypeConstants.LDM, "358", LearningDeliveryFAMTypeConstants.ALB, "22")]
        public void ConditionMet_True(string learnDelFAMTypeMain, string learnDelFAMCodeMain, string learnDelFAMTypeAct, string learnDelFAMCodeAct)
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = learnDelFAMTypeMain,
                        LearnDelFAMCode = learnDelFAMCodeMain
                    },
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = learnDelFAMTypeAct,
                        LearnDelFAMCode = learnDelFAMCodeAct
                    }
                };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(f => f.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, learnDelFAMTypeMain, learnDelFAMCodeMain)).Returns(true);
            learningDeliveryFAMQueryServiceMock.Setup(f => f.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, learnDelFAMTypeAct, learnDelFAMCodeAct)).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).ConditionMet(testLearningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                        LearnDelFAMCode = "358"
                    },
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = LearningDeliveryFAMTypeConstants.ALB,
                        LearnDelFAMCode = "22"
                    }
                };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(f => f.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, "358")).Returns(true);
            learningDeliveryFAMQueryServiceMock.Setup(f => f.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ALB, "22")).Returns(false);

            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearningDeliveryFAMs = testLearningDeliveryFAMs
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                        LearnDelFAMCode = "358"
                    },
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                        LearnDelFAMCode = "2"
                    }
                };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(f => f.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, "358")).Returns(false);
            learningDeliveryFAMQueryServiceMock.Setup(f => f.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT, "2")).Returns(true);

            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearningDeliveryFAMs = testLearningDeliveryFAMs
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.LDM)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnFAMCode, "358"));

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(LearningDeliveryFAMTypeConstants.LDM, "358");
            validationErrorHandlerMock.Verify();
        }

        public LearnDelFAMType_71Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null)
        {
            return new LearnDelFAMType_71Rule(validationErrorHandler: validationErrorHandler, learningDeliveryFAMQueryService: learningDeliveryFAMQueryService);
        }
    }
}
