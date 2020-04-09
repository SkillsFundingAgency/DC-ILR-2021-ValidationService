using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.ESMType;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.EmploymentStatus.ESMType
{
    public class ESMType_02RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange / act / assert
            Assert.Throws<ArgumentNullException>(() => new ESMType_02Rule(null));
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
            Assert.Equal("ESMType_02", result);
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
            Assert.Equal(ESMType_02Rule.Name, result);
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
        /// Last inviable date meets expectation.
        /// </summary>
        [Fact]
        public void LastInviableDateMeetsExpectation()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.LastInviableDate;

            // assert
            Assert.Equal(DateTime.Parse("2012-07-31"), result);
        }

        /// <summary>
        /// Is qualifying employment meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(TypeOfEmploymentStatus.InPaidEmployment, true)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable, false)]
        [InlineData(TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable, false)]
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
        /// Is qualifying period meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("2010-11-12", false)]
        [InlineData("2012-07-31", false)]
        [InlineData("2012-08-01", true)]
        [InlineData("2014-03-17", true)]
        public void IsQualifyingPeriodMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<ILearnerEmploymentStatus>();
            mockItem
                .SetupGet(y => y.DateEmpStatApp)
                .Returns(DateTime.Parse(candidate));

            // act
            var result = sut.IsQualifyingPeriod(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Has qualifying indicator meets expectation
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
        [InlineData(Monitoring.EmploymentStatus.SelfEmployed, false)]
        [InlineData(Monitoring.EmploymentStatus.SmallEmployer, false)]
        [InlineData(Monitoring.EmploymentStatus.UnemployedFor12To23M, false)]
        [InlineData(Monitoring.EmploymentStatus.UnemployedFor24To35M, false)]
        [InlineData(Monitoring.EmploymentStatus.UnemployedFor36MPlus, false)]
        [InlineData(Monitoring.EmploymentStatus.UnemployedFor6To11M, false)]
        [InlineData(Monitoring.EmploymentStatus.UnemployedForLessThan6M, false)]
        [InlineData("BSI0", false)]
        [InlineData("BSI5", false)]
        [InlineData("EII0", true)]
        [InlineData("EII9", true)]
        [InlineData("LOE0", false)]
        [InlineData("LOE5", false)]
        [InlineData("LOU0", false)]
        [InlineData("LOU6", false)]
        [InlineData("PEI0", false)]
        [InlineData("PEI2", false)]
        [InlineData("SEI0", false)]
        [InlineData("SEI2", false)]
        [InlineData("SEM0", false)]
        [InlineData("SEM2", false)]
        public void HasQualifyingIndicatorMeetsExpectation(string candidate, bool expectation)
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
            var result = sut.HasQualifyingIndicator(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Has qualifying indicator with null monitorings returns false
        /// </summary>
        [Fact]
        public void HasQualifyingIndicatorWithNullMonitoringsReturnsFalse()
        {
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<ILearnerEmploymentStatus>();

            // act
            var result = sut.HasQualifyingIndicator(mockItem.Object);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Has qualifying indicator with empty monitorings returns false
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
            var result = sut.HasQualifyingIndicator(mockItem.Object);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor4To6M)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor7To12M)]
        [InlineData(Monitoring.EmploymentStatus.EmployedForMoreThan12M)]
        [InlineData(Monitoring.EmploymentStatus.EmployedForUpTo3M)]
        [InlineData(Monitoring.EmploymentStatus.InFulltimeEducationOrTrainingPriorToEnrolment)]
        [InlineData(Monitoring.EmploymentStatus.InReceiptOfAnotherStateBenefit)]
        [InlineData(Monitoring.EmploymentStatus.InReceiptOfEmploymentAndSupportAllowance)]
        [InlineData(Monitoring.EmploymentStatus.InReceiptOfJobSeekersAllowance)]
        [InlineData(Monitoring.EmploymentStatus.InReceiptOfUniversalCredit)]
        [InlineData(Monitoring.EmploymentStatus.SelfEmployed)]
        [InlineData(Monitoring.EmploymentStatus.SmallEmployer)]
        [InlineData(Monitoring.EmploymentStatus.UnemployedFor12To23M)]
        [InlineData(Monitoring.EmploymentStatus.UnemployedFor24To35M)]
        [InlineData(Monitoring.EmploymentStatus.UnemployedFor36MPlus)]
        [InlineData(Monitoring.EmploymentStatus.UnemployedFor6To11M)]
        [InlineData(Monitoring.EmploymentStatus.UnemployedForLessThan6M)]
        [InlineData("BSI0")]
        [InlineData("BSI5")]
        [InlineData("LOE0")]
        [InlineData("LOE5")]
        [InlineData("LOU0")]
        [InlineData("LOU6")]
        [InlineData("PEI0")]
        [InlineData("PEI2")]
        [InlineData("SEI0")]
        [InlineData("SEI2")]
        [InlineData("SEM0")]
        [InlineData("SEM2")]
        public void InvalidItemRaisesValidationMessage(string candidate)
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
                .Returns(TypeOfEmploymentStatus.InPaidEmployment);
            status
                .SetupGet(x => x.EmploymentStatusMonitorings)
                .Returns(monitorings);

            var statii = new List<ILearnerEmploymentStatus>();
            statii.Add(status.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(y => y.LearnerEmploymentStatuses)
                .Returns(statii);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(
                    Moq.It.Is<string>(y => y == ESMType_02Rule.Name),
                    Moq.It.Is<string>(y => y == LearnRefNumber),
                    null,
                    Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == "EmpStat"),
                    TypeOfEmploymentStatus.InPaidEmployment))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == "DateEmpStatApp"),
                    testDate))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var sut = new ESMType_02Rule(handler.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor0To10HourPW)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor11To20HoursPW)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor16HoursOrMorePW)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor16To19HoursPW)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor20HoursOrMorePW)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor21To30HoursPW)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor31PlusHoursPW)]
        [InlineData(Monitoring.EmploymentStatus.EmployedForLessThan16HoursPW)]
        [InlineData("EII0")]
        [InlineData("EII9")]
        public void ValidItemDoesNotRaiseValidationMessage(string candidate)
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

            var status = new Mock<ILearnerEmploymentStatus>();
            status
                .SetupGet(x => x.DateEmpStatApp)
                .Returns(DateTime.Parse("2013-08-01"));
            status
                .SetupGet(x => x.EmpStat)
                .Returns(TypeOfEmploymentStatus.InPaidEmployment);
            status
                .SetupGet(x => x.EmploymentStatusMonitorings)
                .Returns(monitorings);

            var statii = new List<ILearnerEmploymentStatus>();
            statii.Add(status.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(y => y.LearnerEmploymentStatuses)
                .Returns(statii);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var sut = new ESMType_02Rule(handler.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public ESMType_02Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            return new ESMType_02Rule(handler.Object);
        }
    }
}
