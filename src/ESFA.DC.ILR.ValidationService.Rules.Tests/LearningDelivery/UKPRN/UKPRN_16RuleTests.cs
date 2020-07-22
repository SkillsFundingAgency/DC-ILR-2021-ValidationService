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
            filtered.Should().HaveCount(2);
            filtered.Should().OnlyContain(ca =>
                ca.FundingStreamPeriodCode == FundingStreamPeriodCodeConstants.C1618_NLAP2018 ||
                ca.FundingStreamPeriodCode == FundingStreamPeriodCodeConstants.ANLAP2018);
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

        [Fact]
        public void MatchingLearningDeliveries()
        {
            var deliveryFams = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                    LearnDelFAMCode = "2"
                }
            };

            var learningDeliveries = new TestLearningDelivery[]
            {
                // Arrange
                new TestLearningDelivery()
                {
                    ConRefNumber = "ConRef1",
                    FundModel = 36,
                    AimType = 1,
                    LearningDeliveryFAMs = deliveryFams
                }
            };

            // Act
            var filteredResult = NewRule().MatchingLearningDeliveries(learningDeliveries);

            filteredResult.Should().NotBeEmpty();
        }

        [Fact]
        public void MatchingLearningDeliveries_NoneHasRestart()
        {
            var deliveryFams = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.RES
                },
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                    LearnDelFAMCode = "2"
                }
            };

            var learningDeliveries = new TestLearningDelivery[]
            {
                // Arrange
                new TestLearningDelivery()
                {
                    ConRefNumber = "ConRef1",
                    FundModel = 36,
                    AimType = 1,
                    LearningDeliveryFAMs = deliveryFams
                }
            };

            // Act
            var filteredResult = NewRule().MatchingLearningDeliveries(learningDeliveries);

            filteredResult.Should().BeEmpty();
        }

        [Fact]
        public void MatchingLearningDeliveries_NoneAimType()
        {
            var deliveryFams = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                    LearnDelFAMCode = "2"
                }
            };

            var learningDeliveries = new TestLearningDelivery[]
            {
                // Arrange
                new TestLearningDelivery()
                {
                    ConRefNumber = "ConRef1",
                    FundModel = 36,
                    AimType = 2,
                    LearningDeliveryFAMs = deliveryFams
                }
            };

            // Act
            var filteredResult = NewRule().MatchingLearningDeliveries(learningDeliveries);

            filteredResult.Should().BeEmpty();
        }

        [Fact]
        public void MatchingLearningDeliveries_NoneNoValidDeliveryFam()
        {
            var deliveryFams = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                    LearnDelFAMCode = "3"
                }
            };

            var learningDeliveries = new TestLearningDelivery[]
            {
                // Arrange
                new TestLearningDelivery()
                {
                    ConRefNumber = "ConRef1",
                    FundModel = 36,
                    AimType = 1,
                    LearningDeliveryFAMs = deliveryFams
                }
            };

            // Act
            var filteredResult = NewRule().MatchingLearningDeliveries(learningDeliveries);

            filteredResult.Should().BeEmpty();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learnStartDate = new DateTime(2019, 08, 01);

            var contractAllocations = new List<IFcsContractAllocation>
            {
                new FcsContractAllocation
                {
                    DeliveryUKPRN = 42,
                    FundingStreamPeriodCode = FundingStreamPeriodCodeConstants.C1618_NLAP2018,
                    StopNewStartsFromDate = new DateTime(2018, 12, 01),
                    StartDate = new DateTime(2019, 01, 01)
                },
                new FcsContractAllocation
                {
                    DeliveryUKPRN = 42,
                    FundingStreamPeriodCode = FundingStreamPeriodCodeConstants.C1618_NLAP2018,
                    StopNewStartsFromDate = new DateTime(2018, 12, 03),
                    StartDate = new DateTime(2019, 03, 01)
                },
                new FcsContractAllocation
                {
                    DeliveryUKPRN = 42,
                    FundingStreamPeriodCode = FundingStreamPeriodCodeConstants.C1618_NLAP2018,
                    StopNewStartsFromDate = new DateTime(2018, 12, 02),
                    StartDate = new DateTime(2019, 02, 01)
                }
            };

            NewRule().ConditionMet(learnStartDate, contractAllocations).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_NullStopStarts()
        {
            var learnStartDate = new DateTime(2019, 08, 01);

            var contractAllocations = new List<IFcsContractAllocation>
            {
                new FcsContractAllocation
                {
                    DeliveryUKPRN = 42,
                    FundingStreamPeriodCode = FundingStreamPeriodCodeConstants.C1618_NLAP2018,
                    StopNewStartsFromDate = null,
                    StartDate = new DateTime(2019, 01, 01)
                },
                new FcsContractAllocation
                {
                    DeliveryUKPRN = 42,
                    FundingStreamPeriodCode = FundingStreamPeriodCodeConstants.C1618_NLAP2018,
                    StopNewStartsFromDate = null,
                    StartDate = new DateTime(2019, 03, 01)
                },
                new FcsContractAllocation
                {
                    DeliveryUKPRN = 42,
                    FundingStreamPeriodCode = FundingStreamPeriodCodeConstants.C1618_NLAP2018,
                    StopNewStartsFromDate = null,
                    StartDate = new DateTime(2019, 02, 01)
                }
            };

            NewRule().ConditionMet(learnStartDate, contractAllocations).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_LearnStartDateLessThan()
        {
            var learnStartDate = new DateTime(2018, 08, 01);

            var contractAllocations = new List<IFcsContractAllocation>
            {
                new FcsContractAllocation
                {
                    DeliveryUKPRN = 42,
                    FundingStreamPeriodCode = FundingStreamPeriodCodeConstants.C1618_NLAP2018,
                    StopNewStartsFromDate = new DateTime(2018, 12, 01),
                    StartDate = new DateTime(2019, 01, 01)
                },
                new FcsContractAllocation
                {
                    DeliveryUKPRN = 42,
                    FundingStreamPeriodCode = FundingStreamPeriodCodeConstants.C1618_NLAP2018,
                    StopNewStartsFromDate = new DateTime(2018, 12, 03),
                    StartDate = new DateTime(2019, 03, 01)
                },
                new FcsContractAllocation
                {
                    DeliveryUKPRN = 42,
                    FundingStreamPeriodCode = FundingStreamPeriodCodeConstants.C1618_NLAP2018,
                    StopNewStartsFromDate = new DateTime(2018, 12, 02),
                    StartDate = new DateTime(2019, 02, 01)
                }
            };

            NewRule().ConditionMet(learnStartDate, contractAllocations).Should().BeFalse();
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
                    StopNewStartsFromDate = stopNewStartsFromDate,
                    StartDate = new DateTime(2019, 08, 01)
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
                        AimType = 1,
                        LearnStartDate = new DateTime(2019, 01, 01),
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                                LearnDelFAMCode = "2"
                            }
                        }
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
