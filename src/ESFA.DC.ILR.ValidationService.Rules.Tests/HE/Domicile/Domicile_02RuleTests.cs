using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.HE.DOMICILE;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.DOMICILE
{
    public class DOMICILE_02RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("DOMICILE_02", result);
        }

        [Theory]
        [InlineData(true, "ValidLookUp")]
        [InlineData(false, "InvalidLookUp")]
        [InlineData(true, null)]
        [InlineData(false, "")]
        [InlineData(false, " ")]
        public void HasValidDomicileMeetsExpectation(bool expectation, string domicile)
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            service
                .Setup(x => x.Contains(TypeOfStringCodedLookup.Domicile, domicile))
                .Returns(expectation);

            var sut = new DOMICILE_02Rule(handler.Object, service.Object);

            var mockItem = new Mock<ILearningDeliveryHE>();
            mockItem.Setup(x => x.DOMICILE).Returns(domicile);

            var result = sut.HasValidDomicile(mockItem.Object);

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
            const string domicile = "foo";

            var mockHE = new Mock<ILearningDeliveryHE>();
            mockHE.SetupGet(m => m.DOMICILE).Returns(domicile);

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
            handler
                .Setup(x => x.Handle(
                    Moq.It.Is<string>(y => y == DOMICILE_02Rule.Name),
                    Moq.It.Is<string>(y => y == LearnRefNumber),
                    0,
                    Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == "DOMICILE"),
                    Moq.It.Is<string>(y => y == domicile)))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var service = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            service
                .Setup(x => x.Contains(TypeOfStringCodedLookup.Domicile, Moq.It.IsAny<string>()))
                .Returns(false);

            var sut = new DOMICILE_02Rule(handler.Object, service.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            service.VerifyAll();
        }

        [Fact]
        public void ValidItemDoesNotRaiseValidationMessage()
        {
            const string LearnRefNumber = "123456789X";

            var mockHE = new Mock<ILearningDeliveryHE>();
            mockHE.Setup(x => x.DOMICILE).Returns("A");

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
            var service = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            service
                .Setup(x => x.Contains(TypeOfStringCodedLookup.Domicile, Moq.It.IsAny<string>()))
                .Returns(true);

            var sut = new DOMICILE_02Rule(handler.Object, service.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            service.VerifyAll();
        }

        public DOMICILE_02Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<IProvideLookupDetails>(MockBehavior.Strict);

            return new DOMICILE_02Rule(handler.Object, service.Object);
        }
    }
}
