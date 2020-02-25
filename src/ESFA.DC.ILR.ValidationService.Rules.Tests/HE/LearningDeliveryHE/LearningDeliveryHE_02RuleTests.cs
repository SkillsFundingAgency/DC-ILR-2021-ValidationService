using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.HE.LearningDeliveryHE;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.LearningDeliveryHE
{
    public class LearningDeliveryHE_02RuleTests : AbstractRuleTests<LearningDeliveryHE_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearningDeliveryHE_02");
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            var fundModel = 99;

            NewRule().FundModelConditionMet(fundModel).Should().BeTrue();
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            var fundModel = 0;

            NewRule().FundModelConditionMet(fundModel).Should().BeFalse();
        }

        [Fact]
        public void DeliveryFAMConditionMet_True()
        {
            var famType = "SOF";
            var famCode = "1";

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = famType,
                    LearnDelFAMCode = famCode
                },
                new TestLearningDeliveryFAM()
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, famType, famCode))
                .Returns(true);

            NewRule(learningDeliveryFamQueryServiceMock.Object).DeliveryFAMConditionMet(learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void DeliveryFAMConditionMet_False()
        {
            var famType = "LDM";
            var famCode = "352";

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = famType,
                    LearnDelFAMCode = famCode
                },
                new TestLearningDeliveryFAM()
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, famType, famCode))
                .Returns(true);

            NewRule(learningDeliveryFamQueryServiceMock.Object).DeliveryFAMConditionMet(learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void DeliveryHEConditionMet_True()
        {
            TestLearningDeliveryHE learningDeliveryHe = null;

            NewRule().DeliveryHEConditionMet(learningDeliveryHe).Should().BeTrue();
        }

        [Fact]
        public void DeliveryHEConditionMet_False()
        {
            var learningDeliveryHe = new TestLearningDeliveryHE();

            NewRule().DeliveryHEConditionMet(learningDeliveryHe).Should().BeFalse();
        }

        [Fact]
        public void Excluded_True()
        {
            var ukprn = 12345678;
            var orgType = "UHEO";

            var organisationDataServiceMock = new Mock<IOrganisationDataService>();
            organisationDataServiceMock
                .Setup(od => od.LegalOrgTypeMatchForUkprn(ukprn, orgType))
                .Returns(true);

            NewRule(organisationDataService: organisationDataServiceMock.Object)
                .Excluded(ukprn)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void Excluded_False()
        {
            var ukprn = 12345678;
            var orgType = "UHEO";

            var organisationDataServiceMock = new Mock<IOrganisationDataService>();
            organisationDataServiceMock
                .Setup(od => od.LegalOrgTypeMatchForUkprn(ukprn, orgType))
                .Returns(false);

            NewRule(organisationDataService: organisationDataServiceMock.Object)
                .Excluded(ukprn)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var fundModel = 99;
            var ukprn = 12345678;
            var famType = "SOF";
            var famCode = "1";
            var orgType = "XXX";

            var learningDeliveryFams = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = famType,
                    LearnDelFAMCode = famCode
                }
            };

            TestLearningDeliveryHE learningDeliveryHe = null;

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, famType, famCode))
                .Returns(true);

            var organisationDataServiceMock = new Mock<IOrganisationDataService>();
            organisationDataServiceMock
                .Setup(od => od.LegalOrgTypeMatchForUkprn(ukprn, orgType))
                .Returns(false);

            NewRule(learningDeliveryFamQueryServiceMock.Object, organisationDataService: organisationDataServiceMock.Object).ConditionMet(fundModel, learningDeliveryFams, learningDeliveryHe, ukprn).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_FalseFundModel()
        {
            var fundModel = 1;
            var ukprn = 12345678;
            var famType = "SOF";
            var famCode = "1";
            var orgType = "XXX";

            var learningDeliveryFams = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = famType,
                    LearnDelFAMCode = famCode
                }
            };

            TestLearningDeliveryHE learningDeliveryHe = null;

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, famType, famCode))
                .Returns(true);

            var organisationDataServiceMock = new Mock<IOrganisationDataService>();
            organisationDataServiceMock
                .Setup(od => od.LegalOrgTypeMatchForUkprn(ukprn, orgType))
                .Returns(false);

            NewRule(learningDeliveryFamQueryServiceMock.Object, organisationDataService: organisationDataServiceMock.Object).ConditionMet(fundModel, learningDeliveryFams, learningDeliveryHe, ukprn).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseDeliveryFam()
        {
            var fundModel = 99;
            var ukprn = 12345678;
            var famType = "XXX";
            var famCode = "1";
            var orgType = "XXX";

            var learningDeliveryFams = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = famType,
                    LearnDelFAMCode = famCode
                }
            };

            TestLearningDeliveryHE learningDeliveryHe = null;

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, famType, famCode))
                .Returns(false);

            var organisationDataServiceMock = new Mock<IOrganisationDataService>();
            organisationDataServiceMock
                .Setup(od => od.LegalOrgTypeMatchForUkprn(ukprn, orgType))
                .Returns(false);

            NewRule(learningDeliveryFamQueryServiceMock.Object, organisationDataService: organisationDataServiceMock.Object).ConditionMet(fundModel, learningDeliveryFams, learningDeliveryHe, ukprn).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseDeliveryHe()
        {
            var fundModel = 99;
            var ukprn = 12345678;
            var famType = "SOF";
            var famCode = "1";
            var orgType = "XXX";

            var learningDeliveryFams = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = famType,
                    LearnDelFAMCode = famCode
                }
            };

            var learningDeliveryHe = new TestLearningDeliveryHE();

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, famType, famCode))
                .Returns(true);

            var organisationDataServiceMock = new Mock<IOrganisationDataService>();
            organisationDataServiceMock
                .Setup(od => od.LegalOrgTypeMatchForUkprn(ukprn, orgType))
                .Returns(false);

            NewRule(learningDeliveryFamQueryServiceMock.Object, organisationDataService: organisationDataServiceMock.Object).ConditionMet(fundModel, learningDeliveryFams, learningDeliveryHe, ukprn).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseExcluded()
        {
            var fundModel = 99;
            var ukprn = 12345678;
            var famType = "SOF";
            var famCode = "1";
            var orgType = "UHEO";

            var learningDeliveryFams = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = famType,
                    LearnDelFAMCode = famCode
                }
            };

            TestLearningDeliveryHE learningDeliveryHe = null;

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, famType, famCode))
                .Returns(true);

            var organisationDataServiceMock = new Mock<IOrganisationDataService>();
            organisationDataServiceMock
                .Setup(od => od.LegalOrgTypeMatchForUkprn(ukprn, orgType))
                .Returns(true);

            NewRule(learningDeliveryFamQueryServiceMock.Object, organisationDataService: organisationDataServiceMock.Object).ConditionMet(fundModel, learningDeliveryFams, learningDeliveryHe, ukprn).Should().BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var famType = "SOF";
            var famCode = "1";
            var ukprn = 12345678;
            var orgType = "UHEO";

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 99,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = famType,
                                LearnDelFAMCode = famCode
                            }
                        }
                    }
                }
            };

            var learningDeliveryFams = learner.LearningDeliveries.SelectMany(ld => ld.LearningDeliveryFAMs);

            var fileDataServiceMock = new Mock<IFileDataService>();
            fileDataServiceMock.Setup(fdsm => fdsm.UKPRN()).Returns(ukprn);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, famType, famCode))
                .Returns(true);

            var organisationDataServiceMock = new Mock<IOrganisationDataService>();
            organisationDataServiceMock
                .Setup(od => od.LegalOrgTypeMatchForUkprn(ukprn, orgType))
                .Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learningDeliveryFamQueryServiceMock.Object, fileDataServiceMock.Object, organisationDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError()
        {
            var famType = "SOF";
            var famCode = "1";
            var ukprn = 12345678;
            var orgType = "UHEO";

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 99,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = famType,
                                LearnDelFAMCode = famCode
                            }
                        }
                    }
                }
            };

            var learningDeliveryFams = learner.LearningDeliveries.SelectMany(ld => ld.LearningDeliveryFAMs);

            var fileDataServiceMock = new Mock<IFileDataService>();
            fileDataServiceMock.Setup(fdsm => fdsm.UKPRN()).Returns(ukprn);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, famType, famCode))
                .Returns(true);

            var organisationDataServiceMock = new Mock<IOrganisationDataService>();
            organisationDataServiceMock
                .Setup(od => od.LegalOrgTypeMatchForUkprn(ukprn, orgType))
                .Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learningDeliveryFamQueryServiceMock.Object, fileDataServiceMock.Object, organisationDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("FundModel", 1)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(1);

            validationErrorHandlerMock.Verify();
        }

        private LearningDeliveryHE_02Rule NewRule(
            ILearningDeliveryFAMQueryService learningDeliveryFamQueryService = null,
            IFileDataService fileDataService = null,
            IOrganisationDataService organisationDataService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearningDeliveryHE_02Rule(learningDeliveryFamQueryService, fileDataService, organisationDataService, validationErrorHandler);
        }
    }
}
