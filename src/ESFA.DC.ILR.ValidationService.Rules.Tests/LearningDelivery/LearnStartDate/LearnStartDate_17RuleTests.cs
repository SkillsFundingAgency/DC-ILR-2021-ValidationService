using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnStartDate;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnStartDate
{
    public class LearnStartDate_17RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnStartDate_17Rule(null, larsData.Object, commonOps.Object));
        }

        /// <summary>
        /// New rule with null derived data rule 18 throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullDerivedDataRule18Throws()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnStartDate_17Rule(handler.Object, null, commonOps.Object));
        }

        [Fact]
        public void NewRuleWithNullCommonOperationsThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnStartDate_17Rule(handler.Object, larsData.Object, null));
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
            Assert.Equal("LearnStartDate_17", result);
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
            Assert.Equal(RuleNameConstants.LearnStartDate_17, result);
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
        /// Get standard periods of validity for, meets expectation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void GetStandardPeriodsOfValidityForMeetsExpectation(int candidate)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.StdCodeNullable)
                .Returns(candidate);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            larsData
                .Setup(x => x.GetStandardValiditiesFor(candidate))
                .Returns(Collection.EmptyAndReadOnly<ILARSStandardValidity>());

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            var sut = new LearnStartDate_17Rule(handler.Object, larsData.Object, commonOps.Object);

            // act
            var result = sut.GetStandardPeriodsOfValidityFor(delivery.Object);

            // assert
            Assert.Empty(result);

            handler.VerifyAll();
            larsData.VerifyAll();
            commonOps.VerifyAll();
        }

        /// <summary>
        /// Has qualifying start meets expectation
        /// </summary>
        /// <param name="contractRef">The contract reference.</param>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("Z32cty", "2017-12-31", true)]
        [InlineData("Btr4567", "2017-12-31", false)]
        [InlineData("Byfru", "2018-01-01", true)]
        [InlineData("MdR4es23", "2018-01-01", false)]
        [InlineData("Pfke^5b", "2018-02-01", true)]
        [InlineData("Ax3gBu6", "2018-02-01", false)]
        public void HasQualifyingStartMeetsExpectation(string contractRef, string candidate, bool expectation)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.ConRefNumber)
                .Returns(contractRef);

            var testDate = DateTime.Parse(candidate);

            var validity = new Mock<ILARSStandardValidity>();
            validity
                .SetupGet(x => x.StartDate)
                .Returns(testDate);

            var validities = Collection.Empty<ILARSStandardValidity>();
            validities.Add(validity.Object);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingStart(delivery.Object, testDate, null))
                .Returns(expectation);

            var sut = new LearnStartDate_17Rule(handler.Object, larsData.Object, commonOps.Object);

            // act
            var result = sut.HasQualifyingStart(delivery.Object, validities.AsSafeReadOnlyList());

            // assert
            Assert.Equal(expectation, result);

            handler.VerifyAll();
            larsData.VerifyAll();
            commonOps.VerifyAll();
        }

        /// <summary>
        /// Has qualifying start with null allocations meets expectation
        /// </summary>
        [Fact]
        public void HasQualifyingStartWithNullAllocationsMeetsExpectation()
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            var sut = new LearnStartDate_17Rule(handler.Object, larsData.Object, commonOps.Object);

            // act
            var result = sut.HasQualifyingStart(delivery.Object, null);

            // assert
            Assert.False(result);

            handler.VerifyAll();
            larsData.VerifyAll();
            commonOps.VerifyAll();
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="stdCode">The standard code.</param>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData(1, "2017-12-31")]
        [InlineData(2, "2017-12-31")]
        [InlineData(3, "2018-01-01")]
        [InlineData(4, "2018-01-01")]
        public void InvalidItemRaisesValidationMessage(int stdCode, string candidate)
        {
            // arrange
            const string learnRefNumber = "123456789X";
            const int aimType = 13;
            const int progType = 17;

            var testDate = DateTime.Parse(candidate);

            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.StdCodeNullable)
                .Returns(stdCode);
            delivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(testDate);
            delivery
                .SetupGet(x => x.AimType)
                .Returns(aimType);
            delivery
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(progType);

            var validity = new Mock<ILARSStandardValidity>();
            validity
                .SetupGet(x => x.StartDate)
                .Returns(testDate);

            var validities = Collection.Empty<ILARSStandardValidity>();
            validities.Add(validity.Object);

            var deliveries = Collection.Empty<ILearningDelivery>();
            deliveries.Add(delivery.Object);
            var safeDeliveries = deliveries.AsSafeReadOnlyList();

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(safeDeliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(RuleNameConstants.LearnStartDate_17, learnRefNumber, 0, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("AimType", aimType))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnStartDate", testDate.ToString("d", AbstractRule.RequiredCulture)))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("ProgType", progType))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("StdCode", stdCode))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            larsData
                .Setup(x => x.GetStandardValiditiesFor(stdCode))
                .Returns(validities.AsSafeReadOnlyList());

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.IsRestart(delivery.Object))
                .Returns(false);
            commonOps
                .Setup(x => x.IsStandardApprenticeship(delivery.Object))
                .Returns(true);
            commonOps
                .Setup(x => x.InAProgramme(delivery.Object))
                .Returns(true);

            // pass or fail is based on the return of this function
            commonOps
                .Setup(x => x.HasQualifyingStart(delivery.Object, testDate, null))
                .Returns(false);

            var sut = new LearnStartDate_17Rule(handler.Object, larsData.Object, commonOps.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            larsData.VerifyAll();
            commonOps.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        /// <param name="stdCode">The standard code.</param>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData(1, "2017-12-31")]
        [InlineData(2, "2017-12-31")]
        [InlineData(3, "2018-01-01")]
        [InlineData(4, "2018-01-01")]
        public void ValidItemDoesNotRaiseValidationMessage(int stdCode, string candidate)
        {
            // arrange
            const string learnRefNumber = "123456789X";
            const int aimType = 13;
            const int progType = 17;

            var testDate = DateTime.Parse(candidate);

            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.StdCodeNullable)
                .Returns(stdCode);
            delivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(testDate);
            delivery
                .SetupGet(x => x.AimType)
                .Returns(aimType);
            delivery
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(progType);

            var validity = new Mock<ILARSStandardValidity>();
            validity
                .SetupGet(x => x.StartDate)
                .Returns(testDate);

            var validities = Collection.Empty<ILARSStandardValidity>();
            validities.Add(validity.Object);

            var deliveries = Collection.Empty<ILearningDelivery>();
            deliveries.Add(delivery.Object);
            var safeDeliveries = deliveries.AsSafeReadOnlyList();

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(safeDeliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            larsData
                .Setup(x => x.GetStandardValiditiesFor(stdCode))
                .Returns(validities.AsSafeReadOnlyList());

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.IsRestart(delivery.Object))
                .Returns(false);
            commonOps
                .Setup(x => x.IsStandardApprenticeship(delivery.Object))
                .Returns(true);
            commonOps
                .Setup(x => x.InAProgramme(delivery.Object))
                .Returns(true);

            // pass or fail is based on the return of this function
            commonOps
                .Setup(x => x.HasQualifyingStart(delivery.Object, testDate, null))
                .Returns(true);

            var sut = new LearnStartDate_17Rule(handler.Object, larsData.Object, commonOps.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            larsData.VerifyAll();
            commonOps.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public LearnStartDate_17Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            return new LearnStartDate_17Rule(handler.Object, larsData.Object, commonOps.Object);
        }
    }
}
