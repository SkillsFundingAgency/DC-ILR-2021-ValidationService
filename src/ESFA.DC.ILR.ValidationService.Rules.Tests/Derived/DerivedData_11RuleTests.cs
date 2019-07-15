using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Derived
{
    public class DerivedData_11RuleTests
    {
        /// <summary>
        /// In receipt of benefits meets expectation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("BSI1", true)] // Monitoring.EmploymentStatus.InReceiptOfJobSeekersAllowance
        [InlineData("BSI2", true)] // Monitoring.EmploymentStatus.InReceiptOfEmploymentAndSupportAllowance
        [InlineData("BSI3", true)] // Monitoring.EmploymentStatus.InReceiptOfAnotherStateBenefit
        [InlineData("BSI4", true)] // Monitoring.EmploymentStatus.InReceiptOfUniversalCredit
        [InlineData("EII1", false)]
        [InlineData("EII2", false)]
        [InlineData("EII3", false)]
        [InlineData("EII4", false)]
        [InlineData("EII5", false)]
        [InlineData("LOU1", false)]
        [InlineData("LOU2", false)]
        [InlineData("LOU3", false)]
        [InlineData("LOU4", false)]
        [InlineData("LOU5", false)]
        [InlineData("PEM1", false)]
        [InlineData("SEM1", false)]
        [InlineData("SEI1", false)]
        public void InReceiptOfBenefitsMeetsExpectation(string candidate, bool expectation)
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
            var result = sut.InReceiptOfBenefits(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// In receipt of benefits with null monitors returns false.
        /// </summary>
        [Fact]
        public void InReceiptOfBenefitsWithNullMonitorsReturnsFalse()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.InReceiptOfBenefits((IReadOnlyCollection<IEmploymentStatusMonitoring>)null);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// In receipt of benefits with empty monitors return false.
        /// </summary>
        [Fact]
        public void InReceiptOfBenefitsWithEmptyMonitorsReturnFalse()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.InReceiptOfBenefits(new IEmploymentStatusMonitoring[] { });

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// In receipt of benefits with null employments returns false.
        /// </summary>
        [Fact]
        public void InReceiptOfBenefitsWithNullEmploymentsReturnsFalse()
        {
            // arrange
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.GetEmploymentStatusOn(DateTime.Today, null))
                .Returns((ILearnerEmploymentStatus)null);

            var sut = new DerivedData_11Rule(commonOps.Object);

            // act
            var result = sut.InReceiptOfBenefits(null, DateTime.Today);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// In  receipt of benefits with empty employments return false.
        /// </summary>
        [Fact]
        public void InReceiptOfBenefitsWithEmptyEmploymentsReturnFalse()
        {
            // arrange
            var empty = new ILearnerEmploymentStatus[] { };
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.GetEmploymentStatusOn(DateTime.Today, empty))
                .Returns((ILearnerEmploymentStatus)null);

            var sut = new DerivedData_11Rule(commonOps.Object);

            // act
            var result = sut.InReceiptOfBenefits(empty, DateTime.Today);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// is adult funded on benefits at start of aim returns false
        /// this would previously have thrown an empty set exception for the employment records
        /// </summary>
        [Fact]
        public void IsAdultFundedOnBenefitsAtStartOfAimReturnsFalse()
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(DateTime.Today);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.GetEmploymentStatusOn(DateTime.Today, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>()))
                .Returns((ILearnerEmploymentStatus)null);
            commonOps
                .Setup(x => x.HasQualifyingFunding(delivery.Object, TypeOfFunding.AdultSkills))
                .Returns(true);

            var sut = new DerivedData_11Rule(commonOps.Object);

            // act
            var result = sut.IsAdultFundedOnBenefitsAtStartOfAim(delivery.Object, null);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up derived data rule</returns>
        public DerivedData_11Rule NewRule()
        {
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            return new DerivedData_11Rule(commonOps.Object);
        }
    }
}
