using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.HE.SPECFEE;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.SPECFEE
{
    public class SPECFEE_01RuleTests : AbstractRuleTests<SPECFEE_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("SPECFEE_01");
        }

        [Fact]
        public void ConditionMet_True()
        {
            int specfeeValue = 11;

            var provideLookupDetailsMockup = new Mock<IProvideLookupDetails>();

            provideLookupDetailsMockup.Setup(p => p.Contains(TypeOfIntegerCodedLookup.SpecFee, specfeeValue)).Returns(false);

            NewRule(provideLookupDetails: provideLookupDetailsMockup.Object).ConditionMet(specfeeValue).Should().BeTrue();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(0)]
        [InlineData(9)]
        public void ConditionMet_False(int specfeeValue)
        {
            var provideLookupDetailsMockup = new Mock<IProvideLookupDetails>();

            provideLookupDetailsMockup.Setup(p => p.Contains(TypeOfIntegerCodedLookup.SpecFee, specfeeValue)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, provideLookupDetails: provideLookupDetailsMockup.Object).ConditionMet(specfeeValue).Should().BeFalse();
            }
        }

        [Theory]
        [InlineData(11)]
        [InlineData(12)]
        public void Validate_Error(int specfeeValue)
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
                                SPECFEE = specfeeValue
                            }
                    },
                    new TestLearningDelivery()
                    {
                        FundModel = FundModels.CommunityLearning,
                        LearnAimRef = "50023409",
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                            {
                                SPECFEE = specfeeValue
                            }
                    },
                }
            };

            var provideLookupDetailsMockup = new Mock<IProvideLookupDetails>();

            provideLookupDetailsMockup.Setup(p => p.Contains(TypeOfIntegerCodedLookup.SpecFee, specfeeValue)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, provideLookupDetails: provideLookupDetailsMockup.Object).Validate(testLearner);
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void Validate_NoError(int specfeeValue)
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
                                SPECFEE = specfeeValue
                            }
                    },
                    new TestLearningDelivery()
                    {
                        FundModel = FundModels.CommunityLearning,
                        LearnAimRef = "50023409",
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                            {
                                SPECFEE = specfeeValue
                            }
                    },
                }
            };

            var provideLookupDetailsMockup = new Mock<IProvideLookupDetails>();

            provideLookupDetailsMockup.Setup(p => p.Contains(TypeOfIntegerCodedLookup.SpecFee, specfeeValue)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, provideLookupDetails: provideLookupDetailsMockup.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            int specfeeValue = 12;

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.SPECFEE, specfeeValue)).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(specfeeValue);

            validationErrorHandlerMock.Verify();
        }

        private SPECFEE_01Rule NewRule(IValidationErrorHandler validationErrorHandler = null, IProvideLookupDetails provideLookupDetails = null)
        {
            return new SPECFEE_01Rule(provideLookupDetails: provideLookupDetails, validationErrorHandler: validationErrorHandler);
        }
    }
}
