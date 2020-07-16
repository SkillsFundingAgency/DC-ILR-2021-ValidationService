using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.ProgType;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.ProgType
{
    public class ProgType_03RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("ProgType_03", result);
        }

        [Fact]
        public void ConditionMetWithNullLearningDeliveryReturnsTrue()
        {
            var sut = NewRule();

            var result = sut.ConditionMet(null);

            Assert.True(result);
        }

        [Fact]
        public void ConditionMetWithLearningDeliveryContainingNullProgTypeReturnsFalse()
        {
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();

            var result = sut.ConditionMet(mockDelivery.Object);

            Assert.False(result);
        }

        [Theory]
        [InlineData(1, false)]
        [InlineData(4, false)]
        [InlineData(19, false)]
        [InlineData(26, false)]
        [InlineData(ProgTypes.AdvancedLevelApprenticeship, true)]
        [InlineData(ProgTypes.ApprenticeshipStandard, true)]
        [InlineData(ProgTypes.HigherApprenticeshipLevel4, true)]
        [InlineData(ProgTypes.HigherApprenticeshipLevel5, true)]
        [InlineData(ProgTypes.HigherApprenticeshipLevel6, true)]
        [InlineData(ProgTypes.HigherApprenticeshipLevel7Plus, true)]
        [InlineData(ProgTypes.IntermediateLevelApprenticeship, true)]
        [InlineData(ProgTypes.Traineeship, true)]
        [InlineData(ProgTypes.TLevel, true)]
        [InlineData(ProgTypes.TLevelTransition, true)]
        public void ConditionMetWithLearningDeliveriesContainingProgTypeMeetsExpectation(int progType, bool expectation)
        {
            var lookupMock = new Mock<IProvideLookupDetails>();
            lookupMock.Setup(x => x.Contains(TypeOfIntegerCodedLookup.ProgType, progType)).Returns(expectation);

            var sut = NewRule(lookupMock.Object);
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(progType);

            var result = sut.ConditionMet(mockDelivery.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(1, 4, 19, 26)]
        [InlineData(1, ProgTypes.AdvancedLevelApprenticeship, ProgTypes.HigherApprenticeshipLevel4, ProgTypes.HigherApprenticeshipLevel6)]
        [InlineData(ProgTypes.HigherApprenticeshipLevel7Plus, 4, ProgTypes.HigherApprenticeshipLevel4)]
        [InlineData(ProgTypes.HigherApprenticeshipLevel4, ProgTypes.HigherApprenticeshipLevel6, 19)]
        [InlineData(ProgTypes.IntermediateLevelApprenticeship, ProgTypes.AdvancedLevelApprenticeship, ProgTypes.HigherApprenticeshipLevel6, 26, ProgTypes.ApprenticeshipStandard)]
        public void InvalidItemRaisesValidationMessage(params int[] progTypes)
        {
            const string LearnRefNumber = "123456789X";
            var startDate = new DateTime(2018, 9, 1);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);

            var deliveries = new List<ILearningDelivery>();
            progTypes.ForEach(x =>
            {
                var mockDelivery = new Mock<ILearningDelivery>();
                mockDelivery
                    .SetupGet(y => y.ProgTypeNullable)
                    .Returns(x);
                mockDelivery
                    .SetupGet(y => y.LearnStartDate)
                    .Returns(startDate);

                deliveries.Add(mockDelivery.Object);
            });

            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            mockHandler.Setup(x => x.Handle(
                Moq.It.Is<string>(y => y == ProgType_03Rule.Name),
                Moq.It.Is<string>(y => y == LearnRefNumber),
                0,
                Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));

            mockHandler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == "ProgType"),
                    Moq.It.IsAny<int?>()))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            mockHandler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == "LearnStartDate"),
                    Moq.It.Is<DateTime>(y => y == startDate)))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var lookupMock = new Mock<IProvideLookupDetails>();

            var sut = new ProgType_03Rule(mockHandler.Object, lookupMock.Object);

            sut.Validate(mockLearner.Object);

            mockHandler.VerifyAll();
        }

        [Theory]
        [InlineData(ProgTypes.AdvancedLevelApprenticeship, ProgTypes.HigherApprenticeshipLevel4, ProgTypes.HigherApprenticeshipLevel6)]
        [InlineData(ProgTypes.HigherApprenticeshipLevel7Plus, ProgTypes.HigherApprenticeshipLevel4)]
        [InlineData(ProgTypes.IntermediateLevelApprenticeship, ProgTypes.AdvancedLevelApprenticeship, ProgTypes.HigherApprenticeshipLevel6, ProgTypes.ApprenticeshipStandard)]
        [InlineData(ProgTypes.HigherApprenticeshipLevel4, ProgTypes.HigherApprenticeshipLevel6)]
        [InlineData(ProgTypes.AdvancedLevelApprenticeship, ProgTypes.IntermediateLevelApprenticeship)]
        public void ValidItemDoesNotRaiseAValidationMessage(params int[] progTypes)
        {
            const string LearnRefNumber = "123456789X";

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);

            var deliveries = new List<ILearningDelivery>();
            progTypes.ForEach(x =>
            {
                var mockDelivery = new Mock<ILearningDelivery>();
                mockDelivery
                    .SetupGet(y => y.ProgTypeNullable)
                    .Returns(x);

                deliveries.Add(mockDelivery.Object);
            });

            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var lookupMock = new Mock<IProvideLookupDetails>();
            lookupMock.Setup(x => x.Contains(TypeOfIntegerCodedLookup.ProgType, It.IsAny<int>())).Returns(true);

            var sut = new ProgType_03Rule(mockHandler.Object, lookupMock.Object);

            sut.Validate(mockLearner.Object);

            mockHandler.VerifyAll();
        }

        public ProgType_03Rule NewRule(IProvideLookupDetails lookupDetails = null)
        {
            var handlerMock = new Mock<IValidationErrorHandler>();

            return new ProgType_03Rule(handlerMock.Object, lookupDetails);
        }
    }
}
