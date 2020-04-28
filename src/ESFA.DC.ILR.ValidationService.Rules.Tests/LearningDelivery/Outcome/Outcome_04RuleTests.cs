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
    public class Outcome_04RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("Outcome_04", result);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("2018-04-23", true)]
        public void HasAchievementDateMeetsExpectation(string candidate, bool expectation)
        {
            var sut = NewRule();
            var testdate = string.IsNullOrWhiteSpace(candidate)
                ? (DateTime?)null
                : DateTime.Parse(candidate);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.AchDateNullable)
                .Returns(testdate);

            var result = sut.HasAchievementDate(mockDelivery.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData(01, true)]
        [InlineData(08, false)]
        [InlineData(03, false)]
        [InlineData(02, false)]
        [InlineData(45, false)]
        public void HasQualifyingOutcomeMeetsExpectation(int? candidate, bool expectation)
        {
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.OutcomeNullable)
                .Returns(candidate);

            var result = sut.HasQualifyingOutcome(mockDelivery.Object);

            Assert.Equal(expectation, result);
        }

        [Fact]
        public void IsExcludedByFundModelAndProgTypeMeetsExpectation()
        {
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
               .SetupGet(x => x.ProgTypeNullable)
               .Returns(25);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            common
                .Setup(x => x.HasQualifyingFunding(mockDelivery.Object, TypeOfFunding.ApprenticeshipsFrom1May2017))
                .Returns(true);

            var sut = new Outcome_04Rule(handler.Object, common.Object);

            var result = sut.IsExcluded(mockDelivery.Object);

            handler.VerifyAll();
            common.VerifyAll();

            Assert.True(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(08)]
        [InlineData(03)]
        [InlineData(02)]
        [InlineData(45)]
        public void InvalidItemRaisesValidationMessage(int? candidate)
        {
            const string LearnRefNumber = "123456789X";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.AchDateNullable)
                .Returns(DateTime.Today);
            mockDelivery
                .SetupGet(x => x.OutcomeNullable)
                .Returns(candidate);
            mockDelivery
               .SetupGet(x => x.ProgTypeNullable)
               .Returns(24);

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

            var sut = new Outcome_04Rule(handler.Object, common.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            common.VerifyAll();
        }

        [Theory]
        [InlineData(01)]
        public void ValidItemDoesNotRaiseAValidationMessage(int? candidate)
        {
            const string LearnRefNumber = "123456789X";

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.AchDateNullable)
                .Returns(DateTime.Today);
            mockDelivery
                .SetupGet(x => x.OutcomeNullable)
                .Returns(candidate);
            mockDelivery
               .SetupGet(x => x.ProgTypeNullable)
               .Returns(24);

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

            var sut = new Outcome_04Rule(handler.Object, common.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            common.VerifyAll();
        }

        public Outcome_04Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            return new Outcome_04Rule(handler.Object, common.Object);
        }
    }
}
