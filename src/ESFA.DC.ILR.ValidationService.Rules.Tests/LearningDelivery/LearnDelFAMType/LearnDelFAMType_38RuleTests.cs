using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_38RuleTests : AbstractRuleTests<LearnDelFAMType_38Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMType_38");
        }

        [Fact]
        public void ValidatePasses_IrrelevantFamType()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = TypeOfFunding.AdultSkills,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LSF
                            }
                        }
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidatePasses_AllowedFundingModel()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = TypeOfFunding.NotFundedByESFA,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.ALB
                            }
                        }
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidatePasses_ExceptionApplies()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = TypeOfFunding.AdultSkills,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.ALB
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                                LearnDelFAMCode = LearningDeliveryFAMCodeConstants.LDM_CareerLearningPilot
                            }
                        }
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidatePasses_NoLDs()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearner = new TestLearner();

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidatePasses_NoFAMs()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = TypeOfFunding.NotFundedByESFA
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidateFails_ALB_ButNoExceptionOnSameDelivery()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError();

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = TypeOfFunding.AdultSkills,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.ALB
                            }
                        }
                    },
                    new TestLearningDelivery
                    {
                        FundModel = TypeOfFunding.AdultSkills,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                                LearnDelFAMCode = LearningDeliveryFAMCodeConstants.LDM_CareerLearningPilot
                            }
                        }
                    },
                    new TestLearningDelivery
                    {
                        FundModel = TypeOfFunding.NotFundedByESFA,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LSF
                            }
                        }
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock, 1);
        }

        private LearnDelFAMType_38Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnDelFAMType_38Rule(validationErrorHandler);
        }
    }
}
