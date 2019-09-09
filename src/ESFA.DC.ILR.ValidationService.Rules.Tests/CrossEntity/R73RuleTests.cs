using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    /// <summary>
    /// cross record rule 72 tests
    /// </summary>
    public class R73RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var derivedData = new Mock<IDerivedData_17Rule>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new R73Rule(null, commonOps.Object, derivedData.Object));
        }

        /// <summary>
        /// New rule with null common operations throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullCommonOperationsThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var derivedData = new Mock<IDerivedData_17Rule>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new R73Rule(handler.Object, null, derivedData.Object));
        }

        /// <summary>
        /// New rule with null common operations throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullDerivedDataThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new R73Rule(handler.Object, commonOps.Object, null));
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
            Assert.Equal("R73", result);
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
            Assert.Equal(RuleNameConstants.R73, result);
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
        /// Has qualifying model meets expectation
        /// </summary>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void HasQualifyingModelMeetsExpectation(bool expectation)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(delivery.Object, 81))
                .Returns(expectation);
            var derivedData = new Mock<IDerivedData_17Rule>(MockBehavior.Strict);

            var sut = new R73Rule(handler.Object, commonOps.Object, derivedData.Object);

            // act
            var result = sut.HasQualifyingModel(delivery.Object);

            // assert
            Assert.Equal(expectation, result);

            handler.VerifyAll();
            commonOps.VerifyAll();
            derivedData.VerifyAll();
        }

        /// <summary>
        /// Is programe aim meets expectation
        /// </summary>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void IsProgrammeAimMeetsExpectation(bool expectation)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.InAProgramme(delivery.Object))
                .Returns(expectation);
            var derivedData = new Mock<IDerivedData_17Rule>(MockBehavior.Strict);

            var sut = new R73Rule(handler.Object, commonOps.Object, derivedData.Object);

            // act
            var result = sut.IsProgrammeAim(delivery.Object);

            // assert
            Assert.Equal(expectation, result);

            handler.VerifyAll();
            commonOps.VerifyAll();
            derivedData.VerifyAll();
        }

        /// <summary>
        /// Is standard apprencticeship meets expectation
        /// </summary>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void IsStandardApprencticeshipMeetsExpectation(bool expectation)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.IsStandardApprencticeship(delivery.Object))
                .Returns(expectation);
            var derivedData = new Mock<IDerivedData_17Rule>(MockBehavior.Strict);

            var sut = new R73Rule(handler.Object, commonOps.Object, derivedData.Object);

            // act
            var result = sut.IsStandardApprencticeship(delivery.Object);

            // assert
            Assert.Equal(expectation, result);

            handler.VerifyAll();
            commonOps.VerifyAll();
            derivedData.VerifyAll();
        }

        /// <summary>
        /// Has standard code meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(23, true)]
        [InlineData(null, false)]
        public void HasStandardCodeMeetsExpectation(int? candidate, bool expectation)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.StdCodeNullable)
                .Returns(candidate);

            var sut = NewRule();

            // act
            var result = sut.HasStandardCode(delivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Get record totals with empty set returns zero.
        /// </summary>
        [Fact]
        public void GetRecordTotalsWithEmptySetReturnsZero()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.GetRecordTotals(new IAppFinRecord[] { }, sut.IsPaymentRequest);

            // assert
            Assert.Equal(0, result);
        }

        /// <summary>
        /// Get record totals for payment requests meets expectation.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        /// <param name="recordPairs">The record pairs.</param>
        [Theory]
        [InlineData(25, 1, 4, 2, 6, 1, 7, 2, 8)]
        [InlineData(20, 1, 4, 2, 6, 1, 7, 2, 3, 3, 8)]
        public void GetRecordTotalsForPaymentRequestsMeetsExpectation(int expectation, params int[] recordPairs)
        {
            // arrange
            var records = Collection.Empty<IAppFinRecord>();

            for (var i = 0; i < recordPairs.Length; i += 2)
            {
                var temp = new Mock<IAppFinRecord>();
                temp.SetupGet(x => x.AFinType).Returns("PMR");
                temp.SetupGet(x => x.AFinCode).Returns(recordPairs[i]);
                temp.SetupGet(x => x.AFinAmount).Returns(recordPairs[i + 1]);

                records.Add(temp.Object);
            }

            var sut = NewRule();

            // act
            var result = sut.GetRecordTotals(records.AsSafeReadOnlyList(), sut.IsPaymentRequest);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Get record totals for provider reimbursements meets expectation.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        /// <param name="recordPairs">The record pairs.</param>
        [Theory]
        [InlineData(25, 3, 4, 3, 6, 3, 7, 3, 8)]
        [InlineData(8, 1, 4, 2, 6, 1, 7, 2, 8, 3, 8)]
        public void GetRecordTotalsForProviderReimbursementsMeetsExpectation(int expectation, params int[] recordPairs)
        {
            // arrange
            var records = Collection.Empty<IAppFinRecord>();

            for (var i = 0; i < recordPairs.Length; i += 2)
            {
                var temp = new Mock<IAppFinRecord>();
                temp.SetupGet(x => x.AFinType).Returns("PMR");
                temp.SetupGet(x => x.AFinCode).Returns(recordPairs[i]);
                temp.SetupGet(x => x.AFinAmount).Returns(recordPairs[i + 1]);

                records.Add(temp.Object);
            }

            var sut = NewRule();

            // act
            var result = sut.GetRecordTotals(records.AsSafeReadOnlyList(), sut.IsProviderReimbursement);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Is payment request meets expectation
        /// </summary>
        /// <param name="itemType">Type of the item.</param>
        /// <param name="itemCode">The item code.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("TNP", 25, false)]
        [InlineData("TNP", 1, false)]
        [InlineData("TNP", 2, false)]
        [InlineData("TNP", 3, false)]
        [InlineData("PMR", 1, true)]
        [InlineData("PMR", 2, true)]
        [InlineData("PMR", 3, false)]
        [InlineData("PMR", 4, false)]
        public void IsPaymentRequestMeetsExpectation(string itemType, int itemCode, bool expectation)
        {
            // arrange
            var record = new Mock<IAppFinRecord>();
            record
                .SetupGet(x => x.AFinType)
                .Returns(itemType);
            record
                .SetupGet(x => x.AFinCode)
                .Returns(itemCode);

            var sut = NewRule();

            // act
            var result = sut.IsPaymentRequest(record.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Is provider reimbursement meets expectation
        /// </summary>
        /// <param name="itemType">Type of the item.</param>
        /// <param name="itemCode">The item code.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("TNP", 25, false)]
        [InlineData("TNP", 1, false)]
        [InlineData("TNP", 2, false)]
        [InlineData("TNP", 3, false)]
        [InlineData("PMR", 1, false)]
        [InlineData("PMR", 2, false)]
        [InlineData("PMR", 3, true)]
        [InlineData("PMR", 4, false)]
        public void IsProviderReimbursementMeetsExpectation(string itemType, int itemCode, bool expectation)
        {
            // arrange
            var record = new Mock<IAppFinRecord>();
            record
                .SetupGet(x => x.AFinType)
                .Returns(itemType);
            record
                .SetupGet(x => x.AFinCode)
                .Returns(itemCode);

            var sut = NewRule();

            // act
            var result = sut.IsProviderReimbursement(record.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Has matching standard code meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="comparator">The comparator.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(24, 25, false)]
        [InlineData(25, 25, true)]
        [InlineData(26, 25, false)]
        [InlineData(67, 25, false)]
        [InlineData(26, 56, false)]
        public void HasMatchingStdCodeMeetsExpectation(int candidate, int comparator, bool expectation)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.StdCodeNullable)
                .Returns(candidate);

            var sut = NewRule();

            // act
            var result = sut.HasMatchingStdCode(delivery.Object, comparator);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Is TNP more than contribution cap for, meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(25, false)]
        [InlineData(12, false)]
        [InlineData(25, true)]
        [InlineData(12, true)]
        public void IsTNPMoreThanContributionCapForMeetsExpectation(int candidate, bool expectation)
        {
            // arrange
            var deliveries = new ILearningDelivery[] { };

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var derivedData = new Mock<IDerivedData_17Rule>(MockBehavior.Strict);
            derivedData
                .Setup(x => x.IsTNPMoreThanContributionCapFor(candidate, deliveries))
                .Returns(expectation);

            var sut = new R73Rule(handler.Object, commonOps.Object, derivedData.Object);

            // act
            var result = sut.IsTNPMoreThanContributionCapFor(candidate, deliveries);

            // assert
            Assert.Equal(expectation, result);

            handler.VerifyAll();
            commonOps.VerifyAll();
            derivedData.VerifyAll();
        }

        /// <summary>
        /// Get total TNP price for, meets expectation.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        [Theory]
        [InlineData(25)]
        [InlineData(12)]
        [InlineData(21)]
        [InlineData(6118)]
        public void GetTotalTNPPriceForMeetsExpectation(int expectation)
        {
            // arrange
            var deliveries = new ILearningDelivery[] { };

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var derivedData = new Mock<IDerivedData_17Rule>(MockBehavior.Strict);
            derivedData
                .Setup(x => x.GetTotalTNPPriceFor(deliveries))
                .Returns(expectation);

            var sut = new R73Rule(handler.Object, commonOps.Object, derivedData.Object);

            // act
            var result = sut.GetTotalTNPPriceFor(deliveries);

            // assert
            Assert.Equal(expectation, result);

            handler.VerifyAll();
            commonOps.VerifyAll();
            derivedData.VerifyAll();
        }

        /// <summary>
        /// Is threshold proportion exceeded for, meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="tnp">The TNP.</param>
        /// <param name="cap">The cap.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(6000, 12000, 6001, true)]
        [InlineData(6000, 12000, null, false)]
        [InlineData(3999, 12000, 8000, false)]
        [InlineData(4000, 12000, 8000, false)]
        [InlineData(4001, 12000, 8000, true)]
        [InlineData(4001, 12000, null, false)]
        [InlineData(33, 99, 66.01, true)]
        [InlineData(33, 99, 66.00, false)]
        [InlineData(33, 99, 65.99, false)]
        [InlineData(33, 100, 66.99, false)]
        [InlineData(33, 100, 67.00, false)]
        [InlineData(33, 100, 67.01, true)]
        [InlineData(33, 100, null, false)]
        public void IsThresholdProportionExceededForMeetsExpectation(int candidate, int tnp, double? cap, bool expectation)
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.IsThresholdProportionExceededFor(candidate, tnp, (decimal?)cap);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="tnpTotal">The TNP total.</param>
        /// <param name="fundingCap">The funding cap.</param>
        /// <param name="recordPairs">The record pairs.</param>
        [Theory]
        [InlineData(221, 148, 1, 30, 2, 35, 1, 50, 2, 46, 3, 87)]
        [InlineData(299, 200, 1, 46, 2, 36, 1, 27, 2, 89, 3, 98)]
        public void InvalidItemRaisesValidationMessage(int tnpTotal, int fundingCap, params int[] recordPairs)
        {
            // arrange
            const string LearnRefNumber = "123456789X";
            const int testStdCode = 234;

            var records = Collection.Empty<IAppFinRecord>();

            for (var i = 0; i < recordPairs.Length; i += 2)
            {
                var temp = new Mock<IAppFinRecord>();
                temp.SetupGet(x => x.AFinType).Returns("PMR");
                temp.SetupGet(x => x.AFinCode).Returns(recordPairs[i]);
                temp.SetupGet(x => x.AFinAmount).Returns(recordPairs[i + 1]);

                records.Add(temp.Object);
            }

            var delivery = new Mock<ILearningDelivery>(MockBehavior.Strict);
            delivery
                .SetupGet(x => x.StdCodeNullable)
                .Returns(testStdCode);
            delivery
                .SetupGet(x => x.AppFinRecords)
                .Returns(records.AsSafeReadOnlyList());

            var deliveries = new ILearningDelivery[] { delivery.Object };

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(RuleNameConstants.R73, LearnRefNumber, null, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnRefNumber", LearnRefNumber))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("AimType", 1))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("ProgType", 25))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("StdCode", testStdCode))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(delivery.Object, 81))
                .Returns(true);
            commonOps
                .Setup(x => x.InAProgramme(delivery.Object))
                .Returns(true);
            commonOps
                .Setup(x => x.IsStandardApprencticeship(delivery.Object))
                .Returns(true);

            var derivedData = new Mock<IDerivedData_17Rule>(MockBehavior.Strict);
            derivedData
                .Setup(x => x.IsTNPMoreThanContributionCapFor(testStdCode, Moq.It.IsAny<IReadOnlyCollection<ILearningDelivery>>()))
                .Returns(true);
            derivedData
                .Setup(x => x.GetTotalTNPPriceFor(deliveries))
                .Returns(tnpTotal);
            derivedData
                .Setup(x => x.GetFundingContributionCapFor(testStdCode, Moq.It.IsAny<IReadOnlyCollection<ILearningDelivery>>()))
                .Returns(fundingCap);

            var sut = new R73Rule(handler.Object, commonOps.Object, derivedData.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            commonOps.VerifyAll();
            derivedData.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        /// <param name="tnpTotal">The TNP total.</param>
        /// <param name="fundingCap">The funding cap.</param>
        /// <param name="recordPairs">The record pairs.</param>
        [Theory]
        [InlineData(221, 147, 1, 30, 2, 35, 1, 50, 2, 46, 3, 87)]
        [InlineData(299, 199, 1, 46, 2, 36, 1, 27, 2, 89, 3, 98)]
        public void ValidItemDoesNotRaiseValidationMessage(int tnpTotal, int fundingCap, params int[] recordPairs)
        {
            // arrange
            const string LearnRefNumber = "123456789X";
            const int testStdCode = 234;

            var records = Collection.Empty<IAppFinRecord>();

            for (var i = 0; i < recordPairs.Length; i += 2)
            {
                var temp = new Mock<IAppFinRecord>();
                temp.SetupGet(x => x.AFinType).Returns("PMR");
                temp.SetupGet(x => x.AFinCode).Returns(recordPairs[i]);
                temp.SetupGet(x => x.AFinAmount).Returns(recordPairs[i + 1]);

                records.Add(temp.Object);
            }

            var delivery = new Mock<ILearningDelivery>(MockBehavior.Strict);
            delivery
                .SetupGet(x => x.StdCodeNullable)
                .Returns(testStdCode);
            delivery
                .SetupGet(x => x.AppFinRecords)
                .Returns(records.AsSafeReadOnlyList());

            var deliveries = new ILearningDelivery[] { delivery.Object };

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(delivery.Object, 81))
                .Returns(true);
            commonOps
                .Setup(x => x.InAProgramme(delivery.Object))
                .Returns(true);
            commonOps
                .Setup(x => x.IsStandardApprencticeship(delivery.Object))
                .Returns(true);

            var derivedData = new Mock<IDerivedData_17Rule>(MockBehavior.Strict);
            derivedData
                .Setup(x => x.IsTNPMoreThanContributionCapFor(testStdCode, Moq.It.IsAny<IReadOnlyCollection<ILearningDelivery>>()))
                .Returns(true);
            derivedData
                .Setup(x => x.GetTotalTNPPriceFor(deliveries))
                .Returns(tnpTotal);
            derivedData
                .Setup(x => x.GetFundingContributionCapFor(testStdCode, Moq.It.IsAny<IReadOnlyCollection<ILearningDelivery>>()))
                .Returns(fundingCap);

            var sut = new R73Rule(handler.Object, commonOps.Object, derivedData.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            commonOps.VerifyAll();
            derivedData.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a new rule</returns>
        public R73Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var derivedData = new Mock<IDerivedData_17Rule>(MockBehavior.Strict);

            return new R73Rule(handler.Object, commonOps.Object, derivedData.Object);
        }
    }
}
