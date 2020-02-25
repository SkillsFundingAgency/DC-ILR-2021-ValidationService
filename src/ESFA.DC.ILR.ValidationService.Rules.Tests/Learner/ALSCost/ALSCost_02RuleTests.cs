using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.ALSCost;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.ALSCost
{
    public class ALSCost_02RuleTests : AbstractRuleTests<ALSCost_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("ALSCost_02");
        }

        [Fact]
        public void Validate_Null_Learner()
        {
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(null);
            }
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learnerFamQueryService = new Mock<ILearnerFAMQueryService>();
            learnerFamQueryService
                .Setup(x => x.HasLearnerFAMType(It.IsAny<IEnumerable<ILearnerFAM>>(), It.IsAny<string>()))
                .Returns(false);

            NewRule(null, learnerFamQueryService.Object).ConditionMet(10, new List<ILearnerFAM>()).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_AlsCost_Null()
        {
            NewRule(null, null).ConditionMet(null, new List<ILearnerFAM>()).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var learnerFamQueryService = new Mock<ILearnerFAMQueryService>();
            learnerFamQueryService
                .Setup(x => x.HasLearnerFAMType(It.IsAny<IEnumerable<ILearnerFAM>>(), It.IsAny<string>()))
                .Returns(true);

            NewRule(null, learnerFamQueryService.Object).ConditionMet(10, new List<ILearnerFAM>()).Should().BeFalse();
        }

        [Fact]
        public void Validate_Fail()
        {
            var testLearner = new TestLearner()
            {
                LearnRefNumber = "123456789",
                ALSCostNullable = 1,
                LearnerFAMs = new List<ILearnerFAM>()
                {
                    new TestLearnerFAM()
                    {
                        LearnFAMType = "XYZ"
                    }
                }
            };

            var learnerFamQueryService = new Mock<ILearnerFAMQueryService>();
            learnerFamQueryService
                .Setup(x => x.HasLearnerFAMType(It.IsAny<IEnumerable<ILearnerFAM>>(), It.IsAny<string>()))
                .Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object, learnerFamQueryService.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_Pass()
        {
            var testLearner = new TestLearner()
            {
                LearnRefNumber = "123456789",
                ALSCostNullable = 1,
                LearnerFAMs = new List<ILearnerFAM>()
                {
                    new TestLearnerFAM()
                    {
                        LearnFAMType = "HNS"
                    }
                }
            };

            var learnerFamQueryService = new Mock<ILearnerFAMQueryService>();
            learnerFamQueryService
                .Setup(x => x.HasLearnerFAMType(It.IsAny<IEnumerable<ILearnerFAM>>(), It.IsAny<string>()))
                .Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, learnerFamQueryService.Object).Validate(testLearner);
            }
        }

        private ALSCost_02Rule NewRule(IValidationErrorHandler validationErrorHandler = null, ILearnerFAMQueryService learnerFAMQueryService = null)
        {
            return new ALSCost_02Rule(learnerFAMQueryService, validationErrorHandler);
        }
    }
}
