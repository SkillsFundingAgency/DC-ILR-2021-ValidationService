using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.ESMType;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.EmploymentStatus.ESMType
{
    public class ESMType_12RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new ESMType_12Rule(null, common.Object));
        }

        [Fact]
        public void NewRuleWithNullCommonOperationsThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var ddRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new ESMType_12Rule(handler.Object, null));
        }

        /// <summary>
        /// Rule name 1, matches a literal.
        /// </summary>
        [Fact]
        public void RuleName1()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.RuleName;

            // assert
            Assert.Equal("ESMType_12", result);
        }

        /// <summary>
        /// Rule name 2, matches the constant.
        /// </summary>
        [Fact]
        public void RuleName2()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.RuleName;

            // assert
            Assert.Equal(ESMType_12Rule.Name, result);
        }

        /// <summary>
        /// Rule name 3 test, account for potential false positives.
        /// </summary>
        [Fact]
        public void RuleName3()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.RuleName;

            // assert
            Assert.NotEqual("SomeOtherRuleName_07", result);
        }

        /// <summary>
        /// Validate with null learner throws.
        /// </summary>
        [Fact]
        public void ValidateWithNullLearnerThrows()
        {
            // arrange
            var sut = NewRule();

            // act / assert
            Assert.Throws<ArgumentNullException>(() => sut.Validate(null));
        }

        /// <summary>
        /// First viable date meets expectation.
        /// </summary>
        [Fact]
        public void FirstViableDateMeetsExpectation()
        {
            // arrange / act / assert
            Assert.Equal(DateTime.Parse("2013-08-01"), ESMType_12Rule.FirstViableDate);
        }

        /// <summary>
        /// Is qualifying employment meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(TypeOfEmploymentStatus.InPaidEmployment, false)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable, true)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable, true)]
        [InlineData(TypeOfEmploymentStatus.NotKnownProvided, false)]
        public void IsQualifyingEmploymentMeetsExpectation(int candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<ILearnerEmploymentStatus>();
            mockItem
                .SetupGet(y => y.EmpStat)
                .Returns(candidate);

            // act
            var result = sut.IsQualifyingEmployment(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Has disqualifying indicator meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
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
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<IEmploymentStatusMonitoring>();
            mockItem
                .SetupGet(y => y.ESMType)
                .Returns(candidate.Substring(0, 3));
            mockItem
                .SetupGet(y => y.ESMCode)
                .Returns(int.Parse(candidate.Substring(3)));

            // act
            var result = sut.HasDisqualifyingIndicator(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Has disqualifying indicator with null monitorings returns false
        /// </summary>
        [Fact]
        public void HasDisqualifyingIndicatorWithNullMonitoringsReturnsFalse()
        {
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<ILearnerEmploymentStatus>();

            // act
            var result = sut.HasDisqualifyingIndicator(mockItem.Object);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Has disqualifying indicator with empty monitorings returns false
        /// </summary>
        [Fact]
        public void HasQualifyingIndicatorWithEmptyMonitoringsReturnsFalse()
        {
            // arrange
            var sut = NewRule();

            var monitorings = new List<IEmploymentStatusMonitoring>();
            var mockItem = new Mock<ILearnerEmploymentStatus>();
            mockItem
                .SetupGet(x => x.EmploymentStatusMonitorings)
                .Returns(monitorings);

            // act
            var result = sut.HasDisqualifyingIndicator(mockItem.Object);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="empStat">The emp stat.</param>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData(TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.SelfEmployed)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.EmployedFor0To10HourPW)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.EmployedFor11To20HoursPW)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.EmployedFor16HoursOrMorePW)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.EmployedFor16To19HoursPW)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.EmployedFor20HoursOrMorePW)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.EmployedFor21To30HoursPW)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.EmployedFor31PlusHoursPW)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.EmployedForLessThan16HoursPW)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.SelfEmployed)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.EmployedFor0To10HourPW)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.EmployedFor11To20HoursPW)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.EmployedFor16HoursOrMorePW)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.EmployedFor16To19HoursPW)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.EmployedFor20HoursOrMorePW)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.EmployedFor21To30HoursPW)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.EmployedFor31PlusHoursPW)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.EmployedForLessThan16HoursPW)]
        public void InvalidItemRaisesValidationMessage(int empStat, string candidate)
        {
            // arrange
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

            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            common
                .Setup(x => x.HasQualifyingStart(status.Object, ESMType_12Rule.FirstViableDate, null))
                .Returns(true);

            var sut = new ESMType_12Rule(handler.Object, common.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            common.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        /// <param name="empStat">The emp stat.</param>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData(TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.InFulltimeEducationOrTrainingPriorToEnrolment)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.InReceiptOfAnotherStateBenefit)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.InReceiptOfEmploymentAndSupportAllowance)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.InReceiptOfJobSeekersAllowance)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.InReceiptOfUniversalCredit)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.EmployedFor4To6M)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.EmployedFor7To12M)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.EmployedForMoreThan12M)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.EmployedForUpTo3M)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.UnemployedFor12To23M)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.UnemployedFor24To35M)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.UnemployedFor36MPlus)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.UnemployedFor6To11M)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.UnemployedForLessThan6M)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable, Monitoring.EmploymentStatus.SmallEmployer)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.InFulltimeEducationOrTrainingPriorToEnrolment)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.InReceiptOfAnotherStateBenefit)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.InReceiptOfEmploymentAndSupportAllowance)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.InReceiptOfJobSeekersAllowance)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.InReceiptOfUniversalCredit)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.EmployedFor4To6M)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.EmployedFor7To12M)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.EmployedForMoreThan12M)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.EmployedForUpTo3M)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.UnemployedFor12To23M)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.UnemployedFor24To35M)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.UnemployedFor36MPlus)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.UnemployedFor6To11M)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.UnemployedForLessThan6M)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable, Monitoring.EmploymentStatus.SmallEmployer)]
        public void ValidItemDoesNotRaiseValidationMessage(int empStat, string candidate)
        {
            // arrange
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

            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            common
                .Setup(x => x.HasQualifyingStart(status.Object, ESMType_12Rule.FirstViableDate, null))
                .Returns(true);

            var sut = new ESMType_12Rule(handler.Object, common.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            common.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public ESMType_12Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var common = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            return new ESMType_12Rule(handler.Object, common.Object);
        }
    }
}
