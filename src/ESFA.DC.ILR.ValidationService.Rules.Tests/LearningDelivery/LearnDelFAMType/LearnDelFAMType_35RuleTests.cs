using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_35RuleTests : AbstractRuleTests<LearnDelFAMType_35RuleTests>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMType_35");
        }

        [Theory]
        [InlineData(98)]
        [InlineData(0)]
        [InlineData(1)]
        public void FundModelConditionMet_True(int fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeTrue();
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            var fundModel = 99;
            NewRule().FundModelConditionMet(fundModel).Should().BeFalse();
        }

        [Fact]
        public void FamTypeConditionMet_True()
        {
            var famType = "ADL";

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = famType
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQueryServiceMock
                .Setup(x => x.HasLearningDeliveryFAMType(learningDeliveryFAMs, famType))
                .Returns(true);

            NewRule(learningDeliveryFAMQueryServiceMock.Object).FamTypeConditionMet(learningDeliveryFAMs).Should().BeTrue();
        }

        [Theory]
        [InlineData("XXX")]
        [InlineData("ALB")]
        [InlineData("LDM")]
        public void FamTypeConditionMet_False(string famType)
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = famType
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQueryServiceMock
                .Setup(x => x.HasLearningDeliveryFAMType(learningDeliveryFAMs, famType))
                .Returns(false);

            NewRule(learningDeliveryFAMQueryServiceMock.Object).FamTypeConditionMet(learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var fundModel = 98;
            var famType = "ADL";

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = famType
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQueryServiceMock
                .Setup(x => x.HasLearningDeliveryFAMType(learningDeliveryFAMs, famType))
                .Returns(true);

            NewRule(learningDeliveryFAMQueryServiceMock.Object).ConditionMet(fundModel, learningDeliveryFAMs).Should().BeTrue();
        }

        [Theory]
        [InlineData(99, "ADL", true)]
        [InlineData(0, "ALB", false)]
        public void ConditionMet_False(int fundModel, string famType, bool mockResult)
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = famType
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQueryServiceMock
                .Setup(x => x.HasLearningDeliveryFAMType(learningDeliveryFAMs, famType))
                .Returns(mockResult);

            NewRule(learningDeliveryFAMQueryServiceMock.Object).ConditionMet(fundModel, learningDeliveryFAMs).Should().BeFalse();
        }

        [Theory]
        [InlineData(98, "ADL")]
        public void ValidateError(int fundModel, string famType)
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery
                    {
                        FundModel = fundModel,
                        LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = famType
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = "ALB"
                            }
                        }
                    }
                }
            };

            var learningDeliveryFAMs = learner.LearningDeliveries.SelectMany(ld => ld.LearningDeliveryFAMs);

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "ADL"))
                .Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Theory]
        [InlineData(99, "ADL", true)]
        [InlineData(98, "XXX", false)]
        public void ValidateNoError(int fundModel, string famType, bool mockResult)
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery
                    {
                        FundModel = fundModel,
                        LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = famType
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = "ALB"
                            }
                        }
                    }
                }
            };

            var learningDeliveryFAMs = learner.LearningDeliveries.SelectMany(ld => ld.LearningDeliveryFAMs);

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "ADL"))
                .Returns(mockResult);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            int fundModel = 98;

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("FundModel", fundModel)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(fundModel);

            validationErrorHandlerMock.Verify();
        }

        private LearnDelFAMType_35Rule NewRule(
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnDelFAMType_35Rule(learningDeliveryFAMQueryService, validationErrorHandler);
        }
    }
}
