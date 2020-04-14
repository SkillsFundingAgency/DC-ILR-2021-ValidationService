using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpStat;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.EmploymentStatus.EmpStat
{
    public class EmpStat_14RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new EmpStat_14Rule(null, fcsData.Object, commonOps.Object));
        }

        [Fact]
        public void NewRuleWithNullFCSDataServiceThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new EmpStat_14Rule(handler.Object, null, commonOps.Object));
        }

        [Fact]
        public void NewRuleWithNullCommonOperationsThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new EmpStat_14Rule(handler.Object, fcsData.Object, null));
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
            Assert.Equal("EmpStat_14", result);
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
            Assert.Equal(RuleNameConstants.EmpStat_14, result);
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
        /// Get qualifying aim meets expectation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="d22Dates">The D22 dates.</param>
        [Theory]
        [InlineData("2018-09-11", "2014-08-01", "2018-09-01", "2016-02-11", "2017-06-09")]
        [InlineData("2017-12-31", "2015-12-31", "2017-12-30", "2014-12-31", "2017-10-16")]
        [InlineData("2018-07-01", "2018-06-30", "2014-05-11", "2014-07-12")]
        [InlineData("2016-11-17", "2016-11-16")]
        public void GetQualifyingAimMeetsExpectation(string candidate, params string[] d22Dates)
        {
            // arrange
            var testDate = DateTime.Parse(candidate);

            var contractCandidates = new List<DateTime>();
            d22Dates.ForEach(x => contractCandidates.Add(DateTime.Parse(x)));

            var deliveries = new List<ILearningDelivery>();
            for (int i = 0; i < contractCandidates.Count; i++)
            {
                var mockDelivery = new Mock<ILearningDelivery>();
                mockDelivery
                    .SetupGet(x => x.LearnStartDate)
                    .Returns(contractCandidates.ElementAt(i));
                mockDelivery
                    .SetupGet(x => x.LearnAimRef)
                    .Returns(TypeOfAim.References.ESFLearnerStartandAssessment);
                mockDelivery
                    .SetupGet(x => x.CompStatus)
                    .Returns(CompletionState.HasCompleted);
                mockDelivery
                    .SetupGet(x => x.FundModel)
                    .Returns(TypeOfFunding.EuropeanSocialFund);
                deliveries.Add(mockDelivery.Object);
            }

            var mockItem = new Mock<ILearningDelivery>();
            mockItem
                .SetupGet(x => x.LearnAimRef)
                .Returns(TypeOfAim.References.ESFLearnerStartandAssessment);
            mockItem
                .SetupGet(x => x.CompStatus)
                .Returns(CompletionState.HasCompleted);
            mockItem
                .SetupGet(x => x.FundModel)
                .Returns(TypeOfFunding.EuropeanSocialFund);
            mockItem
                .SetupGet(x => x.LearnStartDate)
                .Returns(testDate);

            deliveries.Add(mockItem.Object);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            var sut = new EmpStat_14Rule(handler.Object, fcsData.Object, commonOps.Object);

            // act
            var result = sut.GetQualifyingdAimOn(deliveries);

            // assert
            handler.VerifyAll();
            fcsData.VerifyAll();

            Assert.Equal(mockItem.Object, result);
        }

        /// <summary>
        /// Get eligible employment status with null delivery meets expectation.
        /// </summary>
        [Fact]
        public void GetEligibleEmploymentStatusWithNullDeliveryMeetsExpectation()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            fcsData
                .Setup(x => x.GetEligibilityRuleEmploymentStatusesFor(null))
                .Returns((IReadOnlyCollection<IEsfEligibilityRuleEmploymentStatus>)null);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            var sut = new EmpStat_14Rule(handler.Object, fcsData.Object, commonOps.Object);

            // act
            var result = sut.GetEligibilityRulesFor(null);

            // assert
            handler.VerifyAll();
            fcsData.VerifyAll();

            Assert.Empty(result);
            Assert.IsAssignableFrom<IReadOnlyCollection<IEsfEligibilityRuleEmploymentStatus>>(result);
        }

        /// <summary>
        /// Has a qualifying employment status meets expectation
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="eligibility">The eligibility.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(10, 10, true)] // TypeOfEmploymentStatus.InPaidEmployment, TypeOfEmploymentStatus.InPaidEmployment
        [InlineData(10, 12, false)] // TypeOfEmploymentStatus.InPaidEmployment, TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable
        [InlineData(10, 11, false)] // TypeOfEmploymentStatus.InPaidEmployment, TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable
        [InlineData(10, 98, false)] // TypeOfEmploymentStatus.InPaidEmployment, TypeOfEmploymentStatus.NotKnownProvided
        [InlineData(12, 11, false)] // TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable, TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable
        [InlineData(12, 98, false)] // TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable, TypeOfEmploymentStatus.NotKnownProvided
        [InlineData(11, 98, false)] // TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable, TypeOfEmploymentStatus.NotKnownProvided
        public void HasAQualifyingEmploymentStatusMeetsExpectation(int status, int eligibility, bool expectation)
        {
            // arrange
            var sut = NewRule();

            var mockStatus = new Mock<ILearnerEmploymentStatus>();
            mockStatus
                .SetupGet(x => x.EmpStat)
                .Returns(status);

            var mockEligibility = new Mock<IEsfEligibilityRuleEmploymentStatus>();
            mockEligibility
                .SetupGet(x => x.Code)
                .Returns(eligibility);

            // act
            var result = sut.HasAQualifyingEmploymentStatus(mockEligibility.Object, mockStatus.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Is not valid meets expectation
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        /// <param name="eligibilities">The eligibilities.</param>
        [Theory]
        [InlineData(10, false, 10, 11, 12, 98)]
        [InlineData(10, true, 11, 12, 98)]
        [InlineData(11, false, 10, 11, 12, 98)]
        [InlineData(11, true, 10, 12, 98)]
        [InlineData(12, false, 10, 11, 12, 98)]
        [InlineData(12, true, 10, 11, 98)]
        [InlineData(98, false, 10, 11, 12, 98)]
        [InlineData(98, true, 10, 11, 12)]
        public void IsNotValidMeetsExpectation(int status, bool expectation, params int[] eligibilities)
        {
            // arrange
            var sut = NewRule();

            var mockStatus = new Mock<ILearnerEmploymentStatus>();
            mockStatus
                .SetupGet(x => x.EmpStat)
                .Returns(status);

            var items = new List<IEsfEligibilityRuleEmploymentStatus>();
            eligibilities.ForEach(x =>
            {
                var mockEligibility = new Mock<IEsfEligibilityRuleEmploymentStatus>();
                mockEligibility
                    .SetupGet(y => y.Code)
                    .Returns(x);

                items.Add(mockEligibility.Object);
            });

            // act
            var result = sut.IsNotValid(items, mockStatus.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="eligibilities">The eligibilities.</param>
        [Theory]
        [InlineData(10, 11, 12, 98)]
        [InlineData(11, 10, 12, 98)]
        [InlineData(12, 10, 11, 98)]
        [InlineData(98, 10, 11, 12)]
        public void InvalidItemRaisesValidationMessage(int candidate, params int[] eligibilities)
        {
            // arrange
            const string LearnRefNumber = "123456789X";
            const string conRefNumber = "test-Con-Ref";

            var testDate = DateTime.Parse("2016-06-14");

            var deliveries = new List<ILearningDelivery>();
            for (int i = -5; i < 1; i++)
            {
                deliveries.Add(GetTestDelivery(testDate.AddDays(i), conRefNumber, i));
            }

            var mockStatus = new Mock<ILearnerEmploymentStatus>();
            mockStatus
                .SetupGet(x => x.EmpStat)
                .Returns(candidate);

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
                .Setup(x => x.Handle(RuleNameConstants.EmpStat_14, LearnRefNumber, 0, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("EmpStat", candidate))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("ConRefNumber", conRefNumber))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnStartDate", AbstractRule.AsRequiredCultureDate(testDate)))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var employmentStatuses = new List<IEsfEligibilityRuleEmploymentStatus>();
            eligibilities.ForEach(x => employmentStatuses.Add(GetEligibility(x)));

            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            fcsData
                .Setup(x => x.GetEligibilityRuleEmploymentStatusesFor(conRefNumber))
                .Returns(employmentStatuses);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.GetEmploymentStatusOn(testDate, statii))
                .Returns(mockStatus.Object);

            var sut = new EmpStat_14Rule(handler.Object, fcsData.Object, commonOps.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            fcsData.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="eligibilities">The eligibilities.</param>
        [Theory]
        [InlineData(10, 10, 11, 12, 98)]
        [InlineData(11, 10, 11, 12, 98)]
        [InlineData(12, 10, 11, 12, 98)]
        [InlineData(98, 10, 11, 12, 98)]
        public void ValidItemDoesNotRaiseValidationMessage(int candidate, params int[] eligibilities)
        {
            // arrange
            const string LearnRefNumber = "123456789X";
            const string conRefNumber = "test-Con-Ref";

            var testDate = DateTime.Parse("2016-06-14");

            var deliveries = new List<ILearningDelivery>();
            for (int i = -5; i < 1; i++)
            {
                deliveries.Add(GetTestDelivery(testDate.AddDays(i), conRefNumber, i));
            }

            var mockStatus = new Mock<ILearnerEmploymentStatus>();
            mockStatus
                .SetupGet(x => x.EmpStat)
                .Returns(candidate);

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

            var employmentStatuses = new List<IEsfEligibilityRuleEmploymentStatus>();
            eligibilities.ForEach(x => employmentStatuses.Add(GetEligibility(x)));

            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            fcsData
                .Setup(x => x.GetEligibilityRuleEmploymentStatusesFor(conRefNumber))
                .Returns(employmentStatuses);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.GetEmploymentStatusOn(testDate, statii))
                .Returns(mockStatus.Object);

            var sut = new EmpStat_14Rule(handler.Object, fcsData.Object, commonOps.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            fcsData.VerifyAll();
        }

        public IEsfEligibilityRuleEmploymentStatus GetEligibility(int candidate)
        {
            var mockEligibility = new Mock<IEsfEligibilityRuleEmploymentStatus>(MockBehavior.Strict);
            mockEligibility
                .SetupGet(y => y.Code)
                .Returns(candidate);

            return mockEligibility.Object;
        }

        public ILearningDelivery GetTestDelivery(DateTime startDate, string conRefNumber, int offset)
        {
            var mockItem = new Mock<ILearningDelivery>(MockBehavior.Strict);
            mockItem
                .SetupGet(x => x.LearnAimRef)
                .Returns(TypeOfAim.References.ESFLearnerStartandAssessment);
            mockItem
                .SetupGet(x => x.CompStatus)
                .Returns(CompletionState.HasCompleted);
            mockItem
                .SetupGet(x => x.FundModel)
                .Returns(TypeOfFunding.EuropeanSocialFund);
            mockItem
                .SetupGet(x => x.LearnStartDate)
                .Returns(startDate);

            if (offset == 0)
            {
                mockItem
                    .SetupGet(x => x.AimSeqNumber)
                    .Returns(0);
                mockItem
                    .SetupGet(x => x.ConRefNumber)
                    .Returns(conRefNumber);
            }

            return mockItem.Object;
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public EmpStat_14Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            return new EmpStat_14Rule(handler.Object, fcsData.Object, commonOps.Object);
        }
    }
}
