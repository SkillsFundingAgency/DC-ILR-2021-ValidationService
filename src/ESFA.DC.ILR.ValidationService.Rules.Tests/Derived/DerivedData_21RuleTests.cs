using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using Moq;
using System;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Derived
{
    public class DerivedData_21RuleTests
    {
        /// <summary>
        /// Is adult skills funded unemployed learner with null delivery throws
        /// </summary>
        [Fact]
        public void IsAdultSkillsFundedUnemployedLearnerWithNullDeliveryThrows()
        {
            // arrange
            var sut = NewRule();

            // act / assert
            Assert.Throws<ArgumentNullException>(() => sut.IsAdultFundedUnemployedWithOtherStateBenefits(null, new Mock<ILearner>().Object));
        }

        /// <summary>
        /// Determines whether [is adult skills funded unemployed learner with null learner throws].
        /// </summary>
        [Fact]
        public void IsAdultSkillsFundedUnemployedLearnerWithNullLearnerThrows()
        {
            // arrange
            var sut = NewRule();

            // act / assert
            Assert.Throws<ArgumentNullException>(() => sut.IsAdultFundedUnemployedWithOtherStateBenefits(new Mock<ILearningDelivery>().Object, null));
        }

        /// <summary>
        /// Determines whether [is not employed meets expectation] [the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable, true)]
        [InlineData(TypeOfEmploymentStatus.InPaidEmployment, false)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable, true)]
        [InlineData(TypeOfEmploymentStatus.NotKnownProvided, false)]
        public void IsNotEmployedMeetsExpectation(int candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearnerEmploymentStatus>();
            mockDelivery
                .SetupGet(y => y.EmpStat)
                .Returns(candidate);

            // act
            var result = sut.IsNotEmployed(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// In receipt of another benefit meets expectation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(Monitoring.EmploymentStatus.EmployedForLessThan16HoursPW, false)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor16HoursOrMorePW, false)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor16To19HoursPW, false)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor20HoursOrMorePW, false)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor0To10HourPW, false)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor11To20HoursPW, false)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor21To30HoursPW, false)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor31PlusHoursPW, false)]
        [InlineData(Monitoring.EmploymentStatus.EmployedForUpTo3M, false)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor4To6M, false)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor7To12M, false)]
        [InlineData(Monitoring.EmploymentStatus.EmployedForMoreThan12M, false)]
        [InlineData(Monitoring.EmploymentStatus.InFulltimeEducationOrTrainingPriorToEnrolment, false)]
        [InlineData(Monitoring.EmploymentStatus.InReceiptOfAnotherStateBenefit, true)]
        [InlineData(Monitoring.EmploymentStatus.InReceiptOfEmploymentAndSupportAllowance, false)]
        [InlineData(Monitoring.EmploymentStatus.InReceiptOfJobSeekersAllowance, false)]
        [InlineData(Monitoring.EmploymentStatus.InReceiptOfUniversalCredit, false)]
        [InlineData(Monitoring.EmploymentStatus.SelfEmployed, false)]
        [InlineData(Monitoring.EmploymentStatus.SmallEmployer, false)]
        [InlineData(Monitoring.EmploymentStatus.UnemployedForLessThan6M, false)]
        [InlineData(Monitoring.EmploymentStatus.UnemployedFor6To11M, false)]
        [InlineData(Monitoring.EmploymentStatus.UnemployedFor12To23M, false)]
        [InlineData(Monitoring.EmploymentStatus.UnemployedFor24To35M, false)]
        [InlineData(Monitoring.EmploymentStatus.UnemployedFor36MPlus, false)]
        public void InReceiptOfAnotherBenefitMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<IEmploymentStatusMonitoring>(MockBehavior.Strict);
            mockItem
                .SetupGet(y => y.ESMType)
                .Returns(candidate.Substring(0, 3));
            mockItem
                .SetupGet(y => y.ESMCode)
                .Returns(int.Parse(candidate.Substring(3)));

            // act
            var result = sut.InReceiptOfAnotherBenefit(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
            mockItem.VerifyAll();
        }

        /// <summary>
        /// In receipt of universal credit meets expectation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(Monitoring.EmploymentStatus.EmployedForLessThan16HoursPW, false)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor16HoursOrMorePW, false)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor16To19HoursPW, false)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor20HoursOrMorePW, false)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor0To10HourPW, false)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor11To20HoursPW, false)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor21To30HoursPW, false)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor31PlusHoursPW, false)]
        [InlineData(Monitoring.EmploymentStatus.EmployedForUpTo3M, false)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor4To6M, false)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor7To12M, false)]
        [InlineData(Monitoring.EmploymentStatus.EmployedForMoreThan12M, false)]
        [InlineData(Monitoring.EmploymentStatus.InFulltimeEducationOrTrainingPriorToEnrolment, false)]
        [InlineData(Monitoring.EmploymentStatus.InReceiptOfAnotherStateBenefit, false)]
        [InlineData(Monitoring.EmploymentStatus.InReceiptOfEmploymentAndSupportAllowance, false)]
        [InlineData(Monitoring.EmploymentStatus.InReceiptOfJobSeekersAllowance, false)]
        [InlineData(Monitoring.EmploymentStatus.InReceiptOfUniversalCredit, true)]
        [InlineData(Monitoring.EmploymentStatus.SelfEmployed, false)]
        [InlineData(Monitoring.EmploymentStatus.SmallEmployer, false)]
        [InlineData(Monitoring.EmploymentStatus.UnemployedForLessThan6M, false)]
        [InlineData(Monitoring.EmploymentStatus.UnemployedFor6To11M, false)]
        [InlineData(Monitoring.EmploymentStatus.UnemployedFor12To23M, false)]
        [InlineData(Monitoring.EmploymentStatus.UnemployedFor24To35M, false)]
        [InlineData(Monitoring.EmploymentStatus.UnemployedFor36MPlus, false)]
        public void InReceiptOfUniversalCreditMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<IEmploymentStatusMonitoring>(MockBehavior.Strict);
            mockItem
                .SetupGet(y => y.ESMType)
                .Returns(candidate.Substring(0, 3));
            mockItem
                .SetupGet(y => y.ESMCode)
                .Returns(int.Parse(candidate.Substring(3)));

            // act
            var result = sut.InReceiptOfUniversalCredit(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
            mockItem.VerifyAll();
        }

        [Theory]
        [InlineData(Monitoring.Delivery.Types.AdvancedLearnerLoansBursaryFunding, false)]
        [InlineData(Monitoring.Delivery.Types.AdvancedLearnerLoan, false)]
        [InlineData(Monitoring.Delivery.Types.ApprenticeshipContract, false)]
        [InlineData(Monitoring.Delivery.Types.CommunityLearningProvision, false)]
        [InlineData(Monitoring.Delivery.Types.EligibilityForEnhancedApprenticeshipFunding, false)]
        [InlineData(Monitoring.Delivery.Types.FamilyEnglishMathsAndLanguage, false)]
        [InlineData(Monitoring.Delivery.Types.FullOrCoFunding, false)]
        [InlineData(Monitoring.Delivery.Types.HEMonitoring, false)]
        [InlineData(Monitoring.Delivery.Types.HouseholdSituation, false)]
        [InlineData(Monitoring.Delivery.Types.Learning, true)]
        [InlineData(Monitoring.Delivery.Types.LearningSupportFunding, false)]
        [InlineData(Monitoring.Delivery.Types.NationalSkillsAcademy, false)]
        [InlineData(Monitoring.Delivery.Types.PercentageOfOnlineDelivery, false)]
        [InlineData(Monitoring.Delivery.Types.Restart, false)]
        [InlineData(Monitoring.Delivery.Types.SourceOfFunding, false)]
        [InlineData(Monitoring.Delivery.Types.WorkProgrammeParticipation, false)]
        public void IsMonitoredMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<ILearningDeliveryFAM>(MockBehavior.Strict);
            mockItem
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(candidate);

            // act
            var result = sut.IsMonitored(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
            mockItem.VerifyAll();
        }

        [Theory]
        [InlineData(Monitoring.Delivery.InReceiptOfLowWages, false)]
        [InlineData(Monitoring.Delivery.MandationToSkillsTraining, true)]
        [InlineData(Monitoring.Delivery.SteelIndustriesRedundancyTraining, false)]
        public void MandatedToSkillsTrainingMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<ILearningDeliveryFAM>(MockBehavior.Strict);
            mockItem
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(candidate.Substring(0, 3));
            mockItem
                .SetupGet(y => y.LearnDelFAMCode)
                .Returns(candidate.Substring(3));

            // act
            var result = sut.MandatedToSkillsTraining(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
            mockItem.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up derived data rule</returns>
        public DerivedData_21Rule NewRule()
        {
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            return new DerivedData_21Rule(commonOps.Object);
        }
    }
}
