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
    public class LearningDeliveryHE_03RuleTests : AbstractRuleTests<LearningDeliveryHE_03Rule>
    {
        private readonly string[] _notionalNVQLevels =
            {
                LARSConstants.NotionalNVQLevelV2Strings.Level4,
                LARSConstants.NotionalNVQLevelV2Strings.Level5,
                LARSConstants.NotionalNVQLevelV2Strings.Level6,
                LARSConstants.NotionalNVQLevelV2Strings.Level7,
                LARSConstants.NotionalNVQLevelV2Strings.Level8,
                LARSConstants.NotionalNVQLevelV2Strings.HigherLevel
            };

        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearningDeliveryHE_03");
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(FundModels.CommunityLearning).Should().BeFalse();
        }

        [Theory]
        [InlineData(FundModels.Age16To19ExcludingApprenticeships)]
        [InlineData(FundModels.AdultSkills)]
        [InlineData(FundModels.NotFundedByESFA)]
        public void FundModelConditionMet_True(int fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeTrue();
        }

        [Fact]
        public void DerivedData27ConditionMet_False()
        {
            int uKPRN = 654123;

            var derivedData27RuleMock = new Mock<IDerivedData_27Rule>();

            derivedData27RuleMock.Setup(d => d.IsUKPRNCollegeOrGrantFundedProvider(uKPRN)).Returns(false);

            NewRule(derivedData_27Rule: derivedData27RuleMock.Object).DerivedData27ConditionMet(uKPRN).Should().BeFalse();
        }

        [Fact]
        public void DerivedData27ConditionMet_True()
        {
            int uKPRN = 963258;

            var derivedData27RuleMock = new Mock<IDerivedData_27Rule>();

            derivedData27RuleMock.Setup(d => d.IsUKPRNCollegeOrGrantFundedProvider(uKPRN)).Returns(true);

            NewRule(derivedData_27Rule: derivedData27RuleMock.Object).DerivedData27ConditionMet(uKPRN).Should().BeTrue();
        }

        [Fact]
        public void LARSNotionalNVQLevelV2ConditionMet_False()
        {
            string learnAimRef = "50022246";

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(l => l.NotionalNVQLevelV2MatchForLearnAimRefAndLevels(learnAimRef, _notionalNVQLevels)).Returns(false);

            NewRule(lARSDataService: larsDataServiceMock.Object).LARSNotionalNVQLevelV2ConditionMet(learnAimRef).Should().BeFalse();
        }

        [Fact]
        public void LARSNotionalNVQLevelV2ConditionMet_True()
        {
            string learnAimRef = "50023408";

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(l => l.NotionalNVQLevelV2MatchForLearnAimRefAndLevels(learnAimRef, _notionalNVQLevels)).Returns(true);

            NewRule(lARSDataService: larsDataServiceMock.Object).LARSNotionalNVQLevelV2ConditionMet(learnAimRef).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryHEConditionMet_False()
        {
            NewRule().LearningDeliveryHEConditionMet(new TestLearningDeliveryHE() { SSN = "TEST1234" }).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryHEConditionMet_True()
        {
            NewRule().LearningDeliveryHEConditionMet(null).Should().BeTrue();
        }

        [Theory]
        [InlineData(ProgTypes.AdvancedLevelApprenticeship)]
        [InlineData(ProgTypes.IntermediateLevelApprenticeship)]
        [InlineData(ProgTypes.HigherApprenticeshipLevel4)]
        [InlineData(ProgTypes.HigherApprenticeshipLevel5)]
        [InlineData(ProgTypes.HigherApprenticeshipLevel6)]
        [InlineData(ProgTypes.HigherApprenticeshipLevel7Plus)]
        [InlineData(ProgTypes.ApprenticeshipStandard)]
        public void DD07ConditionMet_False(int? progType)
        {
            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(d => d.IsApprenticeship(progType)).Returns(true);

            NewRule(dD07: dd07Mock.Object).DD07ConditionMet(progType).Should().BeFalse();
        }

        [Theory]
        [InlineData(ProgTypes.Traineeship)]
        [InlineData(null)]
        public void DD07ConditionMet_True(int? progType)
        {
            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(d => d.IsApprenticeship(ProgTypes.AdvancedLevelApprenticeship)).Returns(false);

            NewRule(dD07: dd07Mock.Object).DD07ConditionMet(progType).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryFAMsCondtionMet_False()
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.FFI,
                    LearnDelFAMCode = "22"
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock.Setup(s => s.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, "352")).Returns(true);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).LearningDeliveryFAMsCondtionMet(testLearningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMsCondtionMet_True()
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                    LearnDelFAMCode = "352"
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock.Setup(s => s.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, "352")).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).LearningDeliveryFAMsCondtionMet(testLearningDeliveryFAMs).Should().BeTrue();
        }

        [Theory]
        [InlineData(FundModels.CommunityLearning, "", ProgTypes.ApprenticeshipStandard)]
        [InlineData(FundModels.Age16To19ExcludingApprenticeships, "50023111", ProgTypes.Traineeship)]
        [InlineData(FundModels.CommunityLearning, "50023408", ProgTypes.Traineeship)]
        public void ConditionMet_False(int fundModel, string learnAimRef, int? progType)
        {
            var testLearningDeliveryHeEntity = new TestLearningDeliveryHE()
            {
                SSN = "12345678"
            };

            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = LearningDeliveryFAMTypeConstants.FFI,
                        LearnDelFAMCode = "22"
                    }
                };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var larsDataServiceMock = new Mock<ILARSDataService>();
            var dd07Mock = new Mock<IDerivedData_07Rule>();

            learningDeliveryFAMsQueryServiceMock.Setup(s => s.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, "352")).Returns(true);
            larsDataServiceMock.Setup(l => l.NotionalNVQLevelV2MatchForLearnAimRefAndLevels(learnAimRef, _notionalNVQLevels)).Returns(false);
            dd07Mock.Setup(d => d.IsApprenticeship(progType)).Returns(true);

            NewRule(
                dD07: dd07Mock.Object,
                lARSDataService: larsDataServiceMock.Object,
                learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object)
                .ConditionMet(fundModel, learnAimRef, progType, testLearningDeliveryHeEntity, testLearningDeliveryFAMs).Should().BeFalse();
        }

        [Theory]
        [InlineData(FundModels.Age16To19ExcludingApprenticeships, "50023408", ProgTypes.AdvancedLevelApprenticeship)]
        [InlineData(FundModels.AdultSkills, "50023408", ProgTypes.AdvancedLevelApprenticeship)]
        [InlineData(FundModels.NotFundedByESFA, "50023408", ProgTypes.AdvancedLevelApprenticeship)]
        public void ConditionMet_True(int fundModel, string learnAimRef, int? progType)
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                    LearnDelFAMCode = "352"
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var larsDataServiceMock = new Mock<ILARSDataService>();
            var dd07Mock = new Mock<IDerivedData_07Rule>();

            learningDeliveryFAMsQueryServiceMock.Setup(s => s.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, "352")).Returns(false);
            larsDataServiceMock.Setup(l => l.NotionalNVQLevelV2MatchForLearnAimRefAndLevels(learnAimRef, _notionalNVQLevels)).Returns(true);
            dd07Mock.Setup(d => d.IsApprenticeship(ProgTypes.AdvancedLevelApprenticeship)).Returns(false);

            NewRule(
                dD07: dd07Mock.Object,
                lARSDataService: larsDataServiceMock.Object,
                learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object)
                .ConditionMet(fundModel, learnAimRef, progType, null, testLearningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                    LearnDelFAMCode = "352"
                }
            };

            var testLearner = new TestLearner
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = FundModels.AdultSkills,
                        ProgTypeNullable = ProgTypes.AdvancedLevelApprenticeship,
                        LearnAimRef = "50023408",
                        LearningDeliveryFAMs = testLearningDeliveryFAMs
                    }
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var organisationDataServiceMock = new Mock<IOrganisationDataService>();
            var derivedDataServiceMock = new Mock<IDerivedData_27Rule>();
            var larsDataServiceMock = new Mock<ILARSDataService>();
            var fileDataServiceMock = new Mock<IFileDataService>();
            var dd07Mock = new Mock<IDerivedData_07Rule>();

            fileDataServiceMock.Setup(f => f.UKPRN()).Returns(98756789);
            learningDeliveryFAMsQueryServiceMock.Setup(s => s.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, "352")).Returns(false);
            organisationDataServiceMock.Setup(o => o.LegalOrgTypeMatchForUkprn(123654321, LegalOrgTypeConstants.ULEA)).Returns(true);
            derivedDataServiceMock.Setup(d => d.IsUKPRNCollegeOrGrantFundedProvider(98756789)).Returns(true);
            larsDataServiceMock.Setup(l => l.NotionalNVQLevelV2MatchForLearnAimRefAndLevels("50023408", _notionalNVQLevels)).Returns(true);
            dd07Mock.Setup(d => d.IsApprenticeship(ProgTypes.AdvancedLevelApprenticeship)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object,
                    dD07: dd07Mock.Object,
                    derivedData_27Rule: derivedDataServiceMock.Object,
                    lARSDataService: larsDataServiceMock.Object,
                    fileDataService: fileDataServiceMock.Object,
                    organisationDataService: organisationDataServiceMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var testLearningDeliveryHeEntity = new TestLearningDeliveryHE()
            {
                SSN = "12345678"
            };

            var testLearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = LearningDeliveryFAMTypeConstants.FFI,
                        LearnDelFAMCode = "22"
                    }
                };

            var testLearner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = FundModels.CommunityLearning,
                        ProgTypeNullable = ProgTypes.ApprenticeshipStandard,
                        LearnAimRef = "50023408",
                        LearningDeliveryFAMs = testLearningDeliveryFAMs,
                        LearningDeliveryHEEntity = testLearningDeliveryHeEntity
                    }
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            var organisationDataServiceMock = new Mock<IOrganisationDataService>();
            var derivedDataServiceMock = new Mock<IDerivedData_27Rule>();
            var larsDataServiceMock = new Mock<ILARSDataService>();
            var fileDataServiceMock = new Mock<IFileDataService>();
            var dd07Mock = new Mock<IDerivedData_07Rule>();

            fileDataServiceMock.Setup(f => f.UKPRN()).Returns(123654321);
            learningDeliveryFAMsQueryServiceMock.Setup(s => s.HasLearningDeliveryFAMCodeForType(testLearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, "352")).Returns(true);
            larsDataServiceMock.Setup(l => l.NotionalNVQLevelV2MatchForLearnAimRefAndLevels("50023411", _notionalNVQLevels)).Returns(false);
            organisationDataServiceMock.Setup(o => o.LegalOrgTypeMatchForUkprn(123654321, LegalOrgTypeConstants.ULEA)).Returns(true);
            derivedDataServiceMock.Setup(d => d.IsUKPRNCollegeOrGrantFundedProvider(98756789)).Returns(true);
            dd07Mock.Setup(d => d.IsApprenticeship(ProgTypes.ApprenticeshipStandard)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    validationErrorHandler: validationErrorHandlerMock.Object,
                    learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object,
                    dD07: dd07Mock.Object,
                    derivedData_27Rule: derivedDataServiceMock.Object,
                    lARSDataService: larsDataServiceMock.Object,
                    fileDataService: fileDataServiceMock.Object,
                    organisationDataService: organisationDataServiceMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError_Learner_NullCheck()
        {
            TestLearner testLearner = null;
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError_LearningDelivery_NullCheck()
        {
            TestLearner testLearner = new TestLearner()
            {
                LearningDeliveries = null
            };
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.FundModel, FundModels.NotFundedByESFA)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(FundModels.NotFundedByESFA);

            validationErrorHandlerMock.Verify();
        }

        private LearningDeliveryHE_03Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null,
            IOrganisationDataService organisationDataService = null,
            IDerivedData_27Rule derivedData_27Rule = null,
            ILARSDataService lARSDataService = null,
            IFileDataService fileDataService = null,
            IDerivedData_07Rule dD07 = null)
        {
            return new LearningDeliveryHE_03Rule(
                validationErrorHandler: validationErrorHandler,
                learningDeliveryFAMQueryService: learningDeliveryFAMQueryService,
                organisationDataService: organisationDataService,
                derivedData_27Rule: derivedData_27Rule,
                lARSDataService: lARSDataService,
                fileDataService: fileDataService,
                dd07: dD07);
        }
    }
}
