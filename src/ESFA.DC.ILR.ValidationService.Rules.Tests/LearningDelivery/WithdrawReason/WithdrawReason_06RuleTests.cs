using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.WithdrawReason;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.WithdrawReason
{
    /// <summary>
    /// withdraw reason 06 tests
    /// </summary>
    public class WithdrawReason_06RuleTests
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
            Assert.Equal("WithdrawReason_06", result);
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
            Assert.Equal(RuleNameConstants.WithdrawReason_06, result);
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
        [InlineData(02, false)] // ReasonForWithdrawal.TransferredToAnotherProvider
        [InlineData(03, false)] // ReasonForWithdrawal.InjuryOrIllness
        [InlineData(07, false)] // ReasonForWithdrawal.TransferredThroughInterventionOrConsent
        [InlineData(28, false)] // ReasonForWithdrawal.OLASSLearnerOutsideProvidersControl
        [InlineData(29, false)] // ReasonForWithdrawal.MadeRedundant
        [InlineData(40, false)] // ReasonForWithdrawal.NewLearningAimWithSameProvider
        [InlineData(41, false)] // ReasonForWithdrawal.TransferredMeetingGovernmentStrategy
        [InlineData(42, false)] // ReasonForWithdrawal.NotAllowedToContinueHEOnly
        [InlineData(43, false)] // ReasonForWithdrawal.Financial
        [InlineData(44, false)] // ReasonForWithdrawal.OtherPersonal
        [InlineData(45, false)] // ReasonForWithdrawal.WrittenOffHEOnly
        [InlineData(46, false)] // ReasonForWithdrawal.Exclusion
        [InlineData(47, false)] // ReasonForWithdrawal.TransferredDueToMerger
        [InlineData(48, true)] // ReasonForWithdrawal.IndustrialPlacementLearnerWithdrew
        [InlineData(97, false)] // ReasonForWithdrawal.Other
        [InlineData(98, false)] // ReasonForWithdrawal.NotKnown
        public void HasWithdrewAsIndustrialPlacementLearnerMeetsExpectation(int? candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.WithdrawReasonNullable)
                .Returns(candidate);

            // act
            var result = sut.HasWithdrewAsIndustrialPlacementLearner(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("ZESF0001", false)] // TypeOfAim.References.ESFLearnerStartandAssessment
        [InlineData("Z0002347", false)] // TypeOfAim.References.SupportedInternship16To19
        [InlineData("Z0007834", false)] // TypeOfAim.References.WorkPlacement0To49Hours
        [InlineData("Z0007835", false)] // TypeOfAim.References.WorkPlacement50To99Hours
        [InlineData("Z0007836", false)] // TypeOfAim.References.WorkPlacement100To199Hours
        [InlineData("Z0007837", false)] // TypeOfAim.References.WorkPlacement200To499Hours
        [InlineData("Z0007838", false)] // TypeOfAim.References.WorkPlacement500PlusHours
        [InlineData("ZWRKX001", false)] // TypeOfAim.References.WorkExperience
        [InlineData("ZWRKX002", true)] // TypeOfAim.References.IndustryPlacement
        public void HasQualifyingAimMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(candidate);

            // act
            var result = sut.HasQualifyingAim(mockDelivery.Object);

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
                .Returns(3); // CompletionState.HasWithdrawn
            mockDelivery
                .SetupGet(x => x.WithdrawReasonNullable)
                .Returns(48); // ReasonForWithdrawal.IndustrialPlacementLearnerWithdrew
            mockDelivery
                .SetupGet(x => x.LearnAimRef)
                .Returns("ZWRKX001"); // TypeOfAim.References.WorkExperience

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
                .Returns(3); // CompletionState.HasWithdrawn
            mockDelivery
                .SetupGet(x => x.WithdrawReasonNullable)
                .Returns(48); // ReasonForWithdrawal.IndustrialPlacementLearnerWithdrew
            mockDelivery
                .SetupGet(x => x.LearnAimRef)
                .Returns("ZWRKX002"); // TypeOfAim.References.IndustryPlacement

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

            // act
            sut.Validate(mockLearner.Object);

            // assert
            mockHandler.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public WithdrawReason_06Rule NewRule()
        {
            var mock = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            return new WithdrawReason_06Rule(mock.Object);
        }
    }
}
