using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpStat;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.EmploymentStatus.EmpStat
{
    public class EmpStat_11RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange / act / assert
            Assert.Throws<ArgumentNullException>(() => new EmpStat_11Rule(null));
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
            Assert.Equal("EmpStat_11", result);
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
            Assert.Equal(EmpStat_11Rule.Name, result);
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
        /// Last inviable date meets expectation.
        /// </summary>
        [Fact]
        public void LastInviableDateMeetsExpectation()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.LastInviableDate;

            // assert
            Assert.Equal(DateTime.Parse("2014-07-31"), result);
        }

        /// <summary>
        /// In training meets expectation.
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
        public void InTrainingMeetsExpectation(int candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<ILearningDelivery>();
            mockItem
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(candidate);

            // act
            var result = sut.InTraining(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Is viable start meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("2013-08-01", false)]
        [InlineData("2014-07-31", false)]
        [InlineData("2014-08-01", true)]
        [InlineData("2014-09-14", true)]
        public void IsViableStartMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(candidate));

            // act
            var result = sut.IsViableStart(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Has qualifying funding meets expectation
        /// </summary>
        /// <param name="funding">The funding.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(TypeOfFunding.AdultSkills, false)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, true)]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, false)]
        [InlineData(TypeOfFunding.CommunityLearning, false)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, false)]
        [InlineData(TypeOfFunding.NotFundedByESFA, false)]
        [InlineData(TypeOfFunding.Other16To19, true)]
        [InlineData(TypeOfFunding.OtherAdult, false)]
        public void HasQualifyingFundingMeetsExpectation(int funding, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(funding);

            // act
            var result = sut.HasQualifyingFunding(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Has a qualifying employment status with null employment returns false
        /// </summary>
        [Fact]
        public void HasAQualifyingEmploymentStatusWithNullEmploymentReturnsFalse()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.HasAQualifyingEmploymentStatus(null, null);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Has a qualifying employment status with null statuses returns false
        /// </summary>
        [Fact]
        public void HasAQualifyingEmploymentStatusWithNullStatusesReturnsFalse()
        {
            // arrange
            var sut = NewRule();

            var mockItem = new Mock<ILearningDelivery>();

            // act
            var result = sut.HasAQualifyingEmploymentStatus(mockItem.Object, null);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Has a qualifying employment status with empty statuses returns false
        /// </summary>
        [Fact]
        public void HasAQualifyingEmploymentStatusWithEmptyStatusesReturnsFalse()
        {
            // arrange
            var sut = NewRule();

            var mockItem = new Mock<ILearningDelivery>();

            // act
            var result = sut.HasAQualifyingEmploymentStatus(mockItem.Object, new List<ILearnerEmploymentStatus>());

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Get qualifying hours meets expectation.
        /// </summary>
        /// <param name="planHours">The plan hours.</param>
        /// <param name="eepHours">The eep hours.</param>
        /// <param name="expectation">The expectation.</param>
        [Theory]
        [InlineData(null, null, 0)]
        [InlineData(null, 0, 0)]
        [InlineData(0, null, 0)]
        [InlineData(0, 0, 0)]
        [InlineData(null, 1, 1)]
        [InlineData(1, null, 1)]
        [InlineData(1, 1, 2)]
        [InlineData(263, 100, 363)]
        [InlineData(63, 10, 73)]
        public void GetQualifyingHoursMeetsExpectation(int? planHours, int? eepHours, int expectation)
        {
            // arrange
            var sut = NewRule();

            var mockItem = new Mock<ILearner>();
            mockItem
                .SetupGet(x => x.PlanLearnHoursNullable)
                .Returns(planHours);
            mockItem
                .SetupGet(x => x.PlanEEPHoursNullable)
                .Returns(eepHours);

            // act
            var result = sut.GetQualifyingHours(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Has qualifying hours meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(263, true)]
        [InlineData(539, true)]
        [InlineData(540, false)]
        [InlineData(591, false)]
        public void HasQualifyingHoursMeetsExpectation(int candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.HasQualifyingHours(candidate);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="fundModel">The fund model.</param>
        /// <param name="planHours">The plan hours.</param>
        /// <param name="eepHours">The eep hours.</param>
        /// <param name="learnStart">The learn start.</param>
        /// <param name="offSet">The off set.</param>
        [Theory]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, 1, 1, "2014-08-01", 1)]
        [InlineData(TypeOfFunding.Other16To19, 1, 1, "2014-08-01", 1)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, 1, 1, "2014-08-01", 0)]
        [InlineData(TypeOfFunding.Other16To19, 1, 1, "2014-08-01", 0)]
        public void InvalidItemRaisesValidationMessage(int fundModel, int? planHours, int? eepHours, string learnStart, int offSet)
        {
            // arrange
            const string LearnRefNumber = "123456789X";
            const int AimSeqNumber = 1;

            var testDate = DateTime.Parse(learnStart);

            var deliveries = new List<ILearningDelivery>();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.FundModel)
                .Returns(fundModel);
            mockDelivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(testDate);
            mockDelivery
                .SetupGet(x => x.AimType)
                .Returns(TypeOfAim.ProgrammeAim);
            mockDelivery
                .SetupGet(x => x.AimSeqNumber)
                .Returns(AimSeqNumber);
            deliveries.Add(mockDelivery.Object);

            var statii = new List<ILearnerEmploymentStatus>();
            var mockStatus = new Mock<ILearnerEmploymentStatus>();
            mockStatus
                .SetupGet(y => y.DateEmpStatApp)
                .Returns(testDate.AddDays(offSet));
            statii.Add(mockStatus.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.PlanLearnHoursNullable)
                .Returns(planHours);
            mockLearner
                .SetupGet(x => x.PlanEEPHoursNullable)
                .Returns(eepHours);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);
            mockLearner
                .SetupGet(x => x.LearnerEmploymentStatuses)
                .Returns(statii);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(
                    Moq.It.Is<string>(y => y == EmpStat_11Rule.Name),
                    Moq.It.Is<string>(y => y == LearnRefNumber),
                    AimSeqNumber,
                    Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == EmpStat_11Rule.MessagePropertyName),
                    TypeOfEmploymentStatus.NotKnownProvided))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == PropertyNameConstants.LearnStartDate),
                    testDate))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var sut = new EmpStat_11Rule(handler.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        /// <param name="fundModel">The fund model.</param>
        /// <param name="planHours">The plan hours.</param>
        /// <param name="eepHours">The eep hours.</param>
        /// <param name="learnStart">The learn start.</param>
        /// <param name="offSet">The off set.</param>
        [Theory]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, 340, 200, "2014-08-01", 0)] // stops at qualifying hours
        [InlineData(TypeOfFunding.Other16To19, 340, 200, "2014-08-01", 0)] // stops at qualifying hours
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, 1, 1, "2014-07-31", 0)] // stops at start date
        [InlineData(TypeOfFunding.Other16To19, 1, 1, "2014-07-31", 0)] // stops at start date
        [InlineData(TypeOfFunding.AdultSkills, 1, 1, "2014-08-01", 0)] // stops at funding model
        [InlineData(TypeOfFunding.OtherAdult, 1, 1, "2014-08-01", 0)] // stops at funding model
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, 1, 1, "2014-08-01", -1)] // stops at 'date emp stat app'
        [InlineData(TypeOfFunding.Other16To19, 1, 1, "2014-08-01", -1)] // stops at 'date emp stat app'
        public void ValidItemDoesNotRaiseValidationMessage(int fundModel, int? planHours, int? eepHours, string learnStart, int offSet)
        {
            // arrange
            const string LearnRefNumber = "123456789X";
            const int AimSeqNumber = 1;

            var testDate = DateTime.Parse(learnStart);

            var deliveries = new List<ILearningDelivery>();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.FundModel)
                .Returns(fundModel);
            mockDelivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(testDate);
            mockDelivery
                .SetupGet(x => x.AimType)
                .Returns(TypeOfAim.ProgrammeAim);
            mockDelivery
                .SetupGet(x => x.AimSeqNumber)
                .Returns(AimSeqNumber);
            deliveries.Add(mockDelivery.Object);

            var statii = new List<ILearnerEmploymentStatus>();
            var mockStatus = new Mock<ILearnerEmploymentStatus>();
            mockStatus
                .SetupGet(y => y.DateEmpStatApp)
                .Returns(testDate.AddDays(offSet));
            statii.Add(mockStatus.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.PlanLearnHoursNullable)
                .Returns(planHours);
            mockLearner
                .SetupGet(x => x.PlanEEPHoursNullable)
                .Returns(eepHours);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);
            mockLearner
                .SetupGet(x => x.LearnerEmploymentStatuses)
                .Returns(statii);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var sut = new EmpStat_11Rule(handler.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public EmpStat_11Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            return new EmpStat_11Rule(handler.Object);
        }
    }
}
