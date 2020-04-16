using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.WithdrawReason;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.WithdrawReason
{
    public class WithdrawReason_06RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("WithdrawReason_06", result);
        }

        [Theory]
        [InlineData(CompletionState.IsOngoing, false)]
        [InlineData(CompletionState.HasCompleted, false)]
        [InlineData(CompletionState.HasTemporarilyWithdrawn, false)]
        [InlineData(CompletionState.HasWithdrawn, true)]
        public void HasWithdrawnMeetsExpectation(int candidate, bool expectation)
        {
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.CompStatus)
                .Returns(candidate);

            var result = sut.HasWithdrawn(mockDelivery.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData(02, false)]
        [InlineData(03, false)]
        [InlineData(07, false)]
        [InlineData(28, false)]
        [InlineData(29, false)]
        [InlineData(40, false)]
        [InlineData(41, false)]
        [InlineData(42, false)]
        [InlineData(43, false)]
        [InlineData(44, false)]
        [InlineData(45, false)]
        [InlineData(46, false)]
        [InlineData(47, false)]
        [InlineData(48, true)]
        [InlineData(97, false)]
        [InlineData(98, false)]
        public void HasWithdrewAsIndustrialPlacementLearnerMeetsExpectation(int? candidate, bool expectation)
        {
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.WithdrawReasonNullable)
                .Returns(candidate);

            var result = sut.HasWithdrewAsIndustrialPlacementLearner(mockDelivery.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("ZESF0001", false)]
        [InlineData("Z0002347", false)]
        [InlineData("Z0007834", false)]
        [InlineData("Z0007835", false)]
        [InlineData("Z0007836", false)]
        [InlineData("Z0007837", false)]
        [InlineData("Z0007838", false)]
        [InlineData("ZWRKX001", false)]
        [InlineData("ZWRKX002", true)]
        public void HasQualifyingAimMeetsExpectation(string candidate, bool expectation)
        {
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(candidate);

            var result = sut.HasQualifyingAim(mockDelivery.Object);

            Assert.Equal(expectation, result);
        }

        [Fact]
        public void InvalidItemRaisesValidationMessage()
        {
            const string LearnRefNumber = "123456789X";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.CompStatus)
                .Returns(3);
            mockDelivery
                .SetupGet(x => x.WithdrawReasonNullable)
                .Returns(48);
            mockDelivery
                .SetupGet(x => x.LearnAimRef)
                .Returns("ZWRKX001");

            var deliveries = new ILearningDelivery[]
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
            mockHandler
               .Setup(x => x.Handle(RuleNameConstants.WithdrawReason_06, LearnRefNumber, 0, It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            mockHandler
                .Setup(x => x.BuildErrorMessageParameter("LearnAimRef", "ZWRKX001"))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            mockHandler
                .Setup(x => x.BuildErrorMessageParameter("WithdrawReason", 48))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var sut = new WithdrawReason_06Rule(mockHandler.Object);

            sut.Validate(mockLearner.Object);

            mockHandler.VerifyAll();
        }

        [Fact]
        public void ValidItemDoesNotRaiseAValidationMessage()
        {
            const string LearnRefNumber = "123456789X";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.CompStatus)
                .Returns(3);
            mockDelivery
                .SetupGet(x => x.WithdrawReasonNullable)
                .Returns(48);
            mockDelivery
                .SetupGet(x => x.LearnAimRef)
                .Returns("ZWRKX002");

            var deliveries = new ILearningDelivery[]
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

            var sut = new WithdrawReason_06Rule(mockHandler.Object);

            sut.Validate(mockLearner.Object);

            mockHandler.VerifyAll();
        }

        public WithdrawReason_06Rule NewRule()
        {
            var mock = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            return new WithdrawReason_06Rule(mock.Object);
        }
    }
}
