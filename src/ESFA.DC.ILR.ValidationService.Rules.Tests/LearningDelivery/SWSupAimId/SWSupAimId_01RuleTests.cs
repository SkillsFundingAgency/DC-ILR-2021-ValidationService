using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.SWSupAimId;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.SWSupAimId
{
    public class SWSupAimId_01RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("SWSupAimId_01", result);
        }

        [Fact]
        public void ConditionMetWithNullLearningDeliveryReturnsTrue()
        {
            var sut = NewRule();

            var result = sut.ConditionMet(null);

            Assert.True(result);
        }

        [Theory]
        [InlineData(AimTypes.References.IndustryPlacement, false)]
        [InlineData("550e8400_e29b_41d4_a716_446655440000", true)]
        [InlineData("|550e8400e29b41d4a716446655440000", true)]
        [InlineData("w;oraeijwq rf;oiew ", false)]
        [InlineData(null, false)]
        public void IsValidGuidMeetsExpectation(string candidate, bool expectation)
        {
            var sut = NewRule();

            var result = sut.IsValidGuid(candidate?.Replace('_', '-').Replace("|", string.Empty));

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(AimTypes.References.IndustryPlacement)]
        [InlineData("w;oraeijwq rf;oiew ")]
        public void InvalidItemRaisesValidationMessage(string candidate)
        {
            const string LearnRefNumber = "123456789X";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.SWSupAimId)
                .Returns(candidate);
            mockDelivery
                .SetupGet(y => y.AimSeqNumber)
                .Returns(0);

            var deliveries = new List<ILearningDelivery>
            {
                mockDelivery.Object
            };

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            mockHandler.Setup(x => x.Handle(
                RuleNameConstants.SWSupAimId_01,
                LearnRefNumber,
                0,
                Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            mockHandler
                .Setup(x => x.BuildErrorMessageParameter(
                    PropertyNameConstants.SWSupAimId,
                    candidate))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var sut = new SWSupAimId_01Rule(mockHandler.Object);

            sut.Validate(mockLearner.Object);

            mockHandler.VerifyAll();
        }

        [Theory]
        [InlineData("550e8400_e29b_41d4_a716_446655440000")]
        [InlineData("|550e8400e29b41d4a716446655440000")]
        public void ValidItemDoesNotRaiseAValidationMessage(string candidate)
        {
            const string LearnRefNumber = "123456789X";
            candidate = candidate?.Replace('_', '-').Replace("|", string.Empty);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.SWSupAimId)
                .Returns(candidate);

            var deliveries = new List<ILearningDelivery>
            {
                mockDelivery.Object
            };

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var sut = new SWSupAimId_01Rule(mockHandler.Object);

            sut.Validate(mockLearner.Object);

            mockHandler.VerifyAll();
        }

        public SWSupAimId_01Rule NewRule()
        {
            var mock = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            return new SWSupAimId_01Rule(mock.Object);
        }
    }
}
