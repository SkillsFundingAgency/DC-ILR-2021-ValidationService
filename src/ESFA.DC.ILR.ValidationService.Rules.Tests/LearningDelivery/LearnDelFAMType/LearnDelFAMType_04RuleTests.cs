using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_04RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("LearnDelFAMType_04", result);
        }

        [Theory]
        [InlineData("SOF4", true)]
        [InlineData("LDM567", false)]
        public void IsNotValidMeetsExpectation(string candidate, bool expectation)
        {
            var famType = candidate.Substring(0, 3);
            var famCode = candidate.Substring(3);
            var mockItem = new Mock<ILearningDeliveryFAM>();

            mockItem
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(famType);
            mockItem
                .SetupGet(y => y.LearnDelFAMCode)
                .Returns(famCode);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var lookups = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            lookups
                .Setup(x => x.Contains(TypeOfLimitedLifeLookup.LearnDelFAMType, $"{famType}{famCode}"))
                .Returns(!expectation);

            var sut = new LearnDelFAMType_04Rule(handler.Object, lookups.Object);

            var result = sut.IsNotValid(mockItem.Object);

            handler.VerifyAll();
            lookups.VerifyAll();

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData("EWP2")]
        [InlineData("SDP5")]
        [InlineData("SKK049")]
        public void InvalidItemRaisesValidationMessage(string candidate)
        {
            const string LearnRefNumber = "123456789X";

            var records = new List<ILearningDeliveryFAM>();
            var famType = candidate.Substring(0, 3);
            var famCode = candidate.Substring(3);

            var mockItem = new Mock<ILearningDeliveryFAM>();
            mockItem
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(famType);
            mockItem
                .SetupGet(y => y.LearnDelFAMCode)
                .Returns(famCode);

            records.Add(mockItem.Object);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.LearningDeliveryFAMs)
                .Returns(records);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(LearnDelFAMType_04Rule.Name, LearnRefNumber, null, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnDelFAMType", famType))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnDelFAMCode", famCode))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var lookups = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            lookups
                .Setup(x => x.Contains(TypeOfLimitedLifeLookup.LearnDelFAMType, $"{famType}{famCode}"))
                .Returns(false);

            var sut = new LearnDelFAMType_04Rule(handler.Object, lookups.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            lookups.VerifyAll();
        }

        [Theory]
        [InlineData("EWP2")]
        [InlineData("SDP5")]
        [InlineData("SKK049")]
        public void ValidItemDoesNotRaiseAValidationMessage(string candidate)
        {
            const string LearnRefNumber = "123456789X";

            var records = new List<ILearningDeliveryFAM>();
            var famType = candidate.Substring(0, 3);
            var famCode = candidate.Substring(3);

            var mockItem = new Mock<ILearningDeliveryFAM>();
            mockItem
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(famType);
            mockItem
                .SetupGet(y => y.LearnDelFAMCode)
                .Returns(famCode);

            records.Add(mockItem.Object);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.LearningDeliveryFAMs)
                .Returns(records);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var lookups = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            lookups
                .Setup(x => x.Contains(TypeOfLimitedLifeLookup.LearnDelFAMType, $"{famType}{famCode}"))
                .Returns(true);

            var sut = new LearnDelFAMType_04Rule(handler.Object, lookups.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            lookups.VerifyAll();
        }

        public LearnDelFAMType_04Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>();
            var lookups = new Mock<IProvideLookupDetails>(MockBehavior.Strict);

            return new LearnDelFAMType_04Rule(handler.Object, lookups.Object);
        }
    }
}
