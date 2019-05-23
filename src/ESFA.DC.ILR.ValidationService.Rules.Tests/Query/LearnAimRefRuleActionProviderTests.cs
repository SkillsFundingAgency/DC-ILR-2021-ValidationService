using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Query
{
    public class LearnAimRefRuleActionProviderTests
    {
        /// <summary>
        /// New rule with null common operations throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullCommonOperationsThrows()
        {
            // arrange
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRefRuleActionProvider(null, derivedData11.Object));
        }

        /// <summary>
        /// New rule with null derived data throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullDerivedDataThrows()
        {
            // arrange
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnAimRefRuleActionProvider(commonOps.Object, null));
        }

        /// <summary>
        /// Apprenticeship minimum start meets expectation.
        /// </summary>
        [Fact]
        public void ApprenticeshipMinimumStartMeetsExpectation()
        {
            // arrange / act / assert
            Assert.Equal(DateTime.Parse("2011-08-01"), LearnAimRefRuleActionProvider.ApprenticeshipMinimumStart);
        }

        /// <summary>
        /// Unemployed maximum start meets expectation.
        /// </summary>
        [Fact]
        public void UnemployedMaximumStartMeetsExpectation()
        {
            // arrange / act / assert
            Assert.Equal(DateTime.Parse("2016-08-01"), LearnAimRefRuleActionProvider.UnemployedMaximumStart);
        }

        [Theory]
        [InlineData(TypeOfFunding.AdultSkills, 4)]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, 2)]
        [InlineData(TypeOfFunding.OtherAdult, 1)]
        [InlineData(TypeOfFunding.NotFundedByESFA, 2)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, 1)]
        [InlineData(TypeOfFunding.Other16To19, 1)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, 1)]
        [InlineData(TypeOfFunding.CommunityLearning, 1)]
        public void GetRoutinesMeetsExpectation(int candidate, int expectation)
        {
            // arrange
            var sut = NewService();

            // act
            var result = sut.GetRoutines(candidate);

            // assert
            Assert.Equal(expectation, result.Count);
        }

        /// <summary>
        /// In receipt of benefits at start meets expectation.
        /// </summary>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void InReceiptOfBenefitsAtStartMeetsExpectation(bool expectation)
        {
            // arrange
            var mockDelivery = new Mock<ILearningDelivery>();
            var employments = new ILearnerEmploymentStatus[] { };

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);
            derivedData11
                .Setup(x => x.IsAdultFundedOnBenefitsAtStartOfAim(mockDelivery.Object, employments))
                .Returns(expectation);

            var sut = new LearnAimRefRuleActionProvider(commonOps.Object, derivedData11.Object);

            // act
            var result = sut.InReceiptOfBenefitsAtStart(mockDelivery.Object, employments);

            // assert
            commonOps.VerifyAll();
            derivedData11.VerifyAll();

            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Is qualifying category olass meets expectation
        /// </summary>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsQualifyingCategoryOLASSMeetsExpectation(bool expectation)
        {
            // arrange
            var mockDelivery = new Mock<ILearningDelivery>();

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.IsRestart(mockDelivery.Object))
                .Returns(false);
            commonOps
                .Setup(x => x.IsLearnerInCustody(mockDelivery.Object))
                .Returns(expectation);

            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            var sut = new LearnAimRefRuleActionProvider(commonOps.Object, derivedData11.Object);

            // act
            var result = sut.IsQualifyingCategoryOLASS(mockDelivery.Object, null);

            // assert
            commonOps.VerifyAll();
            derivedData11.VerifyAll();

            Assert.Equal("OLASS_ADULT", result.Category);
            Assert.Equal(expectation, result.Passed);
        }

        /// <summary>
        /// Is qualifying category unemployed meets expectation
        /// </summary>
        /// <param name="isRestart">if set to <c>true</c> [is restart].</param>
        /// <param name="inApprenticeship">if set to <c>true</c> [in apprenticeship].</param>
        /// <param name="isInCustody">if set to <c>true</c> [is in custody].</param>
        /// <param name="qualifyingStart">if set to <c>true</c> [qualifying start].</param>
        /// <param name="isFundedOnBenefits">if set to <c>true</c> [is funded on benefits].</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(false, false, false, false, true, false)]
        [InlineData(false, false, false, true, true, true)]
        [InlineData(false, false, true, true, true, false)]
        [InlineData(false, true, true, true, true, false)]
        [InlineData(true, true, true, true, true, false)]
        [InlineData(true, true, true, true, false, false)]
        [InlineData(true, true, true, false, false, false)]
        [InlineData(true, true, false, false, false, false)]
        [InlineData(true, false, false, false, false, false)]
        public void IsQualifyingCategoryUnemployedMeetsExpectation(bool isRestart, bool inApprenticeship, bool isInCustody, bool qualifyingStart, bool isFundedOnBenefits, bool expectation)
        {
            // arrange
            var mockDelivery = new Mock<ILearningDelivery>();

            var commonOps = new Mock<IProvideRuleCommonOperations>();
            commonOps
                .Setup(x => x.IsRestart(mockDelivery.Object))
                .Returns(isRestart);
            commonOps
                .Setup(x => x.InApprenticeship(mockDelivery.Object))
                .Returns(inApprenticeship);
            commonOps
                .Setup(x => x.IsLearnerInCustody(mockDelivery.Object))
                .Returns(isInCustody);
            commonOps
                .Setup(x => x.HasQualifyingStart(mockDelivery.Object, DateTime.MinValue, DateTime.Parse("2016-08-01")))
                .Returns(qualifyingStart);

            var derivedData11 = new Mock<IDerivedData_11Rule>();
            derivedData11
                .Setup(x => x.IsAdultFundedOnBenefitsAtStartOfAim(mockDelivery.Object, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>()))
                .Returns(isFundedOnBenefits);

            var sut = new LearnAimRefRuleActionProvider(commonOps.Object, derivedData11.Object);

            // act
            var result = sut.IsQualifyingCategoryUnemployed(mockDelivery.Object, null);

            // assert
            Assert.Equal("UNEMPLOYED", result.Category);
            Assert.Equal(expectation, result.Passed);
        }

        /// <summary>
        /// Is qualifying category adult skills meets expectation
        /// </summary>
        /// <param name="isRestart">if set to <c>true</c> [is restart].</param>
        /// <param name="isInCustody">if set to <c>true</c> [is in custody].</param>
        /// <param name="inApprenticeship">if set to <c>true</c> [in apprenticeship].</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(false, false, false, true)]
        [InlineData(false, false, true, false)]
        [InlineData(false, true, true, false)]
        [InlineData(true, true, true, false)]
        [InlineData(true, true, false, false)]
        [InlineData(true, false, false, false)]
        public void IsQualifyingCategoryAdultSkillsMeetsExpectation(bool isRestart, bool isInCustody, bool inApprenticeship, bool expectation)
        {
            // arrange
            var mockDelivery = new Mock<ILearningDelivery>();

            var commonOps = new Mock<IProvideRuleCommonOperations>();
            commonOps
                .Setup(x => x.IsRestart(mockDelivery.Object))
                .Returns(isRestart);
            commonOps
                .Setup(x => x.IsLearnerInCustody(mockDelivery.Object))
                .Returns(isInCustody);
            commonOps
                .Setup(x => x.InApprenticeship(mockDelivery.Object))
                .Returns(inApprenticeship);

            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            var sut = new LearnAimRefRuleActionProvider(commonOps.Object, derivedData11.Object);

            // act
            var result = sut.IsQualifyingCategoryAdultSkills(mockDelivery.Object, null);

            // assert
            Assert.Equal("ADULT_SKILLS", result.Category);
            Assert.Equal(expectation, result.Passed);
        }

        /// <summary>
        /// Is qualifying category apprenticeship meets expectation
        /// </summary>
        /// <param name="isRestart">if set to <c>true</c> [is restart].</param>
        /// <param name="inStdApp">if set to <c>true</c> [in standard application].</param>
        /// <param name="inApprenticeship">if set to <c>true</c> [in apprenticeship].</param>
        /// <param name="isComponent">if set to <c>true</c> [is component].</param>
        /// <param name="qualifyingStart">if set to <c>true</c> [qualifying start].</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(false, false, true, true, true, true)]
        [InlineData(false, false, true, true, false, false)]
        public void IsQualifyingCategoryApprenticeshipMeetsExpectation(bool isRestart, bool inStdApp, bool inApprenticeship, bool isComponent, bool qualifyingStart, bool expectation)
        {
            // arrange
            var mockDelivery = new Mock<ILearningDelivery>();

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.IsRestart(mockDelivery.Object))
                .Returns(isRestart);
            commonOps
                .Setup(x => x.IsStandardApprencticeship(mockDelivery.Object))
                .Returns(inStdApp);
            commonOps
                .Setup(x => x.InApprenticeship(mockDelivery.Object))
                .Returns(inApprenticeship);
            commonOps
                .Setup(x => x.IsComponentOfAProgram(mockDelivery.Object))
                .Returns(isComponent);
            commonOps
                .Setup(x => x.HasQualifyingStart(mockDelivery.Object, DateTime.Parse("2011-08-01"), DateTime.MaxValue))
                .Returns(qualifyingStart);

            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            var sut = new LearnAimRefRuleActionProvider(commonOps.Object, derivedData11.Object);

            // act
            var result = sut.IsQualifyingCategoryApprenticeship(mockDelivery.Object, null);

            // assert
            commonOps.VerifyAll();
            derivedData11.VerifyAll();

            Assert.Equal("APPRENTICESHIPS", result.Category);
            Assert.Equal(expectation, result.Passed);
        }

        [Theory]
        [InlineData(false, false, false, false)]
        [InlineData(false, false, true, true)]
        [InlineData(true, false, false, false)]
        [InlineData(true, false, true, false)]
        [InlineData(true, true, true, false)]
        public void IsQualifyingCategoryApprencticeshipAnyMeetsExpectation(bool isRestart, bool isAdvLoan, bool isStdApp, bool expectation)
        {
            // arrange
            var mockDelivery = new Mock<ILearningDelivery>();

            var commonOps = new Mock<IProvideRuleCommonOperations>();
            commonOps
                .Setup(x => x.IsStandardApprencticeship(mockDelivery.Object))
                .Returns(isStdApp);
            commonOps
                .Setup(x => x.IsRestart(mockDelivery.Object))
                .Returns(isRestart);
            commonOps
                .Setup(x => x.IsAdvancedLearnerLoan(mockDelivery.Object))
                .Returns(isAdvLoan);

            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            var sut = new LearnAimRefRuleActionProvider(commonOps.Object, derivedData11.Object);

            // act
            var result = sut.IsQualifyingCategoryApprencticeshipAny(mockDelivery.Object, null);

            // assert
            Assert.Equal("ANY", result.Category);
            Assert.Equal(expectation, result.Passed);
        }

        [Theory]
        [InlineData(false, false, true)]
        [InlineData(false, true, false)]
        [InlineData(true, true, false)]
        [InlineData(true, false, false)]
        public void IsQualifyingCategoryOtherFundingAnyMeetsExpectation(bool isRestart, bool isAdvLoan, bool expectation)
        {
            // arrange
            var mockDelivery = new Mock<ILearningDelivery>();

            var commonOps = new Mock<IProvideRuleCommonOperations>();
            commonOps
                .Setup(x => x.IsRestart(mockDelivery.Object))
                .Returns(isRestart);
            commonOps
                .Setup(x => x.IsAdvancedLearnerLoan(mockDelivery.Object))
                .Returns(isAdvLoan);

            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            var sut = new LearnAimRefRuleActionProvider(commonOps.Object, derivedData11.Object);

            // act
            var result = sut.IsQualifyingCategoryOtherFundingAny(mockDelivery.Object, null);

            // assert
            Assert.Equal("ANY", result.Category);
            Assert.Equal(expectation, result.Passed);
        }

        /// <summary>
        /// Is qualifying category advanced learner loan meets expectation
        /// </summary>
        /// <param name="isRestart">if set to <c>true</c> [is restart].</param>
        /// <param name="isAdvLoan">if set to <c>true</c> [is adv loan].</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(false, false, false)]
        [InlineData(false, true, true)]
        [InlineData(true, true, false)]
        [InlineData(true, false, false)]
        public void IsQualifyingCategoryAdvancedLearnerLoanMeetsExpectation(bool isRestart, bool isAdvLoan, bool expectation)
        {
            // arrange
            var mockDelivery = new Mock<ILearningDelivery>();

            var commonOps = new Mock<IProvideRuleCommonOperations>();
            commonOps
                .Setup(x => x.IsRestart(mockDelivery.Object))
                .Returns(isRestart);
            commonOps
                .Setup(x => x.IsAdvancedLearnerLoan(mockDelivery.Object))
                .Returns(isAdvLoan);

            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            var sut = new LearnAimRefRuleActionProvider(commonOps.Object, derivedData11.Object);

            // act
            var result = sut.IsQualifyingCategoryAdvancedLearnerLoan(mockDelivery.Object, null);

            // assert
            Assert.Equal("ADV_LEARN_LOAN", result.Category);
            Assert.Equal(expectation, result.Passed);
        }

        /// <summary>
        /// Is qualifying category 16 to 19 efa meets expectation
        /// </summary>
        /// <param name="isRestart">if set to <c>true</c> [is restart].</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(false, true)]
        [InlineData(true, false)]
        public void IsQualifyingCategory16To19EFAMeetsExpectation(bool isRestart, bool expectation)
        {
            // arrange
            var mockDelivery = new Mock<ILearningDelivery>();

            var commonOps = new Mock<IProvideRuleCommonOperations>();
            commonOps
                .Setup(x => x.IsRestart(mockDelivery.Object))
                .Returns(isRestart);

            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            var sut = new LearnAimRefRuleActionProvider(commonOps.Object, derivedData11.Object);

            // act
            var result = sut.IsQualifyingCategory16To19EFA(mockDelivery.Object, null);

            // assert
            Assert.Equal("1619_EFA", result.Category);
            Assert.Equal(expectation, result.Passed);
        }

        /// <summary>
        /// Is qualifying category community learning meets expectation
        /// </summary>
        /// <param name="isRestart">if set to <c>true</c> [is restart].</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(false, true)]
        [InlineData(true, false)]
        public void IsQualifyingCategoryCommunityLearningMeetsExpectation(bool isRestart, bool expectation)
        {
            // arrange
            var mockDelivery = new Mock<ILearningDelivery>();

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.IsRestart(mockDelivery.Object))
                .Returns(isRestart);

            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            var sut = new LearnAimRefRuleActionProvider(commonOps.Object, derivedData11.Object);

            // act
            var result = sut.IsQualifyingCategoryCommunityLearning(mockDelivery.Object, null);

            // assert
            commonOps.VerifyAll();
            derivedData11.VerifyAll();

            Assert.Equal("COMM_LEARN", result.Category);
            Assert.Equal(expectation, result.Passed);
        }

        /// <summary>
        /// Is qualifying category esf meets expectation
        /// </summary>
        /// <param name="isRestart">if set to <c>true</c> [is restart].</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(false, true)]
        [InlineData(true, false)]
        public void IsQualifyingCategoryESFMeetsExpectation(bool isRestart, bool expectation)
        {
            // arrange
            var mockDelivery = new Mock<ILearningDelivery>();

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.IsRestart(mockDelivery.Object))
                .Returns(isRestart);

            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            var sut = new LearnAimRefRuleActionProvider(commonOps.Object, derivedData11.Object);

            // act
            var result = sut.IsQualifyingCategoryESF(mockDelivery.Object, null);

            // assert
            commonOps.VerifyAll();
            derivedData11.VerifyAll();

            Assert.Equal("ESF", result.Category);
            Assert.Equal(expectation, result.Passed);
        }

        // _________FM, LIC,   Appr, QS,   OnBens, StApr, AimCmp, AdvLL,
        [Theory]
        [InlineData(35, true, false, false, false, false, false, false, "OLASS_ADULT", false)]
        [InlineData(35, false, false, true, true, false, false, false, "UNEMPLOYED", false)]
        [InlineData(35, false, false, false, false, false, false, false, "ADULT_SKILLS", false)]
        [InlineData(35, false, true, true, false, false, true, false, "APPRENTICESHIPS", false)]
        [InlineData(36, false, true, true, false, false, true, false, "APPRENTICESHIPS", false)]
        [InlineData(36, true, true, true, true, true, true, false, "ANY", false)] // can't be adv loan, must be 'std appr'
        [InlineData(81, true, true, true, true, false, true, false, "ANY", false)] // can't be adv loan, turned off 'std appr' to make sure we we are not using it
        [InlineData(99, true, true, true, true, false, true, false, "ANY", false)] // can't be adv loan, turned off 'std appr' to make sure we we are not using
        [InlineData(99, true, true, true, true, true, true, true, "ADV_LEARN_LOAN", false)] // only interested in 'isAdvLLoan'
        [InlineData(25, true, true, true, true, true, true, true, "1619_EFA", false)] // try and trip on anything
        [InlineData(82, true, true, true, true, true, true, true, "1619_EFA", false)] // try and trip on anything
        [InlineData(10, true, true, true, true, true, true, true, "COMM_LEARN", false)] // try and trip on anything
        [InlineData(70, true, true, true, true, true, true, true, "ESF", false)] // try and trip on anything
        public void GetBranchingResultForMeetsExpectation(
            int fundModel,
            bool isLIC,
            bool inAppr,
            bool hasQS,
            bool isOnBenefits,
            bool isStdAppr,
            bool isAimComp,
            bool isAdvLLoan,
            string expectation,
            bool isOutOfScope)
        {
            // arrange
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(x => x.FundModel)
                .Returns(fundModel);

            var commonOps = new Mock<IProvideRuleCommonOperations>();
            commonOps
                .Setup(x => x.IsRestart(mockDelivery.Object))
                .Returns(false);
            commonOps
                .Setup(x => x.IsLearnerInCustody(mockDelivery.Object))
                .Returns(isLIC);
            commonOps
                .Setup(x => x.InApprenticeship(mockDelivery.Object))
                .Returns(inAppr);
            commonOps
                .Setup(x => x.HasQualifyingStart(mockDelivery.Object, It.IsAny<DateTime>(), It.IsAny<DateTime?>()))
                .Returns(hasQS);
            commonOps
                .Setup(x => x.IsStandardApprencticeship(mockDelivery.Object))
                .Returns(isStdAppr);
            commonOps
                .Setup(x => x.IsComponentOfAProgram(mockDelivery.Object))
                .Returns(isAimComp);
            commonOps
                .Setup(x => x.HasQualifyingFunding(mockDelivery.Object, It.IsAny<int>()))
                .Returns(true);
            commonOps
                .Setup(x => x.IsAdvancedLearnerLoan(mockDelivery.Object))
                .Returns(isAdvLLoan);

            var derivedData11 = new Mock<IDerivedData_11Rule>();
            derivedData11
                .Setup(x => x.IsAdultFundedOnBenefitsAtStartOfAim(mockDelivery.Object, It.IsAny<IReadOnlyCollection<ILearnerEmploymentStatus>>()))
                .Returns(isOnBenefits);

            var sut = new LearnAimRefRuleActionProvider(commonOps.Object, derivedData11.Object);

            // act
            var result = sut.GetBranchingResultFor(mockDelivery.Object, new Mock<ILearner>().Object);

            // assert
            Assert.Equal(expectation, result.Category);
            Assert.Equal(isOutOfScope, result.OutOfScope);
        }

        /// <summary>
        /// New service.
        /// </summary>
        /// <returns>a constructed and mocked up service</returns>
        public LearnAimRefRuleActionProvider NewService()
        {
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var derivedData11 = new Mock<IDerivedData_11Rule>(MockBehavior.Strict);

            return new LearnAimRefRuleActionProvider(commonOps.Object, derivedData11.Object);
        }
    }
}
