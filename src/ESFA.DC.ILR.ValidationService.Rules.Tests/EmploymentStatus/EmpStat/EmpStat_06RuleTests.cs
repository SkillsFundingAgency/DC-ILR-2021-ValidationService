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
    public class EmpStat_06RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("EmpStat_06", result);
        }

        [Fact]
        public void FirstViableDateMeetsExpectation()
        {
            Assert.Equal(DateTime.Parse("2013-08-01"), EmpStat_06Rule.FirstViableDate);
        }

        [Fact]
        public void LastViableDateMeetsExpectation()
        {
            Assert.Equal(DateTime.Parse("2014-07-31"), EmpStat_06Rule.LastViableDate);
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
        [InlineData(24, true)]
        [InlineData(25, false)]
        public void IsExcludedMeetsExpectation(int? progType, bool expectation)
        {
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
              .SetupGet(y => y.ProgTypeNullable)
              .Returns(progType);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            var sut = new EmpStat_06Rule(handler.Object, commonOps.Object);

            var result = sut.IsExcluded(mockDelivery.Object);

            Assert.Equal(expectation, result);

            handler.VerifyAll();
            commonOps.VerifyAll();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void HasQualifyingFundingMeetsExpectation(bool expectation)
        {
            var mockItem = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(mockItem.Object, 25, 82))
                .Returns(expectation);

            var sut = new EmpStat_06Rule(handler.Object, commonOps.Object);

            var result = sut.HasQualifyingFunding(mockItem.Object);

            Assert.Equal(expectation, result);

            handler.VerifyAll();
            commonOps.VerifyAll();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void HasQualifyingStartMeetsExpectation(bool expectation)
        {
            var mockItem = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingStart(mockItem.Object, DateTime.Parse("2013-08-01"), DateTime.Parse("2014-07-31")))
                .Returns(expectation);

            var sut = new EmpStat_06Rule(handler.Object, commonOps.Object);

            var result = sut.HasQualifyingStart(mockItem.Object);

            Assert.Equal(expectation, result);

            handler.VerifyAll();
            commonOps.VerifyAll();
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

            var statii = new ILearnerEmploymentStatus[] { status.Object };

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(testDate);
            mockDelivery
              .SetupGet(y => y.ProgTypeNullable)
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
                .Returns(statii);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle("EmpStat_06", LearnRefNumber, 0, It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("DateEmpStatApp", null))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("PlanLearnHours", null))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("PlanEEPHours", null))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnStartDate", AbstractRule.AsRequiredCultureDate(testDate)))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.GetEmploymentStatusOn(testDate, statii))
                .Returns((ILearnerEmploymentStatus)null);
            commonOps
                .Setup(x => x.HasQualifyingFunding(mockDelivery.Object, 25, 82))
                .Returns(true);
            commonOps
                .Setup(x => x.HasQualifyingStart(mockDelivery.Object, DateTime.Parse("2013-08-01"), DateTime.Parse("2014-07-31")))
                .Returns(true);

            var sut = new EmpStat_06Rule(handler.Object, commonOps.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            commonOps.VerifyAll();
        }

        [Fact]
        public void ValidItemDoesNotRaiseValidationMessage()
        {
            const string LearnRefNumber = "123456789X";

            var testDate = DateTime.Parse("2013-08-01");

            var status = new Mock<ILearnerEmploymentStatus>();

            var statii = new ILearnerEmploymentStatus[] { status.Object };

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(testDate);
            mockDelivery
               .SetupGet(y => y.ProgTypeNullable)
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
                .Returns(statii);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.GetEmploymentStatusOn(testDate, statii))
                .Returns(status.Object);
            commonOps
                .Setup(x => x.HasQualifyingFunding(mockDelivery.Object, 25, 82))
                .Returns(true);
            commonOps
                .Setup(x => x.HasQualifyingStart(mockDelivery.Object, DateTime.Parse("2013-08-01"), DateTime.Parse("2014-07-31")))
                .Returns(true);

            var sut = new EmpStat_06Rule(handler.Object, commonOps.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            commonOps.VerifyAll();
        }

        public EmpStat_06Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            return new EmpStat_06Rule(handler.Object, commonOps.Object);
        }
    }
}