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
    public class R49RuleTests : AbstractRuleTests<R49Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R49");
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
        public void Validate_Null_TestProviderSpecLearnerMonitorings()
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
                ProviderSpecLearnerMonitorings = new List<IProviderSpecLearnerMonitoring>()
                {
                    new TestProviderSpecLearnerMonitoring()
                    {
                        ProvSpecLearnMonOccur = "xYz"
                    },
                    new TestProviderSpecLearnerMonitoring()
                    {
                        ProvSpecLearnMonOccur = "XYZ"
                    },
                    new TestProviderSpecLearnerMonitoring()
                    {
                        ProvSpecLearnMonOccur = "ABC"
                    },
                    new TestProviderSpecLearnerMonitoring()
                    {
                        ProvSpecLearnMonOccur = "abc"
                    },
                    new TestProviderSpecLearnerMonitoring()
                    {
                        ProvSpecLearnMonOccur = null
                    },
                    new TestProviderSpecLearnerMonitoring()
                    {
                        ProvSpecLearnMonOccur = null
                    },
                    new TestProviderSpecLearnerMonitoring()
                    {
                        ProvSpecLearnMonOccur = "YXXX"
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
                validationErrorHandlerMock.Verify(h => h.Handle(It.IsAny<string>(), It.IsAny<string>(), null, It.IsAny<IEnumerable<IErrorMessageParameter>>()), Times.Exactly(3));
            }
        }

        [Fact]
        public void Validate_Pass()
        {
            var testLearner = new TestLearner()
            {
                LearnRefNumber = "123456789",
                ProviderSpecLearnerMonitorings = new List<IProviderSpecLearnerMonitoring>()
                {
                    new TestProviderSpecLearnerMonitoring()
                    {
                        ProvSpecLearnMonOccur = "xYz"
                    },
                    new TestProviderSpecLearnerMonitoring()
                    {
                        ProvSpecLearnMonOccur = "XxxZ"
                    },
                    new TestProviderSpecLearnerMonitoring()
                    {
                        ProvSpecLearnMonOccur = "ABC1"
                    },
                    new TestProviderSpecLearnerMonitoring()
                    {
                        ProvSpecLearnMonOccur = "abc"
                    },
                    new TestProviderSpecLearnerMonitoring()
                    {
                        ProvSpecLearnMonOccur = null
                    },
                    new TestProviderSpecLearnerMonitoring()
                    {
                        ProvSpecLearnMonOccur = "YXXX"
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
                validationErrorHandlerMock.Verify(h => h.Handle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long?>(), It.IsAny<IEnumerable<IErrorMessageParameter>>()), Times.Never);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.ProvSpecLearnMonOccur, "XYZ")).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters("XYZ");

            validationErrorHandlerMock.Verify();
        }

        public R49Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new R49Rule(validationErrorHandler);
        }
    }
}
