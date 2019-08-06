using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMDateTo;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMDateTo
{
    public class LearnDelFAMDateTo_03RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnDelFAMDateTo_03Rule(null, commonOps.Object));
        }

        [Fact]
        public void NewRuleWithNullCommonOperationsThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnDelFAMDateTo_03Rule(handler.Object, null));
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
            Assert.Equal("LearnDelFAMDateTo_03", result);
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
            Assert.Equal(RuleNameConstants.LearnDelFAMDateTo_03, result);
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

            // act / assert
            Assert.Throws<ArgumentNullException>(() => sut.Validate(null));
        }

        /// <summary>
        /// Has qualifying funding meets expectation
        /// </summary>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void HasQualifyingFundingMeetsExpectation(bool expectation)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(delivery.Object, 35, 36, 81, 99))
                .Returns(expectation);

            var sut = new LearnDelFAMDateTo_03Rule(handler.Object, commonOps.Object);

            // act
            var result = sut.HasQualifyingFunding(delivery.Object);

            // assert
            Assert.Equal(expectation, result);
            handler.VerifyAll();
            commonOps.VerifyAll();
        }

        /// <summary>
        /// Is qualifying monitor meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("ADL", true)] // Monitoring.Delivery.Types.AdvancedLearnerLoan
        [InlineData("ALB", true)] // Monitoring.Delivery.Types.AdvancedLearnerLoansBursaryFunding
        [InlineData("ACT", false)] // Monitoring.Delivery.Types.ApprenticeshipContract
        [InlineData("ASL", true)] // Monitoring.Delivery.Types.CommunityLearningProvision
        [InlineData("EEF", true)] // Monitoring.Delivery.Types.EligibilityForEnhancedApprenticeshipFunding
        [InlineData("FLN", true)] // Monitoring.Delivery.Types.FamilyEnglishMathsAndLanguage
        [InlineData("FFI", true)] // Monitoring.Delivery.Types.FullOrCoFunding
        [InlineData("HEM", true)] // Monitoring.Delivery.Types.HEMonitoring
        [InlineData("HHS", true)] // Monitoring.Delivery.Types.HouseholdSituation
        [InlineData("LDM", true)] // Monitoring.Delivery.Types.Learning
        [InlineData("LSF", true)] // Monitoring.Delivery.Types.LearningSupportFunding
        [InlineData("NSA", true)] // Monitoring.Delivery.Types.NationalSkillsAcademy
        [InlineData("POD", true)] // Monitoring.Delivery.Types.PercentageOfOnlineDelivery
        [InlineData("RES", true)] // Monitoring.Delivery.Types.Restart
        [InlineData("SOF", true)] // Monitoring.Delivery.Types.SourceOfFunding
        [InlineData("WPP", true)] // Monitoring.Delivery.Types.WorkProgrammeParticipation
        public void IsQualifyingMonitorMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var fam = new Mock<ILearningDeliveryFAM>();
            fam
                .SetupGet(x => x.LearnDelFAMType)
                .Returns(candidate);

            var sut = NewRule();

            // act
            var result = sut.IsQualifyingMonitor(fam.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Has disqualifying dates meets expectation
        /// </summary>
        /// <param name="dateTo">The date to.</param>
        /// <param name="actEnd">The act end.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("2017-12-30", null, false)]
        [InlineData(null, "2017-12-31", false)]
        [InlineData("2017-12-30", "2017-12-31", false)]
        [InlineData("2017-12-31", "2017-12-31", false)]
        [InlineData("2018-01-01", "2017-12-31", true)]
        public void HasDisqualifyingDatesMeetsExpectation(string dateTo, string actEnd, bool expectation)
        {
            // arrange
            var fam = new Mock<ILearningDeliveryFAM>();
            fam
                .SetupGet(x => x.LearnDelFAMDateToNullable)
                .Returns(GetNullableDate(dateTo));

            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.LearnActEndDateNullable)
                .Returns(GetNullableDate(actEnd));

            var sut = NewRule();

            // act
            var result = sut.HasDisqualifyingDates(delivery.Object, fam.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Gets the nullable date.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>a nullable date</returns>
        public DateTime? GetNullableDate(string candidate) =>
            Utility.It.Has(candidate) ? DateTime.Parse(candidate) : (DateTime?)null;

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="famType">Type of FAM.</param>
        /// <param name="dateOffset">The date offset, determines the state of the final condition</param>
        [Theory]
        [InlineData("ADL", 1)] // Monitoring.Delivery.Types.AdvancedLearnerLoan
        [InlineData("ALB", 1)] // Monitoring.Delivery.Types.AdvancedLearnerLoansBursaryFunding
        [InlineData("ASL", 1)] // Monitoring.Delivery.Types.CommunityLearningProvision
        [InlineData("EEF", 1)] // Monitoring.Delivery.Types.EligibilityForEnhancedApprenticeshipFunding
        [InlineData("FLN", 1)] // Monitoring.Delivery.Types.FamilyEnglishMathsAndLanguage
        [InlineData("FFI", 1)] // Monitoring.Delivery.Types.FullOrCoFunding
        [InlineData("HEM", 1)] // Monitoring.Delivery.Types.HEMonitoring
        [InlineData("HHS", 1)] // Monitoring.Delivery.Types.HouseholdSituation
        [InlineData("LDM", 1)] // Monitoring.Delivery.Types.Learning
        [InlineData("LSF", 1)] // Monitoring.Delivery.Types.LearningSupportFunding
        [InlineData("NSA", 1)] // Monitoring.Delivery.Types.NationalSkillsAcademy
        [InlineData("POD", 1)] // Monitoring.Delivery.Types.PercentageOfOnlineDelivery
        [InlineData("RES", 1)] // Monitoring.Delivery.Types.Restart
        [InlineData("SOF", 1)] // Monitoring.Delivery.Types.SourceOfFunding
        [InlineData("WPP", 1)] // Monitoring.Delivery.Types.WorkProgrammeParticipation
        public void InvalidItemRaisesValidationMessage(string famType, int dateOffset)
        {
            // arrange
            const string learnRefNumber = "123456789X";

            var testDate = DateTime.Parse("2017-12-30");

            var fam = new Mock<ILearningDeliveryFAM>();
            fam
                .SetupGet(x => x.LearnDelFAMType)
                .Returns(famType);
            fam
                .SetupGet(x => x.LearnDelFAMDateToNullable)
                .Returns(testDate.AddDays(dateOffset));

            var fams = new ILearningDeliveryFAM[] { fam.Object };

            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.LearnActEndDateNullable)
                .Returns(testDate);
            delivery
                .SetupGet(x => x.FundModel)
                .Returns(35); // TypeOfFunding.AdultSkills
            delivery
                .SetupGet(x => x.LearningDeliveryFAMs)
                .Returns(fams);

            var deliveries = new ILearningDelivery[] { delivery.Object };

            var learner = new Mock<ILearner>();
            learner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRefNumber);
            learner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(RuleNameConstants.LearnDelFAMDateTo_03, learnRefNumber, 0, It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnActEndDate", AbstractRule.AsRequiredCultureDate(testDate)))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnDelFAMDateTo", AbstractRule.AsRequiredCultureDate(testDate.AddDays(dateOffset))))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(delivery.Object, 35, 36, 81, 99))
                .Returns(true);

            var sut = new LearnDelFAMDateTo_03Rule(handler.Object, commonOps.Object);

            // act
            sut.Validate(learner.Object);

            // assert
            handler.VerifyAll();
            commonOps.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        /// <param name="famType">Type of FAM.</param>
        /// <param name="dateOffset">The date offset, determines the state of the final condition</param>
        [Theory]
        [InlineData("ACT", 1)] // Monitoring.Delivery.Types.ApprenticeshipContract
        [InlineData("ADL", 0)] // Monitoring.Delivery.Types.AdvancedLearnerLoan
        [InlineData("ALB", 0)] // Monitoring.Delivery.Types.AdvancedLearnerLoansBursaryFunding
        [InlineData("ASL", 0)] // Monitoring.Delivery.Types.CommunityLearningProvision
        [InlineData("EEF", 0)] // Monitoring.Delivery.Types.EligibilityForEnhancedApprenticeshipFunding
        [InlineData("FLN", 0)] // Monitoring.Delivery.Types.FamilyEnglishMathsAndLanguage
        [InlineData("FFI", 0)] // Monitoring.Delivery.Types.FullOrCoFunding
        [InlineData("HEM", 0)] // Monitoring.Delivery.Types.HEMonitoring
        [InlineData("HHS", 0)] // Monitoring.Delivery.Types.HouseholdSituation
        [InlineData("LDM", 0)] // Monitoring.Delivery.Types.Learning
        [InlineData("LSF", 0)] // Monitoring.Delivery.Types.LearningSupportFunding
        [InlineData("NSA", 0)] // Monitoring.Delivery.Types.NationalSkillsAcademy
        [InlineData("POD", 0)] // Monitoring.Delivery.Types.PercentageOfOnlineDelivery
        [InlineData("RES", 0)] // Monitoring.Delivery.Types.Restart
        [InlineData("SOF", 0)] // Monitoring.Delivery.Types.SourceOfFunding
        [InlineData("WPP", 0)] // Monitoring.Delivery.Types.WorkProgrammeParticipation
        public void ValidItemDoesNotRaiseValidationMessage(string famType, int dateOffset)
        {
            // arrange
            const string learnRefNumber = "123456789X";

            var testDate = DateTime.Parse("2017-12-30");

            var fam = new Mock<ILearningDeliveryFAM>();
            fam
                .SetupGet(x => x.LearnDelFAMType)
                .Returns(famType);
            fam
                .SetupGet(x => x.LearnDelFAMDateToNullable)
                .Returns(testDate.AddDays(dateOffset));

            var fams = new ILearningDeliveryFAM[] { fam.Object };

            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.LearnActEndDateNullable)
                .Returns(testDate);
            delivery
                .SetupGet(x => x.FundModel)
                .Returns(35); // TypeOfFunding.AdultSkills
            delivery
                .SetupGet(x => x.LearningDeliveryFAMs)
                .Returns(fams);

            var deliveries = new ILearningDelivery[] { delivery.Object };

            var learner = new Mock<ILearner>();
            learner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRefNumber);
            learner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(delivery.Object, 35, 36, 81, 99))
                .Returns(true);

            var sut = new LearnDelFAMDateTo_03Rule(handler.Object, commonOps.Object);

            // act
            sut.Validate(learner.Object);

            // assert
            handler.VerifyAll();
            commonOps.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public LearnDelFAMDateTo_03Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            return new LearnDelFAMDateTo_03Rule(handler.Object, commonOps.Object);
        }
    }
}
