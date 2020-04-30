using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using Moq;
using System;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Derived
{
    public class DerivedData_28RuleTests
    {
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
        [InlineData(Monitoring.EmploymentStatus.InReceiptOfEmploymentAndSupportAllowance, true)]
        [InlineData(Monitoring.EmploymentStatus.InReceiptOfJobSeekersAllowance, true)]
        [InlineData(Monitoring.EmploymentStatus.InReceiptOfUniversalCredit, false)]
        [InlineData(Monitoring.EmploymentStatus.SelfEmployed, false)]
        [InlineData(Monitoring.EmploymentStatus.SmallEmployer, false)]
        [InlineData(Monitoring.EmploymentStatus.UnemployedForLessThan6M, false)]
        [InlineData(Monitoring.EmploymentStatus.UnemployedFor6To11M, false)]
        [InlineData(Monitoring.EmploymentStatus.UnemployedFor12To23M, false)]
        [InlineData(Monitoring.EmploymentStatus.UnemployedFor24To35M, false)]
        [InlineData(Monitoring.EmploymentStatus.UnemployedFor36MPlus, false)]
        public void InReceiptOfEmploymentSupportMeetsExpectation(string candidate, bool expectation)
        {
            var sut = NewRule();
            var mockItem = new Mock<IEmploymentStatusMonitoring>(MockBehavior.Strict);
            mockItem
                .SetupGet(y => y.ESMType)
                .Returns(candidate.Substring(0, 3));
            mockItem
                .SetupGet(y => y.ESMCode)
                .Returns(int.Parse(candidate.Substring(3)));

            var result = sut.InReceiptOfEmploymentSupport(mockItem.Object);

            Assert.Equal(expectation, result);
            mockItem.VerifyAll();
        }

        [Fact]
        public void InReceiptOfEmploymentSupportWithEmptyMonitorsMeetsExpectation()
        {
            var sut = NewRule();

            var result = sut.InReceiptOfEmploymentSupport(new IEmploymentStatusMonitoring[] { });

            Assert.False(result);
        }

        [Fact]
        public void InReceiptOfEmploymentSupportWithNullMonitorsMeetsExpectation()
        {
            var sut = NewRule();

            var result = sut.InReceiptOfEmploymentSupport((IEmploymentStatusMonitoring[])null);

            Assert.False(result);
        }

        [Theory]
        [InlineData(EmploymentStatusEmpStats.NotEmployedNotSeekingOrNotAvailable, true)]
        [InlineData(EmploymentStatusEmpStats.InPaidEmployment, true)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedSeekingAndAvailable, true)]
        [InlineData(EmploymentStatusEmpStats.NotKnownProvided, true)]
        public void HasValidEmploymentStatusMeetsExpectation(int candidate, bool expectation)
        {
            var sut = NewRule();
            var mockDelivery = new Mock<ILearnerEmploymentStatus>();
            mockDelivery
                .SetupGet(y => y.EmpStat)
                .Returns(candidate);

            var result = sut.HasValidEmploymentStatus(mockDelivery.Object);

            Assert.Equal(expectation, result);
        }

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
        [InlineData(Monitoring.EmploymentStatus.InReceiptOfUniversalCredit, true)]
        [InlineData(Monitoring.EmploymentStatus.SelfEmployed, false)]
        [InlineData(Monitoring.EmploymentStatus.SmallEmployer, false)]
        [InlineData(Monitoring.EmploymentStatus.UnemployedForLessThan6M, false)]
        [InlineData(Monitoring.EmploymentStatus.UnemployedFor6To11M, false)]
        [InlineData(Monitoring.EmploymentStatus.UnemployedFor12To23M, false)]
        [InlineData(Monitoring.EmploymentStatus.UnemployedFor24To35M, false)]
        [InlineData(Monitoring.EmploymentStatus.UnemployedFor36MPlus, false)]
        public void InReceiptOfCreditsMeetsExpectation(string candidate, bool expectation)
        {
            var sut = NewRule();
            var mockItem = new Mock<IEmploymentStatusMonitoring>(MockBehavior.Strict);
            mockItem
                .SetupGet(y => y.ESMType)
                .Returns(candidate.Substring(0, 3));
            mockItem
                .SetupGet(y => y.ESMCode)
                .Returns(int.Parse(candidate.Substring(3)));

            var result = sut.InReceiptOfCredits(mockItem.Object);

            Assert.Equal(expectation, result);
            mockItem.VerifyAll();
        }

        [Fact]
        public void InReceiptOfCreditsWithEmptyMonitorsMeetsExpectation()
        {
            var sut = NewRule();

            var result = sut.InReceiptOfCredits(new IEmploymentStatusMonitoring[] { });

            Assert.False(result);
        }

        [Fact]
        public void InReceiptOfCreditsWithNullMonitorsMeetsExpectation()
        {
            var sut = NewRule();

            var result = sut.InReceiptOfCredits((IEmploymentStatusMonitoring[])null);

            Assert.False(result);
        }

        [Theory]
        [InlineData(Monitoring.EmploymentStatus.EmployedForLessThan16HoursPW, true)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor16HoursOrMorePW, false)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor16To19HoursPW, false)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor20HoursOrMorePW, false)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor0To10HourPW, true)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor11To20HoursPW, true)]
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
        [InlineData(Monitoring.EmploymentStatus.InReceiptOfUniversalCredit, false)]
        [InlineData(Monitoring.EmploymentStatus.SelfEmployed, false)]
        [InlineData(Monitoring.EmploymentStatus.SmallEmployer, false)]
        [InlineData(Monitoring.EmploymentStatus.UnemployedForLessThan6M, false)]
        [InlineData(Monitoring.EmploymentStatus.UnemployedFor6To11M, false)]
        [InlineData(Monitoring.EmploymentStatus.UnemployedFor12To23M, false)]
        [InlineData(Monitoring.EmploymentStatus.UnemployedFor24To35M, false)]
        [InlineData(Monitoring.EmploymentStatus.UnemployedFor36MPlus, false)]
        public void IsWorkingShortHoursMeetsExpectation(string candidate, bool expectation)
        {
            var sut = NewRule();
            var mockItem = new Mock<IEmploymentStatusMonitoring>(MockBehavior.Strict);
            mockItem
                .SetupGet(y => y.ESMType)
                .Returns(candidate.Substring(0, 3));
            mockItem
                .SetupGet(y => y.ESMCode)
                .Returns(int.Parse(candidate.Substring(3)));

            var result = sut.IsWorkingShortHours(mockItem.Object);

            Assert.Equal(expectation, result);
            mockItem.VerifyAll();
        }

        [Fact]
        public void IsWorkingShortHoursWithEmptyMonitorsMeetsExpectation()
        {
            var sut = NewRule();

            var result = sut.IsWorkingShortHours(new IEmploymentStatusMonitoring[] { });

            Assert.False(result);
        }

        [Fact]
        public void IsWorkingShortHoursWithNullMonitorsMeetsExpectation()
        {
            var sut = NewRule();

            var result = sut.IsWorkingShortHours((IEmploymentStatusMonitoring[])null);

            Assert.False(result);
        }

        [Theory]
        [InlineData(EmploymentStatusEmpStats.NotEmployedNotSeekingOrNotAvailable, false)]
        [InlineData(EmploymentStatusEmpStats.InPaidEmployment, true)]
        [InlineData(EmploymentStatusEmpStats.NotEmployedSeekingAndAvailable, false)]
        [InlineData(EmploymentStatusEmpStats.NotKnownProvided, false)]
        public void IsEmployedMeetsExpectation(int candidate, bool expectation)
        {
            var sut = NewRule();
            var mockDelivery = new Mock<ILearnerEmploymentStatus>();
            mockDelivery
                .SetupGet(y => y.EmpStat)
                .Returns(candidate);

            var result = sut.IsEmployed(mockDelivery.Object);

            Assert.Equal(expectation, result);
        }

        public DerivedData_28Rule NewRule()
        {
            var lEmpQS = new Mock<ILearnerEmploymentStatusQueryService>(MockBehavior.Strict);

            return new DerivedData_28Rule(lEmpQS.Object);
        }
    }
}
