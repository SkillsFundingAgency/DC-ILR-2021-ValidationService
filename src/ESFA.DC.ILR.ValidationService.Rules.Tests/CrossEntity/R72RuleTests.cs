using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R72RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("R72", result);
        }

        [Theory]
        [InlineData(70, false)]
        [InlineData(81, true)]
        public void HasQualifyingModelMeetsExpectation(int fundModel, bool expectation)
        {
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(y => y.FundModel)
                .Returns(fundModel);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var derivedData = new Mock<IDerivedData_17Rule>(MockBehavior.Strict);

            var sut = new R72Rule(handler.Object, derivedData.Object);

            var result = sut.HasQualifyingModel(delivery.Object);

            Assert.Equal(expectation, result);

            handler.VerifyAll();
            derivedData.VerifyAll();
        }

        [Theory]
        [InlineData(2, false)]
        [InlineData(1, true)]
        public void IsProgrammeAimMeetsExpectation(int aimType, bool expectation)
        {
            var delivery = new TestLearningDelivery
            {
                AimType = aimType,
            };

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            NewRule(handler.Object).IsProgrammeAim(delivery).Should().Be(expectation);

            handler.VerifyAll();
        }

        [Theory]
        [InlineData(1, false)]
        [InlineData(25, true)]
        public void IsStandardApprenticeshipMeetsExpectation(int? progType, bool expectation)
        {
            var delivery = new TestLearningDelivery
            {
                ProgTypeNullable = progType,
            };

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            NewRule(handler.Object).IsStandardApprenticeship(delivery).Should().Be(expectation);

            handler.VerifyAll();
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
            var derivedData = new Mock<IDerivedData_17Rule>(MockBehavior.Strict);
            derivedData
                .Setup(x => x.IsTNPMoreThanContributionCapFor(candidate, deliveries))
                .Returns(expectation);

            var sut = new R72Rule(handler.Object, derivedData.Object);

            var result = sut.IsTNPMoreThanContributionCapFor(candidate, deliveries);

            Assert.Equal(expectation, result);

            handler.VerifyAll();
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
            var derivedData = new Mock<IDerivedData_17Rule>(MockBehavior.Strict);
            derivedData
                .Setup(x => x.GetTotalTNPPriceFor(deliveries))
                .Returns(expectation);

            var sut = new R72Rule(handler.Object, derivedData.Object);

            var result = sut.GetTotalTNPPriceFor(deliveries);

            Assert.Equal(expectation, result);

            handler.VerifyAll();
            derivedData.VerifyAll();
        }

        [Theory]
        [InlineData(6000, 12000, true)]
        [InlineData(3999, 12000, false)]
        [InlineData(4000, 12000, false)]
        [InlineData(4001, 12000, true)]
        [InlineData(33, 97, true)]
        [InlineData(33, 98, true)]
        [InlineData(33, 99, false)]
        [InlineData(33, 100, false)]
        public void IsThresholdProportionExceededForMeetsExpectation(int candidate, int cap, bool expectation)
        {
            var sut = NewRule();

            var result = sut.IsThresholdProportionExceededFor(candidate, cap);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(221, 1, 30, 2, 35, 1, 50, 2, 46, 3, 87)]
        [InlineData(299, 1, 46, 2, 36, 1, 27, 2, 89, 3, 98)]
        public void InvalidItemRaisesValidationMessage(int tnpTotal, params int[] recordPairs)
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
            delivery
                .SetupGet(y => y.FundModel)
                .Returns(81);

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
                .Setup(x => x.Handle(RuleNameConstants.R72, LearnRefNumber, null, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
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

            var derivedData = new Mock<IDerivedData_17Rule>(MockBehavior.Strict);
            derivedData
                .Setup(x => x.IsTNPMoreThanContributionCapFor(testStdCode, Moq.It.IsAny<IReadOnlyCollection<ILearningDelivery>>()))
                .Returns(false);
            derivedData
                .Setup(x => x.GetTotalTNPPriceFor(deliveries))
                .Returns(tnpTotal);

            var sut = new R72Rule(handler.Object, derivedData.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            derivedData.VerifyAll();
        }

        [Theory]
        [InlineData(222, 1, 30, 2, 35, 1, 50, 2, 46, 3, 87)]
        [InlineData(300, 1, 46, 2, 36, 1, 27, 2, 89, 3, 98)]
        public void ValidItemDoesNotRaiseValidationMessage(int tnpTotal, params int[] recordPairs)
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
                .SetupGet(y => y.FundModel)
                .Returns(81);
            delivery
                .SetupGet(x => x.AppFinRecords)
                .Returns(records);

            var deliveries = new ILearningDelivery[] { delivery.Object };

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);
            delivery
              .SetupGet(x => x.AimType)
              .Returns(1);
            delivery
               .SetupGet(x => x.ProgTypeNullable)
               .Returns(25);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var derivedData = new Mock<IDerivedData_17Rule>(MockBehavior.Strict);
            derivedData
                .Setup(x => x.IsTNPMoreThanContributionCapFor(testStdCode, Moq.It.IsAny<IReadOnlyCollection<ILearningDelivery>>()))
                .Returns(false);
            derivedData
                .Setup(x => x.GetTotalTNPPriceFor(deliveries))
                .Returns(tnpTotal);

            var sut = new R72Rule(handler.Object, derivedData.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            derivedData.VerifyAll();
        }

        public R72Rule NewRule(
            IValidationErrorHandler handler = null,
            IDerivedData_17Rule derivedData = null)
        {
            return new R72Rule(handler, derivedData);
        }
    }
}
