﻿using ESFA.DC.ILR.Model.Interface;
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
    public class UKPRN_17RuleTests
    {
        public const int TestProviderID = 123456789;

        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("UKPRN_17", result);
        }

        [Theory]
        [InlineData("16-18TRN1920", FundingStreamPeriodCodeConstants.C16_18TRN1920)]
        public void FundingStreamPeriodCodeMeetsExpectation(string candidate, string expectation)
        {
            Assert.Equal(expectation, candidate);
        }

        [Fact]
        public void ProviderUKPRNMeetsExpectation()
        {
            var sut = NewRule();

            Assert.Equal(TestProviderID, sut.ProviderUKPRN);
        }

        [Theory]
        [InlineData(25, TypeOfFunding.Age16To19ExcludingApprenticeships)]
        public void TypeOfFundingMeetsExpectation(int expectation, int candidate)
        {
            Assert.Equal(expectation, candidate);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void HasQualifyingModelMeetsExpectation(bool expectation)
        {
            var mockItem = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(mockItem.Object, 25))
                .Returns(expectation);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.UKPRN())
                .Returns(TestProviderID);

            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);

            var sut = new UKPRN_17Rule(handler.Object, fileData.Object, commonOps.Object, fcsData.Object);

            var result = sut.HasQualifyingModel(mockItem.Object);

            Assert.Equal(expectation, result);

            handler.VerifyAll();
            commonOps.VerifyAll();
            fileData.VerifyAll();
            fcsData.VerifyAll();
        }

        [Theory]
        [InlineData(24, TypeOfLearningProgramme.Traineeship)]
        public void TypeOfProgrammeMeetsExpectation(int expectation, int candidate)
        {
            Assert.Equal(expectation, candidate);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void IsTraineeshipMeetsExpectation(bool expectation)
        {
            var delivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.IsTraineeship(delivery.Object))
                .Returns(expectation);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.UKPRN())
                .Returns(TestProviderID);

            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);

            var sut = new UKPRN_17Rule(handler.Object, fileData.Object, commonOps.Object, fcsData.Object);

            var result = sut.IsTraineeship(delivery.Object);

            Assert.Equal(expectation, result);

            handler.VerifyAll();
            commonOps.VerifyAll();
            fileData.VerifyAll();
            fcsData.VerifyAll();
        }

        [Theory]
        [InlineData("SOF105", Monitoring.Delivery.ESFAAdultFunding)]
        public void MonitoringCodeMeetsExpectation(string expectation, string candidate)
        {
            Assert.Equal(expectation, candidate);
        }

        [Theory]
        [InlineData("ACT", "1", false)]
        [InlineData("LDM", "034", false)]
        [InlineData("FFI", "1", false)]
        [InlineData("FFI", "2", false)]
        [InlineData("LDM", "363", false)]
        [InlineData("LDM", "318", false)]
        [InlineData("LDM", "328", false)]
        [InlineData("LDM", "347", false)]
        [InlineData("SOF", "1", false)]
        [InlineData("SOF", "107", false)]
        [InlineData("SOF", "105", true)]
        [InlineData("SOF", "110", false)]
        [InlineData("SOF", "111", false)]
        [InlineData("SOF", "112", false)]
        [InlineData("SOF", "113", false)]
        [InlineData("SOF", "114", false)]
        [InlineData("SOF", "115", false)]
        [InlineData("SOF", "116", false)]
        public void HasQualifyingMonitorMeetsExpectation(string famType, string famCode, bool expectation)
        {
            var sut = NewRule();
            var fam = new Mock<ILearningDeliveryFAM>();
            fam
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(famType);
            fam
                .SetupGet(y => y.LearnDelFAMCode)
                .Returns(famCode);

            var result = sut.HasQualifyingMonitor(fam.Object);

            Assert.Equal(expectation, result);
        }

        [Fact]
        public void HasQualifyingMonitorWithNullFAMsReturnsFalse()
        {
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

            var sut = new UKPRN_17Rule(handler.Object, fileData.Object, commonOps.Object, fcsData.Object);

            var result = sut.HasQualifyingMonitor(mockItem.Object);

            Assert.False(result);

            handler.VerifyAll();
            commonOps.VerifyAll();
            fileData.VerifyAll();
            fcsData.VerifyAll();
        }

        [Theory]
        [InlineData(FundingStreamPeriodCodeConstants.LEVY1799, false)]
        [InlineData(FundingStreamPeriodCodeConstants.NONLEVY2019, false)]
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
        [InlineData(FundingStreamPeriodCodeConstants.C16_18TRN1920, true)]
        public void HasFundingRelationshipMeetsExpectation(string candidate, bool expectation)
        {
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

            var sut = new UKPRN_17Rule(handler.Object, fileData.Object, commonOps.Object, fcsData.Object);

            var result = sut.HasDisQualifyingFundingRelationship(x => true);

            Assert.Equal(expectation, result);

            handler.VerifyAll();
            commonOps.VerifyAll();
            fileData.VerifyAll();
            fcsData.VerifyAll();
        }

        [Theory]
        [InlineData(FundingStreamPeriodCodeConstants.C16_18TRN1920)]
        public void InvalidItemRaisesValidationMessage(string candidate)
        {
            const string LearnRefNumber = "123456789X";
            var thresholdDate = DateTime.Parse("2017-05-01");

            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(thresholdDate);
            delivery
                .SetupGet(y => y.FundModel)
                .Returns(25);
            delivery
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(24);

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
                .Setup(x => x.Handle(RuleNameConstants.UKPRN_17, LearnRefNumber, 0, It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("UKPRN", TestProviderID))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("FundModel", 25))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("ProgType", 24))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnStartDate", AbstractRule.AsRequiredCultureDate(thresholdDate)))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnDelFAMType", "SOF"))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnDelFAMCode", "105"))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(delivery.Object, 25))
                .Returns(true);
            commonOps
                .Setup(x => x.IsTraineeship(delivery.Object))
                .Returns(true);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.UKPRN())
                .Returns(TestProviderID);

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

            var sut = new UKPRN_17Rule(handler.Object, fileData.Object, commonOps.Object, fcsData.Object);

            commonOps
                .Setup(x => x.CheckDeliveryFAMs(delivery.Object, sut.HasQualifyingMonitor))
                .Returns(true);

            sut.Validate(learner.Object);

            handler.VerifyAll();
            commonOps.VerifyAll();
            fileData.VerifyAll();
            fcsData.VerifyAll();
        }

        [Theory]
        [InlineData(FundingStreamPeriodCodeConstants.C16_18TRN1920)]
        public void ValidItemDoesNotRaiseValidationMessage(string candidate)
        {
            const string LearnRefNumber = "123456789X";
            var thresholdDate = DateTime.Parse("2017-05-01");

            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(thresholdDate);
            delivery
                .SetupGet(y => y.FundModel)
                .Returns(25);
            delivery
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(24);

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
                .Setup(x => x.HasQualifyingFunding(delivery.Object, 25))
                .Returns(true);
            commonOps
                .Setup(x => x.IsTraineeship(delivery.Object))
                .Returns(true);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.UKPRN())
                .Returns(TestProviderID);

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

            var sut = new UKPRN_17Rule(handler.Object, fileData.Object, commonOps.Object, fcsData.Object);

            commonOps
                .Setup(x => x.CheckDeliveryFAMs(delivery.Object, sut.HasQualifyingMonitor))
                .Returns(true);

            sut.Validate(learner.Object);

            handler.VerifyAll();
            commonOps.VerifyAll();
            fileData.VerifyAll();
            fcsData.VerifyAll();
        }

        public UKPRN_17Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.UKPRN())
                .Returns(TestProviderID);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);

            return new UKPRN_17Rule(handler.Object, fileData.Object, commonOps.Object, fcsData.Object);
        }
    }
}
