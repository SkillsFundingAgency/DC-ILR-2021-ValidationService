using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Query
{
    public class RuleCommonOperationsProviderTests
    {
        /// <summary>
        /// Is restart meets expectation
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
        public void IsRestartMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewService();
            var mockDelivery = new Mock<ILearningDeliveryFAM>();
            mockDelivery
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(candidate);

            // act
            var result = sut.IsRestart(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Is advanced learner loan meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(Monitoring.Delivery.Types.AdvancedLearnerLoan, true)]
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
        [InlineData(Monitoring.Delivery.Types.Restart, false)]
        [InlineData(Monitoring.Delivery.Types.SourceOfFunding, false)]
        [InlineData(Monitoring.Delivery.Types.WorkProgrammeParticipation, false)]
        public void IsAdvancedLearnerLoanMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewService();
            var mockDelivery = new Mock<ILearningDeliveryFAM>();
            mockDelivery
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(candidate);

            // act
            var result = sut.IsAdvancedLearnerLoan(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Is loans bursary meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(Monitoring.Delivery.Types.AdvancedLearnerLoan, false)]
        [InlineData(Monitoring.Delivery.Types.AdvancedLearnerLoansBursaryFunding, true)]
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
        [InlineData(Monitoring.Delivery.Types.Restart, false)]
        [InlineData(Monitoring.Delivery.Types.SourceOfFunding, false)]
        [InlineData(Monitoring.Delivery.Types.WorkProgrammeParticipation, false)]
        public void IsLoansBursaryMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewService();
            var mockDelivery = new Mock<ILearningDeliveryFAM>();
            mockDelivery
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(candidate);

            // act
            var result = sut.IsLoansBursary(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Is learner in custody meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
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
            // arrange
            var sut = NewService();
            var mockItem = new Mock<ILearningDeliveryFAM>();
            mockItem
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(candidate.Substring(0, 3));
            mockItem
                .SetupGet(y => y.LearnDelFAMCode)
                .Returns(candidate.Substring(3));

            // act
            var result = sut.IsLearnerInCustody(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Is steel worker redundancy training meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
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
            // arrange
            var sut = NewService();
            var mockItem = new Mock<ILearningDeliveryFAM>();
            mockItem
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(candidate.Substring(0, 3));
            mockItem
                .SetupGet(y => y.LearnDelFAMCode)
                .Returns(candidate.Substring(3));

            // act
            var result = sut.IsSteelWorkerRedundancyTraining(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Is released on temporary licence meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
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
            // arrange
            var sut = NewService();
            var mockItem = new Mock<ILearningDeliveryFAM>();
            mockItem
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(candidate.Substring(0, 3));
            mockItem
                .SetupGet(y => y.LearnDelFAMCode)
                .Returns(candidate.Substring(3));

            // act
            var result = sut.IsReleasedOnTemporaryLicence(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// In apprenticeship meets expectation
        /// </summary>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void InApprenticeshipMeetsExpectation(bool expectation)
        {
            // arrange
            var mockItem = new Mock<ILearningDelivery>();

            var derivedData07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            derivedData07
                .Setup(x => x.IsApprenticeship(null))
                .Returns(expectation);

            var sut = new RuleCommonOperationsProvider(derivedData07.Object);

            // act
            var result = sut.InApprenticeship(mockItem.Object);

            // assert
            derivedData07.VerifyAll();

            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// In a programme meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(TypeOfAim.ProgrammeAim, true)]
        [InlineData(TypeOfAim.AimNotPartOfAProgramme, false)]
        [InlineData(TypeOfAim.ComponentAimInAProgramme, false)]
        [InlineData(TypeOfAim.CoreAim16To19ExcludingApprenticeships, false)]
        public void InAProgrammeMeetsExpectation(int candidate, bool expectation)
        {
            // arrange
            var sut = NewService();
            var mockItem = new Mock<ILearningDelivery>();
            mockItem
                .SetupGet(y => y.AimType)
                .Returns(candidate);

            // act
            var result = sut.InAProgramme(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(TypeOfAim.AimNotPartOfAProgramme, false)]
        [InlineData(TypeOfAim.ComponentAimInAProgramme, true)]
        [InlineData(TypeOfAim.CoreAim16To19ExcludingApprenticeships, false)]
        [InlineData(TypeOfAim.ProgrammeAim, false)]
        public void IsComponentOfAProgramMeetsExpectation(int candidate, bool expectation)
        {
            // arrange
            var sut = NewService();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.AimType)
                .Returns(candidate);

            // act
            var result = sut.IsComponentOfAProgram(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Determines whether [is traineeship meets expectation] [the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(TypeOfLearningProgramme.AdvancedLevelApprenticeship, false)]
        [InlineData(TypeOfLearningProgramme.ApprenticeshipStandard, false)]
        [InlineData(TypeOfLearningProgramme.HigherApprenticeshipLevel4, false)]
        [InlineData(TypeOfLearningProgramme.HigherApprenticeshipLevel5, false)]
        [InlineData(TypeOfLearningProgramme.HigherApprenticeshipLevel6, false)]
        [InlineData(TypeOfLearningProgramme.HigherApprenticeshipLevel7Plus, false)]
        [InlineData(TypeOfLearningProgramme.IntermediateLevelApprenticeship, false)]
        [InlineData(TypeOfLearningProgramme.Traineeship, true)]
        public void IsTraineeshipMeetsExpectation(int candidate, bool expectation)
        {
            // arrange
            var sut = NewService();
            var mockItem = new Mock<ILearningDelivery>(MockBehavior.Strict);
            mockItem
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(candidate);

            // act
            var result = sut.IsTraineeship(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
            mockItem.VerifyAll();
        }

        /// <summary>
        /// Has qualifying funding meets expectation
        /// </summary>
        /// <param name="funding">The funding.</param>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
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
            // arrange
            var sut = NewService();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(funding);

            // act
            var result = sut.HasQualifyingFunding(mockDelivery.Object, candidate);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Delivery has qualifying start meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("2016-02-28", "2016-03-01", "2016-03-10", false)]
        [InlineData("2016-02-28", "2016-03-01", null, false)]
        [InlineData("2016-02-28", "2016-02-28", "2016-03-01", true)]
        [InlineData("2016-02-28", "2016-02-27", "2016-03-01", true)]
        [InlineData("2016-02-28", "2016-02-28", null, true)]
        [InlineData("2016-02-28", "2016-02-27", null, true)]
        public void Delivery_HasQualifyingStartMeetsExpectation(string candidate, string start, string end, bool expectation)
        {
            // arrange
            var sut = NewService();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(candidate));

            var endDate = string.IsNullOrWhiteSpace(end)
                ? (DateTime?)null
                : DateTime.Parse(end);

            // act
            var result = sut.HasQualifyingStart(mockDelivery.Object, DateTime.Parse(start), endDate);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Employment has qualifying start meets expectation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("2016-02-28", "2016-03-01", "2016-03-10", false)]
        [InlineData("2016-02-28", "2016-03-01", null, false)]
        [InlineData("2016-02-28", "2016-02-28", "2016-03-01", true)]
        [InlineData("2016-02-28", "2016-02-27", "2016-03-01", true)]
        [InlineData("2016-02-28", "2016-02-28", null, true)]
        [InlineData("2016-02-28", "2016-02-27", null, true)]
        public void Employment_HasQualifyingStartMeetsExpectation(string candidate, string start, string end, bool expectation)
        {
            // arrange
            var sut = NewService();
            var mockDelivery = new Mock<ILearnerEmploymentStatus>();
            mockDelivery
                .SetupGet(y => y.DateEmpStatApp)
                .Returns(DateTime.Parse(candidate));

            var endDate = string.IsNullOrWhiteSpace(end)
                ? (DateTime?)null
                : DateTime.Parse(end);

            // act
            var result = sut.HasQualifyingStart(mockDelivery.Object, DateTime.Parse(start), endDate);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Get qualifying employment status meets expectation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">The expectation.</param>
        /// <param name="starts">The starts.</param>
        [Theory]
        [InlineData("2018-09-11", "2018-09-04", "2014-08-01", "2018-09-04", "2016-02-11", "2017-06-09")]
        [InlineData("2018-09-11", "2018-09-11", "2014-08-01", "2018-09-11", "2016-02-11", "2017-06-09")]
        [InlineData("2017-12-31", "2017-12-30", "2015-12-31", "2017-12-30", "2014-12-31", "2017-10-16")]
        [InlineData("2017-12-31", "2017-12-30", "2015-12-31", "2017-12-30", "2018-01-01", "2014-12-31", "2017-10-16")]
        [InlineData("2018-07-01", "2018-06-30", "2018-06-30", "2014-05-11", "2014-07-12")]
        [InlineData("2018-07-01", "2014-07-12", "2018-08-30", "2018-07-16", "2014-05-11", "2014-07-12")]
        [InlineData("2016-11-17", "2016-11-17", "2016-11-17")]
        [InlineData("2016-11-17", "2016-11-17", "2016-11-07", "2016-11-18", "2016-11-17")]
        public void GetEmploymentStatusOnMeetsExpectation(string candidate, string expectation, params string[] starts)
        {
            // arrange
            var sut = NewService();
            var learnDate = DateTime.Parse(candidate);
            var expectedDate = DateTime.Parse(expectation);

            var employments = new List<ILearnerEmploymentStatus>();

            foreach (var start in starts)
            {
                var mockItem = new Mock<ILearnerEmploymentStatus>();
                mockItem
                    .SetupGet(y => y.DateEmpStatApp)
                    .Returns(DateTime.Parse(start));

                employments.Add(mockItem.Object);
            }

            // act
            var result = sut.GetEmploymentStatusOn(learnDate, employments);

            // assert
            Assert.Equal(expectedDate, result.DateEmpStatApp);
        }

        /// <summary>
        /// New service.
        /// </summary>
        /// <returns>a new service</returns>
        private RuleCommonOperationsProvider NewService()
        {
            var ddRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);

            return new RuleCommonOperationsProvider(ddRule07.Object);
        }
    }
}
