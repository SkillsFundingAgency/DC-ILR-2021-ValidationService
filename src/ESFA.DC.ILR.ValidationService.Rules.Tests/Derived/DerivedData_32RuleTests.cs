using System;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Derived
{
    public class DerivedData_32RuleTests
    {
        [Fact]
        public void IsOpenApprenticeshipFundedProgramme_True()
        {
            var learningDeliveryMock = new Mock<ILearningDelivery>();

            learningDeliveryMock.SetupGet(ld => ld.AimType).Returns(1);
            learningDeliveryMock.SetupGet(ld => ld.FundModel).Returns(36);
            learningDeliveryMock.SetupGet(ld => ld.LearnActEndDateNullable).Returns((DateTime?)null);

            NewRule().IsOpenApprenticeshipFundedProgramme(learningDeliveryMock.Object).Should().BeTrue();
        }

        [Fact]
        public void IsOpenApprenticeshipFundedProgramme_False()
        {
            var learningDeliveryMock = new Mock<ILearningDelivery>();

            learningDeliveryMock.SetupGet(ld => ld.AimType).Returns(0);

            NewRule().IsOpenApprenticeshipFundedProgramme(learningDeliveryMock.Object).Should().BeFalse();
        }

        [Fact]
        public void AimTypeConditionMet_True()
        {
            NewRule().AimTypeConditionMet(1).Should().BeTrue();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(2)]
        public void AimTypeConditionMet_False(int aimType)
        {
            NewRule().AimTypeConditionMet(aimType).Should().BeFalse();
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            NewRule().FundModelConditionMet(36).Should().BeTrue();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(35)]
        [InlineData(37)]
        public void FundModelConditionMet_False(int fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeFalse();
        }

        [Fact]
        public void LearnStartDateConditionMet_True()
        {
            NewRule().LearnActEndDateConditionMet(null).Should().BeTrue();
        }

        [Fact]
        public void LearnStartDateConditionMet_False()
        {
            NewRule().LearnActEndDateConditionMet(new DateTime(2018, 1, 1)).Should().BeFalse();
        }

        private DerivedData_32Rule NewRule()
        {
            return new DerivedData_32Rule();
        }
    }
}
