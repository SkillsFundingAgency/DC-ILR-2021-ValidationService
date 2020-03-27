using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_67RuleTests : AbstractRuleTests<LearnDelFAMType_67Rule>
    {
        /// <summary>
        /// New rule with null message handler throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullMessageHandlerThrows()
        {
            // arrange
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnDelFAMType_67Rule(null, commonOps.Object, larsData.Object));
        }

        /// <summary>
        /// New rule with null common ops throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullCommonOpsThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnDelFAMType_67Rule(handler.Object, null, larsData.Object));
        }

        /// <summary>
        /// New rule with null lars data throws.
        /// </summary>
        [Fact]
        public void NewRuleWithNullLarsDataThrows()
        {
            // arrange
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            // act / assert
            Assert.Throws<ArgumentNullException>(() => new LearnDelFAMType_67Rule(handler.Object, commonOps.Object, null));
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
            Assert.Equal("LearnDelFAMType_67", result);
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
            Assert.Equal(RuleNameConstants.LearnDelFAMType_67, result);
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

            // act / assert
            Assert.Throws<ArgumentNullException>(() => sut.Validate(null));
        }

        /// <summary>
        /// Gets the nullable date.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>a nullable date</returns>
        public DateTime? GetNullableDate(string candidate) =>
            string.IsNullOrWhiteSpace(candidate) ? (DateTime?)null : DateTime.Parse(candidate);

        /// <summary>
        /// Type of funding meets expectation.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData(36, TypeOfFunding.ApprenticeshipsFrom1May2017)]
        public void TypeOfFundingMeetsExpectation(int expectation, int candidate)
        {
            // arrange / act / assert
            Assert.Equal(expectation, candidate);
        }

        /// <summary>
        /// Has qualifying (fund) model meets expectation
        /// </summary>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void HasQualifyingModelMeetsExpectation(bool expectation)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(delivery.Object, 36))
                .Returns(expectation);

            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);

            var sut = new LearnDelFAMType_67Rule(handler.Object, commonOps.Object, larsData.Object);

            // act
            var result = sut.HasQualifyingModel(delivery.Object);

            // assert
            Assert.Equal(expectation, result);

            handler.VerifyAll();
            commonOps.VerifyAll();
            larsData.VerifyAll();
        }

        /// <summary>
        /// Is component aim meets expectation
        /// </summary>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void IsComponentAimMeetsExpectation(bool expectation)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.IsComponentOfAProgram(delivery.Object))
                .Returns(expectation);

            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);

            var sut = new LearnDelFAMType_67Rule(handler.Object, commonOps.Object, larsData.Object);

            // act
            var result = sut.IsComponentAim(delivery.Object);

            // assert
            Assert.Equal(expectation, result);

            handler.VerifyAll();
            commonOps.VerifyAll();
            larsData.VerifyAll();
        }

        /// <summary>
        /// Has qualifying basic skills type with no annual values returns false
        /// </summary>
        [Fact]
        public void HasQualifyingBasicSkillsTypeWithNoAnnualValuesReturnsFalse()
        {
            // arrange
            var learnAimRef = "shonkyref";
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.LearnAimRef)
                .Returns(learnAimRef);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            larsData
                .Setup(x => x.GetAnnualValuesFor(learnAimRef))
                .Returns(new ILARSAnnualValue[] { });

            var sut = new LearnDelFAMType_67Rule(handler.Object, commonOps.Object, larsData.Object);

            // act
            var result = sut.HasQualifyingBasicSkillsType(delivery.Object);

            // assert
            Assert.False(result);

            handler.VerifyAll();
            commonOps.VerifyAll();
            larsData.VerifyAll();
        }

        /// <summary>
        /// Has a basic skill type meets expectation
        /// </summary>
        /// <param name="basicSkill">The basic skill.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(null, false)]
        [InlineData(1, true)]
        [InlineData(32, true)]
        public void HasABasicSkillTypeMeetsExpectation(int? basicSkill, bool expectation)
        {
            // arrange
            var annualValue = new Mock<ILARSAnnualValue>();
            annualValue
                .SetupGet(x => x.BasicSkillsType)
                .Returns(basicSkill);

            var sut = NewRule();

            // act
            var result = sut.HasABasicSkillType(annualValue.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Types of lars basic skill meets expectation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">The expectation.</param>
        [Theory]
        [InlineData(1, TypeOfLARSBasicSkill.Certificate_AdultLiteracy)]
        [InlineData(2, TypeOfLARSBasicSkill.Certificate_AdultNumeracy)]
        [InlineData(11, TypeOfLARSBasicSkill.GCSE_EnglishLanguage)]
        [InlineData(12, TypeOfLARSBasicSkill.GCSE_Mathematics)]
        [InlineData(13, TypeOfLARSBasicSkill.KeySkill_Communication)]
        [InlineData(14, TypeOfLARSBasicSkill.KeySkill_ApplicationOfNumbers)]
        [InlineData(19, TypeOfLARSBasicSkill.FunctionalSkillsMathematics)]
        [InlineData(20, TypeOfLARSBasicSkill.FunctionalSkillsEnglish)]
        [InlineData(21, TypeOfLARSBasicSkill.UnitsOfTheCertificate_AdultNumeracy)]
        [InlineData(23, TypeOfLARSBasicSkill.UnitsOfTheCertificate_AdultLiteracy)]
        [InlineData(24, TypeOfLARSBasicSkill.NonNQF_QCFS4LLiteracy)]
        [InlineData(25, TypeOfLARSBasicSkill.NonNQF_QCFS4LNumeracy)]
        [InlineData(29, TypeOfLARSBasicSkill.QCFBasicSkillsEnglishLanguage)]
        [InlineData(30, TypeOfLARSBasicSkill.QCFBasicSkillsMathematics)]
        [InlineData(31, TypeOfLARSBasicSkill.UnitQCFBasicSkillsEnglishLanguage)]
        [InlineData(32, TypeOfLARSBasicSkill.UnitQCFBasicSkillsMathematics)]
        [InlineData(33, TypeOfLARSBasicSkill.InternationalGCSEEnglishLanguage)]
        [InlineData(34, TypeOfLARSBasicSkill.InternationalGCSEMathematics)]
        [InlineData(35, TypeOfLARSBasicSkill.FreeStandingMathematicsQualification)]
        public void TypeOfLARSBasicSkillMeetsExpectation(int candidate, int expectation)
        {
            // arrange / act / assert
            Assert.Equal(expectation, candidate);
        }

        /// <summary>
        /// As english and maths basic skills meets expectation.
        /// </summary>
        /// <param name="basicSkill">The basic skill.</param>
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(11)]
        [InlineData(12)]
        [InlineData(13)]
        [InlineData(14)]
        [InlineData(19)]
        [InlineData(20)]
        [InlineData(21)]
        [InlineData(23)]
        [InlineData(24)]
        [InlineData(25)]
        [InlineData(29)]
        [InlineData(30)]
        [InlineData(31)]
        [InlineData(32)]
        [InlineData(33)]
        [InlineData(34)]
        [InlineData(35)]
        public void AsEnglishAndMathsBasicSkillsMeetsExpectation(int basicSkill)
        {
            // arrange / act / assert
            Assert.Contains(basicSkill, TypeOfLARSBasicSkill.AsEnglishAndMathsBasicSkills);
        }

        /// <summary>
        /// Is english or math basic skill meets expectation
        /// </summary>
        /// <param name="basicSkill">The basic skill.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(1, true)]
        [InlineData(2, true)]
        [InlineData(3, false)]
        [InlineData(4, false)]
        [InlineData(5, false)]
        [InlineData(6, false)]
        [InlineData(11, true)]
        [InlineData(12, true)]
        [InlineData(13, true)]
        [InlineData(14, true)]
        [InlineData(15, false)]
        [InlineData(16, false)]
        [InlineData(17, false)]
        [InlineData(19, true)]
        [InlineData(20, true)]
        [InlineData(21, true)]
        [InlineData(23, true)]
        [InlineData(24, true)]
        [InlineData(25, true)]
        [InlineData(26, false)]
        [InlineData(27, false)]
        [InlineData(28, false)]
        [InlineData(29, true)]
        [InlineData(30, true)]
        [InlineData(31, true)]
        [InlineData(32, true)]
        [InlineData(33, true)]
        [InlineData(34, true)]
        [InlineData(35, true)]
        [InlineData(36, false)]
        public void IsEnglishOrMathBasicSkillMeetsExpectation(int basicSkill, bool expectation)
        {
            // arrange
            var annualValue = new Mock<ILARSAnnualValue>();
            annualValue
                .SetupGet(x => x.BasicSkillsType)
                .Returns(basicSkill);

            var sut = NewRule();

            // act
            var result = sut.IsEnglishOrMathBasicSkill(annualValue.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Is value current meets expectation
        /// the 'is current' extension method is tested more thoroughly elsewhere
        /// </summary>
        /// <param name="learnStart">The learn start.</param>
        /// <param name="valueStart">The value start.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("2018-04-01", "2018-04-01", true)]
        [InlineData("2018-04-01", "2018-04-02", false)]
        public void IsValueCurrentMeetsExpectation(string learnStart, string valueStart, bool expectation)
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(DateTime.Parse(learnStart));

            var annualValue = new Mock<ILARSAnnualValue>();
            annualValue
                .SetupGet(x => x.StartDate)
                .Returns(DateTime.Parse(valueStart));

            var sut = NewRule();

            // act
            var result = sut.IsValueCurrent(delivery.Object, annualValue.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Get lars aim meets expectation.
        /// </summary>
        [Fact]
        public void GetLarsAimMeetsExpectation()
        {
            // arrange
            var learnAimRef = "shonkyref";
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.LearnAimRef)
                .Returns(learnAimRef);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);

            var larsAim = new Mock<ILARSLearningDelivery>();

            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            larsData
                .Setup(x => x.GetDeliveryFor(learnAimRef))
                .Returns(larsAim.Object);

            var sut = new LearnDelFAMType_67Rule(handler.Object, commonOps.Object, larsData.Object);

            // act
            var result = sut.GetLarsAim(delivery.Object);

            // assert
            Assert.Equal(larsAim.Object, result);

            handler.VerifyAll();
            commonOps.VerifyAll();
            larsData.VerifyAll();
        }

        /// <summary>
        /// Has qualifying common component with null lars delivery returns false
        /// </summary>
        [Fact]
        public void HasQualifyingCommonComponentWithNullLarsDeliveryReturnsFalse()
        {
            // arrange
            var sut = NewRule();

            // act
            var result = sut.HasQualifyingCommonComponent(null);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Has qualifying common component with null frameworks returns false
        /// </summary>
        [Fact]
        public void HasQualifyingCommonComponentWithNullFrameworksReturnsFalse()
        {
            // arrange
            var delivery = new Mock<ILARSLearningDelivery>();

            var sut = NewRule();

            // act
            var result = sut.HasQualifyingCommonComponent(delivery.Object);

            // assert
            Assert.False(result);
        }

        /// <summary>
        /// Types of lars common component meet expectation.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="expectation">The expectation.</param>
        [Theory]
        [InlineData(20, TypeOfLARSCommonComponent.BritishSignLanguage)]
        public void TypeOfLARSCommonComponentMeetsExpectation(int candidate, int expectation)
        {
            // arrange / act / assert
            Assert.Equal(expectation, candidate);
        }

        /// <summary>
        /// Is british sign language meets expectation.
        /// </summary>
        /// <param name="component">The component.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData(null, false)]
        [InlineData(19, false)]
        [InlineData(20, true)]
        [InlineData(21, false)]
        public void IsBritishSignLanguageMeetsExpectation(int? component, bool expectation)
        {
            // arrange
            var larsDelivery = new Mock<ILARSLearningDelivery>();
            larsDelivery
                .SetupGet(x => x.FrameworkCommonComponent)
                .Returns(component);

            var sut = NewRule();

            // act
            var result = sut.IsBritishSignLanguage(larsDelivery.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Has qualifying monitor with null fams returns false.
        /// </summary>
        [Fact]
        public void HasDisqualifyingMonitorWithNullFAMsReturnsFalse()
        {
            // arrange
            var delivery = new Mock<ILearningDelivery>();

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.CheckDeliveryFAMs(delivery.Object, It.IsAny<Func<ILearningDeliveryFAM, bool>>()))
                .Returns(false);

            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);

            var sut = new LearnDelFAMType_67Rule(handler.Object, commonOps.Object, larsData.Object);

            // act
            var result = sut.HasDisqualifyingMonitor(delivery.Object);

            // assert
            Assert.False(result);

            handler.VerifyAll();
            commonOps.VerifyAll();
            larsData.VerifyAll();
        }

        /// <summary>
        /// Monitoring code meets expectation.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        /// <param name="candidate">The candidate.</param>
        [Theory]
        [InlineData("ACT1", Monitoring.Delivery.ApprenticeshipFundedThroughAContractForServicesWithEmployer)]
        [InlineData("ACT2", Monitoring.Delivery.ApprenticeshipFundedThroughAContractForServicesWithESFA)]
        [InlineData("ACT", Monitoring.Delivery.Types.ApprenticeshipContract)]
        public void MonitoringCodeMeetsExpectation(string expectation, string candidate)
        {
            // arrange / act / assert
            Assert.Equal(expectation, candidate);
        }

        /// <summary>
        /// Has qualifying monitor meets expectation
        /// </summary>
        /// <param name="famType">The Learning Delivery FAM Type.</param>
        /// <param name="famCode">The Learning Delivery FAM Code.</param>
        /// <param name="expectation">if set to <c>true</c> [expectation].</param>
        [Theory]
        [InlineData("ACT", "1", false)] // Monitoring.Delivery.ApprenticeshipFundedThroughAContractForServicesWithEmployer
        [InlineData("ACT", "2", false)] // Monitoring.Delivery.ApprenticeshipFundedThroughAContractForServicesWithESFA
        [InlineData("LDM", "034", false)] // Monitoring.Delivery.OLASSOffendersInCustody
        [InlineData("LSF", "1", true)] // Monitoring.Delivery.Types.LearningSupportFunding
        [InlineData("FFI", "2", false)] // Monitoring.Delivery.CoFundedLearningAim
        [InlineData("LDM", "363", false)] // Monitoring.Delivery.InReceiptOfLowWages
        [InlineData("LDM", "318", false)] // Monitoring.Delivery.MandationToSkillsTraining
        [InlineData("LDM", "328", false)] // Monitoring.Delivery.ReleasedOnTemporaryLicence
        [InlineData("LDM", "347", false)] // Monitoring.Delivery.SteelIndustriesRedundancyTraining
        [InlineData("SOF", "1", false)] // Monitoring.Delivery.HigherEducationFundingCouncilEngland
        [InlineData("SOF", "107", false)] // Monitoring.Delivery.ESFA16To19Funding
        [InlineData("SOF", "105", false)] // Monitoring.Delivery.ESFAAdultFunding
        [InlineData("SOF", "110", false)] // Monitoring.Delivery.GreaterManchesterCombinedAuthority
        [InlineData("SOF", "111", false)] // Monitoring.Delivery.LiverpoolCityRegionCombinedAuthority
        [InlineData("SOF", "112", false)] // Monitoring.Delivery.WestMidlandsCombinedAuthority
        [InlineData("SOF", "113", false)] // Monitoring.Delivery.WestOfEnglandCombinedAuthority
        [InlineData("SOF", "114", false)] // Monitoring.Delivery.TeesValleyCombinedAuthority
        [InlineData("SOF", "115", false)] // Monitoring.Delivery.CambridgeshireAndPeterboroughCombinedAuthority
        [InlineData("SOF", "116", false)] // Monitoring.Delivery.GreaterLondonAuthority
        public void IsLearningSupportFundingMeetsExpectation(string famType, string famCode, bool expectation)
        {
            // arrange
            var sut = NewRule();
            var fam = new Mock<ILearningDeliveryFAM>();
            fam
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(famType);
            fam
                .SetupGet(y => y.LearnDelFAMCode)
                .Returns(famCode);

            // act
            var result = sut.IsLearningSupportFunding(fam.Object);

            // assert
            Assert.Equal(expectation, result);
        }

        /// <summary>
        /// Invalid item raises validation message.
        /// </summary>
        [Fact]
        public void InvalidItemRaisesValidationMessage()
        {
            // arrange
            const string LearnRefNumber = "123456789X";
            const string LearnAimRef = "shonkyRef";

            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse("2017-04-01"));
            delivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(LearnAimRef);
            delivery
                .SetupGet(y => y.AimType)
                .Returns(3);
            delivery
                .SetupGet(y => y.FundModel)
                .Returns(36);

            var deliveries = new ILearningDelivery[] { delivery.Object };

            var learner = new Mock<ILearner>();
            learner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            learner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            handler
                .Setup(x => x.Handle(RuleNameConstants.LearnDelFAMType_67, LearnRefNumber, 0, It.IsAny<IEnumerable<IErrorMessageParameter>>()));
            handler
                .Setup(x => x.BuildErrorMessageParameter("AimType", 3))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("FundModel", 36))
                .Returns(new Mock<IErrorMessageParameter>().Object);
            handler
                .Setup(x => x.BuildErrorMessageParameter("LearnDelFAMType", "LSF"))
                .Returns(new Mock<IErrorMessageParameter>().Object);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(delivery.Object, 36))
                .Returns(true);
            commonOps
                .Setup(x => x.IsComponentOfAProgram(delivery.Object))
                .Returns(true);

            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            larsData
                .Setup(x => x.GetAnnualValuesFor(LearnAimRef))
                .Returns(new ILARSAnnualValue[] { });

            var larsDelivery = new Mock<ILARSLearningDelivery>();
            larsDelivery
                .SetupGet(x => x.FrameworkCommonComponent)
                .Returns(-2);

            larsData
                .Setup(x => x.GetDeliveryFor(LearnAimRef))
                .Returns(larsDelivery.Object);

            var sut = new LearnDelFAMType_67Rule(handler.Object, commonOps.Object, larsData.Object);

            // post construction set ups
            // this is final condition to decide to error or not
            commonOps
                .Setup(x => x.CheckDeliveryFAMs(delivery.Object, sut.IsLearningSupportFunding))
                .Returns(true);

            // act
            sut.Validate(learner.Object);

            // assert
            handler.VerifyAll();
            commonOps.VerifyAll();
            larsData.VerifyAll();
        }

        /// <summary>
        /// Valid item does not raise validation message.
        /// </summary>
        [Fact]
        public void ValidItemDoesNotRaiseValidationMessage()
        {
            // arrange
            const string LearnRefNumber = "123456789X";
            const string LearnAimRef = "shonkyRef";

            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(y => y.LearnStartDate)
                .Returns(DateTime.Parse("2017-04-01"));
            delivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(LearnAimRef);
            delivery
                .SetupGet(y => y.AimType)
                .Returns(3);
            delivery
                .SetupGet(y => y.FundModel)
                .Returns(36);

            var deliveries = new ILearningDelivery[] { delivery.Object };

            var learner = new Mock<ILearner>();
            learner
                .SetupGet(x => x.LearnRefNumber)
                .Returns(LearnRefNumber);
            learner
                .SetupGet(x => x.LearningDeliveries)
                .Returns(deliveries);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);

            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            commonOps
                .Setup(x => x.HasQualifyingFunding(delivery.Object, 36))
                .Returns(true);
            commonOps
                .Setup(x => x.IsComponentOfAProgram(delivery.Object))
                .Returns(true);

            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            larsData
                .Setup(x => x.GetAnnualValuesFor(LearnAimRef))
                .Returns(new ILARSAnnualValue[] { });

            var larsDelivery = new Mock<ILARSLearningDelivery>();
            larsDelivery
                .SetupGet(x => x.FrameworkCommonComponent)
                .Returns(-2);

            larsData
                .Setup(x => x.GetDeliveryFor(LearnAimRef))
                .Returns(larsDelivery.Object);

            var sut = new LearnDelFAMType_67Rule(handler.Object, commonOps.Object, larsData.Object);

            // post construction set ups
            // this is final condition to decide to error or not
            commonOps
                .Setup(x => x.CheckDeliveryFAMs(delivery.Object, sut.IsLearningSupportFunding))
                .Returns(false);

            // act
            sut.Validate(learner.Object);

            // assert
            handler.VerifyAll();
            commonOps.VerifyAll();
            larsData.VerifyAll();
        }

        /// <summary>
        /// New rule.
        /// </summary>
        /// <returns>a constructed and mocked up validation rule.</returns>

        public LearnDelFAMType_67Rule NewRule()
        {
            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var commonOps = new Mock<IProvideRuleCommonOperations>(MockBehavior.Strict);
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);

            return new LearnDelFAMType_67Rule(handler.Object, commonOps.Object, larsData.Object);
        }
    }
}
