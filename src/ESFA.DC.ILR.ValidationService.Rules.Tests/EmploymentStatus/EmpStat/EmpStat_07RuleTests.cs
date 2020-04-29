using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpId;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.EmploymentStatus.EmpId
{
    public class EmpStat_07RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("EmpStat_07", result);
        }

        [Fact]
        public void LastViableDateMeetsExpectation()
        {
            Assert.Equal(DateTime.Parse("2013-07-31"), EmpStat_07Rule.LastViableDate);
        }

        [Fact]
        public void PlannedTotalQualifyingHoursMeetsExpectation()
        {
            Assert.Equal(540, EmpStat_07Rule.PlannedTotalQualifyingHours);
        }

        [Theory]
        [InlineData(null, null, 0)]
        [InlineData(1, null, 1)]
        [InlineData(null, 1, 1)]
        [InlineData(1, 1, 2)]
        public void GetLearningHoursTotalMeetsExpectation(int? planned, int? eep, int expectation)
        {
            var mockItem = new Mock<ILearner>();
            mockItem.SetupGet(x => x.PlanLearnHoursNullable).Returns(planned);
            mockItem.SetupGet(x => x.PlanEEPHoursNullable).Returns(eep);

            var sut = NewRule();

            var result = sut.GetLearningHoursTotal(mockItem.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(25, true)]
        [InlineData(82, true)]
        [InlineData(35, false)]
        public void HasQualifyingFundingMeetsExpectation(int fundModel, bool expectation)
        {
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(fundModel);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            var lEmpQS = new Mock<ILearnerEmploymentStatusQueryService>(MockBehavior.Strict);

            var sut = new EmpStat_07Rule(handler.Object, dateTimeQS.Object, lEmpQS.Object);

            var result = sut.HasQualifyingFunding(mockDelivery.Object);

            Assert.Equal(expectation, result);

            handler.VerifyAll();
            dateTimeQS.VerifyAll();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void HasQualifyingStartMeetsExpectation(bool expectation)
        {
            var mockItem = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTimeQS
                .Setup(x => x.IsDateBetween(mockItem.Object.LearnStartDate, DateTime.MinValue, DateTime.Parse("2013-07-31"), true))
                .Returns(expectation);

            var lEmpQS = new Mock<ILearnerEmploymentStatusQueryService>(MockBehavior.Strict);

            var sut = new EmpStat_07Rule(handler.Object, dateTimeQS.Object, lEmpQS.Object);

            var result = sut.HasQualifyingStart(mockItem.Object);

            Assert.Equal(expectation, result);

            handler.VerifyAll();
            dateTimeQS.VerifyAll();
        }

        [Fact]
        public void HasQualifyingEmploymentStatusReturnsTrue()
        {
            var sut = NewRule();
            var mockItem = new Mock<ILearnerEmploymentStatus>();

            var result = sut.HasQualifyingEmployment(mockItem.Object);

            Assert.True(result);
        }

        [Fact]
        public void HasQualifyingEmploymentStatusWithNullReturnsFalse()
        {
            var sut = NewRule();

            var result = sut.HasQualifyingEmployment(null);

            Assert.False(result);
        }

        [Fact]
        public void InvalidItemRaisesValidationMessage()
        {
            const string LearnRefNumber = "123456789X";

            var testDate = DateTime.Parse("2013-08-01");

            var status = new Mock<ILearnerEmploymentStatus>();

            var employmentStatuses = new ILearnerEmploymentStatus[] { status.Object };

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(testDate);
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(25);

            var deliveries = new ILearningDelivery[] { mockDelivery.Object };

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);
            mockLearner
                .SetupGet(y => y.LearnerEmploymentStatuses)
                .Returns(employmentStatuses);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle("EmpStat_07", LearnRefNumber, 0, It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("PlanLearnHours", null))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("PlanEEPHours", null))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnStartDate", AbstractRule.AsRequiredCultureDate(testDate)))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTimeQS
                .Setup(x => x.IsDateBetween(mockDelivery.Object.LearnStartDate, DateTime.MinValue, DateTime.Parse("2013-07-31"), true))
                .Returns(true);

            var lEmpQS = new Mock<ILearnerEmploymentStatusQueryService>(MockBehavior.Strict);
            lEmpQS
               .Setup(x => x.LearnerEmploymentStatusForDate(employmentStatuses, testDate))
               .Returns((ILearnerEmploymentStatus)null);

            var sut = new EmpStat_07Rule(handler.Object, dateTimeQS.Object, lEmpQS.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            dateTimeQS.VerifyAll();
        }

        [Fact]
        public void ValidItemDoesNotRaiseValidationMessage()
        {
            const string LearnRefNumber = "123456789X";

            var testDate = DateTime.Parse("2013-08-01");

            var status = new Mock<ILearnerEmploymentStatus>();

            var employmentStatuses = new ILearnerEmploymentStatus[] { status.Object };

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(testDate);
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(25);

            var deliveries = new ILearningDelivery[] { mockDelivery.Object };

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);
            mockLearner
                .SetupGet(y => y.LearnerEmploymentStatuses)
                .Returns(employmentStatuses);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTimeQS
                .Setup(x => x.IsDateBetween(mockDelivery.Object.LearnStartDate, DateTime.MinValue, DateTime.Parse("2013-07-31"), true))
                .Returns(true);

            var lEmpQS = new Mock<ILearnerEmploymentStatusQueryService>(MockBehavior.Strict);
            lEmpQS
               .Setup(x => x.LearnerEmploymentStatusForDate(employmentStatuses, testDate))
               .Returns(status.Object);

            var sut = new EmpStat_07Rule(handler.Object, dateTimeQS.Object, lEmpQS.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            dateTimeQS.VerifyAll();
        }

        public EmpStat_07Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var dateTimeQS = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            var lEmpQS = new Mock<ILearnerEmploymentStatusQueryService>(MockBehavior.Strict);

            return new EmpStat_07Rule(handler.Object, dateTimeQS.Object, lEmpQS.Object);
        }
    }
}