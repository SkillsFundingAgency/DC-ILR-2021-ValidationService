using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Learner.ULN;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.ULN
{
    /// <summary>
    /// learning delivery funding and monitoring rule 74 tests
    /// </summary>
    public class ULN_10RuleTests
    {
        /// <summary>
        /// The test provider identifier
        /// </summary>
        public const int TestProviderID = 123456789;

        /// <summary>
        /// The test preparation date
        /// </summary>
        public static readonly DateTime TestPreparationDate = DateTime.Parse("2018-04-02");

        /// <summary>
        /// The test new year date
        /// </summary>
        public static readonly DateTime TestNewYearDate = DateTime.Parse("2018-04-01");

        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var academicYear = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            var dateTime = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new ULN_10Rule(null, academicYear.Object, dateTime.Object, fileData.Object, commonOps.Object));
        }

        /// <summary>
        /// New rule with null academic year throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullAcademicYearThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var dateTime = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new ULN_10Rule(handler.Object, null, dateTime.Object, fileData.Object, commonOps.Object));
        }

        [Fact]
        public void NewRuleWithNullFCSDataThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var academicYear = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new ULN_10Rule(handler.Object, academicYear.Object, null, fileData.Object, commonOps.Object));
        }

        /// <summary>
        /// New rule with null file data throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullFileDataThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var academicYear = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            var dateTime = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new ULN_10Rule(handler.Object, academicYear.Object, dateTime.Object, null, commonOps.Object));
        }

        /// <summary>
        /// New rule with null common ops throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullCommonOpsThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var academicYear = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            var dateTime = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new ULN_10Rule(handler.Object, academicYear.Object, dateTime.Object, fileData.Object, null));
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
            Assert.Equal("ULN_10", result);
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
            Assert.Equal(RuleNameConstants.ULN_10, result);
        }

        /// <summary>
        /// Minimum course duration meets expectation.
        /// </summary>
        [Fact]
        public void MinimumCourseDurationMeetsExpectation()
        {
            // arrange
            var sut = NewRule();

            // act / assert
            Assert.Equal(5, ULN_10Rule.MinimumCourseDuration);
        }

        /// <summary>
        /// Rule leniency period meets expectation.
        /// </summary>
        [Fact]
        public void RuleLeniencyPeriodMeetsExpectation()
        {
            // arrange
            var sut = NewRule();

            // act / assert
            Assert.Equal(60, ULN_10Rule.RuleLeniencyPeriod);
        }

        /// <summary>
        /// File preparation date meets expectation.
        /// </summary>
        [Fact]
        public void FilePreparationDateMeetsExpectation()
        {
            // arrange
            var sut = NewRule();

            // act / assert
            Assert.Equal(TestPreparationDate, sut.FilePreparationDate);
        }

        /// <summary>
        /// First (of) January meets expectation.
        /// </summary>
        [Fact]
        public void FirstJanuaryMeetsExpectation()
        {
            // arrange
            var sut = NewRule();

            // act / assert
            Assert.Equal(TestNewYearDate, sut.FirstJanuary);
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
        [InlineData("2018-04-02", "2018-04-01", false)]
        [InlineData("2018-04-01", "2018-04-01", false)]
        [InlineData("2018-03-31", "2018-04-01", true)]
        public void IsOutsideQualifyingPeriodMeetsExpectation(string candidate, string yearStart, bool expectation)
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var academicYear = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            academicYear
                .Setup(x => x.JanuaryFirst())
                .Returns(DateTime.Parse(yearStart));

            var dateTime = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.FilePreparationDate())
                .Returns(DateTime.Parse(candidate));

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            var sut = new ULN_10Rule(handler.Object, academicYear.Object, dateTime.Object, fileData.Object, commonOps.Object);

            // act
            var result = sut.IsOutsideQualifyingPeriod();

            // assert
            Assert.Equal(expectation, result);

            handler.VerifyAll();
            academicYear.VerifyAll();
            dateTime.VerifyAll();
            fileData.VerifyAll();
            commonOps.VerifyAll();
        }

        /// <summary>
        /// Temporary uln meets expectation.
        /// </summary>
        [Fact]
        public void TemporaryULNMeetsExpectation()
        {
            // arrange / act / assert
            Assert.Equal(9999999999, ValidationConstants.TemporaryULN);
        }

        /// <summary>
        /// Is valid uln meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(12345, true)]
        [InlineData(1234, true)]
        [InlineData(9999999999, false)]
        public void IsValidULNMeetsExpectation(long candidate, bool expectation)
        {
            // arrange
            var learner = new Mock<ILearner>();
            learner.SetupGet(x => x.ULN).Returns(candidate);
            var sut = NewRule();

            // act
            var result = sut.IsValidULN(learner.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Has qualifying start meets expectation
        /// </summary>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void IsLearnerInCustodyMeetsExpectation(bool expectation)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var academicYear = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            academicYear
                .Setup(x => x.JanuaryFirst())
                .Returns(TestNewYearDate);

            var dateTime = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.FilePreparationDate())
                .Returns(TestPreparationDate);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.IsLearnerInCustody(delivery.Object))
                .Returns(expectation);

            var sut = new ULN_10Rule(handler.Object, academicYear.Object, dateTime.Object, fileData.Object, commonOps.Object);

            // act
            var result = sut.IsLearnerInCustody(delivery.Object);

            // assert
            Assert.Equal(expectation, result);

            handler.VerifyAll();
            academicYear.VerifyAll();
            dateTime.VerifyAll();
            fileData.VerifyAll();
            commonOps.VerifyAll();
        }

        /// <summary>
        /// Type of funding meets expectation.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData(99, TypeOfFunding.NotFundedByESFA)]
        public void TypeOfFundingMeetsExpectation(int expectation, int candidate)
        {
            // arrange / act / assert
            Assert.Equal(expectation, candidate);
        }

        /// <summary>
        /// Has qualifying model meets expectation
        /// </summary>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void HasQualifyingModelMeetsExpectation(bool expectation)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var academicYear = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            academicYear
                .Setup(x => x.JanuaryFirst())
                .Returns(TestNewYearDate);

            var dateTime = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.FilePreparationDate())
                .Returns(TestPreparationDate);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(delivery.Object, 99))
                .Returns(expectation);

            var sut = new ULN_10Rule(handler.Object, academicYear.Object, dateTime.Object, fileData.Object, commonOps.Object);

            // act
            var result = sut.HasQualifyingModel(delivery.Object);

            // assert
            Assert.Equal(expectation, result);

            handler.VerifyAll();
            academicYear.VerifyAll();
            dateTime.VerifyAll();
            fileData.VerifyAll();
            commonOps.VerifyAll();
        }

        /// <summary>
        /// Monitoring code meets expectation.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData("SOF1", Monitoring.Delivery.HigherEducationFundingCouncilEngland)]
        [InlineData("LDM034", Monitoring.Delivery.OLASSOffendersInCustody)]
        public void MonitoringCodeMeetsExpectation(string expectation, string candidate)
        {
            // arrange / act / assert
            Assert.Equal(expectation, candidate);
        }

        /// <summary>
        /// Is higher education funding council england, meets expectation
        /// </summary>
        /// <param name="famType">Type of the fam.</param>
        /// <param name="famCode">The fam code.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("ACT", "1", false)] // Monitoring.Delivery.ApprenticeshipFundedThroughAContractForServicesWithEmployer
        [InlineData("LDM", "034", false)] // Monitoring.Delivery.OLASSOffendersInCustody
        [InlineData("FFI", "1", false)] // Monitoring.Delivery.FullyFundedLearningAim
        [InlineData("FFI", "2", false)] // Monitoring.Delivery.CoFundedLearningAim
        [InlineData("LDM", "363", false)] // Monitoring.Delivery.InReceiptOfLowWages
        [InlineData("LDM", "318", false)] // Monitoring.Delivery.MandationToSkillsTraining
        [InlineData("LDM", "328", false)] // Monitoring.Delivery.ReleasedOnTemporaryLicence
        [InlineData("LDM", "347", false)] // Monitoring.Delivery.SteelIndustriesRedundancyTraining
        [InlineData("SOF", "1", true)] // Monitoring.Delivery.HigherEducationFundingCouncilEngland
        [InlineData("SOF", "107", false)] // Monitoring.Delivery.ESFA16To19Funding
        [InlineData("SOF", "105", false)] // Monitoring.Delivery.ESFAAdultFunding
        [InlineData("SOF", "110", false)] // Monitoring.Delivery.GreaterManchesterCombinedAuthority
        [InlineData("SOF", "111", false)] // Monitoring.Delivery.LiverpoolCityRegionCombinedAuthority
        [InlineData("SOF", "112", false)] // Monitoring.Delivery.WestMidlandsCombinedAuthority
        [InlineData("SOF", "113", false)] // Monitoring.Delivery.WestOfEnglandCombinedAuthority
        [InlineData("SOF", "114", false)] // Monitoring.Delivery.TeesValleyCombinedAuthority
        [InlineData("SOF", "115", false)] // Monitoring.Delivery.CambridgeshireAndPeterboroughCombinedAuthority
        [InlineData("SOF", "116", false)] // Monitoring.Delivery.GreaterLondonAuthority
        public void IsHigherEducationFundingCouncilEnglandMeetsExpectation(string famType, string famCode, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var fam = new Mock<ILearningDeliveryFAM>();
            fam
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(famType);
            fam
                .SetupGet(y => y.LearnDelFAMCode)
                .Returns(famCode);

            // act
            var result = sut.IsHigherEducationFundingCouncilEngland(fam.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Has qualifying monitor meets expectation
        /// </summary>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void HasQualifyingMonitorMeetsExpectation(bool expectation)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var academicYear = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            academicYear
                .Setup(x => x.JanuaryFirst())
                .Returns(TestNewYearDate);

            var dateTime = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.FilePreparationDate())
                .Returns(TestPreparationDate);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.CheckDeliveryFAMs(delivery.Object, It.IsAny<Func<ILearningDeliveryFAM, bool>>()))
                .Returns(expectation);

            var sut = new ULN_10Rule(handler.Object, academicYear.Object, dateTime.Object, fileData.Object, commonOps.Object);

            // act
            var result = sut.HasQualifyingMonitor(delivery.Object);

            // assert
            Assert.Equal(expectation, result);

            handler.VerifyAll();
            academicYear.VerifyAll();
            dateTime.VerifyAll();
            fileData.VerifyAll();
            commonOps.VerifyAll();
        }

        [Theory]
        [InlineData(1, false)]
        [InlineData(2, false)]
        [InlineData(3, false)]
        [InlineData(4, false)]
        [InlineData(5, true)]
        [InlineData(6, true)]
        public void HasQualifyingPlannedDurationMeetsExpectation(int duration, bool expectation)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var academicYear = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            academicYear
                .Setup(x => x.JanuaryFirst())
                .Returns(TestNewYearDate);

            var dateTime = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTime
                .Setup(x => x.DaysBetween(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(duration);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.FilePreparationDate())
                .Returns(TestPreparationDate);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            var sut = new ULN_10Rule(handler.Object, academicYear.Object, dateTime.Object, fileData.Object, commonOps.Object);

            // act
            var result = sut.HasQualifyingPlannedDuration(delivery.Object);

            // assert
            Assert.Equal(expectation, result);

            handler.VerifyAll();
            academicYear.VerifyAll();
            dateTime.VerifyAll();
            fileData.VerifyAll();
            commonOps.VerifyAll();
        }

        /// <summary>
        /// Has actual end date meets expectation
        /// </summary>
        /// <param name="actEnd">The act end.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("2018-04-02", true)]
        [InlineData(null, false)]
        public void HasActualEndDateMeetsExpectation(string actEnd, bool expectation)
        {
            // arrange
            var testDate = GetNullableDate(actEnd);
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.LearnActEndDateNullable)
                .Returns(testDate);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var academicYear = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            academicYear
                .Setup(x => x.JanuaryFirst())
                .Returns(TestNewYearDate);

            var dateTime = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.FilePreparationDate())
                .Returns(TestPreparationDate);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            var sut = new ULN_10Rule(handler.Object, academicYear.Object, dateTime.Object, fileData.Object, commonOps.Object);

            // act
            var result = sut.HasActualEndDate(delivery.Object);

            // assert
            Assert.Equal(expectation, result);

            handler.VerifyAll();
            academicYear.VerifyAll();
            dateTime.VerifyAll();
            fileData.VerifyAll();
            commonOps.VerifyAll();
        }

        /// <summary>
        /// Has qualifying actual duration meets expectation
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(1, false)]
        [InlineData(2, false)]
        [InlineData(3, false)]
        [InlineData(4, false)]
        [InlineData(5, true)]
        [InlineData(6, true)]
        public void HasQualifyingActualDurationMeetsExpectation(int duration, bool expectation)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.LearnActEndDateNullable)
                .Returns(DateTime.Today); // any old date for the test...

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var academicYear = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            academicYear
                .Setup(x => x.JanuaryFirst())
                .Returns(TestNewYearDate);

            var dateTime = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTime
                .Setup(x => x.DaysBetween(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(duration);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.FilePreparationDate())
                .Returns(TestPreparationDate);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            var sut = new ULN_10Rule(handler.Object, academicYear.Object, dateTime.Object, fileData.Object, commonOps.Object);

            // act
            var result = sut.HasQualifyingActualDuration(delivery.Object);

            // assert
            Assert.Equal(expectation, result);

            handler.VerifyAll();
            academicYear.VerifyAll();
            dateTime.VerifyAll();
            fileData.VerifyAll();
            commonOps.VerifyAll();
        }

        /// <summary>
        /// Has qualifying actual duration meets expectation
        /// </summary>
        /// <param name="duration">The duration.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(59, false)]
        [InlineData(60, false)]
        [InlineData(61, true)]
        public void IsOutsideLeniencyPeriodMeetsExpectation(int duration, bool expectation)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var academicYear = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            academicYear
                .Setup(x => x.JanuaryFirst())
                .Returns(TestNewYearDate);

            var dateTime = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTime
                .Setup(x => x.DaysBetween(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(duration);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.FilePreparationDate())
                .Returns(TestPreparationDate);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            var sut = new ULN_10Rule(handler.Object, academicYear.Object, dateTime.Object, fileData.Object, commonOps.Object);

            // act
            var result = sut.IsOutsideLeniencyPeriod(delivery.Object);

            // assert
            Assert.Equal(expectation, result);

            handler.VerifyAll();
            academicYear.VerifyAll();
            dateTime.VerifyAll();
            fileData.VerifyAll();
            commonOps.VerifyAll();
        }

        /// <summary>
        /// Gets the nullable date.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>a nullable date</returns>
        public DateTime? GetNullableDate(string candidate) =>
            string.IsNullOrWhiteSpace(candidate) ? (DateTime?)null : DateTime.Parse(candidate);

        /// <summary>
        /// Invalid item raises validation message.
        /// dates are deliberately out of sync to ensure the mock's are controlling the flow
        /// </summary>
        [Fact]
        public void InvalidItemRaisesValidationMessage()
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var testStart = DateTime.Parse("2016-05-01");
            var testEnd = DateTime.Parse("2017-05-01");

            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(y => y.FundModel)
                .Returns(99);
            delivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(testStart);
            delivery
                .SetupGet(y => y.LearnPlanEndDate)
                .Returns(testEnd);
            delivery
                .SetupGet(y => y.LearnActEndDateNullable)
                .Returns(DateTime.Today);

            var deliveries = new ILearningDelivery[] { delivery.Object };

            var learner = new Mock<ILearner>();
            learner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            learner
                .SetupGet(x => x.ULN)
                .Returns(ValidationConstants.TemporaryULN);
            learner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(RuleNameConstants.ULN_10, LearnRefNumber, 0, It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("ULN", ValidationConstants.TemporaryULN))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("FundModel", 99))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnStartDate", AbstractRule.AsRequiredCultureDate(testStart)))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnPlanEndDate", AbstractRule.AsRequiredCultureDate(testEnd)))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnDelFAMType", "SOF"))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnDelFAMCode", "1"))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var academicYear = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            academicYear
                .Setup(x => x.JanuaryFirst())
                .Returns(TestNewYearDate);

            var dateTime = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTime
                .Setup(x => x.DaysBetween(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsInOrder(4, 5, 61);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.FilePreparationDate())
                .Returns(TestPreparationDate);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.IsLearnerInCustody(delivery.Object))
                .Returns(false);
            commonOps
                .Setup(x => x.HasQualifyingFunding(delivery.Object, 99))
                .Returns(true);

            var sut = new ULN_10Rule(handler.Object, academicYear.Object, dateTime.Object, fileData.Object, commonOps.Object);

            // post construction mock setups
            commonOps
                .Setup(x => x.CheckDeliveryFAMs(delivery.Object, sut.IsHigherEducationFundingCouncilEngland))
                .Returns(true);

            // act
            sut.Validate(learner.Object);

            // assert
            handler.VerifyAll();
            academicYear.VerifyAll();
            dateTime.VerifyAll();
            fileData.VerifyAll();
            commonOps.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        [Fact]
        public void ValidItemDoesNotRaiseValidationMessage()
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var testStart = DateTime.Parse("2016-05-01");
            var testEnd = DateTime.Parse("2017-05-01");

            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(y => y.FundModel)
                .Returns(99);
            delivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(testStart);
            delivery
                .SetupGet(y => y.LearnPlanEndDate)
                .Returns(testEnd);
            delivery
                .SetupGet(y => y.LearnActEndDateNullable)
                .Returns(DateTime.Today);

            var deliveries = new ILearningDelivery[] { delivery.Object };

            var learner = new Mock<ILearner>();
            learner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            learner
                .SetupGet(x => x.ULN)
                .Returns(ValidationConstants.TemporaryULN);
            learner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var academicYear = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            academicYear
                .Setup(x => x.JanuaryFirst())
                .Returns(TestNewYearDate);

            var dateTime = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTime
                .Setup(x => x.DaysBetween(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsInOrder(4, 5, 60);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.FilePreparationDate())
                .Returns(TestPreparationDate);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.IsLearnerInCustody(delivery.Object))
                .Returns(false);
            commonOps
                .Setup(x => x.HasQualifyingFunding(delivery.Object, 99))
                .Returns(true);

            var sut = new ULN_10Rule(handler.Object, academicYear.Object, dateTime.Object, fileData.Object, commonOps.Object);

            // post construction mock setups
            commonOps
                .Setup(x => x.CheckDeliveryFAMs(delivery.Object, sut.IsHigherEducationFundingCouncilEngland))
                .Returns(true);

            // act
            sut.Validate(learner.Object);

            // assert
            handler.VerifyAll();
            academicYear.VerifyAll();
            dateTime.VerifyAll();
            fileData.VerifyAll();
            commonOps.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public ULN_10Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var academicYear = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            academicYear
                .Setup(x => x.JanuaryFirst())
                .Returns(TestNewYearDate);

            var dateTime = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.FilePreparationDate())
                .Returns(TestPreparationDate);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            return new ULN_10Rule(handler.Object, academicYear.Object, dateTime.Object, fileData.Object, commonOps.Object);
        }
    }
}
