using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.FundModel;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_90RuleTests : AbstractRuleTests<LearnDelFAMType_90Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMType_90");
        }

        [Fact]
        public void ProgTypeConditionMet_True()
        {
            NewRule().ProgTypeConditionMet(24).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(100)]
        public void ProgTypeConditionMet_False(int? progType)
        {
            NewRule().ProgTypeConditionMet(progType).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learningDeliveryFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMCode = "376",
                    LearnDelFAMType = "SOF"
                },
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMCode = "376",
                    LearnDelFAMType = "HHS"
                },
            };

            var ldFamsMockResult = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMCode = "376",
                    LearnDelFAMType = "SOF"
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = 24,
                        LearningDeliveryFAMs = learningDeliveryFams
                    }
                }
            };

            var learningDeliveryFamQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQSMock.Setup(x => x.GetLearningDeliveryFAMsForType(learningDeliveryFams, "SOF")).Returns(ldFamsMockResult);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object, learningDeliveryFamQSMock.Object).Validate(learner);
                VerifyErrorHandlerMock(validationErrorHandlerMock, 1);
            }
        }

        [Fact]
        public void Validate_Error_Multiple()
        {
            var learningDeliveryFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMCode = "376",
                    LearnDelFAMType = "SOF"
                },
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMCode = "376",
                    LearnDelFAMType = "SOF"
                },
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = 24,
                        LearningDeliveryFAMs = learningDeliveryFams
                    }
                }
            };

            var learningDeliveryFamQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQSMock.Setup(x => x.GetLearningDeliveryFAMsForType(learningDeliveryFams, "SOF")).Returns(learningDeliveryFams);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object, learningDeliveryFamQSMock.Object).Validate(learner);
                VerifyErrorHandlerMock(validationErrorHandlerMock, 2);
            }
        }

        [Fact]
        public void Validate_NoError_ProgType()
        {
            var learningDeliveryFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMCode = "376",
                    LearnDelFAMType = "SOF"
                },
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMCode = "376",
                    LearnDelFAMType = "HHS"
                },
            };

            var ldFamsMockResult = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMCode = "376",
                    LearnDelFAMType = "SOF"
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = 99,
                        LearningDeliveryFAMs = learningDeliveryFams
                    }
                }
            };

            var learningDeliveryFamQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQSMock.Setup(x => x.GetLearningDeliveryFAMsForType(learningDeliveryFams, "SOF")).Returns(ldFamsMockResult);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, learningDeliveryFamQSMock.Object).Validate(learner);
                VerifyErrorHandlerMock(validationErrorHandlerMock, 0);
            }
        }

        [Fact]
        public void Validate_NoError_NoSofs()
        {
            var learningDeliveryFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMCode = "376",
                    LearnDelFAMType = "LDM"
                },
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMCode = "376",
                    LearnDelFAMType = "HHS"
                },
            };

            var ldFamsMockResult = new List<TestLearningDeliveryFAM>();

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = 24,
                        LearningDeliveryFAMs = learningDeliveryFams
                    }
                }
            };

            var learningDeliveryFamQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQSMock.Setup(x => x.GetLearningDeliveryFAMsForType(learningDeliveryFams, "SOF")).Returns(ldFamsMockResult);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, learningDeliveryFamQSMock.Object).Validate(learner);
                VerifyErrorHandlerMock(validationErrorHandlerMock, 0);
            }
        }

        [Fact]
        public void Validate_NoError_SofsValid()
        {
            var learningDeliveryFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMCode = "105",
                    LearnDelFAMType = "SOF"
                },
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMCode = "376",
                    LearnDelFAMType = "HHS"
                },
            };

            var ldFamsMockResult = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMCode = "105",
                    LearnDelFAMType = "SOF"
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = 24,
                        LearningDeliveryFAMs = learningDeliveryFams
                    }
                }
            };

            var learningDeliveryFamQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQSMock.Setup(x => x.GetLearningDeliveryFAMsForType(learningDeliveryFams, "SOF")).Returns(ldFamsMockResult);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, learningDeliveryFamQSMock.Object).Validate(learner);
                VerifyErrorHandlerMock(validationErrorHandlerMock, 0);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("ProgType", 24)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnDelFAMType", "SOF")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnDelFAMCode", "376")).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(24, "376");

            validationErrorHandlerMock.Verify();
        }

        private LearnDelFAMType_90Rule NewRule(IValidationErrorHandler validationErrorHandler = null, ILearningDeliveryFAMQueryService learningDeliveryFamQueryService = null)
        {
            return new LearnDelFAMType_90Rule(validationErrorHandler, learningDeliveryFamQueryService);
        }
    }
}
