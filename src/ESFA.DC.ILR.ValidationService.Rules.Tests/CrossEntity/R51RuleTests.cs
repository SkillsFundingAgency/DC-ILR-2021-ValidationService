using System.Collections.Generic;
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
    public class R51RuleTests : AbstractRuleTests<R51Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R51");
        }

        [Fact]
        public void Validate_Null_LearningFams()
        {
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(new TestLearner());
            }
        }

        [Fact]
        public void Validate_Fail()
        {
            var testLearner = new TestLearner()
            {
                LearnRefNumber = "123456789",
                LearnerFAMs = new TestLearnerFAM[]
                {
                    new TestLearnerFAM()
                    {
                        LearnFAMType = "XYZ",
                        LearnFAMCode = 1
                    },
                    new TestLearnerFAM()
                    {
                        LearnFAMType = "Xyz",
                        LearnFAMCode = 1
                    },
                    new TestLearnerFAM()
                    {
                        LearnFAMType = "XYZ",
                        LearnFAMCode = 2
                    },
                    new TestLearnerFAM()
                    {
                        LearnFAMType = "XXXX",
                        LearnFAMCode = 2
                    },
                    new TestLearnerFAM()
                    {
                        LearnFAMType = null,
                        LearnFAMCode = 0
                    },
                    new TestLearnerFAM()
                    {
                        LearnFAMType = null,
                        LearnFAMCode = 0
                    },
                    new TestLearnerFAM()
                    {
                        LearnFAMType = null,
                        LearnFAMCode = 1
                    },
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
                validationErrorHandlerMock.Verify(h => h.Handle(It.IsAny<string>(), It.IsAny<string>(), null, It.IsAny<IEnumerable<IErrorMessageParameter>>()), Times.Exactly(2));
            }
        }

        [Fact]
        public void Validate_Pass()
        {
            var testLearner = new TestLearner()
            {
                LearnRefNumber = "123456789",
                LearnerFAMs = new TestLearnerFAM[]
               {
                    new TestLearnerFAM()
                    {
                        LearnFAMType = "XYZ",
                        LearnFAMCode = 99
                    },
                    new TestLearnerFAM()
                    {
                        LearnFAMType = "Xyz",
                        LearnFAMCode = 1
                    },
                    new TestLearnerFAM()
                    {
                        LearnFAMType = "XYZ",
                        LearnFAMCode = 888
                    },
                    new TestLearnerFAM()
                    {
                        LearnFAMType = "XXXX",
                        LearnFAMCode = 1
                    },
                    new TestLearnerFAM()
                    {
                        LearnFAMType = null,
                        LearnFAMCode = 3
                    },
                    new TestLearnerFAM()
                    {
                        LearnFAMType = null,
                        LearnFAMCode = 0
                    },
                    new TestLearnerFAM()
                    {
                        LearnFAMType = null,
                        LearnFAMCode = 4
                    },
               }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
                validationErrorHandlerMock.Verify(h => h.Handle(It.IsAny<string>(), It.IsAny<string>(), null, It.IsAny<IEnumerable<IErrorMessageParameter>>()), Times.Never);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnFAMType, "XYZ")).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnFAMCode, 2)).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters("XYZ", 2);

            validationErrorHandlerMock.Verify();
        }

        public R51Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new R51Rule(validationErrorHandler);
        }
    }
}
