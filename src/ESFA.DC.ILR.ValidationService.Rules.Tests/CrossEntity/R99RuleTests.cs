using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using ESFA.DC.ILR.ValidationService.Utility;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R99RuleTests : AbstractRuleTests<R99Rule>
    {
        [Fact]
        public void RuleName1()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.RuleName;

            // assert
            Assert.Equal("R99", result);
        }

        [Fact]
        public void RuleName2()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.RuleName;

            // assert
            Assert.Equal(RuleNameConstants.R99, result);
        }

        [Fact]
        public void RuleName3()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.RuleName;

            // assert
            Assert.NotEqual("SomeOtherRuleName_07", result);
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

        public DateTime? GetNullableDate(string candidate) =>
            string.IsNullOrWhiteSpace(candidate) ? (DateTime?)null : DateTime.Parse(candidate);

        [Fact]
        public void OpenAimConditionMet_True()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                LearnActEndDateNullable = null,
            };

            var comparison = new TestLearningDelivery()
            {
                LearnActEndDateNullable = null,
            };

            NewRule().OpenAimConditionMet(learningDelivery, comparison).Should().BeTrue();
        }

        [Fact]
        public void OpenAimConditionMet_False_LearningDelivery()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                LearnActEndDateNullable = new DateTime(2013, 1, 1),
            };

            var comparison = new TestLearningDelivery()
            {
                LearnActEndDateNullable = null,
            };

            NewRule().OpenAimConditionMet(learningDelivery, comparison).Should().BeFalse();
        }

        [Fact]
        public void OpenAimConditionMet_False_Comparison()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                LearnActEndDateNullable = null,
            };

            var comparison = new TestLearningDelivery()
            {
                LearnActEndDateNullable = new DateTime(2016, 1, 1),
            };

            NewRule().OpenAimConditionMet(learningDelivery, comparison).Should().BeFalse();
        }

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
        public void AchievementDateConditionMet_True()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = 36,
                LearnActEndDateNullable = new DateTime(2018, 1, 1),
                LearnStartDate = new DateTime(2019, 1, 1)
            };

            var comparison = new TestLearningDelivery()
            {
                FundModel = 36,
                ProgTypeNullable = 25,
                LearnStartDate = new DateTime(2018, 1, 1),
                AchDateNullable = new DateTime(2020, 1, 1),
            };

            NewRule().AchievementDateConditionMet(learningDelivery, comparison).Should().BeTrue();
        }

        [Fact]
        public void AchievementDateConditionMet_False_LearningDeliveryFundModel()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = 35,
                LearnActEndDateNullable = new DateTime(2018, 1, 1),
                LearnStartDate = new DateTime(2019, 1, 1)
            };

            var comparison = new TestLearningDelivery()
            {
                FundModel = 36,
                ProgTypeNullable = 25,
                LearnStartDate = new DateTime(2018, 1, 1),
                AchDateNullable = new DateTime(2020, 1, 1),
            };

            NewRule().AchievementDateConditionMet(learningDelivery, comparison).Should().BeFalse();
        }

        [Fact]
        public void AchievementDateConditionMet_False_LearningDeliveryLearnActEndDate()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = 36,
                LearnActEndDateNullable = null,
                LearnStartDate = new DateTime(2019, 1, 1)
            };

            var comparison = new TestLearningDelivery()
            {
                FundModel = 36,
                ProgTypeNullable = 25,
                LearnStartDate = new DateTime(2018, 1, 1),
                AchDateNullable = new DateTime(2020, 1, 1),
            };

            NewRule().AchievementDateConditionMet(learningDelivery, comparison).Should().BeFalse();
        }

        [Fact]
        public void AchievementDateConditionMet_False_ComparisonFundModel()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = 36,
                LearnActEndDateNullable = new DateTime(2018, 1, 1),
                LearnStartDate = new DateTime(2019, 1, 1)
            };

            var comparison = new TestLearningDelivery()
            {
                FundModel = 35,
                ProgTypeNullable = 25,
                LearnStartDate = new DateTime(2018, 1, 1),
                AchDateNullable = new DateTime(2020, 1, 1),
            };

            NewRule().AchievementDateConditionMet(learningDelivery, comparison).Should().BeFalse();
        }

        [Fact]
        public void AchievementDateConditionMet_False_ComparisonProgType()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = 36,
                LearnActEndDateNullable = new DateTime(2018, 1, 1),
                LearnStartDate = new DateTime(2019, 1, 1)
            };

            var comparison = new TestLearningDelivery()
            {
                FundModel = 36,
                ProgTypeNullable = 20,
                LearnStartDate = new DateTime(2018, 1, 1),
                AchDateNullable = new DateTime(2020, 1, 1),
            };

            NewRule().AchievementDateConditionMet(learningDelivery, comparison).Should().BeFalse();
        }

        [Fact]
        public void AchievementDateConditionMet_False_Dates()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = 36,
                LearnActEndDateNullable = new DateTime(2018, 1, 1),
                LearnStartDate = new DateTime(2017, 1, 1)
            };

            var comparison = new TestLearningDelivery()
            {
                FundModel = 36,
                ProgTypeNullable = 25,
                LearnStartDate = new DateTime(2018, 1, 1),
                AchDateNullable = new DateTime(2020, 1, 1),
            };

            NewRule().AchievementDateConditionMet(learningDelivery, comparison).Should().BeFalse();
        }

        [Fact]
        public void ApprenticeshipStandardConditionMet_True()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = 36,
                AchDateNullable = new DateTime(2018, 1, 1),
                ProgTypeNullable = 25,
                LearnStartDate = new DateTime(2019, 1, 1)
            };

            var comparison = new TestLearningDelivery()
            {
                FundModel = 36,
                LearnStartDate = new DateTime(2018, 1, 1),
                AchDateNullable = new DateTime(2020, 1, 1),
            };

            NewRule().ApprenticeshipStandardConditionMet(learningDelivery, comparison).Should().BeTrue();
        }

        [Fact]
        public void ApprenticeshipStandardsConditionMet_False_LearningDeliveryFundModel()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = 35,
                AchDateNullable = new DateTime(2018, 1, 1),
                ProgTypeNullable = 25,
                LearnStartDate = new DateTime(2019, 1, 1)
            };

            var comparison = new TestLearningDelivery()
            {
                FundModel = 36,
                LearnStartDate = new DateTime(2018, 1, 1),
                AchDateNullable = new DateTime(2020, 1, 1),
            };

            NewRule().ApprenticeshipStandardConditionMet(learningDelivery, comparison).Should().BeFalse();
        }

        [Fact]
        public void ApprenticeshipStandardsConditionMet_False_LearningDeliveryAchDate()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = 36,
                AchDateNullable = null,
                ProgTypeNullable = 25,
                LearnStartDate = new DateTime(2019, 1, 1)
            };

            var comparison = new TestLearningDelivery()
            {
                FundModel = 36,
                LearnStartDate = new DateTime(2018, 1, 1),
                AchDateNullable = new DateTime(2020, 1, 1),
            };

            NewRule().ApprenticeshipStandardConditionMet(learningDelivery, comparison).Should().BeFalse();
        }

        [Fact]
        public void ApprenticeshipStandardsConditionMet_False_ComparisonFundModel()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = 36,
                AchDateNullable = new DateTime(2018, 1, 1),
                ProgTypeNullable = 25,
                LearnStartDate = new DateTime(2019, 1, 1)
            };

            var comparison = new TestLearningDelivery()
            {
                FundModel = 35,
                LearnStartDate = new DateTime(2018, 1, 1),
                AchDateNullable = new DateTime(2020, 1, 1),
            };

            NewRule().ApprenticeshipStandardConditionMet(learningDelivery, comparison).Should().BeFalse();
        }

        [Fact]
        public void ApprenticeshipStandardsConditionMet_False_ComparisonProgType()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = 36,
                AchDateNullable = new DateTime(2018, 1, 1),
                ProgTypeNullable = 20,
                LearnStartDate = new DateTime(2019, 1, 1)
            };

            var comparison = new TestLearningDelivery()
            {
                FundModel = 36,
                LearnStartDate = new DateTime(2018, 1, 1),
                AchDateNullable = new DateTime(2020, 1, 1),
            };

            NewRule().ApprenticeshipStandardConditionMet(learningDelivery, comparison).Should().BeFalse();
        }

        [Fact]
        public void ApprenticeshipStandardsConditionMet_False_Dates()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = 36,
                AchDateNullable = new DateTime(2018, 1, 1),
                ProgTypeNullable = 25,
                LearnStartDate = new DateTime(2017, 1, 1)
            };

            var comparison = new TestLearningDelivery()
            {
                FundModel = 36,
                LearnStartDate = new DateTime(2018, 1, 1),
                AchDateNullable = new DateTime(2020, 1, 1),
            };

            NewRule().ApprenticeshipStandardConditionMet(learningDelivery, comparison).Should().BeFalse();
        }

        [Fact]
        public void Validate_Invalid()
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
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        LearnStartDate = new DateTime(2017, 1, 1),
                        LearnActEndDateNullable = new DateTime(2019, 1, 1),
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Valid()
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

        private R99Rule NewRule(IValidationErrorHandler validationErrorHandler = null) => new R99Rule(validationErrorHandler ?? Mock.Of<IValidationErrorHandler>());
    }
}
