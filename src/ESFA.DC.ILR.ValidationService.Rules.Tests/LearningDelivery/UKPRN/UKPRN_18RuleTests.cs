using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.UKPRN;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.UKPRN
{
    public class UKPRN_18RuleTests
    {
        /// <summary>
        /// The test provider identifier
        /// </summary>
        public const int TestProviderID = 123456789;

        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new UKPRN_18Rule(null, fileData.Object, commonOps.Object, fcsData.Object));
        }

        /// <summary>
        /// New rule with null file data throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullFileDataThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new UKPRN_18Rule(handler.Object, null, commonOps.Object, fcsData.Object));
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
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new UKPRN_18Rule(handler.Object, fileData.Object, null, fcsData.Object));
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
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new UKPRN_18Rule(handler.Object, fileData.Object, commonOps.Object, null));
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
            Assert.Equal("UKPRN_18", result);
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
            Assert.Equal(RuleNameConstants.UKPRN_18, result);
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
        /// Funding stream period code meets expectation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">The expectation.</param>
        [Theory]
        [InlineData("AEBC-19TRN1920", FundingStreamPeriodCodeConstants.AEBC_19TRN1920)]
        [InlineData("AEBC-ASCL1920", FundingStreamPeriodCodeConstants.AEBC_ASCL1920)]
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
        [InlineData(FundingStreamPeriodCodeConstants.AEBC_19TRN1920)]
        [InlineData(FundingStreamPeriodCodeConstants.AEBC_ASCL1920)]
        public void FundingStreamsMeetsExpectation(string candidate)
        {
            // arrange
            var sut = NewRule();

            // act / assert
            Assert.Contains(candidate, sut.FundingStreams);
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
        /// Has disqualifying monitor with null fams returns false
        /// </summary>
        [Fact]
        public void HasDisqualifyingMonitorWithNullFAMsReturnsFalse()
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

            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);

            var sut = new UKPRN_18Rule(handler.Object, fileData.Object, commonOps.Object, fcsData.Object);

            // act
            var result = sut.HasDisqualifyingMonitor(mockItem.Object);

            // assert
            Assert.False(result);

            handler.VerifyAll();
            commonOps.VerifyAll();
            fileData.VerifyAll();
            fcsData.VerifyAll();
        }

        /// <summary>
        /// Monitoring code meets expectation.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData("SOF105", Monitoring.Delivery.ESFAAdultFunding)]
        [InlineData("LDM357", Monitoring.Delivery.AdultEducationBudgets)]
        public void MonitoringCodeMeetsExpectation(string expectation, string candidate)
        {
            // arrange / act / assert
            Assert.Equal(expectation, candidate);
        }

        /// <summary>
        /// Is adult education budgets meets expectation
        /// </summary>
        /// <param name="famType">Type of FAM.</param>
        /// <param name="famCode">The FAM code.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("LDM", "357", true)] // Monitoring.Delivery.AdultEducationBudgets
        [InlineData("ACT", "1", false)] // Monitoring.Delivery.ApprenticeshipFundedThroughAContractForServicesWithEmployer
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
        public void IsAdultEducationBudgetsMeetsExpectation(string famType, string famCode, bool expectation)
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
            var result = sut.IsAdultEducationBudgets(fam.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Type of funding meets expectation.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData(35, TypeOfFunding.AdultSkills)]
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
                .Setup(x => x.HasQualifyingFunding(mockItem.Object, 35))
                .Returns(expectation);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.UKPRN())
                .Returns(TestProviderID);

            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);

            var sut = new UKPRN_18Rule(handler.Object, fileData.Object, commonOps.Object, fcsData.Object);

            // act
            var result = sut.HasQualifyingModel(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);

            handler.VerifyAll();
            commonOps.VerifyAll();
            fileData.VerifyAll();
            fcsData.VerifyAll();
        }

        /// <summary>
        /// Is esfa adult funding meets expectation
        /// </summary>
        /// <param name="famType">Type of FAM.</param>
        /// <param name="famCode">The FAM code.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("LDM", "357", false)] // Monitoring.Delivery.AdultEducationBudgets
        [InlineData("ACT", "1", false)] // Monitoring.Delivery.ApprenticeshipFundedThroughAContractForServicesWithEmployer
        [InlineData("LDM", "034", false)] // Monitoring.Delivery.OLASSOffendersInCustody
        [InlineData("FFI", "1", false)] // Monitoring.Delivery.FullyFundedLearningAim
        [InlineData("FFI", "2", false)] // Monitoring.Delivery.CoFundedLearningAim
        [InlineData("LDM", "363", false)] // Monitoring.Delivery.InReceiptOfLowWages
        [InlineData("LDM", "318", false)] // Monitoring.Delivery.MandationToSkillsTraining
        [InlineData("LDM", "328", false)] // Monitoring.Delivery.ReleasedOnTemporaryLicence
        [InlineData("LDM", "347", false)] // Monitoring.Delivery.SteelIndustriesRedundancyTraining
        [InlineData("SOF", "1", false)] // Monitoring.Delivery.HigherEducationFundingCouncilEngland
        [InlineData("SOF", "107", false)] // Monitoring.Delivery.ESFA16To19Funding
        [InlineData("SOF", "105", true)] // Monitoring.Delivery.ESFAAdultFunding
        [InlineData("SOF", "110", false)] // Monitoring.Delivery.GreaterManchesterCombinedAuthority
        [InlineData("SOF", "111", false)] // Monitoring.Delivery.LiverpoolCityRegionCombinedAuthority
        [InlineData("SOF", "112", false)] // Monitoring.Delivery.WestMidlandsCombinedAuthority
        [InlineData("SOF", "113", false)] // Monitoring.Delivery.WestOfEnglandCombinedAuthority
        [InlineData("SOF", "114", false)] // Monitoring.Delivery.TeesValleyCombinedAuthority
        [InlineData("SOF", "115", false)] // Monitoring.Delivery.CambridgeshireAndPeterboroughCombinedAuthority
        [InlineData("SOF", "116", false)] // Monitoring.Delivery.GreaterLondonAuthority
        public void IsESFAAdultFundingMeetsExpectation(string famType, string famCode, bool expectation)
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
            var result = sut.IsESFAAdultFunding(fam.Object);

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

            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);

            var sut = new UKPRN_18Rule(handler.Object, fileData.Object, commonOps.Object, fcsData.Object);

            // act
            var result = sut.HasQualifyingMonitor(mockItem.Object);

            // assert
            Assert.False(result);

            handler.VerifyAll();
            commonOps.VerifyAll();
            fileData.VerifyAll();
            fcsData.VerifyAll();
        }

        /// <summary>
        /// Has funding relationship meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(FundingStreamPeriodCodeConstants.LEVY1799, false)]
        [InlineData(FundingStreamPeriodCodeConstants.NONLEVY2019, false)]
        [InlineData(FundingStreamPeriodCodeConstants.AEBC1819, false)]
        [InlineData(FundingStreamPeriodCodeConstants.AEBC1920, false)]
        [InlineData(FundingStreamPeriodCodeConstants.AEBC_19TRN1920, true)]
        [InlineData(FundingStreamPeriodCodeConstants.AEBC_ASCL1920, true)]
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

            var allocation = new Mock<IFcsContractAllocation>(MockBehavior.Strict);
            allocation
                .SetupGet(x => x.FundingStreamPeriodCode)
                .Returns(candidate);

            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            fcsData
                .Setup(x => x.GetContractAllocationsFor(TestProviderID))
                .Returns(new IFcsContractAllocation[] { allocation.Object });

            var sut = new UKPRN_18Rule(handler.Object, fileData.Object, commonOps.Object, fcsData.Object);

            // act
            var result = sut.HasDisQualifyingFundingRelationship(x => true);

            // assert
            Assert.Equal(expectation, result);

            handler.VerifyAll();
            commonOps.VerifyAll();
            fileData.VerifyAll();
            fcsData.VerifyAll();
        }

        /// <summary>
        /// Has start after stop date with null date returns false
        /// </summary>
        [Fact]
        public void HasStartAfterStopDateWithNullDateReturnsFalse()
        {
            // arrange
            var allocation = new Mock<IFcsContractAllocation>(MockBehavior.Strict);
            allocation
                .SetupGet(x => x.StopNewStartsFromDate)
                .Returns((DateTime?)null);

            var delivery = new Mock<ILearningDelivery>(MockBehavior.Strict);
            delivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(DateTime.Today);

            var sut = NewRule();

            // act
            var result = sut.HasStartedAfterStopDate(allocation.Object, delivery.Object);

            // assert
            Assert.False(result);

            allocation.VerifyAll();
            delivery.VerifyAll();
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// dates are deliberately out of sync to ensure the mock's are controlling the flow
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData(FundingStreamPeriodCodeConstants.AEBC_19TRN1920)]
        [InlineData(FundingStreamPeriodCodeConstants.AEBC_ASCL1920)]
        public void InvalidItemRaisesValidationMessage(string candidate)
        {
            // arrange
            const string LearnRefNumber = "123456789X";
            var thresholdDate = DateTime.Parse("2017-05-01");

            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(thresholdDate);
            delivery
                .SetupGet(y => y.FundModel)
                .Returns(35);
            delivery
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(7);

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
                .Setup(x => x.Handle(RuleNameConstants.UKPRN_18, LearnRefNumber, 0, It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("UKPRN", TestProviderID))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("FundModel", 35))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("ProgType", 7))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnStartDate", AbstractRule.AsRequiredCultureDate(thresholdDate)))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(delivery.Object, 35))
                .Returns(true);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.UKPRN())
                .Returns(TestProviderID);

            // this will induce the error
            var allocation = new Mock<IFcsContractAllocation>(MockBehavior.Strict);
            allocation
                .SetupGet(x => x.FundingStreamPeriodCode)
                .Returns(candidate);
            allocation
                .SetupGet(x => x.StopNewStartsFromDate)
                .Returns(thresholdDate);

            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            fcsData
                .Setup(x => x.GetContractAllocationsFor(TestProviderID))
                .Returns(new IFcsContractAllocation[] { allocation.Object });

            var sut = new UKPRN_18Rule(handler.Object, fileData.Object, commonOps.Object, fcsData.Object);

            // post construction setup
            commonOps
                .Setup(x => x.CheckDeliveryFAMs(delivery.Object, sut.IsAdultEducationBudgets))
                .Returns(false);
            commonOps
                .Setup(x => x.CheckDeliveryFAMs(delivery.Object, sut.IsESFAAdultFunding))
                .Returns(true);

            // act
            sut.Validate(learner.Object);

            // assert
            handler.VerifyAll();
            commonOps.VerifyAll();
            fileData.VerifyAll();
            fcsData.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// dates are deliberately out of sync to ensure the mock's are controlling the flow
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData(FundingStreamPeriodCodeConstants.AEBC_19TRN1920)]
        [InlineData(FundingStreamPeriodCodeConstants.AEBC_ASCL1920)]
        public void ValidItemDoesNotRaiseValidationMessage(string candidate)
        {
            // arrange
            const string LearnRefNumber = "123456789X";
            var thresholdDate = DateTime.Parse("2017-05-01");

            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(thresholdDate);
            delivery
                .SetupGet(y => y.FundModel)
                .Returns(35);

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
                .Setup(x => x.HasQualifyingFunding(delivery.Object, 35))
                .Returns(true);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.UKPRN())
                .Returns(TestProviderID);

            // this will induce the error
            var allocation = new Mock<IFcsContractAllocation>(MockBehavior.Strict);
            allocation
                .SetupGet(x => x.FundingStreamPeriodCode)
                .Returns(candidate);
            allocation
                .SetupGet(x => x.StopNewStartsFromDate)
                .Returns(thresholdDate.AddDays(1));

            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            fcsData
                .Setup(x => x.GetContractAllocationsFor(TestProviderID))
                .Returns(new IFcsContractAllocation[] { allocation.Object });

            var sut = new UKPRN_18Rule(handler.Object, fileData.Object, commonOps.Object, fcsData.Object);

            // post construction setup
            commonOps
                .Setup(x => x.CheckDeliveryFAMs(delivery.Object, sut.IsAdultEducationBudgets))
                .Returns(false);
            commonOps
                .Setup(x => x.CheckDeliveryFAMs(delivery.Object, sut.IsESFAAdultFunding))
                .Returns(true);

            // act
            sut.Validate(learner.Object);

            // assert
            handler.VerifyAll();
            commonOps.VerifyAll();
            fileData.VerifyAll();
            fcsData.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public UKPRN_18Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.UKPRN())
                .Returns(TestProviderID);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);

            return new UKPRN_18Rule(handler.Object, fileData.Object, commonOps.Object, fcsData.Object);
        }
    }
}