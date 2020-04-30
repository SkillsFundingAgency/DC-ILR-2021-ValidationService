using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.HE.FUNDLEV;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.FUNDLEV
{
    public class FUNDLEV_01RuleTests : AbstractRuleTests<FUNDLEV_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("FUNDLEV_01");
        }

        [Fact]
        public void ConditionMet_True()
        {
            int fundlevValue = 12;

            var provideLookupDetailsMockup = new Mock<IProvideLookupDetails>();

            provideLookupDetailsMockup.Setup(p => p.Contains(TypeOfIntegerCodedLookup.FundLev, fundlevValue)).Returns(false);

            NewRule(provideLookupDetails: provideLookupDetailsMockup.Object).ConditionMet(fundlevValue).Should().BeTrue();
        }

        [Theory]
        [InlineData(10)]
        [InlineData(11)]
        [InlineData(20)]
        [InlineData(21)]
        [InlineData(30)]
        [InlineData(31)]
        [InlineData(99)]
        public void ConditionMet_False(int fundlevValue)
        {
            var provideLookupDetailsMockup = new Mock<IProvideLookupDetails>();

            provideLookupDetailsMockup.Setup(p => p.Contains(TypeOfIntegerCodedLookup.FundLev, fundlevValue)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, provideLookupDetails: provideLookupDetailsMockup.Object).ConditionMet(fundlevValue).Should().BeFalse();
            }
        }

        [Theory]
        [InlineData(9)]
        [InlineData(12)]
        public void Validate_Error(int fundlevValue)
        {
            var testLearner = new TestLearner
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = FundModels.EuropeanSocialFund,
                        LearnAimRef = "50023408",
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                            {
                                FUNDLEV = fundlevValue
                            }
                    },
                    new TestLearningDelivery()
                    {
                        FundModel = FundModels.CommunityLearning,
                        LearnAimRef = "50023409",
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                            {
                                FUNDLEV = fundlevValue
                            }
                    },
                }
            };

            var provideLookupDetailsMockup = new Mock<IProvideLookupDetails>();

            provideLookupDetailsMockup.Setup(p => p.Contains(TypeOfIntegerCodedLookup.FundLev, fundlevValue)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, provideLookupDetails: provideLookupDetailsMockup.Object).Validate(testLearner);
            }
        }

        [Theory]
        [InlineData(10)]
        [InlineData(11)]
        [InlineData(20)]
        [InlineData(21)]
        [InlineData(30)]
        [InlineData(31)]
        [InlineData(99)]
        public void Validate_NoError(int fundlevValue)
        {
            var testLearner = new TestLearner
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = FundModels.EuropeanSocialFund,
                        LearnAimRef = "50023408",
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                            {
                                FUNDLEV = fundlevValue
                            }
                    },
                    new TestLearningDelivery()
                    {
                        FundModel = FundModels.CommunityLearning,
                        LearnAimRef = "50023409",
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                            {
                                FUNDLEV = fundlevValue
                            }
                    },
                }
            };

            var provideLookupDetailsMockup = new Mock<IProvideLookupDetails>();

            provideLookupDetailsMockup.Setup(p => p.Contains(TypeOfIntegerCodedLookup.FundLev, fundlevValue)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, provideLookupDetails: provideLookupDetailsMockup.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            int fundlevValue = 12;

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.FUNDLEV, fundlevValue)).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(fundlevValue);

            validationErrorHandlerMock.Verify();
        }

        private FUNDLEV_01Rule NewRule(IValidationErrorHandler validationErrorHandler = null, IProvideLookupDetails provideLookupDetails = null)
        {
            return new FUNDLEV_01Rule(provideLookupDetails: provideLookupDetails, validationErrorHandler: validationErrorHandler);
        }
    }
}
