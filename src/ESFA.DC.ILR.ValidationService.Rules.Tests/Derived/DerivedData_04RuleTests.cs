using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Rules.Derived;
using FluentAssertions;
using System;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Derived
{
    public class DerivedData_04RuleTests
    {
        [Fact]
        public void Derive()
        {
            var earliestLearningDelivery = new TestLearningDelivery()
            {
                ProgTypeNullable = 1,
                FworkCodeNullable = 1,
                PwayCodeNullable = 1,
                AimType = 1,
                LearnStartDate = new DateTime(2015, 1, 1)
            };

            var latestLearningDelivery = new TestLearningDelivery()
            {
                ProgTypeNullable = 1,
                FworkCodeNullable = 1,
                PwayCodeNullable = 1,
                AimType = 1,
                LearnStartDate = new DateTime(2017, 1, 1)
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    latestLearningDelivery,
                    earliestLearningDelivery
                }
            };

            NewRule().GetEarliesStartDateFor(latestLearningDelivery, learner.LearningDeliveries).Should().Be(new DateTime(2015, 1, 1));
        }

        [Fact]
        public void EarliestLearningDeliveryLearnStartDateFor_NoMatch()
        {
            var learningDeliveries = new TestLearningDelivery[]
            {
                new TestLearningDelivery()
                {
                    AimType = 1,
                    ProgTypeNullable = 1,
                    FworkCodeNullable = 1,
                    PwayCodeNullable = 1,
                    LearnStartDate = new DateTime(2017, 1, 1)
                }
            };

            NewRule().GetEarliesStartDateFor(1, 1, 1, 2, learningDeliveries).Should().BeNull();
        }

        [Fact]
        public void EarliestLearningDeliveryLearnStartDateFor_SingleMatch()
        {
            var learnStartDate = new DateTime(2017, 1, 1);

            var learningDeliveries = new TestLearningDelivery[]
            {
                new TestLearningDelivery()
                {
                    AimType = 1,
                    ProgTypeNullable = 1,
                    FworkCodeNullable = 1,
                    PwayCodeNullable = 1,
                    LearnStartDate = learnStartDate
                },
                new TestLearningDelivery()
                {
                    AimType = 1,
                    ProgTypeNullable = 1,
                    FworkCodeNullable = 1,
                    PwayCodeNullable = 2,
                }
            };

            NewRule().GetEarliesStartDateFor(1, 1, 1, 1, learningDeliveries).Should().Be(learnStartDate);
        }

        private DerivedData_04Rule NewRule()
        {
            return new DerivedData_04Rule();
        }
    }
}
