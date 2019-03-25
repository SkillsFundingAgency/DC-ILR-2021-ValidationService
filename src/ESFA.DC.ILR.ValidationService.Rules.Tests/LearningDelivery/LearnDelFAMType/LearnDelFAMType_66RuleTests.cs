using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using ESFA.DC.ILR.ValidationService.Utility;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMType
{
    /// <summary>
    /// from version 1.1 validation spread sheet
    /// </summary>
    public class LearnDelFAMType_66RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var mockService = new Mock<ILARSDataService>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnDelFAMType_66Rule(null, mockService.Object, mockDDRule07.Object, mockDDRule21.Object, mockDDRule28.Object, mockDDRule29.Object));
        }

        /// <summary>
        /// New rule with null lars service throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullLARSServiceThrows()
        {
            // arrange
            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnDelFAMType_66Rule(mockHandler.Object, null, mockDDRule07.Object, mockDDRule21.Object, mockDDRule28.Object, mockDDRule29.Object));
        }

        /// <summary>
        /// New rule with null derived data rule 07 throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullDerivedDataRule07Throws()
        {
            // arrange
            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var mockService = new Mock<ILARSDataService>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnDelFAMType_66Rule(mockHandler.Object, mockService.Object, null, mockDDRule21.Object, mockDDRule28.Object, mockDDRule29.Object));
        }

        /// <summary>
        /// New rule with null derived data rule 21 throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullDerivedDataRule21Throws()
        {
            // arrange
            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var mockService = new Mock<ILARSDataService>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnDelFAMType_66Rule(mockHandler.Object, mockService.Object, mockDDRule07.Object, null, mockDDRule28.Object, mockDDRule29.Object));
        }

        /// <summary>
        /// New rule with null derived data rule 28 throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullDerivedDataRule28Throws()
        {
            // arrange
            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var mockService = new Mock<ILARSDataService>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnDelFAMType_66Rule(mockHandler.Object, mockService.Object, mockDDRule07.Object, mockDDRule21.Object, null, mockDDRule29.Object));
        }

        /// <summary>
        /// New rule with null derived data rule 29 throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullDerivedDataRule29Throws()
        {
            // arrange
            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var mockService = new Mock<ILARSDataService>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnDelFAMType_66Rule(mockHandler.Object, mockService.Object, mockDDRule07.Object, mockDDRule21.Object, mockDDRule28.Object, null));
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
            Assert.Equal("LearnDelFAMType_66", result);
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
            Assert.Equal(RuleNameConstants.LearnDelFAMType_66, result);
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
        /// Last inviable date meets expectation.
        /// </summary>
        [Fact]
        public void LastInviableDateMeetsExpectation()
        {
            // arrange / act / assert
            Assert.Equal(DateTime.Parse("2017-07-31"), LearnDelFAMType_66Rule.LastInviableDate);
        }

        /// <summary>
        /// Is basic skills learner meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(TypeOfLARSBasicSkill.CertificateESOLS4L, false)]
        [InlineData(TypeOfLARSBasicSkill.CertificateESOLS4LSpeakListen, false)]
        [InlineData(TypeOfLARSBasicSkill.Certificate_AdultLiteracy, true)]
        [InlineData(TypeOfLARSBasicSkill.Certificate_AdultNumeracy, true)]
        [InlineData(TypeOfLARSBasicSkill.FreeStandingMathematicsQualification, true)]
        [InlineData(TypeOfLARSBasicSkill.FunctionalSkillsEnglish, true)]
        [InlineData(TypeOfLARSBasicSkill.FunctionalSkillsMathematics, true)]
        [InlineData(TypeOfLARSBasicSkill.GCSE_EnglishLanguage, true)]
        [InlineData(TypeOfLARSBasicSkill.GCSE_Mathematics, true)]
        [InlineData(TypeOfLARSBasicSkill.InternationalGCSEEnglishLanguage, true)]
        [InlineData(TypeOfLARSBasicSkill.InternationalGCSEMathematics, true)]
        [InlineData(TypeOfLARSBasicSkill.KeySkill_ApplicationOfNumbers, true)]
        [InlineData(TypeOfLARSBasicSkill.KeySkill_Communication, true)]
        [InlineData(TypeOfLARSBasicSkill.NonNQF_QCFS4LESOL, false)]
        [InlineData(TypeOfLARSBasicSkill.NonNQF_QCFS4LLiteracy, true)]
        [InlineData(TypeOfLARSBasicSkill.NonNQF_QCFS4LNumeracy, true)]
        [InlineData(TypeOfLARSBasicSkill.NotApplicable, false)]
        [InlineData(TypeOfLARSBasicSkill.OtherS4LNotLiteracyNumeracyOrESOL, false)]
        [InlineData(TypeOfLARSBasicSkill.QCFBasicSkillsEnglishLanguage, true)]
        [InlineData(TypeOfLARSBasicSkill.QCFBasicSkillsMathematics, true)]
        [InlineData(TypeOfLARSBasicSkill.QCFCertificateESOL, false)]
        [InlineData(TypeOfLARSBasicSkill.QCFESOLReading, false)]
        [InlineData(TypeOfLARSBasicSkill.QCFESOLSpeakListen, false)]
        [InlineData(TypeOfLARSBasicSkill.QCFESOLWriting, false)]
        [InlineData(TypeOfLARSBasicSkill.UnitESOLReading, false)]
        [InlineData(TypeOfLARSBasicSkill.UnitESOLSpeakListen, false)]
        [InlineData(TypeOfLARSBasicSkill.UnitESOLWriting, false)]
        [InlineData(TypeOfLARSBasicSkill.UnitQCFBasicSkillsEnglishLanguage, true)]
        [InlineData(TypeOfLARSBasicSkill.UnitQCFBasicSkillsMathematics, true)]
        [InlineData(TypeOfLARSBasicSkill.UnitsOfTheCertificate_AdultLiteracy, true)]
        [InlineData(TypeOfLARSBasicSkill.UnitsOfTheCertificate_AdultNumeracy, true)]
        [InlineData(TypeOfLARSBasicSkill.UnitsOfTheCertificate_ESOLS4L, false)]
        [InlineData(TypeOfLARSBasicSkill.Unknown, false)]
        [InlineData(null, false)]
        public void IsBasicSkillsLearnerMeetsExpectation(int? candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILARSAnnualValue>();
            mockDelivery
                .SetupGet(y => y.BasicSkillsType)
                .Returns(candidate);

            // act
            var result = sut.IsBasicSkillsLearner(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Is learner in custody with learning delivery fam meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(Monitoring.Delivery.OLASSOffendersInCustody, true)]
        [InlineData(Monitoring.Delivery.FullyFundedLearningAim, false)]
        [InlineData(Monitoring.Delivery.InReceiptOfLowWages, false)]
        [InlineData(Monitoring.Delivery.MandationToSkillsTraining, false)]
        [InlineData(Monitoring.Delivery.ReleasedOnTemporaryLicence, false)]
        [InlineData(Monitoring.Delivery.SteelIndustriesRedundancyTraining, false)]
        public void IsLearnerInCustodyMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<ILearningDeliveryFAM>();
            mockItem
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(candidate.Substring(0, 3));
            mockItem
                .SetupGet(y => y.LearnDelFAMCode)
                .Returns(candidate.Substring(3));

            // act
            var result = sut.IsLearnerInCustody(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Is released on temporary licence with learning delivery fam meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(Monitoring.Delivery.OLASSOffendersInCustody, false)]
        [InlineData(Monitoring.Delivery.FullyFundedLearningAim, false)]
        [InlineData(Monitoring.Delivery.InReceiptOfLowWages, false)]
        [InlineData(Monitoring.Delivery.MandationToSkillsTraining, false)]
        [InlineData(Monitoring.Delivery.ReleasedOnTemporaryLicence, true)]
        [InlineData(Monitoring.Delivery.SteelIndustriesRedundancyTraining, false)]
        public void IsReleasedOnTemporaryLicenceMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<ILearningDeliveryFAM>();
            mockItem
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(candidate.Substring(0, 3));
            mockItem
                .SetupGet(y => y.LearnDelFAMCode)
                .Returns(candidate.Substring(3));

            // act
            var result = sut.IsReleasedOnTemporaryLicence(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Is restart with learning delivery fam meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(Monitoring.Delivery.Types.AdvancedLearnerLoansBursaryFunding, false)]
        [InlineData(Monitoring.Delivery.Types.AdvancedLearnerLoan, false)]
        [InlineData(Monitoring.Delivery.Types.ApprenticeshipContract, false)]
        [InlineData(Monitoring.Delivery.Types.CommunityLearningProvision, false)]
        [InlineData(Monitoring.Delivery.Types.EligibilityForEnhancedApprenticeshipFunding, false)]
        [InlineData(Monitoring.Delivery.Types.FamilyEnglishMathsAndLanguage, false)]
        [InlineData(Monitoring.Delivery.Types.FullOrCoFunding, false)]
        [InlineData(Monitoring.Delivery.Types.HEMonitoring, false)]
        [InlineData(Monitoring.Delivery.Types.HouseholdSituation, false)]
        [InlineData(Monitoring.Delivery.Types.Learning, false)]
        [InlineData(Monitoring.Delivery.Types.LearningSupportFunding, false)]
        [InlineData(Monitoring.Delivery.Types.NationalSkillsAcademy, false)]
        [InlineData(Monitoring.Delivery.Types.PercentageOfOnlineDelivery, false)]
        [InlineData(Monitoring.Delivery.Types.Restart, true)]
        [InlineData(Monitoring.Delivery.Types.SourceOfFunding, false)]
        [InlineData(Monitoring.Delivery.Types.WorkProgrammeParticipation, false)]
        public void IsRestartMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<ILearningDeliveryFAM>();
            mockItem
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(candidate);

            // act
            var result = sut.IsRestart(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Is steel worker redundancy training with learning delivery fam meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(Monitoring.Delivery.OLASSOffendersInCustody, false)]
        [InlineData(Monitoring.Delivery.FullyFundedLearningAim, false)]
        [InlineData(Monitoring.Delivery.InReceiptOfLowWages, false)]
        [InlineData(Monitoring.Delivery.MandationToSkillsTraining, false)]
        [InlineData(Monitoring.Delivery.ReleasedOnTemporaryLicence, false)]
        [InlineData(Monitoring.Delivery.SteelIndustriesRedundancyTraining, true)]
        public void IsSteelWorkerRedundancyTrainingMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<ILearningDeliveryFAM>();
            mockItem
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(candidate.Substring(0, 3));
            mockItem
                .SetupGet(y => y.LearnDelFAMCode)
                .Returns(candidate.Substring(3));

            // act
            var result = sut.IsSteelWorkerRedundancyTraining(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// In receipt of low wages with learning delivery fam meets expectation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(Monitoring.Delivery.OLASSOffendersInCustody, false)]
        [InlineData(Monitoring.Delivery.FullyFundedLearningAim, false)]
        [InlineData(Monitoring.Delivery.InReceiptOfLowWages, true)]
        [InlineData(Monitoring.Delivery.MandationToSkillsTraining, false)]
        [InlineData(Monitoring.Delivery.ReleasedOnTemporaryLicence, false)]
        [InlineData(Monitoring.Delivery.SteelIndustriesRedundancyTraining, false)]
        public void InReceiptOfLowWagesMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<ILearningDeliveryFAM>();
            mockItem
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(candidate.Substring(0, 3));
            mockItem
                .SetupGet(y => y.LearnDelFAMCode)
                .Returns(candidate.Substring(3));

            // act
            var result = sut.InReceiptOfLowWages(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Is adult funded unemployed with other state benefits meets expectation
        /// </summary>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void IsAdultFundedUnemployedWithOtherStateBenefitsMeetsExpectation(bool expectation)
        {
            // arrange
            var mockItem = new Mock<ILearner>();
            var delivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);

            mockDDRule21
                .Setup(x => x.IsAdultFundedUnemployedWithOtherStateBenefits(delivery.Object, mockItem.Object))
                .Returns(expectation);

            var sut = new LearnDelFAMType_66Rule(handler.Object, service.Object, mockDDRule07.Object, mockDDRule21.Object, mockDDRule28.Object, mockDDRule29.Object);

            // act
            var result = sut.IsAdultFundedUnemployedWithOtherStateBenefits(delivery.Object, mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
            handler.VerifyAll();
            service.VerifyAll();
            mockDDRule07.VerifyAll();
            mockDDRule21.VerifyAll();
            mockDDRule28.VerifyAll();
            mockDDRule29.VerifyAll();
        }

        /// <summary>
        /// Is adult funded unemployed with benefits meets expectation
        /// </summary>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void IsAdultFundedUnemployedWithBenefitsMeetsExpectation(bool expectation)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();
            var learner = new Mock<ILearner>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);

            mockDDRule28
                .Setup(x => x.IsAdultFundedUnemployedWithBenefits(delivery.Object, learner.Object))
                .Returns(expectation);

            var sut = new LearnDelFAMType_66Rule(handler.Object, service.Object, mockDDRule07.Object, mockDDRule21.Object, mockDDRule28.Object, mockDDRule29.Object);

            // act
            var result = sut.IsAdultFundedUnemployedWithBenefits(delivery.Object, learner.Object);

            // assert
            Assert.Equal(expectation, result);
            handler.VerifyAll();
            service.VerifyAll();
            mockDDRule07.VerifyAll();
            mockDDRule21.VerifyAll();
            mockDDRule28.VerifyAll();
            mockDDRule29.VerifyAll();
        }

        /// <summary>
        /// Is inflexible element of training aim meets expectation
        /// </summary>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void IsInflexibleElementOfTrainingAimMeetsExpectation(bool expectation)
        {
            // arrange
            var mockItem = new Mock<ILearner>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);

            mockDDRule29
                .Setup(x => x.IsInflexibleElementOfTrainingAim(mockItem.Object))
                .Returns(expectation);

            var sut = new LearnDelFAMType_66Rule(handler.Object, service.Object, mockDDRule07.Object, mockDDRule21.Object, mockDDRule28.Object, mockDDRule29.Object);

            // act
            var result = sut.IsInflexibleElementOfTrainingAim(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
            handler.VerifyAll();
            service.VerifyAll();
            mockDDRule07.VerifyAll();
            mockDDRule21.VerifyAll();
            mockDDRule28.VerifyAll();
            mockDDRule29.VerifyAll();
        }

        /// <summary>
        /// Is apprenticeship meets expectation
        /// </summary>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void IsApprenticeshipMeetsExpectation(bool expectation)
        {
            // arrange
            var mockItem = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);

            mockDDRule07
                .Setup(x => x.IsApprenticeship(null))
                .Returns(expectation);

            var sut = new LearnDelFAMType_66Rule(handler.Object, service.Object, mockDDRule07.Object, mockDDRule21.Object, mockDDRule28.Object, mockDDRule29.Object);

            // act
            var result = sut.IsApprenticeship(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
            handler.VerifyAll();
            service.VerifyAll();
            mockDDRule07.VerifyAll();
            mockDDRule21.VerifyAll();
            mockDDRule28.VerifyAll();
            mockDDRule29.VerifyAll();
        }

        /// <summary>
        /// Is adult funding meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(TypeOfFunding.AdultSkills, true)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, false)]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, false)]
        [InlineData(TypeOfFunding.CommunityLearning, false)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, false)]
        [InlineData(TypeOfFunding.NotFundedByESFA, false)]
        [InlineData(TypeOfFunding.Other16To19, false)]
        [InlineData(TypeOfFunding.OtherAdult, false)]
        public void IsAdultFundingMeetsExpectation(int candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(candidate);

            // act
            var result = sut.IsAdultFunding(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Is viable start meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("2016-08-01", false)]
        [InlineData("2017-07-31", false)]
        [InlineData("2017-08-01", true)]
        [InlineData("2017-09-14", true)]
        public void IsViableStartMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(candidate));

            // act
            var result = sut.IsViableStart(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Is target age group meets expectation
        /// </summary>
        /// <param name="birthDate">The birth date.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("1994-08-01", "2018-04-18", false)]
        [InlineData("1994-08-01", "2018-07-31", false)]
        [InlineData("1994-08-01", "2018-08-01", true)]
        [InlineData("1994-08-01", "2018-09-07", true)]
        public void IsTargetAgeGroupMeetsExpectation(string birthDate, string startDate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(y => y.DateOfBirthNullable)
                .Returns(DateTime.Parse(birthDate));

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(startDate));

            // act
            var result = sut.IsTargetAgeGroup(mockLearner.Object, mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Is fully funded with learning delivery fam meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(Monitoring.Delivery.OLASSOffendersInCustody, false)]
        [InlineData(Monitoring.Delivery.FullyFundedLearningAim, true)]
        [InlineData(Monitoring.Delivery.InReceiptOfLowWages, false)]
        [InlineData(Monitoring.Delivery.MandationToSkillsTraining, false)]
        [InlineData(Monitoring.Delivery.ReleasedOnTemporaryLicence, false)]
        [InlineData(Monitoring.Delivery.SteelIndustriesRedundancyTraining, false)]
        public void IsFullyFundedMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<ILearningDeliveryFAM>();
            mockItem
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(candidate.Substring(0, 3));
            mockItem
                .SetupGet(y => y.LearnDelFAMCode)
                .Returns(candidate.Substring(3));

            // act
            var result = sut.IsFullyFunded(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Is excluded for inflexible element of training aim
        /// </summary>
        [Fact]
        public void IsExcludedForInflexibleElementOfTrainingAim()
        {
            // arrange
            var mockItem = new Mock<ILearner>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);

            mockDDRule29
                .Setup(x => x.IsInflexibleElementOfTrainingAim(mockItem.Object))
                .Returns(true);

            var sut = new LearnDelFAMType_66Rule(handler.Object, service.Object, mockDDRule07.Object, mockDDRule21.Object, mockDDRule28.Object, mockDDRule29.Object);

            // act
            var result = sut.IsExcluded(mockItem.Object);

            // assert
            Assert.True(result);
            handler.VerifyAll();
            service.VerifyAll();
            mockDDRule07.VerifyAll();
            mockDDRule21.VerifyAll();
            mockDDRule28.VerifyAll();
            mockDDRule29.VerifyAll();
        }

        /// <summary>
        /// Is excluded for apprenticeship
        /// </summary>
        [Fact]
        public void IsExcludedForApprenticeship()
        {
            // arrange
            const int progType = 23;
            var mockDel = new Mock<ILearningDelivery>();
            mockDel
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(progType);

            var deliveries = Collection.Empty<ILearningDelivery>();
            deliveries.Add(mockDel.Object);

            var mockItem = new Mock<ILearner>();
            mockItem
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries.AsSafeReadOnlyList());

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);

            mockDDRule29
                .Setup(x => x.IsInflexibleElementOfTrainingAim(mockItem.Object))
                .Returns(false);
            mockDDRule07
                .Setup(x => x.IsApprenticeship(progType))
                .Returns(true);

            var sut = new LearnDelFAMType_66Rule(handler.Object, service.Object, mockDDRule07.Object, mockDDRule21.Object, mockDDRule28.Object, mockDDRule29.Object);

            // act
            var result = sut.IsExcluded(mockItem.Object);

            // assert
            Assert.True(result);
            handler.VerifyAll();
            service.VerifyAll();
            mockDDRule07.VerifyAll();
            mockDDRule21.VerifyAll();
            mockDDRule28.VerifyAll();
            mockDDRule29.VerifyAll();
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        [Fact]
        public void InvalidItemRaisesValidationMessage()
        {
            // arrange
            const string LearnRefNumber = "123456789X";
            const string learnAimRef = "salddfkjeifdnase";

            var mockFAM = new Mock<ILearningDeliveryFAM>();
            mockFAM
                .SetupGet(x => x.LearnDelFAMType)
                .Returns(Monitoring.Delivery.Types.FullOrCoFunding);
            mockFAM
                .SetupGet(x => x.LearnDelFAMCode)
                .Returns("1");

            var fams = Collection.Empty<ILearningDeliveryFAM>();
            fams.Add(mockFAM.Object);

            var testDate = DateTime.Parse("2017-08-01");
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
            mockDelivery
                .SetupGet(y => y.LearningDeliveryFAMs)
                .Returns(fams.AsSafeReadOnlyList());

            var deliveries = Collection.Empty<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var birthDate = DateTime.Parse("1993-07-01");
            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.DateOfBirthNullable)
                .Returns(birthDate);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries.AsSafeReadOnlyList());

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(RuleNameConstants.LearnDelFAMType_66, LearnRefNumber, 0, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
               .Setup(x => x.BuildErrorMessageParameter("FundModel", 35))
               .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
              .Setup(x => x.BuildErrorMessageParameter("LearnDelFAMType", "FFI"))
              .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
              .Setup(x => x.BuildErrorMessageParameter("LearnDelFAMCode", "1"))
              .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
              .Setup(x => x.BuildErrorMessageParameter("LearnStartDate", AbstractRule.AsRequiredCultureDate(testDate)))
              .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
              .Setup(x => x.BuildErrorMessageParameter("DateOfBirth", AbstractRule.AsRequiredCultureDate(birthDate)))
              .Returns(new Mock<IErrorMessageParameter>().Object);

            var mock = new Mock<ILARSLearningDelivery>();
            mock
                .SetupGet(x => x.NotionalNVQLevelv2)
                .Returns(LARSNotionalNVQLevelV2.EntryLevel);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetDeliveryFor(learnAimRef))
                .Returns(mock.Object);

            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            mockDDRule21
                .Setup(x => x.IsAdultFundedUnemployedWithOtherStateBenefits(mockDelivery.Object, mockLearner.Object))
                .Returns(false);

            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            mockDDRule28
                .Setup(x => x.IsAdultFundedUnemployedWithBenefits(mockDelivery.Object, mockLearner.Object))
                .Returns(false);

            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);

            var sut = new LearnDelFAMType_66Rule(handler.Object, service.Object, mockDDRule07.Object, mockDDRule21.Object, mockDDRule28.Object, mockDDRule29.Object);

            // act
            sut.ValidateDeliveries(mockLearner.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
            mockDDRule07.VerifyAll();
            mockDDRule21.VerifyAll();
            mockDDRule28.VerifyAll();
            mockDDRule29.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// the conditions here will get you to the final check which will return false for 'IsEarlyStageNVQ'
        /// </summary>
        [Fact]
        public void ValidItemDoesNotRaiseValidationMessage()
        {
            // arrange
            const string LearnRefNumber = "123456789X";
            const string learnAimRef = "salddfkjeifdnase";

            var mockFAM = new Mock<ILearningDeliveryFAM>();
            mockFAM
                .SetupGet(x => x.LearnDelFAMType)
                .Returns(Monitoring.Delivery.Types.FullOrCoFunding);
            mockFAM
                .SetupGet(x => x.LearnDelFAMCode)
                .Returns("1");

            var fams = Collection.Empty<ILearningDeliveryFAM>();
            fams.Add(mockFAM.Object);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse("2017-08-01"));
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(TypeOfFunding.AdultSkills);
            mockDelivery
                .SetupGet(y => y.LearningDeliveryFAMs)
                .Returns(fams.AsSafeReadOnlyList());

            var deliveries = Collection.Empty<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.DateOfBirthNullable)
                .Returns(DateTime.Parse("1993-07-01"));
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries.AsSafeReadOnlyList());

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetDeliveryFor(learnAimRef))
                .Returns((ILARSLearningDelivery)null);

            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            mockDDRule21
                .Setup(x => x.IsAdultFundedUnemployedWithOtherStateBenefits(mockDelivery.Object, mockLearner.Object))
                .Returns(false);

            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            mockDDRule28
                .Setup(x => x.IsAdultFundedUnemployedWithBenefits(mockDelivery.Object, mockLearner.Object))
                .Returns(false);

            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);

            var sut = new LearnDelFAMType_66Rule(handler.Object, service.Object, mockDDRule07.Object, mockDDRule21.Object, mockDDRule28.Object, mockDDRule29.Object);

            // act
            sut.ValidateDeliveries(mockLearner.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
            mockDDRule07.VerifyAll();
            mockDDRule21.VerifyAll();
            mockDDRule28.VerifyAll();
            mockDDRule29.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public LearnDelFAMType_66Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);

            return new LearnDelFAMType_66Rule(handler.Object, service.Object, mockDDRule07.Object, mockDDRule21.Object, mockDDRule28.Object, mockDDRule29.Object);
        }
    }
}
