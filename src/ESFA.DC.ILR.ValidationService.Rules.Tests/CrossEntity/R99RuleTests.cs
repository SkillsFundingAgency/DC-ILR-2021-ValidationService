using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
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

            NewRule().CompareAgainstOtherDeliveries(learningDeliveries, (a, b, c) => false).Should().BeEmpty();
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

            var matches = NewRule().CompareAgainstOtherDeliveries(learningDeliveries, (a, b, c) => true).ToList();

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

            NewRule().CompareAgainstOtherDeliveries(learningDeliveries, (a, b, c) => a.ProgTypeNullable > b.ProgTypeNullable).Should().HaveCount(1);
        }

        [Fact]
        public void CompareAgainstOtherDeliveries_SingleItem()
        {
            var learningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery()
            };

            NewRule().CompareAgainstOtherDeliveries(learningDeliveries, (a, b, c) => true).Should().BeEmpty();
        }

        [Fact]
        public void CompareAgainstOtherDeliveries_Empty()
        {
            var learningDeliveries = new List<ILearningDelivery>();

            NewRule().CompareAgainstOtherDeliveries(learningDeliveries, (a, b, c) => false).Should().BeEmpty();
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

            var matches = NewRule().CompareAgainstOtherDeliveries(learningDeliveries, (a, b, c) => a.ProgTypeNullable < b.ProgTypeNullable).ToList();

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
        public void GetProgrammeAims_FilteringExclusions()
        {
            var match = new TestLearningDelivery()
            {
                AimType = 1,
                AimSeqNumber = 1,
                FundModel = 36,
                ProgTypeNullable = 25,
                LearnStartDate = new DateTime(2018, 5, 11),
                LearnActEndDateNullable = new DateTime(2019, 8, 20),
            };

            var learningDeliveries = new List<ILearningDelivery>()
            {
                match,
                new TestLearningDelivery()
                {
                    AimType = 1,
                    AimSeqNumber = 2,
                    FundModel = 36,
                    ProgTypeNullable = 25,
                    LearnStartDate = new DateTime(2019, 11, 19),
                    LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .SetupSequence(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "RES"))
                .Returns(false).Returns(true);

            var matches = NewRule(learningDeliveryFamQueryService: learningDeliveryFamQueryServiceMock.Object).GetProgrammeAims(learningDeliveries).ToList();

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

        public DateTime? GetNullableDate(string candidate) =>
            string.IsNullOrWhiteSpace(candidate) ? (DateTime?)null : DateTime.Parse(candidate);

        [Theory]
        [InlineData("2016-03-31", "2016-04-01", "2017-04-01", false)] // below lower limit
        [InlineData("2016-04-01", "2016-04-01", "2017-04-01", true)] // on lower limit
        [InlineData("2016-09-16", "2016-04-01", "2017-04-01", true)] // inside
        [InlineData("2017-04-01", "2016-04-01", "2017-04-01", true)] // on upper limit
        [InlineData("2017-04-02", "2016-04-01", "2017-04-01", false)] // outside upper limit
        [InlineData("2019-06-09", "2016-04-01", null, true)] // open ended
        public void HasOverlappingAimEndDatesMeetsExpectation(string candidate, string start, string end, bool expectation)
        {
            var delivery = new TestLearningDelivery()
            {
                LearnStartDate = DateTime.Parse(candidate)
            };

            var temp = new TestLearningDelivery()
            {
                LearnStartDate = DateTime.Parse(start),
                LearnActEndDateNullable = GetNullableDate(end)
            };

            NewRule().OverlappingAimEndDatesConditionMet(delivery, temp).Should().Be(expectation);
        }

        [Fact]
        public void Excluded_False_No_RestartNoWithdrawn()
        {
            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "RES"))
                .Returns(false);

            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = 36,
                ProgTypeNullable = 25,
                LearnStartDate = new DateTime(2019, 2, 2)
            };

            var comparisonLearningDelivery = new TestLearningDelivery()
            {
                FundModel = 36,
                ProgTypeNullable = 25,
                LearnStartDate = new DateTime(2018, 1, 1),
                LearnActEndDateNullable = new DateTime(2019, 1, 1),
                AchDateNullable = new DateTime(2020, 1, 1),
            };

            NewRule(null, learningDeliveryFamQueryServiceMock.Object).Excluded(learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void Excluded_True_Restart()
        {
            var learningDeliveryFAM = new TestLearningDeliveryFAM()
            {
                LearnDelFAMType = "RES"
            };

            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = 36,
                ProgTypeNullable = 25,
                LearnStartDate = new DateTime(2019, 2, 2),
                LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                {
                    learningDeliveryFAM
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "RES"))
                .Returns(true);

            NewRule(null, learningDeliveryFamQueryServiceMock.Object).Excluded(learningDelivery).Should().BeTrue();
        }

        [Fact]
        public void Excluded_True_Withdrawn()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = 36,
                ProgTypeNullable = 25,
                LearnStartDate = new DateTime(2019, 2, 2),
                WithdrawReasonNullable = 97
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "RES"))
                .Returns(false);

            NewRule(null, learningDeliveryFamQueryServiceMock.Object).Excluded(learningDelivery).Should().BeTrue();
        }

        [Fact]
        public void MultipleUnknownLearnActEndDateConditionMet_True()
        {
            var learningDeliveryOne = new TestLearningDelivery();
            var learningDeliveryTwo = new TestLearningDelivery();

            NewRule().MultipleUnknownLearnActEndDateConditionMet(learningDeliveryOne, learningDeliveryTwo).Should().BeTrue();
        }

        [Fact]
        public void MultipleUnknownLearnActEndDateConditionMet_False_BothSupplied()
        {
            var learningDeliveryOne = new TestLearningDelivery()
            {
                LearnActEndDateNullable = new DateTime(2019, 2, 2)
            };

            var learningDeliveryTwo = new TestLearningDelivery()
            {
                LearnActEndDateNullable = new DateTime(2019, 2, 2)
            };

            NewRule().MultipleUnknownLearnActEndDateConditionMet(learningDeliveryOne, learningDeliveryTwo).Should().BeFalse();
        }

        [Fact]
        public void MultipleUnknownLearnActEndDateConditionMet_False_OneSupplied()
        {
            var learningDeliveryOne = new TestLearningDelivery()
            {
                LearnActEndDateNullable = new DateTime(2019, 2, 2)
            };

            var learningDeliveryTwo = new TestLearningDelivery();

            NewRule().MultipleUnknownLearnActEndDateConditionMet(learningDeliveryOne, learningDeliveryTwo).Should().BeFalse();
        }

        [Fact]
        public void ProgAimLearnActEndDateConditionMet_True()
        {
            var learningDeliveryOne = new TestLearningDelivery()
            {
                AimSeqNumber = 1,
                LearnActEndDateNullable = new DateTime(2020, 2, 2)
            };

            var learningDeliveryTwo = new TestLearningDelivery
            {
                AimSeqNumber = 2,
                FundModel = 36,
                ProgTypeNullable = 25,
                LearnStartDate = new DateTime(2019, 8, 1)
            };

            var learningDeliveryThree = new TestLearningDelivery
            {
                AimSeqNumber = 3,
                FundModel = 36,
                ProgTypeNullable = 25,
                LearnStartDate = new DateTime(2019, 8, 1)
            };

            IEnumerable<ILearningDelivery> standardProgAims = new List<TestLearningDelivery>
            {
                learningDeliveryTwo,
                learningDeliveryThree
            };

            NewRule().ProgAimLearnActEndDateConditionMet(learningDeliveryOne, learningDeliveryTwo, standardProgAims).Should().BeTrue();
        }

        [Fact]
        public void ProgAimLearnActEndDateConditionMet_False()
        {
            var learningDeliveryOne = new TestLearningDelivery()
            {
                AimSeqNumber = 1,
                LearnActEndDateNullable = new DateTime(2020, 2, 2)
            };

            var learningDeliveryTwo = new TestLearningDelivery
            {
                AimSeqNumber = 2,
                FundModel = 36,
                ProgTypeNullable = 25,
                LearnStartDate = new DateTime(2020, 8, 1)
            };

            var learningDeliveryThree = new TestLearningDelivery
            {
                AimSeqNumber = 3,
                FundModel = 36,
                ProgTypeNullable = 25,
                LearnStartDate = new DateTime(2019, 8, 1),
                AchDateNullable = new DateTime(2020, 7, 31)
            };

            IEnumerable<ILearningDelivery> standardProgAims = new List<TestLearningDelivery>
            {
                learningDeliveryTwo,
                learningDeliveryThree
            };

            NewRule().ProgAimLearnActEndDateConditionMet(learningDeliveryOne, learningDeliveryTwo, standardProgAims).Should().BeFalse();
        }

        [Fact]
        public void ProgAimLearnActEndDateConditionMet_False_NoLearnActEndDate()
        {
            var learningDeliveryOne = new TestLearningDelivery
            {
                AimSeqNumber = 1,
            };

            var learningDeliveryTwo = new TestLearningDelivery
            {
                AimSeqNumber = 2,
                FundModel = 36,
                ProgTypeNullable = 25,
                LearnStartDate = new DateTime(2019, 8, 1)
            };

            var learningDeliveryThree = new TestLearningDelivery
            {
                AimSeqNumber = 3,
                FundModel = 36,
                ProgTypeNullable = 25,
                LearnStartDate = new DateTime(2019, 8, 1)
            };

            IEnumerable<ILearningDelivery> standardProgAims = new List<TestLearningDelivery>
            {
                learningDeliveryTwo,
                learningDeliveryThree
            };

            NewRule().ProgAimLearnActEndDateConditionMet(learningDeliveryOne, learningDeliveryTwo, standardProgAims).Should().BeFalse();
        }

        [Fact]
        public void ProgAimLearnActEndDateConditionMet_False_NoStandardAims()
        {
            var learningDeliveryOne = new TestLearningDelivery()
            {
                AimSeqNumber = 1,
                LearnActEndDateNullable = new DateTime(2020, 2, 2)
            };

            var learningDeliveryTwo = new TestLearningDelivery
            {
                AimSeqNumber = 2,
                FundModel = 36,
                LearnStartDate = new DateTime(2020, 8, 1)
            };

            var learningDeliveryThree = new TestLearningDelivery
            {
                AimSeqNumber = 3,
                FundModel = 36,
                LearnStartDate = new DateTime(2019, 8, 1),
                AchDateNullable = new DateTime(2020, 7, 31)
            };

            NewRule().ProgAimLearnActEndDateConditionMet(learningDeliveryOne, learningDeliveryTwo, null).Should().BeFalse();
        }

        [Fact]
        public void ApprenticeshipStandardConditionMet_True()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = 36,
                ProgTypeNullable = 25,
                LearnStartDate = new DateTime(2018, 2, 2),
                AchDateNullable = new DateTime(2020, 2, 2)
            };

            var comparison = new TestLearningDelivery()
            {
                FundModel = 36,
                ProgTypeNullable = 25,
                LearnStartDate = new DateTime(2019, 1, 1),
                LearnActEndDateNullable = new DateTime(2020, 1, 1),
            };

            NewRule().ApprenticeshipStandardAchDateConditionMet(learningDelivery, comparison).Should().BeTrue();
        }

        [Fact]
        public void ApprenticeshipStandardsConditionMet_False_LearningDeliveryFundModel()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = 35,
                ProgTypeNullable = 25,
                LearnStartDate = new DateTime(2018, 1, 2)
            };

            var comparison = new TestLearningDelivery()
            {
                FundModel = 35,
                LearnStartDate = new DateTime(2019, 1, 1),
                LearnActEndDateNullable = new DateTime(2020, 1, 1),
            };

            NewRule().ApprenticeshipStandardAchDateConditionMet(learningDelivery, comparison).Should().BeFalse();
        }

        [Fact]
        public void ApprenticeshipStandardsConditionMet_False_DateMisMatch()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = 36,
                ProgTypeNullable = 25,
                LearnStartDate = new DateTime(2020, 1, 2),
                AchDateNullable = new DateTime(2020, 1, 1)
            };

            var comparison = new TestLearningDelivery()
            {
                FundModel = 35,
                LearnStartDate = new DateTime(2018, 1, 1),
                LearnActEndDateNullable = new DateTime(2020, 1, 1),
            };

            NewRule().ApprenticeshipStandardAchDateConditionMet(learningDelivery, comparison).Should().BeFalse();
        }

        [Fact]
        public void ApprenticeshipStandardsConditionMet_False_NoAchDate()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = 36,
                ProgTypeNullable = 25,
                LearnStartDate = new DateTime(2019, 1, 2)
            };

            var comparison = new TestLearningDelivery()
            {
                FundModel = 35,
                LearnStartDate = new DateTime(2020, 1, 1),
                LearnActEndDateNullable = new DateTime(2020, 1, 1),
            };

            NewRule().ApprenticeshipStandardAchDateConditionMet(learningDelivery, comparison).Should().BeFalse();
        }

        [Fact]
        public void AchDateConditionMet_True()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = 36,
                ProgTypeNullable = 25,
                LearnStartDate = new DateTime(2019, 1, 2)
            };

            var comparison = new TestLearningDelivery()
            {
                FundModel = 35,
                LearnStartDate = new DateTime(2020, 1, 1),
                LearnActEndDateNullable = new DateTime(2020, 1, 1),
            };

            NewRule().AchDateConditionMet(learningDelivery, comparison).Should().BeTrue();
        }

        [Fact]
        public void AchDateConditionMet_False_DateMisMatch()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = 36,
                ProgTypeNullable = 25,
                LearnStartDate = new DateTime(2020, 1, 2)
            };

            var comparison = new TestLearningDelivery()
            {
                FundModel = 35,
                LearnStartDate = new DateTime(2020, 1, 1),
                LearnActEndDateNullable = new DateTime(2020, 1, 1),
            };

            NewRule().AchDateConditionMet(learningDelivery, comparison).Should().BeFalse();
        }

        [Fact]
        public void AchDateConditionMet_False_AchDateReturned()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = 36,
                ProgTypeNullable = 25,
                LearnStartDate = new DateTime(2019, 1, 2),
                AchDateNullable = new DateTime(2020, 1, 2)
            };

            var comparison = new TestLearningDelivery()
            {
                FundModel = 35,
                LearnStartDate = new DateTime(2020, 1, 1),
                LearnActEndDateNullable = new DateTime(2020, 1, 1),
            };

            NewRule().AchDateConditionMet(learningDelivery, comparison).Should().BeFalse();
        }

        [Fact]
        public void Validate_Invalid_OpenAims()
        {
            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "RES"))
                .Returns(false);

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        LearnStartDate = new DateTime(2018, 1, 1)
                    },
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        FundModel = 36,
                        LearnStartDate = new DateTime(2017, 1, 1),
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object, learningDeliveryFamQueryServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Invalid_OpenAims_Standard()
        {
            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "RES"))
                .Returns(false);

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        LearnStartDate = new DateTime(2018, 1, 1)
                    },
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        FundModel = 36,
                        ProgTypeNullable = 25,
                        LearnStartDate = new DateTime(2017, 1, 1),
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object, learningDeliveryFamQueryServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Invalid_OverlapActualEndDate()
        {
            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "RES"))
                .Returns(false);

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        LearnStartDate = new DateTime(2018, 1, 1)
                    },
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        FundModel = 36,
                        ProgTypeNullable = 25,
                        LearnStartDate = new DateTime(2017, 1, 1),
                        LearnActEndDateNullable = new DateTime(2019, 1, 1),
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object, learningDeliveryFamQueryServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Invalid_AppStandardCondition()
        {
            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "RES"))
                .Returns(false);

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        LearnStartDate = new DateTime(2019, 1, 1)
                    },
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        FundModel = 36,
                        ProgTypeNullable = 25,
                        LearnStartDate = new DateTime(2017, 1, 1),
                        AchDateNullable = new DateTime(2020, 1, 1),
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object, learningDeliveryFamQueryServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Invalid_AchDateCondition()
        {
            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "RES"))
                .Returns(false);

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        AimSeqNumber = 1,
                        LearnStartDate = new DateTime(2017, 1, 1),
                        LearnActEndDateNullable = new DateTime(2018, 1, 1)
                    },
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        AimSeqNumber = 2,
                        FundModel = 36,
                        ProgTypeNullable = 25,
                        LearnStartDate = new DateTime(2019, 1, 1),
                        LearnActEndDateNullable = new DateTime(2020, 1, 1)
                    },
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        AimSeqNumber = 3,
                        LearnStartDate = new DateTime(2020, 2, 1)
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object, learningDeliveryFamQueryServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Invalid_ProgAimCondition()
        {
            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "RES"))
                .Returns(false);

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        LearnStartDate = new DateTime(2017, 1, 1),
                        LearnActEndDateNullable = new DateTime(2018, 1, 1)
                    },
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        FundModel = 36,
                        ProgTypeNullable = 25,
                        LearnStartDate = new DateTime(2019, 1, 1)
                    },
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        FundModel = 36,
                        ProgTypeNullable = 25,
                        LearnStartDate = new DateTime(2019, 1, 1)
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object, learningDeliveryFamQueryServiceMock.Object).Validate(learner);
            }
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
                        AimType = 1,
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
        public void Validate_Valid_StandardAimHasAchDate()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        AimSeqNumber = 1,
                        LearnStartDate = new DateTime(2021, 1, 1)
                    },
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        AimSeqNumber = 2,
                        ProgTypeNullable = 25,
                        FundModel = 36,
                        LearnStartDate = new DateTime(2018, 1, 2),
                        LearnActEndDateNullable = new DateTime(2020, 1, 1),
                        AchDateNullable = new DateTime(2020, 1, 1)
                    },
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "RES"))
                .Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, learningDeliveryFamQueryServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Valid_Restart()
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
                        FundModel = 36,
                        ProgTypeNullable = 25,
                        LearnStartDate = new DateTime(2018, 1, 1),
                    },
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "RES"))
                .Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object, learningDeliveryFamQueryServiceMock.Object).Validate(learner);
                VerifyErrorHandlerMock(validationErrorHandlerMock, 1);
            }
        }

        [Fact]
        public void Validate_Valid_Withdraw()
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
                        FundModel = 36,
                        ProgTypeNullable = 25,
                        LearnStartDate = new DateTime(2018, 1, 1),
                        WithdrawReasonNullable = 3
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
                VerifyErrorHandlerMock(validationErrorHandlerMock, 1);
            }
        }

        private R99Rule NewRule(IValidationErrorHandler validationErrorHandler = null, ILearningDeliveryFAMQueryService learningDeliveryFamQueryService = null)
        {
            return new R99Rule(validationErrorHandler ?? Mock.Of<IValidationErrorHandler>(), learningDeliveryFamQueryService);
        }
    }
}
