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
        public void ValidationFails()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = TypeOfFunding.Age16To19ExcludingApprenticeships,
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
