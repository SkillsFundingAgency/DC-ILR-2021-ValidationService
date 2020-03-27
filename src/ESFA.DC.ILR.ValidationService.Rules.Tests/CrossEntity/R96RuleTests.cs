using System;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R96RuleTests : AbstractRuleTests<R96Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R96");
        }

        [Fact]
        public void Validate_Error()
        {
            var testLearner = new TestLearner()
            {
                LearnRefNumber = "123456789",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 654,
                        LearningDeliveryWorkPlacements = new TestLearningDeliveryWorkPlacement[]
                        {
                            new TestLearningDeliveryWorkPlacement()
                            {
                                WorkPlaceStartDate = new DateTime(2017, 07, 01)
                            },
                            new TestLearningDeliveryWorkPlacement()
                            {
                                WorkPlaceStartDate = new DateTime(2015, 05, 02)
                            },
                            new TestLearningDeliveryWorkPlacement()
                            {
                                WorkPlaceStartDate = new DateTime(2017, 07, 01)
                            }
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_Error_InMultipleDeliveries()
        {
            var testLearner = new TestLearner()
            {
                LearnRefNumber = "123456789",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 654,
                        LearningDeliveryWorkPlacements = new TestLearningDeliveryWorkPlacement[]
                        {
                            new TestLearningDeliveryWorkPlacement()
                            {
                                WorkPlaceStartDate = new DateTime(2017, 07, 01)
                            },
                            new TestLearningDeliveryWorkPlacement()
                            {
                                WorkPlaceStartDate = new DateTime(2015, 05, 02)
                            }
                        }
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 655,
                        LearningDeliveryWorkPlacements = new TestLearningDeliveryWorkPlacement[]
                        {
                            new TestLearningDeliveryWorkPlacement()
                            {
                                WorkPlaceStartDate = new DateTime(2017, 07, 01)
                            }
                        }
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 656,
                        LearningDeliveryWorkPlacements = new TestLearningDeliveryWorkPlacement[]
                        {
                            new TestLearningDeliveryWorkPlacement()
                            {
                                WorkPlaceStartDate = new DateTime(2015, 05, 02)
                            }
                        }
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 657,
                        LearningDeliveryWorkPlacements = new TestLearningDeliveryWorkPlacement[]
                        {
                            new TestLearningDeliveryWorkPlacement()
                            {
                                WorkPlaceStartDate = new DateTime(2016, 08, 14)
                            }
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var testLearner = new TestLearner()
            {
                LearnRefNumber = "123456789",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 654,
                        LearningDeliveryWorkPlacements = new TestLearningDeliveryWorkPlacement[]
                        {
                            new TestLearningDeliveryWorkPlacement()
                            {
                                WorkPlaceStartDate = new DateTime(2017, 07, 01)
                            },
                            new TestLearningDeliveryWorkPlacement()
                            {
                                WorkPlaceStartDate = new DateTime(2015, 05, 02)
                            }
                        }
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 655,
                        LearningDeliveryWorkPlacements = new TestLearningDeliveryWorkPlacement[]
                        {
                            new TestLearningDeliveryWorkPlacement()
                            {
                                WorkPlaceStartDate = new DateTime(2016, 09, 19)
                            }
                        }
                    }
                }
            };

            using (var validationErrorHandleMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandleMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError_LearnerNullCheck()
        {
            TestLearner testLearner = null;

            using (var validationErrorHandleMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandleMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError_LearningDeliveryNullCheck()
        {
            TestLearner testLearner = new TestLearner()
            {
                LearningDeliveries = null
            };

            using (var validationErrorHandleMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandleMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError_LearningDeliveryWorkPlacementNullCheck()
        {
            TestLearner testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearningDeliveryWorkPlacements = null
                    }
                }
            };

            using (var validationErrorHandleMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandleMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.WorkPlaceStartDate, "01/07/2017"));

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2017, 07, 01));
        }

        private R96Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new R96Rule(validationErrorHandler: validationErrorHandler);
        }
    }
}
