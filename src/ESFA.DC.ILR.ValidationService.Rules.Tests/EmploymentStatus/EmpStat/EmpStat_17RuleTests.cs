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
    public class EmpStat_17RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("EmpStat_17", result);
        }

        [Fact]
        public void LastInviableDateMeetsExpectation()
        {
            var sut = NewRule();

            var result = sut.LastInviableDate;

            Assert.Equal(DateTime.Parse("2016-07-31"), result);
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
        [InlineData("2015-08-01", false)]
        [InlineData("2016-07-31", false)]
        [InlineData("2016-08-01", true)]
        [InlineData("2016-09-14", true)]
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
        [InlineData(AimTypes.ProgrammeAim, true)]
        [InlineData(AimTypes.AimNotPartOfAProgramme, false)]
        [InlineData(AimTypes.ComponentAimInAProgramme, false)]
        [InlineData(AimTypes.CoreAim16To19ExcludingApprenticeships, false)]
        public void IsInAProgrammeMeetsExpectation(int candidate, bool expectation)
        {
            var sut = NewRule();
            var mockItem = new Mock<ILearningDelivery>();
            mockItem
                .SetupGet(y => y.AimType)
                .Returns(candidate);

            var result = sut.IsInAProgramme(mockItem.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(TypeOfEmploymentStatus.InPaidEmployment, true)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable, true)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable, true)]
        [InlineData(TypeOfEmploymentStatus.NotKnownProvided, false)]
        public void HasAQualifyingEmploymentStatusMeetsExpectation(int candidate, bool expectation)
        {
            var sut = NewRule();

            var mockStatus = new Mock<ILearnerEmploymentStatus>();
            mockStatus
                .SetupGet(y => y.EmpStat)
                .Returns(candidate);

            var result = sut.HasAQualifyingEmploymentStatus(mockStatus.Object);

            Assert.Equal(expectation, result);
        }

        [Fact]
        public void HasAQualifyingEmploymentStatusWithNullEmploymentReturnsTrue()
        {
            var sut = NewRule();

            var result = sut.HasAQualifyingEmploymentStatus(null);

            Assert.True(result);
        }

        [Fact]
        public void GetMatchingEmploymentWithNullsReturnsNull()
        {
            var sut = NewRule();

            var result = sut.GetMatchingEmployment(null, null);

            Assert.Null(result);
        }

        [Fact]
        public void GetMatchingEmploymentWithNullStatusesReturnsNull()
        {
            var sut = NewRule();

            var mockItem = new Mock<ILearningDelivery>();

            var result = sut.GetMatchingEmployment(mockItem.Object, null);

            Assert.Null(result);
        }

        [Fact]
        public void GetMatchingEmploymentWithEmptyStatusesReturnsNull()
        {
            var sut = NewRule();

            var mockItem = new Mock<ILearningDelivery>();

            var result = sut.GetMatchingEmployment(mockItem.Object, new List<ILearnerEmploymentStatus>());

            Assert.Null(result);
        }

        [Fact]
        public void InvalidItemRaisesValidationMessage()
        {
            const string LearnRefNumber = "123456789X";
            const int AimSeqNumber = 1;

            var deliveries = new List<ILearningDelivery>();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(DateTime.Today);
            mockDelivery
                .SetupGet(x => x.AimType)
                .Returns(AimTypes.ProgrammeAim);
            mockDelivery
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(TypeOfLearningProgramme.Traineeship);
            mockDelivery
                .SetupGet(x => x.AimSeqNumber)
                .Returns(AimSeqNumber);
            deliveries.Add(mockDelivery.Object);

            var mockEmpStat = new Mock<ILearnerEmploymentStatus>();
            mockEmpStat
                .SetupGet(x => x.EmpStat)
                .Returns(TypeOfEmploymentStatus.NotKnownProvided);

            var statii = new List<ILearnerEmploymentStatus>
            {
                mockEmpStat.Object
            };

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

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(
                    Moq.It.Is<string>(y => y == EmpStat_17Rule.Name),
                    Moq.It.Is<string>(y => y == LearnRefNumber),
                    AimSeqNumber,
                    Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == EmpStat_17Rule.MessagePropertyName),
                    TypeOfEmploymentStatus.NotKnownProvided))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == PropertyNameConstants.LearnStartDate),
                    DateTime.Today))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var sut = new EmpStat_17Rule(handler.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
        }

        [Fact]
        public void ValidItemDoesNotRaiseValidationMessage()
        {
            const string LearnRefNumber = "123456789X";
            const int AimSeqNumber = 1;

            var deliveries = new List<ILearningDelivery>();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(DateTime.Today);
            mockDelivery
                .SetupGet(x => x.AimType)
                .Returns(AimTypes.ProgrammeAim);
            mockDelivery
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(TypeOfLearningProgramme.Traineeship);
            mockDelivery
                .SetupGet(x => x.AimSeqNumber)
                .Returns(AimSeqNumber);
            deliveries.Add(mockDelivery.Object);

            var statii = new List<ILearnerEmploymentStatus>();

            var mockStatus = new Mock<ILearnerEmploymentStatus>();
            mockStatus
                .SetupGet(y => y.DateEmpStatApp)
                .Returns(DateTime.Today.AddDays(-1));
            mockStatus
                .SetupGet(y => y.EmpStat)
                .Returns(TypeOfEmploymentStatus.InPaidEmployment);
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

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var sut = new EmpStat_17Rule(handler.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
        }

        public EmpStat_17Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            return new EmpStat_17Rule(handler.Object);
        }
    }
}
