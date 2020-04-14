using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.HE.DOMICILE;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.DOMICILE
{
    public class DOMICILE_02RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var service = new Mock<IProvideLookupDetails>();

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new DOMICILE_02Rule(null, service.Object));
        }

        [Fact]
        public void NewRuleWithNullLookupServiceThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>();

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new DOMICILE_02Rule(handler.Object, null));
        }

        /// <summary>
        /// Rule name 1, matches a literal.
        /// </summary>
        [Fact]
        public void RuleName1()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.RuleName;

            // assert
            Assert.Equal("DOMICILE_02", result);
        }

        /// <summary>
        /// Rule name 2, matches the constant.
        /// </summary>
        [Fact]
        public void RuleName2()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.RuleName;

            // assert
            Assert.Equal(DOMICILE_02Rule.Name, result);
        }

        /// <summary>
        /// Rule name 3 test, account for potential false positives.
        /// </summary>
        [Fact]
        public void RuleName3()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.RuleName;

            // assert
            Assert.NotEqual("SomeOtherRuleName_07", result);
        }

        /// <summary>
        /// Validate with null learner throws.
        /// </summary>
        [Fact]
        public void ValidateWithNullLearnerThrows()
        {
            // arrange
            var sut = NewRule();

            // act/assert
            Assert.Throws<ArgumentNullException>(() => sut.Validate(null));
        }

        /// <summary>
        /// Has valid domicile meets expectation
        /// </summary>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        /// <param name="domicile">the domicile.</param>
        [Theory]
        [InlineData(true, "ValidLookUp")]
        [InlineData(false, "InvalidLookUp")]
        [InlineData(true, null)]
        [InlineData(false, "")]
        [InlineData(false, " ")]
        public void HasValidDomicileMeetsExpectation(bool expectation, string domicile)
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            service
                .Setup(x => x.Contains(TypeOfStringCodedLookup.Domicile, domicile))
                .Returns(expectation);

            var sut = new DOMICILE_02Rule(handler.Object, service.Object);

            var mockItem = new Mock<ILearningDeliveryHE>();
            mockItem.Setup(x => x.DOMICILE).Returns(domicile);

            // act
            var result = sut.HasValidDomicile(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Has higher ed with null entity returns false
        /// </summary>
        [Fact]
        public void HasHigherEdWithNullEntityReturnsFalse()
        {
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<ILearningDelivery>();

            // act
            var result = sut.HasHigherEd(mockItem.Object);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Has higher ed with entity returns true
        /// </summary>
        [Fact]
        public void HasHigherEdWithEntityReturnsTrue()
        {
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<ILearningDelivery>();
            mockItem
                .SetupGet(y => y.LearningDeliveryHEEntity)
                .Returns(new Mock<ILearningDeliveryHE>().Object);

            // act
            var result = sut.HasHigherEd(mockItem.Object);

            // assert
            Assert.True(result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        [Fact]
        public void InvalidItemRaisesValidationMessage()
        {
            // arrange
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

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        [Fact]
        public void ValidItemDoesNotRaiseValidationMessage()
        {
            // arrange
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

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public DOMICILE_02Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<IProvideLookupDetails>(MockBehavior.Strict);

            return new DOMICILE_02Rule(handler.Object, service.Object);
        }
    }
}
