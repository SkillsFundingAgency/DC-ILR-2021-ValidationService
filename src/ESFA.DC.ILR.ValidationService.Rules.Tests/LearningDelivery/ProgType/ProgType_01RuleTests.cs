using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.ProgType;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.ProgType
{
    public class ProgType_01RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("ProgType_01", result);
        }

        [Fact]
        public void ConditionMetWithNullLearningDeliveryReturnsTrue()
        {
            var sut = NewRule();

            var result = sut.ConditionMet(null);

            Assert.True(result);
        }

        [Fact]
        public void ConditionMetWithLearningDeliveryNullProgTypeReturnsFalse()
        {
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();

            var result = sut.ConditionMet(mockDelivery.Object);

            Assert.False(result);
        }

        [Fact]
        public void ConditionMetWithLearningDeliveryAndProgTypeReturnsTrue()
        {
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery.SetupGet(x => x.ProgTypeNullable).Returns(1);

            var result = sut.ConditionMet(mockDelivery.Object);

            Assert.True(result);
        }

        [Theory]
        [InlineData(AimTypes.ProgrammeAim, 1, null)]
        [InlineData(AimTypes.ComponentAimInAProgramme, 2, null)]
        public void InvalidItemRaisesValidationMessage(int aimType, int aimSeqNumber, int? progType)
        {
            const string LearnRefNumber = "123456789X";

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.AimType)
                .Returns(aimType);
            mockDelivery
                .SetupGet(x => x.AimSeqNumber)
                .Returns(aimSeqNumber);
            mockDelivery
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(progType);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            mockHandler.Setup(x => x.Handle(
                Moq.It.Is<string>(y => y == ProgType_01Rule.Name),
                Moq.It.Is<string>(y => y == LearnRefNumber),
                aimSeqNumber,
                Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));

            mockHandler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == "AimType"),
                    Moq.It.Is<int>(y => y == aimType)))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var sut = new ProgType_01Rule(mockHandler.Object);

            sut.Validate(mockLearner.Object);

            mockHandler.VerifyAll();
        }

        [Theory]
        [InlineData(AimTypes.ProgrammeAim, 1, 2)]
        [InlineData(AimTypes.ComponentAimInAProgramme, 2, 3)]
        [InlineData(AimTypes.AimNotPartOfAProgramme, 3, 5)]
        [InlineData(AimTypes.CoreAim16To19ExcludingApprenticeships, 4, 1)]
        [InlineData(AimTypes.AimNotPartOfAProgramme, 5, null)]
        [InlineData(AimTypes.CoreAim16To19ExcludingApprenticeships, 6, null)]
        public void ValidItemDoesNotRaiseAValidationMessage(int aimType, int aimSeqNumber, int? progType)
        {
            const string LearnRefNumber = "123456789X";

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.AimType)
                .Returns(aimType);
            mockDelivery
                .SetupGet(x => x.AimSeqNumber)
                .Returns(aimSeqNumber);
            mockDelivery
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(progType);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var sut = new ProgType_01Rule(mockHandler.Object);

            sut.Validate(mockLearner.Object);

            mockHandler.VerifyAll();
        }

        public ProgType_01Rule NewRule()
        {
            var mock = new Mock<IValidationErrorHandler>();

            return new ProgType_01Rule(mock.Object);
        }
    }
}
