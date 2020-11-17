using System;
using System.Collections.Generic;
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
    public class LearnDelFAMType_92RuleTests : AbstractRuleTests<LearnDelFAMType_92Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMType_92");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learnStartDate = new DateTime(2022, 01, 10);
            var aimType = 1;
            var learningDeliverFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMCode = "2"
                },
            };

            var learningDeliveryFamQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(learningDeliverFams, "ACT", "2")).Returns(true);
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMType(learningDeliverFams, "RES")).Returns(false);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFamQSMock.Object).ConditionMet(learnStartDate, aimType, learningDeliverFams).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_FalseExcluded()
        {
            var learnStartDate = new DateTime(2022, 01, 10);
            var aimType = 1;
            var learningDeliverFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMCode = "2"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "RES"
                },
            };

            var learningDeliveryFamQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(learningDeliverFams, "ACT", "2")).Returns(true);
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMType(learningDeliverFams, "RES")).Returns(true);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFamQSMock.Object).ConditionMet(learnStartDate, aimType, learningDeliverFams).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseLearnStartDate()
        {
            var learnStartDate = new DateTime(2021, 01, 10);
            var aimType = 1;
            var learningDeliverFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMCode = "2"
                },
            };

            var learningDeliveryFamQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(learningDeliverFams, "ACT", "2")).Returns(true);
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMType(learningDeliverFams, "RES")).Returns(false);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFamQSMock.Object).ConditionMet(learnStartDate, aimType, learningDeliverFams).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseAimType()
        {
            var learnStartDate = new DateTime(2022, 01, 10);
            var aimType = 5;
            var learningDeliverFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMCode = "2"
                },
            };

            var learningDeliveryFamQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(learningDeliverFams, "ACT", "2")).Returns(true);
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMType(learningDeliverFams, "RES")).Returns(false);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFamQSMock.Object).ConditionMet(learnStartDate, aimType, learningDeliverFams).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseFams()
        {
            var learnStartDate = new DateTime(2022, 01, 10);
            var aimType = 1;
            var learningDeliverFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMCode = "5"
                },
            };

            var learningDeliveryFamQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(learningDeliverFams, "ACT", "2")).Returns(false);
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMType(learningDeliverFams, "RES")).Returns(false);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFamQSMock.Object).ConditionMet(learnStartDate, aimType, learningDeliverFams).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learningDeliverFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMCode = "2"
                },
            };

            var learningDeliveryFamQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(learningDeliverFams, "ACT", "2")).Returns(true);
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMType(learningDeliverFams, "RES")).Returns(false);

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        LearnStartDate = new DateTime(2022, 01, 10),
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
        public void Validate_NoError()
        {
            var learningDeliverFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMCode = "2"
                },
            };

            var learningDeliveryFamQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(learningDeliverFams, "ACT", "2")).Returns(true);
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMType(learningDeliverFams, "RES")).Returns(false);

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = 5,
                        LearnStartDate = new DateTime(2022, 01, 10),
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
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            var learnStartDate = new DateTime(2022, 04, 01);
            var aimType = 2;

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnStartDate", "01/04/2022")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("AimType", aimType)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnDelFAMType", "ACT")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnDelFAMCode", "2")).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(learnStartDate, aimType);

            validationErrorHandlerMock.Verify();
        }

        private LearnDelFAMType_92Rule NewRule(IValidationErrorHandler validationErrorHandler = null, ILearningDeliveryFAMQueryService learningDeliveryFamQueryService = null)
        {
            return new LearnDelFAMType_92Rule(validationErrorHandler, learningDeliveryFamQueryService);
        }
    }
}
