using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.AddLine1;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.AddLine1
{
    public class Addline1_03RuleTests : AbstractRuleTests<AddLine1_03Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("Addline1_03");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void AddLine1ConditionMet_True(string addLine1)
        {
            NewRule().AddLine1ConditionMet(addLine1).Should().BeTrue();
        }

        [Fact]
        public void AddLine1ConditionMet_False()
        {
            NewRule().AddLine1ConditionMet("Not Null or White Space").Should().BeFalse();
        }

        [Theory]
        [InlineData(10, 20, "208", false)]
        [InlineData(10, 99, "208", false)]
        [InlineData(10, 99, "108", true)]
        [InlineData(35, 25, "108", true)]
        [InlineData(35, 99, "108", true)]
        public void CrossLearningDeliveryConditionMet_False(int ld1FundModel, int ld2FundModel, string learnDelFamCode, bool famMock)
        {
            IEnumerable<TestLearningDelivery> learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    FundModel = ld1FundModel,
                    LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                    {
                        new TestLearningDeliveryFAM
                        {
                            LearnDelFAMType = "SOF",
                            LearnDelFAMCode = learnDelFamCode
                        }
                    }
                },
                new TestLearningDelivery
                {
                    FundModel = ld2FundModel
                },
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFamQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "SOF", learnDelFamCode))
                .Returns(famMock);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFamQueryServiceMock.Object).CrossLearningDeliveryConditionMet(learningDeliveries).Should().BeFalse();
        }

        [Theory]
        [InlineData(10, 10, "208", false)]
        [InlineData(99, 99, "108", true)]
        public void CrossLearningDeliveryConditionMet_True(int ld1FundModel, int ld2FundModel, string learnDelFamCode, bool famMock)
        {
            IEnumerable<TestLearningDelivery> learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    FundModel = ld1FundModel,
                    LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                    {
                        new TestLearningDeliveryFAM
                        {
                            LearnDelFAMType = "SOF",
                            LearnDelFAMCode = learnDelFamCode
                        }
                    }
                },
                new TestLearningDelivery
                {
                    FundModel = ld2FundModel
                },
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFamQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "SOF", learnDelFamCode))
                .Returns(famMock);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFamQueryServiceMock.Object).CrossLearningDeliveryConditionMet(learningDeliveries).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(11)]
        [InlineData(50)]
        public void PlanLearnHoursConditionMet_True(int? planLearnHours)
        {
            NewRule().PlannedLearnHoursConditionMet(planLearnHours).Should().BeFalse();
        }

        [Theory]
        [InlineData(5)]
        [InlineData(10)]
        public void PlanLearnHoursConditionMet_False(int? planLearnHours)
        {
            NewRule().PlannedLearnHoursConditionMet(planLearnHours).Should().BeTrue();
        }

        [Theory]
        [InlineData(0, "XXX", "XXX", 100, "Not Empty")]
        [InlineData(99, "108", "SOF", 10, "Not Empty")]
        [InlineData(99, "108", "SOF", 5, "")]
        [InlineData(10, "000", "XXX", 5, " ")]
        public void ConditionMet_False(int fundModel, string learnDelFamCode, string learnDelFamType, int? planLearnHours, string addLine1)
        {
            IEnumerable<TestLearningDelivery> learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    FundModel = fundModel,
                    LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                    {
                        new TestLearningDeliveryFAM
                        {
                            LearnDelFAMType = learnDelFamType,
                            LearnDelFAMCode = learnDelFamCode
                        }
                    }
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFamQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), learnDelFamType, learnDelFamCode)).Returns(true);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFamQueryServiceMock.Object).ConditionMet(addLine1, learningDeliveries, planLearnHours).Should().BeFalse();
        }

        [Theory]
        [InlineData(0, "XXX", "XXX", 100, "")]
        [InlineData(99, "108", "XXX", 10, " ")]
        [InlineData(99, "108", "SOF", 50, "")]
        [InlineData(15, "000", "XXX", null, " ")]
        public void ConditionMet_True(int fundModel, string learnDelFamCode, string learnDelFamType, int? planLearnHours, string addLine1)
        {
            IEnumerable<TestLearningDelivery> learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    FundModel = fundModel,
                    LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                    {
                        new TestLearningDeliveryFAM
                        {
                            LearnDelFAMType = learnDelFamType,
                            LearnDelFAMCode = learnDelFamCode
                        }
                    }
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFamQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), learnDelFamType, learnDelFamCode)).Returns(true);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFamQueryServiceMock.Object).ConditionMet(addLine1, learningDeliveries, planLearnHours).Should().BeTrue();
        }

        [Theory]
        [InlineData(0, "XXX", "XXX", 100, "")]
        [InlineData(99, "108", "XXX", 10, " ")]
        [InlineData(99, "108", "SOF", 50, "")]
        [InlineData(15, "000", "XXX", null, " ")]
        public void Validate_Error(int fundModel, string learnDelFamCode, string learnDelFamType, int? planLearnHours, string addLine1)
        {
            var learner = new TestLearner
            {
                AddLine1 = addLine1,
                PlanLearnHoursNullable = planLearnHours,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = fundModel,
                        LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = learnDelFamType,
                                LearnDelFAMCode = learnDelFamCode
                            }
                        }
                    }
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFamQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), learnDelFamType, learnDelFamCode)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object, learningDeliveryFamQueryServiceMock.Object).Validate(learner);
            }
        }

        [Theory]
        [InlineData(0, "XXX", "XXX", 100, "Not Empty")]
        [InlineData(99, "108", "SOF", 10, "Not Empty")]
        [InlineData(99, "108", "SOF", 5, "")]
        [InlineData(10, "000", "XXX", 5, " ")]
        public void Validate_NoError(int fundModel, string learnDelFamCode, string learnDelFamType, int? planLearnHours, string addLine1)
        {
            var learner = new TestLearner
            {
                AddLine1 = addLine1,
                PlanLearnHoursNullable = planLearnHours,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = fundModel,
                        LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = learnDelFamType,
                                LearnDelFAMCode = learnDelFamCode
                            }
                        }
                    }
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFamQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), learnDelFamType, learnDelFamCode)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, learningDeliveryFamQueryServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("AddLine1", " ")).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(" ");

            validationErrorHandlerMock.Verify();
        }

        private AddLine1_03Rule NewRule(IValidationErrorHandler validationErrorHandler = null, ILearningDeliveryFAMQueryService learningDeliveryFamQueryService = null)
        {
            return new AddLine1_03Rule(validationErrorHandler, learningDeliveryFamQueryService);
        }
    }
}