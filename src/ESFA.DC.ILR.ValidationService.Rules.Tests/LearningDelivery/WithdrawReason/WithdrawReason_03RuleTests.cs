using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.WithdrawReason;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.WithdrawReason
{
    /// <summary>
    /// from version 1.1 validation spread sheet
    /// </summary>
    public class WithdrawReason_03RuleTests
    {
        /// <summary>
        /// Rule name 1, matches a literal.
        /// </summary>
        [Fact]
        public void RuleName1()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.RuleName;

            // assert
            Assert.Equal("WithdrawReason_03", result);
        }

        /// <summary>
        /// Rule name 2, matches the constant.
        /// </summary>
        [Fact]
        public void RuleName2()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.RuleName;

            // assert
            Assert.Equal(RuleNameConstants.WithdrawReason_03, result);
        }

        /// <summary>
        /// Rule name 3 test, account for potential false positives.
        /// </summary>
        [Fact]
        public void RuleName3()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.RuleName;

            // assert
            Assert.NotEqual("SomeOtherRuleName_07", result);
        }

        /// <summary>
        /// Validate with null learner throws.
        /// </summary>
        [Fact]
        public void ValidateWithNullLearnerThrows()
        {
            // arrange
            var sut = NewRule();

            // act/assert
            Assert.Throws<ArgumentNullException>(() => sut.Validate(null));
        }

        [Theory]
        [InlineData(CompletionState.IsOngoing, false)]
        [InlineData(CompletionState.HasCompleted, false)]
        [InlineData(CompletionState.HasTemporarilyWithdrawn, false)]
        [InlineData(CompletionState.HasWithdrawn, true)]
        public void HasWithdrawnMeetsExpectation(int candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.CompStatus)
                .Returns(candidate);

            // act
            var result = sut.HasWithdrawn(mockDelivery.Object);

            // assert
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
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.WithdrawReasonNullable)
                .Returns(candidate);

            // act
            var result = sut.HasWithdrawReason(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        [Fact]
        public void InvalidItemRaisesValidationMessage()
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.CompStatus)
                .Returns(3);
            mockDelivery
                .SetupGet(x => x.AimSeqNumber)
                .Returns(0);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

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

            // act
            sut.Validate(mockLearner.Object);

            // assert
            mockHandler.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise a validation message.
        /// </summary>
        [Fact]
        public void ValidItemDoesNotRaiseAValidationMessage()
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.CompStatus)
                .Returns(CompletionState.HasWithdrawn);
            mockDelivery
                .SetupGet(x => x.WithdrawReasonNullable)
                .Returns(ReasonForWithdrawal.TransferredDueToMerger);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var sut = new WithdrawReason_03Rule(mockHandler.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            mockHandler.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public WithdrawReason_03Rule NewRule()
        {
            var mock = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            return new WithdrawReason_03Rule(mock.Object);
        }
    }
}
