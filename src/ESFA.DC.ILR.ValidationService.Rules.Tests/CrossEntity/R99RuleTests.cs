using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R99RuleTests : AbstractRuleTests<R99Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R99");
        }

        [Fact]
        public void CompareAgainstOtherDeliveries_NoMatch()
        {
            var learningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery(),
                new TestLearningDelivery(),
            };

            NewRule().CompareAgainstOtherDeliveries(learningDeliveries, (a, b) => false).Should().BeEmpty();
        }

        [Fact]
        public void CompareAgainstOtherDeliveries_AllMatch()
        {
            var learningDeliveryOne = new TestLearningDelivery();
            var learningDeliveryTwo = new TestLearningDelivery();

            var learningDeliveries = new List<ILearningDelivery>()
            {
                learningDeliveryOne,
                learningDeliveryTwo,
            };

            var matches = NewRule().CompareAgainstOtherDeliveries(learningDeliveries, (a, b) => true).ToList();

            matches.Should().HaveCount(2);
            matches[0].Should().BeSameAs(learningDeliveryOne);
            matches[1].Should().BeSameAs(learningDeliveryTwo);
        }

        [Fact]
        public void CompareAgainstOtherDeliveries_PartialMatch()
        {
            var learningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery() { ProgTypeNullable = 1 },
                new TestLearningDelivery() { ProgTypeNullable = 2 },
            };

            NewRule().CompareAgainstOtherDeliveries(learningDeliveries, (a, b) => a.ProgTypeNullable > b.ProgTypeNullable).Should().HaveCount(1);
        }

        [Fact]
        public void CompareAgainstOtherDeliveries_SingleItem()
        {
            var learningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery()
            };

            NewRule().CompareAgainstOtherDeliveries(learningDeliveries, (a, b) => true).Should().BeEmpty();
        }

        [Fact]
        public void CompareAgainstOtherDeliveries_Empty()
        {
            var learningDeliveries = new List<ILearningDelivery>();

            NewRule().CompareAgainstOtherDeliveries(learningDeliveries, (a, b) => false).Should().BeEmpty();
        }

        [Fact]
        public void CompareAgainstOtherDeliveries_Break()
        {
            var matchOne = new TestLearningDelivery() { ProgTypeNullable = 1 };
            var matchTwo = new TestLearningDelivery() { ProgTypeNullable = 2 };
            var nonMatch = new TestLearningDelivery() { ProgTypeNullable = 3 };

            var learningDeliveries = new List<ILearningDelivery>()
            {
                matchOne,
                matchTwo,
                nonMatch
            };

            var matches = NewRule().CompareAgainstOtherDeliveries(learningDeliveries, (a, b) => a.ProgTypeNullable < b.ProgTypeNullable).ToList();

            matches.Should().HaveCount(2);
            matches[0].Should().BeSameAs(matchOne);
            matches[1].Should().BeSameAs(matchTwo);
        }

        [Fact]
        public void GetProgrammeAims_NoMatch()
        {
            var learningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    AimType = 2,
                },
                new TestLearningDelivery()
                {
                    AimType = 3,
                }
            };

            NewRule().GetProgrammeAims(learningDeliveries).Should().BeEmpty();
        }

        [Fact]
        public void GetProgrammeAims_Match()
        {
            var match = new TestLearningDelivery()
            {
                AimType = 1,
            };

            var learningDeliveries = new List<ILearningDelivery>()
            {
                match,
                new TestLearningDelivery()
                {
                    AimType = 3,
                }
            };

            var matches = NewRule().GetProgrammeAims(learningDeliveries).ToList();

            matches.Should().HaveCount(1);
            matches.First().Should().BeSameAs(match);
        }

        [Fact]
        public void LearningDeliveryCountConditionMet_True()
        {
            var learningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery(),
                new TestLearningDelivery(),
            };

            NewRule().HasMoreThanOneProgrammeAim(learningDeliveries).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryCountConditionMet_False()
        {
            var learningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery()
            };

            NewRule().HasMoreThanOneProgrammeAim(learningDeliveries).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var learningDeliveryOne = new TestLearningDelivery
            {
                AimType = 1,
                LearnStartDate = new DateTime(2021, 1, 1)
            };

            var learningDeliveryTwo = new TestLearningDelivery
            {
                AimType = 1,
                LearnStartDate = new DateTime(2018, 1, 1),
                LearnActEndDateNullable = new DateTime(2020, 1, 1)
            };

            NewRule().ConditionMet(learningDeliveryOne, learningDeliveryTwo).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learningDeliveryOne = new TestLearningDelivery
            {
                AimType = 1,
                LearnStartDate = new DateTime(2021, 1, 1)
            };

            var learningDeliveryTwo = new TestLearningDelivery
            {
                AimType = 1,
                LearnStartDate = new DateTime(2018, 1, 1)
            };

            NewRule().ConditionMet(learningDeliveryOne, learningDeliveryTwo).Should().BeTrue();
        }

        [Fact]
        public void Validate_Valid_One_LearningDelivery()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = 2,
                        LearnStartDate = new DateTime(2018, 1, 1)
                    },
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Valid_No_Overlapping()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        LearnStartDate = new DateTime(2021, 1, 1)
                    },
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        LearnStartDate = new DateTime(2018, 1, 1),
                        LearnActEndDateNullable = new DateTime(2020, 1, 1)
                    },
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Valid_SingleProgAim()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        LearnStartDate = new DateTime(2021, 1, 1)
                    },
                    new TestLearningDelivery()
                    {
                        AimType = 2,
                        LearnStartDate = new DateTime(2018, 1, 1),
                    },
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_InValid_Overlapping()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        LearnStartDate = new DateTime(2021, 1, 1)
                    },
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        LearnStartDate = new DateTime(2018, 1, 1),
                    },
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_InValid_MultipleOverlapping()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        LearnStartDate = new DateTime(2021, 1, 1)
                    },
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        LearnStartDate = new DateTime(2018, 1, 1),
                    },
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        LearnStartDate = new DateTime(2018, 1, 1),
                    },
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        LearnStartDate = new DateTime(2018, 1, 1),
                    },
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);

                validationErrorHandlerMock.Verify(h => h.BuildErrorMessageParameter(It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(12));
            }
        }

        private R99Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new R99Rule(validationErrorHandler ?? Mock.Of<IValidationErrorHandler>());
        }
    }
}
