using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.HE;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE
{
    public class LearnerHE_02RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("LearnerHE_02", result);
        }

        [Fact]
        public void ConditionMetWithNullLearningHEReturnsTrue()
        {
            var sut = NewRule();

            var result = sut.ConditionMet(null, null);

            Assert.True(result);
        }

        [Fact]
        public void ConditionMetWithNullLearningDeliveriesReturnsFalse()
        {
            var sut = NewRule();
            var mock = new Mock<ILearnerHE>();

            var result = sut.ConditionMet(mock.Object, null);

            Assert.False(result);
        }

        [Fact]
        public void ConditionMetWithNoLearningDeliveriesReturnsFalse()
        {
            var sut = NewRule();
            var mock = new Mock<ILearnerHE>();
            var learningDeliveries = new List<ILearningDelivery>();

            var result = sut.ConditionMet(mock.Object, learningDeliveries);

            Assert.False(result);
        }

        [Fact]
        public void ConditionMetWithLearningDeliveriesAndNoHEMatchReturnsFalse()
        {
            var sut = NewRule();
            var mock = new Mock<ILearnerHE>();

            var mockDelivery = new Mock<ILearningDelivery>();

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var result = sut.ConditionMet(mock.Object, deliveries);

            Assert.False(result);
        }

        [Fact]
        public void ConditionMetWithLearningDeliveriesAndHEMatchReturnsTrue()
        {
            var sut = NewRule();
            var mock = new Mock<ILearnerHE>();

            var mockDelivery = new Mock<ILearningDelivery>();
            var mockDeliveryHE = new Mock<ILearningDeliveryHE>();

            mockDelivery.SetupGet(x => x.LearningDeliveryHEEntity)
                .Returns(mockDeliveryHE.Object);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var result = sut.ConditionMet(mock.Object, deliveries);

            Assert.True(result);
        }

        [Fact]
        public void ConditionMetWithNullHEAndLearningDeliveriesReturnsTrue()
        {
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            var mockDeliveryHE = new Mock<ILearningDeliveryHE>();

            mockDelivery.SetupGet(x => x.LearningDeliveryHEEntity)
                .Returns(mockDeliveryHE.Object);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var result = sut.ConditionMet(null, deliveries);

            Assert.True(result);
        }

        [Fact]
        public void InvalidItemRaisesValidationMessage()
        {
            const string LearnRefNumber = "123456789X";

            var mock = new Mock<ILearner>();
            mock.SetupGet(x => x.LearnRefNumber).Returns(LearnRefNumber);

            var mockHE = new Mock<ILearnerHE>();
            mock.SetupGet(x => x.LearnerHEEntity).Returns(mockHE.Object);

            var mockDelivery = new Mock<ILearningDelivery>();

            var deliveries = new List<ILearningDelivery>();
            mock.SetupGet(x => x.LearningDeliveries).Returns(deliveries);

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            mockHandler.Setup(x => x.Handle(
                Moq.It.Is<string>(y => y == LearnerHE_02Rule.Name),
                Moq.It.Is<string>(y => y == LearnRefNumber),
                null,
                Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));

            mockHandler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == LearnerHE_02Rule.MessagePropertyName),
                    Moq.It.Is<object>(y => y == mockHE.Object)))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var sut = new LearnerHE_02Rule(mockHandler.Object);

            sut.Validate(mock.Object);

            mockHandler.VerifyAll();
        }

        [Fact]
        public void ValidItemDoesNotRaiseAValidationMessage()
        {
            const string LearnRefNumber = "123456789X";

            var mock = new Mock<ILearner>();
            mock.SetupGet(x => x.LearnRefNumber).Returns(LearnRefNumber);

            var mockHE = new Mock<ILearnerHE>();
            mock.SetupGet(x => x.LearnerHEEntity).Returns(mockHE.Object);

            var mockDelivery = new Mock<ILearningDelivery>();
            var mockDeliveryHE = new Mock<ILearningDeliveryHE>();

            mockDelivery.SetupGet(x => x.LearningDeliveryHEEntity)
                .Returns(mockDeliveryHE.Object);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);
            mock.SetupGet(x => x.LearningDeliveries).Returns(deliveries);

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var sut = new LearnerHE_02Rule(mockHandler.Object);

            sut.Validate(mock.Object);

            mockHandler.VerifyAll();
        }

        public LearnerHE_02Rule NewRule()
        {
            var mock = new Mock<IValidationErrorHandler>();

            return new LearnerHE_02Rule(mock.Object);
        }
    }
}
