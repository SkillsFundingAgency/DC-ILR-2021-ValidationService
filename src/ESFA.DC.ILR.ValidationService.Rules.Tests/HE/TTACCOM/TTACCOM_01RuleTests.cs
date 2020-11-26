using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.HE.TTACCOM;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.TTACCOM
{
    public class TTACCOM_01RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("TTACCOM_01", result);
        }

        [Fact]
        public void ConditionMetWithNullTTAccomReturnsTrue()
        {
            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var mockService = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            var sut = new TTACCOM_01Rule(mockHandler.Object, mockService.Object);

            var result = sut.ConditionMet(null);

            Assert.True(result);
            mockHandler.VerifyAll();
            mockService.VerifyAll();
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(2, true)]
        [InlineData(3, true)]
        [InlineData(1, false)]
        [InlineData(2, false)]
        [InlineData(3, false)]
        public void ConditionMetWithCandidateMatchesExpectation(int candidate, bool expectation)
        {
            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var mockService = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            mockService
                .Setup(x => x.Contains(TypeOfLimitedLifeLookup.TTAccom, candidate))
                .Returns(expectation);

            var sut = new TTACCOM_01Rule(mockHandler.Object, mockService.Object);

            var result = sut.ConditionMet(candidate);

            Assert.Equal(expectation, result);
            mockHandler.VerifyAll();
            mockService.VerifyAll();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void InvalidItemRaisesValidationMessage(int candidate)
        {
            const string LearnRefNumber = "123456789X";

            var mock = new Mock<ILearner>();
            mock.SetupGet(x => x.LearnRefNumber).Returns(LearnRefNumber);

            var mockHE = new Mock<ILearnerHE>();
            mockHE.SetupGet(x => x.TTACCOMNullable).Returns(candidate);
            mock.SetupGet(x => x.LearnerHEEntity).Returns(mockHE.Object);

            var mockDelivery = new Mock<ILearningDelivery>();

            var deliveries = new List<ILearningDelivery>();
            mock.SetupGet(x => x.LearningDeliveries).Returns(deliveries);

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            mockHandler.Setup(x => x.Handle(
                Moq.It.Is<string>(y => y == TTACCOM_01Rule.Name),
                Moq.It.Is<string>(y => y == LearnRefNumber),
                null,
                Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            mockHandler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == TTACCOM_01Rule.MessagePropertyName),
                    Moq.It.Is<int>(y => y == candidate)))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var mockService = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            mockService
                .Setup(x => x.Contains(TypeOfLimitedLifeLookup.TTAccom, candidate))
                .Returns(false);

            var sut = new TTACCOM_01Rule(mockHandler.Object, mockService.Object);

            sut.Validate(mock.Object);

            mockHandler.VerifyAll();
            mockService.VerifyAll();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void ValidItemDoesNotRaiseAValidationMessage(int candidate)
        {
            const string LearnRefNumber = "123456789X";

            var mock = new Mock<ILearner>();
            mock.SetupGet(x => x.LearnRefNumber).Returns(LearnRefNumber);

            var mockHE = new Mock<ILearnerHE>();
            mockHE.SetupGet(x => x.TTACCOMNullable).Returns(candidate);
            mock.SetupGet(x => x.LearnerHEEntity).Returns(mockHE.Object);

            var mockDelivery = new Mock<ILearningDelivery>();
            var mockDeliveryHE = new Mock<ILearningDeliveryHE>();

            mockDelivery.SetupGet(x => x.LearningDeliveryHEEntity)
                .Returns(mockDeliveryHE.Object);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);
            mock.SetupGet(x => x.LearningDeliveries).Returns(deliveries);

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var mockService = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            mockService
                .Setup(x => x.Contains(TypeOfLimitedLifeLookup.TTAccom, candidate))
                .Returns(true);

            var sut = new TTACCOM_01Rule(mockHandler.Object, mockService.Object);

            sut.Validate(mock.Object);

            mockHandler.VerifyAll();
            mockService.VerifyAll();
        }

        public TTACCOM_01Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>();
            var service = new Mock<IProvideLookupDetails>();

            return new TTACCOM_01Rule(handler.Object, service.Object);
        }
    }
}
