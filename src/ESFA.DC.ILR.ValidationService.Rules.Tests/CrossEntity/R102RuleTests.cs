using System;
using System.Collections.Generic;
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
    public class R102RuleTests : AbstractRuleTests<R102Rule>
    {
        private readonly string _famTypeACT = LearningDeliveryFAMTypeConstants.ACT;

        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R102");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learnStartDate = new DateTime(2018, 10, 1);

            var learningDeliveryFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                    LearnDelFAMType = "LSF"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMDateFromNullable = new DateTime(2018, 9, 1),
                    LearnDelFAMType = "ACT"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                    LearnDelFAMType = "ACT"
                }
            };

            var learningeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFams, _famTypeACT)).Returns(true);
            learningeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMTypeForDate(learningDeliveryFams, _famTypeACT, learnStartDate)).Returns(false);

            NewRule(learningeliveryFAMQueryServiceMock.Object).ConditionMet(learnStartDate, learningDeliveryFams).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_LearnStartDateMatch()
        {
            var learnStartDate = new DateTime(2018, 8, 1);

            var learningDeliveryFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                    LearnDelFAMType = "LSF"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMDateFromNullable = new DateTime(2018, 9, 1),
                    LearnDelFAMType = "ACT"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                    LearnDelFAMType = "ACT"
                }
            };

            var learningeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFams, _famTypeACT)).Returns(true);
            learningeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMTypeForDate(learningDeliveryFams, _famTypeACT, learnStartDate)).Returns(true);

            NewRule(learningeliveryFAMQueryServiceMock.Object).ConditionMet(learnStartDate, learningDeliveryFams).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FamTypeMisMatch()
        {
            var learnStartDate = new DateTime(2018, 10, 1);

            var learningDeliveryFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                    LearnDelFAMType = "LSF"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMDateFromNullable = new DateTime(2018, 9, 1),
                    LearnDelFAMType = "SOF"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                    LearnDelFAMType = "RES"
                }
            };

            var learningeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFams, _famTypeACT)).Returns(false);
            learningeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMTypeForDate(learningDeliveryFams, _famTypeACT, learnStartDate)).Returns(false);

            NewRule(learningeliveryFAMQueryServiceMock.Object).ConditionMet(learnStartDate, learningDeliveryFams).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_NullFAMS()
        {
            var learnStartDate = new DateTime(2018, 8, 1);

            var learningeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(null, _famTypeACT)).Returns(false);
            learningeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMTypeForDate(null, _famTypeACT, learnStartDate)).Returns(false);

            NewRule(learningeliveryFAMQueryServiceMock.Object).ConditionMet(learnStartDate, null).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learnStartDate = new DateTime(2018, 8, 1);

            var learningDeliveryFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                    LearnDelFAMType = "LSF"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMDateFromNullable = new DateTime(2018, 9, 1),
                    LearnDelFAMType = "ACT"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                    LearnDelFAMType = "RES"
                }
            };

            var learner = new TestLearner()
            {
                LearnRefNumber = "00100309",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = learnStartDate,
                        LearningDeliveryFAMs = learningDeliveryFams
                    }
                }
            };

            var learningeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFams, _famTypeACT)).Returns(true);
            learningeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMTypeForDate(learningDeliveryFams, _famTypeACT, learnStartDate)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learningeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learnStartDate = new DateTime(2018, 8, 1);

            var learningDeliveryFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                    LearnDelFAMType = "LSF"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                    LearnDelFAMType = "ACT"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                    LearnDelFAMType = "RES"
                }
            };

            var learner = new TestLearner()
            {
                LearnRefNumber = "00100309",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = learnStartDate,
                        LearningDeliveryFAMs = learningDeliveryFams
                    }
                }
            };

            var learningeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFams, _famTypeACT)).Returns(true);
            learningeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMTypeForDate(learningDeliveryFams, _famTypeACT, learnStartDate)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learningeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_NullFams()
        {
            var learnStartDate = new DateTime(2018, 8, 1);

            var learner = new TestLearner()
            {
                LearnRefNumber = "00100309",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = learnStartDate
                    }
                }
            };

            var learningeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(null, _famTypeACT)).Returns(false);
            learningeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMTypeForDate(null, _famTypeACT, learnStartDate)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learningeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var learnStartDate = new DateTime(2018, 8, 1);

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnStartDate", "01/08/2018")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnDelFAMType", _famTypeACT)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(learnStartDate, _famTypeACT);

            validationErrorHandlerMock.Verify();
        }

        public R102Rule NewRule(ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new R102Rule(learningDeliveryFAMQueryService, validationErrorHandler);
        }
    }
}
