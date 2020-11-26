using System;
using System.Collections.Generic;
using System.Linq;
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
    public class R104RuleTests : AbstractRuleTests<R104Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R104");
        }

        [Fact]
        public void HasValidFamsToCheck_FalseNullFAMs()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>();

            NewRule().HasValidFamsToCheck(learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void HasValidFamsToCheck_FalseNullDateFrom()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMDateFromNullable = null,
                    LearnDelFAMDateToNullable = null
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "XXX",
                    LearnDelFAMDateFromNullable = null,
                    LearnDelFAMDateToNullable = new DateTime(2018, 8, 31)
                }
            };

            NewRule().HasValidFamsToCheck(learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void HasValidFamsToCheck_FalseLessThanTwo()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                    LearnDelFAMDateToNullable = new DateTime(2018, 8, 31)
                }
            };

            NewRule().HasValidFamsToCheck(learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void HasValidFamsToCheck_True()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                    LearnDelFAMDateToNullable = new DateTime(2018, 8, 31)
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                    LearnDelFAMDateToNullable = new DateTime(2018, 8, 31)
                }
            };

            NewRule().HasValidFamsToCheck(learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void GetNonConsecutiveLearningDeliveryFAMsForType_NullDateFrom()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                    LearnDelFAMDateToNullable = new DateTime(2018, 8, 31)
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMDateFromNullable = null,
                    LearnDelFAMDateToNullable = new DateTime(2018, 9, 30)
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMDateFromNullable = new DateTime(2018, 10, 1),
                    LearnDelFAMDateToNullable = new DateTime(2018, 10, 31)
                }
            };

            var result = NewRule().GetNonConsecutiveLearningDeliveryFAMs(learningDeliveryFAMs);

            result.Count().Should().Be(0);
        }

        [Fact]
        public void GetNonConsecutiveLearningDeliveryFAMsForType_None()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                    LearnDelFAMDateToNullable = new DateTime(2018, 8, 31)
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMDateFromNullable = new DateTime(2018, 9, 1),
                    LearnDelFAMDateToNullable = new DateTime(2018, 9, 30)
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMDateFromNullable = new DateTime(2018, 10, 1),
                    LearnDelFAMDateToNullable = new DateTime(2018, 10, 31)
                }
            };

            var result = NewRule().GetNonConsecutiveLearningDeliveryFAMs(learningDeliveryFAMs);

            result.Count().Should().Be(0);
        }

        [Fact]
        public void GetNonConsecutiveLearningDeliveryFAMsForType_NullDates()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                    LearnDelFAMDateToNullable = null
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMDateFromNullable = new DateTime(2018, 9, 1),
                    LearnDelFAMDateToNullable = null
                }
            };

            var result = NewRule().GetNonConsecutiveLearningDeliveryFAMs(learningDeliveryFAMs);

            result.Count().Should().Be(1);
        }

        [Fact]
        public void GetNonConsecutiveLearningDeliveryFAMsForType_Multiple()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                    LearnDelFAMDateToNullable = new DateTime(2018, 8, 31)
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMDateFromNullable = new DateTime(2018, 9, 1),
                    LearnDelFAMDateToNullable = new DateTime(2018, 9, 30)
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMDateFromNullable = new DateTime(2018, 10, 3),
                    LearnDelFAMDateToNullable = new DateTime(2018, 10, 31)
                },
            };

            var result = NewRule().GetNonConsecutiveLearningDeliveryFAMs(learningDeliveryFAMs);

            result.Count().Should().Be(1);
        }

        [Fact]
        public void GetNonConsecutiveLearningDeliveryFAMsForType_OverLappingDates()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                    LearnDelFAMDateToNullable = new DateTime(2018, 8, 31)
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMDateFromNullable = new DateTime(2018, 8, 30),
                    LearnDelFAMDateToNullable = null
                }
            };

            var result = NewRule().GetNonConsecutiveLearningDeliveryFAMs(learningDeliveryFAMs);

            result.Count().Should().Be(1);
        }

        [Fact]
        public void GetNonConsecutiveLearningDeliveryFAMsForType_NonConsecutiveDates()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                    LearnDelFAMDateToNullable = new DateTime(2018, 8, 31)
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMDateFromNullable = new DateTime(2018, 9, 8),
                    LearnDelFAMDateToNullable = null
                }
            };

            var result = NewRule().GetNonConsecutiveLearningDeliveryFAMs(learningDeliveryFAMs);

            result.Count().Should().Be(1);
        }

        [Fact]
        public void Validate_Error()
        {
            var learningDeliveryFamOne = new TestLearningDeliveryFAM
            {
                LearnDelFAMType = "ACT",
                LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                LearnDelFAMDateToNullable = new DateTime(2018, 8, 31)
            };

            var learningDeliveryFamTwo = new TestLearningDeliveryFAM
            {
                LearnDelFAMType = "ACT",
                LearnDelFAMDateFromNullable = new DateTime(2018, 9, 1),
                LearnDelFAMDateToNullable = new DateTime(2018, 9, 2)
            };

            var learningDeliveryFamThree = new TestLearningDeliveryFAM
            {
                LearnDelFAMType = "ACT",
                LearnDelFAMDateFromNullable = new DateTime(2018, 9, 2),
            };

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
               learningDeliveryFamOne,
               learningDeliveryFamTwo,
               learningDeliveryFamThree
            };

            var learner = new TestLearner()
            {
                LearnRefNumber = "00100309",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 1,
                        LearningDeliveryFAMs = learningDeliveryFAMs
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
                validationErrorHandlerMock.Verify(h => h.BuildErrorMessageParameter(It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(5));
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learningDeliveryFamOne = new TestLearningDeliveryFAM
            {
                LearnDelFAMType = "ACT",
                LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                LearnDelFAMDateToNullable = new DateTime(2018, 8, 31)
            };

            var learningDeliveryFamTwo = new TestLearningDeliveryFAM
            {
                LearnDelFAMType = "ACT",
                LearnDelFAMDateFromNullable = new DateTime(2018, 9, 1),
                LearnDelFAMDateToNullable = new DateTime(2018, 9, 2)
            };

            var learningDeliveryFamThree = new TestLearningDeliveryFAM
            {
                LearnDelFAMType = "ACT",
                LearnDelFAMDateFromNullable = new DateTime(2018, 9, 3),
            };

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
               learningDeliveryFamOne,
               learningDeliveryFamTwo,
               learningDeliveryFamThree
            };

            var learner = new TestLearner()
            {
                LearnRefNumber = "00100309",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 1,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 2,
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
        public void BuildErrorMessageParameters()
        {
            var famType = Monitoring.Delivery.Types.ApprenticeshipContract;
            var learnPlanEndDate = new DateTime(2018, 8, 1);
            var learnActEndDate = new DateTime(2018, 8, 1);
            var learnDelFamDateFrom = new DateTime(2018, 8, 1);
            var learnDelFamDateTo = new DateTime(2018, 8, 1);

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnPlanEndDate", "01/08/2018")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnActEndDate", "01/08/2018")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnDelFAMType", "ACT")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnDelFAMDateFrom", "01/08/2018")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnDelFAMDateTo", "01/08/2018")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(learnPlanEndDate, learnActEndDate, famType, learnDelFamDateFrom, learnDelFamDateTo);

            validationErrorHandlerMock.Verify();
        }

        public R104Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new R104Rule(validationErrorHandler);
        }
    }
}
