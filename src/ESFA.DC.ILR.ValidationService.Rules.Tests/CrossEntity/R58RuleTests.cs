using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R58RuleTests : AbstractRuleTests<R104Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R58");
        }

        [Fact]
        public void DoesNotHaveMultipleCoreAims_True()
        {
            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    AimType = 5
                }
            };

            NewRule().DoesNotHaveMultipleCoreAims(learningDeliveries).Should().BeTrue();
        }

        [Fact]
        public void DoesNotHaveMultipleCoreAims_True_Null()
        {
            NewRule().DoesNotHaveMultipleCoreAims(null).Should().BeTrue();
        }

        [Fact]
        public void DoesNotHaveMultipleCoreAims_False()
        {
            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    AimType = 5
                },
                new TestLearningDelivery
                {
                    AimType = 5
                }
            };

            NewRule().DoesNotHaveMultipleCoreAims(learningDeliveries).Should().BeFalse();
        }

        [Fact]
        public void LearnActEndDateForOverlappingCoreAims_ReturnsDate()
        {
            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    AimType = 5,
                    LearnStartDate = new DateTime(2018, 8, 1),
                    LearnActEndDateNullable = new DateTime(2018, 8, 10)
                },
                new TestLearningDelivery
                {
                    AimType = 5,
                    LearnStartDate = new DateTime(2018, 8, 10)
                }
            };

            NewRule().LearnActEndDateForOverlappingCoreAims(learningDeliveries).Should().Be((true, new DateTime(2018, 8, 10)));
        }

        [Fact]
        public void LearnActEndDateForOverlappingCoreAims_ReturnsDateMultiple()
        {
            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    AimType = 5,
                    LearnStartDate = new DateTime(2018, 8, 1),
                    LearnActEndDateNullable = new DateTime(2018, 8, 10)
                },
                new TestLearningDelivery
                {
                    AimType = 5,
                    LearnStartDate = new DateTime(2018, 8, 11),
                    LearnActEndDateNullable = new DateTime(2018, 8, 20)
                },
                new TestLearningDelivery
                {
                    AimType = 5,
                    LearnStartDate = new DateTime(2018, 8, 19)
                }
            };

            NewRule().LearnActEndDateForOverlappingCoreAims(learningDeliveries).Should().Be((true, new DateTime(2018, 8, 20)));
        }

        [Fact]
        public void LearnActEndDateForOverlappingCoreAims_ReturnsNull_NoDeliveries()
        {
            NewRule().LearnActEndDateForOverlappingCoreAims(null).Should().Be((false, null));
        }

        [Fact]
        public void LearnActEndDateForOverlappingCoreAims_ReturnsNull_NoOverlap()
        {
            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    AimType = 5,
                    LearnStartDate = new DateTime(2018, 8, 1),
                    LearnActEndDateNullable = new DateTime(2018, 8, 10)
                },
                new TestLearningDelivery
                {
                    AimType = 5,
                    LearnStartDate = new DateTime(2018, 8, 11),
                    LearnActEndDateNullable = new DateTime(2018, 8, 18)
                },
                new TestLearningDelivery
                {
                    AimType = 5,
                    LearnStartDate = new DateTime(2018, 8, 19)
                }
            };

            NewRule().LearnActEndDateForOverlappingCoreAims(learningDeliveries).Should().Be((false, null));
        }

        [Fact]
        public void LearnActEndDateForOverlappingCoreAims_ReturnsNull_NoEndDates()
        {
            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    AimType = 5,
                    LearnStartDate = new DateTime(2018, 8, 1)
                },
                new TestLearningDelivery
                {
                    AimType = 5,
                    LearnStartDate = new DateTime(2018, 8, 11)
                },
                new TestLearningDelivery
                {
                    AimType = 5,
                    LearnStartDate = new DateTime(2018, 8, 19)
                }
            };

            NewRule().LearnActEndDateForOverlappingCoreAims(learningDeliveries).Should().Be((true, null));
        }

        [Fact]
        public void LearnActEndDateForOverlappingCoreAims_ReturnsNull_MixedDates()
        {
            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    AimType = 5,
                    LearnStartDate = new DateTime(2018, 8, 1)
                },
                new TestLearningDelivery
                {
                    AimType = 5,
                    LearnStartDate = new DateTime(2018, 8, 11),
                    LearnActEndDateNullable = new DateTime(2018, 8, 12)
                },
                new TestLearningDelivery
                {
                    AimType = 5,
                    LearnStartDate = new DateTime(2018, 8, 19)
                }
            };

            NewRule().LearnActEndDateForOverlappingCoreAims(learningDeliveries).Should().Be((true, null));
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner()
            {
                LearnRefNumber = "00100309",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 1,
                        AimType = 5,
                        LearnStartDate = new DateTime(2018, 8, 1),
                        LearnActEndDateNullable = new DateTime(2018, 10, 1)
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 2,
                        AimType = 5,
                        LearnStartDate = new DateTime(2018, 9, 1)
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Error_MultipleDelivieries()
        {
            var learner = new TestLearner()
            {
                LearnRefNumber = "00100309",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 1,
                        AimType = 5,
                        LearnStartDate = new DateTime(2018, 8, 1),
                        LearnActEndDateNullable = new DateTime(2018, 10, 1)
                    },
                   new TestLearningDelivery()
                    {
                        AimSeqNumber = 2,
                        AimType = 5,
                        LearnStartDate = new DateTime(2018, 10, 2),
                        LearnActEndDateNullable = new DateTime(2018, 11, 1)
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 3,
                        AimType = 5,
                        LearnStartDate = new DateTime(2018, 8, 1)
                    },
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Error_NoActEndDate()
        {
            var learner = new TestLearner()
            {
                LearnRefNumber = "00100309",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 1,
                        AimType = 5,
                        LearnStartDate = new DateTime(2018, 8, 1),
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 2,
                        AimType = 5,
                        LearnStartDate = new DateTime(2018, 9, 2)
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_NoDeliveries()
        {
            var learner = new TestLearner();

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_NoCoreAims()
        {
            var learner = new TestLearner()
            {
                LearnRefNumber = "00100309",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 1,
                        AimType = 1,
                        LearnStartDate = new DateTime(2018, 8, 1),
                        LearnActEndDateNullable = new DateTime(2018, 10, 1)
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 2,
                        AimType = 2,
                        LearnStartDate = new DateTime(2018, 9, 1)
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_SingleCoreAims()
        {
            var learner = new TestLearner()
            {
                LearnRefNumber = "00100309",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 1,
                        AimType = 5,
                        LearnStartDate = new DateTime(2018, 9, 1)
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_NoOverlap()
        {
            var learner = new TestLearner()
            {
                LearnRefNumber = "00100309",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 1,
                        AimType = 5,
                        LearnStartDate = new DateTime(2018, 8, 1),
                        LearnActEndDateNullable = new DateTime(2018, 9, 1)
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 2,
                        AimType = 5,
                        LearnStartDate = new DateTime(2018, 9, 2)
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
            var aimType = 5;
            var learnActEndDate = new DateTime(2018, 8, 1);

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("AimType", 5)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnActEndDate", "01/08/2018")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(aimType, learnActEndDate);

            validationErrorHandlerMock.Verify();
        }

        public R58Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new R58Rule(validationErrorHandler);
        }
    }
}
