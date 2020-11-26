using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.OrigLearnStartDate;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.OrigLearnStartDate
{
    public class OrigLearnStartDate_04RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("OrigLearnStartDate_04", result);
        }

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
            var sut = NewRule();

            var mockFAM = new Mock<ILearningDeliveryFAM>();
            mockFAM
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(candidate);

            var result = sut.HasRestartIndicator(mockFAM.Object);

            Assert.Equal(expectation, result);
        }

        [Fact]
        public void HasRestartIndicatorWithNullFAMsReturnsFalse()
        {
            var sut = NewRule();

            var mockDelivery = new Mock<ILearningDelivery>();

            var result = sut.HasRestartIndicator(mockDelivery.Object);

            Assert.False(result);
        }

        [Fact]
        public void HasRestartIndicatorWithEmptyFAMsReturnsFalse()
        {
            var sut = NewRule();

            var fams = new List<ILearningDeliveryFAM>();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.LearningDeliveryFAMs)
                .Returns(fams);

            var result = sut.HasRestartIndicator(mockDelivery.Object);

            Assert.False(result);
        }

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
            const string LearnRefNumber = "123456789X";
            var referenceDate = DateTime.Parse("2019-04-19");

            var mockFAM = new Mock<ILearningDeliveryFAM>();
            mockFAM
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(candidate);

            var fams = new List<ILearningDeliveryFAM>
            {
                mockFAM.Object
            };

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

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
        }

        [Theory]
        [InlineData(Monitoring.Delivery.Types.Restart)]
        public void ValidItemDoesNotRaiseValidationMessage(string candidate)
        {
            const string LearnRefNumber = "123456789X";

            var referenceDate = DateTime.Parse("2019-04-19");

            var mockFAM = new Mock<ILearningDeliveryFAM>();
            mockFAM
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(candidate);

            var fams = new List<ILearningDeliveryFAM>
            {
                mockFAM.Object
            };

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

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var sut = new OrigLearnStartDate_04Rule(handler.Object);

            sut.Validate(mockLearner.Object);

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

        public OrigLearnStartDate_04Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            return new OrigLearnStartDate_04Rule(handler.Object);
        }
    }
}
