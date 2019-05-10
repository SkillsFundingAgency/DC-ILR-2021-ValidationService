using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
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
    public class ESMType_11RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var provider = new Mock<IProvideLookupDetails>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new ESMType_11Rule(null, provider.Object));
        }

        /// <summary>
        /// New rule with null provider throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullProviderThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new ESMType_11Rule(handler.Object, null));
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
            Assert.Equal("ESMType_11", result);
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
            Assert.Equal(ESMType_11Rule.Name, result);
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
        /// Is qualifying period meets expectation
        /// </summary>
        /// <param name="candidateCode">The candidate code.</param>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor4To6M, "2010-11-12", false)]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor7To12M, "2012-07-31", false)]
        [InlineData(Monitoring.EmploymentStatus.EmployedForMoreThan12M, "2012-08-01", true)]
        [InlineData(Monitoring.EmploymentStatus.EmployedForUpTo3M, "2014-03-17", true)]
        public void InQualifyingPeriodMeetsExpectation(string candidateCode, string candidate, bool expectation)
        {
            // arrange
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

            // act
            var result = sut.InQualifyingPeriod(mockItem.Object, testdate);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Is not valid with null monitorings returns false
        /// </summary>
        [Fact]
        public void IsNotValidWithNullMonitoringsReturnsFalse()
        {
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<ILearnerEmploymentStatus>();

            // act
            var result = sut.IsNotValid(mockItem.Object);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Is not valid with empty monitorings returns false
        /// </summary>
        [Fact]
        public void IsNotValidWithEmptyMonitoringsReturnsFalse()
        {
            // arrange
            var sut = NewRule();

            var monitorings = Collection.EmptyAndReadOnly<IEmploymentStatusMonitoring>();
            var mockItem = new Mock<ILearnerEmploymentStatus>();
            mockItem
                .SetupGet(x => x.EmploymentStatusMonitorings)
                .Returns(monitorings);

            // act
            var result = sut.IsNotValid(mockItem.Object);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// the whole thing hinges on what the lookup provider returns for the 'is current' call
        /// </summary>
        /// <param name="candidateCode">The candidate code.</param>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor4To6M, "2010-11-12")]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor7To12M, "2012-07-31")]
        [InlineData(Monitoring.EmploymentStatus.EmployedForMoreThan12M, "2012-08-01")]
        [InlineData(Monitoring.EmploymentStatus.EmployedForUpTo3M, "2014-03-17")]
        public void InvalidItemRaisesValidationMessage(string candidateCode, string candidate)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var testDate = DateTime.Parse(candidate);

            var mockItem = new Mock<IEmploymentStatusMonitoring>();
            mockItem
                .SetupGet(x => x.ESMType)
                .Returns(candidateCode.Substring(0, 3));
            mockItem
                .SetupGet(x => x.ESMCode)
                .Returns(int.Parse(candidateCode.Substring(3)));

            var monitorings = Collection.Empty<IEmploymentStatusMonitoring>();
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
                .Returns(monitorings.AsSafeReadOnlyList());

            var statii = Collection.Empty<ILearnerEmploymentStatus>();
            statii.Add(status.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(y => y.LearnerEmploymentStatuses)
                .Returns(statii.AsSafeReadOnlyList());

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

            // we want it to 'fail'; so we return false
            var provider = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            provider
                .Setup(x => x.IsCurrent(TypeOfLimitedLifeLookup.ESMType, candidateCode, testDate))
                .Returns(false);

            var sut = new ESMType_11Rule(handler.Object, provider.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// the whole thing hinges on what the lookup provider returns for the 'is current' call
        /// </summary>
        /// <param name="candidateCode">The candidate code.</param>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor4To6M, "2010-11-12")]
        [InlineData(Monitoring.EmploymentStatus.EmployedFor7To12M, "2012-07-31")]
        [InlineData(Monitoring.EmploymentStatus.EmployedForMoreThan12M, "2012-08-01")]
        [InlineData(Monitoring.EmploymentStatus.EmployedForUpTo3M, "2014-03-17")]
        public void ValidItemDoesNotRaiseValidationMessage(string candidateCode, string candidate)
        {
            // arrange
            const string LearnRefNumber = "123456789X";

            var testDate = DateTime.Parse(candidate);

            var mockItem = new Mock<IEmploymentStatusMonitoring>();
            mockItem
                .SetupGet(x => x.ESMType)
                .Returns(candidateCode.Substring(0, 3));
            mockItem
                .SetupGet(x => x.ESMCode)
                .Returns(int.Parse(candidateCode.Substring(3)));

            var monitorings = Collection.Empty<IEmploymentStatusMonitoring>();
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
                .Returns(monitorings.AsSafeReadOnlyList());

            var statii = Collection.Empty<ILearnerEmploymentStatus>();
            statii.Add(status.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(y => y.LearnerEmploymentStatuses)
                .Returns(statii.AsSafeReadOnlyList());

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            // we want it to 'succeed'; so we return true
            var provider = new Mock<IProvideLookupDetails>(MockBehavior.Strict);
            provider
                .Setup(x => x.IsCurrent(TypeOfLimitedLifeLookup.ESMType, candidateCode, testDate))
                .Returns(true);

            var sut = new ESMType_11Rule(handler.Object, provider.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public ESMType_11Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var provider = new Mock<IProvideLookupDetails>(MockBehavior.Strict);

            return new ESMType_11Rule(handler.Object, provider.Object);
        }
    }
}
