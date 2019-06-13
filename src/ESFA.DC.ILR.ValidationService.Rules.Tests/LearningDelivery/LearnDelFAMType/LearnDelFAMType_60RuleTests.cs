using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using System.Globalization;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMType
{
    using ESFA.DC.ILR.Model.Interface;
    using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
    using ESFA.DC.ILR.ValidationService.Interface;
    using ESFA.DC.ILR.ValidationService.Rules.Constants;
    using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
    using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
    using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
    using ESFA.DC.ILR.ValidationService.Utility;
    using FluentAssertions;
    using Moq;
    using System;
    using Xunit;

    public class LearnDelFAMType_60RuleTests
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMType_60");
        }

        [Fact]
        public void LastInviableStartDateMeetsExpectation()
        {
            // arrange / act / assert
            Assert.Equal(DateTime.Parse("2016-08-01"), LearnDelFAMType_60Rule.FirstViableDate);
        }

        [Fact]
        public void LastInviableEndDateMeetsExpectation()
        {
            // arrange  / act / assert
            Assert.Equal(DateTime.Parse("2017-07-31"), LearnDelFAMType_60Rule.LastViableDate);
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
            var organisationDataService = new Mock<IOrganisationDataService>(MockBehavior.Strict);
            var fileDataService = new Mock<IFileDataService>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);
            var dateTimeQueryService = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            mockDDRule07
                .Setup(x => x.IsApprenticeship(null))
                .Returns(expectation);

            var sut = new LearnDelFAMType_60Rule(
                handler.Object,
                service.Object,
                mockDDRule07.Object,
                mockDDRule21.Object,
                mockDDRule28.Object,
                mockDDRule29.Object,
                organisationDataService.Object,
                fileDataService.Object,
                dateTimeQueryService.Object);

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
            organisationDataService.VerifyAll();
            fileDataService.VerifyAll();
            dateTimeQueryService.VerifyAll();
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
            var organisationDataService = new Mock<IOrganisationDataService>(MockBehavior.Strict);
            var fileDataService = new Mock<IFileDataService>(MockBehavior.Strict);
            var dateTimeQueryService = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            mockDDRule29
                .Setup(x => x.IsInflexibleElementOfTrainingAimLearningDelivery(mockItem.Object))
                .Returns(expectation);

            var sut = new LearnDelFAMType_60Rule(
                handler.Object,
                service.Object,
                mockDDRule07.Object,
                mockDDRule21.Object,
                mockDDRule28.Object,
                mockDDRule29.Object,
                organisationDataService.Object,
                fileDataService.Object,
                dateTimeQueryService.Object);

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
            dateTimeQueryService.VerifyAll();
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
            var organisationDataService = new Mock<IOrganisationDataService>(MockBehavior.Strict);
            var fileDataService = new Mock<IFileDataService>(MockBehavior.Strict);
            var dateTimeQueryService = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            mockDDRule21
                .Setup(x => x.IsAdultFundedUnemployedWithOtherStateBenefits(delivery.Object, mockItem.Object))
                .Returns(expectation);

            var sut = new LearnDelFAMType_60Rule(
                handler.Object,
                service.Object,
                mockDDRule07.Object,
                mockDDRule21.Object,
                mockDDRule28.Object,
                mockDDRule29.Object,
                organisationDataService.Object,
                fileDataService.Object,
                dateTimeQueryService.Object);

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
            dateTimeQueryService.VerifyAll();
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
            var organisationDataService = new Mock<IOrganisationDataService>(MockBehavior.Strict);
            var fileDataService = new Mock<IFileDataService>(MockBehavior.Strict);
            var dateTimeQueryService = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            mockDDRule28
                .Setup(x => x.IsAdultFundedUnemployedWithBenefits(delivery.Object, learner.Object))
                .Returns(expectation);

            var sut = new LearnDelFAMType_60Rule(
                handler.Object,
                service.Object,
                mockDDRule07.Object,
                mockDDRule21.Object,
                mockDDRule28.Object,
                mockDDRule29.Object,
                organisationDataService.Object,
                fileDataService.Object,
                dateTimeQueryService.Object);

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
            dateTimeQueryService.VerifyAll();
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

        [Theory]
        [InlineData(Monitoring.Delivery.SteelIndustriesRedundancyTraining, true)]
        [InlineData(Monitoring.Delivery.InReceiptOfLowWages, false)]
        [InlineData(Monitoring.Delivery.OLASSOffendersInCustody, false)]
        [InlineData(Monitoring.Delivery.CoFundedLearningAim, false)]
        [InlineData(Monitoring.Delivery.MandationToSkillsTraining, false)]
        [InlineData(Monitoring.Delivery.ReleasedOnTemporaryLicence, false)]
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

        [Theory]
        [InlineData("USDC", true)]
        [InlineData("abcd", false)]
        public void IsLegalOrgTypeMatchForUkprnMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);
            var organisationDataService = new Mock<IOrganisationDataService>();
            var fileDataService = new Mock<IFileDataService>(MockBehavior.Strict);
            var dateTimeQueryService = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            fileDataService.Setup(x => x.UKPRN()).Returns(123456);

            organisationDataService
                .Setup(x => x.LegalOrgTypeMatchForUkprn(Moq.It.IsAny<long>(), candidate))
                .Returns(expectation);

            var sut = new LearnDelFAMType_60Rule(
                handler.Object,
                service.Object,
                mockDDRule07.Object,
                mockDDRule21.Object,
                mockDDRule28.Object,
                mockDDRule29.Object,
                organisationDataService.Object,
                fileDataService.Object,
                dateTimeQueryService.Object);

            // act
            var result = sut.IsLegalOrgTypeMatchForUkprn();

            // assert
            Assert.Equal(expectation, result);
            handler.VerifyAll();
            service.VerifyAll();
            mockDDRule07.VerifyAll();
            mockDDRule21.VerifyAll();
            mockDDRule28.VerifyAll();
            mockDDRule29.VerifyAll();
            dateTimeQueryService.VerifyAll();
        }

        [Theory]
        [InlineData("2016-08-01", true)]
        [InlineData("2017-07-31", true)]
        [InlineData("2017-08-01", false)]
        [InlineData("2017-09-14", false)]
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
        [InlineData(TypeOfFunding.AdultSkills, true)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, false)]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, false)]
        [InlineData(TypeOfFunding.CommunityLearning, false)]
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
        [InlineData("1994-08-01", "2018-07-31", 23, false)]
        [InlineData("1994-08-01", "2018-08-01", 24, true)]
        [InlineData("1995-08-25", "2018-10-03", 23, false)]
        [InlineData("1994-08-25", "2018-10-03", 24, true)]
        public void IsTargetAgeGroupMeetsExpectation(
            string birthDateString,
            string startDateString,
            int yearsOfAge,
            bool expectation)
        {
            // arrange
            DateTime birthDate = DateTime.Parse(birthDateString);
            DateTime learnStartDate = DateTime.Parse(startDateString);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(y => y.DateOfBirthNullable)
                .Returns(birthDate);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(learnStartDate);

            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var organisationDataService = new Mock<IOrganisationDataService>(MockBehavior.Strict);
            var fileDataService = new Mock<IFileDataService>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);
            var dateTimeQueryService = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            dateTimeQueryService
                .Setup(x => x.YearsBetween(birthDate, learnStartDate))
                .Returns(yearsOfAge);

            var sut = new LearnDelFAMType_60Rule(
                handler.Object,
                service.Object,
                mockDDRule07.Object,
                mockDDRule21.Object,
                mockDDRule28.Object,
                mockDDRule29.Object,
                organisationDataService.Object,
                fileDataService.Object,
                dateTimeQueryService.Object);

            // act
            var result = sut.IsTargetAgeGroup(mockLearner.Object, mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
            handler.VerifyAll();
            service.VerifyAll();
            mockDDRule07.VerifyAll();
            mockDDRule21.VerifyAll();
            mockDDRule28.VerifyAll();
            mockDDRule29.VerifyAll();
            organisationDataService.VerifyAll();
            fileDataService.VerifyAll();
            dateTimeQueryService.VerifyAll();
        }

        [Theory]
        [InlineData(LARSNotionalNVQLevelV2.Level2, true)]
        [InlineData(LARSNotionalNVQLevelV2.EntryLevel, true)]
        [InlineData(LARSNotionalNVQLevelV2.Level1, true)]
        [InlineData(LARSNotionalNVQLevelV2.HigherLevel, false)]
        public void IsEarlyStageNVQMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var larsDataService = new Mock<ILARSDataService>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);
            var organisationDataService = new Mock<IOrganisationDataService>();
            var fileDataService = new Mock<IFileDataService>(MockBehavior.Strict);
            var dateTimeQueryService = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            var mockItem = new Mock<ILARSLearningDelivery>();

            mockItem.SetupGet(y => y.NotionalNVQLevelv2).Returns(candidate);
            larsDataService
                .Setup(x => x.GetDeliveryFor(Moq.It.IsAny<string>()))
                .Returns(mockItem.Object);

            var sut = new LearnDelFAMType_60Rule(
                handler.Object,
                larsDataService.Object,
                mockDDRule07.Object,
                mockDDRule21.Object,
                mockDDRule28.Object,
                mockDDRule29.Object,
                organisationDataService.Object,
                fileDataService.Object,
                dateTimeQueryService.Object);

            // act
            var result = sut.IsEarlyStageNVQ(new TestLearningDelivery());

            // assert
            Assert.Equal(expectation, result);
            handler.VerifyAll();
            larsDataService.VerifyAll();
            mockDDRule07.VerifyAll();
            mockDDRule21.VerifyAll();
            mockDDRule28.VerifyAll();
            mockDDRule29.VerifyAll();
            dateTimeQueryService.VerifyAll();
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

        [Fact]
        public void IsExcludedForInflexibleElementOfTrainingAim()
        {
            // arrange
            var mockItem = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDerivedData_07Rule>();
            var mockDDRule21 = new Mock<IDerivedData_21Rule>();
            var mockDDRule28 = new Mock<IDerivedData_28Rule>();
            var mockDDRule29 = new Mock<IDerivedData_29Rule>();
            var organisationDataService = new Mock<IOrganisationDataService>();
            var fileDataService = new Mock<IFileDataService>(MockBehavior.Strict);
            var dateTimeQueryService = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            mockDDRule29
                .Setup(x => x.IsInflexibleElementOfTrainingAimLearningDelivery(mockItem.Object))
                .Returns(true);

            var sut = new LearnDelFAMType_60Rule(
                handler.Object,
                service.Object,
                mockDDRule07.Object,
                mockDDRule21.Object,
                mockDDRule28.Object,
                mockDDRule29.Object,
                organisationDataService.Object,
                fileDataService.Object,
                dateTimeQueryService.Object);

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
            dateTimeQueryService.VerifyAll();
        }

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

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDerivedData_07Rule>();
            var mockDDRule21 = new Mock<IDerivedData_21Rule>();
            var mockDDRule28 = new Mock<IDerivedData_28Rule>();
            var mockDDRule29 = new Mock<IDerivedData_29Rule>();
            var organisationDataService = new Mock<IOrganisationDataService>();
            var fileDataService = new Mock<IFileDataService>(MockBehavior.Strict);
            var dateTimeQueryService = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            mockDDRule07
                .Setup(x => x.IsApprenticeship(progType))
                .Returns(true);

            var sut = new LearnDelFAMType_60Rule(
                handler.Object,
                service.Object,
                mockDDRule07.Object,
                mockDDRule21.Object,
                mockDDRule28.Object,
                mockDDRule29.Object,
                organisationDataService.Object,
                fileDataService.Object,
                dateTimeQueryService.Object);

            // act
            var result = sut.IsExcluded(mockDel.Object);

            // assert
            Assert.True(result);
            handler.VerifyAll();
            service.VerifyAll();
            mockDDRule07.VerifyAll();
            mockDDRule21.VerifyAll();
            mockDDRule28.VerifyAll();
            mockDDRule29.VerifyAll();
            dateTimeQueryService.VerifyAll();
        }

        [Fact]
        public void InvalidItemRaisesValidationMessage()
        {
            // arrange
            IFormatProvider requiredCulture = new CultureInfo("en-GB");
            const string LearnRefNumber = "123456789X";
            const string learnAimRef = "salddfkjeifdnase";
            const int progType = 23;
            const int ukprn = 1000268;
            DateTime learnStartDate = new DateTime(2016, 8, 1);
            DateTime? dateOfBirth = new DateTime(1990, 7, 1);

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
               .SetupGet(y => y.ProgTypeNullable)
               .Returns(progType);
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(learnStartDate);
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
                .Returns(dateOfBirth);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries.AsSafeReadOnlyList());

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            validationErrorHandlerMock
                .Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, Monitoring.Delivery.Types.FullOrCoFunding))
                .Verifiable();
            validationErrorHandlerMock
                .Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, "1"))
                .Verifiable();
            validationErrorHandlerMock
                .Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.FundModel, TypeOfFunding.AdultSkills))
                .Verifiable();
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
                .Returns(TypeOfLARSCategory.LicenseToPractice);

            var larsCats = Collection.Empty<ILARSLearningCategory>();
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
                .Returns(larsCats.AsSafeReadOnlyList());

            var mockLARSValidity = new Mock<ILARSLearningDeliveryValidity>();
            mockLARSValidity
                .SetupGet(x => x.LearnAimRef)
                .Returns(learnAimRef);
            mockLARSValidity
                .SetupGet(x => x.ValidityCategory)
                .Returns(TypeOfLARSValidity.CommunityLearning);
            mockLARSValidity
                .SetupGet(x => x.LastNewStartDate)
                .Returns(new DateTime(2018, 08, 01));

            var larsValidities = Collection.Empty<ILARSLearningDeliveryValidity>();
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

            var larsAnnualValues = Collection.Empty<ILARSAnnualValue>();
            larsAnnualValues.Add(mockLARSAnnualValues.Object);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetDeliveryFor(learnAimRef))
                .Returns(mockLARSDel.Object);
            service
                .Setup(x => x.GetValiditiesFor(learnAimRef))
                .Returns(larsValidities.AsSafeReadOnlyList());
            service
                .Setup(x => x.GetAnnualValuesFor(learnAimRef))
                .Returns(larsAnnualValues.AsSafeReadOnlyList());

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

            var dateTimeQueryService = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTimeQueryService
                .Setup(x => x.YearsBetween(dateOfBirth.Value, learnStartDate))
                .Returns(26);

            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);
            mockDDRule29
                .Setup(x => x.IsInflexibleElementOfTrainingAimLearningDelivery(mockDelivery.Object))
                .Returns(false);

            var organisationDataService = new Mock<IOrganisationDataService>(MockBehavior.Strict);
            organisationDataService
                .Setup(x => x.LegalOrgTypeMatchForUkprn(ukprn, "USDC"))
                .Returns(false);

            var fileDataService = new Mock<IFileDataService>(MockBehavior.Strict);
            fileDataService
                .Setup(x => x.UKPRN())
                .Returns(ukprn);

            var sut = new LearnDelFAMType_60Rule(
                validationErrorHandlerMock.Object,
                service.Object,
                mockDDRule07.Object,
                mockDDRule21.Object,
                mockDDRule28.Object,
                mockDDRule29.Object,
                organisationDataService.Object,
                fileDataService.Object,
                dateTimeQueryService.Object);

            // act
            sut.ValidateDeliveries(mockLearner.Object);

            // assert
            validationErrorHandlerMock.VerifyAll();
            service.VerifyAll();
            mockDDRule07.VerifyAll();
            mockDDRule21.VerifyAll();
            mockDDRule28.VerifyAll();
            mockDDRule29.VerifyAll();
            organisationDataService.VerifyAll();
            fileDataService.VerifyAll();
            dateTimeQueryService.VerifyAll();
        }

        [Fact]
        public void ValidItemDoesNotRaiseValidationMessage()
        {
            // arrange
            const string LearnRefNumber = "123456789X";
            const string learnAimRef = "salddfkjeifdnase";
            const int progType = 23;
            const int ukprn = 1000268;
            DateTime learnStartDate = new DateTime(2016, 8, 1);
            DateTime? dateOfBirth = new DateTime(1990, 7, 1);

            var mockFAM = new Mock<ILearningDeliveryFAM>();
            mockFAM
                .SetupGet(x => x.LearnDelFAMType)
                .Returns(Monitoring.Delivery.Types.FullOrCoFunding);
            mockFAM
                .SetupGet(x => x.LearnDelFAMCode)
                .Returns("2");

            var fams = Collection.Empty<ILearningDeliveryFAM>();
            fams.Add(mockFAM.Object);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);
            mockDelivery
               .SetupGet(y => y.ProgTypeNullable)
               .Returns(progType);
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse("2016-08-01"));
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(TypeOfFunding.AdultSkills);  // Not Adultskills
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
                .Returns(DateTime.Parse("1990-07-01"));
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries.AsSafeReadOnlyList());

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var mockCat = new Mock<ILARSLearningCategory>();
            mockCat
               .SetupGet(x => x.LearnAimRef)
               .Returns(learnAimRef);
            mockCat
                .SetupGet(x => x.CategoryRef)
                .Returns(TypeOfLARSCategory.LicenseToPractice);

            var larsCats = Collection.Empty<ILARSLearningCategory>();
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
                .Returns(larsCats.AsSafeReadOnlyList());

            var mockLARSValidity = new Mock<ILARSLearningDeliveryValidity>();
            mockLARSValidity
                .SetupGet(x => x.LearnAimRef)
                .Returns(learnAimRef);
            mockLARSValidity
                .SetupGet(x => x.ValidityCategory)
                .Returns(TypeOfLARSValidity.CommunityLearning);
            mockLARSValidity
                .SetupGet(x => x.LastNewStartDate)
                .Returns(new DateTime(2018, 08, 01));

            var larsValidities = Collection.Empty<ILARSLearningDeliveryValidity>();
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

            var larsAnnualValues = Collection.Empty<ILARSAnnualValue>();
            larsAnnualValues.Add(mockLARSAnnualValues.Object);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetDeliveryFor(learnAimRef))
                .Returns(mockLARSDel.Object);
            service
                .Setup(x => x.GetValiditiesFor(learnAimRef))
                .Returns(larsValidities.AsSafeReadOnlyList());
            service
                .Setup(x => x.GetAnnualValuesFor(learnAimRef))
                .Returns(larsAnnualValues.AsSafeReadOnlyList());

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

            var organisationDataService = new Mock<IOrganisationDataService>(MockBehavior.Strict);
            organisationDataService
                .Setup(x => x.LegalOrgTypeMatchForUkprn(ukprn, "USDC"))
                .Returns(false);

            var fileDataService = new Mock<IFileDataService>(MockBehavior.Strict);
            fileDataService
                .Setup(x => x.UKPRN())
                .Returns(ukprn);

            var dateTimeQueryService = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTimeQueryService
                .Setup(x => x.YearsBetween(dateOfBirth.Value, learnStartDate))
                .Returns(26);

            var sut = new LearnDelFAMType_60Rule(
                validationErrorHandlerMock.Object,
                service.Object,
                mockDDRule07.Object,
                mockDDRule21.Object,
                mockDDRule28.Object,
                mockDDRule29.Object,
                organisationDataService.Object,
                fileDataService.Object,
                dateTimeQueryService.Object);

            // act
            sut.ValidateDeliveries(mockLearner.Object);

            // assert
            validationErrorHandlerMock.VerifyAll();
            service.VerifyAll();
            mockDDRule07.VerifyAll();
            mockDDRule21.VerifyAll();
            mockDDRule28.VerifyAll();
            mockDDRule29.VerifyAll();
            organisationDataService.VerifyAll();
            fileDataService.VerifyAll();
        }

        public LearnDelFAMType_60Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var organisationDataService = new Mock<IOrganisationDataService>(MockBehavior.Strict);
            var fileDataService = new Mock<IFileDataService>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);
            var dateTimeQueryService = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            return new LearnDelFAMType_60Rule(
                handler.Object,
                service.Object,
                mockDDRule07.Object,
                mockDDRule21.Object,
                mockDDRule28.Object,
                mockDDRule29.Object,
                organisationDataService.Object,
                fileDataService.Object,
                dateTimeQueryService.Object);
        }
    }
}
