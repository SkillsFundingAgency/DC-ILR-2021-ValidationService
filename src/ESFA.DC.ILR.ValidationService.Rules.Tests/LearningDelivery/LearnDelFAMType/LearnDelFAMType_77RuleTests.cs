using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Mocks;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_77RuleTests : AbstractRuleTests<LearnDelFAMType_77Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMType_77");
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
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.DAM
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.DAM
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.DAM
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.DAM
                            }
                        }
                    },
                    new TestLearningDelivery
                    {
                        LearningDeliveryFAMs = new List<ILearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.DAM
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.DAM
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.DAM
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.DAM
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
                            }
                        }
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            VerifyHandleNotCalled(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidationPasses_NoLDs()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearner = new TestLearner();

            NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            VerifyHandleNotCalled(validationErrorHandlerMock);
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
            VerifyHandleNotCalled(validationErrorHandlerMock);
        }

        [Fact]
        public void ValidationFails()
        {
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
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
                                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.DAM
                                },
                                new TestLearningDeliveryFAM
                                {
                                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.DAM
                                },
                                new TestLearningDeliveryFAM
                                {
                                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.DAM
                                },
                                new TestLearningDeliveryFAM
                                {
                                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.DAM
                                },
                                new TestLearningDeliveryFAM
                                {
                                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.DAM
                                }
                            }
                        }
                    }
                };

                NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        private LearnDelFAMType_77Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnDelFAMType_77Rule(validationErrorHandler);
        }

        private void VerifyHandleNotCalled(ValidationErrorHandlerMock errorHandlerMock)
        {
            errorHandlerMock.Verify(
                m => m.Handle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<IEnumerable<IErrorMessageParameter>>()),
                Times.Never);
        }
    }
}
