using System;
using System.Collections.Generic;
using System.Globalization;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_70RuleTests : AbstractRuleTests<LearnDelFAMType_70Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMType_70");
        }

        [Fact]
        public void LearningDeliveryFAMCodeConditionMet_True()
        {
            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock
                .Setup(s => s.HasLearningDeliveryFAMCodeForType(
                    It.IsAny<List<ILearningDeliveryFAM>>(),
                    LearningDeliveryFAMTypeConstants.LDM,
                    LearningDeliveryFAMCodeConstants.LDM_Pilot)).Returns(true);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).LearningDeliveryFAMCodeConditionMet(It.IsAny<List<ILearningDeliveryFAM>>()).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryFAMCodeConditionMet_False()
        {
            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock
                .Setup(s => s.HasLearningDeliveryFAMCodeForType(
                    It.IsAny<List<ILearningDeliveryFAM>>(),
                    LearningDeliveryFAMTypeConstants.LDM,
                    LearningDeliveryFAMCodeConstants.LDM_Pilot)).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).LearningDeliveryFAMCodeConditionMet(It.IsAny<List<ILearningDeliveryFAM>>()).Should().BeFalse();
        }

        [Theory]
        [InlineData("2018-01-01", false)]
        [InlineData("2018-07-31", false)]
        [InlineData("2017-08-01", true)]
        [InlineData("2014-09-14", true)]
        public void LearnStartDateConditionMetMeetsExpectation(string learnStarDate, bool expectation)
        {
            NewRule().LearnStartDateConditionMet(DateTime.Parse(learnStarDate)).Should().Be(expectation);
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learnStartDate = new DateTime(2017, 12, 12);
            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMCode = LearningDeliveryFAMCodeConstants.LDM_Pilot,
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM
                }
            };
            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock
                .Setup(s => s.HasLearningDeliveryFAMCodeForType(
                    It.IsAny<List<ILearningDeliveryFAM>>(),
                    LearningDeliveryFAMTypeConstants.LDM,
                    LearningDeliveryFAMCodeConstants.LDM_Pilot)).Returns(true);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).ConditionMet(learnStartDate, learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_InvalidLearnStartDate()
        {
            var learnStartDate = new DateTime(2018, 12, 12);
            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMCode = LearningDeliveryFAMCodeConstants.LDM_Pilot,
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM
                }
            };
            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock
                .Setup(s => s.HasLearningDeliveryFAMCodeForType(
                    It.IsAny<List<ILearningDeliveryFAM>>(),
                    LearningDeliveryFAMTypeConstants.LDM,
                    LearningDeliveryFAMCodeConstants.LDM_Pilot)).Returns(true);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).ConditionMet(learnStartDate, learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_InvalidLearnDelFamTypes()
        {
            var learnStartDate = new DateTime(2017, 12, 12);
            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMCode = LearningDeliveryFAMCodeConstants.LDM_Pilot,
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ALB
                }
            };
            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock
                .Setup(s => s.HasLearningDeliveryFAMCodeForType(
                    It.IsAny<List<ILearningDeliveryFAM>>(),
                    LearningDeliveryFAMTypeConstants.ALB,
                    LearningDeliveryFAMCodeConstants.LDM_Pilot)).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).ConditionMet(learnStartDate, learningDeliveryFAMs).Should().BeFalse();
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
                        FundModel = FundModels.NotFundedByESFA,
                        LearnStartDate = new DateTime(2017, 01, 01),
                        LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMCode = LearningDeliveryFAMCodeConstants.LDM_Pilot,
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.ALB
                            },
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMCode = LearningDeliveryFAMCodeConstants.LDM_Pilot,
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM
                            },
                        }
                    },
                    new TestLearningDelivery()
                    {
                        FundModel = FundModels.AdultSkills,
                        LearnStartDate = new DateTime(2018, 01, 01),
                        LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMCode = LearningDeliveryFAMCodeConstants.RES_Code,
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.ALB
                            },
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMCode = LearningDeliveryFAMCodeConstants.ACT_ContractESFA,
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM
                            },
                        }
                    }
                }
            };

           var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

           learningDeliveryFAMsQueryServiceMock
                .Setup(s => s.HasLearningDeliveryFAMCodeForType(
                    It.IsAny<List<ILearningDeliveryFAM>>(),
                    LearningDeliveryFAMTypeConstants.LDM,
                    LearningDeliveryFAMCodeConstants.LDM_Pilot)).Returns(true);

           using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object, learningDeliveryFAMsQueryServiceMock.Object).Validate(learner);
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
                        FundModel = FundModels.NotFundedByESFA,
                        LearnStartDate = new DateTime(2018, 07, 01),
                        LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMCode = LearningDeliveryFAMCodeConstants.LDM_Pilot,
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.ALB
                            },
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMCode = LearningDeliveryFAMCodeConstants.LDM_Pilot,
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM
                            },
                        }
                    },
                    new TestLearningDelivery()
                    {
                        FundModel = FundModels.AdultSkills,
                        LearnStartDate = new DateTime(2018, 01, 01),
                        LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMCode = LearningDeliveryFAMCodeConstants.RES_Code,
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.ALB
                            },
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMCode = LearningDeliveryFAMCodeConstants.ACT_ContractESFA,
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM
                            },
                        }
                    }
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock
                .Setup(s => s.HasLearningDeliveryFAMCodeForType(
                    It.IsAny<List<ILearningDeliveryFAM>>(),
                    LearningDeliveryFAMTypeConstants.LDM,
                    LearningDeliveryFAMCodeConstants.LDM_Pilot)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, learningDeliveryFAMsQueryServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            var learnDelFamType70Rule = NewRule(validationErrorHandler: validationErrorHandlerMock.Object);

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnDelFamType70Rule.LastViableLearnStartDate.ToString("d", new CultureInfo("en-GB")))).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.LDM)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnFAMCode, LearningDeliveryFAMCodeConstants.LDM_Pilot));

            learnDelFamType70Rule.BuildErrorMessageParameters(learnDelFamType70Rule.LastViableLearnStartDate, LearningDeliveryFAMTypeConstants.LDM, LearningDeliveryFAMCodeConstants.LDM_Pilot);
            validationErrorHandlerMock.Verify();
        }

        public LearnDelFAMType_70Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null)
        {
            return new LearnDelFAMType_70Rule(validationErrorHandler: validationErrorHandler, learningDeliveryFAMQueryService: learningDeliveryFAMQueryService);
        }
    }
}
