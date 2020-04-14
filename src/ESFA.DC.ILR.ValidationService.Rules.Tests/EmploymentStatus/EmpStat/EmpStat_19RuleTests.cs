using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpStat;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.EmploymentStatus.EmpStat
{
    public class EmpStat_19RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var commonchecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new EmpStat_19Rule(null, commonchecks.Object));
        }

        /// <summary>
        /// New rule with null common checks throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullCommonChecksThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new EmpStat_19Rule(handler.Object, null));
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
            Assert.Equal("EmpStat_19", result);
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
            Assert.Equal(RuleNameConstants.EmpStat_19, result);
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

            // act/assert
            Assert.Throws<ArgumentNullException>(() => sut.Validate(null));
        }

        /// <summary>
        /// Old code monitoring threshold date meets expectation.
        /// </summary>
        [Fact]
        public void NewCodeMonitoringThresholdDateMeetsExpectation()
        {
            // arrange / act / assert
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
            // arrange
            var testDate = DateTime.Parse(candidate);
            var employments = new List<ILearnerEmploymentStatus>();
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonchecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonchecks
                .Setup(x => x.GetEmploymentStatusOn(testDate, employments))
                .Returns((ILearnerEmploymentStatus)null);

            var sut = new EmpStat_19Rule(handler.Object, commonchecks.Object);

            // act
            var result = sut.GetEmploymentStatusOn(testDate, employments);

            // assert
            Assert.Null(result);
            handler.VerifyAll();
            commonchecks.VerifyAll();
        }

        /// <summary>
        /// Has a disqualifying monitor status meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("EII1", true)] // Monitoring.EmploymentStatus.EmployedFor16HoursOrMorePW
        [InlineData("EII2", true)] // Monitoring.EmploymentStatus.EmployedForLessThan16HoursPW
        [InlineData("EII3", true)] // Monitoring.EmploymentStatus.EmployedFor16To19HoursPW
        [InlineData("EII4", true)] // Monitoring.EmploymentStatus.EmployedFor20HoursOrMorePW
        [InlineData("EII5", false)] // Monitoring.EmploymentStatus.EmployedFor0To10HourPW
        [InlineData("EII6", false)] // Monitoring.EmploymentStatus.EmployedFor11To20HoursPW
        [InlineData("EII7", true)] // Monitoring.EmploymentStatus.EmployedFor21To30HoursPW
        [InlineData("EII8", true)] // Monitoring.EmploymentStatus.EmployedFor31PlusHoursPW
        [InlineData("LOE1", false)] // Monitoring.EmploymentStatus.EmployedForUpTo3M
        [InlineData("LOE2", false)] // Monitoring.EmploymentStatus.EmployedFor4To6M
        [InlineData("LOE3", false)] // Monitoring.EmploymentStatus.EmployedFor7To12M
        [InlineData("LOE4", false)] // Monitoring.EmploymentStatus.EmployedForMoreThan12M
        [InlineData("PEI1", false)] // Monitoring.EmploymentStatus.InFulltimeEducationOrTrainingPriorToEnrolment
        [InlineData("BSI3", false)] // Monitoring.EmploymentStatus.InReceiptOfAnotherStateBenefit
        [InlineData("BSI2", false)] // Monitoring.EmploymentStatus.InReceiptOfEmploymentAndSupportAllowance
        [InlineData("BSI1", false)] // Monitoring.EmploymentStatus.InReceiptOfJobSeekersAllowance
        [InlineData("BSI4", false)] // Monitoring.EmploymentStatus.InReceiptOfUniversalCredit
        [InlineData("SEI1", false)] // Monitoring.EmploymentStatus.SelfEmployed
        [InlineData("SEM1", false)] // Monitoring.EmploymentStatus.SmallEmployer
        [InlineData("LOU1", false)] // Monitoring.EmploymentStatus.UnemployedForLessThan6M
        [InlineData("LOU2", false)] // Monitoring.EmploymentStatus.UnemployedFor6To11M
        [InlineData("LOU3", false)] // Monitoring.EmploymentStatus.UnemployedFor12To23M
        [InlineData("LOU4", false)] // Monitoring.EmploymentStatus.UnemployedFor24To35M
        [InlineData("LOU5", false)] // Monitoring.EmploymentStatus.UnemployedFor36MPlus
        public void HasADisqualifyingMonitorStatusMeetsExpectation(string candidate, bool expectation)
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
            var result = sut.HasADisqualifyingMonitorStatus(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        [Fact]
        public void CheckEmploymentStatusMeetsExpectation()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonchecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            var sut = new EmpStat_19Rule(handler.Object, commonchecks.Object);

            // act - unfortunately there isn't a 'does not throw'
            sut.CheckEmploymentStatus(null, null);

            // assert - so the best we can do is verify
            handler.VerifyAll();
            commonchecks.VerifyAll();
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData("EII1")] // Monitoring.EmploymentStatus.EmployedFor16HoursOrMorePW
        [InlineData("EII2")] // Monitoring.EmploymentStatus.EmployedForLessThan16HoursPW
        [InlineData("EII3")] // Monitoring.EmploymentStatus.EmployedFor16To19HoursPW
        [InlineData("EII4")] // Monitoring.EmploymentStatus.EmployedFor20HoursOrMorePW
        [InlineData("EII7")] // Monitoring.EmploymentStatus.EmployedFor21To30HoursPW
        [InlineData("EII8")] // Monitoring.EmploymentStatus.EmployedFor31PlusHoursPW
        public void InvalidItemRaisesValidationMessage(string candidate)
        {
            // arrange
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
                .Returns(TypeOfEmploymentStatus.InPaidEmployment);
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

            var commonchecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonchecks
                .Setup(x => x.IsTraineeship(mockDelivery.Object))
                .Returns(true);
            commonchecks
                .Setup(x => x.InAProgramme(mockDelivery.Object))
                .Returns(true);
            commonchecks
                .Setup(x => x.HasQualifyingStart(mockDelivery.Object, EmpStat_19Rule.NewCodeMonitoringThresholdDate, null))
                .Returns(true);
            commonchecks
                .Setup(x => x.GetEmploymentStatusOn(testDate, Moq.It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>()))
                .Returns(mockStatus.Object);

            var sut = new EmpStat_19Rule(handler.Object, commonchecks.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            commonchecks.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData("EII5")] // Monitoring.EmploymentStatus.EmployedFor0To10HourPW
        [InlineData("EII6")] // Monitoring.EmploymentStatus.EmployedFor11To20HoursPW
        [InlineData("LOE1")] // Monitoring.EmploymentStatus.EmployedForUpTo3M
        [InlineData("LOE2")] // Monitoring.EmploymentStatus.EmployedFor4To6M
        [InlineData("LOE3")] // Monitoring.EmploymentStatus.EmployedFor7To12M
        [InlineData("LOE4")] // Monitoring.EmploymentStatus.EmployedForMoreThan12M
        [InlineData("PEI1")] // Monitoring.EmploymentStatus.InFulltimeEducationOrTrainingPriorToEnrolment
        [InlineData("BSI3")] // Monitoring.EmploymentStatus.InReceiptOfAnotherStateBenefit
        [InlineData("BSI2")] // Monitoring.EmploymentStatus.InReceiptOfEmploymentAndSupportAllowance
        [InlineData("BSI1")] // Monitoring.EmploymentStatus.InReceiptOfJobSeekersAllowance
        [InlineData("BSI4")] // Monitoring.EmploymentStatus.InReceiptOfUniversalCredit
        [InlineData("SEI1")] // Monitoring.EmploymentStatus.SelfEmployed
        [InlineData("SEM1")] // Monitoring.EmploymentStatus.SmallEmployer
        [InlineData("LOU1")] // Monitoring.EmploymentStatus.UnemployedForLessThan6M
        [InlineData("LOU2")] // Monitoring.EmploymentStatus.UnemployedFor6To11M
        [InlineData("LOU3")] // Monitoring.EmploymentStatus.UnemployedFor12To23M
        [InlineData("LOU4")] // Monitoring.EmploymentStatus.UnemployedFor24To35M
        [InlineData("LOU5")] // Monitoring.EmploymentStatus.UnemployedFor36MPlus
        public void ValidItemDoesNotRaiseValidationMessage(string candidate)
        {
            // arrange
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
                .Returns(TypeOfEmploymentStatus.InPaidEmployment);
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
            var commonchecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonchecks
                .Setup(x => x.IsTraineeship(mockDelivery.Object))
                .Returns(true);
            commonchecks
                .Setup(x => x.InAProgramme(mockDelivery.Object))
                .Returns(true);
            commonchecks
                .Setup(x => x.HasQualifyingStart(mockDelivery.Object, EmpStat_19Rule.NewCodeMonitoringThresholdDate, null))
                .Returns(true);
            commonchecks
                .Setup(x => x.GetEmploymentStatusOn(testDate, Moq.It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>()))
                .Returns(mockStatus.Object);

            var sut = new EmpStat_19Rule(handler.Object, commonchecks.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            commonchecks.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public EmpStat_19Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonchecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            return new EmpStat_19Rule(handler.Object, commonchecks.Object);
        }
    }
}
