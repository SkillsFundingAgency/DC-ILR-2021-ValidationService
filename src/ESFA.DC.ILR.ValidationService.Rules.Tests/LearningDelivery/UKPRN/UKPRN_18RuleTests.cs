using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.UKPRN;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.UKPRN
{
    public class UKPRN_18RuleTests : AbstractRuleTests<UKPRN_18Rule>
    {
        private readonly IEnumerable<string> _fundingStreamPeriodCodes = new HashSet<string>
        {
            FundingStreamPeriodCodeConstants.AEBC_19TRN1920,
            FundingStreamPeriodCodeConstants.AEBC_ASCL1920
        };

        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("UKPRN_18");
        }

        [Fact]
        public void FundModelCondition_ShouldMatch()
        {
            var fundModel = 35;
            NewRule().FundModelConditionMet(fundModel).Should().BeTrue();
        }

        [Fact]
        public void FundModelCondition_ShouldNotMatch()
        {
            var fundModel = 2;
            NewRule().FundModelConditionMet(fundModel).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMsCondition_ShouldMatch()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>();
            var mockLearningDeliveryFAMQueryService = new Mock<ILearningDeliveryFAMQueryService>();

            var result = true;

            mockLearningDeliveryFAMQueryService
                .Setup(x => x.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "SOF", "105"))
                .Returns(result);

            var rule18 = NewRule(learningDeliveryFAMQueryService: mockLearningDeliveryFAMQueryService.Object);

            rule18.LearningDeliveryFAMsConditionMet(learningDeliveryFAMs).Should().Be(result);
        }

        [Fact]
        public void FCTFundingCondition_ShouldPass()
        {
            var mockfcsDataServices = new Mock<IFCSDataService>();
            mockfcsDataServices
                .Setup(x => x.FundingRelationshipFCTExists(_fundingStreamPeriodCodes))
                .Returns(true);

            var rule18 = NewRule(fcsDataService: mockfcsDataServices.Object).FCTFundingConditionMet();

            rule18.Should().BeTrue();
            mockfcsDataServices.Verify(fcs => fcs.FundingRelationshipFCTExists(_fundingStreamPeriodCodes), Times.Once);
        }

        [Fact]
        public void FCTFundingCondition_ShouldFail()
        {
            var mockFcsDataServices = new Mock<IFCSDataService>();
            mockFcsDataServices
                .Setup(x => x.FundingRelationshipFCTExists(_fundingStreamPeriodCodes))
                .Returns(false);

            var rule18 = NewRule(fcsDataService: mockFcsDataServices.Object).FCTFundingConditionMet();

            rule18.Should().BeFalse();
            mockFcsDataServices.Verify(fcs => fcs.FundingRelationshipFCTExists(_fundingStreamPeriodCodes), Times.Once);
        }

        [Fact]
        public void FromDateCondition_IsFalse_WithNullFromDate()
        {
            string contractRef = "123";
            var startDate = DateTime.Now;
            DateTime? stopNewStartsFromDate = null;

            var mockContractAllocation = new Mock<IFcsContractAllocation>();
            mockContractAllocation.SetupGet(x => x.StopNewStartsFromDate).Returns(stopNewStartsFromDate);

            var mockFcsDataServices = new Mock<IFCSDataService>();
            mockFcsDataServices.Setup(x => x.GetContractAllocationFor(contractRef)).Returns(mockContractAllocation.Object);

            var rule18 = NewRule(fcsDataService: mockFcsDataServices.Object).StopNewStartsFromDateConditionMet(contractRef, startDate);

            rule18.Should().BeFalse();

            mockContractAllocation.VerifyGet(x => x.StopNewStartsFromDate, Times.Once);
            mockFcsDataServices.Verify(x => x.GetContractAllocationFor(contractRef), Times.Once);
        }

        [Fact]
        public void FromDateCondition_IsTrue_AsStartDateIsGreater()
        {
            string contractRef = "123";
            var startDate = new DateTime(2019, 04, 15);
            var stopNewStartsFromDate = new DateTime(2019, 01, 11);

            var mockContractAllocation = new Mock<IFcsContractAllocation>();
            mockContractAllocation.SetupGet(x => x.StopNewStartsFromDate).Returns(stopNewStartsFromDate);

            var mockFcsDataServices = new Mock<IFCSDataService>();
            mockFcsDataServices.Setup(x => x.GetContractAllocationFor(contractRef)).Returns(mockContractAllocation.Object);

            var rule18 = NewRule(fcsDataService: mockFcsDataServices.Object);

            rule18.StopNewStartsFromDateConditionMet(contractRef, startDate).Should().BeTrue();

            mockContractAllocation.VerifyGet(x => x.StopNewStartsFromDate, Times.AtLeastOnce);
            mockFcsDataServices.Verify(x => x.GetContractAllocationFor(contractRef), Times.Once);
        }

        [Fact]
        public void FromDateCondition_IsTrue_AsStartDateIsEqualTo()
        {
            string contractRef = "123";
            var startDate = new DateTime(2019, 01, 15);
            var stopNewStartsFromDate = new DateTime(2019, 01, 15);

            var mockContractAllocation = new Mock<IFcsContractAllocation>();
            mockContractAllocation.SetupGet(x => x.StopNewStartsFromDate).Returns(stopNewStartsFromDate);

            var mockFcsDataServices = new Mock<IFCSDataService>();
            mockFcsDataServices.Setup(x => x.GetContractAllocationFor(contractRef)).Returns(mockContractAllocation.Object);

            var rule18 = NewRule(fcsDataService: mockFcsDataServices.Object);

            rule18.StopNewStartsFromDateConditionMet(contractRef, startDate).Should().BeTrue();

            mockContractAllocation.VerifyGet(x => x.StopNewStartsFromDate, Times.AtLeastOnce);
            mockFcsDataServices.Verify(x => x.GetContractAllocationFor(contractRef), Times.Once);
        }

        [Fact]
        public void FromDateCondition_IsFalse_AsStartDateIsLess()
        {
            string contractRef = "123";
            var startDate = new DateTime(2019, 01, 15);
            var stopNewStartsFromDate = new DateTime(2019, 11, 11);

            var mockContractAllocation = new Mock<IFcsContractAllocation>();
            mockContractAllocation.SetupGet(x => x.StopNewStartsFromDate).Returns(stopNewStartsFromDate);

            var mockFcsDataServices = new Mock<IFCSDataService>();
            mockFcsDataServices.Setup(x => x.GetContractAllocationFor(contractRef)).Returns(mockContractAllocation.Object);

            var rule18 = NewRule(fcsDataService: mockFcsDataServices.Object);

            rule18.StopNewStartsFromDateConditionMet(contractRef, startDate).Should().BeFalse();

            mockContractAllocation.VerifyGet(x => x.StopNewStartsFromDate, Times.AtLeastOnce);
            mockFcsDataServices.Verify(x => x.GetContractAllocationFor(contractRef), Times.Once);
        }

        [Fact]
        public void FromDateCondition_Fails_WithNullContractAllocator()
        {
            string contractRef = "123";
            var startDate = DateTime.Now;
            IFcsContractAllocation obj = null;

            var mockFcsDataServices = new Mock<IFCSDataService>();
            mockFcsDataServices.Setup(x => x.GetContractAllocationFor(contractRef)).Returns(obj);

            var rule18 = NewRule(fcsDataService: mockFcsDataServices.Object).StopNewStartsFromDateConditionMet(contractRef, startDate);

            rule18.Should().BeFalse();
            mockFcsDataServices.Verify(x => x.GetContractAllocationFor(contractRef), Times.Once);
        }

        [Fact]
        public void ConditionMet_Pass()
        {
            int fundModel = TypeOfFunding.AdultSkills;
            var conRefNumber = "123";
            DateTime startDate = new DateTime(2018, 8, 1);

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.SOF,
                    LearnDelFAMCode = "105"
                }
            };

            var result = true;
            var rule18 = NewRuleMock();

            rule18.Setup(x => x.FundModelConditionMet(fundModel)).Returns(result);
            rule18.Setup(x => x.LearningDeliveryFAMsConditionMet(learningDeliveryFAMs)).Returns(result);
            rule18.Setup(x => x.FCTFundingConditionMet()).Returns(result);
            rule18.Setup(x => x.StopNewStartsFromDateConditionMet(conRefNumber, startDate)).Returns(result);

            rule18.Object.ConditionMet(fundModel, learningDeliveryFAMs, conRefNumber, startDate).Should().BeTrue();
        }

        [Theory]
        [InlineData(true, false, false, false)]
        [InlineData(false, false, false, false)]
        [InlineData(false, true, false, false)]
        [InlineData(false, false, true, false)]
        [InlineData(false, false, false, true)]
        public void ConditionMet_Fails(bool fundCondition, bool delFAMSCondition, bool fctCondition, bool fromDateCondition)
        {
            int fundModel = TypeOfFunding.AdultSkills;
            var conRefNumber = "123";
            DateTime startDate = new DateTime(2019, 6, 18);

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.SOF,
                    LearnDelFAMCode = LearningDeliveryFAMCodeConstants.SOF_ESFA_Adult
                }
            };

            var rule18 = NewRuleMock();
            rule18.Setup(x => x.FundModelConditionMet(fundModel)).Returns(fundCondition);
            rule18.Setup(x => x.LearningDeliveryFAMsConditionMet(learningDeliveryFAMs)).Returns(delFAMSCondition);
            rule18.Setup(x => x.FCTFundingConditionMet()).Returns(fctCondition);
            rule18.Setup(x => x.StopNewStartsFromDateConditionMet(conRefNumber, startDate)).Returns(fromDateCondition);

            rule18.Object.ConditionMet(fundModel, learningDeliveryFAMs, conRefNumber, startDate).Should().BeFalse();
        }

        [Fact]
        public void ValidatWithNull()
        {
            NewRule().Validate(null);
        }

        [Fact]
        public void Validate_WithError()
        {
            int ukprn = 12345678;
            string conRefNumber = "123";
            DateTime stopNewStartsFromDate = new DateTime(2018, 8, 1);
            DateTime startDate = stopNewStartsFromDate.AddYears(1);

            var learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = startDate,
                        FundModel = TypeOfFunding.AdultSkills,
                        ConRefNumber = conRefNumber,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = "SOF",
                                LearnDelFAMCode = "105"
                            }
                        }
                    }
                }
            };

            var mockContractAllocation = new Mock<IFcsContractAllocation>();
            mockContractAllocation.SetupGet(x => x.ContractAllocationNumber).Returns(conRefNumber);
            mockContractAllocation.SetupGet(x => x.StopNewStartsFromDate).Returns(stopNewStartsFromDate);

            var fileDataServiceMock = new Mock<IFileDataService>();
            fileDataServiceMock.Setup(ds => ds.UKPRN()).Returns(ukprn);

            var mockAcademicYearQueryService = new Mock<IAcademicYearQueryService>();

            var mockAcademicYearDataService = new Mock<IAcademicYearDataService>();

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQueryServiceMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(learner.LearningDeliveries.FirstOrDefault().LearningDeliveryFAMs, "SOF", "105")).Returns(true);

            var fcsDataServiceMock = new Mock<IFCSDataService>();
            fcsDataServiceMock.Setup(qs => qs.FundingRelationshipFCTExists(_fundingStreamPeriodCodes)).Returns(true);
            fcsDataServiceMock.Setup(m => m.GetContractAllocationFor(conRefNumber)).Returns(mockContractAllocation.Object);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    fileDataServiceMock.Object,
                    mockAcademicYearDataService.Object,
                    mockAcademicYearQueryService.Object,
                    fcsDataServiceMock.Object,
                    learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object,
                    validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            int ukprn = 12345678;
            string conRefNumber = "123";
            DateTime startDate = new DateTime(2018, 8, 1);  // testing condition StartDate is lower than stopNewStartsFromDate
            DateTime stopNewStartsFromDate = startDate.AddMonths(7);

            var learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = startDate,
                        FundModel = TypeOfFunding.AdultSkills,
                        ConRefNumber = conRefNumber,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = "SOF",
                                LearnDelFAMCode = "105"
                            }
                        }
                    }
                }
            };

            var mockContractAllocation = new Mock<IFcsContractAllocation>();
            mockContractAllocation.SetupGet(x => x.ContractAllocationNumber).Returns(conRefNumber);
            mockContractAllocation.SetupGet(x => x.StopNewStartsFromDate).Returns(stopNewStartsFromDate);

            var fileDataServiceMock = new Mock<IFileDataService>();
            fileDataServiceMock.Setup(ds => ds.UKPRN()).Returns(ukprn);

            var mockAcademicYearQueryService = new Mock<IAcademicYearQueryService>();

            var mockAcademicYearDataService = new Mock<IAcademicYearDataService>();

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQueryServiceMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(learner.LearningDeliveries.FirstOrDefault().LearningDeliveryFAMs, "SOF", "105")).Returns(true);

            var fcsDataServiceMock = new Mock<IFCSDataService>();
            fcsDataServiceMock.Setup(qs => qs.FundingRelationshipFCTExists(_fundingStreamPeriodCodes)).Returns(true);
            fcsDataServiceMock.Setup(m => m.GetContractAllocationFor(conRefNumber)).Returns(mockContractAllocation.Object);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    fileDataServiceMock.Object,
                    mockAcademicYearDataService.Object,
                    mockAcademicYearQueryService.Object,
                    fcsDataServiceMock.Object,
                    learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object,
                    validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        private UKPRN_18Rule NewRule(
             IFileDataService fileDataService = null,
            IAcademicYearDataService academicYearDataService = null,
            IAcademicYearQueryService academicYearQueryService = null,
            IFCSDataService fcsDataService = null,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new UKPRN_18Rule(fileDataService, academicYearDataService, academicYearQueryService, fcsDataService, learningDeliveryFAMQueryService, validationErrorHandler);
        }
    }
}
