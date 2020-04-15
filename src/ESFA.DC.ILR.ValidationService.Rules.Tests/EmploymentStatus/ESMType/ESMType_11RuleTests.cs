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
    public class ESMType_11RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("ESMType_11", result);
        }

        [Theory]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor4To6M, "2010-11-12", false)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor7To12M, "2012-07-31", false)]
        [InlineData(Monitoring.EmploymentStatus.EmployedForMoreThan12M, "2012-08-01", true)]
        [InlineData(Monitoring.EmploymentStatus.EmployedForUpTo3M, "2014-03-17", true)]
        public void InQualifyingPeriodMeetsExpectation(string candidateCode, string candidate, bool expectation)
        {
            var testdate = DateTime.Parse(candidate);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var provider = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            provider
                .Setup(x => x.IsCurrent(TypeOfLimitedLifeLookup.ESMType, candidateCode, testdate))
                .Returns(expectation);

            var sut = new ESMType_11Rule(handler.Object, provider.Object);

            var mockItem = new Mock<IEmploymentStatusMonitoring>();
            mockItem
                .SetupGet(x => x.ESMType)
                .Returns(candidateCode.Substring(0, 3));
            mockItem
                .SetupGet(x => x.ESMCode)
                .Returns(int.Parse(candidateCode.Substring(3)));

            var result = sut.InQualifyingPeriod(mockItem.Object, testdate);

            Assert.Equal(expectation, result);
        }

        [Fact]
        public void IsNotValidWithNullMonitoringsReturnsFalse()
        {
            var sut = NewRule();
            var mockItem = new Mock<ILearnerEmploymentStatus>();

            var result = sut.IsNotValid(mockItem.Object);

            Assert.False(result);
        }

        [Fact]
        public void IsNotValidWithEmptyMonitoringsReturnsFalse()
        {
            var sut = NewRule();

            var monitorings = new List<IEmploymentStatusMonitoring>();
            var mockItem = new Mock<ILearnerEmploymentStatus>();
            mockItem
                .SetupGet(x => x.EmploymentStatusMonitorings)
                .Returns(monitorings);

            var result = sut.IsNotValid(mockItem.Object);

            Assert.False(result);
        }

        [Theory]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor4To6M, "2010-11-12")]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor7To12M, "2012-07-31")]
        [InlineData(Monitoring.EmploymentStatus.EmployedForMoreThan12M, "2012-08-01")]
        [InlineData(Monitoring.EmploymentStatus.EmployedForUpTo3M, "2014-03-17")]
        public void InvalidItemRaisesValidationMessage(string candidateCode, string candidate)
        {
            const string LearnRefNumber = "123456789X";

            var testDate = DateTime.Parse(candidate);

            var mockItem = new Mock<IEmploymentStatusMonitoring>();
            mockItem
                .SetupGet(x => x.ESMType)
                .Returns(candidateCode.Substring(0, 3));
            mockItem
                .SetupGet(x => x.ESMCode)
                .Returns(int.Parse(candidateCode.Substring(3)));

            var monitorings = new List<IEmploymentStatusMonitoring>();
            monitorings.Add(mockItem.Object);

            var status = new Mock<ILearnerEmploymentStatus>();
            status
                .SetupGet(x => x.DateEmpStatApp)
                .Returns(testDate);
            status
                .SetupGet(x => x.EmpStat)
                .Returns(TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable);
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
                    Moq.It.Is<string>(y => y == ESMType_11Rule.Name),
                    Moq.It.Is<string>(y => y == LearnRefNumber),
                    null,
                    Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == "DateEmpStatApp"),
                    testDate))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var provider = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            provider
                .Setup(x => x.IsCurrent(TypeOfLimitedLifeLookup.ESMType, candidateCode, testDate))
                .Returns(false);

            var sut = new ESMType_11Rule(handler.Object, provider.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
        }

        [Theory]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor4To6M, "2010-11-12")]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor7To12M, "2012-07-31")]
        [InlineData(Monitoring.EmploymentStatus.EmployedForMoreThan12M, "2012-08-01")]
        [InlineData(Monitoring.EmploymentStatus.EmployedForUpTo3M, "2014-03-17")]
        public void ValidItemDoesNotRaiseValidationMessage(string candidateCode, string candidate)
        {
            const string LearnRefNumber = "123456789X";

            var testDate = DateTime.Parse(candidate);

            var mockItem = new Mock<IEmploymentStatusMonitoring>();
            mockItem
                .SetupGet(x => x.ESMType)
                .Returns(candidateCode.Substring(0, 3));
            mockItem
                .SetupGet(x => x.ESMCode)
                .Returns(int.Parse(candidateCode.Substring(3)));

            var monitorings = new List<IEmploymentStatusMonitoring>();
            monitorings.Add(mockItem.Object);

            var status = new Mock<ILearnerEmploymentStatus>();
            status
                .SetupGet(x => x.DateEmpStatApp)
                .Returns(testDate);
            status
                .SetupGet(x => x.EmpStat)
                .Returns(TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable);
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

            var provider = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            provider
                .Setup(x => x.IsCurrent(TypeOfLimitedLifeLookup.ESMType, candidateCode, testDate))
                .Returns(true);

            var sut = new ESMType_11Rule(handler.Object, provider.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
        }

        public ESMType_11Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var provider = new Mock<IProvideLookupDetails>(MockBehavior.Strict);

            return new ESMType_11Rule(handler.Object, provider.Object);
        }
    }
}
