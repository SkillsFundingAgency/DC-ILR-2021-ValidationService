using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpStat;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.EmploymentStatus.EmpStat
{
    public class EmpStat_10RuleTests
    {
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            var mockDDRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);

            Assert.Throws<ArgumentNullException>(() => new EmpStat_10Rule(null, mockDDRule22.Object));
        }

        [Fact]
        public void NewRuleWithNullDerivedDataRule22Throws()
        {
            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            Assert.Throws<ArgumentNullException>(() => new EmpStat_10Rule(mockHandler.Object, null));
        }

        [Fact]
        public void RuleName1()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("EmpStat_10", result);
        }

        [Fact]
        public void RuleName2()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal(EmpStat_10Rule.Name, result);
        }

        [Fact]
        public void RuleName3()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.NotEqual("SomeOtherRuleName_07", result);
        }

        [Fact]
        public void ValidateWithNullLearnerThrows()
        {
            var sut = NewRule();

            Assert.Throws<ArgumentNullException>(() => sut.Validate(null));
        }

        [Theory]
        [InlineData("2018-04-18", "2018-03-10", true)]
        [InlineData("2018-04-18", "2018-04-17", true)]
        [InlineData("2018-04-18", "2018-04-18", false)]
        [InlineData("2018-04-18", "2018-04-19", false)]
        public void HasAQualifyingEmploymentStatusMeetsExpectation(string candidate, string startDate, bool expectation)
        {
            var sut = NewRule();

            var thresholdDate = DateTime.Parse(candidate);
            var mockStatus = new Mock<ILearnerEmploymentStatus>();
            mockStatus
                .SetupGet(y => y.DateEmpStatApp)
                .Returns(DateTime.Parse(startDate));

            var result = sut.HasAQualifyingEmploymentStatus(mockStatus.Object, thresholdDate);

            Assert.Equal(expectation, result);
        }

        [Fact]
        public void IsNotValidWithNullStatusesReturnsTrue()
        {
            var sut = NewRule();

            var mockItem = new Mock<ILearner>();

            var result = sut.IsNotValid(mockItem.Object, DateTime.MinValue);

            Assert.True(result);
        }

        [Fact]
        public void IsNotValidWithEmptyStatusesReturnsTrue()
        {
            var sut = NewRule();

            var mockItem = new Mock<ILearner>();
            mockItem
                .SetupGet(x => x.LearnerEmploymentStatuses)
                .Returns(new List<ILearnerEmploymentStatus>());

            var result = sut.IsNotValid(mockItem.Object, DateTime.MinValue);

            Assert.True(result);
        }

        [Theory]
        [InlineData("2018-09-11", "2014-08-01", "2018-09-11", null, "2016-02-11", null, "2017-06-09")]
        [InlineData("2017-12-31", null, "2015-12-31", "2017-12-30", "2014-12-31", null, "2017-10-16", null)]
        [InlineData("2018-07-01", "2018-06-30", "2014-05-11", "2014-07-12")]
        [InlineData("2016-11-17", "2016-11-17", null)]
        public void InvalidItemRaisesValidationMessage(string candidate, params string[] d22Dates)
        {
            const string LearnRefNumber = "123456789X";

            var testDate = DateTime.Parse(candidate);

            var latestContractCandidates = new List<DateTime?>();
            d22Dates.ForEach(x => latestContractCandidates.Add(GetNullableDate(x)));
            var expectedContractDate = latestContractCandidates.Max();

            var deliveries = new List<ILearningDelivery>();
            for (int i = 0; i < latestContractCandidates.Count; i++)
            {
                var mockDelivery = new Mock<ILearningDelivery>();
                deliveries.Add(mockDelivery.Object);
            }

            var mockStatus = new Mock<ILearnerEmploymentStatus>();
            mockStatus
                .SetupGet(y => y.DateEmpStatApp)
                .Returns(testDate);

            var statii = new List<ILearnerEmploymentStatus>();
            statii.Add(mockStatus.Object);

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
                .Setup(x => x.Handle(
                    Moq.It.Is<string>(y => y == EmpStat_10Rule.Name),
                    Moq.It.Is<string>(y => y == LearnRefNumber),
                    null,
                    Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == EmpStat_10Rule.MessagePropertyName),
                    expectedContractDate))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter(
                    Moq.It.Is<string>(y => y == PropertyNameConstants.LearnAimRef),
                    TypeOfAim.References.ESFLearnerStartandAssessment))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var mockDDRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);
            mockDDRule22
                .Setup(x => x.GetLatestLearningStartForESFContract(Moq.It.IsAny<ILearningDelivery>(), deliveries))
                .ReturnsInOrder(latestContractCandidates);

            var sut = new EmpStat_10Rule(handler.Object, mockDDRule22.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            mockDDRule22.VerifyAll();
        }

        [Theory]
        [InlineData("2018-09-11", "2014-08-01", "2018-09-12", null, "2016-02-11", null, "2017-06-09")]
        [InlineData("2017-12-31", null, "2015-12-31", "2018-01-01", "2014-12-31", null, "2017-10-16", null)]
        [InlineData("2018-07-01", "2018-07-02", "2014-05-11", "2014-07-12")]
        [InlineData("2016-11-17", "2016-11-18", null)]
        public void ValidItemDoesNotRaiseValidationMessage(string candidate, params string[] d22Dates)
        {
            const string LearnRefNumber = "123456789X";

            var testDate = DateTime.Parse(candidate);

            var latestContractCandidates = new List<DateTime?>();
            d22Dates.ForEach(x => latestContractCandidates.Add(GetNullableDate(x)));
            var expectedContractDate = latestContractCandidates.Max();

            var deliveries = new List<ILearningDelivery>();
            for (int i = 0; i < latestContractCandidates.Count; i++)
            {
                var mockDelivery = new Mock<ILearningDelivery>();
                deliveries.Add(mockDelivery.Object);
            }

            var mockStatus = new Mock<ILearnerEmploymentStatus>();
            mockStatus
                .SetupGet(y => y.DateEmpStatApp)
                .Returns(testDate);

            var statii = new List<ILearnerEmploymentStatus>();
            statii.Add(mockStatus.Object);

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

            var mockDDRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);
            mockDDRule22
                .Setup(x => x.GetLatestLearningStartForESFContract(Moq.It.IsAny<ILearningDelivery>(), deliveries))
                .ReturnsInOrder(latestContractCandidates);

            var sut = new EmpStat_10Rule(handler.Object, mockDDRule22.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            mockDDRule22.VerifyAll();
        }

        public DateTime? GetNullableDate(string candidate) =>
            Utility.It.Has(candidate) ? DateTime.Parse(candidate) : (DateTime?)null;

        public EmpStat_10Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var mockDDRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);

            return new EmpStat_10Rule(handler.Object, mockDDRule22.Object);
        }
    }
}
