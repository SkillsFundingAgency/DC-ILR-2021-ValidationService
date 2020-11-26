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
        [Fact]
        public void RuleName()
        {
            var sut = NewRule();

            var result = sut.RuleName;

            Assert.Equal("LearnDelFAMType_67", result);
        }

        public DateTime? GetNullableDate(string candidate) =>
            string.IsNullOrWhiteSpace(candidate) ? (DateTime?)null : DateTime.Parse(candidate);

        [Theory]
        [InlineData(36, FundModels.ApprenticeshipsFrom1May2017)]
        public void TypeOfFundingMeetsExpectation(int expectation, int candidate)
        {
            Assert.Equal(expectation, candidate);
        }

        [Theory]
        [InlineData(36, true)]
        [InlineData(35, false)]
        public void HasQualifyingModelMeetsExpectation(int fundModel, bool expectation)
        {
            var delivery = new Mock<ILearningDelivery>();
            delivery.Setup(x => x.FundModel).Returns(fundModel);

            var result = NewRule().HasQualifyingModel(delivery.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(1, false)]
        [InlineData(3, true)]
        public void IsComponentAimMeetsExpectation(int aimType, bool expectation)
        {
            var delivery = new Mock<ILearningDelivery>();
            delivery
             .SetupGet(x => x.AimType)
             .Returns(aimType);

            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);
            var result = NewRule(learningDeliveryFAMQS: learningDeliveryFAMQS.Object).IsComponentAim(delivery.Object);

            Assert.Equal(expectation, result);
            learningDeliveryFAMQS.VerifyAll();
        }

        [Fact]
        public void HasQualifyingBasicSkillsTypeWithNoAnnualValuesReturnsFalse()
        {
            var learnAimRef = "shonkyref";
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.LearnAimRef)
                .Returns(learnAimRef);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);

            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            larsData
                .Setup(x => x.GetAnnualValuesFor(learnAimRef))
                .Returns(new ILARSAnnualValue[] { });

            var sut = NewRule(handler.Object, learningDeliveryFAMQS.Object, larsData.Object);

            var result = sut.HasQualifyingBasicSkillsType(delivery.Object);

            Assert.False(result);

            handler.VerifyAll();
            learningDeliveryFAMQS.VerifyAll();
            larsData.VerifyAll();
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData(1, true)]
        [InlineData(32, true)]
        public void HasABasicSkillTypeMeetsExpectation(int? basicSkill, bool expectation)
        {
            var annualValue = new Mock<ILARSAnnualValue>();
            annualValue
                .SetupGet(x => x.BasicSkillsType)
                .Returns(basicSkill);

            var sut = NewRule();

            var result = sut.HasABasicSkillType(annualValue.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData(1, LARSConstants.BasicSkills.Certificate_AdultLiteracy)]
        [InlineData(2, LARSConstants.BasicSkills.Certificate_AdultNumeracy)]
        [InlineData(11, LARSConstants.BasicSkills.GCSE_EnglishLanguage)]
        [InlineData(12, LARSConstants.BasicSkills.GCSE_Mathematics)]
        [InlineData(13, LARSConstants.BasicSkills.KeySkill_Communication)]
        [InlineData(14, LARSConstants.BasicSkills.KeySkill_ApplicationOfNumbers)]
        [InlineData(19, LARSConstants.BasicSkills.FunctionalSkillsMathematics)]
        [InlineData(20, LARSConstants.BasicSkills.FunctionalSkillsEnglish)]
        [InlineData(21, LARSConstants.BasicSkills.UnitsOfTheCertificate_AdultNumeracy)]
        [InlineData(23, LARSConstants.BasicSkills.UnitsOfTheCertificate_AdultLiteracy)]
        [InlineData(24, LARSConstants.BasicSkills.NonNQF_QCFS4LLiteracy)]
        [InlineData(25, LARSConstants.BasicSkills.NonNQF_QCFS4LNumeracy)]
        [InlineData(29, LARSConstants.BasicSkills.QCFBasicSkillsEnglishLanguage)]
        [InlineData(30, LARSConstants.BasicSkills.QCFBasicSkillsMathematics)]
        [InlineData(31, LARSConstants.BasicSkills.UnitQCFBasicSkillsEnglishLanguage)]
        [InlineData(32, LARSConstants.BasicSkills.UnitQCFBasicSkillsMathematics)]
        [InlineData(33, LARSConstants.BasicSkills.InternationalGCSEEnglishLanguage)]
        [InlineData(34, LARSConstants.BasicSkills.InternationalGCSEMathematics)]
        [InlineData(35, LARSConstants.BasicSkills.FreeStandingMathematicsQualification)]
        public void TypeOfLARSBasicSkillMeetsExpectation(int candidate, int expectation)
        {
            Assert.Equal(expectation, candidate);
        }

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
            Assert.Contains(basicSkill, LARSConstants.BasicSkills.EnglishAndMathsList);
        }

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
            var annualValue = new Mock<ILARSAnnualValue>();
            annualValue
                .SetupGet(x => x.BasicSkillsType)
                .Returns(basicSkill);

            var sut = NewRule();

            var result = sut.IsEnglishOrMathBasicSkill(annualValue.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData("2018-04-01", "2018-04-01", true)]
        [InlineData("2018-04-01", "2018-04-02", false)]
        public void IsValueCurrentMeetsExpectation(string learnStart, string valueStart, bool expectation)
        {
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.LearnStartDate)
                .Returns(DateTime.Parse(learnStart));

            var annualValue = new Mock<ILARSAnnualValue>();
            annualValue
                .SetupGet(x => x.StartDate)
                .Returns(DateTime.Parse(valueStart));

            var larsService = new Mock<ILARSDataService>(MockBehavior.Strict);
            larsService
                .Setup(x => x.IsCurrentAndNotWithdrawn(annualValue.Object, delivery.Object.LearnStartDate, null))
                .Returns(expectation);

            var sut = NewRule(larsData: larsService.Object);

            var result = sut.IsValueCurrent(delivery.Object, annualValue.Object);

            Assert.Equal(expectation, result);
        }

        [Fact]
        public void GetLarsAimMeetsExpectation()
        {
            var learnAimRef = "shonkyref";
            var delivery = new Mock<ILearningDelivery>();
            delivery
                .SetupGet(x => x.LearnAimRef)
                .Returns(learnAimRef);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);

            var larsAim = new Mock<ILARSLearningDelivery>();

            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);
            larsData
                .Setup(x => x.GetDeliveryFor(learnAimRef))
                .Returns(larsAim.Object);

            var sut = NewRule(handler.Object, learningDeliveryFAMQS.Object, larsData.Object);

            var result = sut.GetLarsAim(delivery.Object);

            Assert.Equal(larsAim.Object, result);

            handler.VerifyAll();
            learningDeliveryFAMQS.VerifyAll();
            larsData.VerifyAll();
        }

        [Fact]
        public void HasQualifyingCommonComponentWithNullLarsDeliveryReturnsFalse()
        {
            var sut = NewRule();

            var result = sut.HasQualifyingCommonComponent(null);

            Assert.False(result);
        }

        [Fact]
        public void HasQualifyingCommonComponentWithNullFrameworksReturnsFalse()
        {
            var delivery = new Mock<ILARSLearningDelivery>();

            var sut = NewRule();

            var result = sut.HasQualifyingCommonComponent(delivery.Object);

            Assert.False(result);
        }

        [Theory]
        [InlineData(20, LARSConstants.CommonComponents.BritishSignLanguage)]
        public void TypeOfLARSCommonComponentMeetsExpectation(int candidate, int expectation)
        {
            Assert.Equal(expectation, candidate);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData(19, false)]
        [InlineData(20, true)]
        [InlineData(21, false)]
        public void IsBritishSignLanguageMeetsExpectation(int? component, bool expectation)
        {
            var larsDelivery = new Mock<ILARSLearningDelivery>();
            larsDelivery
                .SetupGet(x => x.FrameworkCommonComponent)
                .Returns(component);

            var sut = NewRule();

            var result = sut.IsBritishSignLanguage(larsDelivery.Object);

            Assert.Equal(expectation, result);
        }

        [Theory]
        [InlineData("ACT1", Monitoring.Delivery.ApprenticeshipFundedThroughAContractForServicesWithEmployer)]
        [InlineData("ACT2", Monitoring.Delivery.ApprenticeshipFundedThroughAContractForServicesWithESFA)]
        [InlineData("ACT", Monitoring.Delivery.Types.ApprenticeshipContract)]
        public void MonitoringCodeMeetsExpectation(string expectation, string candidate)
        {
            Assert.Equal(expectation, candidate);
        }

        [Theory]
        [InlineData("ACT", "1", false)]
        [InlineData("ACT", "2", false)]
        [InlineData("LDM", "034", false)]
        [InlineData("LSF", "1", true)]
        [InlineData("FFI", "2", false)]
        [InlineData("LDM", "363", false)]
        [InlineData("LDM", "318", false)]
        [InlineData("LDM", "328", false)]
        [InlineData("LDM", "347", false)]
        [InlineData("SOF", "1", false)]
        [InlineData("SOF", "107", false)]
        [InlineData("SOF", "105", false)]
        [InlineData("SOF", "110", false)]
        [InlineData("SOF", "111", false)]
        [InlineData("SOF", "112", false)]
        [InlineData("SOF", "113", false)]
        [InlineData("SOF", "114", false)]
        [InlineData("SOF", "115", false)]
        [InlineData("SOF", "116", false)]
        public void IsLearningSupportFundingMeetsExpectation(string famType, string famCode, bool expectation)
        {
            var fam = new Mock<ILearningDeliveryFAM>();
            fam
                .SetupGet(y => y.LearnDelFAMType)
                .Returns(famType);
            fam
                .SetupGet(y => y.LearnDelFAMCode)
                .Returns(famCode);

            var fams = new List<ILearningDeliveryFAM>
            {
                fam.Object
            };

            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearningDeliveryFAMs)
                .Returns(fams);

            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);
            learningDeliveryFAMQS
               .Setup(x => x.HasLearningDeliveryFAMType(
                   mockDelivery.Object.LearningDeliveryFAMs,
                   "LSF"))
               .Returns(expectation);

            var handler = new Mock<IValidationErrorHandler>(MockBehavior.Strict);
            var larsData = new Mock<ILARSDataService>(MockBehavior.Strict);

            var sut = new LearnDelFAMType_67Rule(handler.Object, learningDeliveryFAMQS.Object, larsData.Object);
            var result = sut.IsLearningSupportFunding(mockDelivery.Object);

            Assert.Equal(expectation, result);
        }

        [Fact]
        public void InvalidItemRaisesValidationMessage()
        {
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

            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);
            learningDeliveryFAMQS
               .Setup(x => x.HasLearningDeliveryFAMType(
                   delivery.Object.LearningDeliveryFAMs,
                   "LSF"))
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

            var sut = NewRule(handler.Object, learningDeliveryFAMQS.Object, larsData.Object);

            sut.Validate(learner.Object);

            handler.VerifyAll();
            learningDeliveryFAMQS.VerifyAll();
            larsData.VerifyAll();
        }

        [Fact]
        public void ValidItemDoesNotRaiseValidationMessage()
        {
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

            var learningDeliveryFAMQS = new Mock<ILearningDeliveryFAMQueryService>(MockBehavior.Strict);
            learningDeliveryFAMQS
               .Setup(x => x.HasLearningDeliveryFAMType(
                   delivery.Object.LearningDeliveryFAMs,
                   "LSF"))
               .Returns(false);

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

            var sut = NewRule(handler.Object, learningDeliveryFAMQS.Object, larsData.Object);

            sut.Validate(learner.Object);

            handler.VerifyAll();
            learningDeliveryFAMQS.VerifyAll();
            larsData.VerifyAll();
        }

        public LearnDelFAMType_67Rule NewRule(
         IValidationErrorHandler handler = null,
         ILearningDeliveryFAMQueryService learningDeliveryFAMQS = null,
         ILARSDataService larsData = null)
        {
            return new LearnDelFAMType_67Rule(handler, learningDeliveryFAMQS, larsData);
        }
    }
}
