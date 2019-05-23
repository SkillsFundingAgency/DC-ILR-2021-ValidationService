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
    public class EmpStat_18RuleTests
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
            Assert.Throws<ArgumentNullException>(() => new EmpStat_18Rule(null, commonchecks.Object));
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
            Assert.Throws<ArgumentNullException>(() => new EmpStat_18Rule(handler.Object, null));
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
            Assert.Equal("EmpStat_18", result);
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
            Assert.Equal(RuleNameConstants.EmpStat_18, result);
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
        public void OldCodeMonitoringThresholdDateMeetsExpectation()
        {
            // arrange / act / assert
            Assert.Equal(DateTime.Parse("2018-07-31"), EmpStat_18Rule.OldCodeMonitoringThresholdDate);
        }

        [Fact]
        public void IsQualifyingPrimaryLearningAimWithNullReturnsFalse()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.IsQualifyingPrimaryLearningAim(null);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void IsQualifyingPrimaryLearningAimWithInvalidDateReturnsFalse()
        {
            // arrange
            var mockDelivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonchecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonchecks
                .Setup(x => x.HasQualifyingStart(mockDelivery.Object, DateTime.MinValue, EmpStat_18Rule.OldCodeMonitoringThresholdDate))
                .Returns(false);

            var sut = new EmpStat_18Rule(handler.Object, commonchecks.Object);

            // act
            var result = sut.IsQualifyingPrimaryLearningAim(mockDelivery.Object);

            // assert
            Assert.False(result);

            handler.VerifyAll();
            commonchecks.VerifyAll();
        }

        [Fact]
        public void IsQualifyingPrimaryLearningAimNotTraineeReturnsFalse()
        {
            // arrange
            var mockDelivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonchecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonchecks
                .Setup(x => x.HasQualifyingStart(mockDelivery.Object, DateTime.MinValue, DateTime.Parse("2018-07-31"))) // EmpStat_18Rule.OldCodeMonitoringThresholdDate
                .Returns(true);
            commonchecks
                .Setup(x => x.IsTraineeship(mockDelivery.Object))
                .Returns(false);

            var sut = new EmpStat_18Rule(handler.Object, commonchecks.Object);

            // act
            var result = sut.IsQualifyingPrimaryLearningAim(mockDelivery.Object);

            // assert
            Assert.False(result);

            handler.VerifyAll();
            commonchecks.VerifyAll();
        }

        [Fact]
        public void IsQualifyingPrimaryLearningAimWithWrongAimTypeReturnsFalse()
        {
            // arrange
            var mockDelivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonchecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonchecks
                .Setup(x => x.HasQualifyingStart(mockDelivery.Object, DateTime.MinValue, DateTime.Parse("2018-07-31"))) // EmpStat_18Rule.OldCodeMonitoringThresholdDate
                .Returns(true);
            commonchecks
                .Setup(x => x.IsTraineeship(mockDelivery.Object))
                .Returns(true);
            commonchecks
                .Setup(x => x.InAProgramme(mockDelivery.Object))
                .Returns(false);

            var sut = new EmpStat_18Rule(handler.Object, commonchecks.Object);

            // act
            var result = sut.IsQualifyingPrimaryLearningAim(mockDelivery.Object);

            // assert
            Assert.False(result);

            handler.VerifyAll();
            commonchecks.VerifyAll();
        }

        [Fact]
        public void IsQualifyingPrimaryLearningAimPassingChecksReturnsTrue()
        {
            // arrange
            var mockDelivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonchecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonchecks
                .Setup(x => x.HasQualifyingStart(mockDelivery.Object, DateTime.MinValue, DateTime.Parse("2018-07-31"))) // EmpStat_18Rule.OldCodeMonitoringThresholdDate
                .Returns(true);
            commonchecks
                .Setup(x => x.IsTraineeship(mockDelivery.Object))
                .Returns(true);
            commonchecks
                .Setup(x => x.InAProgramme(mockDelivery.Object))
                .Returns(true);

            var sut = new EmpStat_18Rule(handler.Object, commonchecks.Object);

            // act
            var result = sut.IsQualifyingPrimaryLearningAim(mockDelivery.Object);

            // assert
            Assert.True(result);

            handler.VerifyAll();
            commonchecks.VerifyAll();
        }

        [Fact]
        public void HasQualifyingEmploymentStatusWithNullReturnsFalse()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.HasQualifyingEmploymentStatus(null);

            // assert
            Assert.False(result);
        }

        [Theory]
        [InlineData(10, true)] // TypeOfEmploymentStatus.InPaidEmployment
        [InlineData(11, false)] // TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable
        [InlineData(12, false)] // TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable
        [InlineData(98, false)] // TypeOfEmploymentStatus.NotKnownProvided
        public void HasQualifyingEmploymentStatusMeetsExpectation(int candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();

            var mockItem = new Mock<ILearnerEmploymentStatus>();
            mockItem
                .SetupGet(x => x.EmpStat)
                .Returns(candidate);

            // act
            var result = sut.HasQualifyingEmploymentStatus(mockItem.Object);

            // assert
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
            var result = sut.HasDisqualifyingMonitor(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
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
            // arrange
            const string LearnRefNumber = "123456789X";
            const int AimSeqNumber = 1;

            var testDate = DateTime.Parse("2018-07-31");

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(testDate);
            mockDelivery
                .SetupGet(x => x.AimType)
                .Returns(TypeOfAim.ProgrammeAim);
            mockDelivery
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(TypeOfLearningProgramme.Traineeship);
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
                .Returns(TypeOfEmploymentStatus.InPaidEmployment);
            mockStatus
                .SetupGet(y => y.EmploymentStatusMonitorings)
                .Returns(monitors.AsSafeReadOnlyList());

            var statii = new ILearnerEmploymentStatus[] { mockStatus.Object };

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
                .Setup(x => x.Handle("EmpStat_18", LearnRefNumber, AimSeqNumber, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("ESMType", esmType))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("ESMCode", esmCode))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var commonchecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonchecks
                .Setup(x => x.GetEmploymentStatusOn(testDate, statii))
                .Returns(mockStatus.Object);
            commonchecks
                .Setup(x => x.HasQualifyingStart(mockDelivery.Object, DateTime.MinValue, EmpStat_18Rule.OldCodeMonitoringThresholdDate))
                .Returns(true);
            commonchecks
                .Setup(x => x.IsTraineeship(mockDelivery.Object))
                .Returns(true);
            commonchecks
                .Setup(x => x.InAProgramme(mockDelivery.Object))
                .Returns(true);

            var sut = new EmpStat_18Rule(handler.Object, commonchecks.Object);

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
            // arrange
            const string LearnRefNumber = "123456789X";
            const int AimSeqNumber = 1;

            var testDate = DateTime.Parse("2018-07-31");

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(testDate);
            mockDelivery
                .SetupGet(x => x.AimType)
                .Returns(TypeOfAim.ProgrammeAim);
            mockDelivery
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(TypeOfLearningProgramme.Traineeship);
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
                .Returns(TypeOfEmploymentStatus.InPaidEmployment);
            mockStatus
                .SetupGet(y => y.EmploymentStatusMonitorings)
                .Returns(monitors);

            var statii = new ILearnerEmploymentStatus[] { mockStatus.Object };

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
            var commonchecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonchecks
                .Setup(x => x.GetEmploymentStatusOn(testDate, statii))
                .Returns(mockStatus.Object);
            commonchecks
                .Setup(x => x.HasQualifyingStart(mockDelivery.Object, DateTime.MinValue, EmpStat_18Rule.OldCodeMonitoringThresholdDate))
                .Returns(true);
            commonchecks
                .Setup(x => x.IsTraineeship(mockDelivery.Object))
                .Returns(true);
            commonchecks
                .Setup(x => x.InAProgramme(mockDelivery.Object))
                .Returns(true);

            var sut = new EmpStat_18Rule(handler.Object, commonchecks.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public EmpStat_18Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonchecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            return new EmpStat_18Rule(handler.Object, commonchecks.Object);
        }
    }
}
