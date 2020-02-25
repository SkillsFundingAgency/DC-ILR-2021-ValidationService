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
    public class LearnDelFAMType_18RuleTests : AbstractRuleTests<LearnDelFAMType_18Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMType_18");
        }

        [Theory]
        [InlineData(LearningDeliveryFAMTypeConstants.SOF)]
        [InlineData(LearningDeliveryFAMTypeConstants.FFI)]
        [InlineData(LearningDeliveryFAMTypeConstants.EEF)]
        [InlineData(LearningDeliveryFAMTypeConstants.RES)]
        [InlineData(LearningDeliveryFAMTypeConstants.ADL)]
        [InlineData(LearningDeliveryFAMTypeConstants.ASL)]
        [InlineData(LearningDeliveryFAMTypeConstants.SPP)]
        [InlineData(LearningDeliveryFAMTypeConstants.NSA)]
        [InlineData(LearningDeliveryFAMTypeConstants.WPP)]
        [InlineData(LearningDeliveryFAMTypeConstants.POD)]
        [InlineData(LearningDeliveryFAMTypeConstants.FLN)]
        public void ConditionMet_False(string learDelFamType)
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = LearningDeliveryFAMTypeConstants.HHS
                    }
                };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock.Setup(s => s.GetLearningDeliveryFAMsCountByFAMType(testLearningDeliveryFAMs, learDelFamType)).Returns(1);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).ConditionMet(learDelFamType, testLearningDeliveryFAMs).Should().BeFalse();
        }

        [Theory]
        [InlineData(LearningDeliveryFAMTypeConstants.SOF)]
        [InlineData(LearningDeliveryFAMTypeConstants.FFI)]
        [InlineData(LearningDeliveryFAMTypeConstants.EEF)]
        [InlineData(LearningDeliveryFAMTypeConstants.RES)]
        [InlineData(LearningDeliveryFAMTypeConstants.ADL)]
        [InlineData(LearningDeliveryFAMTypeConstants.ASL)]
        [InlineData(LearningDeliveryFAMTypeConstants.SPP)]
        [InlineData(LearningDeliveryFAMTypeConstants.NSA)]
        [InlineData(LearningDeliveryFAMTypeConstants.WPP)]
        [InlineData(LearningDeliveryFAMTypeConstants.POD)]
        [InlineData(LearningDeliveryFAMTypeConstants.FLN)]
        public void ConditionMet_True(string learDelFamType)
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.HHS
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock.Setup(s => s.GetLearningDeliveryFAMsCountByFAMType(testLearningDeliveryFAMs, learDelFamType)).Returns(2);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).ConditionMet(learDelFamType, testLearningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_NullCheck()
        {
            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMsQueryServiceMock.Setup(d => d.GetLearningDeliveryFAMsCountByFAMType(null, LearningDeliveryFAMTypeConstants.HHS)).Returns(0);
            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).ConditionMet(null, null).Should().BeFalse();
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.HHS)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(LearningDeliveryFAMTypeConstants.HHS);

            validationErrorHandlerMock.Verify();
        }

        public LearnDelFAMType_18Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null)
        {
            return new LearnDelFAMType_18Rule(
                validationErrorHandler: validationErrorHandler,
                learningDeliveryFAMQueryService: learningDeliveryFAMQueryService);
        }
    }
}
