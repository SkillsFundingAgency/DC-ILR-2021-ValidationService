using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.StdCode;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.StdCode
{
    public class StdCode_02RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new StdCode_02Rule(null, service.Object));
        }

        /// <summary>
        /// New rule with null data service throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullDataServiceThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new StdCode_02Rule(handler.Object, null));
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
            Assert.Equal("StdCode_02", result);
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
            Assert.Equal(StdCode_02Rule.Name, result);
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

        [Theory]
        [InlineData(2, true)]
        [InlineData(null, false)]
        public void HasStandardCodeMeetsExpectation(int? candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();

            var mockItem = new Mock<ILearningDelivery>();
            mockItem
                .SetupGet(x => x.StdCodeNullable)
                .Returns(candidate);

            // act
            var result = sut.HasStandardCode(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(2, true)]
        [InlineData(23, true)]
        [InlineData(38, true)]
        [InlineData(2, false)]
        [InlineData(23, false)]
        [InlineData(38, false)]
        public void IsValidStandardCodeLARSDataServiceVerifiesOK(int candidate, bool expectation)
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.ContainsStandardFor(candidate))
                .Returns(expectation);

            var sut = new StdCode_02Rule(handler.Object, service.Object);

            var mockItem = new Mock<ILearningDelivery>();
            mockItem
                .SetupGet(x => x.StdCodeNullable)
                .Returns(candidate);

            // act
            var result = sut.IsValidStandardCode(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
            handler.VerifyAll();
            service.VerifyAll();
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// 'failure' hinges on a null return for the standard validity on the lars data service
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData(2)]
        [InlineData(23)]
        [InlineData(38)]
        public void InvalidItemRaisesValidationMessage(int candidate)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.StdCodeNullable)
                .Returns(candidate);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mock = new Mock<ILearner>();
            mock
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mock
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(
                    Moq.It.Is<string>(y => y == "StdCode_02"),
                    Moq.It.Is<string>(y => y == LearnRefNumber),
                    0,
                    Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == StdCode_02Rule.MessagePropertyName),
                    candidate))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.ContainsStandardFor(candidate))
                .Returns(false);

            var sut = new StdCode_02Rule(handler.Object, service.Object);

            // act
            sut.Validate(mock.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise a validation message.
        /// 'success' hinges on a valid return for the standard validity on the lars data service
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData(2)]
        [InlineData(23)]
        [InlineData(38)]
        public void ValidItemDoesNotRaiseAValidationMessage(int candidate)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.StdCodeNullable)
                .Returns(candidate);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mock = new Mock<ILearner>();
            mock
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mock
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.ContainsStandardFor(candidate))
                .Returns(true);

            var sut = new StdCode_02Rule(handler.Object, service.Object);

            // act
            sut.Validate(mock.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public StdCode_02Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);

            return new StdCode_02Rule(handler.Object, service.Object);
        }
    }
}
