using System;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Rules.Derived;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Derived
{
    public class DerivedData_36RuleTests
    {
        [Fact]
        public void DeriveEffectiveEndDate_ReturnsNull()
        {
            DD().DeriveEffectiveEndDate(new TestLearningDelivery()).Should().BeNull();
        }

        [Fact]
        public void DeriveEffectiveEndDate_ReturnsNull_NotProgAim()
        {
            DD().DeriveEffectiveEndDate(new TestLearningDelivery { AimType = 2 }).Should().BeNull();
        }

        [Theory]
        [InlineData(20)]
        [InlineData(null)]
        [InlineData(25)]
        public void DeriveEffectiveEndDate_ReturnsLearnActEndDate(int? progType)
        {
            var learnActEndDate = new DateTime(2019, 8, 1);

            var delivery = new TestLearningDelivery
            {
                AimType = 1,
                ProgTypeNullable = progType,
                LearnActEndDateNullable = learnActEndDate
            };

            DD().DeriveEffectiveEndDate(delivery).Should().Be(learnActEndDate);
        }

        [Fact]
        public void DeriveEffectiveEndDate_ReturnsAchDate()
        {
            var learnActEndDate = new DateTime(2019, 8, 1);
            var achDate = new DateTime(2019, 8, 1);

            var delivery = new TestLearningDelivery
            {
                AimType = 1,
                ProgTypeNullable = 25,
                LearnActEndDateNullable = learnActEndDate,
                AchDateNullable = achDate
            };

            DD().DeriveEffectiveEndDate(delivery).Should().Be(achDate);
        }

        private DerivedData_36Rule DD()
        {
            return new DerivedData_36Rule();
        }
    }
}
