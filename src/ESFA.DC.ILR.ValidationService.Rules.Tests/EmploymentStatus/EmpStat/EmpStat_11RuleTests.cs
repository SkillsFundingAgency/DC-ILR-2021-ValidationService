using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpStat;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.EmploymentStatus.EmpStat
{
    public class EmpStat_11RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("EmpStat_11", result);
        }

        [Fact]
        public void LastInviableDateMeetsExpectation()
        {
            var sut = NewRule();

            var result = sut.LastInviableDate;

            Assert.Equal(DateTime.Parse("2014-07-31"), result);
        }

        [Theory]
        [InlineData(ProgTypes.AdvancedLevelApprenticeship, false)]
        [InlineData(ProgTypes.ApprenticeshipStandard, false)]
        [InlineData(ProgTypes.HigherApprenticeshipLevel4, false)]
        [InlineData(ProgTypes.HigherApprenticeshipLevel5, false)]
        [InlineData(ProgTypes.HigherApprenticeshipLevel6, false)]
        [InlineData(ProgTypes.HigherApprenticeshipLevel7Plus, false)]
        [InlineData(ProgTypes.IntermediateLevelApprenticeship, false)]
        [InlineData(ProgTypes.Traineeship, true)]
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
        [InlineData("2013-08-01", false)]
        [InlineData("2014-07-31", false)]
        [InlineData("2014-08-01", true)]
        [InlineData("2014-09-14", true)]
        public void IsViableStartMeetsExpectation(string candidate, bool expectation)
        {
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(candidate));

            var result = sut.IsViableStart(mockDelivery.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(FundModels.AdultSkills, false)]
        [InlineData(FundModels.Age16To19ExcludingApprenticeships, true)]
        [InlineData(FundModels.ApprenticeshipsFrom1May2017, false)]
        [InlineData(FundModels.CommunityLearning, false)]
        [InlineData(FundModels.EuropeanSocialFund, false)]
        [InlineData(FundModels.NotFundedByESFA, false)]
        [InlineData(FundModels.Other16To19, true)]
        [InlineData(FundModels.OtherAdult, false)]
        public void HasQualifyingFundingMeetsExpectation(int funding, bool expectation)
        {
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(funding);

            var result = sut.HasQualifyingFunding(mockDelivery.Object);

            Assert.Equal(expectation, result);
        }

        [Fact]
        public void HasAQualifyingEmploymentStatusWithNullEmploymentReturnsFalse()
        {
            var sut = NewRule();

            var result = sut.HasAQualifyingEmploymentStatus(null, null);

            Assert.False(result);
        }

        [Fact]
        public void HasAQualifyingEmploymentStatusWithNullStatusesReturnsFalse()
        {
            var sut = NewRule();

            var mockItem = new Mock<ILearningDelivery>();

            var result = sut.HasAQualifyingEmploymentStatus(mockItem.Object, null);

            Assert.False(result);
        }

        [Fact]
        public void HasAQualifyingEmploymentStatusWithEmptyStatusesReturnsFalse()
        {
            var sut = NewRule();

            var mockItem = new Mock<ILearningDelivery>();

            var result = sut.HasAQualifyingEmploymentStatus(mockItem.Object, new List<ILearnerEmploymentStatus>());

            Assert.False(result);
        }

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
            var sut = NewRule();

            var mockItem = new Mock<ILearner>();
            mockItem
                .SetupGet(x => x.PlanLearnHoursNullable)
                .Returns(planHours);
            mockItem
                .SetupGet(x => x.PlanEEPHoursNullable)
                .Returns(eepHours);

            var result = sut.GetQualifyingHours(mockItem.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(263, true)]
        [InlineData(539, true)]
        [InlineData(540, false)]
        [InlineData(591, false)]
        public void HasQualifyingHoursMeetsExpectation(int candidate, bool expectation)
        {
            var sut = NewRule();

            var result = sut.HasQualifyingHours(candidate);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(FundModels.Age16To19ExcludingApprenticeships, 1, 1, "2014-08-01", 1)]
        [InlineData(FundModels.Other16To19, 1, 1, "2014-08-01", 1)]
        [InlineData(FundModels.Age16To19ExcludingApprenticeships, 1, 1, "2014-08-01", 0)]
        [InlineData(FundModels.Other16To19, 1, 1, "2014-08-01", 0)]
        public void InvalidItemRaisesValidationMessage(int fundModel, int? planHours, int? eepHours, string learnStart, int offSet)
        {
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
                .Returns(AimTypes.ProgrammeAim);
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
                    EmploymentStatusEmpStats.NotKnownProvided))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == PropertyNameConstants.LearnStartDate),
                    testDate))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var sut = new EmpStat_11Rule(handler.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
        }

        [Theory]
        [InlineData(FundModels.Age16To19ExcludingApprenticeships, 340, 200, "2014-08-01", 0)]
        [InlineData(FundModels.Other16To19, 340, 200, "2014-08-01", 0)]
        [InlineData(FundModels.Age16To19ExcludingApprenticeships, 1, 1, "2014-07-31", 0)]
        [InlineData(FundModels.Other16To19, 1, 1, "2014-07-31", 0)]
        [InlineData(FundModels.AdultSkills, 1, 1, "2014-08-01", 0)]
        [InlineData(FundModels.OtherAdult, 1, 1, "2014-08-01", 0)]
        [InlineData(FundModels.Age16To19ExcludingApprenticeships, 1, 1, "2014-08-01", -1)]
        [InlineData(FundModels.Other16To19, 1, 1, "2014-08-01", -1)]
        public void ValidItemDoesNotRaiseValidationMessage(int fundModel, int? planHours, int? eepHours, string learnStart, int offSet)
        {
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
                .Returns(AimTypes.ProgrammeAim);
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

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
        }

        public EmpStat_11Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            return new EmpStat_11Rule(handler.Object);
        }
    }
}
