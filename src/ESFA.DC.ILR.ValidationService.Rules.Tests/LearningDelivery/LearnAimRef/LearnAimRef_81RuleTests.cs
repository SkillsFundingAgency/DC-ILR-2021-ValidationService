using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_81RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var commonChecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRef_81Rule(null, service.Object, commonChecks.Object));
        }

        /// <summary>
        /// New rule with null lars service throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullLARSServiceThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonChecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRef_81Rule(handler.Object, null, commonChecks.Object));
        }

        /// <summary>
        /// New rule with null common checks throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullDerivedData07Throws()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRef_81Rule(handler.Object, service.Object, null));
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
            Assert.Equal("LearnAimRef_81", result);
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
            Assert.Equal(LearnAimRef_81Rule.Name, result);
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
        /// First viable date meets expectation.
        /// </summary>
        [Fact]
        public void FirstViableDateMeetsExpectation()
        {
            // arrange / act
            var result = LearnAimRef_81Rule.FirstViableDate;

            // assert
            Assert.Equal(DateTime.Parse("2016-08-01"), result);
        }

        /// <summary>
        /// Has disqualifying learning category meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(TypeOfLARSCategory.LegalEntitlementLevel2, false)]
        [InlineData(TypeOfLARSCategory.LicenseToPractice, true)]
        [InlineData(TypeOfLARSCategory.OnlyForLegalEntitlementAtLevel3, false)]
        [InlineData(TypeOfLARSCategory.WorkPlacementSFAFunded, false)]
        [InlineData(TypeOfLARSCategory.WorkPreparationSFATraineeships, false)]
        public void HasDisqualifyingLearningCategoryMeetsExpectation(int candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<ILARSLearningCategory>();
            mockItem
                .SetupGet(y => y.CategoryRef)
                .Returns(candidate);

            // act
            var result = sut.HasDisqualifyingLearningCategory(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Has disqualifying learning category for delivery meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData(null)]
        [InlineData("testAim1")]
        [InlineData("testAim2")]
        public void HasDisqualifyingLearningCategoryForDeliveryMeetsExpectation(string candidate)
        {
            // arrange
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

            // act
            var result = sut.HasDisqualifyingLearningCategory(mockDelivery.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
            commonChecks.VerifyAll();

            Assert.False(result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        [Fact]
        public void InvalidItemRaisesValidationMessage()
        {
            // arrange
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

            // it's this value that triggers the rule
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

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
            commonChecks.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        [Fact]
        public void ValidItemDoesNotRaiseValidationMessage()
        {
            // arrange
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

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
            commonChecks.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public LearnAimRef_81Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var commonChecks = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            return new LearnAimRef_81Rule(handler.Object, service.Object, commonChecks.Object);
        }
    }
}
