using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.UKPRN;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.UKPRN
{
    public class UKPRN_19RuleTests
    {
        public const int TestProviderID = 123456789;

        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("UKPRN_19", result);
        }

        [Theory]
        [InlineData("AEB-19TRN1920", FundingStreamPeriodCodeConstants.AEB_19TRN1920)]
        [InlineData("AEB-AS1920", FundingStreamPeriodCodeConstants.AEB_AS1920)]
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
        [InlineData("SOF105", Monitoring.Delivery.ESFAAdultFunding)]
        [InlineData("LDM357", Monitoring.Delivery.AdultEducationBudgets)]
        public void MonitoringCodeMeetsExpectation(string expectation, string candidate)
        {
            Assert.Equal(expectation, candidate);
        }

        [Theory]
        [InlineData("LDM", "357", true)]
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
        [InlineData("SOF", "105", false)]
        [InlineData("SOF", "110", false)]
        [InlineData("SOF", "111", false)]
        [InlineData("SOF", "112", false)]
        [InlineData("SOF", "113", false)]
        [InlineData("SOF", "114", false)]
        [InlineData("SOF", "115", false)]
        [InlineData("SOF", "116", false)]
        public void IsAdultEducationBudgetsMeetsExpectation(string famType, string famCode, bool expectation)
        {
            var fam = new Mock<ILearningDeliveryFAM>();
            fam
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(famType);
            fam
                .SetupGet(y => y.LearnDelFAMCode)
                .Returns(famCode);

            var fams = new List<ILearningDeliveryFAM>
            {
                fam.Object
            };

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearningDeliveryFAMs)
                .Returns(fams);

            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);
            learningDeliveryFAMQS
               .Setup(x => x.HasLearningDeliveryFAMCodeForType(
                   mockDelivery.Object.LearningDeliveryFAMs,
                   "LDM",
                   "357"))
               .Returns(expectation);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fcsDataService = new Mock<IFCSDataService>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.UKPRN())
                .Returns(TestProviderID);

            var result = new UKPRN_19Rule(handler.Object, fileData.Object, learningDeliveryFAMQS.Object, fcsDataService.Object).IsAdultEducationBudgets(mockDelivery.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(35, FundModels.AdultSkills)]
        public void TypeOfFundingMeetsExpectation(int expectation, int candidate)
        {
            Assert.Equal(expectation, candidate);
        }

        [Theory]
        [InlineData(35, true)]
        [InlineData(25, false)]
        public void HasQualifyingModelMeetsExpectation(int fundModel, bool expectation)
        {
            var mockItem = new Mock<ILearningDelivery>();
            mockItem.Setup(x => x.FundModel).Returns(fundModel);

            Assert.Equal(NewRule().HasQualifyingModel(mockItem.Object), expectation);
        }

        [Theory]
        [InlineData("LDM", "357", false)]
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
        public void IsESFAAdultFundingMeetsExpectation(string famType, string famCode, bool expectation)
        {
            var fam = new Mock<ILearningDeliveryFAM>();
            fam
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(famType);
            fam
                .SetupGet(y => y.LearnDelFAMCode)
                .Returns(famCode);

            var fams = new List<ILearningDeliveryFAM>
            {
                fam.Object
            };

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearningDeliveryFAMs)
                .Returns(fams);

            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);
            learningDeliveryFAMQS
               .Setup(x => x.HasLearningDeliveryFAMCodeForType(
                   mockDelivery.Object.LearningDeliveryFAMs,
                   "SOF",
                   "105"))
               .Returns(expectation);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fcsDataService = new Mock<IFCSDataService>(MockBehavior.Strict);
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.UKPRN())
                .Returns(TestProviderID);

            var result = new UKPRN_19Rule(handler.Object, fileData.Object, learningDeliveryFAMQS.Object, fcsDataService.Object).IsESFAAdultFunding(mockDelivery.Object);

            Assert.Equal(expectation, result);
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
        [InlineData(FundingStreamPeriodCodeConstants.AEB_ASLS1920, false)]
        [InlineData(FundingStreamPeriodCodeConstants.AEB_LS1819, false)]
        [InlineData(FundingStreamPeriodCodeConstants.AEB_LS1920, false)]
        [InlineData(FundingStreamPeriodCodeConstants.AEB_TOL1819, false)]
        [InlineData(FundingStreamPeriodCodeConstants.AEB_TOL1920, false)]
        [InlineData(FundingStreamPeriodCodeConstants.ALLB1819, false)]
        [InlineData(FundingStreamPeriodCodeConstants.ALLB1920, false)]
        [InlineData(FundingStreamPeriodCodeConstants.C16_18TRN1920, false)]
        [InlineData(FundingStreamPeriodCodeConstants.AEB_19TRN1920, false)]
        [InlineData(FundingStreamPeriodCodeConstants.AEB_AS1920, false)]
        [InlineData(FundingStreamPeriodCodeConstants.AEB_19TRN2021, true)]
        [InlineData(FundingStreamPeriodCodeConstants.AEB_AS2021, true)]
        public void HasFundingRelationshipMeetsExpectation(string candidate, bool expectation)
        {
            var thresholdDate = DateTime.Parse("2017-05-01");

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);

            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.UKPRN())
                .Returns(TestProviderID);

            var allocation = new Mock<IFcsContractAllocation>(MockBehavior.Strict);
            allocation
                .SetupGet(x => x.FundingStreamPeriodCode)
                .Returns(candidate);
            allocation
                .SetupGet(x => x.StartDate)
                .Returns(thresholdDate);

            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            fcsData
                .Setup(x => x.GetContractAllocationsFor(TestProviderID))
                .Returns(new IFcsContractAllocation[] { allocation.Object });

            var sut = new UKPRN_19Rule(handler.Object, fileData.Object, learningDeliveryFAMQS.Object, fcsData.Object);

            var result = sut.HasDisQualifyingFundingRelationship(x => true);

            Assert.Equal(expectation, result);

            handler.VerifyAll();
            learningDeliveryFAMQS.VerifyAll();
            fileData.VerifyAll();
            fcsData.VerifyAll();
        }

        [Fact]
        public void HasStartAfterStopDateWithNullDateReturnsFalse()
        {
            var allocation = new Mock<IFcsContractAllocation>(MockBehavior.Strict);
            allocation
                .SetupGet(x => x.StopNewStartsFromDate)
                .Returns((DateTime?)null);

            var delivery = new Mock<ILearningDelivery>(MockBehavior.Strict);
            delivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(DateTime.Today);

            var sut = NewRule();

            var result = sut.HasStartedAfterStopDate(allocation.Object, delivery.Object);

            Assert.False(result);

            allocation.VerifyAll();
            delivery.VerifyAll();
        }

        [Theory]
        [InlineData(FundingStreamPeriodCodeConstants.AEB_19TRN2021)]
        [InlineData(FundingStreamPeriodCodeConstants.AEB_AS2021)]
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
                .Setup(x => x.Handle(RuleNameConstants.UKPRN_19, LearnRefNumber, 0, It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("UKPRN", TestProviderID))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("FundModel", 35))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnStartDate", AbstractRule.AsRequiredCultureDate(thresholdDate)))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnDelFAMType", "LDM"))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnDelFAMCode", "357"))
                .Returns(new Mock<IErrorMessageParameter>().Object);

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
            allocation
                .SetupGet(x => x.StartDate)
                .Returns(thresholdDate);

            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            fcsData
                .Setup(x => x.GetContractAllocationsFor(TestProviderID))
                .Returns(new IFcsContractAllocation[] { allocation.Object });

            var learningDeliveryFamqs = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);
            learningDeliveryFamqs
               .Setup(x => x.HasLearningDeliveryFAMCodeForType(
                   delivery.Object.LearningDeliveryFAMs,
                   "SOF",
                   "105"))
               .Returns(true);
            learningDeliveryFamqs
               .Setup(x => x.HasLearningDeliveryFAMCodeForType(
                   delivery.Object.LearningDeliveryFAMs,
                   "LDM",
                   "357"))
               .Returns(true);

            learningDeliveryFamqs
                .Setup(x => x.HasLearningDeliveryFAMType(
                    delivery.Object.LearningDeliveryFAMs,
                    "RES"))
                .Returns(false);

            var sut = new UKPRN_19Rule(handler.Object, fileData.Object, learningDeliveryFamqs.Object, fcsData.Object);

            sut.Validate(learner.Object);

            handler.VerifyAll();
            learningDeliveryFamqs.VerifyAll();
            fileData.VerifyAll();
            fcsData.VerifyAll();
        }

        [Theory]
        [InlineData(FundingStreamPeriodCodeConstants.AEB_19TRN2021)]
        [InlineData(FundingStreamPeriodCodeConstants.AEB_AS2021)]
        public void ValidItemDoesNotRaiseValidationMessage(string candidate)
        {
            const string LearnRefNumber = "123456789X";
            var thresholdDate = DateTime.Parse("2017-05-01");

            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(thresholdDate.AddDays(-1));
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
            allocation
                .SetupGet(x => x.StartDate)
                .Returns(thresholdDate);

            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            fcsData
                .Setup(x => x.GetContractAllocationsFor(TestProviderID))
                .Returns(new IFcsContractAllocation[] { allocation.Object });

            var learningDeliveryFamqs = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);
            learningDeliveryFamqs
               .Setup(x => x.HasLearningDeliveryFAMCodeForType(
                   delivery.Object.LearningDeliveryFAMs,
                   "SOF",
                   "105"))
               .Returns(true);
            learningDeliveryFamqs
               .Setup(x => x.HasLearningDeliveryFAMCodeForType(
                   delivery.Object.LearningDeliveryFAMs,
                   "LDM",
                   "357"))
               .Returns(true);

            learningDeliveryFamqs
                .Setup(x => x.HasLearningDeliveryFAMType(
                    delivery.Object.LearningDeliveryFAMs,
                    "RES"))
                .Returns(false);

            var sut = new UKPRN_19Rule(handler.Object, fileData.Object, learningDeliveryFamqs.Object, fcsData.Object);

            sut.Validate(learner.Object);

            handler.VerifyAll();
            learningDeliveryFamqs.VerifyAll();
            fileData.VerifyAll();
            fcsData.VerifyAll();
        }

        [Theory]
        [InlineData(FundingStreamPeriodCodeConstants.AEB_19TRN2021)]
        [InlineData(FundingStreamPeriodCodeConstants.AEB_AS2021)]
        public void ValidItemMultipleContractsDoesNotRaiseValidationMessage(string candidate)
        {
            const string LearnRefNumber = "123456789X";
            var thresholdDate = DateTime.Parse("2017-05-01");

            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(thresholdDate.AddDays(-1));
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
                .Returns(thresholdDate.AddDays(-1));
            allocation
                .SetupGet(x => x.StartDate)
                .Returns(thresholdDate);

            var allocation2 = new Mock<IFcsContractAllocation>(MockBehavior.Strict);
            allocation2
                .SetupGet(x => x.FundingStreamPeriodCode)
                .Returns(candidate);
            allocation2
                .SetupGet(x => x.StopNewStartsFromDate)
                .Returns((DateTime?)null);
            allocation2
                .SetupGet(x => x.StartDate)
                .Returns(thresholdDate.AddDays(1));

            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            fcsData
                .Setup(x => x.GetContractAllocationsFor(TestProviderID))
                .Returns(new IFcsContractAllocation[] { allocation.Object, allocation2.Object });

            var learningDeliveryFamqs = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);
            learningDeliveryFamqs
               .Setup(x => x.HasLearningDeliveryFAMCodeForType(
                   delivery.Object.LearningDeliveryFAMs,
                   "SOF",
                   "105"))
               .Returns(true);
            learningDeliveryFamqs
               .Setup(x => x.HasLearningDeliveryFAMCodeForType(
                   delivery.Object.LearningDeliveryFAMs,
                   "LDM",
                   "357"))
               .Returns(true);

            learningDeliveryFamqs
                .Setup(x => x.HasLearningDeliveryFAMType(
                    delivery.Object.LearningDeliveryFAMs,
                    "RES"))
                .Returns(false);

            var sut = new UKPRN_19Rule(handler.Object, fileData.Object, learningDeliveryFamqs.Object, fcsData.Object);

            sut.Validate(learner.Object);

            handler.VerifyAll();
            learningDeliveryFamqs.VerifyAll();
            fileData.VerifyAll();
            fcsData.VerifyAll();
        }

        [Fact]
        public void HasNoRestartFamType_Fails()
        {
            IEnumerable<ILearningDeliveryFAM> deliveryFams = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = "RES" }
            };

            var mockFamQuerySrvc = new Mock<ILearningDeliveryFAMQueryService>();
            mockFamQuerySrvc.Setup(x => x.HasLearningDeliveryFAMType(deliveryFams, "RES")).Returns(true);

            var result = NewRule(null, null, mockFamQuerySrvc.Object, null).HasNoRestartFamType(deliveryFams);

            result.Should().BeFalse();
            mockFamQuerySrvc.Verify(x => x.HasLearningDeliveryFAMType(deliveryFams, "RES"), Times.Exactly(1));
        }

        [Fact]
        public void HasNoRestartFamType_Pass()
        {
            IEnumerable<ILearningDeliveryFAM> deliveryFams = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = "ADL" }
            };

            var mockFamQuerySrvc = new Mock<ILearningDeliveryFAMQueryService>();
            mockFamQuerySrvc.Setup(x => x.HasLearningDeliveryFAMType(deliveryFams, "RES")).Returns(false);

            var result = NewRule(null, null, mockFamQuerySrvc.Object, null).HasNoRestartFamType(deliveryFams);

            result.Should().BeTrue();
            mockFamQuerySrvc.Verify(x => x.HasLearningDeliveryFAMType(deliveryFams, "RES"), Times.Exactly(1));
        }

        public UKPRN_19Rule NewRule(
            IValidationErrorHandler handler = null,
            IFileDataService fileDataService = null,
            ILearningDeliveryFAMQueryService learningDeliveryFamqs = null,
            IFCSDataService fcsDataService = null)
        {
            var fileData = new Mock<IFileDataService>(MockBehavior.Strict);
            fileData
                .Setup(x => x.UKPRN())
                .Returns(TestProviderID);

            return new UKPRN_19Rule(
                handler ?? Mock.Of<IValidationErrorHandler>(),
                fileDataService ?? fileData.Object,
                learningDeliveryFamqs ?? Mock.Of<ILearningDeliveryFAMQueryService>(),
                fcsDataService ?? Mock.Of<IFCSDataService>());
        }
    }
}
