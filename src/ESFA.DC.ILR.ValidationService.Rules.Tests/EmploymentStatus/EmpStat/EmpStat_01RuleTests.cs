using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpStat;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.EmploymentStatus.EmpStat
{
    public class EmpStat_01RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            var yeardata = new Mock<IAcademicYearDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new EmpStat_01Rule(null, mockDDRule07.Object, yeardata.Object));
        }

        /// <summary>
        /// New rule with null derived data rule 07 throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullDerivedDataRule07Throws()
        {
            // arrange
            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var yeardata = new Mock<IAcademicYearDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new EmpStat_01Rule(mockHandler.Object, null, yeardata.Object));
        }

        /// <summary>
        /// New rule with null year data throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullYearDataThrows()
        {
            // arrange
            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            var yeardata = new Mock<IAcademicYearDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new EmpStat_01Rule(mockHandler.Object, mockDDRule07.Object, null));
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
            Assert.Equal("EmpStat_01", result);
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
            Assert.Equal(EmpStat_01Rule.Name, result);
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
        /// First viable date meets expectation.
        /// </summary>
        [Fact]
        public void FirstViableDateMeetsExpectation()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.FirstViableDate;

            // assert
            Assert.Equal(DateTime.Parse("2012-08-01"), result);
        }

        /// <summary>
        /// Last viable date meets expectation.
        /// </summary>
        [Fact]
        public void LastViableDateMeetsExpectation()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.LastViableDate;

            // assert
            Assert.Equal(DateTime.Parse("2014-07-31"), result);
        }

        /// <summary>
        /// Is learner in custody with learning delivery fam meets expectation
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
        public void IsLearnerInCustodyMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
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
        /// Is comunity learning fund meets expectation
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
        [InlineData(Monitoring.Delivery.SteelIndustriesRedundancyTraining, false)]
        [InlineData(Monitoring.Delivery.ESFA16To19Funding, false)]
        [InlineData(Monitoring.Delivery.ESFAAdultFunding, false)]
        [InlineData(Monitoring.Delivery.HigherEducationFundingCouncilEngland, false)]
        [InlineData(Monitoring.Delivery.LocalAuthorityCommunityLearningFunds, true)]
        public void IsComunityLearningFundMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<ILearningDeliveryFAM>();
            mockItem
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(candidate.Substring(0, 3));
            mockItem
                .SetupGet(y => y.LearnDelFAMCode)
                .Returns(candidate.Substring(3));

            // act
            var result = sut.IsComunityLearningFund(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Is not funded by esfa meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(TypeOfFunding.AdultSkills, false)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, false)]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, false)]
        [InlineData(TypeOfFunding.CommunityLearning, false)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, false)]
        [InlineData(TypeOfFunding.NotFundedByESFA, true)]
        [InlineData(TypeOfFunding.Other16To19, false)]
        [InlineData(TypeOfFunding.OtherAdult, false)]
        public void IsNotFundedByESFAMeetsExpectation(int candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(candidate);

            // act
            var result = sut.IsNotFundedByESFA(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Is apprenticeship meets expectation
        /// </summary>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void IsApprenticeshipMeetsExpectation(bool expectation)
        {
            // arrange
            var mockItem = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);

            mockDDRule07
                .Setup(x => x.IsApprenticeship(null))
                .Returns(expectation);
            var yeardata = new Mock<IAcademicYearDataService>(MockBehavior.Strict);

            var sut = new EmpStat_01Rule(handler.Object, mockDDRule07.Object, yeardata.Object);

            // act
            var result = sut.IsApprenticeship(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
            handler.VerifyAll();
            mockDDRule07.VerifyAll();
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
        /// Is qualifying funding meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(TypeOfFunding.AdultSkills, true)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, false)]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, false)]
        [InlineData(TypeOfFunding.CommunityLearning, false)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, false)]
        [InlineData(TypeOfFunding.NotFundedByESFA, true)]
        [InlineData(TypeOfFunding.Other16To19, false)]
        [InlineData(TypeOfFunding.OtherAdult, true)]
        public void IsQualifyingFundingMeetsExpectation(int candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(candidate);

            // act
            var result = sut.IsQualifyingFunding(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Is qualifying aim meets expectation
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("2012-06-30", false)]
        [InlineData("2012-08-01", true)]
        [InlineData("2012-09-19", true)]
        [InlineData("2013-02-14", true)]
        [InlineData("2013-12-31", true)]
        [InlineData("2014-01-01", true)]
        [InlineData("2014-07-31", true)]
        [InlineData("2014-08-01", false)]
        public void IsQualifyingAimMeetsExpectation(string startDate, bool expectation)
        {
            // arrange
            var sut = NewRule();

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(startDate));

            // act
            var result = sut.IsQualifyingAim(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Get year of learning commencement date meets expectation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData("2017-08-26")]
        [InlineData("2017-08-31")]
        [InlineData("2017-09-01")]
        public void GetYearOfLearningCommencementDateMeetsExpectation(string candidate)
        {
            // arrange
            var testDate = DateTime.Parse(candidate);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            var yearData = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            yearData
                .Setup(x => x.GetAcademicYearOfLearningDate(testDate, AcademicYearDates.PreviousYearEnd))
                .Returns(DateTime.Today);

            var sut = new EmpStat_01Rule(handler.Object, mockDDRule07.Object, yearData.Object);

            // act, not interested in the result just that we hit a strict signature
            sut.GetYearOfLearningCommencementDate(testDate);

            // assert
            handler.VerifyAll();
            mockDDRule07.VerifyAll();
            yearData.VerifyAll();
        }

        /// <summary>
        /// Has qualifying employment status meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("2018-04-18", "2018-04-17", true)]
        [InlineData("2018-04-18", "2018-04-18", true)]
        [InlineData("2018-04-18", "2018-04-19", false)]
        [InlineData("2018-04-18", "2018-04-20", false)]
        public void HasQualifyingEmploymentStatusMeetsExpectation(string candidate, string startDate, bool expectation)
        {
            // arrange
            var sut = NewRule();

            var testDate = DateTime.Parse(candidate);
            var mockStatus = new Mock<ILearnerEmploymentStatus>();
            mockStatus
                .SetupGet(y => y.DateEmpStatApp)
                .Returns(DateTime.Parse(startDate));

            // act
            var result = sut.HasQualifyingEmploymentStatus(mockStatus.Object, testDate);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Has qualifying employment status with null statuses returns false
        /// </summary>
        [Fact]
        public void HasQualifyingEmploymentStatusWithNullStatusesReturnsFalse()
        {
            // arrange
            var sut = NewRule();

            var mockItem = new Mock<ILearner>();

            // act
            var result = sut.HasQualifyingEmploymentStatus(mockItem.Object, null);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Has qualifying employment status with empty statuses returns false
        /// </summary>
        [Fact]
        public void HasQualifyingEmploymentStatusWithEmptyStatusesReturnsFalse()
        {
            // arrange
            var sut = NewRule();

            var mockItem = new Mock<ILearner>();
            mockItem
                .SetupGet(x => x.LearnerEmploymentStatuses)
                .Returns(new List<ILearnerEmploymentStatus>());

            // act
            var result = sut.HasQualifyingEmploymentStatus(mockItem.Object, null);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="fundModel">The fund model.</param>
        /// <param name="learnStart">The learn start.</param>
        /// <param name="previousYearEnd">The previous year end.</param>
        [Theory]
        [InlineData(TypeOfFunding.AdultSkills, "2013-08-01", "2012-07-31")]
        [InlineData(TypeOfFunding.NotFundedByESFA, "2013-08-01", "2012-07-31")]
        [InlineData(TypeOfFunding.OtherAdult, "2013-08-01", "2012-07-31")]
        [InlineData(TypeOfFunding.AdultSkills, "2013-12-31", "2013-07-31")]
        [InlineData(TypeOfFunding.NotFundedByESFA, "2013-12-31", "2013-07-31")]
        [InlineData(TypeOfFunding.OtherAdult, "2013-12-31", "2013-07-31")]
        [InlineData(TypeOfFunding.AdultSkills, "2014-07-31", "2013-07-31")]
        [InlineData(TypeOfFunding.NotFundedByESFA, "2014-07-31", "2013-07-31")]
        [InlineData(TypeOfFunding.OtherAdult, "2014-07-31", "2013-07-31")]
        public void InvalidItemRaisesValidationMessage(int fundModel, string learnStart, string previousYearEnd)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var testDate = DateTime.Parse(learnStart);
            var previousYearEndDate = DateTime.Parse(previousYearEnd);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(fundModel);
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(testDate);
            mockDelivery
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(TypeOfLearningProgramme.IntermediateLevelApprenticeship);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            // ensure the status is OUTSIDE the qualifying date range
            var mockStatus = new Mock<ILearnerEmploymentStatus>();
            mockStatus
                .SetupGet(y => y.DateEmpStatApp)
                .Returns(testDate.AddDays(1));

            var statii = new List<ILearnerEmploymentStatus>();
            statii.Add(mockStatus.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);
            mockLearner
                .SetupGet(x => x.LearnerEmploymentStatuses)
                .Returns(statii);

            // get the learner inside the qualifying date range
            mockLearner
                .SetupGet(x => x.DateOfBirthNullable)
                .Returns(previousYearEndDate.AddYears(-19));

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(
                    Moq.It.Is<string>(y => y == EmpStat_01Rule.Name),
                    Moq.It.Is<string>(y => y == LearnRefNumber),
                    0,
                    Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == EmpStat_01Rule.MessagePropertyName),
                    "(missing)"))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == PropertyNameConstants.FundModel),
                    fundModel))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == PropertyNameConstants.LearnStartDate),
                    testDate))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == PropertyNameConstants.DateOfBirth),
                    previousYearEndDate.AddYears(-19)))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            mockDDRule07
                .Setup(x => x.IsApprenticeship(Moq.It.IsAny<int>()))
                .Returns(false);
            var yearData = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            yearData
                .Setup(x => x.GetAcademicYearOfLearningDate(testDate, AcademicYearDates.PreviousYearEnd))
                .Returns(previousYearEndDate);

            var sut = new EmpStat_01Rule(handler.Object, mockDDRule07.Object, yearData.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            mockDDRule07.VerifyAll();
            yearData.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        /// <param name="fundModel">The fund model.</param>
        /// <param name="learnStart">The learn start.</param>
        /// <param name="previousYearEnd">The previous year end.</param>
        [Theory]
        [InlineData(TypeOfFunding.AdultSkills, "2012-07-01", "2011-07-31")]
        [InlineData(TypeOfFunding.NotFundedByESFA, "2012-07-01", "2011-07-31")]
        [InlineData(TypeOfFunding.OtherAdult, "2012-07-01", "2011-07-31")]
        [InlineData(TypeOfFunding.AdultSkills, "2013-12-31", "2013-07-31")]
        [InlineData(TypeOfFunding.NotFundedByESFA, "2013-12-31", "2013-07-31")]
        [InlineData(TypeOfFunding.OtherAdult, "2013-12-31", "2013-07-31")]
        [InlineData(TypeOfFunding.AdultSkills, "2014-07-31", "2013-07-31")]
        [InlineData(TypeOfFunding.NotFundedByESFA, "2014-07-31", "2013-07-31")]
        [InlineData(TypeOfFunding.OtherAdult, "2014-07-31", "2013-07-31")]
        public void ValidItemDoesNotRaiseValidationMessage(int fundModel, string learnStart, string previousYearEnd)
        {
            // arrange
            const string LearnRefNumber = "123456789X";
            var testDate = DateTime.Parse(learnStart);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(fundModel);
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(testDate);
            mockDelivery
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(TypeOfLearningProgramme.IntermediateLevelApprenticeship);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            // ensure the status is INSIDE the qualifying date range
            var mockStatus = new Mock<ILearnerEmploymentStatus>();
            mockStatus
                .SetupGet(y => y.DateEmpStatApp)
                .Returns(testDate);

            var statii = new List<ILearnerEmploymentStatus>();
            statii.Add(mockStatus.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);
            mockLearner
                .SetupGet(x => x.LearnerEmploymentStatuses)
                .Returns(statii);

            // get the learner inside the qualifying date range
            mockLearner
                .SetupGet(x => x.DateOfBirthNullable)
                .Returns(testDate.AddYears(-20));

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            mockDDRule07
                .Setup(x => x.IsApprenticeship(Moq.It.IsAny<int>()))
                .Returns(false);
            var yearData = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            yearData
                .Setup(x => x.GetAcademicYearOfLearningDate(testDate, AcademicYearDates.PreviousYearEnd))
                .Returns(DateTime.Parse(previousYearEnd));

            var sut = new EmpStat_01Rule(handler.Object, mockDDRule07.Object, yearData.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            mockDDRule07.VerifyAll();
            yearData.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public EmpStat_01Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            var yeardata = new Mock<IAcademicYearDataService>(MockBehavior.Strict);

            return new EmpStat_01Rule(handler.Object, mockDDRule07.Object, yeardata.Object);
        }
    }
}
