using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Derived
{
    public class DerivedData_17RuleTests
    {
        [Theory]
        [InlineData(23, 24, false)]
        [InlineData(24, 24, true)]
        [InlineData(25, 24, false)]
        [InlineData(203, 204, false)]
        [InlineData(240, 240, true)]
        [InlineData(205, 240, false)]
        [InlineData(null, 240, false)]
        public void HasQualifyingStdCodeMeetsExpectation(int? candidate, int stdCode, bool expectation)
        {
            var delivery = new Mock<ILearningDelivery>(MockBehavior.Strict);
            delivery
                .SetupGet(x => x.StdCodeNullable)
                .Returns(candidate);

            var sut = NewRule();

            var result = sut.HasQualifyingStdCode(delivery.Object, stdCode);

            Assert.Equal(expectation, result);
            delivery.VerifyAll();
        }

        [Theory]
        [InlineData(2, false)]
        [InlineData(1, true)]
        public void IsProgrameAimMeetsExpectation(int aimType, bool expectation)
        {
            var delivery = new Mock<ILearningDelivery>();
            delivery
               .SetupGet(x => x.AimType)
               .Returns(aimType);

            var result = NewRule().IsProgrameAim(delivery.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(24, false)]
        [InlineData(25, true)]
        public void IsStandardApprenticeshipMeetsExpectation(int? progType, bool expectation)
        {
            var delivery = new Mock<ILearningDelivery>();
            delivery
               .SetupGet(x => x.ProgTypeNullable)
               .Returns(progType);

            var result = NewRule().IsStandardApprenticeship(delivery.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(81, true)]
        [InlineData(25, false)]
        public void HasQualifyingFundingMeetsExpectation(int fundModel, bool expectation)
        {
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(y => y.FundModel)
                .Returns(fundModel);

            var result = NewRule().HasQualifyingModel(delivery.Object);

            Assert.Equal(expectation, result);
        }

        [Fact]
        public void GetEarliestDateForCapCheckingWithEmptyCollectionReturnsMinValue()
        {
            var result = NewRule().GetEarliestDateForCapChecking(new ILearningDelivery[] { });

            Assert.Equal(DateTime.MinValue, result);
        }

        public DateTime? GetNullableDate(string candidate) =>
            string.IsNullOrWhiteSpace(candidate) ? (DateTime?)null : DateTime.Parse(candidate);

        [Theory]
        [InlineData("2018-04-01", null, "2018-04-06", "2018-04-01", "2018-04-02", "2018-04-03", "2018-04-04")]
        [InlineData("2018-05-15", "2018-08-06", "2018-08-06", "2018-08-01", "2018-05-15", "2018-08-03", "2018-08-04", "2018-07-13", "2018-07-14")]
        [InlineData("2018-07-13", "2018-08-06", "2018-08-06", "2018-08-01", "2018-07-13", null, "2018-08-04", null, "2018-07-14")]
        public void GetEarliestDateForCapCheckingMeetsExpectation(string expectation, params string[] datePairs)
        {
            var expectedDate = DateTime.Parse(expectation);
            var deliveries = new List<ILearningDelivery>();

            for (var i = 0; i < datePairs.Length; i += 2)
            {
                var delivery = new Mock<ILearningDelivery>(MockBehavior.Strict);
                delivery
                    .SetupGet(x => x.OrigLearnStartDateNullable)
                    .Returns(GetNullableDate(datePairs[i]));
                delivery
                    .SetupGet(x => x.LearnStartDate)
                    .Returns(DateTime.Parse(datePairs[i + 1]));

                deliveries.Add(delivery.Object);
            }

            var sut = NewRule();

            var result = sut.GetEarliestDateForCapChecking(deliveries);

            Assert.Equal(expectedDate, result);
        }

        [Theory]
        [InlineData(23)]
        [InlineData(25)]
        [InlineData(1)]
        [InlineData(6000000)]
        public void GetTotalTNPPriceForModelMeetsExpectation(int expectation)
        {
            var deliveries = new ILearningDelivery[] { };

            var appFinData = new Mock<ILearningDeliveryAppFinRecordQueryService>(MockBehavior.Strict);
            appFinData
                .Setup(x => x.GetTotalTNPPriceForLatestAppFinRecordsForLearning(deliveries))
                .Returns(expectation);

            var sut = NewRule(appFinRecordQueryService: appFinData.Object);

            var result = sut.GetTotalTNPPriceFor(deliveries);

            Assert.Equal(expectation, result);
            appFinData.VerifyAll();
        }

        [Theory]
        [InlineData(12, "2018-06-03")]
        [InlineData(18, "2019-02-07")]
        [InlineData(15, "2016-10-19")]
        public void GetStandardFundingForMeetsExpectation(int candidate, string earliestDate)
        {
            var testDate = DateTime.Parse(earliestDate);
            var standard = new Mock<ILARSStandard>();
            var standardFunding = new Mock<ILARSStandardFunding>();
            var standardFundings = new List<ILARSStandardFunding>
            {
                standardFunding.Object
            };

            standard.Setup(x => x.StandardsFunding).Returns(standardFundings);
            var lars = new Mock<ILARSDataService>(MockBehavior.Strict);
            lars
                .Setup(x => x.GetStandardFor(candidate))
                .Returns(standard.Object);

            var dateTimeQueryService = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTimeQueryService
                .Setup(x => x.IsDateBetween(testDate, Moq.It.IsAny<DateTime>(), Moq.It.IsAny<DateTime>(), true))
                .Returns(true);

            var sut = NewRule(dateTimeQueryService.Object, lars.Object);

            var result = sut.GetStandardFundingFor(candidate, testDate);

            Assert.Equal(standardFunding.Object, result);

            lars.VerifyAll();
            dateTimeQueryService.VerifyAll();
        }

        [Theory]
        [InlineData(12000, 6000, true)]
        [InlineData(52, 33.3, true)]
        [InlineData(52, null, false)]
        public void HasExceededCappedThresholdMeetsExpectation(int candidate, double? cap, bool expectation)
        {
            var sut = NewRule();

            var result = sut.HasExceededCappedThreshold(candidate, (decimal?)cap);

            Assert.Equal(expectation, result);
        }

        public DerivedData_17Rule NewRule(
            IDateTimeQueryService dateTimeQueryService = null,
            ILARSDataService larsDataService = null,
            ILearningDeliveryAppFinRecordQueryService appFinRecordQueryService = null)
        {
            return new DerivedData_17Rule(dateTimeQueryService, larsDataService, appFinRecordQueryService);
        }
    }
}
