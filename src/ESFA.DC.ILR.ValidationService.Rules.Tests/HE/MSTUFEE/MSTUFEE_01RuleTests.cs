using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.HE.MSTUFEE;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.MSTUFEE
{
    public class MSTUFEE_01RuleTests : AbstractRuleTests<MSTUFEE_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("MSTUFEE_01");
        }

        [Fact]
        public void ConditionMet_True()
        {
            int mstufeeValue = 10;

            var provideLookupDetailsMockup = new Mock<IProvideLookupDetails>();

            provideLookupDetailsMockup.Setup(p => p.Contains(TypeOfIntegerCodedLookup.MSTuFee, mstufeeValue)).Returns(false);

            NewRule(provideLookupDetails: provideLookupDetailsMockup.Object).ConditionMet(mstufeeValue).Should().BeTrue();
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
        [InlineData(11)]
        [InlineData(12)]
        [InlineData(13)]
        [InlineData(14)]
        [InlineData(15)]
        [InlineData(17)]
        [InlineData(18)]
        [InlineData(19)]
        public void ConditionMet_False(int mstufeeValue)
        {
            var provideLookupDetailsMockup = new Mock<IProvideLookupDetails>();

            provideLookupDetailsMockup.Setup(p => p.Contains(TypeOfIntegerCodedLookup.MSTuFee, mstufeeValue)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, provideLookupDetails: provideLookupDetailsMockup.Object).ConditionMet(mstufeeValue).Should().BeFalse();
            }
        }

        [Theory]
        [InlineData(10)]
        [InlineData(20)]
        public void Validate_Error(int mstufeeValue)
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
                                MSTUFEE = mstufeeValue
                            }
                    },
                    new TestLearningDelivery()
                    {
                        FundModel = FundModels.CommunityLearning,
                        LearnAimRef = "50023409",
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                            {
                                MSTUFEE = mstufeeValue
                            }
                    },
                }
            };

            var provideLookupDetailsMockup = new Mock<IProvideLookupDetails>();

            provideLookupDetailsMockup.Setup(p => p.Contains(TypeOfIntegerCodedLookup.MSTuFee, mstufeeValue)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, provideLookupDetails: provideLookupDetailsMockup.Object).Validate(testLearner);
            }
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
        [InlineData(11)]
        [InlineData(12)]
        [InlineData(13)]
        [InlineData(14)]
        [InlineData(15)]
        [InlineData(17)]
        [InlineData(18)]
        [InlineData(19)]
        public void Validate_NoError(int mstufeeValue)
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
                                MSTUFEE = mstufeeValue
                            }
                    },
                    new TestLearningDelivery()
                    {
                        FundModel = FundModels.CommunityLearning,
                        LearnAimRef = "50023409",
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                            {
                                MSTUFEE = mstufeeValue
                            }
                    },
                }
            };

            var provideLookupDetailsMockup = new Mock<IProvideLookupDetails>();

            provideLookupDetailsMockup.Setup(p => p.Contains(TypeOfIntegerCodedLookup.MSTuFee, mstufeeValue)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, provideLookupDetails: provideLookupDetailsMockup.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            int mstufeeValue = 10;

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.MSTUFEE, mstufeeValue)).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(mstufeeValue);

            validationErrorHandlerMock.Verify();
        }

        private MSTUFEE_01Rule NewRule(IValidationErrorHandler validationErrorHandler = null, IProvideLookupDetails provideLookupDetails = null)
        {
            return new MSTUFEE_01Rule(provideLookupDetails: provideLookupDetails, validationErrorHandler: validationErrorHandler);
        }
    }
}
