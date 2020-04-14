using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.HE.TTACCOM;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.TTACCOM
{
    /// <summary>
    /// from version 0.7.1 validation spread sheet
    /// </summary>
    public class TTACCOM_02RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var mockService = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            var mockDerived = new Mock<IDerivedData_06Rule>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new TTACCOM_02Rule(null, mockService.Object, mockDerived.Object));
        }

        /// <summary>
        /// New rule with null accomodation details service throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullAccomodationDetailsServiceThrows()
        {
            // arrange
            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var mockDerived = new Mock<IDerivedData_06Rule>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new TTACCOM_02Rule(mockHandler.Object, null, mockDerived.Object));
        }

        /// <summary>
        /// New rule with null accomodation details service throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullDerivedData06RuleThrows()
        {
            // arrange
            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var mockService = new Mock<IProvideLookupDetails>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new TTACCOM_02Rule(mockHandler.Object, mockService.Object, null));
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
            Assert.Equal("TTACCOM_02", result);
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
            Assert.Equal(TTACCOM_02Rule.Name, result);
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
        /// Condition met with null TTAccom returns true.
        /// </summary>
        [Fact]
        public void ConditionMetWithNullTTAccomReturnsTrue()
        {
            // arrange
            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var mockService = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            var mockDerived = new Mock<IDerivedData_06Rule>(MockBehavior.Strict);
            var sut = new TTACCOM_02Rule(mockHandler.Object, mockService.Object, mockDerived.Object);

            // act
            var result = sut.ConditionMet(null, DateTime.MaxValue);

            // assert
            Assert.True(result);
            mockHandler.VerifyAll();
            mockService.VerifyAll();
            mockDerived.VerifyAll();
        }

        /// <summary>
        /// Condition met with valid TTAccom code returns true.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="present">Does the lookup contain the candidate.</param>
        /// <param name="testCaseDate">The test case date.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(1, true, "2013-06-14", true)]
        [InlineData(2, true, "2015-09-03", true)]
        [InlineData(3, true, "2012-06-18", true)]
        [InlineData(1, true, "2013-06-14", false)]
        [InlineData(2, true, "2015-09-03", false)]
        [InlineData(3, true, "2012-06-18", false)]
        [InlineData(1, false, "2013-06-14", true)]
        [InlineData(2, false, "2015-09-03", true)]
        [InlineData(3, false, "2012-06-18", true)]
        public void ConditionMetWithPossibleCandidateExpectation(int candidate, bool present, string testCaseDate, bool expectation)
        {
            // arrange
            var testDate = DateTime.Parse(testCaseDate);

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var mockService = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            mockService
                .Setup(x => x.Contains(TypeOfLimitedLifeLookup.TTAccom, candidate))
                .Returns(present);
            if (present)
            { // IsCurrent only gets called if Contains returns true.
                mockService
                    .Setup(x => x.IsCurrent(TypeOfLimitedLifeLookup.TTAccom, candidate, testDate))
                    .Returns(expectation);
            }

            var mockDerived = new Mock<IDerivedData_06Rule>(MockBehavior.Strict);

            var sut = new TTACCOM_02Rule(mockHandler.Object, mockService.Object, mockDerived.Object);

            // act
            var result = sut.ConditionMet(candidate, testDate);

            // assert
            Assert.Equal(expectation, result);
            mockHandler.VerifyAll();
            mockService.VerifyAll();
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="testCaseDate">The test case date.</param>
        [Theory]
        [InlineData(1, "2013-06-14")]
        [InlineData(2, "2015-09-03")]
        [InlineData(3, "2012-06-18")]
        [InlineData(4, "2008-12-01")]
        public void InvalidItemRaisesValidationMessage(int candidate, string testCaseDate)
        {
            // arrange
            const string LearnRefNumber = "123456789X";
            var testDate = DateTime.Parse(testCaseDate);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);

            var mockHE = new Mock<ILearnerHE>();
            mockHE
                .SetupGet(x => x.TTACCOMNullable)
                .Returns(candidate);
            mockLearner
                .SetupGet(x => x.LearnerHEEntity)
                .Returns(mockHE.Object);

            var mockDelivery = new Mock<ILearningDelivery>();

            var deliveries = new List<ILearningDelivery>();
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            mockHandler.Setup(x => x.Handle(
                Moq.It.Is<string>(y => y == TTACCOM_02Rule.Name),
                Moq.It.Is<string>(y => y == LearnRefNumber),
                null,
                Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            mockHandler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == TTACCOM_02Rule.MessagePropertyName),
                    Moq.It.Is<int>(y => y == candidate)))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var mockService = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            mockService
                .Setup(x => x.Contains(TypeOfLimitedLifeLookup.TTAccom, candidate))
                .Returns(true);
            mockService
                .Setup(x => x.IsCurrent(TypeOfLimitedLifeLookup.TTAccom, candidate, testDate))
                .Returns(false);

            var mockDerived = new Mock<IDerivedData_06Rule>(MockBehavior.Strict);
            mockDerived
                .Setup(x => x.Derive(deliveries))
                .Returns(testDate);

            var sut = new TTACCOM_02Rule(mockHandler.Object, mockService.Object, mockDerived.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            mockHandler.VerifyAll();
            mockService.VerifyAll();
            mockDerived.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise a validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="testCaseDate">The test case date.</param>
        [Theory]
        [InlineData(1, "2013-06-14")]
        [InlineData(2, "2015-09-03")]
        [InlineData(3, "2012-06-18")]
        [InlineData(4, "2008-12-01")]
        public void ValidItemDoesNotRaiseAValidationMessage(int candidate, string testCaseDate)
        {
            // arrange
            const string LearnRefNumber = "123456789X";
            var testDate = DateTime.Parse(testCaseDate);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);

            var mockHE = new Mock<ILearnerHE>();
            mockHE
                .SetupGet(x => x.TTACCOMNullable)
                .Returns(candidate);
            mockLearner
                .SetupGet(x => x.LearnerHEEntity)
                .Returns(mockHE.Object);

            var mockDelivery = new Mock<ILearningDelivery>();

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var mockService = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            mockService
                .Setup(x => x.Contains(TypeOfLimitedLifeLookup.TTAccom, candidate))
                .Returns(true);
            mockService
                .Setup(x => x.IsCurrent(TypeOfLimitedLifeLookup.TTAccom, candidate, testDate))
                .Returns(true);

            var mockDerived = new Mock<IDerivedData_06Rule>(MockBehavior.Strict);
            mockDerived
                .Setup(x => x.Derive(deliveries))
                .Returns(testDate);

            var sut = new TTACCOM_02Rule(mockHandler.Object, mockService.Object, mockDerived.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            mockHandler.VerifyAll();
            mockService.VerifyAll();
            mockDerived.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public TTACCOM_02Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>();
            var service = new Mock<IProvideLookupDetails>();
            var rule = new Mock<IDerivedData_06Rule>(MockBehavior.Strict);

            return new TTACCOM_02Rule(handler.Object, service.Object, rule.Object);
        }
    }
}
