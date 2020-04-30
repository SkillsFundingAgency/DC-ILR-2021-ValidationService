using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.HE.LearningDeliveryHE;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.LearningDeliveryHE
{
    public class LearningDeliveryHE_06RuleTests : AbstractRuleTests<LearningDeliveryHE_06Rule>
    {
        private readonly string[] _notionalNVQLevels =
            {
                "4",
                "5",
                "6",
                "7",
                "8",
                "H"
            };

        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearningDeliveryHE_06");
        }

        [Theory]
        [InlineData(FundModels.AdultSkills)]
        [InlineData(FundModels.OtherAdult)]
        public void FundModelConditionMet_False(int fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeFalse();
        }

        [Theory]
        [InlineData(FundModels.EuropeanSocialFund)]
        [InlineData(FundModels.CommunityLearning)]
        public void FundModelConditionMet_True(int fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryHEConditionMet_True()
        {
            NewRule().LearningDeliveryHEConditionMet(new TestLearningDeliveryHE() { SSN = "TEST1234" }).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryHEConditionMet_False()
        {
            NewRule().LearningDeliveryHEConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void LARSNotionalNVQLevelV2ConditionMet_False()
        {
            string learnAimRef = "50022246";

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(l => l.NotionalNVQLevelV2MatchForLearnAimRefAndLevels(learnAimRef, _notionalNVQLevels)).Returns(false);

            NewRule(lARSDataService: larsDataServiceMock.Object).LARSNotionalNVQLevelV2Exclusion(learnAimRef).Should().BeFalse();
        }

        [Fact]
        public void LARSNotionalNVQLevelV2ConditionMet_True()
        {
            string learnAimRef = "50023408";

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(l => l.NotionalNVQLevelV2MatchForLearnAimRefAndLevels(learnAimRef, _notionalNVQLevels)).Returns(true);

            NewRule(lARSDataService: larsDataServiceMock.Object).LARSNotionalNVQLevelV2Exclusion(learnAimRef).Should().BeTrue();
        }

        [Theory]
        [InlineData(10)]
        [InlineData(70)]
        public void BuildErrorMessageParameters(int fundModel)
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(fundModel);

            validationErrorHandlerMock.Verify();
        }

        [Fact]
        public void Validate_Error()
        {
            var testLearner = new TestLearner
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = FundModels.EuropeanSocialFund,
                        LearnAimRef = "50023408",
                        LearningDeliveryHEEntity =
                            new TestLearningDeliveryHE()
                            {
                                DOMICILE = "DOMICILE"
                            }
                    }
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(l => l.NotionalNVQLevelV2MatchForLearnAimRefAndLevels("50023408", _notionalNVQLevels)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    lARSDataService: larsDataServiceMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError()
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
                                DOMICILE = "AD"
                            }
                    },
                    new TestLearningDelivery()
                    {
                        FundModel = FundModels.CommunityLearning,
                        LearnAimRef = "50023409",
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE()
                            {
                                DOMICILE = "AE"
                            }
                    },
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(l => l.NotionalNVQLevelV2MatchForLearnAimRefAndLevels("50023408", _notionalNVQLevels)).Returns(true);
            larsDataServiceMock.Setup(l => l.NotionalNVQLevelV2MatchForLearnAimRefAndLevels("50023409", _notionalNVQLevels)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    lARSDataService: larsDataServiceMock.Object).Validate(testLearner);
            }
        }

        private LearningDeliveryHE_06Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            ILARSDataService lARSDataService = null)
        {
            return new LearningDeliveryHE_06Rule(validationErrorHandler: validationErrorHandler, lARSDataService: lARSDataService);
        }
    }
}
