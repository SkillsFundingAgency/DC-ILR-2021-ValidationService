using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.WithdrawReason;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.WithdrawReason
{
    public class WithdrawReason_05RuleTests : AbstractRuleTests<WithdrawReason_05Rule>
    {
        [Fact]
        public void RuleName_Test()
        {
            NewRule().RuleName.Should().Be("WithdrawReason_05");
        }

        [Fact]
        public void WithdrawReasonConditionMet_True()
        {
            int? withdrawReason = 28;
            NewRule().WithdrawReasonConditionMet(withdrawReason).Should().BeTrue();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(null)]
        public void WithdrawReasonConditionMet_False(int? withdrawReason)
        {
            NewRule().WithdrawReasonConditionMet(withdrawReason).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMConditionMet_True()
        {
            IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = "LDM1", LearnDelFAMCode = "334" },
                new TestLearningDeliveryFAM()
            };
            var learningDeliveryFAMQueryService = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryService.Setup(ld => ld.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, LearningDeliveryFAMCodeConstants.LDM_OLASS)).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryService.Object).LearningDeliveryFAMConditionMet(learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryFAMConditionMet_False()
        {
            IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = "LDM", LearnDelFAMCode = "034" }
            };
            var learningDeliveryFAMQueryService = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryService.Setup(ld => ld.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, LearningDeliveryFAMCodeConstants.LDM_OLASS)).Returns(true);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryService.Object).LearningDeliveryFAMConditionMet(learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False()
        {
            IReadOnlyCollection<ILearningDeliveryFAM> learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = "LDM", LearnDelFAMCode = "034" }
            };
            ILearningDelivery learningDelivery = new TestLearningDelivery() { WithdrawReasonNullable = 28, LearningDeliveryFAMs = learningDeliveryFAMs };

            var learningDeliveryFAMQueryService = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryService.Setup(ld => ld.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, LearningDeliveryFAMCodeConstants.LDM_OLASS)).Returns(true);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryService.Object).ConditionMet(learningDelivery).Should().BeFalse();
        }

        [Theory]
        [InlineData(28, "LDM1", "334")]
        public void ConditionMet_True(int? withdrawReason, string learnDelFAMType, string learnDelFAMCode)
        {
            ILearningDelivery learningDelivery = new TestLearningDelivery()
            {
                WithdrawReasonNullable = withdrawReason,
                LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
                {
                    new TestLearningDeliveryFAM() { LearnDelFAMType = learnDelFAMType, LearnDelFAMCode = learnDelFAMCode }
                }
            };

            var learningDeliveryFAMQueryService = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryService.Setup(ld => ld.HasLearningDeliveryFAMCodeForType(learningDelivery.LearningDeliveryFAMs, learnDelFAMType, learnDelFAMCode)).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryService.Object).ConditionMet(learningDelivery).Should().BeTrue();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(null)]
        public void BuildErrorMessageParameters(int? withdrawReason)
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("WithdrawReason", withdrawReason)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(withdrawReason);

            validationErrorHandlerMock.Verify();
        }

        [Theory]
        [InlineData(28, "LDM", "034")]
        public void Validate_Error(int? withdrawReason, string learnDelFAMType, string learnDelFAMCode)
        {
            ILearner learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        WithdrawReasonNullable = withdrawReason,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM() { LearnDelFAMType = learnDelFAMType, LearnDelFAMCode = learnDelFAMCode }
                        }
                    }
                }
            };

            var learningDeliveryFAMs = learner.LearningDeliveries.SelectMany(ld => ld.LearningDeliveryFAMs);

            var learningDeliveryFAMQueryService = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryService.Setup(ld => ld.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, learnDelFAMType, learnDelFAMCode)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryService.Object, validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            ILearner learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        WithdrawReasonNullable = 28,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM() { LearnDelFAMType = "LDM", LearnDelFAMCode = "034" }
                        }
                    }
                }
            };
            var learningDeliveryFAMs = learner.LearningDeliveries.SelectMany(ld => ld.LearningDeliveryFAMs);

            var learningDeliveryFAMQueryService = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryService.Setup(ld => ld.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "034")).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryService.Object).Validate(learner);
            }
        }

        private WithdrawReason_05Rule NewRule(ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new WithdrawReason_05Rule(learningDeliveryFAMQueryService, validationErrorHandler);
        }
    }
}
