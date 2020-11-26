using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.HE.DOMICILE;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.DOMICILE
{
    public class DOMICILE_01RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("DOMICILE_01", result);
        }

        [Theory]
        [InlineData("2013-08-01", true)]
        [InlineData("2017-06-24", true)]
        [InlineData("2013-07-31", false)]
        [InlineData("2010-11-09", false)]
        public void IsQualifyingStartDateMeetsExpectation(string candidate, bool expectation)
        {
            var sut = NewRule();
            var mockItem = new Mock<ILearningDelivery>();
            mockItem
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(candidate));

            var result = sut.IsQualifyingStartDate(mockItem.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData("UK-Thingy", true)]
        [InlineData("!", true)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void HasDomicileMeetsExpectation(string candidate, bool expectation)
        {
            var sut = NewRule();
            var mockItem = new Mock<ILearningDeliveryHE>();
            mockItem
                .SetupGet(y => y.DOMICILE)
                .Returns(candidate);

            var result = sut.HasDomicile(mockItem.Object);

            Assert.Equal(expectation, result);
        }

        [Fact]
        public void HasHigherEdWithNullEntityReturnsFalse()
        {
            var sut = NewRule();
            var mockItem = new Mock<ILearningDelivery>();

            var result = sut.HasHigherEd(mockItem.Object);

            Assert.False(result);
        }

        [Fact]
        public void HasHigherEdWithEntityReturnsTrue()
        {
            var sut = NewRule();
            var mockItem = new Mock<ILearningDelivery>();
            mockItem
                .SetupGet(y => y.LearningDeliveryHEEntity)
                .Returns(new Mock<ILearningDeliveryHE>().Object);

            var result = sut.HasHigherEd(mockItem.Object);

            Assert.True(result);
        }

        [Fact]
        public void InvalidItemRaisesValidationMessage()
        {
            const string LearnRefNumber = "123456789X";

            var mockHE = new Mock<ILearningDeliveryHE>();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse("2013-08-01"));
            mockDelivery
                .SetupGet(y => y.LearningDeliveryHEEntity)
                .Returns(mockHE.Object);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(y => y.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(
                    Moq.It.Is<string>(y => y == DOMICILE_01Rule.Name),
                    Moq.It.Is<string>(y => y == LearnRefNumber),
                    0,
                    Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == "LearnStartDate"),
                    Moq.It.Is<DateTime>(y => y == mockDelivery.Object.LearnStartDate)))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var sut = new DOMICILE_01Rule(handler.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
        }

        [Fact]
        public void ValidItemDoesNotRaiseValidationMessage()
        {
            const string LearnRefNumber = "123456789X";

            var mockHE = new Mock<ILearningDeliveryHE>();
            mockHE
                .SetupGet(x => x.DOMICILE)
                .Returns("i am a domicile");

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearningDeliveryHEEntity)
                .Returns(mockHE.Object);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(y => y.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var sut = new DOMICILE_01Rule(handler.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
        }

        public DOMICILE_01Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            return new DOMICILE_01Rule(handler.Object);
        }
    }
}
