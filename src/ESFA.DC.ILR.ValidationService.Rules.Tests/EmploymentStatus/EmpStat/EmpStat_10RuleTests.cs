using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpStat;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.EmploymentStatus.EmpStat
{
    public class EmpStat_10RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var mockDDRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new EmpStat_10Rule(null, mockDDRule22.Object));
        }

        /// <summary>
        /// New rule with null derived data rule 22 throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullDerivedDataRule22Throws()
        {
            // arrange
            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new EmpStat_10Rule(mockHandler.Object, null));
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
            Assert.Equal("EmpStat_10", result);
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
            Assert.Equal(EmpStat_10Rule.Name, result);
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
        /// Has a qualifying employment status meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("2018-04-18", "2018-03-10", true)]
        [InlineData("2018-04-18", "2018-04-17", true)]
        [InlineData("2018-04-18", "2018-04-18", false)]
        [InlineData("2018-04-18", "2018-04-19", false)]
        public void HasAQualifyingEmploymentStatusMeetsExpectation(string candidate, string startDate, bool expectation)
        {
            // arrange
            var sut = NewRule();

            var thresholdDate = DateTime.Parse(candidate);
            var mockStatus = new Mock<ILearnerEmploymentStatus>();
            mockStatus
                .SetupGet(y => y.DateEmpStatApp)
                .Returns(DateTime.Parse(startDate));

            // act
            var result = sut.HasAQualifyingEmploymentStatus(mockStatus.Object, thresholdDate);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Is not valid with null statuses returns true
        /// </summary>
        [Fact]
        public void IsNotValidWithNullStatusesReturnsTrue()
        {
            // arrange
            var sut = NewRule();

            var mockItem = new Mock<ILearner>();

            // act
            var result = sut.IsNotValid(mockItem.Object, DateTime.MinValue);

            // assert
            Assert.True(result);
        }

        /// <summary>
        /// Is not valid with empty statuses returns true
        /// </summary>
        [Fact]
        public void IsNotValidWithEmptyStatusesReturnsTrue()
        {
            // arrange
            var sut = NewRule();

            var mockItem = new Mock<ILearner>();
            mockItem
                .SetupGet(x => x.LearnerEmploymentStatuses)
                .Returns(new List<ILearnerEmploymentStatus>());

            // act
            var result = sut.IsNotValid(mockItem.Object, DateTime.MinValue);

            // assert
            Assert.True(result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// one of the d22 dates has to be the same or 'exceed' the candidate to generate the error
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="d22Dates">The D22 dates.</param>
        [Theory]
        [InlineData("2018-09-11", "2014-08-01", "2018-09-11", null, "2016-02-11", null, "2017-06-09")]
        [InlineData("2017-12-31", null, "2015-12-31", "2017-12-30", "2014-12-31", null, "2017-10-16", null)]
        [InlineData("2018-07-01", "2018-06-30", "2014-05-11", "2014-07-12")]
        [InlineData("2016-11-17", "2016-11-17", null)]
        public void InvalidItemRaisesValidationMessage(string candidate, params string[] d22Dates)
        {
            // arrange
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

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            mockDDRule22.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="d22Dates">The D22 dates.</param>
        [Theory]
        [InlineData("2018-09-11", "2014-08-01", "2018-09-12", null, "2016-02-11", null, "2017-06-09")]
        [InlineData("2017-12-31", null, "2015-12-31", "2018-01-01", "2014-12-31", null, "2017-10-16", null)]
        [InlineData("2018-07-01", "2018-07-02", "2014-05-11", "2014-07-12")]
        [InlineData("2016-11-17", "2016-11-18", null)]
        public void ValidItemDoesNotRaiseValidationMessage(string candidate, params string[] d22Dates)
        {
            // arrange
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

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            mockDDRule22.VerifyAll();
        }

        /// <summary>
        /// Gets a nullable date.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>a nullable date time</returns>
        public DateTime? GetNullableDate(string candidate) =>
            Utility.It.Has(candidate) ? DateTime.Parse(candidate) : (DateTime?)null;

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public EmpStat_10Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var mockDDRule22 = new Mock<IDerivedData_22Rule>(MockBehavior.Strict);

            return new EmpStat_10Rule(handler.Object, mockDDRule22.Object);
        }
    }
}
