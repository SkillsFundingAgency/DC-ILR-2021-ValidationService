using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpStat;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.EmploymentStatus.EmpStat
{
    public class EmpStat_09RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("EmpStat_09", result);
        }

        [Fact]
        public void LastInviableDateMeetsExpectation()
        {
            var sut = NewRule();

            var result = sut.LastInviableDate;

            Assert.Equal(DateTime.Parse("2014-07-31"), result);
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

            var sut = new EmpStat_09Rule(handler.Object, mockDDRule07.Object);

            var result = sut.IsApprenticeship(mockItem.Object);

            Assert.Equal(expectation, result);
            handler.VerifyAll();
            mockDDRule07.VerifyAll();
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
        [InlineData("2014-01-01", false)]
        [InlineData("2014-07-31", false)]
        [InlineData("2014-08-01", true)]
        [InlineData("2014-12-31", true)]
        public void HasAViableStartMeetsExpectation(string startDate, bool expectation)
        {
            var sut = NewRule();

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(startDate));

            var result = sut.HasAViableStart(mockDelivery.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData("2018-04-18", "2018-03-10", true)]
        [InlineData("2018-04-18", "2018-04-17", true)]
        [InlineData("2018-04-18", "2018-04-18", false)]
        [InlineData("2018-04-18", "2018-04-19", false)]
        public void HasAQualifyingEmploymentStatusMeetsExpectation(string candidate, string startDate, bool expectation)
        {
            var sut = NewRule();

            var testDate = DateTime.Parse(candidate);
            var mockStatus = new Mock<ILearnerEmploymentStatus>();
            mockStatus
                .SetupGet(y => y.DateEmpStatApp)
                .Returns(DateTime.Parse(startDate));

            var result = sut.HasAQualifyingEmploymentStatus(mockStatus.Object, testDate);

            Assert.Equal(expectation, result);
        }

        [Fact]
        public void HasAQualifyingEmploymentStatusWithNullStatusesReturnsFalse()
        {
            var sut = NewRule();

            var mockItem = new Mock<ILearner>();

            var result = sut.HasAQualifyingEmploymentStatus(mockItem.Object, null);

            Assert.False(result);
        }

        [Fact]
        public void HasAQualifyingEmploymentStatusWithEmptyStatusesReturnsFalse()
        {
            var sut = NewRule();

            var mockItem = new Mock<ILearner>();
            mockItem
                .SetupGet(x => x.LearnerEmploymentStatuses)
                .Returns(new List<ILearnerEmploymentStatus>());

            var result = sut.HasAQualifyingEmploymentStatus(mockItem.Object, null);

            Assert.False(result);
        }

        [Theory]
        [InlineData("2014-08-01", 2)]
        [InlineData("2014-12-31", 2)]
        [InlineData("2014-08-01", 1)]
        [InlineData("2014-12-31", 1)]
        public void InvalidItemRaisesValidationMessage(string learnStart, int dateOffset)
        {
            const string LearnRefNumber = "123456789X";

            var testDate = DateTime.Parse(learnStart);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(testDate);
            mockDelivery
                .SetupGet(y => y.AimType)
                .Returns(AimTypes.ProgrammeAim);
            mockDelivery
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(ProgTypes.ApprenticeshipStandard);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockStatus = new Mock<ILearnerEmploymentStatus>();
            mockStatus
                .SetupGet(y => y.DateEmpStatApp)
                .Returns(testDate.AddDays(dateOffset));

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

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(
                    Moq.It.Is<string>(y => y == EmpStat_09Rule.Name),
                    Moq.It.Is<string>(y => y == LearnRefNumber),
                    0,
                    Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == EmpStat_09Rule.MessagePropertyName),
                    "(missing)"))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == PropertyNameConstants.LearnStartDate),
                    testDate))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            mockDDRule07
                .Setup(x => x.IsApprenticeship(ProgTypes.ApprenticeshipStandard))
                .Returns(true);

            var sut = new EmpStat_09Rule(handler.Object, mockDDRule07.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            mockDDRule07.VerifyAll();
        }

        [Theory]
        [InlineData("2014-07-31", 0)]
        [InlineData("2014-08-01", -1)]
        [InlineData("2014-12-31", -1)]
        public void ValidItemDoesNotRaiseValidationMessage(string learnStart, int dateOffset)
        {
            const string LearnRefNumber = "123456789X";

            var testDate = DateTime.Parse(learnStart);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(testDate);
            mockDelivery
                .SetupGet(y => y.AimType)
                .Returns(AimTypes.ProgrammeAim);
            mockDelivery
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(ProgTypes.ApprenticeshipStandard);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockStatus = new Mock<ILearnerEmploymentStatus>();
            mockStatus
                .SetupGet(y => y.DateEmpStatApp)
                .Returns(testDate.AddDays(dateOffset));

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

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            mockDDRule07
                .Setup(x => x.IsApprenticeship(ProgTypes.ApprenticeshipStandard))
                .Returns(true);

            var sut = new EmpStat_09Rule(handler.Object, mockDDRule07.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            mockDDRule07.VerifyAll();
        }

        public EmpStat_09Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);

            return new EmpStat_09Rule(handler.Object, mockDDRule07.Object);
        }
    }
}
