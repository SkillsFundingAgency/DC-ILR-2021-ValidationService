using System;
using System.Collections.Generic;
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
    public class UKPRN_20RuleTests : AbstractRuleTests<UKPRN_20Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("UKPRN_20");
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
                        FundModel = FundModels.EuropeanSocialFund,
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
        public void ContractAllocationsForUkprnAndFundingStreamPeriodCodes_FcsContractAllocationsContainsNulls()
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
                        FundModel = FundModels.EuropeanSocialFund,
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
        public void ContractAllocationsForUkprnAndFundingStreamPeriodCodes_LearningDeliveryWithNullConRef()
        {
            // Arrange
            var fileDataServiceMock = new Mock<IFileDataService>();
            fileDataServiceMock.Setup(s => s.UKPRN()).Returns(42);

            var contractAllocationsForUkprn = new List<IFcsContractAllocation>
            {
                new FcsContractAllocation { DeliveryUKPRN = 42, FundingStreamPeriodCode = FundingStreamPeriodCodeConstants.ESF1420 },
            };

            var fcsDataServiceMock = new Mock<IFCSDataService>();
            fcsDataServiceMock.Setup(s => s.GetContractAllocationsFor(42)).Returns(contractAllocationsForUkprn);

            var learner = new TestLearner
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        ConRefNumber = null,
                        FundModel = FundModels.EuropeanSocialFund,
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
                new FcsContractAllocation { DeliveryUKPRN = 42, FundingStreamPeriodCode = FundingStreamPeriodCodeConstants.ESF1420 },
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
                new FcsContractAllocation { DeliveryUKPRN = 42, FundingStreamPeriodCode = FundingStreamPeriodCodeConstants.ESF1420 },
                new FcsContractAllocation { DeliveryUKPRN = 42, FundingStreamPeriodCode = "ZZZZZ" },
            };

            var fcsDataServiceMock = new Mock<IFCSDataService>();
            fcsDataServiceMock.Setup(s => s.GetContractAllocationsFor(42)).Returns(contractAllocationsForUkprn);

            var rule = new UKPRN_20Rule(fileDataServiceMock.Object, fcsDataServiceMock.Object, null);

            // Act
            var filtered = rule.ContractAllocationsForUkprnAndFundingStreamPeriodCodes(42);

            // Assert
            filtered.Should().NotBeNull();
            filtered.Should().NotBeEmpty();
            filtered.Should().HaveCount(1);
            filtered.Should().OnlyContain(ca => ca.FundingStreamPeriodCode == FundingStreamPeriodCodeConstants.ESF1420);
        }

        public static IEnumerable<object[]> ConditionMet_TestData()
        {
            yield return new object[] { "TestValue2", new DateTime(2018, 12, 31), false };
            yield return new object[] { "TestValue2", null, false };
            yield return new object[] { "TestValue1", null, false };
            yield return new object[] { "TestValue1", new DateTime(2019, 12, 31), false };
            yield return new object[] { "TestValue1", new DateTime(2019, 01, 01), true };
            yield return new object[] { "TestValue1", new DateTime(2018, 12, 31), true };
        }

        [Theory]
        [MemberData(nameof(ConditionMet_TestData))]
        public void ConditionMet(string contractAllocationNumber, DateTime? stopNewStartsFromDate, bool expectedResult)
        {
            // Arrange
            var conRef = "TestValue1";
            var learnStartDate = new DateTime(2019, 01, 01);
            var contractAllocations = new List<IFcsContractAllocation>
            {
                new FcsContractAllocation { ContractAllocationNumber = contractAllocationNumber, StopNewStartsFromDate = stopNewStartsFromDate }
            };

            // Act
            var result = NewRule().ConditionMet(conRef, learnStartDate, contractAllocations);

            // Assert
            result.Should().Equals(expectedResult);
        }

        public static IEnumerable<object[]> Validate_TestData()
        {
            yield return new object[] { "ConRef2", FundingStreamPeriodCodeConstants.ESF1420, new DateTime(2018, 12, 31), false };
            yield return new object[] { "ConRef1", FundingStreamPeriodCodeConstants.AEBTO_TOL1920, new DateTime(2018, 12, 31), false };
            yield return new object[] { "ConRef1", FundingStreamPeriodCodeConstants.ESF1420, new DateTime(2019, 12, 31), false };
            yield return new object[] { "ConRef1", FundingStreamPeriodCodeConstants.ESF1420, new DateTime(2019, 01, 02), false };
            yield return new object[] { "ConRef1", FundingStreamPeriodCodeConstants.ESF1420, new DateTime(2019, 01, 01), true };
            yield return new object[] { "ConRef1", FundingStreamPeriodCodeConstants.ESF1420, new DateTime(2018, 12, 31), true };
        }

        [Theory]
        [MemberData(nameof(Validate_TestData))]
        public void Validate(string contractAllocationNumber, string fundingStreamPeriodCode, DateTime? stopNewStartsFromDate, bool expectViolation)
        {
            // Arrange
            var fileDataServiceMock = new Mock<IFileDataService>();
            fileDataServiceMock.Setup(s => s.UKPRN()).Returns(42);

            var contractAllocationsForUkprn = new List<IFcsContractAllocation>
            {
                new FcsContractAllocation
                {
                    ContractAllocationNumber = contractAllocationNumber,
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
                        FundModel = FundModels.EuropeanSocialFund,
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

        [Fact]
        public void Validate_NoContractAllocations_NoViolation()
        {
            // Arrange
            var fileDataServiceMock = new Mock<IFileDataService>();
            fileDataServiceMock.Setup(s => s.UKPRN()).Returns(42);

            var contractAllocationsForUkprn = new List<IFcsContractAllocation> { };

            var fcsDataServiceMock = new Mock<IFCSDataService>();
            fcsDataServiceMock.Setup(s => s.GetContractAllocationsFor(42)).Returns(contractAllocationsForUkprn);

            var learner = new TestLearner
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = FundModels.EuropeanSocialFund,
                        LearnStartDate = new DateTime(2019, 01, 01)
                    }
                }
            };

            // Act
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    fileDataServiceMock.Object,
                    fcsDataServiceMock.Object,
                    validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        private UKPRN_20Rule NewRule(
            IFileDataService fileDataService = null,
            IFCSDataService fcsDataService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new UKPRN_20Rule(fileDataService, fcsDataService, validationErrorHandler);
        }
    }
}
