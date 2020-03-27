using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_54RuleTests : AbstractRuleTests<LearnDelFAMType_54Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMType_54");
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(35).Should().BeFalse();
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            NewRule().FundModelConditionMet(36).Should().BeTrue();
        }

        [Theory]
        [InlineData(21)]
        [InlineData(null)]
        public void ProgTypeConditionMet_False(int? progType)
        {
            NewRule().ProgTypeConditionMet(progType).Should().BeFalse();
        }

        [Fact]
        public void ProgTypeConditionMet_True()
        {
            NewRule().ProgTypeConditionMet(25).Should().BeTrue();
        }

        [Theory]
        [InlineData("EEF", "2")]
        [InlineData("FFI", "2")]
        public void LearningDeliveryFAMsCondtionMet_False(string learnDelFAMType, string learnDelFAMCode)
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = learnDelFAMType,
                        LearnDelFAMCode = learnDelFAMCode
                    },
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = "ACT",
                        LearnDelFAMCode = "34"
                    }
                };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(d => d.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, "EEF", "2")).Returns(false);
            learningDeliveryFAMQueryServiceMock.Setup(d => d.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, "FFI", "2")).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object)
                .LearningDeliveryFAMsCondtionMet(testLearningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMsCondtionMet_False_NullCheck()
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = "ACT",
                        LearnDelFAMCode = "34"
                    }
                };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(d => d.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, "EEF", "2")).Returns(false);
            learningDeliveryFAMQueryServiceMock.Setup(d => d.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, "FFI", "2")).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object)
                .LearningDeliveryFAMsCondtionMet(null).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMsCondtionMet_True()
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = "EEF",
                        LearnDelFAMCode = "2"
                    },
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = "FFI",
                        LearnDelFAMCode = "2"
                    },
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = "ACT",
                        LearnDelFAMCode = "34"
                    }
                };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(d => d.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, "EEF", "2")).Returns(true);
            learningDeliveryFAMQueryServiceMock.Setup(d => d.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, "FFI", "2")).Returns(true);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object)
                .LearningDeliveryFAMsCondtionMet(testLearningDeliveryFAMs).Should().BeTrue();
        }

        [Theory]
        [InlineData(35, 20, "ACT", "3")]
        [InlineData(35, 20, "ACT", "2")]
        [InlineData(35, 20, "EEF", "2")]
        [InlineData(35, 25, "EEF", "2")]
        [InlineData(36, 3, "EEF", "2")]
        [InlineData(36, 25, "EEF", "3")]
        [InlineData(36, 25, "EEF", "2")]
        public void ConditonMet_False(int fundModel, int? progType, string learnDelFAMType, string learnDelFAMCode)
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = learnDelFAMType,
                        LearnDelFAMCode = learnDelFAMCode
                    }
                };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(d => d.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, "EEF", "2")).Returns(false);
            learningDeliveryFAMQueryServiceMock.Setup(d => d.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, "FFI", "2")).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object)
                .ConditionMet(fundModel, progType, testLearningDeliveryFAMs).Should().BeFalse();
        }

        [Theory]
        [InlineData(35, 2, "FFI", "2")]
        [InlineData(35, null, "FFI", "2")]
        public void ConditonMet_True(int fundModel, int? progType, string learnDelFAMType, string learnDelFAMCode)
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = learnDelFAMType,
                        LearnDelFAMCode = learnDelFAMCode
                    },
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = "EEF",
                        LearnDelFAMCode = "2"
                    }
                };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(d => d.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, "EEF", "2")).Returns(true);
            learningDeliveryFAMQueryServiceMock.Setup(d => d.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, "FFI", "2")).Returns(true);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object)
                .ConditionMet(fundModel, progType, testLearningDeliveryFAMs).Should().BeTrue();
        }

        [Theory]
        [InlineData(81, 3)]
        [InlineData(81, null)]
        public void Validate_Error(int fundModel, int? progType)
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = "EEF",
                        LearnDelFAMCode = "2"
                    },
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = "FFI",
                        LearnDelFAMCode = "2"
                    }
                };

            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = fundModel,
                        ProgTypeNullable = progType,
                        LearningDeliveryFAMs = testLearningDeliveryFAMs
                    }
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(d => d.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, "EEF", "2")).Returns(true);
            learningDeliveryFAMQueryServiceMock.Setup(d => d.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, "FFI", "2")).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object)
                    .Validate(testLearner);
            }
        }

        [Theory]
        [InlineData(36, 25)]
        [InlineData(36, 3)]
        [InlineData(81, 25)]
        public void Validate_NoError(int fundModel, int? progType)
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = "ACT",
                        LearnDelFAMCode = "44"
                    }
                };

            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = fundModel,
                        ProgTypeNullable = progType,
                        LearningDeliveryFAMs = testLearningDeliveryFAMs
                    }
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(d => d.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, "EEF", "2")).Returns(false);
            learningDeliveryFAMQueryServiceMock.Setup(d => d.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, "FFI", "2")).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter("ProgType", 25)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter("LearnDelFAMType", "EEF")).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter("LearnDelFAMCode", "2")).Verifiable();

            NewRule(
                validationErrorHandler: validationErrorHandlerMock.Object)
                .BuildErrorMessageParameters(25, "EEF", "2");

            validationErrorHandlerMock.Verify();
        }

        public LearnDelFAMType_54Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null)
        {
            return new LearnDelFAMType_54Rule(
                validationErrorHandler: validationErrorHandler,
                learningDeliveryFAMQueryService: learningDeliveryFAMQueryService);
        }
    }
}
