using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.HE.LearningDeliveryHE;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.LearningDeliveryHE
{
    public class LearningDeliveryHE_08RuleTests : AbstractRuleTests<LearningDeliveryHE_08Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearningDeliveryHE_08");
        }

        [Fact]
        public void DerivedData27ConditionMet_False()
        {
            int ukprn = 123456;
            var derivedData27Mock = new Mock<IDerivedData_27Rule>();

            derivedData27Mock.Setup(d => d.IsUKPRNCollegeOrGrantFundedProvider(ukprn)).Returns(false);

            NewRule(derivedData_27Rule: derivedData27Mock.Object).DerivedData27ConditionMet(ukprn).Should().BeFalse();
        }

        [Fact]
        public void DerivedData27ConditionMet_True()
        {
            int ukprn = 987654;
            var derivedData27Mock = new Mock<IDerivedData_27Rule>();

            derivedData27Mock.Setup(d => d.IsUKPRNCollegeOrGrantFundedProvider(ukprn)).Returns(true);

            NewRule(derivedData_27Rule: derivedData27Mock.Object).DerivedData27ConditionMet(ukprn).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryHEConditionMet_False()
        {
            var testLearningDeliveryHE = new TestLearningDeliveryHE()
            {
                SSN = "123456789",
                DOMICILE = "DC987654"
            };
            NewRule().LearningDeliveryHEConditionMet(testLearningDeliveryHE).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryHEConditionMet_True()
        {
            NewRule().LearningDeliveryHEConditionMet(null).Should().BeTrue();
        }

        [Fact]
        public void LARSConditionMet_False()
        {
            string learnAimRef = "ZSF12346";
            HashSet<int?> englishPrescribedIDs = new HashSet<int?>() { 1, 2 };

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(d => d.EnglishPrescribedIdsExistsforLearnAimRef(learnAimRef, englishPrescribedIDs)).Returns(false);

            NewRule(lARSDataService: larsDataServiceMock.Object).LARSConditionMet(learnAimRef).Should().BeFalse();
        }

        [Fact]
        public void LARSConditionMet_True()
        {
            string learnAimRef = "ZSF12346";
            HashSet<int?> englishPrescribedIDs = new HashSet<int?>() { 1, 2 };

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(d => d.EnglishPrescribedIdsExistsforLearnAimRef(learnAimRef, englishPrescribedIDs)).Returns(true);

            NewRule(lARSDataService: larsDataServiceMock.Object).LARSConditionMet(learnAimRef).Should().BeTrue();
        }

        [Fact]
        public void FAMSConditionMet_False()
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                    LearnDelFAMCode = "352"
                }
            };

            var famsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            famsQueryServiceMock.Setup(f => f.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, "352")).Returns(true);

            NewRule(learningDeliveryFAMQueryService: famsQueryServiceMock.Object).FAMSConditionMet(testLearningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void FAMSConditionMet_True()
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                    LearnDelFAMCode = "17"
                }
            };

            var famsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            famsQueryServiceMock.Setup(f => f.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, "352")).Returns(false);

            NewRule(learningDeliveryFAMQueryService: famsQueryServiceMock.Object).FAMSConditionMet(testLearningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            string learnAimRef = "AB123456";
            HashSet<int?> englishPrescribedIDs = new HashSet<int?>() { 1, 2 };

            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                    LearnDelFAMCode = "352"
                }
            };

            var testLearningDeliveryHE = new TestLearningDeliveryHE()
            {
                SSN = "123456789",
                DOMICILE = "DC987654"
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var famsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            larsDataServiceMock.Setup(d => d.EnglishPrescribedIdsExistsforLearnAimRef(learnAimRef, englishPrescribedIDs)).Returns(false);
            famsQueryServiceMock.Setup(f => f.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, "352")).Returns(true);

            NewRule(
                lARSDataService: larsDataServiceMock.Object,
                learningDeliveryFAMQueryService: famsQueryServiceMock.Object)
                .ConditionMet(learnAimRef, testLearningDeliveryHE, testLearningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            string learnAimRef = "ZSF987654";
            HashSet<int?> englishPrescribedIDs = new HashSet<int?>() { 1, 2 };

            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                    LearnDelFAMCode = "17"
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var famsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            larsDataServiceMock.Setup(d => d.EnglishPrescribedIdsExistsforLearnAimRef(learnAimRef, englishPrescribedIDs)).Returns(true);
            famsQueryServiceMock.Setup(f => f.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, "352")).Returns(false);

            NewRule(
                lARSDataService: larsDataServiceMock.Object,
                learningDeliveryFAMQueryService: famsQueryServiceMock.Object)
                .ConditionMet(learnAimRef, null, testLearningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            string learnAimRef = "ZSF987654";
            HashSet<int?> englishPrescribedIDs = new HashSet<int?>() { 1, 2 };
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                    LearnDelFAMCode = "17"
                }
            };

            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        ProgTypeNullable = TypeOfLearningProgramme.HigherApprenticeshipLevel4,
                        LearningDeliveryFAMs = testLearningDeliveryFAMs,
                        LearningDeliveryHEEntity = null
                    }
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var famsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var derivedData27Mock = new Mock<IDerivedData_27Rule>();
            var organisationDataServiceMock = new Mock<IOrganisationDataService>();
            var fileServiceMock = new Mock<IFileDataService>();

            larsDataServiceMock.Setup(d => d.EnglishPrescribedIdsExistsforLearnAimRef(learnAimRef, englishPrescribedIDs)).Returns(true);
            famsQueryServiceMock.Setup(f => f.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, "352")).Returns(false);
            derivedData27Mock.Setup(d => d.IsUKPRNCollegeOrGrantFundedProvider(123456)).Returns(false);
            organisationDataServiceMock.Setup(o => o.LegalOrgTypeMatchForUkprn(123456, LegalOrgTypeConstants.UHEO)).Returns(false);
            fileServiceMock.Setup(f => f.UKPRN()).Returns(123456);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    fileDataService: fileServiceMock.Object,
                    lARSDataService: larsDataServiceMock.Object,
                    learningDeliveryFAMQueryService: famsQueryServiceMock.Object,
                    organisationDataService: organisationDataServiceMock.Object,
                    derivedData_27Rule: derivedData27Mock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            string learnAimRef = "ZSF123456";
            HashSet<int?> englishPrescribedIDs = new HashSet<int?>() { 1, 2 };
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                    LearnDelFAMCode = "352"
                }
            };

            var testLearningDeliveryHE = new TestLearningDeliveryHE()
            {
                SSN = "123456789",
                DOMICILE = "DC987654"
            };

            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = learnAimRef,
                        ProgTypeNullable = TypeOfLearningProgramme.HigherApprenticeshipLevel4,
                        LearningDeliveryFAMs = testLearningDeliveryFAMs,
                        LearningDeliveryHEEntity = testLearningDeliveryHE
                    }
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            var famsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var derivedData27Mock = new Mock<IDerivedData_27Rule>();
            var organisationDataServiceMock = new Mock<IOrganisationDataService>();
            var fileServiceMock = new Mock<IFileDataService>();

            larsDataServiceMock.Setup(d => d.EnglishPrescribedIdsExistsforLearnAimRef(learnAimRef, englishPrescribedIDs)).Returns(false);
            famsQueryServiceMock.Setup(f => f.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, "352")).Returns(true);
            derivedData27Mock.Setup(d => d.IsUKPRNCollegeOrGrantFundedProvider(654321)).Returns(false);
            organisationDataServiceMock.Setup(o => o.LegalOrgTypeMatchForUkprn(654321, LegalOrgTypeConstants.UHEO)).Returns(false);
            fileServiceMock.Setup(f => f.UKPRN()).Returns(654321);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    fileDataService: fileServiceMock.Object,
                    lARSDataService: larsDataServiceMock.Object,
                    learningDeliveryFAMQueryService: famsQueryServiceMock.Object,
                    organisationDataService: organisationDataServiceMock.Object,
                    derivedData_27Rule: derivedData27Mock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NullCheck()
        {
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(null);
            }
        }

        [Fact]
        public void Validate_LearningDelivery_NullCheck()
        {
            var testLearner = new TestLearner()
            {
                LearningDeliveries = null
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(null);
            }
        }

        public LearningDeliveryHE_08Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            IFileDataService fileDataService = null,
            ILARSDataService lARSDataService = null,
            IDerivedData_27Rule derivedData_27Rule = null,
            IOrganisationDataService organisationDataService = null,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null)
        {
            return new LearningDeliveryHE_08Rule(
                validationErrorHandler: validationErrorHandler,
                fileDataService: fileDataService,
                lARSDataService: lARSDataService,
                derivedData_27Rule: derivedData_27Rule,
                organisationDataService: organisationDataService,
                learningDeliveryFAMQueryService: learningDeliveryFAMQueryService);
        }
    }
}
