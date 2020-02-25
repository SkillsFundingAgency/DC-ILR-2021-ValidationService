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
    public class LearnDelFAMType_48RuleTests : AbstractRuleTests<LearnDelFAMType_48Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMType_48");
        }

        [Theory]
        [InlineData(LearningDeliveryFAMTypeConstants.HHS, "1")]
        [InlineData(LearningDeliveryFAMTypeConstants.HHS, "3")]
        [InlineData(LearningDeliveryFAMTypeConstants.ACT, "2")]
        public void ConditionMet_False(string learDelFamType, string famCode)
        {
            var testLearningDeliveryFam = new TestLearningDeliveryFAM()
            {
                LearnDelFAMType = learDelFamType,
                LearnDelFAMCode = famCode
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock
                .Setup(s => s.GetLearningDeliveryFAMsCountByFAMType(It.IsAny<List<ILearningDeliveryFAM>>(), learDelFamType))
                .Returns(2);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).ConditionMet(testLearningDeliveryFam, null).Should().BeFalse();
        }

        [Theory]
        [InlineData(LearningDeliveryFAMTypeConstants.HHS, "2")]
        [InlineData(LearningDeliveryFAMTypeConstants.HHS, "4")]
        public void ConditionMet_True(string learDelFamType, string famCode)
        {
            var testLearningDeliveryFam = new TestLearningDeliveryFAM()
            {
                LearnDelFAMType = learDelFamType,
                LearnDelFAMCode = famCode
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock
                .Setup(s => s.GetLearningDeliveryFAMsCountByFAMType(It.IsAny<List<ILearningDeliveryFAM>>(), learDelFamType))
                .Returns(2);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).ConditionMet(testLearningDeliveryFam, null).Should().BeTrue();
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
            var testLearningDeliveryFam = new TestLearningDeliveryFAM()
            {
                LearnDelFAMType = LearningDeliveryFAMTypeConstants.HHS,
                LearnDelFAMCode = "1"
            };

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.HHS)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, "1")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(testLearningDeliveryFam);

            validationErrorHandlerMock.Verify();
        }

        public LearnDelFAMType_48Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null)
        {
            return new LearnDelFAMType_48Rule(
                validationErrorHandler: validationErrorHandler,
                learningDeliveryFAMQueryService: learningDeliveryFAMQueryService);
        }
    }
}
