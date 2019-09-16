using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Learner.PriorAttain;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.PriorAttain
{
    public class PriorAttain_06RuleTests : AbstractRuleTests<PriorAttain_06Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("PriorAttain_06");
        }

        [Theory]
        [InlineData(70, true)]
        [InlineData(25, false)]
        [InlineData(0, false)]
        public void FundModelConditionMeetsExpectation(int fundModel, bool expectation)
        {
            NewRule().FundModelConditionMet(fundModel).Should().Be(expectation);
        }

        [Fact]
        public void PriorAttainConditionMet_False()
        {
            int priorAttain = 2;
            string conRefNumber = "ZESF00098";
            var fcsDataServiceMock = new Mock<IFCSDataService>();
            fcsDataServiceMock
                .Setup(m => m.GetContractAllocationFor(conRefNumber))
                .Returns(new FcsContractAllocation
                {
                    EsfEligibilityRule = new EsfEligibilityRule()
                    {
                        MinPriorAttainment = "1",
                        MaxPriorAttainment = "3"
                    }
                });

            NewRule(fcsDataService: fcsDataServiceMock.Object).PriorAttainConditionMet(priorAttain, conRefNumber).Should().BeFalse();
        }

        [Theory]
        [InlineData(2, "1", "3", false)]
        [InlineData(2, null, null, false)]
        [InlineData(2, "99", "3", false)]
        [InlineData(2, "3", null, true)]
        [InlineData(2, "99", null, false)]
        [InlineData(2, null, "99", true)]
        [InlineData(2, null, "3", false)]
        public void PriorAttain_VariousMinMaxLevels_ConditionMeetsExpectation(int learnerPriorAttain, string minPriorAttain, string maxPriorAttain, bool expectation)
        {
            string conRefNumber = "ZESF00098";
            var fcsDataServiceMock = new Mock<IFCSDataService>();
            fcsDataServiceMock
                .Setup(m => m.GetContractAllocationFor(conRefNumber))
                .Returns(new FcsContractAllocation
                {
                    EsfEligibilityRule = new EsfEligibilityRule()
                    {
                        MinPriorAttainment = minPriorAttain,
                        MaxPriorAttainment = maxPriorAttain
                    }
                });

            NewRule(fcsDataService: fcsDataServiceMock.Object).PriorAttainConditionMet(learnerPriorAttain, conRefNumber).Should().Be(expectation);
        }

        [Fact]
        public void PriorAttainConditionMet_True()
        {
            int priorAttain = 9;
            string conRefNumber = "ZESF00098";
            var fcsDataServiceMock = new Mock<IFCSDataService>();
            fcsDataServiceMock
                .Setup(m => m.GetContractAllocationFor(conRefNumber))
                .Returns(new FcsContractAllocation
                {
                    EsfEligibilityRule = new EsfEligibilityRule()
                    {
                        MinPriorAttainment = "1",
                        MaxPriorAttainment = "3"
                    }
                });

            NewRule(fcsDataService: fcsDataServiceMock.Object).PriorAttainConditionMet(priorAttain, conRefNumber).Should().BeTrue();
        }

        [Fact]
        public void PriorAttainConditionMet_MinMaxLevelsNull_ReturnsFalse()
        {
            int priorAttain = 2;
            string conRefNumber = "ZESF00098";
            var fcsDataServiceMock = new Mock<IFCSDataService>();
            fcsDataServiceMock
                .Setup(m => m.GetContractAllocationFor(conRefNumber))
                .Returns(new FcsContractAllocation
                {
                    EsfEligibilityRule = new EsfEligibilityRule()
                    {
                        MinPriorAttainment = null,
                        MaxPriorAttainment = null
                    }
                });

            NewRule(fcsDataService: fcsDataServiceMock.Object).PriorAttainConditionMet(priorAttain, conRefNumber).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False()
        {
            int fundModel = TypeOfFunding.EuropeanSocialFund;
            int priorAttain = 2;
            string conRefNumber = "ZESF00098";
            var fcsDataServiceMock = new Mock<IFCSDataService>();
            fcsDataServiceMock
                .Setup(m => m.GetContractAllocationFor(conRefNumber))
                .Returns(new FcsContractAllocation
                {
                    EsfEligibilityRule = new EsfEligibilityRule()
                    {
                        MinPriorAttainment = "1",
                        MaxPriorAttainment = "3"
                    }
                });
            NewRule(fcsDataService: fcsDataServiceMock.Object).ConditionMet(priorAttain, fundModel, conRefNumber).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            int fundModel = TypeOfFunding.EuropeanSocialFund;
            int priorAttain = 9;
            string conRefNumber = "ZESF00098";
            var fcsDataServiceMock = new Mock<IFCSDataService>();
            fcsDataServiceMock
                .Setup(m => m.GetContractAllocationFor(conRefNumber))
                .Returns(new FcsContractAllocation
                {
                    EsfEligibilityRule = new EsfEligibilityRule()
                    {
                        MinPriorAttainment = "1",
                        MaxPriorAttainment = "3"
                    }
                });
            NewRule(fcsDataService: fcsDataServiceMock.Object).ConditionMet(priorAttain, fundModel, conRefNumber).Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            var conRefNumber = "ZESF00098";
            var learner = new TestLearner()
            {
                PriorAttainNullable = 4,
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = TypeOfFunding.EuropeanSocialFund,
                        ConRefNumber = conRefNumber
                    }
                }
            };

            var fcsDataServiceMock = new Mock<IFCSDataService>();
            fcsDataServiceMock
                .Setup(m => m.GetContractAllocationFor(conRefNumber))
                .Returns(new FcsContractAllocation
                {
                    EsfEligibilityRule = new EsfEligibilityRule()
                    {
                        MinPriorAttainment = "1",
                        MaxPriorAttainment = "3"
                    }
                });

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object, fcsDataServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var conRefNumber = "ZESF00098";
            var learner = new TestLearner()
            {
                PriorAttainNullable = 3,
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = TypeOfFunding.EuropeanSocialFund,
                        ConRefNumber = conRefNumber
                    }
                }
            };

            var fcsDataServiceMock = new Mock<IFCSDataService>();
            fcsDataServiceMock
                .Setup(m => m.GetContractAllocationFor(conRefNumber))
                .Returns(new FcsContractAllocation
                {
                    EsfEligibilityRule = new EsfEligibilityRule()
                    {
                        MinPriorAttainment = "1",
                        MaxPriorAttainment = "H"
                    }
                });

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, fcsDataServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_OtherFundingModel_NoError()
        {
            var conRefNumber = "ZESF00098";
            var learner = new TestLearner()
            {
                PriorAttainNullable = 3,
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 0,
                        ConRefNumber = conRefNumber
                    }
                }
            };

            var fcsDataServiceMock = new Mock<IFCSDataService>();
            fcsDataServiceMock
                .Setup(m => m.GetContractAllocationFor(conRefNumber))
                .Returns(new FcsContractAllocation
                {
                    EsfEligibilityRule = new EsfEligibilityRule()
                    {
                        MinPriorAttainment = "1",
                        MaxPriorAttainment = "H"
                    }
                });

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, fcsDataServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NullLearnerPriorAttainValue_NoError()
        {
            var conRefNumber = "ZESF00098";
            var learner = new TestLearner()
            {
                PriorAttainNullable = 0,
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 0,
                        ConRefNumber = conRefNumber
                    }
                }
            };

            var fcsDataServiceMock = new Mock<IFCSDataService>();
            fcsDataServiceMock
                .Setup(m => m.GetContractAllocationFor(conRefNumber))
                .Returns(new FcsContractAllocation
                {
                    EsfEligibilityRule = new EsfEligibilityRule()
                    {
                        MinPriorAttainment = "1",
                        MaxPriorAttainment = "3"
                    }
                });

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, fcsDataServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_OtherFundModels_NoError()
        {
            var conRefNumber = "ZESF00098";
            var learner = new TestLearner()
            {
                PriorAttainNullable = 0,
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 25,
                        ConRefNumber = conRefNumber
                    }
                }
            };

            var fcsDataServiceMock = new Mock<IFCSDataService>();
            fcsDataServiceMock
                .Setup(m => m.GetContractAllocationFor(conRefNumber))
                .Returns(new FcsContractAllocation
                {
                    EsfEligibilityRule = new EsfEligibilityRule()
                    {
                        MinPriorAttainment = "1",
                        MaxPriorAttainment = "3"
                    }
                });
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, fcsDataServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError_NoLearningDeliveries()
        {
            var learner = new TestLearner()
            {
                PriorAttainNullable = 0,
                LearningDeliveries = null
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.PriorAttain, 1)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.FundModel, 1)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.ConRefNumber, "1")).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(1, 1, "1");

            validationErrorHandlerMock.Verify();
        }

        private PriorAttain_06Rule NewRule(IValidationErrorHandler validationErrorHandler = null, IFCSDataService fcsDataService = null)
        {
            return new PriorAttain_06Rule(validationErrorHandler, fcsDataService);
        }
    }
}
