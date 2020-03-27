using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.LLDDHealthProb;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.LLDDHealthProb
{
    public class LLDDHealthProb_04RuleTests : AbstractRuleTests<LLDDHealthProb_04Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LLDDHealthProb_04");
        }

        [Fact]
        public void ConditionMet_False_Null()
        {
            var rule = NewRule();
            rule.ConditionMet(2, null).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_Empty()
        {
            var rule = NewRule();
            rule.ConditionMet(2,  new List<ILLDDAndHealthProblem>()).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var rule = NewRule();
            var llddAndHealthProblems = new List<ILLDDAndHealthProblem>()
            {
                new TestLLDDAndHealthProblem() { }
            };
            rule.ConditionMet(2, llddAndHealthProblems).Should().BeTrue();
        }

        [Fact]
        public void Validate_False()
        {
            var learner = new TestLearner()
            {
                LLDDHealthProb = 2
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_True()
        {
            var learner = new TestLearner()
            {
                LLDDHealthProb = 2,

                LLDDAndHealthProblems = new List<ILLDDAndHealthProblem>()
                {
                    new TestLLDDAndHealthProblem() { }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        private LLDDHealthProb_04Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new LLDDHealthProb_04Rule(validationErrorHandler);
        }
    }
}
