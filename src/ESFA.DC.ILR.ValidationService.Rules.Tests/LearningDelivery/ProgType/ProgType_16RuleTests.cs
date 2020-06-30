using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.ProgType;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.ProgType
{
    public class ProgType_16RuleTests
    {
        private readonly string[] _validLearnAimRefTypes = { "1468" };

        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("ProgType_16");
        }

        [Fact]
        public void ProgTypeConditionMet_True()
        {
            NewRule().ProgTypeConditionMet(30).Should().BeTrue();
        }

        [Fact]
        public void ProgTypeConditionMet_Null_True()
        {
            NewRule().ProgTypeConditionMet(null).Should().BeTrue();
        }

        [Fact]
        public void ProgTypeConditionMet_False()
        {
            NewRule().ProgTypeConditionMet(31).Should().BeFalse();
        }

        [Fact]
        public void LARSConditionMet_True()
        {
            var larsDataMock = new Mock<ILARSDataService>();
            larsDataMock.Setup(ld => ld.HasAnyLearningDeliveryForLearnAimRefAndTypes("match", _validLearnAimRefTypes)).Returns(true);

            NewRule(larsDataMock.Object).LARSConditionMet("match").Should().BeTrue();
        }

        [Fact]
        public void LARSConditionMet_False()
        {
            var larsDataMock = new Mock<ILARSDataService>();
            larsDataMock.Setup(ld => ld.HasAnyLearningDeliveryForLearnAimRefAndTypes("noMatch", _validLearnAimRefTypes)).Returns(false);

            NewRule(larsDataMock.Object).LARSConditionMet("noMatch").Should().BeFalse();
        }

        [Theory]
        [InlineData(false, null, false)]
        [InlineData(true, null, true)]
        [InlineData(false, 30, false)]
        [InlineData(false, 31, false)]
        [InlineData(true, 30, true)]
        [InlineData(true, 31, false)]
        public void ConditionMet_False_LearnStartDate(bool larsDataResult, int? progType, bool expected)
        {
            var larsDataMock = new Mock<ILARSDataService>();
            larsDataMock
                .Setup(ldsm => ldsm.HasAnyLearningDeliveryForLearnAimRefAndTypes("match", _validLearnAimRefTypes))
                .Returns(larsDataResult);

            NewRule(larsDataMock.Object).ConditionMet("match", progType).Should().Be(expected);
        }

        private ProgType_16Rule NewRule(
            ILARSDataService larsDataService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new ProgType_16Rule(validationErrorHandler, larsDataService);
        }
    }
}