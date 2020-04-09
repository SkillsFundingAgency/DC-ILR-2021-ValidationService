using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnStartDate;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnStartDate
{
    public class LearnStartDate_15RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var ddRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnStartDate_15Rule(null, ddRule22.Object, commonOps.Object));
        }

        /// <summary>
        /// New rule with null derived data rule 18 throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullDerivedDataRule18Throws()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var ddRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnStartDate_15Rule(handler.Object, null, commonOps.Object));
        }

        [Fact]
        public void NewRuleWithNullCommonOperationsThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var ddRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnStartDate_15Rule(handler.Object, ddRule22.Object, null));
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
            Assert.Equal("LearnStartDate_15", result);
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
            Assert.Equal(RuleNameConstants.LearnStartDate_15, result);
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

            // act / assert
            Assert.Throws<ArgumentNullException>(() => sut.Validate(null));
        }

        /// <summary>
        /// Get start for, meets expectation.
        /// </summary>
        [Fact]
        public void GetStartForMeetsExpectation()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var ddRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);
            ddRule22
                .Setup(x => x.GetLatestLearningStartForESFContract(null, null))
                .Returns((DateTime?)null);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            var sut = new LearnStartDate_15Rule(handler.Object, ddRule22.Object, commonOps.Object);

            // act
            var result = sut.GetStartFor(null, null);

            // assert
            Assert.Null(result);

            handler.VerifyAll();
            ddRule22.VerifyAll();
            commonOps.VerifyAll();
        }

        /// <summary>
        /// Has qualifying start meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("2017-12-31", true)]
        [InlineData("2017-12-31", false)]
        [InlineData("2018-01-01", true)]
        [InlineData("2018-01-01", false)]
        [InlineData("2018-02-01", true)]
        [InlineData("2018-02-01", false)]
        public void HasQualifyingStartMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();
            var testDate = DateTime.Parse(candidate);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var ddRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingStart(delivery.Object, testDate, null))
                .Returns(expectation);

            var sut = new LearnStartDate_15Rule(handler.Object, ddRule22.Object, commonOps.Object);

            // act
            var result = sut.HasQualifyingStart(delivery.Object, testDate);

            // assert
            Assert.Equal(expectation, result);

            handler.VerifyAll();
            ddRule22.VerifyAll();
            commonOps.VerifyAll();
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData("2017-12-31")]
        [InlineData("2018-12-30")]
        public void InvalidItemRaisesValidationMessage(string candidate)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var testDate = DateTime.Parse(candidate);

            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(testDate);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(delivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(RuleNameConstants.LearnStartDate_15, LearnRefNumber, 0, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnStartDate", testDate.ToString("d", AbstractRule.RequiredCulture)))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var ddRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);
            ddRule22
                .Setup(x => x.GetLatestLearningStartForESFContract(delivery.Object, deliveries))
                .Returns(testDate);

            // pass or fail is based on the return of this function
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingStart(delivery.Object, testDate, null))
                .Returns(false);

            var sut = new LearnStartDate_15Rule(handler.Object, ddRule22.Object, commonOps.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            ddRule22.VerifyAll();
            commonOps.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData("2018-01-01")]
        [InlineData("2018-02-01")]
        [InlineData("2018-12-31")]
        public void ValidItemDoesNotRaiseValidationMessage(string candidate)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var testDate = DateTime.Parse(candidate);
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(testDate);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(delivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var ddRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);
            ddRule22
                .Setup(x => x.GetLatestLearningStartForESFContract(delivery.Object, deliveries))
                .Returns(testDate);

            // pass or fail is based on the return of this function
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingStart(delivery.Object, testDate, null))
                .Returns(true);

            var sut = new LearnStartDate_15Rule(handler.Object, ddRule22.Object, commonOps.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            ddRule22.VerifyAll();
            commonOps.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public LearnStartDate_15Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var ddRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            return new LearnStartDate_15Rule(handler.Object, ddRule22.Object, commonOps.Object);
        }
    }
}
