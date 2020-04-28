using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Query
{
    public class RuleCommonOperationsProviderTests
    {
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
        public void IsRestartMeetsExpectation(string candidate, bool expectation)
        {
            var sut = NewService();
            var mockDelivery = new Mock<ILearningDeliveryFAM>();
            mockDelivery
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(candidate);

            var result = sut.IsRestart(mockDelivery.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(Monitoring.Delivery.OLASSOffendersInCustody, true)]
        [InlineData(Monitoring.Delivery.FullyFundedLearningAim, false)]
        [InlineData(Monitoring.Delivery.CoFundedLearningAim, false)]
        [InlineData(Monitoring.Delivery.InReceiptOfLowWages, false)]
        [InlineData(Monitoring.Delivery.MandationToSkillsTraining, false)]
        [InlineData(Monitoring.Delivery.ReleasedOnTemporaryLicence, false)]
        [InlineData(Monitoring.Delivery.SteelIndustriesRedundancyTraining, false)]
        [InlineData(Monitoring.Delivery.ESFA16To19Funding, false)]
        [InlineData(Monitoring.Delivery.ESFAAdultFunding, false)]
        [InlineData(Monitoring.Delivery.HigherEducationFundingCouncilEngland, false)]
        [InlineData(Monitoring.Delivery.LocalAuthorityCommunityLearningFunds, false)]
        [InlineData(Monitoring.Delivery.FinancedByAdvancedLearnerLoans, false)]
        public void IsLearnerInCustodyMeetsExpectation(string candidate, bool expectation)
        {
            var sut = NewService();
            var mockItem = new Mock<ILearningDeliveryFAM>();
            mockItem
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(candidate.Substring(0, 3));
            mockItem
                .SetupGet(y => y.LearnDelFAMCode)
                .Returns(candidate.Substring(3));

            var result = sut.IsLearnerInCustody(mockItem.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(Monitoring.Delivery.OLASSOffendersInCustody, false)]
        [InlineData(Monitoring.Delivery.FullyFundedLearningAim, false)]
        [InlineData(Monitoring.Delivery.CoFundedLearningAim, false)]
        [InlineData(Monitoring.Delivery.InReceiptOfLowWages, false)]
        [InlineData(Monitoring.Delivery.MandationToSkillsTraining, false)]
        [InlineData(Monitoring.Delivery.ReleasedOnTemporaryLicence, false)]
        [InlineData(Monitoring.Delivery.SteelIndustriesRedundancyTraining, true)]
        [InlineData(Monitoring.Delivery.ESFA16To19Funding, false)]
        [InlineData(Monitoring.Delivery.ESFAAdultFunding, false)]
        [InlineData(Monitoring.Delivery.HigherEducationFundingCouncilEngland, false)]
        [InlineData(Monitoring.Delivery.LocalAuthorityCommunityLearningFunds, false)]
        [InlineData(Monitoring.Delivery.FinancedByAdvancedLearnerLoans, false)]
        public void IsSteelWorkerRedundancyTrainingMeetsExpectation(string candidate, bool expectation)
        {
            var sut = NewService();
            var mockItem = new Mock<ILearningDeliveryFAM>();
            mockItem
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(candidate.Substring(0, 3));
            mockItem
                .SetupGet(y => y.LearnDelFAMCode)
                .Returns(candidate.Substring(3));

            var result = sut.IsSteelWorkerRedundancyTraining(mockItem.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(Monitoring.Delivery.OLASSOffendersInCustody, false)]
        [InlineData(Monitoring.Delivery.FullyFundedLearningAim, false)]
        [InlineData(Monitoring.Delivery.CoFundedLearningAim, false)]
        [InlineData(Monitoring.Delivery.InReceiptOfLowWages, false)]
        [InlineData(Monitoring.Delivery.MandationToSkillsTraining, false)]
        [InlineData(Monitoring.Delivery.ReleasedOnTemporaryLicence, true)]
        [InlineData(Monitoring.Delivery.SteelIndustriesRedundancyTraining, false)]
        [InlineData(Monitoring.Delivery.ESFA16To19Funding, false)]
        [InlineData(Monitoring.Delivery.ESFAAdultFunding, false)]
        [InlineData(Monitoring.Delivery.HigherEducationFundingCouncilEngland, false)]
        [InlineData(Monitoring.Delivery.LocalAuthorityCommunityLearningFunds, false)]
        [InlineData(Monitoring.Delivery.FinancedByAdvancedLearnerLoans, false)]
        public void IsReleasedOnTemporaryLicenceMeetsExpectation(string candidate, bool expectation)
        {
            var sut = NewService();
            var mockItem = new Mock<ILearningDeliveryFAM>();
            mockItem
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(candidate.Substring(0, 3));
            mockItem
                .SetupGet(y => y.LearnDelFAMCode)
                .Returns(candidate.Substring(3));

            var result = sut.IsReleasedOnTemporaryLicence(mockItem.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(TypeOfFunding.AdultSkills, TypeOfFunding.AdultSkills, true)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfFunding.AdultSkills, false)]
        [InlineData(TypeOfFunding.CommunityLearning, TypeOfFunding.AdultSkills, false)]
        [InlineData(TypeOfFunding.Other16To19, TypeOfFunding.Other16To19, true)]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, TypeOfFunding.Other16To19, false)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, TypeOfFunding.EuropeanSocialFund, true)]
        [InlineData(TypeOfFunding.NotFundedByESFA, TypeOfFunding.EuropeanSocialFund, false)]
        [InlineData(TypeOfFunding.OtherAdult, TypeOfFunding.OtherAdult, true)]
        public void HasQualifyingFundingMeetsExpectation(int funding, int candidate, bool expectation)
        {
            var sut = NewService();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(funding);

            var result = sut.HasQualifyingFunding(mockDelivery.Object, candidate);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData("2016-02-28", "2016-03-01", "2016-03-10", false)]
        [InlineData("2016-02-28", "2016-03-01", null, false)]
        [InlineData("2016-02-28", "2016-02-28", "2016-03-01", true)]
        [InlineData("2016-02-28", "2016-02-27", "2016-03-01", true)]
        [InlineData("2016-02-28", "2016-02-28", null, true)]
        [InlineData("2016-02-28", "2016-02-27", null, true)]
        public void Delivery_HasQualifyingStartMeetsExpectation(string candidate, string start, string end, bool expectation)
        {
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(candidate));

            var startDate = DateTime.Parse(start);
            var endDate = string.IsNullOrWhiteSpace(end)
                ? (DateTime?)null
                : DateTime.Parse(end);

            var dateTimeQueryService = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTimeQueryService
                .Setup(x => x.IsDateBetween(mockDelivery.Object.LearnStartDate, startDate, endDate ?? DateTime.MaxValue, true))
                .Returns(expectation);

            var result = NewService(dateTimeQueryService: dateTimeQueryService.Object).HasQualifyingStart(mockDelivery.Object, startDate, endDate);

            Assert.Equal(expectation, result);
        }

        private RuleCommonOperationsProvider NewService(IDateTimeQueryService dateTimeQueryService = null)
        {
            return new RuleCommonOperationsProvider(dateTimeQueryService);
        }
    }
}
