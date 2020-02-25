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
    public class R50RuleTests : AbstractRuleTests<R50Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R50");
        }

        [Fact]
        public void Validate_Null_LearningDeliveries()
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
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 1,
                        ProviderSpecDeliveryMonitorings = new List<IProviderSpecDeliveryMonitoring>()
                        {
                            new TestProviderSpecDeliveryMonitoring()
                            {
                                ProvSpecDelMonOccur = "AB"
                            },
                            new TestProviderSpecDeliveryMonitoring()
                            {
                                ProvSpecDelMonOccur = "C1"
                            },
                            new TestProviderSpecDeliveryMonitoring()
                            {
                                ProvSpecDelMonOccur = "AB"
                            },
                            new TestProviderSpecDeliveryMonitoring()
                            {
                                ProvSpecDelMonOccur = "C1"
                            },
                            new TestProviderSpecDeliveryMonitoring()
                            {
                                ProvSpecDelMonOccur = "XYZ"
                            }
                        }
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 3,
                        ProviderSpecDeliveryMonitorings = new List<IProviderSpecDeliveryMonitoring>()
                        {
                            new TestProviderSpecDeliveryMonitoring()
                            {
                                ProvSpecDelMonOccur = "AB"
                            },
                            new TestProviderSpecDeliveryMonitoring()
                            {
                                ProvSpecDelMonOccur = "AB"
                            },
                            new TestProviderSpecDeliveryMonitoring()
                            {
                                ProvSpecDelMonOccur = "XYZ"
                            }
                        }
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 5
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
                validationErrorHandlerMock.Verify(h => h.Handle(It.IsAny<string>(), It.IsAny<string>(), 1, It.IsAny<IEnumerable<IErrorMessageParameter>>()), Times.Exactly(2));
                validationErrorHandlerMock.Verify(h => h.Handle(It.IsAny<string>(), It.IsAny<string>(), 3, It.IsAny<IEnumerable<IErrorMessageParameter>>()), Times.Once);
                validationErrorHandlerMock.Verify(h => h.Handle(It.IsAny<string>(), It.IsAny<string>(), 5, It.IsAny<IEnumerable<IErrorMessageParameter>>()), Times.Never);
            }
        }

        [Fact]
        public void Validate_Fail_CaseInsensitive()
        {
            var testLearner = new TestLearner()
            {
                LearnRefNumber = "123456789",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 1,
                        ProviderSpecDeliveryMonitorings = new List<IProviderSpecDeliveryMonitoring>()
                        {
                            new TestProviderSpecDeliveryMonitoring()
                            {
                                ProvSpecDelMonOccur = "AB"
                            },
                            new TestProviderSpecDeliveryMonitoring()
                            {
                                ProvSpecDelMonOccur = "C1"
                            },
                            new TestProviderSpecDeliveryMonitoring()
                            {
                                ProvSpecDelMonOccur = "ab"
                            }
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(testLearner);
                validationErrorHandlerMock.Verify(h => h.Handle(It.IsAny<string>(), It.IsAny<string>(), 1, It.IsAny<IEnumerable<IErrorMessageParameter>>()), Times.Once);
            }
        }

        [Fact]
        public void Validate_Pass_Null_ProviderSpecDeliveryMonitoring()
        {
            var testLearner = new TestLearner()
            {
                LearnRefNumber = "123456789",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 1,
                        ProviderSpecDeliveryMonitorings = new List<IProviderSpecDeliveryMonitoring>()
                        {
                            new TestProviderSpecDeliveryMonitoring()
                            {
                                ProvSpecDelMonOccur = null
                            }
                        }
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 3,
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
        public void Validate_Pass()
        {
            var testLearner = new TestLearner()
            {
                LearnRefNumber = "123456789",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 1,
                        ProviderSpecDeliveryMonitorings = new List<IProviderSpecDeliveryMonitoring>()
                        {
                            new TestProviderSpecDeliveryMonitoring()
                            {
                                ProvSpecDelMonOccur = "AB"
                            },
                            new TestProviderSpecDeliveryMonitoring()
                            {
                                ProvSpecDelMonOccur = "C1"
                            },
                            new TestProviderSpecDeliveryMonitoring()
                            {
                                ProvSpecDelMonOccur = "AC"
                            }
                        }
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

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.ProvSpecDelMonOccur, "XYZ")).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters("XYZ");

            validationErrorHandlerMock.Verify();
        }

        public R50Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new R50Rule(validationErrorHandler);
        }
    }
}
