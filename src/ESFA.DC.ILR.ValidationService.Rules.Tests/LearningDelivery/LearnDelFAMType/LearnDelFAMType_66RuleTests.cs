using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_66RuleTests
    {
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("LearnDelFAMType_66", result);
        }

        [Fact]
        public void LastInviableDateMeetsExpectation()
        {
            Assert.Equal(DateTime.Parse("2017-07-31"), LearnDelFAMType_66Rule.LastInviableDate);
        }

        [Theory]
        [InlineData(LARSBasicSkills.CertificateESOLS4L, false)]
        [InlineData(LARSBasicSkills.CertificateESOLS4LSpeakListen, false)]
        [InlineData(LARSBasicSkills.Certificate_AdultLiteracy, true)]
        [InlineData(LARSBasicSkills.Certificate_AdultNumeracy, true)]
        [InlineData(LARSBasicSkills.FreeStandingMathematicsQualification, true)]
        [InlineData(LARSBasicSkills.FunctionalSkillsEnglish, true)]
        [InlineData(LARSBasicSkills.FunctionalSkillsMathematics, true)]
        [InlineData(LARSBasicSkills.GCSE_EnglishLanguage, true)]
        [InlineData(LARSBasicSkills.GCSE_Mathematics, true)]
        [InlineData(LARSBasicSkills.InternationalGCSEEnglishLanguage, true)]
        [InlineData(LARSBasicSkills.InternationalGCSEMathematics, true)]
        [InlineData(LARSBasicSkills.KeySkill_ApplicationOfNumbers, true)]
        [InlineData(LARSBasicSkills.KeySkill_Communication, true)]
        [InlineData(LARSBasicSkills.NonNQF_QCFS4LESOL, false)]
        [InlineData(LARSBasicSkills.NonNQF_QCFS4LLiteracy, true)]
        [InlineData(LARSBasicSkills.NonNQF_QCFS4LNumeracy, true)]
        [InlineData(LARSBasicSkills.NotApplicable, false)]
        [InlineData(LARSBasicSkills.OtherS4LNotLiteracyNumeracyOrESOL, false)]
        [InlineData(LARSBasicSkills.QCFBasicSkillsEnglishLanguage, true)]
        [InlineData(LARSBasicSkills.QCFBasicSkillsMathematics, true)]
        [InlineData(LARSBasicSkills.QCFCertificateESOL, false)]
        [InlineData(LARSBasicSkills.QCFESOLReading, false)]
        [InlineData(LARSBasicSkills.QCFESOLSpeakListen, false)]
        [InlineData(LARSBasicSkills.QCFESOLWriting, false)]
        [InlineData(LARSBasicSkills.UnitESOLReading, false)]
        [InlineData(LARSBasicSkills.UnitESOLSpeakListen, false)]
        [InlineData(LARSBasicSkills.UnitESOLWriting, false)]
        [InlineData(LARSBasicSkills.UnitQCFBasicSkillsEnglishLanguage, true)]
        [InlineData(LARSBasicSkills.UnitQCFBasicSkillsMathematics, true)]
        [InlineData(LARSBasicSkills.UnitsOfTheCertificate_AdultLiteracy, true)]
        [InlineData(LARSBasicSkills.UnitsOfTheCertificate_AdultNumeracy, true)]
        [InlineData(LARSBasicSkills.UnitsOfTheCertificate_ESOLS4L, false)]
        [InlineData(LARSBasicSkills.Unknown, false)]
        [InlineData(null, false)]
        public void IsBasicSkillsLearnerMeetsExpectation(int? candidate, bool expectation)
        {
            var sut = NewRule();
            var mockDelivery = new Mock<ILARSAnnualValue>();
            mockDelivery
                .SetupGet(y => y.BasicSkillsType)
                .Returns(candidate);

            var result = sut.IsBasicSkillsLearner(mockDelivery.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(Monitoring.Delivery.OLASSOffendersInCustody, true)]
        [InlineData(Monitoring.Delivery.FullyFundedLearningAim, false)]
        [InlineData(Monitoring.Delivery.InReceiptOfLowWages, false)]
        [InlineData(Monitoring.Delivery.MandationToSkillsTraining, false)]
        [InlineData(Monitoring.Delivery.ReleasedOnTemporaryLicence, false)]
        [InlineData(Monitoring.Delivery.SteelIndustriesRedundancyTraining, false)]
        public void IsLearnerInCustodyMeetsExpectation(string candidate, bool expectation)
        {
            var sut = NewRule();
            var mockItem = new Mock<ILearningDeliveryFAM>();
            mockItem
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(candidate.Substring(0, 3));
            mockItem
                .SetupGet(y => y.LearnDelFAMCode)
                .Returns(candidate.Substring(3));

            var result = sut.IsLearnerInCustody(mockItem.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(Monitoring.Delivery.OLASSOffendersInCustody, false)]
        [InlineData(Monitoring.Delivery.FullyFundedLearningAim, false)]
        [InlineData(Monitoring.Delivery.InReceiptOfLowWages, false)]
        [InlineData(Monitoring.Delivery.MandationToSkillsTraining, false)]
        [InlineData(Monitoring.Delivery.ReleasedOnTemporaryLicence, true)]
        [InlineData(Monitoring.Delivery.SteelIndustriesRedundancyTraining, false)]
        public void IsReleasedOnTemporaryLicenceMeetsExpectation(string candidate, bool expectation)
        {
            var sut = NewRule();
            var mockItem = new Mock<ILearningDeliveryFAM>();
            mockItem
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(candidate.Substring(0, 3));
            mockItem
                .SetupGet(y => y.LearnDelFAMCode)
                .Returns(candidate.Substring(3));

            var result = sut.IsReleasedOnTemporaryLicence(mockItem.Object);

            Assert.Equal(expectation, result);
        }

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
            var sut = NewRule();
            var mockItem = new Mock<ILearningDeliveryFAM>();
            mockItem
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(candidate);

            var result = sut.IsRestart(mockItem.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(Monitoring.Delivery.OLASSOffendersInCustody, false)]
        [InlineData(Monitoring.Delivery.FullyFundedLearningAim, false)]
        [InlineData(Monitoring.Delivery.InReceiptOfLowWages, false)]
        [InlineData(Monitoring.Delivery.MandationToSkillsTraining, false)]
        [InlineData(Monitoring.Delivery.ReleasedOnTemporaryLicence, false)]
        [InlineData(Monitoring.Delivery.SteelIndustriesRedundancyTraining, true)]
        public void IsSteelWorkerRedundancyTrainingMeetsExpectation(string candidate, bool expectation)
        {
            var sut = NewRule();
            var mockItem = new Mock<ILearningDeliveryFAM>();
            mockItem
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(candidate.Substring(0, 3));
            mockItem
                .SetupGet(y => y.LearnDelFAMCode)
                .Returns(candidate.Substring(3));

            var result = sut.IsSteelWorkerRedundancyTraining(mockItem.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(Monitoring.Delivery.OLASSOffendersInCustody, false)]
        [InlineData(Monitoring.Delivery.FullyFundedLearningAim, false)]
        [InlineData(Monitoring.Delivery.InReceiptOfLowWages, true)]
        [InlineData(Monitoring.Delivery.MandationToSkillsTraining, false)]
        [InlineData(Monitoring.Delivery.ReleasedOnTemporaryLicence, false)]
        [InlineData(Monitoring.Delivery.SteelIndustriesRedundancyTraining, false)]
        public void InReceiptOfLowWagesMeetsExpectation(string candidate, bool expectation)
        {
            var sut = NewRule();
            var mockItem = new Mock<ILearningDeliveryFAM>();
            mockItem
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(candidate.Substring(0, 3));
            mockItem
                .SetupGet(y => y.LearnDelFAMCode)
                .Returns(candidate.Substring(3));

            var result = sut.InReceiptOfLowWages(mockItem.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void IsAdultFundedUnemployedWithOtherStateBenefitsMeetsExpectation(bool expectation)
        {
            var mockItem = new Mock<ILearner>();
            var delivery = new Mock<ILearningDelivery>();
            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);

            mockDDRule21
                .Setup(x => x.IsAdultFundedUnemployedWithOtherStateBenefits(delivery.Object, mockItem.Object))
                .Returns(expectation);

            var result = NewRule(mockDDRule21: mockDDRule21.Object).IsAdultFundedUnemployedWithOtherStateBenefits(delivery.Object, mockItem.Object);

            Assert.Equal(expectation, result);
            mockDDRule21.VerifyAll();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void IsAdultFundedUnemployedWithBenefitsMeetsExpectation(bool expectation)
        {
            var delivery = new Mock<ILearningDelivery>();
            var learner = new Mock<ILearner>();
            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);

            mockDDRule28
                .Setup(x => x.IsAdultFundedUnemployedWithBenefits(delivery.Object, learner.Object))
                .Returns(expectation);

            var result = NewRule(mockDDRule28: mockDDRule28.Object).IsAdultFundedUnemployedWithBenefits(delivery.Object, learner.Object);

            Assert.Equal(expectation, result);
            mockDDRule28.VerifyAll();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void IsInflexibleElementOfTrainingAimMeetsExpectation(bool expectation)
        {
            var mockItem = new Mock<ILearningDelivery>();
            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);

            mockDDRule29
                .Setup(x => x.IsInflexibleElementOfTrainingAimLearningDelivery(mockItem.Object))
                .Returns(expectation);

            var result = NewRule(mockDDRule29: mockDDRule29.Object).IsInflexibleElementOfTrainingAim(mockItem.Object);

            Assert.Equal(expectation, result);
            mockDDRule29.VerifyAll();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void IsApprenticeshipMeetsExpectation(bool expectation)
        {
            var mockItem = new Mock<ILearningDelivery>();
            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);

            mockDDRule07
                .Setup(x => x.IsApprenticeship(null))
                .Returns(expectation);

            var result = NewRule(mockDDRule07: mockDDRule07.Object).IsApprenticeship(mockItem.Object);

            Assert.Equal(expectation, result);
            mockDDRule07.VerifyAll();
        }

        [Theory]
        [InlineData(FundModels.AdultSkills, true)]
        [InlineData(FundModels.Age16To19ExcludingApprenticeships, false)]
        [InlineData(FundModels.ApprenticeshipsFrom1May2017, false)]
        [InlineData(FundModels.CommunityLearning, false)]
        [InlineData(FundModels.EuropeanSocialFund, false)]
        [InlineData(FundModels.NotFundedByESFA, false)]
        [InlineData(FundModels.Other16To19, false)]
        [InlineData(FundModels.OtherAdult, false)]
        public void IsAdultFundingMeetsExpectation(int candidate, bool expectation)
        {
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(candidate);

            var result = sut.IsAdultFunding(mockDelivery.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData("2016-08-01", false)]
        [InlineData("2017-07-31", false)]
        [InlineData("2017-08-01", true)]
        [InlineData("2017-09-14", true)]
        public void IsViableStartMeetsExpectation(string candidate, bool expectation)
        {
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(candidate));

            var result = sut.IsViableStart(mockDelivery.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData("1994-08-01", "2018-04-18", 23, false)]
        [InlineData("1994-08-01", "2018-07-31", 23, false)]
        [InlineData("1994-08-01", "2018-08-01", 24, true)]
        [InlineData("1994-08-01", "2018-09-07", 24, true)]
        public void IsTargetAgeGroupMeetsExpectation(string birthDate, string startDate, int ageInYears, bool expectation)
        {
            DateTime dateOfBirth = DateTime.Parse(birthDate);
            DateTime learnStartDate = DateTime.Parse(startDate);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(y => y.DateOfBirthNullable)
                .Returns(dateOfBirth);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(learnStartDate);

            var mockDateTimeQueryService = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            mockDateTimeQueryService.Setup(x => x.YearsBetween(dateOfBirth, learnStartDate)).Returns(ageInYears);

            var result = NewRule(mockDateTimeQueryService: mockDateTimeQueryService.Object).IsTargetAgeGroup(mockLearner.Object, mockDelivery.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(Monitoring.Delivery.OLASSOffendersInCustody, false)]
        [InlineData(Monitoring.Delivery.FullyFundedLearningAim, true)]
        [InlineData(Monitoring.Delivery.InReceiptOfLowWages, false)]
        [InlineData(Monitoring.Delivery.MandationToSkillsTraining, false)]
        [InlineData(Monitoring.Delivery.ReleasedOnTemporaryLicence, false)]
        [InlineData(Monitoring.Delivery.SteelIndustriesRedundancyTraining, false)]
        public void IsFullyFundedMeetsExpectation(string candidate, bool expectation)
        {
            var sut = NewRule();
            var mockItem = new Mock<ILearningDeliveryFAM>();
            mockItem
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(candidate.Substring(0, 3));
            mockItem
                .SetupGet(y => y.LearnDelFAMCode)
                .Returns(candidate.Substring(3));

            var result = sut.IsFullyFunded(mockItem.Object);

            Assert.Equal(expectation, result);
        }

        [Fact]
        public void IsExcludedForInflexibleElementOfTrainingAim()
        {
            var mockItem = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);
            var mockDateTimeQueryService = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            mockDDRule29
                .Setup(x => x.IsInflexibleElementOfTrainingAimLearningDelivery(mockItem.Object))
                .Returns(true);

            var sut = new LearnDelFAMType_66Rule(
                handler.Object,
                service.Object,
                mockDDRule07.Object,
                mockDDRule21.Object,
                mockDDRule28.Object,
                mockDDRule29.Object,
                mockDateTimeQueryService.Object);

            var result = sut.IsExcluded(mockItem.Object);

            Assert.True(result);
            handler.VerifyAll();
            service.VerifyAll();
            mockDDRule07.VerifyAll();
            mockDDRule21.VerifyAll();
            mockDDRule28.VerifyAll();
            mockDDRule29.VerifyAll();
            mockDateTimeQueryService.VerifyAll();
        }

        [Fact]
        public void IsExcludedForApprenticeship()
        {
            const int progType = 23;
            var mockDel = new Mock<ILearningDelivery>();
            mockDel
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(progType);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDel.Object);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);
            var mockDateTimeQueryService = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            mockDDRule29
                .Setup(x => x.IsInflexibleElementOfTrainingAimLearningDelivery(mockDel.Object))
                .Returns(false);
            mockDDRule07
                .Setup(x => x.IsApprenticeship(progType))
                .Returns(true);

            var sut = new LearnDelFAMType_66Rule(
                handler.Object,
                service.Object,
                mockDDRule07.Object,
                mockDDRule21.Object,
                mockDDRule28.Object,
                mockDDRule29.Object,
                mockDateTimeQueryService.Object);

            var result = sut.IsExcluded(mockDel.Object);

            Assert.True(result);
            handler.VerifyAll();
            service.VerifyAll();
            mockDDRule07.VerifyAll();
            mockDDRule21.VerifyAll();
            mockDDRule28.VerifyAll();
            mockDDRule29.VerifyAll();
            mockDateTimeQueryService.VerifyAll();
        }

        [Fact]
        public void InvalidItemRaisesValidationMessage()
        {
            const string LearnRefNumber = "123456789X";
            const string learnAimRef = "salddfkjeifdnase";
            const int progType = 23;

            var mockFAM = new Mock<ILearningDeliveryFAM>();
            mockFAM
                .SetupGet(x => x.LearnDelFAMType)
                .Returns(Monitoring.Delivery.Types.FullOrCoFunding);
            mockFAM
                .SetupGet(x => x.LearnDelFAMCode)
                .Returns("1");

            var fams = new List<ILearningDeliveryFAM>();
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
               .SetupGet(y => y.ProgTypeNullable)
               .Returns(progType);
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(FundModels.AdultSkills);
            mockDelivery
                .SetupGet(y => y.LearningDeliveryFAMs)
                .Returns(fams);

            var deliveries = new List<ILearningDelivery>();
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
                .Returns(deliveries);

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

            var mockCat = new Mock<ILARSLearningCategory>();
            mockCat
                .SetupGet(x => x.CategoryRef)
                .Returns(LARSCategories.LegalEntitlementLevel2);

            var larsCats = new List<ILARSLearningCategory>();
            larsCats.Add(mockCat.Object);

            var mockLARSDel = new Mock<ILARSLearningDelivery>();
            mockLARSDel
                .SetupGet(x => x.LearnAimRef)
                .Returns(learnAimRef);
            mockLARSDel
                .SetupGet(x => x.NotionalNVQLevelv2)
                .Returns(LARSNotionalNVQLevelV2.Level2);
            mockLARSDel
                .SetupGet(x => x.Categories)
                .Returns(larsCats);

            var mockLARSValidity = new Mock<ILARSLearningDeliveryValidity>();
            mockLARSValidity
                .SetupGet(x => x.LearnAimRef)
                .Returns(learnAimRef);
            mockLARSValidity
                .SetupGet(x => x.ValidityCategory)
                .Returns(LARSValidities.CommunityLearning);
            mockLARSValidity
                .SetupGet(x => x.LastNewStartDate)
                .Returns(new DateTime(2018, 08, 01));

            var larsValidities = new List<ILARSLearningDeliveryValidity>();
            larsValidities.Add(mockLARSValidity.Object);

            var mockLARSAnnualValues = new Mock<ILARSAnnualValue>();
            mockLARSAnnualValues
                .SetupGet(x => x.LearnAimRef)
                .Returns(learnAimRef);
            mockLARSAnnualValues
                .SetupGet(x => x.BasicSkillsType)
                .Returns(190);
            mockLARSAnnualValues
                .SetupGet(x => x.BasicSkills)
                .Returns(1);

            var larsAnnualValues = new List<ILARSAnnualValue>();
            larsAnnualValues.Add(mockLARSAnnualValues.Object);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetDeliveryFor(learnAimRef))
                .Returns(mockLARSDel.Object);
            service
                .Setup(x => x.GetValiditiesFor(learnAimRef))
                .Returns(larsValidities);
            service
                .Setup(x => x.GetAnnualValuesFor(learnAimRef))
                .Returns(larsAnnualValues);
            service
                .Setup(x => x.IsCurrentAndNotWithdrawn(mockLARSValidity.Object, mockDelivery.Object.LearnStartDate, null))
                .Returns(true);

            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            mockDDRule07
                .Setup(x => x.IsApprenticeship(progType))
                .Returns(false);

            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            mockDDRule21
                .Setup(x => x.IsAdultFundedUnemployedWithOtherStateBenefits(mockDelivery.Object, mockLearner.Object))
                .Returns(false);

            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            mockDDRule28
                .Setup(x => x.IsAdultFundedUnemployedWithBenefits(mockDelivery.Object, mockLearner.Object))
                .Returns(false);

            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);
            mockDDRule29
                .Setup(x => x.IsInflexibleElementOfTrainingAimLearningDelivery(mockDelivery.Object))
                .Returns(false);

            var mockDateTimeQueryService = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            mockDateTimeQueryService.Setup(x => x.YearsBetween(birthDate, testDate)).Returns(24);

            var sut = new LearnDelFAMType_66Rule(
                handler.Object,
                service.Object,
                mockDDRule07.Object,
                mockDDRule21.Object,
                mockDDRule28.Object,
                mockDDRule29.Object,
                mockDateTimeQueryService.Object);

            sut.ValidateDeliveries(mockLearner.Object);

            handler.VerifyAll();
            service.VerifyAll();
            mockDDRule07.VerifyAll();
            mockDDRule21.VerifyAll();
            mockDDRule28.VerifyAll();
            mockDDRule29.VerifyAll();
            mockDateTimeQueryService.VerifyAll();
        }

        [Fact]
        public void ValidItemDoesNotRaiseValidationMessage()
        {
            const string LearnRefNumber = "123456789X";
            const string learnAimRef = "salddfkjeifdnase";
            const int progType = 23;
            DateTime dateOfBirth = new DateTime(1993, 07, 01);
            DateTime learnStartDate = new DateTime(2017, 08, 01);

            var mockFAM = new Mock<ILearningDeliveryFAM>();
            mockFAM
                .SetupGet(x => x.LearnDelFAMType)
                .Returns(Monitoring.Delivery.Types.FullOrCoFunding);
            mockFAM
                .SetupGet(x => x.LearnDelFAMCode)
                .Returns("1");

            var fams = new List<ILearningDeliveryFAM>();
            fams.Add(mockFAM.Object);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(learnStartDate);
            mockDelivery
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(progType);
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(FundModels.AdultSkills);
            mockDelivery
                .SetupGet(y => y.LearningDeliveryFAMs)
                .Returns(fams);

            var deliveries = new List<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.DateOfBirthNullable)
                .Returns(dateOfBirth);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var mockCat = new Mock<ILARSLearningCategory>();
            mockCat
                .SetupGet(x => x.CategoryRef)
                .Returns(LARSCategories.LegalEntitlementLevel2);

            var larsCats = new List<ILARSLearningCategory>();
            larsCats.Add(mockCat.Object);

            var mockLARSDel = new Mock<ILARSLearningDelivery>();
            mockLARSDel
                .SetupGet(x => x.LearnAimRef)
                .Returns(learnAimRef);
            mockLARSDel
                .SetupGet(x => x.NotionalNVQLevelv2)
                .Returns(LARSNotionalNVQLevelV2.Level3);
            mockLARSDel
                .SetupGet(x => x.Categories)
                .Returns(larsCats);

            var mockLARSValidity = new Mock<ILARSLearningDeliveryValidity>();
            mockLARSValidity
                .SetupGet(x => x.LearnAimRef)
                .Returns(learnAimRef);
            mockLARSValidity
                .SetupGet(x => x.ValidityCategory)
                .Returns(LARSValidities.CommunityLearning);
            mockLARSValidity
                .SetupGet(x => x.LastNewStartDate)
                .Returns(new DateTime(2018, 08, 01));

            var larsValidities = new List<ILARSLearningDeliveryValidity>();
            larsValidities.Add(mockLARSValidity.Object);

            var mockLARSAnnualValues = new Mock<ILARSAnnualValue>();
            mockLARSAnnualValues
                .SetupGet(x => x.LearnAimRef)
                .Returns(learnAimRef);
            mockLARSAnnualValues
                .SetupGet(x => x.BasicSkillsType)
                .Returns(190);
            mockLARSAnnualValues
                .SetupGet(x => x.BasicSkills)
                .Returns(1);

            var larsAnnualValues = new List<ILARSAnnualValue>();
            larsAnnualValues.Add(mockLARSAnnualValues.Object);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetDeliveryFor(learnAimRef))
                .Returns(mockLARSDel.Object);
            service
                .Setup(x => x.GetValiditiesFor(learnAimRef))
                .Returns(larsValidities);
            service
                .Setup(x => x.GetAnnualValuesFor(learnAimRef))
                .Returns(larsAnnualValues);
            service
                .Setup(x => x.IsCurrentAndNotWithdrawn(mockLARSValidity.Object, mockDelivery.Object.LearnStartDate, null))
                .Returns(true);
            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            mockDDRule07
                .Setup(x => x.IsApprenticeship(progType))
                .Returns(false);

            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            mockDDRule21
                .Setup(x => x.IsAdultFundedUnemployedWithOtherStateBenefits(mockDelivery.Object, mockLearner.Object))
                .Returns(false);

            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            mockDDRule28
                .Setup(x => x.IsAdultFundedUnemployedWithBenefits(mockDelivery.Object, mockLearner.Object))
                .Returns(false);

            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);
            mockDDRule29
                .Setup(x => x.IsInflexibleElementOfTrainingAimLearningDelivery(mockDelivery.Object))
                .Returns(false);

            var mockDateTimeQueryService = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            mockDateTimeQueryService.Setup(x => x.YearsBetween(dateOfBirth, learnStartDate)).Returns(24);

            var sut = new LearnDelFAMType_66Rule(
                handler.Object,
                service.Object,
                mockDDRule07.Object,
                mockDDRule21.Object,
                mockDDRule28.Object,
                mockDDRule29.Object,
                mockDateTimeQueryService.Object);

            sut.ValidateDeliveries(mockLearner.Object);

            handler.VerifyAll();
            service.VerifyAll();
            mockDDRule07.VerifyAll();
            mockDDRule21.VerifyAll();
            mockDDRule28.VerifyAll();
            mockDDRule29.VerifyAll();
            mockDateTimeQueryService.VerifyAll();
        }

        public LearnDelFAMType_66Rule NewRule(
            IValidationErrorHandler handler = null,
            ILARSDataService lars = null,
            IDerivedData_07Rule mockDDRule07 = null,
            IDerivedData_21Rule mockDDRule21 = null,
            IDerivedData_28Rule mockDDRule28 = null,
            IDerivedData_29Rule mockDDRule29 = null,
            IDateTimeQueryService mockDateTimeQueryService = null)
        {
            return new LearnDelFAMType_66Rule(
                handler,
                lars,
                mockDDRule07,
                mockDDRule21,
                mockDDRule28,
                mockDDRule29,
                mockDateTimeQueryService);
        }
    }
}
