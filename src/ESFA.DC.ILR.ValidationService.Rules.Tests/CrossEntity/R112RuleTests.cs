using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R112RuleTests : AbstractRuleTests<R112Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R112");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("2018-02-01")]
        public void ConditionMet_True(string dateTo)
        {
            int fundModel = 36;
            DateTime learnActEndDate = new DateTime(2018, 01, 01);
            DateTime? learnDelFamDateTo = string.IsNullOrEmpty(dateTo) ? (DateTime?)null : DateTime.Parse(dateTo);

            TestLearningDeliveryFAM learningDeliveryFam = new TestLearningDeliveryFAM
            {
                LearnDelFAMDateToNullable = learnDelFamDateTo
            };

            NewRule().ConditionMet(fundModel, learningDeliveryFam, learnActEndDate).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            int fundModel = 36;
            DateTime learnActEndDate = new DateTime(2018, 01, 01);
            DateTime? learnDelFamDateTo = new DateTime(2018, 01, 01);

            TestLearningDeliveryFAM learningDeliveryFam = new TestLearningDeliveryFAM
            {
                LearnDelFAMDateToNullable = learnDelFamDateTo
            };

            NewRule().ConditionMet(fundModel, learningDeliveryFam, learnActEndDate).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_DueToFundModel()
        {
            int fundModel = 81;
            DateTime learnActEndDate = new DateTime(2018, 01, 01);
            DateTime? learnDelFamDateTo = new DateTime(2018, 01, 01);

            TestLearningDeliveryFAM learningDeliveryFam = new TestLearningDeliveryFAM
            {
                LearnDelFAMDateToNullable = learnDelFamDateTo
            };

            NewRule().ConditionMet(fundModel, learningDeliveryFam, learnActEndDate).Should().BeFalse();
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            NewRule().FundModelConditionMet(36).Should().BeTrue();
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(81).Should().BeFalse();
        }

        [Fact]
        public void ExclusionConditionMet_True()
        {
            var fundModel = 36;
            var progType = 25;
            NewRule().ExclusionConditonMet(fundModel, progType).Should().BeTrue();
        }

        [Theory]
        [InlineData(36, null)]
        [InlineData(81, 20)]
        [InlineData(81, 25)]
        public void ExclusionConditionMet_False(int fundModel, int? progType)
        {
            NewRule().ExclusionConditonMet(fundModel, progType).Should().BeFalse();
        }

        [Fact]
        public void Validate_LearnActEndDateNull_NoError()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 36,
                        LearnActEndDateNullable = null
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_LearningDeliveryFamsNull_NoError()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new[]
                {
                    new TestLearningDelivery()
                    {
                        LearnActEndDateNullable = new DateTime(2000, 01, 01),
                        LearningDeliveryFAMs = null
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_LearningDeliveryFamToCheckNull_NoError()
        {
            var learningDeliveryFams = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.RES,
                    LearnDelFAMDateFromNullable = new DateTime(2018, 06, 01),
                    LearnDelFAMDateToNullable = new DateTime(2018, 06, 02)
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new[]
                {
                    new TestLearningDelivery()
                    {
                        LearnActEndDateNullable = new DateTime(2000, 01, 01),
                        LearningDeliveryFAMs = learningDeliveryFams
                    }
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock.Setup(qs => qs.GetLearningDeliveryFAMsForType(learningDeliveryFams, LearningDeliveryFAMTypeConstants.ACT))
                .Returns(new List<ILearningDeliveryFAM>());

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, learningDeliveryFAMsQueryServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learningDeliveryFams = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                    LearnDelFAMDateFromNullable = new DateTime(2018, 06, 01),
                    LearnDelFAMDateToNullable = new DateTime(2018, 06, 02)
                },
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                    LearnDelFAMDateFromNullable = new DateTime(2018, 08, 01),
                    LearnDelFAMDateToNullable = new DateTime(2018, 08, 02)
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new[]
                {
                    new TestLearningDelivery()
                    {
                        LearnActEndDateNullable = new DateTime(2018, 08, 02),
                        LearningDeliveryFAMs = learningDeliveryFams
                    }
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock.Setup(qs => qs.GetLearningDeliveryFAMsForType(learningDeliveryFams, LearningDeliveryFAMTypeConstants.ACT))
                .Returns(learningDeliveryFams);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, learningDeliveryFAMsQueryServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_MultipleLearningDeliveries()
        {
            var learningDeliveryFams = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                    LearnDelFAMDateFromNullable = new DateTime(2018, 06, 01),
                    LearnDelFAMDateToNullable = new DateTime(2018, 06, 02)
                },
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                    LearnDelFAMDateFromNullable = new DateTime(2018, 08, 01),
                    LearnDelFAMDateToNullable = new DateTime(2018, 08, 02)
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 36,
                        LearnActEndDateNullable = new DateTime(2018, 08, 02),
                        LearningDeliveryFAMs = learningDeliveryFams
                    },
                    new TestLearningDelivery()
                    {
                        FundModel = 36,
                        LearnActEndDateNullable = null,
                        LearningDeliveryFAMs = learningDeliveryFams
                    },
                    new TestLearningDelivery()
                    {
                        FundModel = 36,
                        LearnActEndDateNullable = new DateTime(2018, 08, 02),
                        LearningDeliveryFAMs = null
                    },
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock.Setup(qs => qs.GetLearningDeliveryFAMsForType(learningDeliveryFams, LearningDeliveryFAMTypeConstants.ACT))
                .Returns(learningDeliveryFams);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, learningDeliveryFAMsQueryServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_DueToExcludingCondition()
        {
            var learningDeliveryFams = new List<TestLearningDeliveryFAM>();

            var learner = new TestLearner()
            {
                LearningDeliveries = new[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 36,
                        ProgTypeNullable = 25,
                        LearnActEndDateNullable = new DateTime(2018, 08, 02),
                        LearningDeliveryFAMs = learningDeliveryFams
                    }
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock.Setup(qs => qs.GetLearningDeliveryFAMsForType(learningDeliveryFams, LearningDeliveryFAMTypeConstants.ACT))
                .Returns(learningDeliveryFams);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, learningDeliveryFAMsQueryServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Error()
        {
            var learningDeliveryFams = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                    LearnDelFAMDateFromNullable = new DateTime(2018, 06, 01),
                    LearnDelFAMDateToNullable = new DateTime(2018, 06, 02)
                },
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT,
                    LearnDelFAMDateFromNullable = new DateTime(2018, 08, 01),
                    LearnDelFAMDateToNullable = new DateTime(2018, 08, 02)
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new[]
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 36,
                        LearnActEndDateNullable = new DateTime(2018, 08, 10),
                        LearningDeliveryFAMs = learningDeliveryFams
                    }
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMsQueryServiceMock.Setup(qs => qs.GetLearningDeliveryFAMsForType(learningDeliveryFams, LearningDeliveryFAMTypeConstants.ACT))
                .Returns(learningDeliveryFams);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object, learningDeliveryFAMsQueryServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, "01/06/2018"));
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.ACT));
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMDateTo, "02/06/2018"));

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2018, 06, 01), LearningDeliveryFAMTypeConstants.ACT, new DateTime(2018, 06, 02));
            validationErrorHandlerMock.Verify();
        }

        public R112Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null)
        {
            return new R112Rule(
                validationErrorHandler: validationErrorHandler,
                learningDeliveryFAMQueryService: learningDeliveryFAMQueryService);
        }
    }
}
