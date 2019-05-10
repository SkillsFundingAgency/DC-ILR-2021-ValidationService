using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_04RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var lookups = new Mock<IProvideLookupDetails>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnDelFAMType_04Rule(null, lookups.Object));
        }

        [Fact]
        public void NewRuleWithNullLookupsProviderThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnDelFAMType_04Rule(handler.Object, null));
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
            Assert.Equal("LearnDelFAMType_04", result);
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
            Assert.Equal(LearnDelFAMType_04Rule.Name, result);
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
        [InlineData("SOF4", true)]
        [InlineData("LDM567", false)]
        public void IsNotValidMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
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

            // act
            var result = sut.IsNotValid(mockItem.Object);

            // assert
            handler.VerifyAll();
            lookups.VerifyAll();

            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData("EWP2")]
        [InlineData("SDP5")]
        [InlineData("SKK049")]
        public void InvalidItemRaisesValidationMessage(string candidate)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var records = Collection.Empty<ILearningDeliveryFAM>();
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
                .Returns(records.AsSafeReadOnlyList());

            var deliveries = Collection.Empty<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries.AsSafeReadOnlyList());

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

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            lookups.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise a validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData("EWP2")]
        [InlineData("SDP5")]
        [InlineData("SKK049")]
        public void ValidItemDoesNotRaiseAValidationMessage(string candidate)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var records = Collection.Empty<ILearningDeliveryFAM>();
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
                .Returns(records.AsSafeReadOnlyList());

            var deliveries = Collection.Empty<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries.AsSafeReadOnlyList());

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var lookups = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            lookups
                .Setup(x => x.Contains(TypeOfLimitedLifeLookup.LearnDelFAMType, $"{famType}{famCode}"))
                .Returns(true);

            var sut = new LearnDelFAMType_04Rule(handler.Object, lookups.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            lookups.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public LearnDelFAMType_04Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>();
            var lookups = new Mock<IProvideLookupDetails>(MockBehavior.Strict);

            return new LearnDelFAMType_04Rule(handler.Object, lookups.Object);
        }
    }
}
