using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_14RuleTests : AbstractRuleTests<LearnDelFAMType_14Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMType_14");
        }

        [Theory]
        [InlineData(FundModels.AdultSkills)]
        [InlineData(FundModels.ApprenticeshipsFrom1May2017)]
        [InlineData(FundModels.OtherAdult)]
        public void FundModelConditionMet_False(int fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeFalse();
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            NewRule().FundModelConditionMet(FundModels.Other16To19).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryFAMsConditionMet_False()
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock.Setup(lds => lds.HasLearningDeliveryFAMType(testLearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ADL)).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).LearningDeliveryFAMsConditionMet(testLearningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMsConditionMet_True()
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.EEF
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock.Setup(lds => lds.HasLearningDeliveryFAMType(testLearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.EEF)).Returns(true);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).LearningDeliveryFAMsConditionMet(testLearningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void DD07ConditionMet_False()
        {
            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(ProgTypes.ApprenticeshipStandard)).Returns(true);

            NewRule(dd07: dd07Mock.Object).DD07ConditionMet(ProgTypes.ApprenticeshipStandard).Should().BeFalse();
        }

        [Fact]
        public void DD07ConditionMet_True()
        {
            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(null)).Returns(false);

            NewRule(dd07: dd07Mock.Object).DD07ConditionMet(null).Should().BeTrue();
        }

        [Theory]
        [InlineData(FundModels.AdultSkills, false, true)]
        [InlineData(FundModels.ApprenticeshipsFrom1May2017, true, false)]
        [InlineData(FundModels.OtherAdult, false, false)]
        [InlineData(FundModels.NotFundedByESFA, false, false)]
        public void ConditionMet_False(int fundModel, bool dd07Result, bool famsResult)
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var dd07Mock = new Mock<IDerivedData_07Rule>();

            learningDeliveryFAMsQueryServiceMock.Setup(lds => lds.HasLearningDeliveryFAMType(testLearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ADL)).Returns(famsResult);
            dd07Mock.Setup(dd => dd.IsApprenticeship(ProgTypes.ApprenticeshipStandard)).Returns(dd07Result);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object, dd07: dd07Mock.Object).ConditionMet(fundModel, ProgTypes.ApprenticeshipStandard, testLearningDeliveryFAMs).Should().BeFalse();
        }

        [Theory]
        [InlineData(FundModels.AdultSkills, true, true)]
        [InlineData(FundModels.NotFundedByESFA, true, true)]
        [InlineData(FundModels.NotFundedByESFA, false, true)]
        public void ConditionMet_True(int fundModel, bool dd07Result, bool famsResult)
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.EEF
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var dd07Mock = new Mock<IDerivedData_07Rule>();

            learningDeliveryFAMsQueryServiceMock.Setup(lds => lds.HasLearningDeliveryFAMType(testLearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.EEF)).Returns(famsResult);
            dd07Mock.Setup(dd => dd.IsApprenticeship(null)).Returns(dd07Result);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object, dd07: dd07Mock.Object).ConditionMet(fundModel, ProgTypes.ApprenticeshipStandard, testLearningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.EEF
                }
            };

            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = FundModels.NotFundedByESFA,
                        ProgTypeNullable = null,
                        LearningDeliveryFAMs = testLearningDeliveryFAMs
                    }
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var dd07Mock = new Mock<IDerivedData_07Rule>();

            learningDeliveryFAMsQueryServiceMock.Setup(lds => lds.HasLearningDeliveryFAMType(testLearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.EEF)).Returns(true);
            dd07Mock.Setup(dd => dd.IsApprenticeship(null)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object, dd07: dd07Mock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT
                }
            };

            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = FundModels.ApprenticeshipsFrom1May2017,
                        ProgTypeNullable = ProgTypes.AdvancedLevelApprenticeship,
                        LearningDeliveryFAMs = testLearningDeliveryFAMs
                    }
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var dd07Mock = new Mock<IDerivedData_07Rule>();

            learningDeliveryFAMsQueryServiceMock.Setup(lds => lds.HasLearningDeliveryFAMType(testLearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ACT)).Returns(false);
            dd07Mock.Setup(dd => dd.IsApprenticeship(ProgTypes.AdvancedLevelApprenticeship)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object, dd07: dd07Mock.Object).Validate(testLearner);
            }
        }

        [Theory]
        [InlineData(FundModels.ApprenticeshipsFrom1May2017)]
        [InlineData(FundModels.AdultSkills)]
        [InlineData(FundModels.OtherAdult)]
        public void BuildErrorMessageParameters(int fundModel)
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.EEF)).Verifiable();
            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(fundModel, LearningDeliveryFAMTypeConstants.EEF);

            validationErrorHandlerMock.Verify();
        }

        public LearnDelFAMType_14Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null,
            IDerivedData_07Rule dd07 = null)
        {
            return new LearnDelFAMType_14Rule(validationErrorHandler: validationErrorHandler, learningDeliveryFAMQueryService: learningDeliveryFAMQueryService, dd07: dd07);
        }
    }
}
