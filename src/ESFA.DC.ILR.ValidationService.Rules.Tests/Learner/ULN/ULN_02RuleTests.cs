using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Learner.ULN;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.ULN
{
    public class ULN_02RuleTests : AbstractRuleTests<ULN_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("ULN_02");
        }

        [Fact]
        public void LearningDeliveryFAMConditionMet_False()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "SOF",
                    LearnDelFAMCode = "1"
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "SOF", "1")).Returns(true);

            NewRule(learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMConditionMet(learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMConditionMet_True()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMCode = "1"
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "SOF", "1")).Returns(false);

            NewRule(learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMConditionMet(learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void ULNConditionMet_False_FundModel()
        {
            NewRule().ULNConditionMet(35, ValidationConstants.TemporaryULN).Should().BeFalse();
        }

        [Fact]
        public void ULNConditionMet_False_ULN()
        {
            NewRule().ULNConditionMet(10, 123456789).Should().BeFalse();
        }

        [Fact]
        public void ULNConditionMet_True()
        {
            NewRule().ULNConditionMet(10, ValidationConstants.TemporaryULN).Should().BeTrue();
        }

        [Theory]
        [InlineData(35, ValidationConstants.TemporaryULN, "SOF")]
        [InlineData(10, 123456789, "SOF")]
        [InlineData(10, ValidationConstants.TemporaryULN, "ACT")]
        public void ConditionMet_False(int fundModel, long uln, string famType)
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = famType,
                    LearnDelFAMCode = "1"
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "SOF", "1")).Returns(true);

            NewRule(learningDeliveryFAMQueryServiceMock.Object).ConditionMet(fundModel, uln, learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMCode = "1"
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "SOF", "1")).Returns(false);

            NewRule(learningDeliveryFAMQueryServiceMock.Object).ConditionMet(10, ValidationConstants.TemporaryULN, learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var learner = new TestLearner()
            {
                ULN = 9999999999,
                LearningDeliveries = new TestLearningDelivery[]
                            {
                    new TestLearningDelivery()
                    {
                        FundModel = 10,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = "SOF",
                                LearnDelFAMCode = "1"
                            }
                        }
                    },
                            }
            };

            var fams = learner.LearningDeliveries.SelectMany(ld => ld.LearningDeliveryFAMs);

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(fams, "SOF", "1")).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner()
            {
                ULN = 9999999999,
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 10,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = "ACT",
                                LearnDelFAMCode = "1"
                            }
                        }
                    },
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learner.LearningDeliveries.SelectMany(ld => ld.LearningDeliveryFAMs), "SOF", "1")).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("ULN", (long)1234567890)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(1234567890);

            validationErrorHandlerMock.Verify();
        }

        private ULN_02Rule NewRule(ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new ULN_02Rule(learningDeliveryFAMQueryService, validationErrorHandler);
        }
    }
}
