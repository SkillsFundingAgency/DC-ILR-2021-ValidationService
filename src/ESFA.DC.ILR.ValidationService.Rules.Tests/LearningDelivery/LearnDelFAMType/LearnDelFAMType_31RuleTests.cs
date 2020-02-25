using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_31RuleTests : AbstractRuleTests<LearnDelFAMType_31Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMType_31");
        }

        [Fact]
        public void ValidationPasses()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.HEM
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM
                            }
                        }
                    },
                    new TestLearningDelivery
                    {
                        LearningDeliveryFAMs = new List<ILearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM
                            }
                        }
                    },
                    new TestLearningDelivery
                    {
                        LearningDeliveryFAMs = new List<ILearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM
                            }
                        }
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidationPasses_NoLDs()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearner = new TestLearner();

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidationPasses_NoFAMs()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery()
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidationFails()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError();

            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM
                            }
                        }
                    },
                    new TestLearningDelivery
                    {
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM
                            }
                        }
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            VerifyErrorHandlerMock(validationErrorHandlerMock, 1);
        }

        private LearnDelFAMType_31Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnDelFAMType_31Rule(validationErrorHandler);
        }
    }
}
