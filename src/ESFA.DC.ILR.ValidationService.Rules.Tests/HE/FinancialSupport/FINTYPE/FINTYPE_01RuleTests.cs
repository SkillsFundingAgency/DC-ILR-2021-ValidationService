using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.HE.FinancialSupport.FINTYPE;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.FinancialSupport.FINTYPE
{
    /// <summary>
    /// from version 0.7.1 validation spread sheet
    /// </summary>
    public class FINTYPE_01RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var provider = new Mock<IProvideLookupDetails>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new FINTYPE_01Rule(null, provider.Object));
        }

        /// <summary>
        /// New rule with null provider throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullProviderThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new FINTYPE_01Rule(handler.Object, null));
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
            Assert.Equal("FINTYPE_01", result);
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
            Assert.Equal(FINTYPE_01Rule.Name, result);
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
        /// Condition met with NULL finacial support returns true.
        /// </summary>
        [Fact]
        public void ConditionMetWithNullTTAccomReturnsTrue()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.ConditionMet(null);

            // assert
            Assert.True(result);
        }

        /// <summary>
        /// Condition met with no finacial support returns true.
        /// </summary>
        [Fact]
        public void ConditionMetWithEmptyFinancialSupportReturnsTrue()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var provider = new Mock<IProvideLookupDetails>(MockBehavior.Strict);

            var sut = new FINTYPE_01Rule(handler.Object, provider.Object);

            // act
            var result = sut.ConditionMet(new List<ILearnerHEFinancialSupport>());

            // assert
            Assert.True(result);
            handler.VerifyAll();
            provider.VerifyAll();
        }

        /// <summary>
        /// Condition met with valid financial support combinations returns true.
        /// </summary>
        /// <param name="candidates">The candidates.</param>
        [Theory]
        [InlineData(1, 1)]
        [InlineData(1, 2, 2)]
        [InlineData(1, 2, 3, 1)]
        [InlineData(1, 2, 3, 4, 3)]
        public void ConditionMetWithValidFinancialSupportCombinationsReturnsTrue(params int[] candidates)
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var provider = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            provider
                .Setup(x => x.Contains(TypeOfIntegerCodedLookup.FinType, Moq.It.IsAny<int>()))
                .Returns(true);

            var sut = new FINTYPE_01Rule(handler.Object, provider.Object);

            // act
            var result = sut.ConditionMet(GetFinancialSupport(candidates));

            // assert
            Assert.True(result);
            handler.VerifyAll();
            provider.VerifyAll();
        }

        /// <summary>
        /// Condition met with invalid financial support combinations returns false.
        /// </summary>
        /// <param name="candidates">The candidates.</param>
        [Theory]
        [InlineData(1, 1)]
        [InlineData(1, 2, 2)]
        [InlineData(1, 2, 3, 1)]
        [InlineData(1, 2, 3, 4, 3)]
        public void ConditionMetWithInvalidFinancialSupportCombinationsReturnsFalse(params int[] candidates)
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var provider = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            provider
                .Setup(x => x.Contains(TypeOfIntegerCodedLookup.FinType, Moq.It.IsAny<int>()))
                .Returns(false);

            var sut = new FINTYPE_01Rule(handler.Object, provider.Object);

            // act
            var result = sut.ConditionMet(GetFinancialSupport(candidates));

            // assert
            Assert.False(result);
            handler.VerifyAll();
            provider.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise a validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData(51)]
        [InlineData(16)]
        [InlineData(9)]
        [InlineData(0)]
        [InlineData(12)]
        public void ValidItemDoesNotRaiseAValidationMessage(int candidate)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var mock = new Mock<ILearner>();
            mock.SetupGet(x => x.LearnRefNumber).Returns(LearnRefNumber);

            var mockHE = new Mock<ILearnerHE>();
            mockHE.SetupGet(x => x.LearnerHEFinancialSupports).Returns(GetFinancialSupport(candidate));
            mock.SetupGet(x => x.LearnerHEEntity).Returns(mockHE.Object);

            var mockDelivery = new Mock<ILearningDelivery>();
            var mockDeliveryHE = new Mock<ILearningDeliveryHE>();

            mockDelivery.SetupGet(x => x.LearningDeliveryHEEntity)
                .Returns(mockDeliveryHE.Object);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);
            mock.SetupGet(x => x.LearningDeliveries).Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var provider = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            provider
                .Setup(x => x.Contains(TypeOfIntegerCodedLookup.FinType, candidate))
                .Returns(true);

            var sut = new FINTYPE_01Rule(handler.Object, provider.Object);

            // act
            sut.Validate(mock.Object);

            // assert
            handler.VerifyAll();
            provider.VerifyAll();
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData(51)]
        [InlineData(16)]
        [InlineData(9)]
        [InlineData(0)]
        [InlineData(12)]
        public void InvalidItemRaisesValidationMessage(int candidate)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var mock = new Mock<ILearner>();
            mock.SetupGet(x => x.LearnRefNumber).Returns(LearnRefNumber);

            var mockHE = new Mock<ILearnerHE>();
            mockHE.SetupGet(x => x.LearnerHEFinancialSupports).Returns(GetFinancialSupport(candidate));
            mock.SetupGet(x => x.LearnerHEEntity).Returns(mockHE.Object);

            var mockDelivery = new Mock<ILearningDelivery>();

            var deliveries = new List<ILearningDelivery>();
            mock.SetupGet(x => x.LearningDeliveries).Returns(deliveries);

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            mockHandler.Setup(x => x.Handle(
                Moq.It.Is<string>(y => y == FINTYPE_01Rule.Name),
                Moq.It.Is<string>(y => y == LearnRefNumber),
                null,
                Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            mockHandler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == FINTYPE_01Rule.MessagePropertyName),
                    Moq.It.IsAny<IReadOnlyCollection<ILearnerHEFinancialSupport>>()))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var mockProvider = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            mockProvider
                .Setup(x => x.Contains(TypeOfIntegerCodedLookup.FinType, candidate))
                .Returns(false);

            var sut = new FINTYPE_01Rule(mockHandler.Object, mockProvider.Object);

            // act
            sut.Validate(mock.Object);

            // assert
            mockHandler.VerifyAll();
            mockProvider.VerifyAll();
        }

        /// <summary>
        /// Gets the financial support.
        /// </summary>
        /// <param name="candidates">The candidates.</param>
        /// <returns>a collection of mocks built from the candidate "Fin Types"</returns>
        public IReadOnlyCollection<ILearnerHEFinancialSupport> GetFinancialSupport(int[] candidates)
        {
            var collection = new List<ILearnerHEFinancialSupport>();

            candidates.ForEach(x =>
            {
                var mock = new Mock<ILearnerHEFinancialSupport>();
                mock.SetupGet(y => y.FINTYPE).Returns(x);
                collection.Add(mock.Object);
            });

            return collection;
        }

        /// <summary>
        /// Gets the financial support.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        /// a collection of mocks built from the candidate "Fin Types"
        /// </returns>
        public IReadOnlyCollection<ILearnerHEFinancialSupport> GetFinancialSupport(int candidate)
        {
            var collection = new List<ILearnerHEFinancialSupport>();

            var mock = new Mock<ILearnerHEFinancialSupport>();
            mock.SetupGet(y => y.FINTYPE).Returns(candidate);
            collection.Add(mock.Object);

            return collection;
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public FINTYPE_01Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>();
            var provider = new Mock<IProvideLookupDetails>();

            return new FINTYPE_01Rule(handler.Object, provider.Object);
        }
    }
}
