using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_81RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("LearnAimRef_81", result);
        }

        [Fact]
        public void FirstViableDateMeetsExpectation()
        {
            var result = LearnAimRef_81Rule.FirstViableDate;

            Assert.Equal(DateTime.Parse("2016-08-01"), result);
        }

        [Theory]
        [InlineData(TypeOfLARSCategory.LegalEntitlementLevel2, false)]
        [InlineData(TypeOfLARSCategory.LicenseToPractice, true)]
        [InlineData(TypeOfLARSCategory.OnlyForLegalEntitlementAtLevel3, false)]
        [InlineData(TypeOfLARSCategory.WorkPlacementSFAFunded, false)]
        [InlineData(TypeOfLARSCategory.WorkPreparationSFATraineeships, false)]
        public void HasDisqualifyingLearningCategoryMeetsExpectation(int candidate, bool expectation)
        {
            var sut = NewRule();
            var mockItem = new Mock<ILARSLearningCategory>();
            mockItem
                .SetupGet(y => y.CategoryRef)
                .Returns(candidate);

            var result = sut.HasDisqualifyingLearningCategory(mockItem.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("testAim1")]
        [InlineData("testAim2")]
        public void HasDisqualifyingLearningCategoryForDeliveryMeetsExpectation(string candidate)
        {
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.LearnAimRef)
                .Returns(candidate);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetCategoriesFor(candidate))
                .Returns(new List<ILARSLearningCategory>());

            var commonChecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            var sut = new LearnAimRef_81Rule(handler.Object, service.Object, commonChecks.Object);

            var result = sut.HasDisqualifyingLearningCategory(mockDelivery.Object);

            handler.VerifyAll();
            service.VerifyAll();
            commonChecks.VerifyAll();

            Assert.False(result);
        }

        [Fact]
        public void InvalidItemRaisesValidationMessage()
        {
            const string learnRefNumber = "123456789X";
            const string learnAimRef = "salddfkjeifdnase";

            var testDate = DateTime.Parse("2016-08-01");

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(testDate);
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(TypeOfFunding.AdultSkills);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle("LearnAimRef_81", learnRefNumber, 0, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnAimRef", learnAimRef))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnStartDate", testDate))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("FundModel", TypeOfFunding.AdultSkills))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("ESMType", Monitoring.EmploymentStatus.Types.BenefitStatusIndicator))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("ESMCode", 3))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var mockLars = new Mock<ILARSLearningCategory>();
            mockLars
                .SetupGet(x => x.CategoryRef)
                .Returns(TypeOfLARSCategory.LicenseToPractice);

            var larsItems = new List<ILARSLearningCategory>();
            larsItems.Add(mockLars.Object);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetCategoriesFor(learnAimRef))
                .Returns(larsItems);

            var mockMonitor = new Mock<IEmploymentStatusMonitoring>();
            mockMonitor
                .SetupGet(y => y.ESMType)
                .Returns(Monitoring.EmploymentStatus.Types.BenefitStatusIndicator);
            mockMonitor
                .SetupGet(y => y.ESMCode)
                .Returns(3);

            var monitors = new List<IEmploymentStatusMonitoring>();
            monitors.Add(mockMonitor.Object);

            var mockEmployment = new Mock<ILearnerEmploymentStatus>();
            mockEmployment
                .SetupGet(y => y.EmploymentStatusMonitorings)
                .Returns(monitors);

            var commonChecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonChecks
                .Setup(x => x.IsSteelWorkerRedundancyTraining(mockDelivery.Object))
                .Returns(false);
            commonChecks
                .Setup(x => x.HasQualifyingFunding(mockDelivery.Object, TypeOfFunding.AdultSkills, TypeOfFunding.OtherAdult, TypeOfFunding.EuropeanSocialFund))
                .Returns(true);
            commonChecks
                .Setup(x => x.HasQualifyingStart(mockDelivery.Object, LearnAimRef_81Rule.FirstViableDate, null))
                .Returns(true);
            commonChecks
                .Setup(x => x.GetEmploymentStatusOn(testDate, Moq.It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>()))
                .Returns(mockEmployment.Object);

            var sut = new LearnAimRef_81Rule(handler.Object, service.Object, commonChecks.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            service.VerifyAll();
            commonChecks.VerifyAll();
        }

        [Fact]
        public void ValidItemDoesNotRaiseValidationMessage()
        {
            const string learnRefNumber = "123456789X";
            const string learnAimRef = "salddfkjeifdnase";

            var testDate = DateTime.Parse("2016-08-01");

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(testDate);
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(TypeOfFunding.AdultSkills);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var mockLars = new Mock<ILARSLearningCategory>();

            var larsItems = new List<ILARSLearningCategory>();
            larsItems.Add(mockLars.Object);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetCategoriesFor(learnAimRef))
                .Returns(larsItems);

            var mockMonitor = new Mock<IEmploymentStatusMonitoring>();
            mockMonitor
                .SetupGet(y => y.ESMType)
                .Returns(Monitoring.EmploymentStatus.Types.BenefitStatusIndicator);
            mockMonitor
                .SetupGet(y => y.ESMCode)
                .Returns(3);

            var monitors = new List<IEmploymentStatusMonitoring>();
            monitors.Add(mockMonitor.Object);

            var mockEmployment = new Mock<ILearnerEmploymentStatus>();
            mockEmployment
                .SetupGet(y => y.EmploymentStatusMonitorings)
                .Returns(monitors);

            var commonChecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonChecks
                .Setup(x => x.IsSteelWorkerRedundancyTraining(mockDelivery.Object))
                .Returns(false);
            commonChecks
                .Setup(x => x.HasQualifyingFunding(mockDelivery.Object, TypeOfFunding.AdultSkills, TypeOfFunding.OtherAdult, TypeOfFunding.EuropeanSocialFund))
                .Returns(true);
            commonChecks
                .Setup(x => x.HasQualifyingStart(mockDelivery.Object, LearnAimRef_81Rule.FirstViableDate, null))
                .Returns(true);
            commonChecks
                .Setup(x => x.GetEmploymentStatusOn(testDate, Moq.It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>()))
                .Returns(mockEmployment.Object);

            var sut = new LearnAimRef_81Rule(handler.Object, service.Object, commonChecks.Object);

            sut.Validate(mockLearner.Object);

            handler.VerifyAll();
            service.VerifyAll();
            commonChecks.VerifyAll();
        }

        public LearnAimRef_81Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var commonChecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            return new LearnAimRef_81Rule(handler.Object, service.Object, commonChecks.Object);
        }
    }
}
