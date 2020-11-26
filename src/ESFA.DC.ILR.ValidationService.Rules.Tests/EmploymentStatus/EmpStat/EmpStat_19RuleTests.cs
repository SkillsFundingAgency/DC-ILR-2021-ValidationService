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
    public class EmpStat_19RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("EmpStat_19", result);
        }

        [Fact]
        public void NewCodeMonitoringThresholdDateMeetsExpectation()
        {
            Assert.Equal(DateTime.Parse("2018-08-01"), EmpStat_19Rule.NewCodeMonitoringThresholdDate);
        }

        [Theory]
        [InlineData("2016-04-05")]
        [InlineData("2016-05-10")]
        [InlineData("2016-06-15")]
        [InlineData("2016-07-20")]
        [InlineData("2016-08-25")]
        public void GetEmploymentStatusOnMeetsExpectation(string candidate)
        {
            var testDate = DateTime.Parse(candidate);
            var employments = new List<ILearnerEmploymentStatus>();
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            var lEmpQS = new Mock<ILearnerEmploymentStatusQueryService>(MockBehavior.Strict);
            lEmpQS
               .Setup(x => x.LearnerEmploymentStatusForDate(employments, testDate))
               .Returns((ILearnerEmploymentStatus)null);

            var sut = new EmpStat_19Rule(handler.Object, dateTimeQS.Object, lEmpQS.Object);

            var result = sut.GetEmploymentStatusOn(testDate, employments);

            Assert.Null(result);
            handler.VerifyAll();
            dateTimeQS.VerifyAll();
        }

        [Theory]
        [InlineData("EII1", true)]
        [InlineData("EII2", true)]
        [InlineData("EII3", true)]
        [InlineData("EII4", true)]
        [InlineData("EII5", false)]
        [InlineData("EII6", false)]
        [InlineData("EII7", true)]
        [InlineData("EII8", true)]
        [InlineData("LOE1", false)]
        [InlineData("LOE2", false)]
        [InlineData("LOE3", false)]
        [InlineData("LOE4", false)]
        [InlineData("PEI1", false)]
        [InlineData("BSI3", false)]
        [InlineData("BSI2", false)]
        [InlineData("BSI1", false)]
        [InlineData("BSI4", false)]
        [InlineData("SEI1", false)]
        [InlineData("SEM1", false)]
        [InlineData("LOU1", false)]
        [InlineData("LOU2", false)]
        [InlineData("LOU3", false)]
        [InlineData("LOU4", false)]
        [InlineData("LOU5", false)]
        public void HasADisqualifyingMonitorStatusMeetsExpectation(string candidate, bool expectation)
        {
            var sut = NewRule();
            var mockItem = new Mock<IEmploymentStatusMonitoring>();
            mockItem
                .SetupGet(y => y.ESMType)
                .Returns(candidate.Substring(0, 3));
            mockItem
                .SetupGet(y => y.ESMCode)
                .Returns(int.Parse(candidate.Substring(3)));

            var result = sut.HasADisqualifyingMonitorStatus(mockItem.Object);

            Assert.Equal(expectation, result);
        }

        [Fact]
        public void CheckEmploymentStatusMeetsExpectation()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            var lEmpQS = new Mock<ILearnerEmploymentStatusQueryService>(MockBehavior.Strict);

            var sut = new EmpStat_19Rule(handler.Object, dateTimeQS.Object, lEmpQS.Object);

            sut.CheckEmploymentStatus(null, null);

            handler.VerifyAll();
            dateTimeQS.VerifyAll();
        }

        [Theory]
        [InlineData("EII1")]
        [InlineData("EII2")]
        [InlineData("EII3")]
        [InlineData("EII4")]
        [InlineData("EII7")]
        [InlineData("EII8")]
        public void InvalidItemRaisesValidationMessage(string candidate)
        {
            const string LearnRefNumber = "123456789X";
            const int AimSeqNumber = 1;

            var testDate = DateTime.Parse("2018-07-31");

            var deliveries = new List<ILearningDelivery>();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(testDate);
            mockDelivery
                .SetupGet(x => x.AimSeqNumber)
                .Returns(AimSeqNumber);
            mockDelivery
              .SetupGet(x => x.AimType)
              .Returns(1);
            mockDelivery
              .SetupGet(x => x.ProgTypeNullable)
              .Returns(24);
            deliveries.Add(mockDelivery.Object);

            var esmType = candidate.Substring(0, 3);
            var esmCode = int.Parse(candidate.Substring(3));

            var monitors = new List<IEmploymentStatusMonitoring>();
            var mockItem = new Mock<IEmploymentStatusMonitoring>(MockBehavior.Strict);
            mockItem
                .SetupGet(y => y.ESMType)
                .Returns(esmType);
            mockItem
                .SetupGet(y => y.ESMCode)
                .Returns(esmCode);
            monitors.Add(mockItem.Object);

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

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(RuleNameConstants.EmpStat_19, LearnRefNumber, AimSeqNumber, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("ESMType", esmType))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("ESMCode", esmCode))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTimeQS
                .Setup(x => x.IsDateBetween(mockDelivery.Object.LearnStartDate, EmpStat_19Rule.NewCodeMonitoringThresholdDate, DateTime.MaxValue, true))
                .Returns(true);

            var lEmpQS = new Mock<ILearnerEmploymentStatusQueryService>(MockBehavior.Strict);
            lEmpQS
               .Setup(x => x.LearnerEmploymentStatusForDate(It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>(), testDate))
               .Returns(mockStatus.Object);

            var sut = new EmpStat_19Rule(handler.Object, dateTimeQS.Object, lEmpQS.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            dateTimeQS.VerifyAll();
        }

        [Theory]
        [InlineData("EII5")]
        [InlineData("EII6")]
        [InlineData("LOE1")]
        [InlineData("LOE2")]
        [InlineData("LOE3")]
        [InlineData("LOE4")]
        [InlineData("PEI1")]
        [InlineData("BSI3")]
        [InlineData("BSI2")]
        [InlineData("BSI1")]
        [InlineData("BSI4")]
        [InlineData("SEI1")]
        [InlineData("SEM1")]
        [InlineData("LOU1")]
        [InlineData("LOU2")]
        [InlineData("LOU3")]
        [InlineData("LOU4")]
        [InlineData("LOU5")]
        public void ValidItemDoesNotRaiseValidationMessage(string candidate)
        {
            const string LearnRefNumber = "123456789X";
            const int AimSeqNumber = 1;

            var testDate = DateTime.Parse("2018-07-31");

            var deliveries = new List<ILearningDelivery>();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(testDate);
            mockDelivery
                .SetupGet(x => x.AimSeqNumber)
                .Returns(AimSeqNumber);
            deliveries.Add(mockDelivery.Object);
            mockDelivery
              .SetupGet(x => x.AimType)
              .Returns(1);
            mockDelivery
              .SetupGet(x => x.ProgTypeNullable)
              .Returns(24);
            deliveries.Add(mockDelivery.Object);

            var esmType = candidate.Substring(0, 3);
            var esmCode = int.Parse(candidate.Substring(3));

            var monitors = new List<IEmploymentStatusMonitoring>();
            var mockItem = new Mock<IEmploymentStatusMonitoring>(MockBehavior.Strict);
            mockItem
                .SetupGet(y => y.ESMType)
                .Returns(esmType);
            mockItem
                .SetupGet(y => y.ESMCode)
                .Returns(esmCode);
            monitors.Add(mockItem.Object);

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

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTimeQS
                .Setup(x => x.IsDateBetween(mockDelivery.Object.LearnStartDate, EmpStat_19Rule.NewCodeMonitoringThresholdDate, DateTime.MaxValue, true))
                .Returns(true);

            var lEmpQS = new Mock<ILearnerEmploymentStatusQueryService>(MockBehavior.Strict);
            lEmpQS
               .Setup(x => x.LearnerEmploymentStatusForDate(It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>(), testDate))
               .Returns(mockStatus.Object);

            var sut = new EmpStat_19Rule(handler.Object, dateTimeQS.Object, lEmpQS.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            dateTimeQS.VerifyAll();
        }

        public EmpStat_19Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            var lEmpQS = new Mock<ILearnerEmploymentStatusQueryService>(MockBehavior.Strict);

            return new EmpStat_19Rule(handler.Object, dateTimeQS.Object, lEmpQS.Object);
        }
    }
}
