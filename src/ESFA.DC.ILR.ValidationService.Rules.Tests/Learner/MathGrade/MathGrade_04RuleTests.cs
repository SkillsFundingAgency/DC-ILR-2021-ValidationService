using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.EngGrade;
using ESFA.DC.ILR.ValidationService.Rules.Learner.MathGrade;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.MathGrade
{
    public class MathGrade_04RuleTests : AbstractRuleTests<MathGrade_04Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("MathGrade_04");
        }

        [Fact]
        public void MathGradeConditionMet_True()
        {
            var mathGrade = "A";

            NewRule().MathGradeConditionMet(mathGrade).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("NONE")]
        [InlineData("None")]
        [InlineData("none")]
        public void MathGradeConditionMet_False(string mathGrade)
        {
            NewRule().MathGradeConditionMet(mathGrade).Should().BeFalse();
        }

        [Fact]
        public void LearnerFAMConditionMet_True()
        {
            var learnerFAMQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            learnerFAMQueryServiceMock
                .Setup(qs => qs.HasAnyLearnerFAMCodesForType(It.IsAny<IEnumerable<ILearnerFAM>>(), "MCF", new[] { 2, 3, 4 }))
                .Returns(true);

            NewRule(learnerFAMQueryServiceMock.Object).LearnerFAMConditionMet(It.IsAny<IEnumerable<ILearnerFAM>>()).Should().BeTrue();
        }

        [Fact]
        public void LearnerFamConditionMet_False()
        {
            var learnerFAMQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            learnerFAMQueryServiceMock
                .Setup(qs => qs.HasAnyLearnerFAMCodesForType(It.IsAny<IEnumerable<ILearnerFAM>>(), "MCF", new[] { 2, 3, 4 }))
                .Returns(false);

            NewRule(learnerFAMQueryServiceMock.Object).LearnerFAMConditionMet(It.IsAny<IEnumerable<ILearnerFAM>>()).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var mathGrade = "A";

            var learnerFams = new List<TestLearnerFAM>()
            {
                new TestLearnerFAM()
                {
                    LearnFAMType = "MCF",
                    LearnFAMCode = 2
                }
            };

            var learnerFAMQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            learnerFAMQueryServiceMock
                .Setup(qs => qs.HasAnyLearnerFAMCodesForType(learnerFams, "MCF", new[] { 2, 3, 4 }))
                .Returns(true);

            NewRule(learnerFAMQueryServiceMock.Object).ConditionMet(mathGrade, learnerFams).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_FalseNoneGrade()
        {
            var mathGrade = "NONE";

            var learnerFams = new List<TestLearnerFAM>()
            {
                new TestLearnerFAM()
                {
                    LearnFAMType = "MCF",
                    LearnFAMCode = 2
                }
            };

            var learnerFAMQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            learnerFAMQueryServiceMock
                .Setup(qs => qs.HasAnyLearnerFAMCodesForType(learnerFams, "MCF", new[] { 2, 3, 4 }))
                .Returns(true);

            NewRule(learnerFAMQueryServiceMock.Object).ConditionMet(mathGrade, learnerFams).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseFAMType()
        {
            var mathGrade = "A";

            var learnerFams = new List<TestLearnerFAM>()
            {
                new TestLearnerFAM()
                {
                    LearnFAMType = "XXX",
                    LearnFAMCode = 2
                }
            };

            var learnerFAMQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            learnerFAMQueryServiceMock
                .Setup(qs => qs.HasAnyLearnerFAMCodesForType(learnerFams, "MCF", new[] { 2, 3, 4 }))
                .Returns(false);

            NewRule(learnerFAMQueryServiceMock.Object).ConditionMet(mathGrade, learnerFams).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseLearnerFAMNull()
        {
            var mathGrade = "A";

            var learnerFAMQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            learnerFAMQueryServiceMock
                .Setup(qs => qs.HasAnyLearnerFAMCodesForType(null, "MCF", new[] { 2, 3, 4 }))
                .Returns(false);

            NewRule(learnerFAMQueryServiceMock.Object).ConditionMet(mathGrade, null).Should().BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var learner = new TestLearner()
            {
                MathGrade = "A",
                LearnerFAMs = new List<TestLearnerFAM>()
                {
                    new TestLearnerFAM()
                    {
                        LearnFAMType = "MCF",
                        LearnFAMCode = 2
                    }
                }
            };

            var learnerFAMQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            learnerFAMQueryServiceMock
                .Setup(qs => qs.HasAnyLearnerFAMCodesForType(learner.LearnerFAMs, "MCF", new[] { 2, 3, 4 }))
                .Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learnerFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError()
        {
            var learner = new TestLearner()
            {
                MathGrade = "NONE",
                LearnerFAMs = new List<TestLearnerFAM>()
                {
                    new TestLearnerFAM()
                    {
                        LearnFAMType = "XXX",
                        LearnFAMCode = 2
                    }
                }
            };

            var learnerFAMQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            learnerFAMQueryServiceMock
                .Setup(qs => qs.HasAnyLearnerFAMCodesForType(learner.LearnerFAMs, "MCF", new[] { 2, 3, 4 }))
                .Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learnerFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("MathGrade", "A")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters("A");

            validationErrorHandlerMock.Verify();
        }

        private MathGrade_04Rule NewRule(
            ILearnerFAMQueryService learnerFamQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new MathGrade_04Rule(learnerFamQueryService, validationErrorHandler);
        }
    }
}
