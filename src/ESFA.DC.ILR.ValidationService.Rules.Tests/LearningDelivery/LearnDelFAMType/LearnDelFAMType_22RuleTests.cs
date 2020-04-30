using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_22RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("LearnDelFAMType_22", result);
        }

        [Theory]
        [InlineData(Monitoring.Delivery.Types.AdvancedLearnerLoan, false)]
        [InlineData(Monitoring.Delivery.Types.AdvancedLearnerLoansBursaryFunding, false)]
        [InlineData(Monitoring.Delivery.Types.ApprenticeshipContract, false)]
        [InlineData(Monitoring.Delivery.Types.CommunityLearningProvision, false)]
        [InlineData(Monitoring.Delivery.Types.EligibilityForEnhancedApprenticeshipFunding, false)]
        [InlineData(Monitoring.Delivery.Types.FamilyEnglishMathsAndLanguage, false)]
        [InlineData(Monitoring.Delivery.Types.FullOrCoFunding, true)]
        [InlineData(Monitoring.Delivery.Types.HEMonitoring, false)]
        [InlineData(Monitoring.Delivery.Types.HouseholdSituation, false)]
        [InlineData(Monitoring.Delivery.Types.Learning, false)]
        [InlineData(Monitoring.Delivery.Types.LearningSupportFunding, false)]
        [InlineData(Monitoring.Delivery.Types.NationalSkillsAcademy, false)]
        [InlineData(Monitoring.Delivery.Types.PercentageOfOnlineDelivery, false)]
        [InlineData(Monitoring.Delivery.Types.Restart, false)]
        [InlineData(Monitoring.Delivery.Types.SourceOfFunding, false)]
        [InlineData(Monitoring.Delivery.Types.WorkProgrammeParticipation, false)]
        public void HasFullOrCoFundingIndicatorMeetsExpectation(string candidate, bool expectation)
        {
            var sut = NewRule();

            var mockFAM = new Mock<ILearningDeliveryFAM>();
            mockFAM
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(candidate);

            var result = sut.HasFullOrCoFundingIndicator(mockFAM.Object);

            Assert.Equal(expectation, result);
        }

        [Fact]
        public void HasFullOrCoFundingIndicatorWithNullFAMsReturnsFalse()
        {
            var sut = NewRule();

            var mockDelivery = new Mock<ILearningDelivery>();

            var result = sut.HasFullOrCoFundingIndicator(mockDelivery.Object);

            Assert.False(result);
        }

        [Fact]
        public void HasFullOrCoFundingIndicatorWithEmptyFAMsReturnsFalse()
        {
            var sut = NewRule();

            var fams = new List<ILearningDeliveryFAM>();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.LearningDeliveryFAMs)
                .Returns(fams);

            var result = sut.HasFullOrCoFundingIndicator(mockDelivery.Object);

            Assert.False(result);
        }

        [Theory]
        [InlineData(FundModels.AdultSkills, true)]
        [InlineData(FundModels.Age16To19ExcludingApprenticeships, false)]
        [InlineData(FundModels.ApprenticeshipsFrom1May2017, false)]
        [InlineData(FundModels.CommunityLearning, false)]
        [InlineData(FundModels.EuropeanSocialFund, false)]
        [InlineData(FundModels.NotFundedByESFA, false)]
        [InlineData(FundModels.Other16To19, false)]
        [InlineData(FundModels.OtherAdult, true)]
        public void IsQualifyingFundModelMeetsExpectation(int candidate, bool expectation)
        {
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(candidate);

            var result = sut.IsQualifyingFundModel(mockDelivery.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(FundModels.Age16To19ExcludingApprenticeships)]
        [InlineData(FundModels.ApprenticeshipsFrom1May2017)]
        [InlineData(FundModels.CommunityLearning)]
        [InlineData(FundModels.EuropeanSocialFund)]
        [InlineData(FundModels.NotFundedByESFA)]
        [InlineData(FundModels.Other16To19)]
        public void InvalidItemRaisesValidationMessage(int candidate)
        {
            const string LearnRefNumber = "123456789X";

            var mockFAM = new Mock<ILearningDeliveryFAM>();
            mockFAM
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(Monitoring.Delivery.Types.FullOrCoFunding);

            var fams = new List<ILearningDeliveryFAM>();
            fams.Add(mockFAM.Object);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(candidate);
            mockDelivery
                .SetupGet(x => x.LearningDeliveryFAMs)
                .Returns(fams);

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
                    Moq.It.Is<string>(y => y == LearnDelFAMType_22Rule.Name),
                    Moq.It.Is<string>(y => y == LearnRefNumber),
                    0,
                    Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == PropertyNameConstants.FundModel),
                    candidate))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == PropertyNameConstants.LearnDelFAMType),
                    Monitoring.Delivery.Types.FullOrCoFunding))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var sut = new LearnDelFAMType_22Rule(handler.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
        }

        [Theory]
        [InlineData(FundModels.AdultSkills)]
        [InlineData(FundModels.OtherAdult)]
        public void ValidItemDoesNotRaiseValidationMessage(int candidate)
        {
            const string LearnRefNumber = "123456789X";

            var mockFAM = new Mock<ILearningDeliveryFAM>();
            mockFAM
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(Monitoring.Delivery.Types.FullOrCoFunding);

            var fams = new List<ILearningDeliveryFAM>();
            fams.Add(mockFAM.Object);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(candidate);
            mockDelivery
                .SetupGet(x => x.LearningDeliveryFAMs)
                .Returns(fams);

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

            var sut = new LearnDelFAMType_22Rule(handler.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
        }

        public LearnDelFAMType_22Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            return new LearnDelFAMType_22Rule(handler.Object);
        }
    }
}
