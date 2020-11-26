using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.WithdrawReason;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.WithdrawReason
{
    public class WithdrawReason_03RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("WithdrawReason_03", result);
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
        [InlineData(ReasonForWithdrawal.Financial, true)]
        [InlineData(ReasonForWithdrawal.Exclusion, true)]
        [InlineData(ReasonForWithdrawal.InjuryOrIllness, true)]
        [InlineData(ReasonForWithdrawal.MadeRedundant, true)]
        [InlineData(ReasonForWithdrawal.NewLearningAimWithSameProvider, true)]
        [InlineData(ReasonForWithdrawal.NotAllowedToContinueHEOnly, true)]
        [InlineData(ReasonForWithdrawal.NotKnown, true)]
        [InlineData(ReasonForWithdrawal.OLASSLearnerOutsideProvidersControl, true)]
        [InlineData(ReasonForWithdrawal.Other, true)]
        [InlineData(ReasonForWithdrawal.OtherPersonal, true)]
        [InlineData(ReasonForWithdrawal.TransferredDueToMerger, true)]
        [InlineData(ReasonForWithdrawal.TransferredMeetingGovernmentStrategy, true)]
        [InlineData(ReasonForWithdrawal.TransferredThroughInterventionOrConsent, true)]
        [InlineData(ReasonForWithdrawal.TransferredToAnotherProvider, true)]
        [InlineData(ReasonForWithdrawal.WrittenOffHEOnly, true)]
        public void HasWithdrawReasonMeetsExpectation(int? candidate, bool expectation)
        {
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.WithdrawReasonNullable)
                .Returns(candidate);

            var result = sut.HasWithdrawReason(mockDelivery.Object);

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
                .SetupGet(x => x.AimSeqNumber)
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
            mockHandler
               .Setup(x => x.Handle(RuleNameConstants.WithdrawReason_03, LearnRefNumber, 0, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            mockHandler
                .Setup(x => x.BuildErrorMessageParameter("CompStatus", "3"))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var sut = new WithdrawReason_03Rule(mockHandler.Object);

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
                .Returns(CompletionState.HasWithdrawn);
            mockDelivery
                .SetupGet(x => x.WithdrawReasonNullable)
                .Returns(ReasonForWithdrawal.TransferredDueToMerger);

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

            var sut = new WithdrawReason_03Rule(mockHandler.Object);

            sut.Validate(mockLearner.Object);

            mockHandler.VerifyAll();
        }

        public WithdrawReason_03Rule NewRule()
        {
            var mock = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            return new WithdrawReason_03Rule(mock.Object);
        }
    }
}
