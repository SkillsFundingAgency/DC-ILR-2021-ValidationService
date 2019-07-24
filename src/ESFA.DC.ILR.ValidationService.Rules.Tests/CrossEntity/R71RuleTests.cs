using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R71RuleTests : AbstractRuleTests<R71Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R71");
        }

        [Fact]
        public void ValidationPasses()
        {
            var testMessage = new TestMessage()
            {
                LearnerDestinationAndProgressions = new List<ILearnerDestinationAndProgression>
                {
                    new TestLearnerDestinationAndProgression
                    {
                        LearnRefNumber = "12345"
                    },
                    new TestLearnerDestinationAndProgression
                    {
                        LearnRefNumber = "123456"
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(testMessage);
            }
        }

        [Fact]
        public void ValidationFails()
        {
            var testMessage = new TestMessage()
            {
                LearnerDestinationAndProgressions = new List<ILearnerDestinationAndProgression>
                {
                    new TestLearnerDestinationAndProgression
                    {
                        LearnRefNumber = "0r71" // fails due to string i.e. 0r71 & 0R71
                    },
                    new TestLearnerDestinationAndProgression
                    {
                        LearnRefNumber = "0R71"
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(testMessage);
            }
        }

        private R71Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new R71Rule(validationErrorHandler);
        }
    }
}
