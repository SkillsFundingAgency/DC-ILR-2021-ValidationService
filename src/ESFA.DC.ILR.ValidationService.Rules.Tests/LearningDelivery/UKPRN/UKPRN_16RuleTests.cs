using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Model;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.UKPRN;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.UKPRN
{
    public class UKPRN_16RuleTests : AbstractRuleTests<UKPRN_16Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("UKPRN_16");
        }

        [Fact]
        public void ContractAllocationsForUkprnAndFundingStreamPeriodCodes_NullFcsContractAllocations()
        {
            // Arrange
            var fileDataServiceMock = new Mock<IFileDataService>();
            fileDataServiceMock.Setup(s => s.UKPRN()).Returns(42);

            List<IFcsContractAllocation> contractAllocationsForUkprn = null;

            var fcsDataServiceMock = new Mock<IFCSDataService>();
            fcsDataServiceMock.Setup(s => s.GetContractAllocationsFor(42)).Returns(contractAllocationsForUkprn);

            var learner = new TestLearner
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        ConRefNumber = "ConRef1",
                        FundModel = FundModels.ApprenticeshipsFrom1May2017,
                        LearnStartDate = new DateTime(2019, 01, 01)
                    }
                }
            };

            // Act & Assert
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    fileDataServiceMock.Object,
                    fcsDataServiceMock.Object,
                    validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ContractAllocationsForUkprnAndFundingStreamPeriodCodes_FcsContractAllocationsContainsNull()
        {
            // Arrange
            var fileDataServiceMock = new Mock<IFileDataService>();
            fileDataServiceMock.Setup(s => s.UKPRN()).Returns(42);

            var contractAllocationsForUkprn = new List<IFcsContractAllocation> { null };

            var fcsDataServiceMock = new Mock<IFCSDataService>();
            fcsDataServiceMock.Setup(s => s.GetContractAllocationsFor(42)).Returns(contractAllocationsForUkprn);

            var learner = new TestLearner
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        ConRefNumber = "ConRef1",
                        FundModel = FundModels.ApprenticeshipsFrom1May2017,
                        LearnStartDate = new DateTime(2019, 01, 01)
                    }
                }
            };

            // Act & Assert
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    fileDataServiceMock.Object,
                    fcsDataServiceMock.Object,
                    validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ContractAllocationsForUkprnAndFundingStreamPeriodCodes_NullLearningDeliveries()
        {
            // Arrange
            var fileDataServiceMock = new Mock<IFileDataService>();
            fileDataServiceMock.Setup(s => s.UKPRN()).Returns(42);

            var contractAllocationsForUkprn = new List<IFcsContractAllocation>
            {
                new FcsContractAllocation { DeliveryUKPRN = 42, FundingStreamPeriodCode = FundingStreamPeriodCodeConstants.C1618_NLAP2018 },
            };

            var fcsDataServiceMock = new Mock<IFCSDataService>();
            fcsDataServiceMock.Setup(s => s.GetContractAllocationsFor(42)).Returns(contractAllocationsForUkprn);

            var learner = new TestLearner
            {
                LearningDeliveries = null
            };

            // Act & Assert
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    fileDataServiceMock.Object,
                    fcsDataServiceMock.Object,
                    validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ContractAllocationsForUkprnAndFundingStreamPeriodCodes_FiltersOutNonMatchingPeriodCodes()
        {
            // Arrange
            var fileDataServiceMock = new Mock<IFileDataService>();
            fileDataServiceMock.Setup(s => s.UKPRN()).Returns(42);

            var contractAllocationsForUkprn = new List<IFcsContractAllocation>
            {
                new FcsContractAllocation { DeliveryUKPRN = 42, FundingStreamPeriodCode = "AAAAA" },
                new FcsContractAllocation { DeliveryUKPRN = 42, FundingStreamPeriodCode = FundingStreamPeriodCodeConstants.C1618_NLAP2018 },
                new FcsContractAllocation { DeliveryUKPRN = 42, FundingStreamPeriodCode = FundingStreamPeriodCodeConstants.ANLAP2018 },
                new FcsContractAllocation { DeliveryUKPRN = 42, FundingStreamPeriodCode = FundingStreamPeriodCodeConstants.LEVY1799 },
                new FcsContractAllocation { DeliveryUKPRN = 42, FundingStreamPeriodCode = FundingStreamPeriodCodeConstants.NONLEVY2019 },
                new FcsContractAllocation { DeliveryUKPRN = 42, FundingStreamPeriodCode = "ZZZZZ" },
            };

            var fcsDataServiceMock = new Mock<IFCSDataService>();
            fcsDataServiceMock.Setup(s => s.GetContractAllocationsFor(42)).Returns(contractAllocationsForUkprn);

            var rule = new UKPRN_16Rule(fileDataServiceMock.Object, fcsDataServiceMock.Object, null);

            // Act
            var filtered = rule.ContractAllocationsForUkprnAndFundingStreamPeriodCodes(42);

            // Assert
            filtered.Should().NotBeNull();
            filtered.Should().NotBeEmpty();
            filtered.Should().HaveCount(4);
            filtered.Should().OnlyContain(ca =>
                ca.FundingStreamPeriodCode == FundingStreamPeriodCodeConstants.C1618_NLAP2018 ||
                ca.FundingStreamPeriodCode == FundingStreamPeriodCodeConstants.ANLAP2018 ||
                ca.FundingStreamPeriodCode == FundingStreamPeriodCodeConstants.LEVY1799 ||
                ca.FundingStreamPeriodCode == FundingStreamPeriodCodeConstants.NONLEVY2019);
        }

        public static IEnumerable<object[]> ConditionMet_TestData()
        {
            yield return new object[] { null, false };
            yield return new object[] { new DateTime(2019, 12, 31), false };
            yield return new object[] { new DateTime(2019, 01, 01), true };
            yield return new object[] { new DateTime(2018, 12, 31), true };
        }

        [Theory]
        [MemberData(nameof(ConditionMet_TestData))]
        public void ConditionMet(DateTime? stopNewStartsFromDate, bool expectedResult)
        {
            // Arrange
            var learnStartDate = new DateTime(2019, 01, 01);
            var contractAllocations = new List<IFcsContractAllocation>
            {
                new FcsContractAllocation { StopNewStartsFromDate = stopNewStartsFromDate }
            };

            // Act
            var result = NewRule().ConditionMet(learnStartDate, contractAllocations);

            // Assert
            result.Should().Equals(expectedResult);
        }

        public static IEnumerable<object[]> MatchingLearningDeliveries_TestData()
        {
            yield return new object[] { FundModels.ApprenticeshipsFrom1May2017, new TestLearningDeliveryFAM { LearnDelFAMType = LearningDeliveryFAMTypeConstants.RES }, false };
            yield return new object[] { FundModels.ApprenticeshipsFrom1May2017, new TestLearningDeliveryFAM { LearnDelFAMType = LearningDeliveryFAMTypeConstants.ALB }, true };
            yield return new object[] { FundModels.ApprenticeshipsFrom1May2017, null, true };
            yield return new object[] { FundModels.AdultSkills, new TestLearningDeliveryFAM { LearnDelFAMType = LearningDeliveryFAMTypeConstants.RES }, false };
        }

        [Theory]
        [MemberData(nameof(MatchingLearningDeliveries_TestData))]
        public void MatchingLearningDeliveries_FiltersOnFundModelAndFAMType(int fundModel, ILearningDeliveryFAM learningDeliveryFAM, bool returned)
        {
            var learningDeliveryFAMs =
                learningDeliveryFAM != null ? new ILearningDeliveryFAM[] { learningDeliveryFAM } : null;

            var learningDeliveries = new TestLearningDelivery[]
            {
                // Arrange
                new TestLearningDelivery()
                {
                    ConRefNumber = "ConRef1",
                    FundModel = fundModel,
                    LearningDeliveryFAMs = learningDeliveryFAMs
                }
            };

            // Act
            var filteredResult = NewRule().MatchingLearningDeliveries(learningDeliveries);

            // Assert
            if (returned)
            {
                filteredResult.Should().NotBeEmpty();
            }
            else
            {
                filteredResult.Should().BeEmpty();
            }
        }

        public static IEnumerable<object[]> Validate_TestData()
        {
            yield return new object[] { FundingStreamPeriodCodeConstants.C1618_NLAP2018, null, false };
            yield return new object[] { FundingStreamPeriodCodeConstants.AEBTO_TOL1920, new DateTime(2018, 12, 31), false };
            yield return new object[] { FundingStreamPeriodCodeConstants.C1618_NLAP2018, new DateTime(2019, 12, 31), false };
            yield return new object[] { FundingStreamPeriodCodeConstants.C1618_NLAP2018, new DateTime(2019, 01, 02), false };
            yield return new object[] { FundingStreamPeriodCodeConstants.C1618_NLAP2018, new DateTime(2019, 01, 01), true };
            yield return new object[] { FundingStreamPeriodCodeConstants.C1618_NLAP2018, new DateTime(2018, 12, 31), true };
        }

        [Theory]
        [MemberData(nameof(Validate_TestData))]
        public void Validate(string fundingStreamPeriodCode, DateTime? stopNewStartsFromDate, bool expectViolation)
        {
            // Arrange
            var fileDataServiceMock = new Mock<IFileDataService>();
            fileDataServiceMock.Setup(s => s.UKPRN()).Returns(42);

            var contractAllocationsForUkprn = new List<IFcsContractAllocation>
            {
                new FcsContractAllocation
                {
                    DeliveryUKPRN = 42,
                    FundingStreamPeriodCode = fundingStreamPeriodCode,
                    StopNewStartsFromDate = stopNewStartsFromDate
                }
            };

            var fcsDataServiceMock = new Mock<IFCSDataService>();
            fcsDataServiceMock.Setup(s => s.GetContractAllocationsFor(42)).Returns(contractAllocationsForUkprn);

            var learner = new TestLearner
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        ConRefNumber = "ConRef1",
                        FundModel = FundModels.ApprenticeshipsFrom1May2017,
                        LearnStartDate = new DateTime(2019, 01, 01)
                    }
                }
            };

            // Act & Assert
            using (var validationErrorHandlerMock = expectViolation ? BuildValidationErrorHandlerMockForError() : BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    fileDataServiceMock.Object,
                    fcsDataServiceMock.Object,
                    validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        private UKPRN_16Rule NewRule(
            IFileDataService fileDataService = null,
            IFCSDataService fcsDataService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new UKPRN_16Rule(fileDataService, fcsDataService, validationErrorHandler);
        }
    }
}
