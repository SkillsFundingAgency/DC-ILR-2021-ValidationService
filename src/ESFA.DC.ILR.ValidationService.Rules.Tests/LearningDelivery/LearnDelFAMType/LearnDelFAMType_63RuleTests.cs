using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
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
    public class LearnDelFAMType_63RuleTests
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var mockService = new Mock<ILARSDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnDelFAMType_63Rule(null, mockService.Object));
        }

        /// <summary>
        /// New rule with null lars service throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullLARSServiceThrows()
        {
            // arrange
            var mockHandler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnDelFAMType_63Rule(mockHandler.Object, null));
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
            Assert.Equal("LearnDelFAMType_63", result);
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
            Assert.Equal(LearnDelFAMType_63Rule.Name, result);
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
        /// Is apprenticeship meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(TypeOfFunding.AdultSkills, false)]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, false)]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, true)]
        [InlineData(TypeOfFunding.CommunityLearning, false)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, false)]
        [InlineData(TypeOfFunding.NotFundedByESFA, false)]
        [InlineData(TypeOfFunding.Other16To19, false)]
        [InlineData(TypeOfFunding.OtherAdult, false)]
        public void IsApprenticeshipMeetsExpectation(int candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(candidate);

            // act
            var result = sut.IsApprenticeship(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(TypeOfAim.AimNotPartOfAProgramme, false)]
        [InlineData(TypeOfAim.ComponentAimInAProgramme, false)]
        [InlineData(TypeOfAim.CoreAim16To19ExcludingApprenticeships, false)]
        [InlineData(TypeOfAim.ProgrammeAim, true)]
        public void IsProgrammeAimMeetsExpectation(int candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.AimType)
                .Returns(candidate);

            // act
            var result = sut.IsProgrammeAim(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(TypeOfAim.AimNotPartOfAProgramme, false)]
        [InlineData(TypeOfAim.ComponentAimInAProgramme, true)]
        [InlineData(TypeOfAim.CoreAim16To19ExcludingApprenticeships, false)]
        [InlineData(TypeOfAim.ProgrammeAim, false)]
        public void IsComponentAimMeetsExpectation(int candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.AimType)
                .Returns(candidate);

            // act
            var result = sut.IsComponentAim(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Is apprenticeship contract meets expectation
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(Monitoring.Delivery.Types.AdvancedLearnerLoan, false)]
        [InlineData(Monitoring.Delivery.Types.AdvancedLearnerLoansBursaryFunding, false)]
        [InlineData(Monitoring.Delivery.Types.ApprenticeshipContract, true)]
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
        [InlineData(Monitoring.Delivery.Types.Restart, false)]
        [InlineData(Monitoring.Delivery.Types.SourceOfFunding, false)]
        [InlineData(Monitoring.Delivery.Types.WorkProgrammeParticipation, false)]
        public void IsApprenticeshipContractMeetsExpectation(string candidate, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDeliveryFAM>();
            mockDelivery
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(candidate);

            // act
            var result = sut.IsApprenticeshipContract(mockDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
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

        [Fact]
        public void IsCommonComponent_ReturnsTrue()
        {
            // arrange
            var frameworkComponents = new List<ILARSFrameworkCommonComponent>()
            {
                new FrameworkCommonComponent { CommonComponent = TypeOfLARSCommonComponent.BritishSignLanguage },
                new FrameworkCommonComponent { CommonComponent = TypeOfLARSCommonComponent.FunctionalSkillsEnglish }
            };

            var sut = NewRule();
            var mocLarsFramework = new Mock<ILARSFramework>();
            mocLarsFramework
                    .SetupGet(y => y.FrameworkCommonComponents)
                    .Returns(frameworkComponents);

            // act
            var result = sut.IsCommonComponent(mocLarsFramework.Object);

            // assert
            Assert.True(result);
        }

        [Fact]
        public void IsCommonComponent_False_AsWrongCommonComponent()
        {
            // arrange
            var frameworkComponents = new List<ILARSFrameworkCommonComponent>()
            {
                new FrameworkCommonComponent { CommonComponent = TypeOfLARSCommonComponent.FunctionalSkillsEnglish }
            };

            var sut = NewRule();
            var mocLarsFramework = new Mock<ILARSFramework>();
            mocLarsFramework
                    .SetupGet(y => y.FrameworkCommonComponents)
                    .Returns(frameworkComponents);

            // act
            var result = sut.IsCommonComponent(mocLarsFramework.Object);

            // assert
            Assert.False(result);
        }

        [Fact]
        public void IsCommonComponent_False_DueToEmptyList()
        {
            // arrange
            var frameworkComponents = new List<ILARSFrameworkCommonComponent>();

            var sut = NewRule();
            var mocLarsFramework = new Mock<ILARSFramework>();
            mocLarsFramework
                    .SetupGet(y => y.FrameworkCommonComponents)
                    .Returns(frameworkComponents);

            // act
            var result = sut.IsCommonComponent(mocLarsFramework.Object);

            // assert
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
            const int fundModel = TypeOfFunding.ApprenticeshipsFrom1May2017;
            const int aimType = 2;
            const string famType = Monitoring.Delivery.Types.ApprenticeshipContract;

            var mockFAM = new Mock<ILearningDeliveryFAM>();
            mockFAM
                .SetupGet(x => x.LearnDelFAMType)
                .Returns(famType);

            var fams = Collection.Empty<ILearningDeliveryFAM>();
            fams.Add(mockFAM.Object);

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(learnAimRef);
            mockDelivery
                .SetupGet(y => y.AimType)
                .Returns(aimType);
            mockDelivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse("2017-08-01"));
            mockDelivery
                .SetupGet(y => y.FundModel)
                .Returns(fundModel);
            mockDelivery
                .SetupGet(y => y.LearningDeliveryFAMs)
                .Returns(fams.AsSafeReadOnlyList());

            var deliveries = Collection.Empty<ILearningDelivery>();
            deliveries.Add(mockDelivery.Object);

            var mockLearner = new Mock<ILearner>();
            mockLearner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(learnRefNumber);
            mockLearner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries.AsSafeReadOnlyList());

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle("LearnDelFAMType_63", learnRefNumber, 0, Moq.It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("AimType", aimType))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("FundModel", fundModel))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnDelFAMType", famType))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var service = new Mock<ILARSDataService>(MockBehavior.Strict);

            var sut = new LearnDelFAMType_63Rule(handler.Object, service.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
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
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries.AsSafeReadOnlyList());

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);

            var sut = new LearnDelFAMType_63Rule(handler.Object, service.Object);

            // act
            sut.Validate(mockLearner.Object);

            // assert
            handler.VerifyAll();
            service.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule</returns>
        public LearnDelFAMType_63Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var service = new Mock<ILARSDataService>(MockBehavior.Strict);

            return new LearnDelFAMType_63Rule(handler.Object, service.Object);
        }
    }
}
