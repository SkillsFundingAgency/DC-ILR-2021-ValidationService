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
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_89RuleTests : AbstractRuleTests<LearnDelFAMType_89Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMType_89");
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
                NewRule(larsDataServiceMock.Object, learningDeliveryFamQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Error()
        {
            var ldmLearnDelFams = new List<TestLearningDeliveryFAM>()
            {
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
                                LearnDelFAMCode = LearningDeliveryFAMCodeConstants.LDM_370
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

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(larsDataServiceMock.Object, learningDeliveryFamQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void HasClassroomBased_True()
        {
            var ldmLearnDelFams = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                    LearnDelFAMCode = LearningDeliveryFAMCodeConstants.LDM_376
                }
            };

            var learningDelivery = new TestLearningDelivery()
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
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock.Setup(f => f.GetLearningDeliveryFAMsForTypeAndCodes(It.IsAny<IReadOnlyCollection<ILearningDeliveryFAM>>(), It.IsAny<string>(), It.IsAny<HashSet<string>>()))
                .Returns(ldmLearnDelFams);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFamQueryServiceMock.Object).HasClassroomBased(learningDelivery).Should().BeTrue();
        }

        [Fact]
        public void HasClassroomBased_False()
        {
            var ldmLearnDelFams = new List<TestLearningDeliveryFAM>()
            {
            };

            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = FundModels.AdultSkills,
                LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                {
                     new TestLearningDeliveryFAM()
                     {
                          LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                          LearnDelFAMCode = LearningDeliveryFAMCodeConstants.LDM_370
                     }
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock.Setup(f => f.GetLearningDeliveryFAMsForTypeAndCodes(It.IsAny<IReadOnlyCollection<ILearningDeliveryFAM>>(), It.IsAny<string>(), It.IsAny<HashSet<string>>()))
                .Returns(ldmLearnDelFams);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFamQueryServiceMock.Object).HasClassroomBased(learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void IsCovid19SkillsOffer_True()
        {
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

            NewRule(larsDataServiceMock.Object).IsCovid19SkillsOffer(larsCategories).Should().BeTrue();
        }

        [Fact]
        public void IsCovid19SkillsOffer_False()
        {
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

            NewRule(larsDataServiceMock.Object).IsCovid19SkillsOffer(larsCategories).Should().BeFalse();
        }

        private LearnDelFAMType_89Rule NewRule(
            ILARSDataService larsDataService = null,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnDelFAMType_89Rule(
                larsDataService,
                learningDeliveryFAMQueryService,
                validationErrorHandler);
        }
    }
}
