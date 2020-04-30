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
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("EmpStat_14", result);
        }

        [Theory]
        [InlineData("2018-09-11", "2014-08-01", "2018-09-01", "2016-02-11", "2017-06-09")]
        [InlineData("2017-12-31", "2015-12-31", "2017-12-30", "2014-12-31", "2017-10-16")]
        [InlineData("2018-07-01", "2018-06-30", "2014-05-11", "2014-07-12")]
        [InlineData("2016-11-17", "2016-11-16")]
        public void GetQualifyingAimMeetsExpectation(string candidate, params string[] d22Dates)
        {
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
                    .Returns(FundModels.EuropeanSocialFund);
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
                .Returns(FundModels.EuropeanSocialFund);
            mockItem
                .SetupGet(x => x.LearnStartDate)
                .Returns(testDate);

            deliveries.Add(mockItem.Object);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            var lEmpQS = new Mock<ILearnerEmploymentStatusQueryService>(MockBehavior.Strict);

            var sut = new EmpStat_14Rule(handler.Object, fcsData.Object, lEmpQS.Object);

            var result = sut.GetQualifyingdAimOn(deliveries);

            handler.VerifyAll();
            fcsData.VerifyAll();

            Assert.Equal(mockItem.Object, result);
        }

        [Fact]
        public void GetEligibleEmploymentStatusWithNullDeliveryMeetsExpectation()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            fcsData
                .Setup(x => x.GetEligibilityRuleEmploymentStatusesFor(null))
                .Returns((IReadOnlyCollection<IEsfEligibilityRuleEmploymentStatus>)null);
            var lEmpQS = new Mock<ILearnerEmploymentStatusQueryService>(MockBehavior.Strict);

            var sut = new EmpStat_14Rule(handler.Object, fcsData.Object, lEmpQS.Object);

            var result = sut.GetEligibilityRulesFor(null);

            handler.VerifyAll();
            fcsData.VerifyAll();

            Assert.Empty(result);
            Assert.IsAssignableFrom<IReadOnlyCollection<IEsfEligibilityRuleEmploymentStatus>>(result);
        }

        [Theory]
        [InlineData(10, 10, true)]
        [InlineData(10, 12, false)]
        [InlineData(10, 11, false)]
        [InlineData(10, 98, false)]
        [InlineData(12, 11, false)]
        [InlineData(12, 98, false)]
        [InlineData(11, 98, false)]
        public void HasAQualifyingEmploymentStatusMeetsExpectation(int status, int eligibility, bool expectation)
        {
            var sut = NewRule();

            var mockStatus = new Mock<ILearnerEmploymentStatus>();
            mockStatus
                .SetupGet(x => x.EmpStat)
                .Returns(status);

            var mockEligibility = new Mock<IEsfEligibilityRuleEmploymentStatus>();
            mockEligibility
                .SetupGet(x => x.Code)
                .Returns(eligibility);

            var result = sut.HasAQualifyingEmploymentStatus(mockEligibility.Object, mockStatus.Object);

            Assert.Equal(expectation, result);
        }

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

            var result = sut.IsNotValid(items, mockStatus.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(10, 11, 12, 98)]
        [InlineData(11, 10, 12, 98)]
        [InlineData(12, 10, 11, 98)]
        [InlineData(98, 10, 11, 12)]
        public void InvalidItemRaisesValidationMessage(int candidate, params int[] eligibilities)
        {
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

            var employmentStatuses = new ILearnerEmploymentStatus[] { mockStatus.Object };

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);
            mockLearner
                .SetupGet(x => x.LearnerEmploymentStatuses)
                .Returns(employmentStatuses);

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

            var esfEmploymentStatuses = new List<IEsfEligibilityRuleEmploymentStatus>();
            eligibilities.ForEach(x => esfEmploymentStatuses.Add(GetEligibility(x)));

            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            fcsData
                .Setup(x => x.GetEligibilityRuleEmploymentStatusesFor(conRefNumber))
                .Returns(esfEmploymentStatuses);

            var lEmpQS = new Mock<ILearnerEmploymentStatusQueryService>(MockBehavior.Strict);
            lEmpQS
                .Setup(x => x.LearnerEmploymentStatusForDate(employmentStatuses, testDate))
                .Returns(mockStatus.Object);

            var sut = new EmpStat_14Rule(handler.Object, fcsData.Object, lEmpQS.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            fcsData.VerifyAll();
        }

        [Theory]
        [InlineData(10, 10, 11, 12, 98)]
        [InlineData(11, 10, 11, 12, 98)]
        [InlineData(12, 10, 11, 12, 98)]
        [InlineData(98, 10, 11, 12, 98)]
        public void ValidItemDoesNotRaiseValidationMessage(int candidate, params int[] eligibilities)
        {
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

            var employmentStatuses = new ILearnerEmploymentStatus[] { mockStatus.Object };

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);
            mockLearner
                .SetupGet(x => x.LearnerEmploymentStatuses)
                .Returns(employmentStatuses);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var esfEmploymentStatuses = new List<IEsfEligibilityRuleEmploymentStatus>();
            eligibilities.ForEach(x => esfEmploymentStatuses.Add(GetEligibility(x)));

            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            fcsData
                .Setup(x => x.GetEligibilityRuleEmploymentStatusesFor(conRefNumber))
                .Returns(esfEmploymentStatuses);

            var lEmpQS = new Mock<ILearnerEmploymentStatusQueryService>(MockBehavior.Strict);
            lEmpQS
                .Setup(x => x.LearnerEmploymentStatusForDate(employmentStatuses, testDate))
                .Returns(mockStatus.Object);

            var sut = new EmpStat_14Rule(handler.Object, fcsData.Object, lEmpQS.Object);

            sut.Validate(mockLearner.Object);

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
                .Returns(FundModels.EuropeanSocialFund);
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

        public EmpStat_14Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var fcsData = new Mock<IFCSDataService>(MockBehavior.Strict);
            var lEmpQS = new Mock<ILearnerEmploymentStatusQueryService>(MockBehavior.Strict);

            return new EmpStat_14Rule(handler.Object, fcsData.Object, lEmpQS.Object);
        }
    }
}
