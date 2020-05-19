using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Derived
{
    public class DerivedData_35RuleTests
    {
        private readonly List<string> _famCodesSOF = new List<string>()
        {
            LearningDeliveryFAMCodeConstants.SOF_MCAGLA_GMCA,
            LearningDeliveryFAMCodeConstants.SOF_MCAGLA_LCRCA,
            LearningDeliveryFAMCodeConstants.SOF_MCAGLA_WMCA,
            LearningDeliveryFAMCodeConstants.SOF_MCAGLA_WECA,
            LearningDeliveryFAMCodeConstants.SOF_MCAGLA_TVCA,
            LearningDeliveryFAMCodeConstants.SOF_MCAGLA_CPCA,
            LearningDeliveryFAMCodeConstants.SOF_MCAGLA_London,
            LearningDeliveryFAMCodeConstants.SOF_MCAGLA_NTCA
        };

        [Fact]
        public void FAMConditionMet_True()
        {
            List<TestLearningDeliveryFAM> deliveryFAMs = GetLearnDeliveryFAMList();

            var fAMQrySrvc = new Mock<ILearningDeliveryFAMQueryService>();
            fAMQrySrvc.Setup(x => x.HasAnyLearningDeliveryFAMCodesForType(deliveryFAMs, "SOF", _famCodesSOF)).Returns(true);

            var rule = NewRule(fAMQrySrvc.Object).LearningDeliveryFAMConditionMet(deliveryFAMs);
            rule.Should().BeTrue();

            fAMQrySrvc.Verify(x => x.HasAnyLearningDeliveryFAMCodesForType(deliveryFAMs, "SOF", _famCodesSOF), Times.Once);
        }

        [Fact]
        public void FAMConditionMet_False()
        {
            var deliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMCode = "110"
                }
            };

            var fAMQrySrvc = new Mock<ILearningDeliveryFAMQueryService>();
            fAMQrySrvc.Setup(x => x.HasAnyLearningDeliveryFAMCodesForType(deliveryFAMs, "SOF", _famCodesSOF)).Returns(false);

            var rule = NewRule(fAMQrySrvc.Object).LearningDeliveryFAMConditionMet(deliveryFAMs);
            rule.Should().BeFalse();

            fAMQrySrvc.Verify(x => x.HasAnyLearningDeliveryFAMCodesForType(deliveryFAMs, "SOF", _famCodesSOF), Times.Once);
        }

        [Fact]
        public void IsCombinedAuthorities_True()
        {
            var deliveryFAMs = GetLearnDeliveryFAMList();
            var delivery = new TestLearningDelivery
            {
                LearnAimRef = "ZPROG001",
                LearningDeliveryFAMs = deliveryFAMs
            };

            var fAMQrySrvc = new Mock<ILearningDeliveryFAMQueryService>();
            fAMQrySrvc.Setup(x => x.HasAnyLearningDeliveryFAMCodesForType(deliveryFAMs, "SOF", _famCodesSOF)).Returns(true);

            var rule = NewRule(fAMQrySrvc.Object).IsCombinedAuthorities(delivery);
            rule.Should().BeTrue();

            fAMQrySrvc.Verify(x => x.HasAnyLearningDeliveryFAMCodesForType(deliveryFAMs, "SOF", _famCodesSOF), Times.Once);
        }

        [Fact]
        public void IsCombinedAuthorities_False()
        {
            var deliveryFAMs = GetLearnDeliveryFAMList();
            var delivery = new TestLearningDelivery
            {
                LearnAimRef = "ZPROG001",
                LearningDeliveryFAMs = deliveryFAMs
            };

            var fAMQrySrvc = new Mock<ILearningDeliveryFAMQueryService>();
            fAMQrySrvc.Setup(x => x.HasAnyLearningDeliveryFAMCodesForType(deliveryFAMs, "SOF", _famCodesSOF)).Returns(false);

            var rule = NewRule(fAMQrySrvc.Object).IsCombinedAuthorities(delivery);
            rule.Should().BeFalse();

            fAMQrySrvc.Verify(x => x.HasAnyLearningDeliveryFAMCodesForType(deliveryFAMs, "SOF", _famCodesSOF), Times.Once);
        }

        [Fact]
        public void NullLearningDelivery_False()
        {
            ILearningDelivery learningDelivery = null;

            NewRule().IsCombinedAuthorities(learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void NullLearningDeliveryFams_False()
        {
            ILearningDelivery learningDelivery = new TestLearningDelivery();

            NewRule().IsCombinedAuthorities(learningDelivery).Should().BeFalse();
        }

        public DerivedData_35Rule NewRule(ILearningDeliveryFAMQueryService learnDelFAMQueryService = null)
        {
            return new DerivedData_35Rule(learnDelFAMQueryService);
        }

        private List<TestLearningDeliveryFAM> GetLearnDeliveryFAMList()
        {
            return new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "SOF",
                    LearnDelFAMCode = "110"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMCode = "110"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "SOF",
                    LearnDelFAMCode = "111"
                },
            };
        }
    }
}
