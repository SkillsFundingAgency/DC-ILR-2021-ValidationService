using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.UKPRN;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.UKPRN
{
    /// <summary>
    /// learning delivery funding and monitoring rule 74 tests
    /// </summary>
    public class UKPRN_10RuleTests
    {
        /// <summary>
        /// The test provider identifier
        /// </summary>
        public const int TestProviderID = 123456789;

        /// <summary>
        /// The test start date
        /// </summary>
        public static readonly DateTime TestStartDate = DateTime.Parse("2018-04-01");

        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            var academicYear = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new UKPRN_10Rule(null, fileData.Object, academicYear.Object, commonOps.Object, fcsData.Object));
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
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new UKPRN_10Rule(handler.Object, null, academicYear.Object, commonOps.Object, fcsData.Object));
        }

        /// <summary>
        /// New rule with null academic year throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullAcademicYearThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new UKPRN_10Rule(handler.Object, fileData.Object, null, commonOps.Object, fcsData.Object));
        }

        /// <summary>
        /// New rule with null common ops throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullCommonOpsThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            var academicYear = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new UKPRN_10Rule(handler.Object, fileData.Object, academicYear.Object, null, fcsData.Object));
        }

        /// <summary>
        /// New rule with null FCS data throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullFCSDataThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            var academicYear = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new UKPRN_10Rule(handler.Object, fileData.Object, academicYear.Object, commonOps.Object, null));
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
            Assert.Equal("UKPRN_10", result);
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
            Assert.Equal(RuleNameConstants.UKPRN_10, result);
        }

        /// <summary>
        /// Funding stream period code meets expectation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">The expectation.</param>
        [Theory]
        [InlineData("LEVY1799", FundingStreamPeriodCodeConstants.LEVY1799)]
        [InlineData("NONLEVY2019", FundingStreamPeriodCodeConstants.NONLEVY2019)]
        public void FundingStreamPeriodCodeMeetsExpectation(string candidate, string expectation)
        {
            // arrange / act / assert
            Assert.Equal(expectation, candidate);
        }

        /// <summary>
        /// Funding streams meet expectation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData(FundingStreamPeriodCodeConstants.LEVY1799)]
        [InlineData(FundingStreamPeriodCodeConstants.NONLEVY2019)]
        public void FundingStreamsMeetsExpectation(string candidate)
        {
            // arrange
            var sut = NewRule();

            // act / assert
            Assert.Contains(candidate, sut.FundingStreams);
        }

        /// <summary>
        /// First viable start meets expectation.
        /// </summary>
        [Fact]
        public void FirstViableStartMeetsExpectation()
        {
            // arrange
            var sut = NewRule();

            // act / assert
            Assert.Equal(DateTime.Parse("2017-05-01"), sut.FirstViableStart);
        }

        /// <summary>
        /// Academic year start date meets expectation.
        /// </summary>
        [Fact]
        public void AcademicYearStartDateMeetsExpectation()
        {
            // arrange
            var sut = NewRule();

            // act / assert
            Assert.Equal(TestStartDate, sut.AcademicYearStartDate);
        }

        /// <summary>
        /// Provider ukprn meets expectation.
        /// </summary>
        [Fact]
        public void ProviderUKPRNMeetsExpectation()
        {
            // arrange
            var sut = NewRule();

            // act / assert
            Assert.Equal(TestProviderID, sut.ProviderUKPRN);
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
        /// Gets the nullable date.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>a nullable date</returns>
        public DateTime? GetNullableDate(string candidate) =>
            string.IsNullOrWhiteSpace(candidate) ? (DateTime?)null : DateTime.Parse(candidate);

        /// <summary>
        /// Has disqualifying end date meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="yearStart">The year start.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(null, "2018-03-01", false)]
        [InlineData("2018-04-02", "2018-04-01", false)]
        [InlineData("2018-04-01", "2018-04-01", false)]
        [InlineData("2018-03-31", "2018-04-01", true)]
        public void HasDisqualifyingEndDateMeetsExpectation(string candidate, string yearStart, bool expectation)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.LearnActEndDateNullable)
                .Returns(GetNullableDate(candidate));

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.UKPRN())
                .Returns(TestProviderID);

            var academicYear = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            academicYear
                .Setup(x => x.Start())
                .Returns(DateTime.Parse(yearStart));

            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);

            var sut = new UKPRN_10Rule(handler.Object, fileData.Object, academicYear.Object, commonOps.Object, fcsData.Object);

            // act
            var result = sut.HasDisqualifyingEndDate(delivery.Object);

            // assert
            Assert.Equal(expectation, result);

            handler.VerifyAll();
            commonOps.VerifyAll();
            fileData.VerifyAll();
            academicYear.VerifyAll();
            fcsData.VerifyAll();
        }

        /// <summary>
        /// Type of funding meets expectation.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData(36, TypeOfFunding.ApprenticeshipsFrom1May2017)]
        public void TypeOfFundingMeetsExpectation(int expectation, int candidate)
        {
            // arrange / act / assert
            Assert.Equal(expectation, candidate);
        }

        /// <summary>
        /// Has qualifying (fund) model meets expectation
        /// </summary>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void HasQualifyingModelMeetsExpectation(bool expectation)
        {
            // arrange
            var mockItem = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(mockItem.Object, 36)) // TypeOfFunding.ApprenticeshipsFrom1May2017,
                .Returns(expectation);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.UKPRN())
                .Returns(TestProviderID);

            var academicYear = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            academicYear
                .Setup(x => x.Start())
                .Returns(TestStartDate);

            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);

            var sut = new UKPRN_10Rule(handler.Object, fileData.Object, academicYear.Object, commonOps.Object, fcsData.Object);

            // act
            var result = sut.HasQualifyingModel(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);

            handler.VerifyAll();
            commonOps.VerifyAll();
            fileData.VerifyAll();
            academicYear.VerifyAll();
            fcsData.VerifyAll();
        }

        /// <summary>
        /// Has qualifying start meets expectation
        /// </summary>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void HasQualifyingStartMeetsExpectation(bool expectation)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingStart(delivery.Object, DateTime.Parse("2017-05-01"), null))
                .Returns(expectation);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.UKPRN())
                .Returns(TestProviderID);

            var academicYear = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            academicYear
                .Setup(x => x.Start())
                .Returns(TestStartDate);

            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);

            var sut = new UKPRN_10Rule(handler.Object, fileData.Object, academicYear.Object, commonOps.Object, fcsData.Object);

            // act
            var result = sut.HasQualifyingStart(delivery.Object);

            // assert
            Assert.Equal(expectation, result);

            handler.VerifyAll();
            commonOps.VerifyAll();
            fileData.VerifyAll();
            academicYear.VerifyAll();
            fcsData.VerifyAll();
        }

        /// <summary>
        /// Monitoring code meets expectation.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData("ACT1", Monitoring.Delivery.ApprenticeshipFundedThroughAContractForServicesWithEmployer)]
        public void MonitoringCodeMeetsExpectation(string expectation, string candidate)
        {
            // arrange / act / assert
            Assert.Equal(expectation, candidate);
        }

        /// <summary>
        /// Has qualifying monitor meets expectation
        /// </summary>
        /// <param name="famType">The Learning Delivery FAM Type.</param>
        /// <param name="famCode">The Learning Delivery FAM Code.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("ACT", "1", true)] // Monitoring.Delivery.ApprenticeshipFundedThroughAContractForServicesWithEmployer
        [InlineData("LDM", "034", false)] // Monitoring.Delivery.OLASSOffendersInCustody
        [InlineData("FFI", "1", false)] // Monitoring.Delivery.FullyFundedLearningAim
        [InlineData("FFI", "2", false)] // Monitoring.Delivery.CoFundedLearningAim
        [InlineData("LDM", "363", false)] // Monitoring.Delivery.InReceiptOfLowWages
        [InlineData("LDM", "318", false)] // Monitoring.Delivery.MandationToSkillsTraining
        [InlineData("LDM", "328", false)] // Monitoring.Delivery.ReleasedOnTemporaryLicence
        [InlineData("LDM", "347", false)] // Monitoring.Delivery.SteelIndustriesRedundancyTraining
        [InlineData("SOF", "1", false)] // Monitoring.Delivery.HigherEducationFundingCouncilEngland
        [InlineData("SOF", "107", false)] // Monitoring.Delivery.ESFA16To19Funding
        [InlineData("SOF", "105", false)] // Monitoring.Delivery.ESFAAdultFunding
        [InlineData("SOF", "110", false)] // Monitoring.Delivery.GreaterManchesterCombinedAuthority
        [InlineData("SOF", "111", false)] // Monitoring.Delivery.LiverpoolCityRegionCombinedAuthority
        [InlineData("SOF", "112", false)] // Monitoring.Delivery.WestMidlandsCombinedAuthority
        [InlineData("SOF", "113", false)] // Monitoring.Delivery.WestOfEnglandCombinedAuthority
        [InlineData("SOF", "114", false)] // Monitoring.Delivery.TeesValleyCombinedAuthority
        [InlineData("SOF", "115", false)] // Monitoring.Delivery.CambridgeshireAndPeterboroughCombinedAuthority
        [InlineData("SOF", "116", false)] // Monitoring.Delivery.GreaterLondonAuthority
        public void HasQualifyingMonitorMeetsExpectation(string famType, string famCode, bool expectation)
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
            var result = sut.HasQualifyingMonitor(fam.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Has qualifying monitor with null fams returns false
        /// </summary>
        [Fact]
        public void HasQualifyingMonitorWithNullFAMsReturnsFalse()
        {
            // arrange
            var mockItem = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.CheckDeliveryFAMs(mockItem.Object, It.IsAny<Func<ILearningDeliveryFAM, bool>>()))
                .Returns(false);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.UKPRN())
                .Returns(TestProviderID);

            var academicYear = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            academicYear
                .Setup(x => x.Start())
                .Returns(TestStartDate);

            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);

            var sut = new UKPRN_10Rule(handler.Object, fileData.Object, academicYear.Object, commonOps.Object, fcsData.Object);

            // act
            var result = sut.HasQualifyingMonitor(mockItem.Object);

            // assert
            Assert.False(result);

            handler.VerifyAll();
            commonOps.VerifyAll();
            fileData.VerifyAll();
            academicYear.VerifyAll();
            fcsData.VerifyAll();
        }

        /// <summary>
        /// Has funding relationship meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(FundingStreamPeriodCodeConstants.LEVY1799, true)]
        [InlineData(FundingStreamPeriodCodeConstants.NONLEVY2019, true)]
        [InlineData(FundingStreamPeriodCodeConstants.AEBC1819, false)]
        [InlineData(FundingStreamPeriodCodeConstants.AEBC1920, false)]
        [InlineData(FundingStreamPeriodCodeConstants.AEBC_19TRN1920, false)]
        [InlineData(FundingStreamPeriodCodeConstants.AEBC_ASCL1920, false)]
        [InlineData(FundingStreamPeriodCodeConstants.AEBTO_TOL1819, false)]
        [InlineData(FundingStreamPeriodCodeConstants.AEBTO_TOL1920, false)]
        [InlineData(FundingStreamPeriodCodeConstants.AEB_19TRLS1920, false)]
        [InlineData(FundingStreamPeriodCodeConstants.AEB_19TRN1920, false)]
        [InlineData(FundingStreamPeriodCodeConstants.AEB_AS1920, false)]
        [InlineData(FundingStreamPeriodCodeConstants.AEB_ASLS1920, false)]
        [InlineData(FundingStreamPeriodCodeConstants.AEB_LS1819, false)]
        [InlineData(FundingStreamPeriodCodeConstants.AEB_LS1920, false)]
        [InlineData(FundingStreamPeriodCodeConstants.AEB_TOL1819, false)]
        [InlineData(FundingStreamPeriodCodeConstants.AEB_TOL1920, false)]
        [InlineData(FundingStreamPeriodCodeConstants.ALLB1819, false)]
        [InlineData(FundingStreamPeriodCodeConstants.ALLB1920, false)]
        [InlineData(FundingStreamPeriodCodeConstants.C16_18TRN1920, false)]
        public void HasFundingRelationshipMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.UKPRN())
                .Returns(TestProviderID);

            var academicYear = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            academicYear
                .Setup(x => x.Start())
                .Returns(TestStartDate);

            var allocation = new Mock<IFcsContractAllocation>(MockBehavior.Strict);
            allocation
                .SetupGet(x => x.FundingStreamPeriodCode)
                .Returns(candidate);

            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            fcsData
                .Setup(x => x.GetContractAllocationsFor(TestProviderID))
                .Returns(new IFcsContractAllocation[] { allocation.Object });

            var sut = new UKPRN_10Rule(handler.Object, fileData.Object, academicYear.Object, commonOps.Object, fcsData.Object);

            // act
            var result = sut.HasFundingRelationship();

            // assert
            Assert.Equal(expectation, result);

            handler.VerifyAll();
            commonOps.VerifyAll();
            fileData.VerifyAll();
            academicYear.VerifyAll();
            fcsData.VerifyAll();
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// dates are deliberately out of sync to ensure the mock's are controlling the flow
        /// </summary>
        [Fact]
        public void InvalidItemRaisesValidationMessage()
        {
            // arrange
            const string LearnRefNumber = "123456789X";
            var firstViableStart = DateTime.Parse("2017-05-01");

            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse("2017-04-01"));
            delivery
                .SetupGet(y => y.FundModel)
                .Returns(36);

            var deliveries = new ILearningDelivery[] { delivery.Object };

            var learner = new Mock<ILearner>();
            learner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            learner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(RuleNameConstants.UKPRN_10, LearnRefNumber, 0, It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("UKPRN", TestProviderID))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("FundModel", 36))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnDelFAMType", "ACT"))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnDelFAMCode", "1"))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingStart(delivery.Object, firstViableStart, null))
                .Returns(true);
            commonOps
                .Setup(x => x.HasQualifyingFunding(delivery.Object, 36)) // TypeOfFunding.ApprenticeshipsFrom1May2017,
                .Returns(true);
            commonOps
                .Setup(x => x.CheckDeliveryFAMs(delivery.Object, It.IsAny<Func<ILearningDeliveryFAM, bool>>()))
                .Returns(true);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.UKPRN())
                .Returns(TestProviderID);

            var academicYear = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            academicYear
                .Setup(x => x.Start())
                .Returns(TestStartDate);

            // this will induce the error
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            fcsData
                .Setup(x => x.GetContractAllocationsFor(TestProviderID))
                .Returns(new IFcsContractAllocation[] { });

            var sut = new UKPRN_10Rule(handler.Object, fileData.Object, academicYear.Object, commonOps.Object, fcsData.Object);

            // act
            sut.Validate(learner.Object);

            // assert
            handler.VerifyAll();
            commonOps.VerifyAll();
            fileData.VerifyAll();
            academicYear.VerifyAll();
            fcsData.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// dates are deliberately out of sync to ensure the mock's are controlling the flow
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData(FundingStreamPeriodCodeConstants.LEVY1799)]
        [InlineData(FundingStreamPeriodCodeConstants.NONLEVY2019)]
        public void ValidItemDoesNotRaiseValidationMessage(string candidate)
        {
            // arrange
            const string LearnRefNumber = "123456789X";
            var firstViableStart = DateTime.Parse("2017-05-01");

            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse("2017-04-01"));
            delivery
                .SetupGet(y => y.FundModel)
                .Returns(36);

            var deliveries = new ILearningDelivery[] { delivery.Object };

            var learner = new Mock<ILearner>();
            learner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            learner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingStart(delivery.Object, firstViableStart, null))
                .Returns(true);
            commonOps
                .Setup(x => x.HasQualifyingFunding(delivery.Object, 36)) // TypeOfFunding.ApprenticeshipsFrom1May2017,
                .Returns(true);
            commonOps
                .Setup(x => x.CheckDeliveryFAMs(delivery.Object, It.IsAny<Func<ILearningDeliveryFAM, bool>>()))
                .Returns(true);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.UKPRN())
                .Returns(TestProviderID);

            var academicYear = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            academicYear
                .Setup(x => x.Start())
                .Returns(TestStartDate);

            // this will induce the a pass
            var allocation = new Mock<IFcsContractAllocation>(MockBehavior.Strict);
            allocation
                .SetupGet(x => x.FundingStreamPeriodCode)
                .Returns(candidate);

            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            fcsData
                .Setup(x => x.GetContractAllocationsFor(TestProviderID))
                .Returns(new IFcsContractAllocation[] { allocation.Object });

            var sut = new UKPRN_10Rule(handler.Object, fileData.Object, academicYear.Object, commonOps.Object, fcsData.Object);

            // act
            sut.Validate(learner.Object);

            // assert
            handler.VerifyAll();
            commonOps.VerifyAll();
            fileData.VerifyAll();
            academicYear.VerifyAll();
            fcsData.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public UKPRN_10Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.UKPRN())
                .Returns(TestProviderID);

            var academicYear = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            academicYear
                .Setup(x => x.Start())
                .Returns(TestStartDate);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);

            return new UKPRN_10Rule(handler.Object, fileData.Object, academicYear.Object, commonOps.Object, fcsData.Object);
        }
    }
}
