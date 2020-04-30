using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpStat;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.EmploymentStatus.EmpStat
{
    public class EmpStat_18RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("EmpStat_18", result);
        }

        [Fact]
        public void OldCodeMonitoringThresholdDateMeetsExpectation()
        {
            Assert.Equal(DateTime.Parse("2018-07-31"), EmpStat_18Rule.OldCodeMonitoringThresholdDate);
        }

        [Fact]
        public void IsQualifyingPrimaryLearningAimWithNullReturnsFalse()
        {
            var sut = NewRule();

            var result = sut.IsQualifyingPrimaryLearningAim(null);

            Assert.False(result);
        }

        [Fact]
        public void IsQualifyingPrimaryLearningAimWithInvalidDateReturnsFalse()
        {
            var mockDelivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTimeQS
                .Setup(x => x.IsDateBetween(mockDelivery.Object.LearnStartDate, DateTime.MinValue, EmpStat_18Rule.OldCodeMonitoringThresholdDate, true))
                .Returns(false);

            var lEmpQS = new Mock<ILearnerEmploymentStatusQueryService>(MockBehavior.Strict);

            var sut = new EmpStat_18Rule(handler.Object, dateTimeQS.Object, lEmpQS.Object);

            var result = sut.IsQualifyingPrimaryLearningAim(mockDelivery.Object);

            Assert.False(result);

            handler.VerifyAll();
            dateTimeQS.VerifyAll();
        }

        [Fact]
        public void IsQualifyingPrimaryLearningAimNotTraineeReturnsFalse()
        {
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(ProgTypes.Traineeship);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTimeQS
                .Setup(x => x.IsDateBetween(mockDelivery.Object.LearnStartDate, DateTime.MinValue, DateTime.Parse("2018-07-31"), true))
                .Returns(true);

            var lEmpQS = new Mock<ILearnerEmploymentStatusQueryService>(MockBehavior.Strict);

            var sut = new EmpStat_18Rule(handler.Object, dateTimeQS.Object, lEmpQS.Object);

            var result = sut.IsQualifyingPrimaryLearningAim(mockDelivery.Object);

            Assert.False(result);

            handler.VerifyAll();
            dateTimeQS.VerifyAll();
        }

        [Fact]
        public void IsQualifyingPrimaryLearningAimWithWrongAimTypeReturnsFalse()
        {
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.AimType)
                .Returns(AimTypes.AimNotPartOfAProgramme);
            mockDelivery
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(ProgTypes.Traineeship);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTimeQS
                .Setup(x => x.IsDateBetween(mockDelivery.Object.LearnStartDate, DateTime.MinValue, DateTime.Parse("2018-07-31"), true))
                .Returns(true);

            var lEmpQS = new Mock<ILearnerEmploymentStatusQueryService>(MockBehavior.Strict);

            var sut = new EmpStat_18Rule(handler.Object, dateTimeQS.Object, lEmpQS.Object);

            var result = sut.IsQualifyingPrimaryLearningAim(mockDelivery.Object);

            Assert.False(result);

            handler.VerifyAll();
            dateTimeQS.VerifyAll();
        }

        [Fact]
        public void IsQualifyingPrimaryLearningAimPassingChecksReturnsTrue()
        {
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.AimType)
                .Returns(AimTypes.ProgrammeAim);
            mockDelivery
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(ProgTypes.Traineeship);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTimeQS
                .Setup(x => x.IsDateBetween(mockDelivery.Object.LearnStartDate, DateTime.MinValue, DateTime.Parse("2018-07-31"), true))
                .Returns(true);

            var lEmpQS = new Mock<ILearnerEmploymentStatusQueryService>(MockBehavior.Strict);

            var sut = new EmpStat_18Rule(handler.Object, dateTimeQS.Object, lEmpQS.Object);

            var result = sut.IsQualifyingPrimaryLearningAim(mockDelivery.Object);

            Assert.True(result);

            handler.VerifyAll();
            dateTimeQS.VerifyAll();
        }

        [Fact]
        public void HasQualifyingEmploymentStatusWithNullReturnsFalse()
        {
            var sut = NewRule();

            var result = sut.HasQualifyingEmploymentStatus(null);

            Assert.False(result);
        }

        [Theory]
        [InlineData(10, true)]
        [InlineData(11, false)]
        [InlineData(12, false)]
        [InlineData(98, false)]
        public void HasQualifyingEmploymentStatusMeetsExpectation(int candidate, bool expectation)
        {
            var sut = NewRule();

            var mockItem = new Mock<ILearnerEmploymentStatus>();
            mockItem
                .SetupGet(x => x.EmpStat)
                .Returns(candidate);

            var result = sut.HasQualifyingEmploymentStatus(mockItem.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor16HoursOrMorePW, true)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor16To19HoursPW, true)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor20HoursOrMorePW, true)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor0To10HourPW, true)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor11To20HoursPW, true)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor21To30HoursPW, true)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor31PlusHoursPW, true)]
        [InlineData(Monitoring.EmploymentStatus.EmployedForLessThan16HoursPW, false)]
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
        public void HasDisqualifyingMonitorMeetsExpectation(string candidate, bool expectation)
        {
            var sut = NewRule();
            var mockItem = new Mock<IEmploymentStatusMonitoring>();
            mockItem
                .SetupGet(y => y.ESMType)
                .Returns(candidate.Substring(0, 3));
            mockItem
                .SetupGet(y => y.ESMCode)
                .Returns(int.Parse(candidate.Substring(3)));

            var result = sut.HasDisqualifyingMonitor(mockItem.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor16HoursOrMorePW)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor16To19HoursPW)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor20HoursOrMorePW)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor0To10HourPW)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor11To20HoursPW)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor21To30HoursPW)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor31PlusHoursPW)]
        public void InvalidItemRaisesValidationMessage(string candidate)
        {
            const string LearnRefNumber = "123456789X";
            const int AimSeqNumber = 1;

            var testDate = DateTime.Parse("2018-07-31");

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(testDate);
            mockDelivery
                .SetupGet(x => x.AimType)
                .Returns(AimTypes.ProgrammeAim);
            mockDelivery
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(ProgTypes.Traineeship);
            mockDelivery
                .SetupGet(x => x.AimSeqNumber)
                .Returns(AimSeqNumber);

            var deliveries = new ILearningDelivery[] { mockDelivery.Object };

            var esmType = candidate.Substring(0, 3);
            var esmCode = int.Parse(candidate.Substring(3));

            var mockItem = new Mock<IEmploymentStatusMonitoring>(MockBehavior.Strict);
            mockItem
                .SetupGet(y => y.ESMType)
                .Returns(esmType);
            mockItem
                .SetupGet(y => y.ESMCode)
                .Returns(esmCode);

            var monitors = new IEmploymentStatusMonitoring[] { mockItem.Object };

            var mockStatus = new Mock<ILearnerEmploymentStatus>();
            mockStatus
                .SetupGet(y => y.DateEmpStatApp)
                .Returns(testDate);
            mockStatus
                .SetupGet(y => y.EmpStat)
                .Returns(EmploymentStatusEmpStats.InPaidEmployment);
            mockStatus
                .SetupGet(y => y.EmploymentStatusMonitorings)
                .Returns(monitors);

            var employmentStatuses = new ILearnerEmploymentStatus[] { mockStatus.Object };

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);
            mockLearner
                .SetupGet(x => x.LearnerEmploymentStatuses)
                .Returns(employmentStatuses);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle("EmpStat_18", LearnRefNumber, AimSeqNumber, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("ESMType", esmType))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("ESMCode", esmCode))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTimeQS
                .Setup(x => x.IsDateBetween(mockDelivery.Object.LearnStartDate, DateTime.MinValue, EmpStat_18Rule.OldCodeMonitoringThresholdDate, true))
                .Returns(true);

            var lEmpQS = new Mock<ILearnerEmploymentStatusQueryService>(MockBehavior.Strict);
            lEmpQS
               .Setup(x => x.LearnerEmploymentStatusForDate(employmentStatuses, testDate))
               .Returns(mockStatus.Object);

            var sut = new EmpStat_18Rule(handler.Object, dateTimeQS.Object, lEmpQS.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            dateTimeQS.VerifyAll();
        }

        [Theory]
        [InlineData(Monitoring.EmploymentStatus.EmployedForLessThan16HoursPW)]
        [InlineData(Monitoring.EmploymentStatus.EmployedForUpTo3M)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor4To6M)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor7To12M)]
        [InlineData(Monitoring.EmploymentStatus.EmployedForMoreThan12M)]
        [InlineData(Monitoring.EmploymentStatus.InFulltimeEducationOrTrainingPriorToEnrolment)]
        [InlineData(Monitoring.EmploymentStatus.InReceiptOfAnotherStateBenefit)]
        [InlineData(Monitoring.EmploymentStatus.InReceiptOfEmploymentAndSupportAllowance)]
        [InlineData(Monitoring.EmploymentStatus.InReceiptOfJobSeekersAllowance)]
        [InlineData(Monitoring.EmploymentStatus.InReceiptOfUniversalCredit)]
        [InlineData(Monitoring.EmploymentStatus.SelfEmployed)]
        [InlineData(Monitoring.EmploymentStatus.SmallEmployer)]
        [InlineData(Monitoring.EmploymentStatus.UnemployedForLessThan6M)]
        [InlineData(Monitoring.EmploymentStatus.UnemployedFor6To11M)]
        [InlineData(Monitoring.EmploymentStatus.UnemployedFor12To23M)]
        [InlineData(Monitoring.EmploymentStatus.UnemployedFor24To35M)]
        [InlineData(Monitoring.EmploymentStatus.UnemployedFor36MPlus)]
        public void ValidItemDoesNotRaiseValidationMessage(string candidate)
        {
            const string LearnRefNumber = "123456789X";
            const int AimSeqNumber = 1;

            var testDate = DateTime.Parse("2018-07-31");

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(testDate);
            mockDelivery
                .SetupGet(x => x.AimType)
                .Returns(AimTypes.ProgrammeAim);
            mockDelivery
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(ProgTypes.Traineeship);
            mockDelivery
                .SetupGet(x => x.AimSeqNumber)
                .Returns(AimSeqNumber);

            var deliveries = new ILearningDelivery[] { mockDelivery.Object };

            var mockItem = new Mock<IEmploymentStatusMonitoring>(MockBehavior.Strict);
            mockItem
                .SetupGet(y => y.ESMType)
                .Returns(candidate.Substring(0, 3));
            mockItem
                .SetupGet(y => y.ESMCode)
                .Returns(int.Parse(candidate.Substring(3)));

            var monitors = new IEmploymentStatusMonitoring[] { mockItem.Object };

            var mockStatus = new Mock<ILearnerEmploymentStatus>();
            mockStatus
                .SetupGet(y => y.DateEmpStatApp)
                .Returns(testDate);
            mockStatus
                .SetupGet(y => y.EmpStat)
                .Returns(EmploymentStatusEmpStats.InPaidEmployment);
            mockStatus
                .SetupGet(y => y.EmploymentStatusMonitorings)
                .Returns(monitors);

            var employmentStatuses = new ILearnerEmploymentStatus[] { mockStatus.Object };

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);
            mockLearner
                .SetupGet(x => x.LearnerEmploymentStatuses)
                .Returns(employmentStatuses);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTimeQS
                .Setup(x => x.IsDateBetween(mockDelivery.Object.LearnStartDate, DateTime.MinValue, EmpStat_18Rule.OldCodeMonitoringThresholdDate, true))
                .Returns(true);

            var lEmpQS = new Mock<ILearnerEmploymentStatusQueryService>(MockBehavior.Strict);
            lEmpQS
               .Setup(x => x.LearnerEmploymentStatusForDate(employmentStatuses, testDate))
               .Returns(mockStatus.Object);

            var sut = new EmpStat_18Rule(handler.Object, dateTimeQS.Object, lEmpQS.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
        }

        public EmpStat_18Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            var lEmpQS = new Mock<ILearnerEmploymentStatusQueryService>(MockBehavior.Strict);

            return new EmpStat_18Rule(handler.Object, dateTimeQS.Object, lEmpQS.Object);
        }
    }
}
