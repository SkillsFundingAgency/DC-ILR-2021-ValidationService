using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Mocks;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_78RuleTests : AbstractRuleTests<LearnDelFAMType_78Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMType_78");
        }

        [Theory]
        [InlineData(TypeOfFunding.AdultSkills, LearningDeliveryFAMTypeConstants.DAM)]
        [InlineData(TypeOfFunding.CommunityLearning, LearningDeliveryFAMTypeConstants.DAM)]
        public void ConditionMet_False(int fundingModel, string famType)
        {
            var testLearningDelivery = new TestLearningDelivery()
            {
                FundModel = fundingModel,
                LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = famType
                    }
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQueryServiceMock
                .Setup(s => s.HasLearningDeliveryFAMType(It.IsAny<List<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.DAM))
                .Returns(true);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).ConditionMet(testLearningDelivery).Should().BeFalse();
        }

        [Theory]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships, LearningDeliveryFAMTypeConstants.DAM)]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017, LearningDeliveryFAMTypeConstants.DAM)]
        [InlineData(TypeOfFunding.EuropeanSocialFund, LearningDeliveryFAMTypeConstants.DAM)]
        [InlineData(TypeOfFunding.Other16To19, LearningDeliveryFAMTypeConstants.DAM)]
        [InlineData(TypeOfFunding.OtherAdult, LearningDeliveryFAMTypeConstants.DAM)]
        [InlineData(TypeOfFunding.NotFundedByESFA, LearningDeliveryFAMTypeConstants.DAM)]
        public void ConditionMet_True(int fundingModel, string famType)
        {
            var testLearningDelivery = new TestLearningDelivery()
            {
                FundModel = fundingModel,
                LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                {
                    new TestLearningDeliveryFAM()
                    {
                        LearnDelFAMType = famType
                    }
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQueryServiceMock
                .Setup(s => s.HasLearningDeliveryFAMType(It.IsAny<List<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.DAM))
                .Returns(true);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).ConditionMet(testLearningDelivery).Should().BeTrue();
        }

        [Theory]
        [InlineData(TypeOfFunding.AdultSkills)]
        [InlineData(TypeOfFunding.CommunityLearning)]
        public void ValidationPasses(int fundingModel)
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = fundingModel,
                        LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.DAM
                            },
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM
                            },
                        }
                    },
                }
            };
            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock
                .Setup(s => s.HasLearningDeliveryFAMType(
                    It.IsAny<List<ILearningDeliveryFAM>>(),
                    LearningDeliveryFAMTypeConstants.DAM)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, learningDeliveryFAMsQueryServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidationPasses_NoLDs()
        {
            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock
                .Setup(s => s.HasLearningDeliveryFAMType(It.IsAny<List<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.DAM))
                .Returns(false);

            var testLearner = new TestLearner();

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, learningDeliveryFAMsQueryServiceMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void ValidationPasses_NoFAMs()
        {
            var testLearner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery()
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock
                .Setup(s => s.HasLearningDeliveryFAMType(It.IsAny<List<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.DAM))
                .Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, learningDeliveryFAMsQueryServiceMock.Object).Validate(testLearner);
            }
        }

        [Theory]
        [InlineData(TypeOfFunding.Age16To19ExcludingApprenticeships)]
        [InlineData(TypeOfFunding.ApprenticeshipsFrom1May2017)]
        [InlineData(TypeOfFunding.EuropeanSocialFund)]
        [InlineData(TypeOfFunding.Other16To19)]
        [InlineData(TypeOfFunding.OtherAdult)]
        [InlineData(TypeOfFunding.NotFundedByESFA)]
        public void ValidationFails(int fundingModel)
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = fundingModel,
                        LearningDeliveryFAMs = new List<ILearningDeliveryFAM>()
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.DAM
                            },
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM
                            },
                        }
                    },
                }
            };
            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock
                .Setup(s => s.HasLearningDeliveryFAMType(
                    It.IsAny<List<ILearningDeliveryFAM>>(),
                    LearningDeliveryFAMTypeConstants.DAM)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object, learningDeliveryFAMsQueryServiceMock.Object).Validate(learner);
            }
        }

        private LearnDelFAMType_78Rule NewRule(IValidationErrorHandler validationErrorHandler = null, ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null)
        {
            return new LearnDelFAMType_78Rule(
                validationErrorHandler: validationErrorHandler,
                learningDeliveryFAMQueryService: learningDeliveryFAMQueryService);
        }
    }
}
