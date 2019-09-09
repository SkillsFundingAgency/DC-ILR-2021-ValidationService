using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Derived
{
    /// <summary>
    /// derived data rule 17 tests
    /// </summary>
    public class DerivedData_17RuleTests
    {
        /// <summary>
        /// New rule throws with null lars service.
        /// </summary>
        [Fact]
        public void NewRuleThrowsWithNullLARSService()
        {
            // arrange
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var appFinData = new Mock<ILearningDeliveryAppFinRecordQueryService>(MockBehavior.Strict);

            Assert.Throws<ArgumentNullException>(() => new DerivedData_17Rule(null, commonOps.Object, appFinData.Object));
        }

        /// <summary>
        /// New rule throws with null common operations provider.
        /// </summary>
        [Fact]
        public void NewRuleThrowsWithNullCommonOperations()
        {
            // arrange
            var lars = new Mock<ILARSDataService>(MockBehavior.Strict);
            var appFinData = new Mock<ILearningDeliveryAppFinRecordQueryService>(MockBehavior.Strict);

            Assert.Throws<ArgumentNullException>(() => new DerivedData_17Rule(lars.Object, null, appFinData.Object));
        }

        /// <summary>
        /// New rule throws with null app fin data service.
        /// </summary>
        [Fact]
        public void NewRuleThrowsWithNullAppFinDataService()
        {
            // arrange
            var lars = new Mock<ILARSDataService>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            Assert.Throws<ArgumentNullException>(() => new DerivedData_17Rule(lars.Object, commonOps.Object, null));
        }

        /// <summary>
        /// Has qualifying standard code meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="stdCode">The standard code.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
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
            // arrange
            var delivery = new Mock<ILearningDelivery>(MockBehavior.Strict);
            delivery
                .SetupGet(x => x.StdCodeNullable)
                .Returns(candidate);

            var sut = NewRule();

            // act
            var result = sut.HasQualifyingStdCode(delivery.Object, stdCode);

            // assert
            Assert.Equal(expectation, result);
            delivery.VerifyAll();
        }

        /// <summary>
        /// Is programe aim meets expectation
        /// </summary>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void IsProgrameAimMeetsExpectation(bool expectation)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();

            var lars = new Mock<ILARSDataService>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.InAProgramme(delivery.Object))
                .Returns(expectation);

            var appFinData = new Mock<ILearningDeliveryAppFinRecordQueryService>(MockBehavior.Strict);

            var sut = new DerivedData_17Rule(lars.Object, commonOps.Object, appFinData.Object);

            // act
            var result = sut.IsProgrameAim(delivery.Object);

            // assert
            Assert.Equal(expectation, result);

            lars.VerifyAll();
            commonOps.VerifyAll();
            appFinData.VerifyAll();
        }

        /// <summary>
        /// Is standard apprenticeship meets expectation
        /// </summary>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void IsStandardApprenticeshipMeetsExpectation(bool expectation)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();

            var lars = new Mock<ILARSDataService>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.IsStandardApprencticeship(delivery.Object))
                .Returns(expectation);

            var appFinData = new Mock<ILearningDeliveryAppFinRecordQueryService>(MockBehavior.Strict);

            var sut = new DerivedData_17Rule(lars.Object, commonOps.Object, appFinData.Object);

            // act
            var result = sut.IsStandardApprenticeship(delivery.Object);

            // assert
            Assert.Equal(expectation, result);

            lars.VerifyAll();
            commonOps.VerifyAll();
            appFinData.VerifyAll();
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

            var lars = new Mock<ILARSDataService>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(delivery.Object, 81))
                .Returns(expectation);

            var appFinData = new Mock<ILearningDeliveryAppFinRecordQueryService>(MockBehavior.Strict);

            var sut = new DerivedData_17Rule(lars.Object, commonOps.Object, appFinData.Object);

            // act
            var result = sut.HasQualifyingModel(delivery.Object);

            // assert
            Assert.Equal(expectation, result);

            lars.VerifyAll();
            commonOps.VerifyAll();
            appFinData.VerifyAll();
        }

        /// <summary>
        /// Gets the earliest date for cap checking with empty collection returns minimum value.
        /// </summary>
        [Fact]
        public void GetEarliestDateForCapCheckingWithEmptyCollectionReturnsMinValue()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.GetEarliestDateForCapChecking(new ILearningDelivery[] { });

            // assert
            Assert.Equal(DateTime.MinValue, result);
        }

        /// <summary>
        /// Gets the nullable date.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>a nullable date</returns>
        public DateTime? GetNullableDate(string candidate) =>
            string.IsNullOrWhiteSpace(candidate) ? (DateTime?)null : DateTime.Parse(candidate);

        /// <summary>
        /// Get earliest date for cap checking, meets expectation.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        /// <param name="datePairs">The date pairs.</param>
        [Theory]
        [InlineData("2018-04-01", null, "2018-04-06", "2018-04-01", "2018-04-02", "2018-04-03", "2018-04-04")]
        [InlineData("2018-05-15", "2018-08-06", "2018-08-06", "2018-08-01", "2018-05-15", "2018-08-03", "2018-08-04", "2018-07-13", "2018-07-14")]
        [InlineData("2018-07-13", "2018-08-06", "2018-08-06", "2018-08-01", "2018-07-13", null, "2018-08-04", null, "2018-07-14")]
        public void GetEarliestDateForCapCheckingMeetsExpectation(string expectation, params string[] datePairs)
        {
            // arrange
            var expectedDate = DateTime.Parse(expectation);
            var deliveries = Collection.Empty<ILearningDelivery>();

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

            // act
            var result = sut.GetEarliestDateForCapChecking(deliveries.AsSafeReadOnlyList());

            // assert
            Assert.Equal(expectedDate, result);
        }

        /// <summary>
        /// Get total TNP price for model meets expectation.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        [Theory]
        [InlineData(23)]
        [InlineData(25)]
        [InlineData(1)]
        [InlineData(6000000)]
        public void GetTotalTNPPriceForModelMeetsExpectation(int expectation)
        {
            // arrange
            var deliveries = new ILearningDelivery[] { };

            var lars = new Mock<ILARSDataService>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var appFinData = new Mock<ILearningDeliveryAppFinRecordQueryService>(MockBehavior.Strict);
            appFinData
                .Setup(x => x.GetTotalTNPPriceForLatestAppFinRecordsForLearning(deliveries))
                .Returns(expectation);

            var sut = new DerivedData_17Rule(lars.Object, commonOps.Object, appFinData.Object);

            // act
            var result = sut.GetTotalTNPPriceFor(deliveries);

            // assert
            Assert.Equal(expectation, result);

            lars.VerifyAll();
            commonOps.VerifyAll();
            appFinData.VerifyAll();
        }

        /// <summary>
        /// Get standard funding for, meets expectation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="earliestDate">The earliest date.</param>
        [Theory]
        [InlineData(12, "2018-06-03")]
        [InlineData(18, "2019-02-07")]
        [InlineData(15, "2016-10-19")]
        public void GetStandardFundingForMeetsExpectation(int candidate, string earliestDate)
        {
            // arrange
            var testDate = DateTime.Parse(earliestDate);
            var standardFunding = new Mock<ILARSStandardFunding>();
            var lars = new Mock<ILARSDataService>(MockBehavior.Strict);
            lars
                .Setup(x => x.GetStandardFundingFor(candidate, testDate))
                .Returns(standardFunding.Object);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var appFinData = new Mock<ILearningDeliveryAppFinRecordQueryService>(MockBehavior.Strict);

            var sut = new DerivedData_17Rule(lars.Object, commonOps.Object, appFinData.Object);

            // act
            var result = sut.GetStandardFundingFor(candidate, testDate);

            // assert
            Assert.Equal(standardFunding.Object, result);

            lars.VerifyAll();
            commonOps.VerifyAll();
            appFinData.VerifyAll();
        }

        /// <summary>
        /// Has exceeded capped threshold meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="cap">The cap.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(12000, 6000, true)]
        [InlineData(52, 33.3, true)]
        [InlineData(52, null, false)]
        public void HasExceededCappedThresholdMeetsExpectation(int candidate, double? cap, bool expectation)
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.HasExceededCappedThreshold(candidate, (decimal?)cap);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a new system under test</returns>
        public DerivedData_17Rule NewRule()
        {
            var lars = new Mock<ILARSDataService>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var appFindata = new Mock<ILearningDeliveryAppFinRecordQueryService>(MockBehavior.Strict);

            return new DerivedData_17Rule(lars.Object, commonOps.Object, appFindata.Object);
        }
    }
}
