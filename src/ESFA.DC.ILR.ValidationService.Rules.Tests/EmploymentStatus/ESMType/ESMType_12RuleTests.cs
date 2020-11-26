using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.ESMType;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.EmploymentStatus.ESMType
{
    public class ESMType_12RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("ESMType_12", result);
        }

        [Fact]
        public void FirstViableDateMeetsExpectation()
        {
            Assert.Equal(DateTime.Parse("2013-08-01"), ESMType_12Rule.FirstViableDate);
        }

        [Theory]
        [InlineData(EmploymentStatusEmpStats.InPaidEmployment, false)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedNotSeekingOrNotAvailable, true)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedSeekingAndAvailable, true)]
        [InlineData(EmploymentStatusEmpStats.NotKnownProvided, false)]
        public void IsQualifyingEmploymentMeetsExpectation(int candidate, bool expectation)
        {
            var sut = NewRule();
            var mockItem = new Mock<ILearnerEmploymentStatus>();
            mockItem
                .SetupGet(y => y.EmpStat)
                .Returns(candidate);

            var result = sut.IsQualifyingEmployment(mockItem.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor0To10HourPW, true)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor11To20HoursPW, true)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor16HoursOrMorePW, true)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor16To19HoursPW, true)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor20HoursOrMorePW, true)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor21To30HoursPW, true)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor31PlusHoursPW, true)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor4To6M, false)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor7To12M, false)]
        [InlineData(Monitoring.EmploymentStatus.EmployedForLessThan16HoursPW, true)]
        [InlineData(Monitoring.EmploymentStatus.EmployedForMoreThan12M, false)]
        [InlineData(Monitoring.EmploymentStatus.EmployedForUpTo3M, false)]
        [InlineData(Monitoring.EmploymentStatus.InFulltimeEducationOrTrainingPriorToEnrolment, false)]
        [InlineData(Monitoring.EmploymentStatus.InReceiptOfAnotherStateBenefit, false)]
        [InlineData(Monitoring.EmploymentStatus.InReceiptOfEmploymentAndSupportAllowance, false)]
        [InlineData(Monitoring.EmploymentStatus.InReceiptOfJobSeekersAllowance, false)]
        [InlineData(Monitoring.EmploymentStatus.InReceiptOfUniversalCredit, false)]
        [InlineData(Monitoring.EmploymentStatus.SelfEmployed, true)]
        [InlineData(Monitoring.EmploymentStatus.SmallEmployer, false)]
        [InlineData(Monitoring.EmploymentStatus.UnemployedFor12To23M, false)]
        [InlineData(Monitoring.EmploymentStatus.UnemployedFor24To35M, false)]
        [InlineData(Monitoring.EmploymentStatus.UnemployedFor36MPlus, false)]
        [InlineData(Monitoring.EmploymentStatus.UnemployedFor6To11M, false)]
        [InlineData(Monitoring.EmploymentStatus.UnemployedForLessThan6M, false)]
        public void HasDisqualifyingIndicatorMeetsExpectation(string candidate, bool expectation)
        {
            var sut = NewRule();
            var mockItem = new Mock<IEmploymentStatusMonitoring>();
            mockItem
                .SetupGet(y => y.ESMType)
                .Returns(candidate.Substring(0, 3));
            mockItem
                .SetupGet(y => y.ESMCode)
                .Returns(int.Parse(candidate.Substring(3)));

            var result = sut.HasDisqualifyingIndicator(mockItem.Object);

            Assert.Equal(expectation, result);
        }

        [Fact]
        public void HasDisqualifyingIndicatorWithNullMonitoringsReturnsFalse()
        {
            var sut = NewRule();
            var mockItem = new Mock<ILearnerEmploymentStatus>();

            var result = sut.HasDisqualifyingIndicator(mockItem.Object);

            Assert.False(result);
        }

        [Fact]
        public void HasQualifyingIndicatorWithEmptyMonitoringsReturnsFalse()
        {
            var sut = NewRule();

            var monitorings = new List<IEmploymentStatusMonitoring>();
            var mockItem = new Mock<ILearnerEmploymentStatus>();
            mockItem
                .SetupGet(x => x.EmploymentStatusMonitorings)
                .Returns(monitorings);

            var result = sut.HasDisqualifyingIndicator(mockItem.Object);

            Assert.False(result);
        }

        [Theory]
        [InlineData(EmploymentStatusEmpStats.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.SelfEmployed)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.EmployedFor0To10HourPW)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.EmployedFor11To20HoursPW)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.EmployedFor16HoursOrMorePW)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.EmployedFor16To19HoursPW)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.EmployedFor20HoursOrMorePW)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.EmployedFor21To30HoursPW)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.EmployedFor31PlusHoursPW)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.EmployedForLessThan16HoursPW)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.SelfEmployed)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.EmployedFor0To10HourPW)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.EmployedFor11To20HoursPW)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.EmployedFor16HoursOrMorePW)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.EmployedFor16To19HoursPW)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.EmployedFor20HoursOrMorePW)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.EmployedFor21To30HoursPW)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.EmployedFor31PlusHoursPW)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.EmployedForLessThan16HoursPW)]
        public void InvalidItemRaisesValidationMessage(int empStat, string candidate)
        {
            const string LearnRefNumber = "123456789X";

            var monitor = new Mock<IEmploymentStatusMonitoring>();
            monitor
                .SetupGet(y => y.ESMType)
                .Returns(candidate.Substring(0, 3));
            monitor
                .SetupGet(y => y.ESMCode)
                .Returns(int.Parse(candidate.Substring(3)));

            var monitorings = new List<IEmploymentStatusMonitoring>();
            monitorings.Add(monitor.Object);

            var testDate = DateTime.Parse("2013-08-01");

            var status = new Mock<ILearnerEmploymentStatus>();
            status
                .SetupGet(x => x.DateEmpStatApp)
                .Returns(testDate);
            status
                .SetupGet(x => x.EmpStat)
                .Returns(empStat);
            status
                .SetupGet(x => x.EmploymentStatusMonitorings)
                .Returns(monitorings);

            var statii = new List<ILearnerEmploymentStatus>();
            statii.Add(status.Object);

            var learnStart = DateTime.Parse("2016-09-24");
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(learnStart);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);
            mockLearner
                .SetupGet(y => y.LearnerEmploymentStatuses)
                .Returns(statii);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(ESMType_12Rule.Name, LearnRefNumber, null, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter(ESMType_12Rule.MessagePropertyName, "Invalid"))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("EmpStat", empStat))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("DateEmpStatApp", testDate))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTimeQS
                .Setup(x => x.IsDateBetween(status.Object.DateEmpStatApp, ESMType_12Rule.FirstViableDate, DateTime.MaxValue, true))
                .Returns(true);

            NewRule(handler.Object, dateTimeQS.Object).Validate(mockLearner.Object);

            handler.VerifyAll();
            dateTimeQS.VerifyAll();
        }

        [Theory]
        [InlineData(EmploymentStatusEmpStats.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.InFulltimeEducationOrTrainingPriorToEnrolment)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.InReceiptOfAnotherStateBenefit)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.InReceiptOfEmploymentAndSupportAllowance)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.InReceiptOfJobSeekersAllowance)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.InReceiptOfUniversalCredit)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.EmployedFor4To6M)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.EmployedFor7To12M)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.EmployedForMoreThan12M)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.EmployedForUpTo3M)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.UnemployedFor12To23M)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.UnemployedFor24To35M)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.UnemployedFor36MPlus)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.UnemployedFor6To11M)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.UnemployedForLessThan6M)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.SmallEmployer)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.InFulltimeEducationOrTrainingPriorToEnrolment)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.InReceiptOfAnotherStateBenefit)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.InReceiptOfEmploymentAndSupportAllowance)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.InReceiptOfJobSeekersAllowance)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.InReceiptOfUniversalCredit)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.EmployedFor4To6M)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.EmployedFor7To12M)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.EmployedForMoreThan12M)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.EmployedForUpTo3M)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.UnemployedFor12To23M)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.UnemployedFor24To35M)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.UnemployedFor36MPlus)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.UnemployedFor6To11M)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.UnemployedForLessThan6M)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.SmallEmployer)]
        public void ValidItemDoesNotRaiseValidationMessage(int empStat, string candidate)
        {
            const string LearnRefNumber = "123456789X";

            var monitor = new Mock<IEmploymentStatusMonitoring>();
            monitor
                .SetupGet(y => y.ESMType)
                .Returns(candidate.Substring(0, 3));
            monitor
                .SetupGet(y => y.ESMCode)
                .Returns(int.Parse(candidate.Substring(3)));

            var monitorings = new List<IEmploymentStatusMonitoring>();
            monitorings.Add(monitor.Object);

            var testDate = DateTime.Parse("2013-08-01");

            var status = new Mock<ILearnerEmploymentStatus>();
            status
                .SetupGet(x => x.DateEmpStatApp)
                .Returns(testDate);
            status
                .SetupGet(x => x.EmpStat)
                .Returns(empStat);
            status
                .SetupGet(x => x.EmploymentStatusMonitorings)
                .Returns(monitorings);

            var statii = new List<ILearnerEmploymentStatus>();
            statii.Add(status.Object);

            var learnStart = DateTime.Parse("2016-09-24");
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(learnStart);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);
            mockLearner
                .SetupGet(y => y.LearnerEmploymentStatuses)
                .Returns(statii);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTimeQS
                .Setup(x => x.IsDateBetween(status.Object.DateEmpStatApp, ESMType_12Rule.FirstViableDate, DateTime.MaxValue, true))
                .Returns(true);

            NewRule(handler.Object, dateTimeQS.Object).Validate(mockLearner.Object);

            handler.VerifyAll();
            dateTimeQS.VerifyAll();
        }

        public ESMType_12Rule NewRule(
            IValidationErrorHandler handler = null,
            IDateTimeQueryService dateTimeQueryService = null)
        {
            return new ESMType_12Rule(handler, dateTimeQueryService);
        }
    }
}
