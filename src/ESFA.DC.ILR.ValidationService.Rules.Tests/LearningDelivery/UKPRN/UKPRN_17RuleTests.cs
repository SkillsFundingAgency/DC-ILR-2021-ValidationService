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
    public class UKPRN_17RuleTests : AbstractRuleTests<UKPRN_17Rule>
    {
        private readonly IEnumerable<string> _fundingStreamPeriodCodes = new HashSet<string>
        {
             FundingStreamPeriodCodeConstants.C16_18TRN1920
        };

        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("UKPRN_17");
        }

        [Fact]
        public void FundModelCondition_ShouldMatch()
        {
            var fundModel = 25;
            NewRule().FundModelConditionMet(fundModel).Should().BeTrue();
        }

        [Fact]
        public void FundModelCondition_ShouldNotMatch()
        {
            var fundModel = 21;
            NewRule().FundModelConditionMet(fundModel).Should().BeFalse();
        }

        [Theory]
        [InlineData(1, false)]
        [InlineData(24, true)]
        public void IsTraineeshipConditionMet(int? progType, bool expected)
        {
            NewRule().TraineeshipConditionMet(progType).Should().Be(expected);
        }

        [Fact]
        public void LearningDeliveryFAMsConditionMet()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>();
            var mockLearningDeliveryFAMQueryService = new Mock<ILearningDeliveryFAMQueryService>();

            var result = true;

            mockLearningDeliveryFAMQueryService
                .Setup(x => x.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "SOF", "105"))
                .Returns(result);

            var rule17 = NewRule(learningDeliveryFAMQueryService: mockLearningDeliveryFAMQueryService.Object);

            rule17.LearningDeliveryFAMsConditionMet(learningDeliveryFAMs).Should().Be(result);
        }

        [Fact]
        public void FCTFundingCondition_ShouldPass()
        {
            var mockfcsDataServices = new Mock<IFCSDataService>();
            mockfcsDataServices
                .Setup(x => x.FundingRelationshipFCTExists(_fundingStreamPeriodCodes))
                .Returns(true);

            var rule17 = NewRule(fcsDataService: mockfcsDataServices.Object).FCTFundingConditionMet();

            rule17.Should().BeTrue();
            mockfcsDataServices.Verify(fcs => fcs.FundingRelationshipFCTExists(_fundingStreamPeriodCodes), Times.Once);
        }

        [Fact]
        public void FCTFundingCondition_ShouldFail()
        {
            var mockFcsDataServices = new Mock<IFCSDataService>();
            mockFcsDataServices
                .Setup(x => x.FundingRelationshipFCTExists(_fundingStreamPeriodCodes))
                .Returns(false);

            var rule17 = NewRule(fcsDataService: mockFcsDataServices.Object).FCTFundingConditionMet();

            rule17.Should().BeFalse();
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

            var rule17 = NewRule(fcsDataService: mockFcsDataServices.Object).StopNewStartsFromDateConditionMet(contractRef, startDate);

            rule17.Should().BeFalse();

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

            var rule17 = NewRule(fcsDataService: mockFcsDataServices.Object);

            rule17.StopNewStartsFromDateConditionMet(contractRef, startDate).Should().BeTrue();

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

            var rule17 = NewRule(fcsDataService: mockFcsDataServices.Object);

            rule17.StopNewStartsFromDateConditionMet(contractRef, startDate).Should().BeTrue();

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

            var rule17 = NewRule(fcsDataService: mockFcsDataServices.Object);

            rule17.StopNewStartsFromDateConditionMet(contractRef, startDate).Should().BeFalse();

            mockContractAllocation.VerifyGet(x => x.StopNewStartsFromDate, Times.AtLeastOnce);
            mockFcsDataServices.Verify(x => x.GetContractAllocationFor(contractRef), Times.Once);
        }

        [Fact]
        public void ConditionMet_Pass()
        {
            int fundModel = TypeOfFunding.Age16To19ExcludingApprenticeships;
            var progType = 24;
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

            var rule17 = NewRuleMock();
            rule17.Setup(x => x.FundModelConditionMet(fundModel)).Returns(true);
            rule17.Setup(x => x.TraineeshipConditionMet(progType)).Returns(true);
            rule17.Setup(x => x.LearningDeliveryFAMsConditionMet(learningDeliveryFAMs)).Returns(true);
            rule17.Setup(x => x.FCTFundingConditionMet()).Returns(true);
            rule17.Setup(x => x.StopNewStartsFromDateConditionMet(conRefNumber, startDate)).Returns(true);

            rule17.Object.ConditionMet(fundModel, progType, learningDeliveryFAMs, conRefNumber, startDate).Should().BeTrue();
        }

        [Theory]
        [InlineData(true, false, false, false, false)]
        [InlineData(false, true, false, false, false)]
        [InlineData(false, false, true, false, false)]
        [InlineData(false, false, false, true, false)]
        [InlineData(false, false, false, false, true)]
        public void ConditionMet_Fails(bool fundCondition, bool isTraineeCondition, bool delFAMSCondition, bool fctCondition, bool fromDateCondition)
        {
            int fundModel = TypeOfFunding.Age16To19ExcludingApprenticeships;
            var progType = 24;
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

            var rule17 = NewRuleMock();
            rule17.Setup(x => x.FundModelConditionMet(fundModel)).Returns(fundCondition);
            rule17.Setup(x => x.TraineeshipConditionMet(progType)).Returns(isTraineeCondition);
            rule17.Setup(x => x.LearningDeliveryFAMsConditionMet(learningDeliveryFAMs)).Returns(delFAMSCondition);
            rule17.Setup(x => x.FCTFundingConditionMet()).Returns(fctCondition);
            rule17.Setup(x => x.StopNewStartsFromDateConditionMet(conRefNumber, startDate)).Returns(fromDateCondition);

            rule17.Object
                .ConditionMet(fundModel, progType, learningDeliveryFAMs, conRefNumber, startDate).Should().BeFalse();
        }

        [Fact]
        public void ValidatWithNull()
        {
            NewRule().Validate(null);
        }

        [Fact]
        public void Validate_Error()
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
                        ProgTypeNullable = 24,
                        FundModel = 25,
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
            DateTime startDate = new DateTime(2018, 8, 1);
            DateTime stopNewStartsFromDate = startDate.AddMonths(7);

            var learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = startDate,
                        ProgTypeNullable = 24,
                        FundModel = 25,
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

        private UKPRN_17Rule NewRule(
             IFileDataService fileDataService = null,
            IAcademicYearDataService academicYearDataService = null,
            IAcademicYearQueryService academicYearQueryService = null,
            IFCSDataService fcsDataService = null,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new UKPRN_17Rule(fileDataService, academicYearDataService, academicYearQueryService, fcsDataService, learningDeliveryFAMQueryService, validationErrorHandler);
        }
    }
}
