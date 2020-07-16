using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.ESMType;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.EmploymentStatus.ESMType
{
    public class ESMType_01RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("ESMType_01", result);
        }

        [Theory]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor0To10HourPW, false)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor11To20HoursPW, false)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor16HoursOrMorePW, false)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor16To19HoursPW, false)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor20HoursOrMorePW, false)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor21To30HoursPW, false)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor31PlusHoursPW, false)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor4To6M, false)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor7To12M, false)]
        [InlineData(Monitoring.EmploymentStatus.EmployedForLessThan16HoursPW, false)]
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
        [InlineData("BSI0", true)]
        [InlineData("BSI5", true)]
        [InlineData("EII0", true)]
        [InlineData("EII9", true)]
        [InlineData("LOE0", true)]
        [InlineData("LOE5", true)]
        [InlineData("LOU0", true)]
        [InlineData("LOU6", true)]
        [InlineData("PEI0", true)]
        [InlineData("PEI2", true)]
        [InlineData("SEI0", true)]
        [InlineData("SEI2", true)]
        [InlineData("SEM0", true)]
        [InlineData("SEM2", true)]
        public void IsInvalidDomainItemMeetsExpectation(string candidate, bool expectation)
        {
            var mockItem = new Mock<IEmploymentStatusMonitoring>();
            mockItem
                .SetupGet(y => y.ESMType)
                .Returns(candidate.Substring(0, 3));
            mockItem
                .SetupGet(y => y.ESMCode)
                .Returns(int.Parse(candidate.Substring(3)));

            var lookupDetailsMock = new Mock<IProvideLookupDetails>();
            lookupDetailsMock.Setup(x => x.Contains(TypeOfStringCodedLookup.ESMType, candidate)).Returns(!expectation);

            var result = NewRule(lookupDetailsMock.Object).IsInvalidDomainItem(mockItem.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData("BSI0")]
        [InlineData("BSI5")]
        [InlineData("EII0")]
        [InlineData("EII9")]
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
            const string LearnRefNumber = "123456789X";

            var monitor = new Mock<IEmploymentStatusMonitoring>();
            var esmType = candidate.Substring(0, 3);
            var esmCode = int.Parse(candidate.Substring(3));
            monitor
                .SetupGet(y => y.ESMType)
                .Returns(esmType);
            monitor
                .SetupGet(y => y.ESMCode)
                .Returns(esmCode);

            var monitorings = new List<IEmploymentStatusMonitoring>();
            monitorings.Add(monitor.Object);

            var status = new Mock<ILearnerEmploymentStatus>();
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
                    Moq.It.Is<string>(y => y == ESMType_01Rule.Name),
                    Moq.It.Is<string>(y => y == LearnRefNumber),
                    null,
                    Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == "ESMType"),
                    esmType))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == "ESMCode"),
                    esmCode))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var lookupDetailsMock = new Mock<IProvideLookupDetails>();
            lookupDetailsMock.Setup(x => x.Contains(TypeOfStringCodedLookup.ESMType, candidate)).Returns(false);

            var sut = new ESMType_01Rule(handler.Object, lookupDetailsMock.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
        }

        [Theory]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor0To10HourPW)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor11To20HoursPW)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor16HoursOrMorePW)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor16To19HoursPW)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor20HoursOrMorePW)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor21To30HoursPW)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor31PlusHoursPW)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor4To6M)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor7To12M)]
        [InlineData(Monitoring.EmploymentStatus.EmployedForLessThan16HoursPW)]
        [InlineData(Monitoring.EmploymentStatus.EmployedForMoreThan12M)]
        [InlineData(Monitoring.EmploymentStatus.EmployedForUpTo3M)]
        [InlineData(Monitoring.EmploymentStatus.InFulltimeEducationOrTrainingPriorToEnrolment)]
        [InlineData(Monitoring.EmploymentStatus.InReceiptOfEmploymentAndSupport)]
        [InlineData(Monitoring.EmploymentStatus.InReceiptOfOtherStateBenefits)]
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
        public void ValidItemDoesNotRaiseValidationMessage(string candidate)
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

            var status = new Mock<ILearnerEmploymentStatus>();
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
            var lookupDetailsMock = new Mock<IProvideLookupDetails>();
            lookupDetailsMock.Setup(x => x.Contains(TypeOfStringCodedLookup.ESMType, candidate)).Returns(true);

            var sut = new ESMType_01Rule(handler.Object, lookupDetailsMock.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
        }

        [Fact]
        public void ValidItemWithEmptyEmploymentsDoesNotRaiseValidationMessage()
        {
            const string LearnRefNumber = "123456789X";

            var statii = new List<ILearnerEmploymentStatus>();

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(y => y.LearnerEmploymentStatuses)
                .Returns(statii);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var lookupDetailsMock = new Mock<IProvideLookupDetails>();

            var sut = new ESMType_01Rule(handler.Object, lookupDetailsMock.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
        }

        [Fact]
        public void ValidItemWithNullEmploymentsDoesNotRaiseValidationMessage()
        {
            const string LearnRefNumber = "123456789X";

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var lookupDetailsMock = new Mock<IProvideLookupDetails>();

            var sut = new ESMType_01Rule(handler.Object, lookupDetailsMock.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
        }

        public ESMType_01Rule NewRule(IProvideLookupDetails lookupDetails = null)
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            return new ESMType_01Rule(handler.Object, lookupDetails);
        }
    }
}
