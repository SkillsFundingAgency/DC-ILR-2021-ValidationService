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
    public class LearnDelFAMDateTo_05RuleTests
    {
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnDelFAMDateTo_05Rule(null, commonOps.Object));
        }

        [Fact]
        public void NewRuleWithNullCommonOperationsThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnDelFAMDateTo_05Rule(handler.Object, null));
        }

        [Fact]
        public void RuleName1()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.RuleName;

            // assert
            Assert.Equal("LearnDelFAMDateTo_05", result);
        }

        [Fact]
        public void ValidateWithNullLearnerThrows()
        {
            // arrange
            var sut = NewRule();

            // act / assert
            Assert.Throws<ArgumentNullException>(() => sut.Validate(null));
        }

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
                .Setup(x => x.HasQualifyingFunding(delivery.Object, 36)) // TypeOfFunding.ApprenticeshipsFrom1May2017
                .Returns(expectation);

            var sut = new LearnDelFAMDateTo_05Rule(handler.Object, commonOps.Object);

            // act
            var result = sut.HasQualifyingFunding(delivery.Object);

            // assert
            Assert.Equal(expectation, result);
            handler.VerifyAll();
            commonOps.VerifyAll();
        }

        [Theory]
        [InlineData("ADL", false)] // Monitoring.Delivery.Types.AdvancedLearnerLoan
        [InlineData("ALB", false)] // Monitoring.Delivery.Types.AdvancedLearnerLoansBursaryFunding
        [InlineData("ACT", true)] // Monitoring.Delivery.Types.ApprenticeshipContract
        [InlineData("ASL", false)] // Monitoring.Delivery.Types.CommunityLearningProvision
        [InlineData("EEF", false)] // Monitoring.Delivery.Types.EligibilityForEnhancedApprenticeshipFunding
        [InlineData("FLN", false)] // Monitoring.Delivery.Types.FamilyEnglishMathsAndLanguage
        [InlineData("FFI", false)] // Monitoring.Delivery.Types.FullOrCoFunding
        [InlineData("HEM", false)] // Monitoring.Delivery.Types.HEMonitoring
        [InlineData("HHS", false)] // Monitoring.Delivery.Types.HouseholdSituation
        [InlineData("LDM", false)] // Monitoring.Delivery.Types.Learning
        [InlineData("LSF", false)] // Monitoring.Delivery.Types.LearningSupportFunding
        [InlineData("NSA", false)] // Monitoring.Delivery.Types.NationalSkillsAcademy
        [InlineData("POD", false)] // Monitoring.Delivery.Types.PercentageOfOnlineDelivery
        [InlineData("RES", false)] // Monitoring.Delivery.Types.Restart
        [InlineData("SOF", false)] // Monitoring.Delivery.Types.SourceOfFunding
        [InlineData("WPP", false)] // Monitoring.Delivery.Types.WorkProgrammeParticipation
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
                .SetupGet(x => x.AchDateNullable)
                .Returns(GetNullableDate(actEnd));

            var sut = NewRule();

            // act
            var result = sut.HasDisqualifyingDates(delivery.Object, fam.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        public DateTime? GetNullableDate(string candidate) =>
            DateTime.TryParse(candidate, out var result) ? result : (DateTime?)null;

        [Theory]
        [InlineData("ACT", 1)] // Monitoring.Delivery.Types.ApprenticeshipContract
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
                .SetupGet(x => x.AchDateNullable)
                .Returns(testDate);
            delivery
                .SetupGet(x => x.FundModel)
                .Returns(36); // TypeOfFunding.ApprenticeshipsFrom1May2017
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
                .Setup(x => x.Handle(RuleNameConstants.LearnDelFAMDateTo_05, learnRefNumber, 0, It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("AchDate", AbstractRule.AsRequiredCultureDate(testDate)))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnDelFAMDateTo", AbstractRule.AsRequiredCultureDate(testDate.AddDays(dateOffset))))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(delivery.Object, 36)) // TypeOfFunding.ApprenticeshipsFrom1May2017
                .Returns(true);

            var sut = new LearnDelFAMDateTo_05Rule(handler.Object, commonOps.Object);

            // act
            sut.Validate(learner.Object);

            // assert
            handler.VerifyAll();
            commonOps.VerifyAll();
        }

        [Theory]
        [InlineData("ACT", 0)] // Monitoring.Delivery.Types.ApprenticeshipContract
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
                .SetupGet(x => x.AchDateNullable)
                .Returns(testDate);
            delivery
                .SetupGet(x => x.FundModel)
                .Returns(36); // TypeOfFunding.ApprenticeshipsFrom1May2017
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
                .Setup(x => x.HasQualifyingFunding(delivery.Object, 36)) // TypeOfFunding.ApprenticeshipsFrom1May2017
                .Returns(true);

            var sut = new LearnDelFAMDateTo_05Rule(handler.Object, commonOps.Object);

            // act
            sut.Validate(learner.Object);

            // assert
            handler.VerifyAll();
            commonOps.VerifyAll();
        }

        public LearnDelFAMDateTo_05Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            return new LearnDelFAMDateTo_05Rule(handler.Object, commonOps.Object);
        }
    }
}
