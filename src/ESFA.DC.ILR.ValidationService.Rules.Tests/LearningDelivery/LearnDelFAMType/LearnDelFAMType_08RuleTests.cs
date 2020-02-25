using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
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
    public class LearnDelFAMType_08RuleTests : AbstractRuleTests<LearnDelFAMType_08Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMType_08");
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(90).Should().BeFalse();
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            NewRule().FundModelConditionMet(99).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryFAMsConditionMet_False()
        {
            List<ILearningDeliveryFAM> learningDeliveryFAMs = new List<ILearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                    LearnDelFAMCode = "135"
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock.Setup(ldf => ldf.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ADL, new HashSet<string>() { "145", "124" })).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).LearningDeliveryFAMsConditionMet(learningDeliveryFAMs).Should().BeFalse();
        }

        [Theory]
        [InlineData(LearningDeliveryFAMTypeConstants.SOF, "105")]
        [InlineData(LearningDeliveryFAMTypeConstants.SOF, "107")]
        public void LearningDeliveryFAMsConditionMet_True(string learnDelFAMType, string learnDelFAMCode)
        {
            List<ILearningDeliveryFAM> learningDeliveryFAMs = new List<ILearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = learnDelFAMType,
                    LearnDelFAMCode = learnDelFAMCode
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock.Setup(ldf => ldf.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.SOF, new HashSet<string>() { "105", "107" })).Returns(true);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).LearningDeliveryFAMsConditionMet(learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            List<ILearningDeliveryFAM> learningDeliveryFAMs = new List<ILearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                    LearnDelFAMCode = "135"
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock.Setup(ldf => ldf.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ADL, new[] { "102", "103" })).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).ConditionMet(99, learningDeliveryFAMs).Should().BeFalse();
        }

        [Theory]
        [InlineData(LearningDeliveryFAMTypeConstants.SOF, "105")]
        [InlineData(LearningDeliveryFAMTypeConstants.SOF, "107")]
        public void ConditionMet_True(string learnDelFAMType, string learnDelFAMCode)
        {
            List<ILearningDeliveryFAM> learningDeliveryFAMs = new List<ILearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = learnDelFAMType,
                    LearnDelFAMCode = learnDelFAMCode
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock.Setup(ldf => ldf.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.SOF, new[] { "105", "107" })).Returns(true);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).ConditionMet(99, learningDeliveryFAMs).Should().BeTrue();
        }

        [Theory]
        [InlineData(LearningDeliveryFAMTypeConstants.SOF, "105")]
        [InlineData(LearningDeliveryFAMTypeConstants.SOF, "107")]
        public void Validate_Error(string learnDelFAMType, string learnDelFAMCode)
        {
            List<ILearningDeliveryFAM> learningDeliveryFAMs = new List<ILearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = learnDelFAMType,
                    LearnDelFAMCode = learnDelFAMCode
                }
            };

            var testLearner = new TestLearner()
            {
                LearnRefNumber = "16testLearner",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 99,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock.Setup(ldf => ldf.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.SOF, new[] { "105", "107" })).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            List<ILearningDeliveryFAM> learningDeliveryFAMs = new List<ILearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                    LearnDelFAMCode = "350"
                }
            };

            var testLearner = new TestLearner()
            {
                LearnRefNumber = "16testLearner",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 75,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock.Setup(ldf => ldf.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT, new[] { "350" })).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError_NullCheck()
        {
            List<ILearningDeliveryFAM> learningDeliveryFAMs = new List<ILearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                    LearnDelFAMCode = "350"
                }
            };

            TestLearner testLearner = null;

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock.Setup(ldf => ldf.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT, new[] { "350" })).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError_LearninDeliveryNullCheck()
        {
            List<ILearningDeliveryFAM> learningDeliveryFAMs = new List<ILearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                    LearnDelFAMCode = "350"
                }
            };

            var testLearner = new TestLearner()
            {
                LearnRefNumber = "16testLearner"
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock.Setup(ldf => ldf.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT, new[] { "350" })).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.FundModel, 99)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.SOF));
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, "105,107"));

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(99, LearningDeliveryFAMTypeConstants.SOF, "105,107");
            validationErrorHandlerMock.Verify();
        }

        public LearnDelFAMType_08Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null)
        {
            return new LearnDelFAMType_08Rule(validationErrorHandler: validationErrorHandler, learningDeliveryFAMQueryService: learningDeliveryFAMQueryService);
        }
    }
}
