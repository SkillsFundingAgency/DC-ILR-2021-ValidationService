using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Learner.MathGrade;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.MathGrade
{
    public class MathGrade_03RuleTests : AbstractRuleTests<MathGrade_03Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("MathGrade_03");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("AA")]
        public void LearnerMathGradeConditionMet_False(string mathGrade)
        {
            NewRule().LearnerMathGradeConditionMet(mathGrade).Should().BeFalse();
        }

        [Theory]
        [InlineData("D")]
        [InlineData("DD")]
        [InlineData("DE")]
        [InlineData("E")]
        [InlineData("EE")]
        [InlineData("EF")]
        [InlineData("F")]
        [InlineData("FF")]
        [InlineData("FG")]
        [InlineData("G")]
        [InlineData("GG")]
        [InlineData("N")]
        [InlineData("U")]
        [InlineData("d")]
        [InlineData("dd")]
        [InlineData("de")]
        [InlineData("e")]
        [InlineData("ee")]
        [InlineData("ef")]
        [InlineData("f")]
        [InlineData("ff")]
        [InlineData("fg")]
        [InlineData("g")]
        [InlineData("gg")]
        [InlineData("n")]
        [InlineData("u")]
        [InlineData("3")]
        [InlineData("2")]
        [InlineData("1")]
        public void LearnerMathGradeConditionMet_True(string mathGrade)
        {
            NewRule().LearnerMathGradeConditionMet(mathGrade).Should().BeTrue();
        }

        [Fact]
        public void LearnerFAMsConditionMet_False()
        {
            IEnumerable<TestLearnerFAM> learnerFAMs = new[]
            {
                new TestLearnerFAM()
                {
                    LearnFAMType = LearnerFAMTypeConstants.EDF,
                    LearnFAMCode = 1
                },
                new TestLearnerFAM()
                {
                    LearnFAMType = LearnerFAMTypeConstants.HNS,
                    LearnFAMCode = 101
                }
            };

            var learnerFAMQueryServiceMock = new Mock<ILearnerFAMQueryService>();

            learnerFAMQueryServiceMock.Setup(dd => dd.HasLearnerFAMCodeForType(learnerFAMs, LearnerFAMTypeConstants.EDF, 1)).Returns(true);

            NewRule(learnerFAMQueryService: learnerFAMQueryServiceMock.Object).LearnerFAMsConditionMet(learnerFAMs).Should().BeFalse();
        }

        [Fact]
        public void LearnerFAMsConditionMet_True()
        {
            IEnumerable<TestLearnerFAM> learnerFAMs = new[]
            {
                new TestLearnerFAM()
                {
                    LearnFAMType = LearnerFAMTypeConstants.DLA,
                    LearnFAMCode = 20
                },
                new TestLearnerFAM()
                {
                    LearnFAMType = LearnerFAMTypeConstants.HNS,
                    LearnFAMCode = 101
                }
            };

            var learnerFAMQueryServiceMock = new Mock<ILearnerFAMQueryService>();

            learnerFAMQueryServiceMock.Setup(dd => dd.HasLearnerFAMCodeForType(learnerFAMs, LearnerFAMTypeConstants.EDF, 1)).Returns(false);

            NewRule(learnerFAMQueryService: learnerFAMQueryServiceMock.Object).LearnerFAMsConditionMet(learnerFAMs).Should().BeTrue();
        }

        [Fact]
        public void LearnerFAMsConditionMet_True_Null()
        {
            IEnumerable<TestLearnerFAM> learnerFAMs = new[]
            {
                new TestLearnerFAM()
                {
                    LearnFAMType = LearnerFAMTypeConstants.DLA,
                    LearnFAMCode = 20
                },
                new TestLearnerFAM()
                {
                    LearnFAMType = LearnerFAMTypeConstants.HNS,
                    LearnFAMCode = 101
                }
            };

            var learnerFAMQueryServiceMock = new Mock<ILearnerFAMQueryService>();

            learnerFAMQueryServiceMock.Setup(dd => dd.HasLearnerFAMCodeForType(learnerFAMs, LearnerFAMTypeConstants.EDF, 1)).Returns(false);

            NewRule(learnerFAMQueryService: learnerFAMQueryServiceMock.Object).LearnerFAMsConditionMet(null).Should().BeTrue();
        }

        [Theory]
        [InlineData("D")]
        [InlineData("DD")]
        [InlineData("DE")]
        [InlineData("E")]
        [InlineData("EE")]
        [InlineData("EF")]
        [InlineData("F")]
        [InlineData("FF")]
        [InlineData("FG")]
        [InlineData("G")]
        [InlineData("GG")]
        [InlineData("N")]
        [InlineData("U")]
        [InlineData("3")]
        [InlineData("2")]
        [InlineData("1")]
        public void ConditionMet_True(string mathGrade)
        {
            IEnumerable<TestLearnerFAM> learnerFAMs = new[]
            {
                new TestLearnerFAM()
                {
                    LearnFAMType = LearnerFAMTypeConstants.DLA,
                    LearnFAMCode = 20
                },
                new TestLearnerFAM()
                {
                    LearnFAMType = LearnerFAMTypeConstants.HNS,
                    LearnFAMCode = 101
                }
            };

            var learnerFAMQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            learnerFAMQueryServiceMock.Setup(dd => dd.HasLearnerFAMCodeForType(learnerFAMs, LearnerFAMTypeConstants.EHC, 25)).Returns(false);

            NewRule(learnerFAMQueryService: learnerFAMQueryServiceMock.Object).ConditionMet(mathGrade, learnerFAMs).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("AA")]
        public void ConditionMet_False(string mathGrade)
        {
            IEnumerable<TestLearnerFAM> learnerFAMs = new[]
            {
                new TestLearnerFAM()
                {
                    LearnFAMType = LearnerFAMTypeConstants.ECF,
                    LearnFAMCode = 1
                },
                new TestLearnerFAM()
                {
                    LearnFAMType = LearnerFAMTypeConstants.HNS,
                    LearnFAMCode = 101
                }
            };

            var learnerFAMQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            learnerFAMQueryServiceMock.Setup(dd => dd.HasLearnerFAMCodeForType(learnerFAMs, "EDF", 1)).Returns(true);

            NewRule(learnerFAMQueryService: learnerFAMQueryServiceMock.Object).ConditionMet(mathGrade, learnerFAMs).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            IReadOnlyCollection<TestLearnerFAM> learnerFAMs = new[]
            {
                new TestLearnerFAM()
                {
                    LearnFAMType = LearnerFAMTypeConstants.EDF,
                    LearnFAMCode = 2
                },
                new TestLearnerFAM()
                {
                    LearnFAMType = LearnerFAMTypeConstants.HNS,
                    LearnFAMCode = 101
                }
            };

            var learner = new TestLearner()
            {
                MathGrade = "D",
                LearnerFAMs = learnerFAMs
            };

            var learnerFamQueryServiceMock = new Mock<ILearnerFAMQueryService>();

            learnerFamQueryServiceMock.Setup(dd => dd.HasLearnerFAMCodeForType(learnerFAMs, LearnerFAMTypeConstants.DLA, 5)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learnerFAMQueryService: learnerFamQueryServiceMock.Object, validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            IReadOnlyCollection<TestLearnerFAM> learnerFAMs = new[]
           {
                new TestLearnerFAM()
                {
                    LearnFAMType = LearnerFAMTypeConstants.ECF,
                    LearnFAMCode = 1
                },
                new TestLearnerFAM()
                {
                    LearnFAMType = LearnerFAMTypeConstants.HNS,
                    LearnFAMCode = 101
                }
            };

            var learner = new TestLearner()
            {
                MathGrade = "AA",
                LearnerFAMs = learnerFAMs
            };

            var learnerFamQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            learnerFamQueryServiceMock.Setup(dd => dd.HasLearnerFAMCodeForType(learnerFAMs, LearnerFAMTypeConstants.ECF, 1)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object, learnerFAMQueryService: learnerFamQueryServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorHandlerParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            validationErrorHandlerMock.Setup(dd => dd.BuildErrorMessageParameter(PropertyNameConstants.MathGrade, "D")).Verifiable();
            validationErrorHandlerMock.Setup(dd => dd.BuildErrorMessageParameter(PropertyNameConstants.LearnFAMType, LearnerFAMTypeConstants.EDF)).Verifiable();
            validationErrorHandlerMock.Setup(dd => dd.BuildErrorMessageParameter(PropertyNameConstants.LearnFAMCode, 1));

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters("D", LearnerFAMTypeConstants.EDF, 1);
            validationErrorHandlerMock.Verify();
        }

        private MathGrade_03Rule NewRule(IValidationErrorHandler validationErrorHandler = null, ILearnerFAMQueryService learnerFAMQueryService = null)
        {
            return new MathGrade_03Rule(validationErrorHandler: validationErrorHandler, learnerFAMQueryService: learnerFAMQueryService);
        }
    }
}
