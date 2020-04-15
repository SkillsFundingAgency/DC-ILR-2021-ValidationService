using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpStat;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.EmploymentStatus.EmpStat
{
    public class EmpStat_01RuleTests : AbstractRuleTests<EmpStat_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("EmpStat_01", result);
        }

        [Fact]
        public void FirstViableDateMeetsExpectation()
        {
            var sut = NewRule();

            var result = sut.FirstViableDate;

            Assert.Equal(DateTime.Parse("2012-08-01"), result);
        }

        [Fact]
        public void LastViableDateMeetsExpectation()
        {
            var sut = NewRule();

            var result = sut.LastViableDate;

            Assert.Equal(DateTime.Parse("2014-07-31"), result);
        }

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
            var sut = NewRule();
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
        [InlineData(Monitoring.Delivery.SteelIndustriesRedundancyTraining, false)]
        [InlineData(Monitoring.Delivery.ESFA16To19Funding, false)]
        [InlineData(Monitoring.Delivery.ESFAAdultFunding, false)]
        [InlineData(Monitoring.Delivery.HigherEducationFundingCouncilEngland, false)]
        [InlineData(Monitoring.Delivery.LocalAuthorityCommunityLearningFunds, true)]
        public void IsComunityLearningFundMeetsExpectation(string candidate, bool expectation)
        {
            var sut = NewRule();
            var mockItem = new Mock<ILearningDeliveryFAM>();
            mockItem
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(candidate.Substring(0, 3));
            mockItem
                .SetupGet(y => y.LearnDelFAMCode)
                .Returns(candidate.Substring(3));

            var result = sut.IsComunityLearningFund(mockItem.Object);

            Assert.Equal(expectation, result);
        }

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
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(candidate);

            var result = sut.IsNotFundedByESFA(mockDelivery.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void IsApprenticeshipMeetsExpectation(bool expectation)
        {
            var mockItem = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);

            mockDDRule07
                .Setup(x => x.IsApprenticeship(null))
                .Returns(expectation);
            var yeardata = new Mock<IAcademicYearDataService>(MockBehavior.Strict);

            var sut = NewRule(handler.Object, mockDDRule07.Object, yeardata.Object);

            var result = sut.IsApprenticeship(mockItem.Object);

            Assert.Equal(expectation, result);
            handler.VerifyAll();
            mockDDRule07.VerifyAll();
        }

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
            var sut = NewRule();
            var mockItem = new Mock<ILearningDelivery>();
            mockItem
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(candidate);

            var result = sut.InTraining(mockItem.Object);

            Assert.Equal(expectation, result);
        }

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
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(candidate);

            var result = sut.IsQualifyingFunding(mockDelivery.Object);

            Assert.Equal(expectation, result);
        }

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
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(startDate));

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(x => x.IsDateBetween(
                mockDelivery.Object.LearnStartDate, Moq.It.IsAny<DateTime>(), Moq.It.IsAny<DateTime>(), true)).Returns(expectation);

            var result = NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object).IsQualifyingAim(mockDelivery.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData("2017-08-26")]
        [InlineData("2017-08-31")]
        [InlineData("2017-09-01")]
        public void GetYearOfLearningCommencementDateMeetsExpectation(string candidate)
        {
            var testDate = DateTime.Parse(candidate);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            var yearData = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            yearData
                .Setup(x => x.GetAcademicYearOfLearningDate(testDate, AcademicYearDates.PreviousYearEnd))
                .Returns(DateTime.Today);

            var sut = NewRule(handler.Object, mockDDRule07.Object, yearData.Object);

            sut.GetYearOfLearningCommencementDate(testDate);

            handler.VerifyAll();
            mockDDRule07.VerifyAll();
            yearData.VerifyAll();
        }

        [Theory]
        [InlineData("2018-04-18", "2018-04-17", true)]
        [InlineData("2018-04-18", "2018-04-18", true)]
        [InlineData("2018-04-18", "2018-04-19", false)]
        [InlineData("2018-04-18", "2018-04-20", false)]
        public void HasQualifyingEmploymentStatusMeetsExpectation(string candidate, string startDate, bool expectation)
        {
            var sut = NewRule();

            var testDate = DateTime.Parse(candidate);
            var mockStatus = new Mock<ILearnerEmploymentStatus>();
            mockStatus
                .SetupGet(y => y.DateEmpStatApp)
                .Returns(DateTime.Parse(startDate));

            var result = sut.HasQualifyingEmploymentStatus(mockStatus.Object, testDate);

            Assert.Equal(expectation, result);
        }

        [Fact]
        public void HasQualifyingEmploymentStatusWithNullStatusesReturnsFalse()
        {
            var sut = NewRule();

            var mockItem = new Mock<ILearner>();

            var result = sut.HasQualifyingEmploymentStatus(mockItem.Object, null);

            Assert.False(result);
        }

        [Fact]
        public void HasQualifyingEmploymentStatusWithEmptyStatusesReturnsFalse()
        {
            var sut = NewRule();

            var mockItem = new Mock<ILearner>();
            mockItem
                .SetupGet(x => x.LearnerEmploymentStatuses)
                .Returns(new List<ILearnerEmploymentStatus>());

            var result = sut.HasQualifyingEmploymentStatus(mockItem.Object, null);

            Assert.False(result);
        }

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

            mockLearner
                .SetupGet(x => x.DateOfBirthNullable)
                .Returns(previousYearEndDate.AddYears(-19));

            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            mockDDRule07
                .Setup(x => x.IsApprenticeship(Moq.It.IsAny<int>()))
                .Returns(false);
            var yearData = new Mock<IAcademicYearDataService>(MockBehavior.Strict);
            yearData
                .Setup(x => x.GetAcademicYearOfLearningDate(testDate, AcademicYearDates.PreviousYearEnd))
                .Returns(previousYearEndDate);
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(x => x.IsDateBetween(
                mockDelivery.Object.LearnStartDate, Moq.It.IsAny<DateTime>(), Moq.It.IsAny<DateTime>(), true)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object, mockDDRule07.Object, yearData.Object, dateTimeQueryServiceMock.Object).Validate(mockLearner.Object);
            }
        }

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
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(x => x.IsDateBetween(
                mockDelivery.Object.LearnStartDate, Moq.It.IsAny<DateTime>(), Moq.It.IsAny<DateTime>(), true)).Returns(true);

            var sut = NewRule(handler.Object, mockDDRule07.Object, yearData.Object, dateTimeQueryServiceMock.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            mockDDRule07.VerifyAll();
            yearData.VerifyAll();
        }

        public EmpStat_01Rule NewRule(
            IValidationErrorHandler handler = null,
            IDerivedData_07Rule dd07 = null,
            IAcademicYearDataService academicYearDataService = null,
            IDateTimeQueryService dateTimeQueryService = null)
        {
            return new EmpStat_01Rule(
                handler,
                dd07,
                academicYearDataService,
                dateTimeQueryService);
        }
    }
}
