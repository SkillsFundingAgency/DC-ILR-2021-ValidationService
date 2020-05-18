using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R59RuleTests : AbstractRuleTests<R59Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R59");
        }

        [Fact]
        public void Validate_Pass_DuplicateTempUln()
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();
            var testMessage = new TestMessage()
            {
                Learners = new List<ILearner>
                {
                    new TestLearner()
                    {
                        ULN = ValidationConstants.TemporaryULN
                    },
                    new TestLearner()
                    {
                        ULN = ValidationConstants.TemporaryULN
                    }
                }
            };

            NewRule(validationErrorHandlerMock.Object).Validate(testMessage);
        }

        [Fact]
        public void Validate_Pass()
        {
            var testMessage = new TestMessage()
            {
                Learners = new List<ILearner>
                {
                    new TestLearner()
                    {
                        ULN = 899999
                    },
                    new TestLearner()
                    {
                        ULN = 100000
                    },
                    new TestLearner()
                    {
                        ULN = ValidationConstants.TemporaryULN
                    },
                    new TestLearner()
                    {
                        ULN = ValidationConstants.TemporaryULN
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(testMessage);
                validationErrorHandlerMock.Verify(h => h.Handle(RuleNameConstants.R59, It.IsAny<string>(), It.IsAny<int>(), It.IsAny<IEnumerable<IErrorMessageParameter>>()), Times.Never);
            }
        }

        [Fact]
        public void Validate_Fail()
        {
            var testMessage = new TestMessage()
            {
                Learners = new List<ILearner>
                {
                    new TestLearner()
                    {
                        ULN = 899999,
                        LearnRefNumber = "test1",
                    },
                    new TestLearner()
                    {
                        ULN = 899999,
                        LearnRefNumber = "test2",
                    },
                    new TestLearner()
                    {
                        ULN = ValidationConstants.TemporaryULN
                    },
                    new TestLearner()
                    {
                        ULN = ValidationConstants.TemporaryULN
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(testMessage);
                validationErrorHandlerMock.Verify(h => h.Handle(RuleNameConstants.R59, "test1", null, It.IsAny<IEnumerable<IErrorMessageParameter>>()), Times.Once);
                validationErrorHandlerMock.Verify(h => h.Handle(RuleNameConstants.R59, "test2", null, It.IsAny<IEnumerable<IErrorMessageParameter>>()), Times.Once);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.UKPRN, (long)546)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.ULN, (long)123467)).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(546, 123467);

            validationErrorHandlerMock.Verify();
        }

        private R59Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new R59Rule(validationErrorHandler);
        }
    }
}
