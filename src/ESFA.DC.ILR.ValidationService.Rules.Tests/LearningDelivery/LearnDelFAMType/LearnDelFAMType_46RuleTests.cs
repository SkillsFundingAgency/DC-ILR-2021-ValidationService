using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_46RuleTests : AbstractRuleTests<LearnDelFAMType_46Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMType_46");
        }

        [Theory]
        [InlineData(FundModels.NotFundedByESFA, true)]
        [InlineData(FundModels.CommunityLearning, true)]
        [InlineData(FundModels.AdultSkills, false)]
        public void FundModelConditionMetMeetsExpectation(int fundModel, bool expectation)
        {
            NewRule().FundModelConditionMet(fundModel).Should().Be(expectation);
        }

        [Theory]
        [InlineData(LearningDeliveryFAMTypeConstants.ADL, false)]
        [InlineData(LearningDeliveryFAMTypeConstants.EEF, false)]
        [InlineData("ABC", false)]
        [InlineData(null, false)]
        [InlineData(LearningDeliveryFAMTypeConstants.FLN, true)]
        [InlineData("fln", true)]
        public void FAMTypeConditionMetMeetsExpectation(string learnDelFamType, bool expectation)
        {
            NewRule().FAMTypeConditionMet(learnDelFamType).Should().Be(expectation);
        }

        [Fact]
        public void ConditionMet_True()
        {
            var fundModel = FundModels.NotFundedByESFA;
            var learnDelFamType = LearningDeliveryFAMTypeConstants.FLN;

            NewRule().ConditionMet(fundModel, learnDelFamType).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_InvalidFundModel()
        {
            var fundModel = FundModels.AdultSkills;
            var learnDelFamType = LearningDeliveryFAMTypeConstants.FLN;

            NewRule().ConditionMet(fundModel, learnDelFamType).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_InvalidFAMType()
        {
            var fundModel = FundModels.NotFundedByESFA;
            var learnDelFamType = LearningDeliveryFAMTypeConstants.ADL;

            NewRule().ConditionMet(fundModel, learnDelFamType).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = FundModels.AdultSkills
                    },
                    new TestLearningDelivery()
                    {
                        FundModel = FundModels.ApprenticeshipsFrom1May2017,
                        LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.ADL
                            },
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.FLN
                            }
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = FundModels.ApprenticeshipsFrom1May2017
                    },
                    new TestLearningDelivery()
                    {
                        FundModel = FundModels.AdultSkills,
                        LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.ADL
                            },
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.FLN
                            }
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_NoLearningDeliveries()
        {
            var learner = new TestLearner()
            {
                LearnRefNumber = "123"
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_NoLearningDeliveryFams()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = FundModels.AdultSkills
                    }
                }
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
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.FLN)).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(LearningDeliveryFAMTypeConstants.FLN);

            validationErrorHandlerMock.Verify();
        }

        public LearnDelFAMType_46Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnDelFAMType_46Rule(validationErrorHandler);
        }
    }
}
