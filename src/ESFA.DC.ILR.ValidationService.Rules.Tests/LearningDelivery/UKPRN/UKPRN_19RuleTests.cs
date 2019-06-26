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
    public class UKPRN_19RuleTests : AbstractRuleTests<UKPRN_19Rule>
    {
        private readonly HashSet<string> _fundingStreamPeriodCodes = new HashSet<string>
        {
            FundingStreamPeriodCodeConstants.AEB_19TRN1920,
            FundingStreamPeriodCodeConstants.AEB_AS1920
        };

        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("UKPRN_19");
        }

        [Theory]
        [InlineData(35, true)]
        [InlineData(25, false)]
        public void FundModelConditionMet(int fundModel, bool expected)
        {
            NewRule().FundModelConditionMet(fundModel).Should().Be(expected);
        }

        [Fact]
        public void LearningDeliveryFAMsCondition_ShouldMatch()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>();
            var mockQueryService = new Mock<ILearningDeliveryFAMQueryService>();
            mockQueryService.Setup(x => x.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LDM", "357")).Returns(true);

            NewRule(learningDeliveryFAMQueryService: mockQueryService.Object).LearningDeliveryFAMsConditionMet(learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryFAMsCondition_ShouldFail()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>();
            var mockQueryService = new Mock<ILearningDeliveryFAMQueryService>();
            mockQueryService.Setup(x => x.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "LD", "30")).Returns(false);

            NewRule(learningDeliveryFAMQueryService: mockQueryService.Object).LearningDeliveryFAMsConditionMet(learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void FCTFundingCondition_IsTrue()
        {
            var mockFCSDataService = new Mock<IFCSDataService>();
            mockFCSDataService.Setup(x => x.FundingRelationshipFCTExists(_fundingStreamPeriodCodes)).Returns(true);

            var rule19 = NewRule(fcsDataService: mockFCSDataService.Object);

            rule19.FCTFundingConditionMet().Should().BeTrue();
            mockFCSDataService.Verify(x => x.FundingRelationshipFCTExists(_fundingStreamPeriodCodes), Times.AtLeastOnce);
        }

        [Fact]
        public void FCTFundingCondition_IsFalse()
        {
            var mockFCSDataService = new Mock<IFCSDataService>();
            mockFCSDataService.Setup(x => x.FundingRelationshipFCTExists(_fundingStreamPeriodCodes)).Returns(false);

            var rule19 = NewRule(fcsDataService: mockFCSDataService.Object);

            rule19.FCTFundingConditionMet().Should().BeFalse();
            mockFCSDataService.Verify(x => x.FundingRelationshipFCTExists(_fundingStreamPeriodCodes), Times.AtLeastOnce);
        }

        [Fact]
        public void FromDateCondition_False_AsStartDateIsLess()
        {
            string conRefNumber = "123";
            DateTime startDate = new DateTime(2018, 8, 15);
            DateTime fromDate = new DateTime(2019, 8, 15);

            var mockContAllocation = new Mock<IFcsContractAllocation>();
            mockContAllocation.SetupGet(x => x.StopNewStartsFromDate).Returns(fromDate);

            var mockFCSDataService = new Mock<IFCSDataService>();
            mockFCSDataService.Setup(x => x.GetContractAllocationFor(conRefNumber)).Returns(mockContAllocation.Object);

            var rule19 = NewRule(fcsDataService: mockFCSDataService.Object);

            rule19.StopNewStartsFromDateConditionMet(conRefNumber, startDate).Should().BeFalse();
            mockContAllocation.VerifyGet(x => x.StopNewStartsFromDate, Times.AtLeastOnce);
        }

        [Fact]
        public void FromDateCondition_IsTrue_AsStartDateIsGreater()
        {
            string conRefNumber = "123";
            DateTime startDate = new DateTime(2019, 8, 15);
            DateTime fromDate = new DateTime(2018, 8, 15);

            var mockContAllocation = new Mock<IFcsContractAllocation>();
            mockContAllocation.SetupGet(x => x.StopNewStartsFromDate).Returns(fromDate);

            var mockFCSDataService = new Mock<IFCSDataService>();
            mockFCSDataService.Setup(x => x.GetContractAllocationFor(conRefNumber)).Returns(mockContAllocation.Object);

            var rule19 = NewRule(fcsDataService: mockFCSDataService.Object);

            rule19.StopNewStartsFromDateConditionMet(conRefNumber, startDate).Should().BeTrue();
            mockContAllocation.VerifyGet(x => x.StopNewStartsFromDate, Times.AtLeastOnce);
        }

        [Fact]
        public void FromDateCondition_IsTrue_AsBothDatesAreSame()
        {
            string conRefNumber = "123";
            DateTime startDate = new DateTime(2019, 6, 15);
            DateTime fromDate = new DateTime(2019, 6, 15);

            var mockContAllocation = new Mock<IFcsContractAllocation>();
            mockContAllocation.SetupGet(x => x.StopNewStartsFromDate).Returns(fromDate);

            var mockFCSDataService = new Mock<IFCSDataService>();
            mockFCSDataService.Setup(x => x.GetContractAllocationFor(conRefNumber)).Returns(mockContAllocation.Object);

            var rule19 = NewRule(fcsDataService: mockFCSDataService.Object);

            rule19.StopNewStartsFromDateConditionMet(conRefNumber, startDate).Should().BeTrue();
            mockContAllocation.VerifyGet(x => x.StopNewStartsFromDate, Times.AtLeastOnce);
        }

        [Fact]
        public void FromDateCondition_IsFalse_AsFromDateNull()
        {
            string conRefNumber = "123";
            DateTime startDate = new DateTime(2019, 8, 15);
            DateTime? fromDate = null;

            var mockContAllocation = new Mock<IFcsContractAllocation>();
            mockContAllocation.SetupGet(x => x.StopNewStartsFromDate).Returns(fromDate);

            var mockFCSDataService = new Mock<IFCSDataService>();
            mockFCSDataService.Setup(x => x.GetContractAllocationFor(conRefNumber)).Returns(mockContAllocation.Object);

            var rule19 = NewRule(fcsDataService: mockFCSDataService.Object);

            rule19.StopNewStartsFromDateConditionMet(conRefNumber, startDate).Should().BeFalse();
            mockContAllocation.VerifyGet(x => x.StopNewStartsFromDate, Times.AtLeastOnce);
        }

        [Fact]
        public void ConditionMet_IsTrue()
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

            var expected = true;
            var rule19 = NewRuleMock();

            rule19.Setup(x => x.FundModelConditionMet(fundModel)).Returns(expected);
            rule19.Setup(x => x.LearningDeliveryFAMsConditionMet(learningDeliveryFAMs)).Returns(expected);
            rule19.Setup(x => x.FCTFundingConditionMet()).Returns(expected);
            rule19.Setup(x => x.StopNewStartsFromDateConditionMet(conRefNumber, startDate)).Returns(expected);

            var result = rule19.Object.ConditionMet(fundModel, learningDeliveryFAMs, conRefNumber, startDate);

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(true, false, false, false)]
        [InlineData(false, false, false, false)]
        [InlineData(false, true, false, false)]
        [InlineData(false, false, true, false)]
        [InlineData(false, false, false, true)]
        public void ConditionMet_IsFalse(bool fundCondition, bool delFAMSCondition, bool fctCondition, bool fromDateCondition)
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

            var rule19 = NewRuleMock();

            rule19.Setup(x => x.FundModelConditionMet(fundModel)).Returns(fundCondition);
            rule19.Setup(x => x.LearningDeliveryFAMsConditionMet(learningDeliveryFAMs)).Returns(delFAMSCondition);
            rule19.Setup(x => x.FCTFundingConditionMet()).Returns(fctCondition);
            rule19.Setup(x => x.StopNewStartsFromDateConditionMet(conRefNumber, startDate)).Returns(fromDateCondition);

            var result = rule19.Object.ConditionMet(fundModel, learningDeliveryFAMs, conRefNumber, startDate);

            result.Should().BeFalse();
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
            DateTime startDate = new DateTime(2019, 8, 1);

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
                                LearnDelFAMType = "LDM",
                                LearnDelFAMCode = "357"
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
            learningDeliveryFAMQueryServiceMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(learner.LearningDeliveries.FirstOrDefault().LearningDeliveryFAMs, "LDM", "357")).Returns(true);

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
                                LearnDelFAMType = "LDM",
                                LearnDelFAMCode = "357"
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
            learningDeliveryFAMQueryServiceMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(learner.LearningDeliveries.FirstOrDefault().LearningDeliveryFAMs, "LDM", "357")).Returns(true);

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

        private UKPRN_19Rule NewRule(
            IFileDataService fileDataService = null,
           IAcademicYearDataService academicYearDataService = null,
           IAcademicYearQueryService academicYearQueryService = null,
           IFCSDataService fcsDataService = null,
           ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null,
           IValidationErrorHandler validationErrorHandler = null)
        {
            return new UKPRN_19Rule(fileDataService, academicYearDataService, academicYearQueryService, fcsDataService, learningDeliveryFAMQueryService, validationErrorHandler);
        }
    }
}
