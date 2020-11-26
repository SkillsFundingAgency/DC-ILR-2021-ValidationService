using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Model;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_83RuleTests : AbstractRuleTests<LearnDelFamType_83Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMType_83");
        }

        [Fact]
        public void ConditionMet_TrueNoMatch_LdmCode()
        {
            var ukprn = 123456789;
            var learnStartDate = new DateTime(2019, 8, 1);

            var learningDeliveryFam = new TestLearningDeliveryFAM
            {
                LearnDelFAMType = "LDM",
                LearnDelFAMCode = "370"
            };

            var organisation = new Organisation()
            {
                UKPRN = ukprn,
                ShortTermFundingInitiatives = new ShortTermFundingInitiative[]
                {
                    new ShortTermFundingInitiative()
                    {
                        UKPRN = ukprn,
                        LdmCode = "373",
                        EffectiveFrom = new DateTime(2019, 7, 1)
                    }
                }
            };

            var organisationDataServiceMock = new Mock<IOrganisationDataService>();
            organisationDataServiceMock.Setup(ods => ods.GetOrganisationFor(ukprn)).Returns(organisation);

            NewRule(organisationDataService: organisationDataServiceMock.Object).ConditionMet(ukprn, learnStartDate, learningDeliveryFam).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_TrueNoMatch_LearnStartDate()
        {
            var ukprn = 123456789;
            var learnStartDate = new DateTime(2019, 6, 1);

            var learningDeliveryFam = new TestLearningDeliveryFAM
            {
                LearnDelFAMType = "LDM",
                LearnDelFAMCode = "373"
            };

            var organisation = new Organisation()
            {
                UKPRN = ukprn,
                ShortTermFundingInitiatives = new ShortTermFundingInitiative[]
                {
                    new ShortTermFundingInitiative()
                    {
                        UKPRN = ukprn,
                        LdmCode = "373",
                        EffectiveFrom = new DateTime(2019, 7, 1)
                    }
                }
            };

            var organisationDataServiceMock = new Mock<IOrganisationDataService>();
            organisationDataServiceMock.Setup(ods => ods.GetOrganisationFor(ukprn)).Returns(organisation);

            NewRule(organisationDataService: organisationDataServiceMock.Object).ConditionMet(ukprn, learnStartDate, learningDeliveryFam).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_FalseHasMatch()
        {
            var ukprn = 123456789;
            var learnStartDate = new DateTime(2019, 8, 1);

            var learningDeliveryFam = new TestLearningDeliveryFAM
            {
                LearnDelFAMType = "LDM",
                LearnDelFAMCode = "370"
            };

            var organisation = new Organisation()
            {
                UKPRN = ukprn,
                ShortTermFundingInitiatives = new ShortTermFundingInitiative[]
                {
                    new ShortTermFundingInitiative()
                    {
                        UKPRN = ukprn,
                        LdmCode = "370",
                        EffectiveFrom = new DateTime(2019, 7, 1)
                    }
                }
            };

            var organisationDataServiceMock = new Mock<IOrganisationDataService>();
            organisationDataServiceMock.Setup(ods => ods.GetOrganisationFor(ukprn)).Returns(organisation);

            NewRule(organisationDataService: organisationDataServiceMock.Object).ConditionMet(ukprn, learnStartDate, learningDeliveryFam).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var ukprn = 123456789;
            var ldmCodes = new HashSet<string>()
            {
                "370",
                "371",
                "372",
                "373",
            };

            var ldmLearnDelFams = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "373"
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = new DateTime(2019, 6, 1),
                        LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = "ALB",
                                LearnDelFAMCode = "012"
                            },
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = "LDM",
                                LearnDelFAMCode = "373"
                            }
                        }
                    }
                }
            };

            var organisation = new Organisation()
            {
                UKPRN = ukprn,
                ShortTermFundingInitiatives = new ShortTermFundingInitiative[]
                {
                    new ShortTermFundingInitiative()
                    {
                        UKPRN = ukprn,
                        LdmCode = "373",
                        EffectiveFrom = new DateTime(2019, 7, 1)
                    }
                }
            };

            var organisationDataServiceMock = new Mock<IOrganisationDataService>();
            organisationDataServiceMock.Setup(ods => ods.GetOrganisationFor(ukprn)).Returns(organisation);

            var fileServiceMock = new Mock<IFileDataService>();
            fileServiceMock.Setup(fs => fs.UKPRN()).Returns(ukprn);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(ldm => ldm.GetLearningDeliveryFAMsForTypeAndCodes(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "LDM", ldmCodes))
                .Returns(ldmLearnDelFams);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learningDeliveryFamQueryServiceMock.Object, organisationDataServiceMock.Object, fileServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var ukprn = 123456789;
            var ldmCodes = new HashSet<string>()
            {
                "370",
                "371",
                "372",
                "373",
            };

            var ldmLearnDelFams = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "373"
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = new DateTime(2019, 8, 1),
                        LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = "ALB",
                                LearnDelFAMCode = "012"
                            },
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = "LDM",
                                LearnDelFAMCode = "373"
                            }
                        }
                    }
                }
            };

            var organisation = new Organisation()
            {
                UKPRN = ukprn,
                ShortTermFundingInitiatives = new ShortTermFundingInitiative[]
                {
                    new ShortTermFundingInitiative()
                    {
                        UKPRN = ukprn,
                        LdmCode = "373",
                        EffectiveFrom = new DateTime(2019, 7, 1)
                    }
                }
            };

            var organisationDataServiceMock = new Mock<IOrganisationDataService>();
            organisationDataServiceMock.Setup(ods => ods.GetOrganisationFor(ukprn)).Returns(organisation);

            var fileServiceMock = new Mock<IFileDataService>();
            fileServiceMock.Setup(fs => fs.UKPRN()).Returns(ukprn);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(ldm => ldm.GetLearningDeliveryFAMsForTypeAndCodes(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "LDM", ldmCodes))
                .Returns(ldmLearnDelFams);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learningDeliveryFamQueryServiceMock.Object, organisationDataServiceMock.Object, fileServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            var famCode = "370";

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, "LDM")).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, famCode)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(famCode);

            validationErrorHandlerMock.Verify();
        }

        private LearnDelFamType_83Rule NewRule(
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null,
            IOrganisationDataService organisationDataService = null,
            IFileDataService fileDataService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnDelFamType_83Rule(
                learningDeliveryFAMQueryService,
                organisationDataService,
                fileDataService,
                validationErrorHandler);
        }
    }
}
