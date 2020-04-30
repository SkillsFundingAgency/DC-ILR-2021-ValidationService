using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.HE.SEC;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.SEC
{
    public class SEC_01RuleTests : AbstractRuleTests<SEC_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("SEC_01");
        }

        [Fact]
        public void ConditionMet_True()
        {
            int secValue = 11;

            var provideLookupDetailsMockup = new Mock<IProvideLookupDetails>();

            provideLookupDetailsMockup.Setup(p => p.Contains(TypeOfIntegerCodedLookup.SEC, secValue)).Returns(false);

            NewRule(provideLookupDetails: provideLookupDetailsMockup.Object).ConditionMet(secValue).Should().BeTrue();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        [InlineData(9)]
        public void ConditionMet_False(int secValue)
        {
            var provideLookupDetailsMockup = new Mock<IProvideLookupDetails>();

            provideLookupDetailsMockup.Setup(p => p.Contains(TypeOfIntegerCodedLookup.SEC, secValue)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, provideLookupDetails: provideLookupDetailsMockup.Object).ConditionMet(secValue).Should().BeFalse();
            }
        }

        [Theory]
        [InlineData(11)]
        [InlineData(12)]
        public void Validate_Error(int secValue)
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
                                SECNullable = secValue
                            }
                    },
                    new TestLearningDelivery()
                    {
                        FundModel = FundModels.CommunityLearning,
                        LearnAimRef = "50023409",
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                            {
                                SECNullable = secValue
                            }
                    },
                }
            };

            var provideLookupDetailsMockup = new Mock<IProvideLookupDetails>();

            provideLookupDetailsMockup.Setup(p => p.Contains(TypeOfIntegerCodedLookup.SEC, secValue)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, provideLookupDetails: provideLookupDetailsMockup.Object).Validate(testLearner);
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void Validate_NoError(int secValue)
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
                                SECNullable = secValue
                            }
                    },
                    new TestLearningDelivery()
                    {
                        FundModel = FundModels.CommunityLearning,
                        LearnAimRef = "50023409",
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                            {
                                SECNullable = secValue
                            }
                    },
                }
            };

            var provideLookupDetailsMockup = new Mock<IProvideLookupDetails>();

            provideLookupDetailsMockup.Setup(p => p.Contains(TypeOfIntegerCodedLookup.SEC, secValue)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, provideLookupDetails: provideLookupDetailsMockup.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            int secValue = 12;

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.SEC, secValue)).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(secValue);

            validationErrorHandlerMock.Verify();
        }

        private SEC_01Rule NewRule(IValidationErrorHandler validationErrorHandler = null, IProvideLookupDetails provideLookupDetails = null)
        {
            return new SEC_01Rule(provideLookupDetails: provideLookupDetails, validationErrorHandler: validationErrorHandler);
        }
    }
}
