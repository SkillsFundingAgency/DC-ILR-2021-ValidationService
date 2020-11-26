using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
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
    public class LearnDelFAMType_91RuleTests : AbstractRuleTests<LearnDelFAMType_91Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMType_91");
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            NewRule().FundModelConditionMet(99).Should().BeTrue();
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(35).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMConditionMet_True()
        {
            var learningDeliverFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ADL"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "SOF"
                },
            };

            var learningDeliveryFamQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMType(learningDeliverFams, "ADL")).Returns(true);
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMType(learningDeliverFams, "SOF")).Returns(true);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFamQSMock.Object).LearningDeliveryFAMConditionMet(learningDeliverFams).Should().BeTrue();
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, false)]
        [InlineData(false, true)]
        public void LearningDeliveryFAMConditionMet_False(bool adlMock, bool sofMock)
        {
            var learningDeliverFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ADL"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM"
                },
            };

            var learningDeliveryFamQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMType(learningDeliverFams, "ADL")).Returns(adlMock);
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMType(learningDeliverFams, "SOF")).Returns(sofMock);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFamQSMock.Object).LearningDeliveryFAMConditionMet(learningDeliverFams).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var fundModel = 99;
            var learningDeliverFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ADL"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "SOF"
                },
            };

            var learningDeliveryFamQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMType(learningDeliverFams, "ADL")).Returns(true);
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMType(learningDeliverFams, "SOF")).Returns(true);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFamQSMock.Object).ConditionMet(fundModel, learningDeliverFams).Should().BeTrue();
        }

        [Theory]
        [InlineData(99, true, false)]
        [InlineData(99, false, false)]
        [InlineData(99, false, true)]
        [InlineData(35, true, true)]
        public void ConditionMet_False(int fundModel, bool adlMock, bool sofMock)
        {
            var learningDeliverFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ADL"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM"
                },
            };

            var learningDeliveryFamQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMType(learningDeliverFams, "ADL")).Returns(adlMock);
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMType(learningDeliverFams, "SOF")).Returns(sofMock);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFamQSMock.Object).ConditionMet(fundModel, learningDeliverFams).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learningDeliverFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ADL"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM"
                },
            };

            var learningDeliveryFamQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMType(learningDeliverFams, "ADL")).Returns(true);
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMType(learningDeliverFams, "SOF")).Returns(true);

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 99,
                        LearningDeliveryFAMs = learningDeliverFams
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object, learningDeliveryFamQSMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoErrors_FundModel()
        {
            var learningDeliverFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ADL"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM"
                },
            };

            var learningDeliveryFamQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMType(learningDeliverFams, "ADL")).Returns(true);
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMType(learningDeliverFams, "SOF")).Returns(true);

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 35,
                        LearningDeliveryFAMs = learningDeliverFams
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, learningDeliveryFamQSMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoErrors_FAMsMisMatch()
        {
            var learningDeliverFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ADL"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM"
                },
            };

            var learningDeliveryFamQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMType(learningDeliverFams, "ADL")).Returns(false);
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMType(learningDeliverFams, "SOF")).Returns(true);

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 99,
                        LearningDeliveryFAMs = learningDeliverFams
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, learningDeliveryFamQSMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoErrors_NoFAMs()
        {
            var learningDeliveryFamQSMock = new Mock<ILearningDeliveryFAMQueryService>();

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, learningDeliveryFamQSMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("FundModel", 99)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnDelFAMType", "SOF")).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters();

            validationErrorHandlerMock.Verify();
        }

        private LearnDelFAMType_91Rule NewRule(IValidationErrorHandler validationErrorHandler = null, ILearningDeliveryFAMQueryService learningDeliveryFamQueryService = null)
        {
            return new LearnDelFAMType_91Rule(validationErrorHandler, learningDeliveryFamQueryService);
        }
    }
}
