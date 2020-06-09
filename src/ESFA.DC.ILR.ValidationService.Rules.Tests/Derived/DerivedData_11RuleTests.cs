using ESFA.DC.ILR.Model.Interface;
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
        [Theory]
        [InlineData("BSI1", true)]
        [InlineData("BSI2", true)]
        [InlineData("BSI3", true)]
        [InlineData("BSI4", true)]
        [InlineData("BSI5", true)]
        [InlineData("BSI6", true)]
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
            var sut = NewRule();
            var mockItem = new Mock<IEmploymentStatusMonitoring>();
            mockItem
                .SetupGet(y => y.ESMType)
                .Returns(candidate.Substring(0, 3));
            mockItem
                .SetupGet(y => y.ESMCode)
                .Returns(int.Parse(candidate.Substring(3)));

            var result = sut.InReceiptOfBenefits(mockItem.Object);

            Assert.Equal(expectation, result);
        }

        [Fact]
        public void InReceiptOfBenefitsWithNullMonitorsReturnsFalse()
        {
            var sut = NewRule();

            var result = sut.InReceiptOfBenefits((IReadOnlyCollection<IEmploymentStatusMonitoring>)null);

            Assert.False(result);
        }

        [Fact]
        public void InReceiptOfBenefitsWithEmptyMonitorsReturnFalse()
        {
            var sut = NewRule();

            var result = sut.InReceiptOfBenefits(new IEmploymentStatusMonitoring[] { });

            Assert.False(result);
        }

        [Fact]
        public void InReceiptOfBenefitsWithNullEmploymentsReturnsFalse()
        {
            var lEmpQS = new Mock<ILearnerEmploymentStatusQueryService>(MockBehavior.Strict);
            lEmpQS
                .Setup(x => x.LearnerEmploymentStatusForDate(null, DateTime.Today))
                .Returns((ILearnerEmploymentStatus)null);

            var sut = new DerivedData_11Rule(lEmpQS.Object);

            var result = sut.InReceiptOfBenefits(null, DateTime.Today);

            Assert.False(result);
        }

        [Fact]
        public void InReceiptOfBenefitsWithEmptyEmploymentsReturnFalse()
        {
            var empty = new ILearnerEmploymentStatus[] { };
            var lEmpQS = new Mock<ILearnerEmploymentStatusQueryService>(MockBehavior.Strict);
            lEmpQS
                .Setup(x => x.LearnerEmploymentStatusForDate(empty, DateTime.Today))
                .Returns((ILearnerEmploymentStatus)null);

            var sut = new DerivedData_11Rule(lEmpQS.Object);

            var result = sut.InReceiptOfBenefits(empty, DateTime.Today);

            Assert.False(result);
        }

        [Fact]
        public void IsAdultFundedOnBenefitsAtStartOfAimReturnsFalse()
        {
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(DateTime.Today);
            delivery
                .SetupGet(x => x.FundModel)
                .Returns(35);

            var lEmpQS = new Mock<ILearnerEmploymentStatusQueryService>(MockBehavior.Strict);
            lEmpQS
                .Setup(x => x.LearnerEmploymentStatusForDate(It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>(), DateTime.Today))
                .Returns((ILearnerEmploymentStatus)null);

            var sut = new DerivedData_11Rule(lEmpQS.Object);

            var result = sut.IsAdultFundedOnBenefitsAtStartOfAim(delivery.Object, null);

            Assert.False(result);
        }

        public DerivedData_11Rule NewRule()
        {
            var lEmpQS = new Mock<ILearnerEmploymentStatusQueryService>(MockBehavior.Strict);

            return new DerivedData_11Rule(lEmpQS.Object);
        }
    }
}
