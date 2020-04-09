using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.OrigLearnStartDate;
using ESFA.DC.ILR.ValidationService.Utility;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.OrigLearnStartDate
{
    public class OrigLearnStartDate_04RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange / act / assert
            Assert.Throws<ArgumentNullException>(() => new OrigLearnStartDate_04Rule(null));
        }

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
            Assert.Equal("OrigLearnStartDate_04", result);
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
            Assert.Equal(OrigLearnStartDate_04Rule.Name, result);
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
        /// Has restart indicator meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(Monitoring.Delivery.Types.AdvancedLearnerLoan, false)]
        [InlineData(Monitoring.Delivery.Types.AdvancedLearnerLoansBursaryFunding, false)]
        [InlineData(Monitoring.Delivery.Types.ApprenticeshipContract, false)]
        [InlineData(Monitoring.Delivery.Types.CommunityLearningProvision, false)]
        [InlineData(Monitoring.Delivery.Types.EligibilityForEnhancedApprenticeshipFunding, false)]
        [InlineData(Monitoring.Delivery.Types.FamilyEnglishMathsAndLanguage, false)]
        [InlineData(Monitoring.Delivery.Types.FullOrCoFunding, false)]
        [InlineData(Monitoring.Delivery.Types.HEMonitoring, false)]
        [InlineData(Monitoring.Delivery.Types.HouseholdSituation, false)]
        [InlineData(Monitoring.Delivery.Types.Learning, false)]
        [InlineData(Monitoring.Delivery.Types.LearningSupportFunding, false)]
        [InlineData(Monitoring.Delivery.Types.NationalSkillsAcademy, false)]
        [InlineData(Monitoring.Delivery.Types.PercentageOfOnlineDelivery, false)]
        [InlineData(Monitoring.Delivery.Types.Restart, true)]
        [InlineData(Monitoring.Delivery.Types.SourceOfFunding, false)]
        [InlineData(Monitoring.Delivery.Types.WorkProgrammeParticipation, false)]
        public void HasRestartIndicatorMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();

            var mockFAM = new Mock<ILearningDeliveryFAM>();
            mockFAM
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(candidate);

            // act
            var result = sut.HasRestartIndicator(mockFAM.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Has restart indicator with null fams returns false
        /// </summary>
        [Fact]
        public void HasRestartIndicatorWithNullFAMsReturnsFalse()
        {
            // arrange
            var sut = NewRule();

            var mockDelivery = new Mock<ILearningDelivery>();

            // act
            var result = sut.HasRestartIndicator(mockDelivery.Object);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Has restart indicator with empty fams returns false
        /// </summary>
        [Fact]
        public void HasRestartIndicatorWithEmptyFAMsReturnsFalse()
        {
            // arrange
            var sut = NewRule();

            var fams = new List<ILearningDeliveryFAM>();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.LearningDeliveryFAMs)
                .Returns(fams);

            // act
            var result = sut.HasRestartIndicator(mockDelivery.Object);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData(Monitoring.Delivery.Types.AdvancedLearnerLoan)]
        [InlineData(Monitoring.Delivery.Types.AdvancedLearnerLoansBursaryFunding)]
        [InlineData(Monitoring.Delivery.Types.ApprenticeshipContract)]
        [InlineData(Monitoring.Delivery.Types.CommunityLearningProvision)]
        [InlineData(Monitoring.Delivery.Types.EligibilityForEnhancedApprenticeshipFunding)]
        [InlineData(Monitoring.Delivery.Types.FamilyEnglishMathsAndLanguage)]
        [InlineData(Monitoring.Delivery.Types.FullOrCoFunding)]
        [InlineData(Monitoring.Delivery.Types.HEMonitoring)]
        [InlineData(Monitoring.Delivery.Types.HouseholdSituation)]
        [InlineData(Monitoring.Delivery.Types.Learning)]
        [InlineData(Monitoring.Delivery.Types.LearningSupportFunding)]
        [InlineData(Monitoring.Delivery.Types.NationalSkillsAcademy)]
        [InlineData(Monitoring.Delivery.Types.PercentageOfOnlineDelivery)]
        [InlineData(Monitoring.Delivery.Types.SourceOfFunding)]
        [InlineData(Monitoring.Delivery.Types.WorkProgrammeParticipation)]
        public void InvalidItemRaisesValidationMessage(string candidate)
        {
            // arrange
            const string LearnRefNumber = "123456789X";
            var referenceDate = DateTime.Parse("2019-04-19");

            var mockFAM = new Mock<ILearningDeliveryFAM>();
            mockFAM
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(candidate);

            var fams = new List<ILearningDeliveryFAM>();
            fams.Add(mockFAM.Object);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.OrigLearnStartDateNullable)
                .Returns(referenceDate);
            mockDelivery
                .SetupGet(x => x.LearningDeliveryFAMs)
                .Returns(fams);

            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(35);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(
                    Moq.It.Is<string>(y => y == OrigLearnStartDate_04Rule.Name),
                    Moq.It.Is<string>(y => y == LearnRefNumber),
                    0,
                    Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == OrigLearnStartDate_04Rule.MessagePropertyName),
                    referenceDate))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var sut = new OrigLearnStartDate_04Rule(handler.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData(Monitoring.Delivery.Types.Restart)]
        public void ValidItemDoesNotRaiseValidationMessage(string candidate)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var referenceDate = DateTime.Parse("2019-04-19");

            var mockFAM = new Mock<ILearningDeliveryFAM>();
            mockFAM
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(candidate);

            var fams = new List<ILearningDeliveryFAM>();
            fams.Add(mockFAM.Object);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.OrigLearnStartDateNullable)
                .Returns(referenceDate);
            mockDelivery
                .SetupGet(x => x.LearningDeliveryFAMs)
                .Returns(fams);

            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(35);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var sut = new OrigLearnStartDate_04Rule(handler.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
        }

        [Theory]
        [InlineData(35)]
        [InlineData(36)]
        [InlineData(81)]
        [InlineData(99)]
        public void FundModelConditionMet_True(int fundModel)
        {
            var learningDelivery = new TestLearningDelivery
            {
                FundModel = fundModel
            };

            NewRule().HasValidFundModel(learningDelivery).Should().BeTrue();
        }

        [Theory]
        [InlineData(25)]
        [InlineData(88)]
        [InlineData(10)]
        [InlineData(100)]
        public void FundModelConditionMet_False(int fundModel)
        {
            var learningDelivery = new TestLearningDelivery
            {
                FundModel = fundModel
            };

            NewRule().HasValidFundModel(learningDelivery).Should().BeFalse();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public OrigLearnStartDate_04Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            return new OrigLearnStartDate_04Rule(handler.Object);
        }
    }
}
