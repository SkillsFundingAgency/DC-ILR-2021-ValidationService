using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.GivenNames;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.GivenNames
{
    public class GivenNames_04RuleTests : AbstractRuleTests<GivenNames_04Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("GivenNames_04");
        }

        [Theory]
        [InlineData(10, 10, "208", false, true)]
        [InlineData(10, 20, "208", false, false)]
        [InlineData(10, 99, "208", false, false)]
        [InlineData(99, 10, "108", true, true)]
        [InlineData(99, 99, "108", true, true)]
        [InlineData(35, 25, "108", true, false)]
        [InlineData(35, 99, "108", true, false)]
        public void CrossLearningDeliveryConditionMet(int ld1FundModel, int ld2FundModel, string learnDelFamCode, bool famMock, bool testPass)
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

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "SOF", learnDelFamCode)).Returns(famMock);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).CrossLearningDeliveryConditionMet(learningDeliveries).Should().Be(testPass);
        }

        [Fact]
        public void GivenNamesConditionMet_True_Null()
        {
            NewRule().GivenNamesConditionMet(null).Should().BeTrue();
        }

        [Fact]
        public void GivenNamesConditionMet_True_Whitespace()
        {
            NewRule().GivenNamesConditionMet("    ").Should().BeTrue();
        }

        [Fact]
        public void GivenNamesConditionMet_False()
        {
            NewRule().GivenNamesConditionMet("Not Null or White Space").Should().BeFalse();
        }

        [Fact]
        public void PlanLearnHoursConditionMet_True()
        {
            NewRule().PlanLearnHoursConditionMet(10).Should().BeTrue();
        }

        [Fact]
        public void PlanLearnHoursConditionMet_False_Null()
        {
            NewRule().PlanLearnHoursConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void PlanLearnHoursConditionMet_False()
        {
            NewRule().PlanLearnHoursConditionMet(20).Should().BeFalse();
        }

        [Fact]
        public void ULNConditionMet_True()
        {
            long uln = 1111111111;

            NewRule().ULNConditionMet(uln).Should().BeTrue();
        }

        [Fact]
        public void ULNConditionMet_False_TemporaryULN()
        {
            long uln = 9999999999;

            NewRule().ULNConditionMet(uln).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var fundModel = 10;
            var learnDelFamCode = "108";
            long uln = 1111111111;

            IEnumerable<TestLearningDelivery> learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    FundModel = fundModel,
                    LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                    {
                        new TestLearningDeliveryFAM
                        {
                            LearnDelFAMType = "SOF",
                            LearnDelFAMCode = learnDelFamCode
                        }
                    }
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "SOF", learnDelFamCode)).Returns(false);

            NewRule(learningDeliveryFAMQueryServiceMock.Object).ConditionMet(null, 5, uln, learningDeliveries).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var fundModel = 35;
            var learnDelFamCode = "108";
            long uln = 1111111111;

            IEnumerable<TestLearningDelivery> learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    FundModel = fundModel,
                    LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                    {
                        new TestLearningDeliveryFAM
                        {
                            LearnDelFAMType = "SOF",
                            LearnDelFAMCode = learnDelFamCode
                        }
                    }
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "SOF", learnDelFamCode)).Returns(false);

            NewRule(learningDeliveryFAMQueryServiceMock.Object).ConditionMet(null, 20, uln, learningDeliveries).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_PlanLearnHours()
        {
            var fundModel = 10;
            var learnDelFamCode = "108";
            var uln = 1111111111;

            IEnumerable<TestLearningDelivery> learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    FundModel = fundModel,
                    LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                    {
                        new TestLearningDeliveryFAM
                        {
                            LearnDelFAMType = "SOF",
                            LearnDelFAMCode = learnDelFamCode
                        }
                    }
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "SOF", learnDelFamCode)).Returns(false);

            NewRule(learningDeliveryFAMQueryServiceMock.Object).ConditionMet(null, 20, uln, learningDeliveries).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_ULN()
        {
            var fundModel = 10;
            var learnDelFamCode = "108";
            var uln = 1111111111;

            IEnumerable<TestLearningDelivery> learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    FundModel = fundModel,
                    LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                    {
                        new TestLearningDeliveryFAM
                        {
                            LearnDelFAMType = "SOF",
                            LearnDelFAMCode = learnDelFamCode
                        }
                    }
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "SOF", learnDelFamCode)).Returns(false);

            NewRule(learningDeliveryFAMQueryServiceMock.Object).ConditionMet(null, 20, uln, learningDeliveries).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner()
            {
                PlanLearnHoursNullable = 5,
                GivenNames = null,
                ULN = 1111111111,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = 10,
                        LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = "SOF",
                                LearnDelFAMCode = "100"
                            }
                        }
                    }
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "SOF", "100")).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoErrors_ConditionMet()
        {
            var learner = new TestLearner()
            {
                GivenNames = "Not Null or White Space",
                ULN = 1111111111,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = 35,
                        LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = "SOF",
                                LearnDelFAMCode = "100"
                            }
                        }
                    }
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "SOF", "100")).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoErrors_LearningDeliveryConditionMet()
        {
            var learner = new TestLearner()
            {
                GivenNames = null,
                ULN = 1111111111,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        FundModel = 10,
                        LearningDeliveryFAMs = new TestLearningDeliveryFAM[]
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = "SOF",
                                LearnDelFAMCode = "100"
                            }
                        }
                    }
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "SOF", "100")).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("GivenNames", " ")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("PlanLearnHours", 1)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(" ", 1);

            validationErrorHandlerMock.Verify();
        }

        private GivenNames_04Rule NewRule(ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new GivenNames_04Rule(learningDeliveryFAMQueryService, validationErrorHandler);
        }
    }
}
