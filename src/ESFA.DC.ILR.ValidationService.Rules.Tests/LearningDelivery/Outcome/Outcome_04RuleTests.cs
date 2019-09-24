using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.Outcome;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.Outcome
{
    /// <summary>
    /// outcome 04 (last altered for 1920)
    /// </summary>
    public class Outcome_04RuleTests
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
            Assert.Equal("Outcome_04", result);
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
            Assert.Equal(RuleNameConstants.Outcome_04, result);
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

        /// <summary>
        /// has achievement date meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(null, false)]
        [InlineData("2018-04-23", true)]
        public void HasAchievementDateMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var testdate = string.IsNullOrWhiteSpace(candidate)
                ? (DateTime?)null
                : DateTime.Parse(candidate);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.AchDateNullable)
                .Returns(testdate);

            // act
            var result = sut.HasAchievementDate(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// has qualifying outcome meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(null, false)]
        [InlineData(01, true)] // OutcomeConstants.Achieved
        [InlineData(08, false)] // OutcomeConstants.LearningActivitiesCompleteButOutcomeNotKnown
        [InlineData(03, false)] // OutcomeConstants.NoAchievement
        [InlineData(02, false)] // OutcomeConstants.PartialAchievement
        [InlineData(45, false)] // a random value
        public void HasQualifyingOutcomeMeetsExpectation(int? candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.OutcomeNullable)
                .Returns(candidate);

            // act
            var result = sut.HasQualifyingOutcome(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// is excluded (by fund model and programme type), meets expectation
        /// </summary>
        [Fact]
        public void IsExcludedByFundModelAndProgTypeMeetsExpectation()
        {
            // arrange
            var mockDelivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            common
                .Setup(x => x.IsStandardApprenticeship(mockDelivery.Object))
                .Returns(true);
            common
                .Setup(x => x.HasQualifyingFunding(mockDelivery.Object, TypeOfFunding.ApprenticeshipsFrom1May2017))
                .Returns(true);

            var sut = new Outcome_04Rule(handler.Object, common.Object);

            // act
            var result = sut.IsExcluded(mockDelivery.Object);

            // assert
            handler.VerifyAll();
            common.VerifyAll();

            Assert.True(result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData(null)]
        [InlineData(08)] // OutcomeConstants.LearningActivitiesCompleteButOutcomeNotKnown
        [InlineData(03)] // OutcomeConstants.NoAchievement
        [InlineData(02)] // OutcomeConstants.PartialAchievement
        [InlineData(45)] // a random value
        public void InvalidItemRaisesValidationMessage(int? candidate)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.AchDateNullable)
                .Returns(DateTime.Today);
            mockDelivery
                .SetupGet(x => x.OutcomeNullable)
                .Returns(candidate);

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

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
               .Setup(x => x.Handle(RuleNameConstants.Outcome_04, LearnRefNumber, 0, It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("AchDate", AbstractRule.AsRequiredCultureDate(DateTime.Today)))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("Outcome", candidate))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            common
                .Setup(x => x.IsStandardApprenticeship(mockDelivery.Object))
                .Returns(false);

            var sut = new Outcome_04Rule(handler.Object, common.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            common.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise a validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData(01)] // OutcomeConstants.Achieved
        public void ValidItemDoesNotRaiseAValidationMessage(int? candidate)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.AchDateNullable)
                .Returns(DateTime.Today);
            mockDelivery
                .SetupGet(x => x.OutcomeNullable)
                .Returns(candidate);

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

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            common
                .Setup(x => x.IsStandardApprenticeship(mockDelivery.Object))
                .Returns(false);

            var sut = new Outcome_04Rule(handler.Object, common.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            common.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public Outcome_04Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            return new Outcome_04Rule(handler.Object, common.Object);
        }
    }
}
