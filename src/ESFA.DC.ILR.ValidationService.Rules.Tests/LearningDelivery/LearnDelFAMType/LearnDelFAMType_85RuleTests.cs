using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Model;
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
    public class LearnDelFAMType_85RuleTests : AbstractRuleTests<LearnDelFamType_85Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMType_85");
        }

        [Fact]
        public void Validate_Error()
        {
            var ldmLearnDelFams = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                    LearnDelFAMCode = LearningDeliveryFAMCodeConstants.LDM_376
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = FundModels.AdultSkills,
                        LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                                LearnDelFAMCode = LearningDeliveryFAMCodeConstants.LDM_376
                            }
                        }
                    }
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock.Setup(f => f.GetLearningDeliveryFAMsForTypeAndCodes(It.IsAny<IReadOnlyCollection<ILearningDeliveryFAM>>(), It.IsAny<string>(), It.IsAny<HashSet<string>>()))
                .Returns(ldmLearnDelFams);

            var larsCategories = new List<ILARSLearningCategory>()
            {
                new LearningDeliveryCategory()
                {
                    CategoryRef = LARSConstants.Categories.LegalEntitlementLevel2
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(l => l.GetCategoriesFor(It.IsAny<string>()))
                .Returns(larsCategories);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learningDeliveryFamQueryServiceMock.Object, validationErrorHandlerMock.Object, larsDataServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_Excluded()
        {
            var ldmLearnDelFams = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                    LearnDelFAMCode = LearningDeliveryFAMCodeConstants.LDM_376
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = FundModels.AdultSkills,
                        LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                                LearnDelFAMCode = LearningDeliveryFAMCodeConstants.LDM_376
                            }
                        }
                    }
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock.Setup(f => f.GetLearningDeliveryFAMsForTypeAndCodes(It.IsAny<IReadOnlyCollection<ILearningDeliveryFAM>>(), It.IsAny<string>(), It.IsAny<HashSet<string>>()))
                .Returns(ldmLearnDelFams);

            var larsCategories = new List<ILARSLearningCategory>()
            {
                new LearningDeliveryCategory()
                {
                    CategoryRef = LARSConstants.Categories.LegalEntitlementLevel2
                },
                new LearningDeliveryCategory()
                {
                    CategoryRef = LARSConstants.Categories.Covid19SkillsOfferOnly
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(l => l.GetCategoriesFor(It.IsAny<string>()))
                .Returns(larsCategories);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learningDeliveryFamQueryServiceMock.Object, validationErrorHandlerMock.Object, larsDataServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var ldmLearnDelFams = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                    LearnDelFAMCode = LearningDeliveryFAMCodeConstants.LDM_376
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = FundModels.AdultSkills,
                        LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                                LearnDelFAMCode = LearningDeliveryFAMCodeConstants.LDM_376
                            }
                        }
                    }
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock.Setup(f => f.GetLearningDeliveryFAMsForTypeAndCodes(It.IsAny<IReadOnlyCollection<ILearningDeliveryFAM>>(), It.IsAny<string>(), It.IsAny<HashSet<string>>()))
                .Returns(ldmLearnDelFams);

            var larsCategories = new List<ILARSLearningCategory>()
            {
                new LearningDeliveryCategory()
                {
                    CategoryRef = LARSConstants.Categories.Covid19SkillsOffer
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(l => l.GetCategoriesFor(It.IsAny<string>()))
                .Returns(larsCategories);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learningDeliveryFamQueryServiceMock.Object, validationErrorHandlerMock.Object, larsDataServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            var larsCategories = new List<ILARSLearningCategory>()
            {
                new LearningDeliveryCategory()
                {
                    CategoryRef = LARSConstants.Categories.LegalEntitlementLevel2,
                },
                new LearningDeliveryCategory()
                {
                    CategoryRef = LARSConstants.Categories.LicenseToPractice
                }
            };

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.LDM)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, LearningDeliveryFAMCodeConstants.LDM_376)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.FundModel, FundModels.AdultSkills)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LarsCategoryRef, LARSConstants.Categories.LegalEntitlementLevel2)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LarsCategoryRef, LARSConstants.Categories.LicenseToPractice)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(larsCategories);

            validationErrorHandlerMock.Verify();
        }

        private LearnDelFamType_85Rule NewRule(
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null,
            IValidationErrorHandler validationErrorHandler = null,
            ILARSDataService larsDataService = null)
        {
            return new LearnDelFamType_85Rule(
                learningDeliveryFAMQueryService,
                validationErrorHandler,
                larsDataService);
        }
    }
}
