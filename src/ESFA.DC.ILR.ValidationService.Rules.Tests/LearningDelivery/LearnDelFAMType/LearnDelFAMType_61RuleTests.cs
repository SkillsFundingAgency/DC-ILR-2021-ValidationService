using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_61RuleTests
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMType_61");
        }

        [Fact]
        public void LastInviableDateMeetsExpectation()
        {
            // arrange / act / assert
            Assert.Equal(DateTime.Parse("2017-07-31"), LearnDelFAMType_61Rule.LastInviableDate);
        }

        [Theory]
        [InlineData(Monitoring.Delivery.OLASSOffendersInCustody, true)]
        [InlineData(Monitoring.Delivery.FullyFundedLearningAim, false)]
        [InlineData(Monitoring.Delivery.CoFundedLearningAim, false)]
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

        [Theory]
        [InlineData(Monitoring.Delivery.ReleasedOnTemporaryLicence, true)]
        [InlineData(Monitoring.Delivery.MandationToSkillsTraining, false)]
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

        [Theory]
        [InlineData(Monitoring.Delivery.Types.Restart, true)]
        [InlineData(Monitoring.Delivery.Types.AdvancedLearnerLoansBursaryFunding, false)]
        [InlineData(Monitoring.Delivery.Types.AdvancedLearnerLoan, false)]
        [InlineData(Monitoring.Delivery.Types.ApprenticeshipContract, false)]
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

        [Theory]
        [InlineData(Monitoring.Delivery.SteelIndustriesRedundancyTraining, true)]
        [InlineData(Monitoring.Delivery.InReceiptOfLowWages, true)]
        [InlineData(Monitoring.Delivery.OLASSOffendersInCustody, false)]
        [InlineData(Monitoring.Delivery.CoFundedLearningAim, false)]
        [InlineData(Monitoring.Delivery.MandationToSkillsTraining, false)]
        [InlineData(Monitoring.Delivery.ReleasedOnTemporaryLicence, false)]
        public void IsSteelWorkerRedundancyTrainingOrIsInReceiptOfLowWagesMeetsExpectation(string candidate, bool expectation)
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
            var result = sut.IsSteelWorkerRedundancyTrainingOrIsInReceiptOfLowWages(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

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
            var mockDateTimeQueryService = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            mockDDRule28
                .Setup(x => x.IsAdultFundedUnemployedWithBenefits(delivery.Object, learner.Object))
                .Returns(expectation);

            var sut = new LearnDelFAMType_61Rule(
                handler.Object,
                service.Object,
                mockDDRule07.Object,
                mockDDRule21.Object,
                mockDDRule28.Object,
                mockDDRule29.Object,
                mockDateTimeQueryService.Object);

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
            mockDateTimeQueryService.VerifyAll();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void IsInflexibleElementOfTrainingAimMeetsExpectation(bool expectation)
        {
            // arrange
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
                .Returns(expectation);

            var sut = new LearnDelFAMType_61Rule(
                handler.Object,
                service.Object,
                mockDDRule07.Object,
                mockDDRule21.Object,
                mockDDRule28.Object,
                mockDDRule29.Object,
                mockDateTimeQueryService.Object);

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
            mockDateTimeQueryService.VerifyAll();
        }

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
            var mockDateTimeQueryService = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            mockDDRule21
                .Setup(x => x.IsAdultFundedUnemployedWithOtherStateBenefits(delivery.Object, mockItem.Object))
                .Returns(expectation);

            var sut = new LearnDelFAMType_61Rule(
                handler.Object,
                service.Object,
                mockDDRule07.Object,
                mockDDRule21.Object,
                mockDDRule28.Object,
                mockDDRule29.Object,
                mockDateTimeQueryService.Object);

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
            mockDateTimeQueryService.VerifyAll();
        }

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
            var mockDateTimeQueryService = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            mockDDRule07
                .Setup(x => x.IsApprenticeship(null))
                .Returns(expectation);

            var sut = new LearnDelFAMType_61Rule(
                handler.Object,
                service.Object,
                mockDDRule07.Object,
                mockDDRule21.Object,
                mockDDRule28.Object,
                mockDDRule29.Object,
                mockDateTimeQueryService.Object);

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
            mockDateTimeQueryService.VerifyAll();
        }

        [Theory]
        [InlineData(FundModels.AdultSkills, true)]
        [InlineData(FundModels.Age16To19ExcludingApprenticeships, false)]
        [InlineData(FundModels.ApprenticeshipsFrom1May2017, false)]
        [InlineData(FundModels.CommunityLearning, false)]
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

        [Theory]
        [InlineData("1997-08-23", "2018-04-18", 20, true)] // in age group
        [InlineData("1998-05-11", "2018-04-18", 19, true)] // in age group
        [InlineData("1999-04-18", "2018-04-18", 19, true)] // in age group, lower boundary
        [InlineData("1999-04-19", "2018-04-18", 18, false)] // too young
        [InlineData("1995-04-18", "2018-04-18", 23, true)] // in age group, upper boundary
        [InlineData("1995-04-17", "2019-04-18", 24, false)] // too old
        [InlineData("1995-08-25", "2018-10-03", 23, true)] // testers scenario
        public void IsTargetAgeGroupMeetsExpectation(string birthDate, string startDate, int ageinYears, bool expectation)
        {
            // arrange
            DateTime dateOfBirth = DateTime.Parse(birthDate);
            DateTime learnStartDate = DateTime.Parse(startDate);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(y => y.DateOfBirthNullable)
                .Returns(DateTime.Parse(birthDate));

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(startDate));

            var mockLARS = new Mock<ILARSDataService>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            var mockDateTimeQueryService = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            mockDateTimeQueryService
                .Setup(x => x.YearsBetween(dateOfBirth, learnStartDate))
                .Returns(ageinYears);

            var sut = new LearnDelFAMType_61Rule(
                validationErrorHandlerMock.Object,
                mockLARS.Object,
                mockDDRule07.Object,
                mockDDRule21.Object,
                mockDDRule28.Object,
                mockDDRule29.Object,
                mockDateTimeQueryService.Object);

            // act
            var result = sut.IsTargetAgeGroup(mockLearner.Object, mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(Monitoring.Delivery.OLASSOffendersInCustody, false)]
        [InlineData(Monitoring.Delivery.FullyFundedLearningAim, true)]
        [InlineData(Monitoring.Delivery.InReceiptOfLowWages, false)]
        [InlineData(Monitoring.Delivery.MandationToSkillsTraining, false)]
        public void IsFullyFundedLearningAimMeetsExpectation(string candidate, bool expectation)
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
            var result = sut.IsFullyFundedLearningAim(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(LARSConstants.NotionalNVQLevelV2Strings.Level2, true)]
        [InlineData(LARSConstants.NotionalNVQLevelV2Strings.EntryLevel, false)]
        [InlineData(LARSConstants.NotionalNVQLevelV2Strings.HigherLevel, false)]
        [InlineData(LARSConstants.NotionalNVQLevelV2Strings.Level1, false)]
        public void IsV2NotionalLevel2MeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockItem = new Mock<ILARSLearningDelivery>();
            mockItem
                .SetupGet(y => y.NotionalNVQLevelv2)
                .Returns(candidate);

            // act
            var result = sut.IsV2NotionalLevel2(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(LARSConstants.Categories.LegalEntitlementLevel2, false)]
        [InlineData(LARSConstants.Categories.WorkPlacementSFAFunded, true)]
        [InlineData(LARSConstants.Categories.WorkPreparationSFATraineeships, true)]
        [InlineData(39, true)]
        [InlineData(23, true)]
        public void IsNotEntitledMeetsExpectation(int candidate, bool expectation)
        {
            // arrange
            const string learnAimRef = "asdfbasdf";
            var mockCat = new Mock<ILARSLearningCategory>();
            mockCat
                .SetupGet(x => x.CategoryRef)
                .Returns(candidate);

            var larsCats = new List<ILARSLearningCategory>
            {
                mockCat.Object
            };

            var mock = new Mock<ILARSLearningDelivery>();
            mock
                .SetupGet(x => x.NotionalNVQLevelv2)
                .Returns(LARSConstants.NotionalNVQLevelV2Strings.Level2);
            mock
                .SetupGet(x => x.Categories)
                .Returns(larsCats);

            var service = new Mock<ILARSDataService>();
            service
                .Setup(x => x.GetDeliveryFor(learnAimRef))
                .Returns(mock.Object);

            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            var mockDateTimeQueryService = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            var sut = new LearnDelFAMType_61Rule(
                validationErrorHandlerMock.Object,
                service.Object,
                mockDDRule07.Object,
                mockDDRule21.Object,
                mockDDRule28.Object,
                mockDDRule29.Object,
                mockDateTimeQueryService.Object);

            // act
            var result = sut.IsNotEntitled(mock.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(LARSConstants.BasicSkills.CertificateESOLS4L, false)]
        [InlineData(LARSConstants.BasicSkills.CertificateESOLS4LSpeakListen, false)]
        [InlineData(LARSConstants.BasicSkills.Certificate_AdultLiteracy, true)]
        [InlineData(LARSConstants.BasicSkills.Certificate_AdultNumeracy, true)]
        [InlineData(LARSConstants.BasicSkills.FreeStandingMathematicsQualification, true)]
        [InlineData(LARSConstants.BasicSkills.FunctionalSkillsEnglish, true)]
        [InlineData(LARSConstants.BasicSkills.FunctionalSkillsMathematics, true)]
        [InlineData(LARSConstants.BasicSkills.GCSE_EnglishLanguage, true)]
        [InlineData(LARSConstants.BasicSkills.GCSE_Mathematics, true)]
        [InlineData(LARSConstants.BasicSkills.InternationalGCSEEnglishLanguage, true)]
        [InlineData(LARSConstants.BasicSkills.InternationalGCSEMathematics, true)]
        [InlineData(LARSConstants.BasicSkills.KeySkill_ApplicationOfNumbers, true)]
        [InlineData(LARSConstants.BasicSkills.KeySkill_Communication, true)]
        [InlineData(LARSConstants.BasicSkills.NonNQF_QCFS4LESOL, false)]
        [InlineData(LARSConstants.BasicSkills.NonNQF_QCFS4LLiteracy, true)]
        [InlineData(LARSConstants.BasicSkills.NonNQF_QCFS4LNumeracy, true)]
        [InlineData(LARSConstants.BasicSkills.NotApplicable, false)]
        [InlineData(LARSConstants.BasicSkills.OtherS4LNotLiteracyNumeracyOrESOL, false)]
        [InlineData(LARSConstants.BasicSkills.QCFBasicSkillsEnglishLanguage, true)]
        [InlineData(LARSConstants.BasicSkills.QCFBasicSkillsMathematics, true)]
        [InlineData(LARSConstants.BasicSkills.QCFCertificateESOL, false)]
        [InlineData(LARSConstants.BasicSkills.QCFESOLReading, false)]
        [InlineData(LARSConstants.BasicSkills.QCFESOLSpeakListen, false)]
        [InlineData(LARSConstants.BasicSkills.QCFESOLWriting, false)]
        [InlineData(LARSConstants.BasicSkills.UnitESOLReading, false)]
        [InlineData(LARSConstants.BasicSkills.UnitESOLSpeakListen, false)]
        [InlineData(LARSConstants.BasicSkills.UnitESOLWriting, false)]
        [InlineData(LARSConstants.BasicSkills.UnitQCFBasicSkillsEnglishLanguage, true)]
        [InlineData(LARSConstants.BasicSkills.UnitQCFBasicSkillsMathematics, true)]
        [InlineData(LARSConstants.BasicSkills.UnitsOfTheCertificate_AdultLiteracy, true)]
        [InlineData(LARSConstants.BasicSkills.UnitsOfTheCertificate_AdultNumeracy, true)]
        [InlineData(LARSConstants.BasicSkills.UnitsOfTheCertificate_ESOLS4L, false)]
        [InlineData(LARSConstants.BasicSkills.Unknown, false)]
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

        [Fact]
        public void IsExcludedForAdultFundedUnemployedWithBenefits()
        {
            var learnAimRef = "LearnAimRef";

            var mockItem = new Mock<ILearningDelivery>();
            mockItem
              .SetupGet(x => x.LearnAimRef)
              .Returns(learnAimRef);

            var mockLARSDel = new Mock<ILARSLearningDelivery>();
            mockLARSDel
                .SetupGet(x => x.LearnAimRef)
                .Returns(learnAimRef);
            mockLARSDel
                .SetupGet(x => x.NotionalNVQLevelv2)
                .Returns(LARSConstants.NotionalNVQLevelV2Strings.Level3);
            mockLARSDel
                .SetupGet(x => x.EffectiveFrom)
                .Returns(new DateTime(2018, 08, 01));
            mockLARSDel
                .SetupGet(x => x.EffectiveTo)
                .Returns(new DateTime(2022, 08, 01));

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

            var sut = new LearnDelFAMType_61Rule(
                handler.Object,
                service.Object,
                mockDDRule07.Object,
                mockDDRule21.Object,
                mockDDRule28.Object,
                mockDDRule29.Object,
                mockDateTimeQueryService.Object);

            // act
            var result = sut.IsExcluded(mockItem.Object, mockLARSDel.Object);

            // assert
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
        public void IsExcludedForInflexibleElementOfTrainingAim()
        {
            var learnAimRef = "LearnAimRef";

            var mockItem = new Mock<ILearningDelivery>();
            mockItem
               .SetupGet(x => x.LearnAimRef)
               .Returns(learnAimRef);

            var mockLARSDel = new Mock<ILARSLearningDelivery>();
            mockLARSDel
                .SetupGet(x => x.LearnAimRef)
                .Returns(learnAimRef);
            mockLARSDel
                .SetupGet(x => x.NotionalNVQLevelv2)
                .Returns(LARSConstants.NotionalNVQLevelV2Strings.Level3);
            mockLARSDel
                .SetupGet(x => x.EffectiveFrom)
                .Returns(new DateTime(2018, 08, 01));
            mockLARSDel
                .SetupGet(x => x.EffectiveTo)
                .Returns(new DateTime(2022, 08, 01));

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

            var sut = new LearnDelFAMType_61Rule(
                handler.Object,
                service.Object,
                mockDDRule07.Object,
                mockDDRule21.Object,
                mockDDRule28.Object,
                mockDDRule29.Object,
                mockDateTimeQueryService.Object);

            // act
            var result = sut.IsExcluded(mockItem.Object, mockLARSDel.Object);

            // assert
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
            var learnAimRef = "LearnAimRef";
            const int progType = 23;

            var mockItem = new Mock<ILearningDelivery>();
            mockItem
               .SetupGet(x => x.LearnAimRef)
               .Returns(learnAimRef);
            mockItem
                .SetupGet(x => x.ProgTypeNullable)
                .Returns(progType);

            var mockLARSDel = new Mock<ILARSLearningDelivery>();
            mockLARSDel
                .SetupGet(x => x.LearnAimRef)
                .Returns(learnAimRef);
            mockLARSDel
                .SetupGet(x => x.NotionalNVQLevelv2)
                .Returns(LARSConstants.NotionalNVQLevelV2Strings.Level3);
            mockLARSDel
                .SetupGet(x => x.EffectiveFrom)
                .Returns(new DateTime(2018, 08, 01));
            mockLARSDel
                .SetupGet(x => x.EffectiveTo)
                .Returns(new DateTime(2022, 08, 01));

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);
            var mockDateTimeQueryService = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            mockDDRule29
                .Setup(x => x.IsInflexibleElementOfTrainingAimLearningDelivery(mockItem.Object))
                .Returns(false);
            mockDDRule07
                .Setup(x => x.IsApprenticeship(progType))
                .Returns(true);

            var sut = new LearnDelFAMType_61Rule(
                handler.Object,
                service.Object,
                mockDDRule07.Object,
                mockDDRule21.Object,
                mockDDRule28.Object,
                mockDDRule29.Object,
                mockDateTimeQueryService.Object);

            // act
            var result = sut.IsExcluded(mockItem.Object, mockLARSDel.Object);

            // assert
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
            // arrange
            IFormatProvider requiredCulture = new CultureInfo("en-GB");
            const string LearnRefNumber = "123456789X";
            const string learnAimRef = "salddfkjeifdnase";
            const int progType = 23;
            DateTime learnStartDate = new DateTime(2017, 8, 1);
            DateTime? dateOfBirth = new DateTime(1996, 7, 1);

            var mockFAM = new Mock<ILearningDeliveryFAM>();
            mockFAM
                .SetupGet(x => x.LearnDelFAMType)
                .Returns(Monitoring.Delivery.Types.FullOrCoFunding);
            mockFAM
                .SetupGet(x => x.LearnDelFAMCode)
                .Returns("1");

            var fams = new List<ILearningDeliveryFAM>
            {
                mockFAM.Object
            };

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);
            mockDelivery
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(progType);
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(learnStartDate);
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(FundModels.AdultSkills);
            mockDelivery
                .SetupGet(y => y.LearningDeliveryFAMs)
                .Returns(fams);

            var deliveries = new List<ILearningDelivery>
            {
                mockDelivery.Object
            };

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.PriorAttainNullable)
                .Returns(PriorAttainments.FullLevel2);
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.DateOfBirthNullable)
                .Returns(dateOfBirth);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            validationErrorHandlerMock
                .Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, Monitoring.Delivery.Types.FullOrCoFunding)).Verifiable();
            validationErrorHandlerMock
                .Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, "1")).Verifiable();
            validationErrorHandlerMock
                .Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.FundModel, FundModels.AdultSkills)).Verifiable();
            validationErrorHandlerMock
                .Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate.ToString("d", requiredCulture)))
                .Verifiable();
            validationErrorHandlerMock
                .Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.DateOfBirth, dateOfBirth.Value.ToString("d", requiredCulture)))
                .Verifiable();

            var mockCat = new Mock<ILARSLearningCategory>();
            mockCat
               .SetupGet(x => x.LearnAimRef)
               .Returns(learnAimRef);
            mockCat
                .SetupGet(x => x.CategoryRef)
                .Returns(LARSConstants.Categories.LicenseToPractice);

            var larsCats = new List<ILARSLearningCategory>
            {
                mockCat.Object
            };

            var mockLARSDel = new Mock<ILARSLearningDelivery>();
            mockLARSDel
                .SetupGet(x => x.LearnAimRef)
                .Returns(learnAimRef);
            mockLARSDel
                .SetupGet(x => x.NotionalNVQLevelv2)
                .Returns(LARSConstants.NotionalNVQLevelV2Strings.Level2);
            mockLARSDel
                .SetupGet(x => x.Categories)
                .Returns(larsCats);

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

            var larsAnnualValues = new List<ILARSAnnualValue>
            {
                mockLARSAnnualValues.Object
            };

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetDeliveryFor(learnAimRef))
                .Returns(mockLARSDel.Object);
            service
                .Setup(x => x.GetAnnualValuesFor(learnAimRef))
                .Returns(larsAnnualValues);

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
            mockDateTimeQueryService.Setup(x => x.YearsBetween(dateOfBirth.Value, learnStartDate)).Returns(21);

            var sut = new LearnDelFAMType_61Rule(
                validationErrorHandlerMock.Object,
                service.Object,
                mockDDRule07.Object,
                mockDDRule21.Object,
                mockDDRule28.Object,
                mockDDRule29.Object,
                mockDateTimeQueryService.Object);

            // act
            sut.ValidateDeliveries(mockLearner.Object);

            // assert
            validationErrorHandlerMock.VerifyAll();
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
            // arrange
            const string LearnRefNumber = "123456789X";
            const string learnAimRef = "salddfkjeifdnase";
            const int progType = 23;
            DateTime learnStartDate = new DateTime(2017, 08, 01);
            DateTime dateOfBirth = new DateTime(1996, 07, 01);

            var mockFAM = new Mock<ILearningDeliveryFAM>();
            mockFAM
                .SetupGet(x => x.LearnDelFAMType)
                .Returns(Monitoring.Delivery.Types.FullOrCoFunding);
            mockFAM
                .SetupGet(x => x.LearnDelFAMCode)
                .Returns("1");

            var fams = new List<ILearningDeliveryFAM>
            {
                mockFAM.Object
            };

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);
            mockDelivery
                .SetupGet(y => y.ProgTypeNullable)
                .Returns(progType);
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(learnStartDate);
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(FundModels.AdultSkills);
            mockDelivery
                .SetupGet(y => y.LearningDeliveryFAMs)
                .Returns(fams);

            var deliveries = new List<ILearningDelivery>
            {
                mockDelivery.Object
            };

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.PriorAttainNullable)
                .Returns(PriorAttainments.FullLevel3);
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
                .Returns(LARSConstants.Categories.LegalEntitlementLevel2);

            var larsCats = new List<ILARSLearningCategory>
            {
                mockCat.Object
            };

            var mockLARSDel = new Mock<ILARSLearningDelivery>();
            mockLARSDel
                .SetupGet(x => x.LearnAimRef)
                .Returns(learnAimRef);
            mockLARSDel
                .SetupGet(x => x.NotionalNVQLevelv2)
                .Returns(LARSConstants.NotionalNVQLevelV2Strings.Level3);
            mockLARSDel
                .SetupGet(x => x.Categories)
                .Returns(larsCats);

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

            var larsAnnualValues = new List<ILARSAnnualValue>
            {
                mockLARSAnnualValues.Object
            };

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetDeliveryFor(learnAimRef))
                .Returns(mockLARSDel.Object);
            service
                .Setup(x => x.GetAnnualValuesFor(learnAimRef))
                .Returns(larsAnnualValues);

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
            mockDateTimeQueryService.Setup(x => x.YearsBetween(dateOfBirth, learnStartDate)).Returns(21);

            var sut = new LearnDelFAMType_61Rule(
                handler.Object,
                service.Object,
                mockDDRule07.Object,
                mockDDRule21.Object,
                mockDDRule28.Object,
                mockDDRule29.Object,
                mockDateTimeQueryService.Object);

            // act
            sut.ValidateDeliveries(mockLearner.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
            mockDDRule07.VerifyAll();
            mockDDRule21.VerifyAll();
            mockDDRule28.VerifyAll();
            mockDDRule29.VerifyAll();
            mockDateTimeQueryService.VerifyAll();
        }

        public LearnDelFAMType_61Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);
            var mockDateTimeQueryService = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            return new LearnDelFAMType_61Rule(
                handler.Object,
                service.Object,
                mockDDRule07.Object,
                mockDDRule21.Object,
                mockDDRule28.Object,
                mockDDRule29.Object,
                mockDateTimeQueryService.Object);
        }
    }
}
