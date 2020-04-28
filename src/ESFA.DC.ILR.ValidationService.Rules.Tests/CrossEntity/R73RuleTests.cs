using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R73RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("R73", result);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void HasQualifyingModelMeetsExpectation(bool expectation)
        {
            var delivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(delivery.Object, 81))
                .Returns(expectation);
            var derivedData = new Mock<IDerivedData_17Rule>(MockBehavior.Strict);

            var sut = new R73Rule(handler.Object, commonOps.Object, derivedData.Object);

            var result = sut.HasQualifyingModel(delivery.Object);

            Assert.Equal(expectation, result);

            handler.VerifyAll();
            commonOps.VerifyAll();
            derivedData.VerifyAll();
        }

        [Theory]
        [InlineData(25, false)]
        [InlineData(1, true)]
        public void IsProgrammeAimMeetsExpectation(int aimType, bool expectation)
        {
            var delivery = new Mock<ILearningDelivery>();
            delivery
                   .SetupGet(x => x.AimType)
                   .Returns(aimType);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var derivedData = new Mock<IDerivedData_17Rule>(MockBehavior.Strict);

            var sut = new R73Rule(handler.Object, commonOps.Object, derivedData.Object);

            var result = sut.IsProgrammeAim(delivery.Object);

            Assert.Equal(expectation, result);

            handler.VerifyAll();
            commonOps.VerifyAll();
            derivedData.VerifyAll();
        }

        [Theory]
        [InlineData(1, false)]
        [InlineData(25, true)]
        public void IsStandardApprenticeshipMeetsExpectation(int? progType, bool expectation)
        {
            var delivery = new Mock<ILearningDelivery>();
            delivery
                   .SetupGet(x => x.ProgTypeNullable)
                   .Returns(progType);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var derivedData = new Mock<IDerivedData_17Rule>(MockBehavior.Strict);

            var sut = new R73Rule(handler.Object, commonOps.Object, derivedData.Object);

            var result = sut.IsStandardApprenticeship(delivery.Object);

            Assert.Equal(expectation, result);

            handler.VerifyAll();
            commonOps.VerifyAll();
            derivedData.VerifyAll();
        }

        [Theory]
        [InlineData(23, true)]
        [InlineData(null, false)]
        public void HasStandardCodeMeetsExpectation(int? candidate, bool expectation)
        {
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.StdCodeNullable)
                .Returns(candidate);

            var sut = NewRule();

            var result = sut.HasStandardCode(delivery.Object);

            Assert.Equal(expectation, result);
        }

        [Fact]
        public void GetRecordTotalsWithEmptySetReturnsZero()
        {
            var sut = NewRule();

            var result = sut.GetRecordTotals(new IAppFinRecord[] { }, sut.IsPaymentRequest);

            Assert.Equal(0, result);
        }

        [Theory]
        [InlineData(25, 1, 4, 2, 6, 1, 7, 2, 8)]
        [InlineData(20, 1, 4, 2, 6, 1, 7, 2, 3, 3, 8)]
        public void GetRecordTotalsForPaymentRequestsMeetsExpectation(int expectation, params int[] recordPairs)
        {
            var records = new List<IAppFinRecord>();

            for (var i = 0; i < recordPairs.Length; i += 2)
            {
                var temp = new Mock<IAppFinRecord>();
                temp.SetupGet(x => x.AFinType).Returns("PMR");
                temp.SetupGet(x => x.AFinCode).Returns(recordPairs[i]);
                temp.SetupGet(x => x.AFinAmount).Returns(recordPairs[i + 1]);

                records.Add(temp.Object);
            }

            var sut = NewRule();

            var result = sut.GetRecordTotals(records, sut.IsPaymentRequest);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(25, 3, 4, 3, 6, 3, 7, 3, 8)]
        [InlineData(8, 1, 4, 2, 6, 1, 7, 2, 8, 3, 8)]
        public void GetRecordTotalsForProviderReimbursementsMeetsExpectation(int expectation, params int[] recordPairs)
        {
            var records = new List<IAppFinRecord>();

            for (var i = 0; i < recordPairs.Length; i += 2)
            {
                var temp = new Mock<IAppFinRecord>();
                temp.SetupGet(x => x.AFinType).Returns("PMR");
                temp.SetupGet(x => x.AFinCode).Returns(recordPairs[i]);
                temp.SetupGet(x => x.AFinAmount).Returns(recordPairs[i + 1]);

                records.Add(temp.Object);
            }

            var sut = NewRule();

            var result = sut.GetRecordTotals(records, sut.IsProviderReimbursement);

            Assert.Equal(expectation, result);
        }

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
            var record = new Mock<IAppFinRecord>();
            record
                .SetupGet(x => x.AFinType)
                .Returns(itemType);
            record
                .SetupGet(x => x.AFinCode)
                .Returns(itemCode);

            var sut = NewRule();

            var result = sut.IsPaymentRequest(record.Object);

            Assert.Equal(expectation, result);
        }

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
            var record = new Mock<IAppFinRecord>();
            record
                .SetupGet(x => x.AFinType)
                .Returns(itemType);
            record
                .SetupGet(x => x.AFinCode)
                .Returns(itemCode);

            var sut = NewRule();

            var result = sut.IsProviderReimbursement(record.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(24, 25, false)]
        [InlineData(25, 25, true)]
        [InlineData(26, 25, false)]
        [InlineData(67, 25, false)]
        [InlineData(26, 56, false)]
        public void HasMatchingStdCodeMeetsExpectation(int candidate, int comparator, bool expectation)
        {
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.StdCodeNullable)
                .Returns(candidate);

            var sut = NewRule();

            var result = sut.HasMatchingStdCode(delivery.Object, comparator);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(25, false)]
        [InlineData(12, false)]
        [InlineData(25, true)]
        [InlineData(12, true)]
        public void IsTNPMoreThanContributionCapForMeetsExpectation(int candidate, bool expectation)
        {
            var deliveries = new ILearningDelivery[] { };

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var derivedData = new Mock<IDerivedData_17Rule>(MockBehavior.Strict);
            derivedData
                .Setup(x => x.IsTNPMoreThanContributionCapFor(candidate, deliveries))
                .Returns(expectation);

            var sut = new R73Rule(handler.Object, commonOps.Object, derivedData.Object);

            var result = sut.IsTNPMoreThanContributionCapFor(candidate, deliveries);

            Assert.Equal(expectation, result);

            handler.VerifyAll();
            commonOps.VerifyAll();
            derivedData.VerifyAll();
        }

        [Theory]
        [InlineData(25)]
        [InlineData(12)]
        [InlineData(21)]
        [InlineData(6118)]
        public void GetTotalTNPPriceForMeetsExpectation(int expectation)
        {
            var deliveries = new ILearningDelivery[] { };

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var derivedData = new Mock<IDerivedData_17Rule>(MockBehavior.Strict);
            derivedData
                .Setup(x => x.GetTotalTNPPriceFor(deliveries))
                .Returns(expectation);

            var sut = new R73Rule(handler.Object, commonOps.Object, derivedData.Object);

            var result = sut.GetTotalTNPPriceFor(deliveries);

            Assert.Equal(expectation, result);

            handler.VerifyAll();
            commonOps.VerifyAll();
            derivedData.VerifyAll();
        }

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
            var sut = NewRule();

            var result = sut.IsThresholdProportionExceededFor(candidate, tnp, (decimal?)cap);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(221, 148, 1, 30, 2, 35, 1, 50, 2, 46, 3, 87)]
        [InlineData(299, 200, 1, 46, 2, 36, 1, 27, 2, 89, 3, 98)]
        public void InvalidItemRaisesValidationMessage(int tnpTotal, int fundingCap, params int[] recordPairs)
        {
            const string LearnRefNumber = "123456789X";
            const int testStdCode = 234;

            var records = new List<IAppFinRecord>();

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
                .Returns(records);
            delivery
                   .SetupGet(x => x.AimType)
                   .Returns(1);
            delivery
                   .SetupGet(x => x.ProgTypeNullable)
                   .Returns(25);

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

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            commonOps.VerifyAll();
            derivedData.VerifyAll();
        }

        [Theory]
        [InlineData(221, 147, 1, 30, 2, 35, 1, 50, 2, 46, 3, 87)]
        [InlineData(299, 199, 1, 46, 2, 36, 1, 27, 2, 89, 3, 98)]
        public void ValidItemDoesNotRaiseValidationMessage(int tnpTotal, int fundingCap, params int[] recordPairs)
        {
            const string LearnRefNumber = "123456789X";
            const int testStdCode = 234;

            var records = new List<IAppFinRecord>();

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
                .Returns(records);
            delivery
                  .SetupGet(x => x.AimType)
                  .Returns(1);
            delivery
                   .SetupGet(x => x.ProgTypeNullable)
                   .Returns(25);

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

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            commonOps.VerifyAll();
            derivedData.VerifyAll();
        }

        public R73Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var derivedData = new Mock<IDerivedData_17Rule>(MockBehavior.Strict);

            return new R73Rule(handler.Object, commonOps.Object, derivedData.Object);
        }
    }
}
