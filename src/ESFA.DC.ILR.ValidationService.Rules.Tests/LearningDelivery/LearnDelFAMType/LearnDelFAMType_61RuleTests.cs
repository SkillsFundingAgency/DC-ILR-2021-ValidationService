using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using ESFA.DC.ILR.ValidationService.Utility;
using FluentAssertions;
using Moq;
using System;
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

            mockDDRule28
                .Setup(x => x.IsAdultFundedUnemployedWithBenefits(delivery.Object, learner.Object))
                .Returns(expectation);

            var sut = new LearnDelFAMType_61Rule(handler.Object, service.Object, mockDDRule07.Object, mockDDRule21.Object, mockDDRule28.Object, mockDDRule29.Object);

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

            var sut = new LearnDelFAMType_61Rule(handler.Object, service.Object, mockDDRule07.Object, mockDDRule21.Object, mockDDRule28.Object, mockDDRule29.Object);

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

            var sut = new LearnDelFAMType_61Rule(handler.Object, service.Object, mockDDRule07.Object, mockDDRule21.Object, mockDDRule28.Object, mockDDRule29.Object);

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

            var sut = new LearnDelFAMType_61Rule(handler.Object, service.Object, mockDDRule07.Object, mockDDRule21.Object, mockDDRule28.Object, mockDDRule29.Object);

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
        [InlineData("1997-08-23", "2018-04-18", true)] // in age group
        [InlineData("1998-05-11", "2018-04-18", true)] // in age group
        [InlineData("1999-04-18", "2018-04-18", true)] // in age group, lower boundary
        [InlineData("1999-04-19", "2018-04-18", false)] // too young
        [InlineData("1995-04-18", "2018-04-18", true)] // in age group, upper boundary
        [InlineData("1995-04-17", "2018-04-18", false)] // too old
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
        [InlineData(LARSNotionalNVQLevelV2.Level2, true)]
        [InlineData(LARSNotionalNVQLevelV2.EntryLevel, false)]
        [InlineData(LARSNotionalNVQLevelV2.HigherLevel, false)]
        [InlineData(LARSNotionalNVQLevelV2.Level1, false)]
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
        [InlineData(TypeOfLARSCategory.LegalEntitlementLevel2, false)]
        [InlineData(TypeOfLARSCategory.WorkPlacementSFAFunded, true)]
        [InlineData(TypeOfLARSCategory.WorkPreparationSFATraineeships, true)]
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

            var larsCats = Collection.Empty<ILARSLearningCategory>();
            larsCats.Add(mockCat.Object);

            var mock = new Mock<ILARSLearningDelivery>();
            mock
                .SetupGet(x => x.NotionalNVQLevelv2)
                .Returns(LARSNotionalNVQLevelV2.Level2);
            mock
                .SetupGet(x => x.Categories)
                .Returns(larsCats.AsSafeReadOnlyList());

            var service = new Mock<ILARSDataService>();
            service
                .Setup(x => x.GetDeliveryFor(learnAimRef))
                .Returns(mock.Object);

            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            var sut = new LearnDelFAMType_61Rule(validationErrorHandlerMock.Object, service.Object, mockDDRule07.Object, mockDDRule21.Object, mockDDRule28.Object, mockDDRule29.Object);

            // act
            var result = sut.IsNotEntitled(new TestLearningDelivery() { LearnAimRef = learnAimRef });

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

        [Fact]
        public void IsExcludedForAdultFundedUnemployedWithBenefits()
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

            var sut = new LearnDelFAMType_61Rule(handler.Object, service.Object, mockDDRule07.Object, mockDDRule21.Object, mockDDRule28.Object, mockDDRule29.Object);

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

            var sut = new LearnDelFAMType_61Rule(handler.Object, service.Object, mockDDRule07.Object, mockDDRule21.Object, mockDDRule28.Object, mockDDRule29.Object);

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

            var sut = new LearnDelFAMType_61Rule(handler.Object, service.Object, mockDDRule07.Object, mockDDRule21.Object, mockDDRule28.Object, mockDDRule29.Object);

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

        [Fact]
        public void InvalidItemRaisesValidationMessage()
        {
            // arrange
            IFormatProvider requiredCulture = new CultureInfo("en-GB");
            const string LearnRefNumber = "123456789X";
            const string learnAimRef = "salddfkjeifdnase";
            DateTime learnStartDate = new DateTime(2017, 8, 1);
            DateTime? dateOfBirth = new DateTime(1996, 7, 1);

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
                .SetupGet(x => x.PriorAttainNullable)
                .Returns(TypeOfPriorAttainment.FullLevel2);
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
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, Monitoring.Delivery.Types.FullOrCoFunding)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, "1")).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.FundModel, TypeOfFunding.AdultSkills)).Verifiable();
            validationErrorHandlerMock
                .Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, learnStartDate.ToString("d", requiredCulture)))
                .Verifiable();
            validationErrorHandlerMock
                .Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.DateOfBirth, dateOfBirth.Value.ToString("d", requiredCulture)))
                .Verifiable();

            var mockCat = new Mock<ILARSLearningCategory>();
            mockCat
                .SetupGet(x => x.CategoryRef)
                .Returns(TypeOfLARSCategory.LicenseToPractice);

            var larsCats = Collection.Empty<ILARSLearningCategory>();
            larsCats.Add(mockCat.Object);

            var mock = new Mock<ILARSLearningDelivery>();
            mock
                .SetupGet(x => x.NotionalNVQLevelv2)
                .Returns(LARSNotionalNVQLevelV2.Level2);
            mock
                .SetupGet(x => x.Categories)
                .Returns(larsCats.AsSafeReadOnlyList());

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

            var sut = new LearnDelFAMType_61Rule(validationErrorHandlerMock.Object, service.Object, mockDDRule07.Object, mockDDRule21.Object, mockDDRule28.Object, mockDDRule29.Object);

            // act
            sut.ValidateDeliveries(mockLearner.Object);

            // assert
            validationErrorHandlerMock.VerifyAll();
            service.VerifyAll();
            mockDDRule07.VerifyAll();
            mockDDRule21.VerifyAll();
            mockDDRule28.VerifyAll();
            mockDDRule29.VerifyAll();
        }

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
                .SetupGet(x => x.PriorAttainNullable)
                .Returns(TypeOfPriorAttainment.FullLevel3);
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            mockLearner
                .SetupGet(x => x.DateOfBirthNullable)
                .Returns(DateTime.Parse("1996-07-01"));
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries.AsSafeReadOnlyList());

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var mockCat = new Mock<ILARSLearningCategory>();
            mockCat
                .SetupGet(x => x.CategoryRef)
                .Returns(TypeOfLARSCategory.LegalEntitlementLevel2);

            var larsCats = Collection.Empty<ILARSLearningCategory>();
            larsCats.Add(mockCat.Object);

            var mockLARSDel = new Mock<ILARSLearningDelivery>();
            mockLARSDel
                .SetupGet(x => x.NotionalNVQLevelv2)
                .Returns(LARSNotionalNVQLevelV2.Level3);
            mockLARSDel
                .SetupGet(x => x.Categories)
                .Returns(larsCats.AsSafeReadOnlyList());

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            service
                .Setup(x => x.GetDeliveryFor(learnAimRef))
                .Returns(mockLARSDel.Object);

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

            var sut = new LearnDelFAMType_61Rule(handler.Object, service.Object, mockDDRule07.Object, mockDDRule21.Object, mockDDRule28.Object, mockDDRule29.Object);

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

        public LearnDelFAMType_61Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);
            var mockDDRule07 = new Mock<IDerivedData_07Rule>(MockBehavior.Strict);
            var mockDDRule21 = new Mock<IDerivedData_21Rule>(MockBehavior.Strict);
            var mockDDRule28 = new Mock<IDerivedData_28Rule>(MockBehavior.Strict);
            var mockDDRule29 = new Mock<IDerivedData_29Rule>(MockBehavior.Strict);

            return new LearnDelFAMType_61Rule(handler.Object, service.Object, mockDDRule07.Object, mockDDRule21.Object, mockDDRule28.Object, mockDDRule29.Object);
        }
    }
}
