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
    public class EmpStat_15RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("EmpStat_15", result);
        }

        [Fact]
        public void LastInviableDateMeetsExpectation()
        {
            var sut = NewRule();

            var result = sut.LastInviableDate;

            Assert.Equal(DateTime.Parse("2016-07-31"), result);
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

            var sut = new EmpStat_15Rule(handler.Object, mockDDRule07.Object);

            var result = sut.IsApprenticeship(mockItem.Object);

            Assert.Equal(expectation, result);
            handler.VerifyAll();
            mockDDRule07.VerifyAll();
        }

        [Theory]
        [InlineData(AimTypes.AimNotPartOfAProgramme, false)]
        [InlineData(AimTypes.ComponentAimInAProgramme, false)]
        [InlineData(AimTypes.CoreAim16To19ExcludingApprenticeships, false)]
        [InlineData(AimTypes.ProgrammeAim, true)]
        public void InAProgrammeMeetsExpectation(int candidate, bool expectation)
        {
            var sut = NewRule();
            var mockItem = new Mock<ILearningDelivery>();
            mockItem
                .SetupGet(y => y.AimType)
                .Returns(candidate);

            var result = sut.InAProgramme(mockItem.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData("2016-06-30", false)]
        [InlineData("2016-07-31", false)]
        [InlineData("2016-08-01", true)]
        [InlineData("2016-09-30", true)]
        public void IsQualifyingAimMeetsExpectation(string startDate, bool expectation)
        {
            var sut = NewRule();

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(startDate));

            var result = sut.IsQualifyingAim(mockDelivery.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(EmploymentStatusEmpStats.InPaidEmployment, true)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedNotSeekingOrNotAvailable, true)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedSeekingAndAvailable, true)]
        [InlineData(EmploymentStatusEmpStats.NotKnownProvided, false)]
        public void HasQualifyingEmploymentStatusMeetsExpectation(int candidate, bool expectation)
        {
            var sut = NewRule();

            var mockStatus = new Mock<ILearnerEmploymentStatus>();
            mockStatus
                .SetupGet(y => y.EmpStat)
                .Returns(candidate);

            var result = sut.HasQualifyingEmploymentStatus(mockStatus.Object);

            Assert.Equal(expectation, result);
        }

        [Fact]
        public void HasQualifyingEmploymentStatusWithNullStatusesReturnsTrue()
        {
            var sut = NewRule();

            var mockItem = new Mock<ILearner>();

            var result = sut.HasQualifyingEmploymentStatus(mockItem.Object, null);

            Assert.True(result);
        }

        [Fact]
        public void HasQualifyingEmploymentStatusWithEmptyStatusesReturnsTrue()
        {
            var sut = NewRule();

            var mockItem = new Mock<ILearner>();
            mockItem
                .SetupGet(x => x.LearnerEmploymentStatuses)
                .Returns(new List<ILearnerEmploymentStatus>());

            var result = sut.HasQualifyingEmploymentStatus(mockItem.Object, null);

            Assert.True(result);
        }

        [Fact]
        public void InvalidItemRaisesValidationMessage()
        {
            const string LearnRefNumber = "123456789X";

            var testDate = DateTime.Parse("2016-09-24");

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(testDate);
            mockDelivery
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(ProgTypes.ApprenticeshipStandard);
            mockDelivery
                .SetupGet(y => y.AimType)
                .Returns(AimTypes.ProgrammeAim);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockStatus = new Mock<ILearnerEmploymentStatus>();
            mockStatus
                .SetupGet(y => y.EmpStat)
                .Returns(EmploymentStatusEmpStats.NotKnownProvided);
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

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(
                    Moq.It.Is<string>(y => y == EmpStat_15Rule.Name),
                    Moq.It.Is<string>(y => y == LearnRefNumber),
                    0,
                    Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == EmpStat_15Rule.MessagePropertyName),
                    EmploymentStatusEmpStats.NotKnownProvided))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == PropertyNameConstants.LearnStartDate),
                    testDate))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            mockDDRule07
                .Setup(x => x.IsApprenticeship(Moq.It.IsAny<int>()))
                .Returns(true);

            var sut = new EmpStat_15Rule(handler.Object, mockDDRule07.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            mockDDRule07.VerifyAll();
        }

        [Fact]
        public void ValidItemDoesNotRaiseValidationMessage()
        {
            const string LearnRefNumber = "123456789X";

            var testDate = DateTime.Parse("2016-09-24");

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(testDate);
            mockDelivery
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(ProgTypes.ApprenticeshipStandard);
            mockDelivery
                .SetupGet(y => y.AimType)
                .Returns(AimTypes.ProgrammeAim);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockStatus = new Mock<ILearnerEmploymentStatus>();
            mockStatus
                .SetupGet(y => y.EmpStat)
                .Returns(EmploymentStatusEmpStats.InPaidEmployment);
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

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            mockDDRule07
                .Setup(x => x.IsApprenticeship(Moq.It.IsAny<int>()))
                .Returns(true);

            var sut = new EmpStat_15Rule(handler.Object, mockDDRule07.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            mockDDRule07.VerifyAll();
        }

        public EmpStat_15Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);

            return new EmpStat_15Rule(handler.Object, mockDDRule07.Object);
        }
    }
}
