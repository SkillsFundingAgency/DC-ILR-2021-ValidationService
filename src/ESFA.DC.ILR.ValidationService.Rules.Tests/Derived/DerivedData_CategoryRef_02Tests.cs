using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Derived
{
    public class DerivedData_CategoryRef_02Tests
    {
        [Fact]
        public void McaAdultSkillsMatch_True()
        {
            var delivery = new TestLearningDelivery { FundModel = 35 };

            var dd35Mock = new Mock<IDerivedData_35Rule>();
            dd35Mock.Setup(qs => qs.IsCombinedAuthorities(delivery)).Returns(true);

            NewDDRule(dd35: dd35Mock.Object).McaAdultSkillsMatch(delivery).Should().BeTrue();
        }

        [Theory]
        [InlineData(35, false)]
        [InlineData(99, true)]
        [InlineData(99, false)]
        public void McaAdultSkillsMatch_False(int fundModel, bool mock)
        {
            var delivery = new TestLearningDelivery { FundModel = fundModel };

            var dd35Mock = new Mock<IDerivedData_35Rule>();
            dd35Mock.Setup(qs => qs.IsCombinedAuthorities(delivery)).Returns(mock);

            NewDDRule(dd35: dd35Mock.Object).McaAdultSkillsMatch(delivery).Should().BeFalse();
        }

        [Fact]
        public void Derive()
        {
            var delivery = new TestLearningDelivery
            {
                FundModel = 35,
                AimType = 3,
                ProgTypeNullable = 24,
                LearnStartDate = new DateTime(2015, 09, 01),
                LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                {
                    new TestLearningDeliveryFAM
                    {
                        LearnDelFAMType = "SOF",
                        LearnDelFAMCode = "115"
                    },
                    new TestLearningDeliveryFAM
                    {
                        LearnDelFAMType = "LDM",
                        LearnDelFAMCode = "115"
                    }
                }
            };

            var dd35RuleMock = new Mock<IDerivedData_35Rule>();
            dd35RuleMock.Setup(qs => qs.IsCombinedAuthorities(delivery)).Returns(true);

            NewDDRule(dd35RuleMock.Object).Derive(delivery).Should().Be(41);
        }

        [Fact]
        public void Derive_Null()
        {
            var delivery = new TestLearningDelivery
            {
                FundModel = 35,
                AimType = 3,
                ProgTypeNullable = 24,
                LearnStartDate = new DateTime(2015, 09, 01),
                LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                {
                    new TestLearningDeliveryFAM
                    {
                        LearnDelFAMType = "ADL",
                        LearnDelFAMCode = "115"
                    },
                    new TestLearningDeliveryFAM
                    {
                        LearnDelFAMType = "LDM",
                        LearnDelFAMCode = "115"
                    }
                }
            };

            var dd35RuleMock = new Mock<IDerivedData_35Rule>();
            dd35RuleMock.Setup(qs => qs.IsCombinedAuthorities(delivery)).Returns(false);

            NewDDRule(dd35RuleMock.Object).Derive(delivery).Should().BeNull();
        }

        private DerivedData_CategoryRef_02 NewDDRule(IDerivedData_35Rule dd35 = null)
        {
            return new DerivedData_CategoryRef_02(dd35);
        }
    }
}
