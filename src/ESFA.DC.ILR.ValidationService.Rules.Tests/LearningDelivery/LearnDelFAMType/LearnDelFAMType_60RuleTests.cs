using System;
using System.Collections.Generic;
using System.Globalization;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMType
{
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

            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);

            mockDDRule07
                .Setup(x => x.IsApprenticeship(null))
                .Returns(expectation);

            var sut = NewRule(dd07: mockDDRule07.Object);

            // act
            var result = sut.IsApprenticeship(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
            mockDDRule07.VerifyAll();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void IsInflexibleElementOfTrainingAimMeetsExpectation(bool expectation)
        {
            // arrange
            var mockItem = new Mock<ILearningDelivery>();

            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);
            mockDDRule29
                .Setup(x => x.IsInflexibleElementOfTrainingAimLearningDelivery(mockItem.Object))
                .Returns(expectation);

            var sut = NewRule(dd29: mockDDRule29.Object);

            // act
            var result = sut.IsInflexibleElementOfTrainingAim(mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
            mockDDRule29.VerifyAll();
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

            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            mockDDRule21
                .Setup(x => x.IsAdultFundedUnemployedWithOtherStateBenefits(delivery.Object, mockItem.Object))
                .Returns(expectation);

            var sut = NewRule(dd21: mockDDRule21.Object);

            // act
            var result = sut.IsAdultFundedUnemployedWithOtherStateBenefits(delivery.Object, mockItem.Object);

            // assert
            Assert.Equal(expectation, result);
            mockDDRule21.VerifyAll();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void IsAdultFundedUnemployedWithBenefitsMeetsExpectation(bool expectation)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();
            var learner = new Mock<ILearner>();

            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            mockDDRule28
                .Setup(x => x.IsAdultFundedUnemployedWithBenefits(delivery.Object, learner.Object))
                .Returns(expectation);

            var sut = NewRule(dd28: mockDDRule28.Object);

            // act
            var result = sut.IsAdultFundedUnemployedWithBenefits(delivery.Object, learner.Object);

            // assert
            Assert.Equal(expectation, result);
            mockDDRule28.VerifyAll();
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
        [InlineData(123456, true)]
        [InlineData(67890, false)]
        public void IsLegalOrgTypeMatchForUkprnMeetsExpectation(int candidate, bool expectation)
        {
            // arrange
            var organisationDataService = new Mock<IOrganisationDataService>();
            var fileDataService = new Mock<IFileDataService>(MockBehavior.Strict);

            fileDataService.Setup(x => x.UKPRN()).Returns(candidate);

            organisationDataService
                .Setup(x => x.LegalOrgTypeMatchForUkprn(fileDataService.Object.UKPRN(), "USDC"))
                .Returns(expectation);

            var sut = NewRule(
                organisationDataService: organisationDataService.Object,
                fileDataService: fileDataService.Object);

            // act
            var result = sut.IsLegalOrgTypeMatchForUkprn();

            // assert
            Assert.Equal(expectation, result);
            organisationDataService.VerifyAll();
            fileDataService.VerifyAll();
        }

        [Theory]
        [InlineData("2016-08-01", true)]
        [InlineData("2017-07-31", true)]
        [InlineData("2017-08-01", false)]
        [InlineData("2017-09-14", false)]
        public void IsViableStartMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse(candidate));
            var dateTimeQueryService = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTimeQueryService
                .Setup(x => x.IsDateBetween(mockDelivery.Object.LearnStartDate, Moq.It.IsAny<DateTime>(), Moq.It.IsAny<DateTime>(), true))
                .Returns(expectation);

            // act
            var result = NewRule(dateTimeQueryService: dateTimeQueryService.Object).IsViableStart(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
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
            var dateTimeQueryService = new Mock<IDateTimeQueryService>(MockBehavior.Strict);

            dateTimeQueryService
                .Setup(x => x.YearsBetween(birthDate, learnStartDate))
                .Returns(yearsOfAge);

            var sut = NewRule(dateTimeQueryService: dateTimeQueryService.Object);

            // act
            var result = sut.IsTargetAgeGroup(mockLearner.Object, mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
            dateTimeQueryService.VerifyAll();
        }

        [Theory]
        [InlineData(LARSConstants.NotionalNVQLevelV2Strings.Level2, true)]
        [InlineData(LARSConstants.NotionalNVQLevelV2Strings.EntryLevel, true)]
        [InlineData(LARSConstants.NotionalNVQLevelV2Strings.Level1, true)]
        [InlineData(LARSConstants.NotionalNVQLevelV2Strings.HigherLevel, false)]
        public void IsEarlyStageNVQMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var larsDataService = new Mock<ILARSDataService>(MockBehavior.Strict);
            var mockItem = new Mock<ILARSLearningDelivery>();

            mockItem.SetupGet(y => y.NotionalNVQLevelv2).Returns(candidate);
            larsDataService
                .Setup(x => x.GetDeliveryFor(Moq.It.IsAny<string>()))
                .Returns(mockItem.Object);

            var sut = NewRule(larsDataService: larsDataService.Object);

            // act
            var result = sut.IsEarlyStageNVQ(new TestLearningDelivery());

            // assert
            Assert.Equal(expectation, result);
            larsDataService.VerifyAll();
        }

        [Fact]
        public void IsEarlyStageNVQMeetsExpectation_NullLarsDelivery()
        {
            var larsDataService = new Mock<ILARSDataService>(MockBehavior.Strict);
            var mockItem = new Mock<ILARSLearningDelivery>();

            mockItem.SetupGet(y => y.NotionalNVQLevelv2).Returns(LARSConstants.NotionalNVQLevelV2Strings.Level2);
            larsDataService
                .Setup(x => x.GetDeliveryFor(Moq.It.IsAny<string>()))
                .Returns((ILARSLearningDelivery)null);

            var sut = NewRule(larsDataService: larsDataService.Object);

            // act
            var result = sut.IsEarlyStageNVQ(new TestLearningDelivery());

            // assert
            Assert.Equal(false, result);
            larsDataService.VerifyAll();
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

            var mockDDRule07 = new Mock<IDerivedData_07Rule>();
            var mockDDRule29 = new Mock<IDerivedData_29Rule>();
            mockDDRule29
                .Setup(x => x.IsInflexibleElementOfTrainingAimLearningDelivery(mockItem.Object))
                .Returns(true);

            var sut = NewRule(
                dd07: mockDDRule07.Object,
                dd29: mockDDRule29.Object);

            // act
            var result = sut.IsExcluded(mockItem.Object);

            // assert
            Assert.True(result);
            mockDDRule29.VerifyAll();
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

            var deliveries = new List<ILearningDelivery>
            {
                mockDel.Object
            };

            var mockDDRule07 = new Mock<IDerivedData_07Rule>();

            mockDDRule07
                .Setup(x => x.IsApprenticeship(progType))
                .Returns(true);

            var sut = NewRule(dd07: mockDDRule07.Object);

            // act
            var result = sut.IsExcluded(mockDel.Object);

            // assert
            Assert.True(result);
            mockDDRule07.VerifyAll();
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
                .Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, Monitoring.Delivery.Types.FullOrCoFunding))
                .Verifiable();
            validationErrorHandlerMock
                .Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, "1"))
                .Verifiable();
            validationErrorHandlerMock
                .Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.FundModel, FundModels.AdultSkills))
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

            var mockLARSValidity = new Mock<ILARSLearningDeliveryValidity>();
            mockLARSValidity
                .SetupGet(x => x.LearnAimRef)
                .Returns(learnAimRef);
            mockLARSValidity
                .SetupGet(x => x.ValidityCategory)
                .Returns(LARSConstants.Validities.CommunityLearning);
            mockLARSValidity
                .SetupGet(x => x.LastNewStartDate)
                .Returns(new DateTime(2018, 08, 01));

            var larsValidities = new List<ILARSLearningDeliveryValidity>
            {
                mockLARSValidity.Object
            };

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

            var dateTimeQueryService = new Mock<IDateTimeQueryService>(MockBehavior.Strict);
            dateTimeQueryService
                .Setup(x => x.YearsBetween(dateOfBirth.Value, learnStartDate))
                .Returns(26);
            dateTimeQueryService
                .Setup(x => x.IsDateBetween(mockDelivery.Object.LearnStartDate, Moq.It.IsAny<DateTime>(), Moq.It.IsAny<DateTime>(), true))
                .Returns(true);

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

            var sut = NewRule(
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
                .Returns(DateTime.Parse("2016-08-01"));
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(FundModels.AdultSkills);  // Not Adultskills
            mockDelivery
                .SetupGet(y => y.LearningDeliveryFAMs)
                .Returns(fams);

            var deliveries = new List<ILearningDelivery>
            {
                mockDelivery.Object
            };

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.DateOfBirthNullable)
                .Returns(DateTime.Parse("1990-07-01"));
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

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

            var mockLARSValidity = new Mock<ILARSLearningDeliveryValidity>();
            mockLARSValidity
                .SetupGet(x => x.LearnAimRef)
                .Returns(learnAimRef);
            mockLARSValidity
                .SetupGet(x => x.ValidityCategory)
                .Returns(LARSConstants.Validities.CommunityLearning);
            mockLARSValidity
                .SetupGet(x => x.LastNewStartDate)
                .Returns(new DateTime(2018, 08, 01));

            var larsValidities = new List<ILARSLearningDeliveryValidity>
            {
                mockLARSValidity.Object
            };

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
            dateTimeQueryService
                .Setup(x => x.IsDateBetween(mockDelivery.Object.LearnStartDate, Moq.It.IsAny<DateTime>(), Moq.It.IsAny<DateTime>(), true))
                .Returns(true);

            var sut = NewRule(
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

        public LearnDelFAMType_60Rule NewRule(
            IValidationErrorHandler handler = null,
            ILARSDataService larsDataService = null,
            IDerivedData_07Rule dd07 = null,
            IDerivedData_21Rule dd21 = null,
            IDerivedData_28Rule dd28 = null,
            IDerivedData_29Rule dd29 = null,
            IOrganisationDataService organisationDataService = null,
            IFileDataService fileDataService = null,
            IDateTimeQueryService dateTimeQueryService = null)
        {
            return new LearnDelFAMType_60Rule(
                handler,
                larsDataService,
                dd07,
                dd21,
                dd28,
                dd29,
                organisationDataService,
                fileDataService,
                dateTimeQueryService);
        }
    }
}
