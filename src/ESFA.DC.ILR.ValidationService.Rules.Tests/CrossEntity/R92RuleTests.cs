using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R92RuleTests : AbstractRuleTests<R92Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R92");
        }

        [Fact]
        public void DuplicateConRefNumbersForLearnAimRef_OneDuplicate()
        {
            var conRefNumber = "ConRef1";

            var learningDeliveries = new List<TestLearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    AimSeqNumber = 1,
                    LearnAimRef = "ZESF0001",
                    ConRefNumber = conRefNumber
                },
                new TestLearningDelivery()
                {
                    AimSeqNumber = 2,
                    LearnAimRef = "ZESF0001",
                    ConRefNumber = conRefNumber
                }
            };

            NewRule().DuplicateConRefNumbersForLearnAimRef(learningDeliveries).Should().Be(conRefNumber);
        }

        [Fact]
        public void DuplicateConRefNumbersForLearnAimRef_ManyDuplicates()
        {
            var conRefNumber1 = "ConRef1";
            var conRefNumber2 = "ConRef2";

            var learningDeliveries = new List<TestLearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    AimSeqNumber = 1,
                    LearnAimRef = "ZESF0001",
                    ConRefNumber = conRefNumber1
                },
                new TestLearningDelivery()
                {
                    AimSeqNumber = 2,
                    LearnAimRef = "ZESF0001",
                    ConRefNumber = conRefNumber2
                },
                new TestLearningDelivery()
                {
                    AimSeqNumber = 3,
                    LearnAimRef = "ZESF0001",
                    ConRefNumber = conRefNumber1
                },
                new TestLearningDelivery()
                {
                    AimSeqNumber = 4,
                    LearnAimRef = "ZESF0001",
                    ConRefNumber = conRefNumber2
                }
            };

            NewRule().DuplicateConRefNumbersForLearnAimRef(learningDeliveries).Should().Be(conRefNumber1 + ", " + conRefNumber2);
        }

        [Fact]
        public void DuplicateConRefNumbersForLearnAimRef_NoDeliveries()
        {
            NewRule().DuplicateConRefNumbersForLearnAimRef(null).Should().Be(string.Empty);
        }

        [Fact]
        public void DuplicateConRefNumbersForLearnAimRef_NoDuplicate()
        {
            var conRefNumber1 = "ConRef1";
            var conRefNumber2 = "ConRef2";

            var learningDeliveries = new List<TestLearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    AimSeqNumber = 1,
                    LearnAimRef = "ZESF0001",
                    ConRefNumber = conRefNumber1
                },
                new TestLearningDelivery()
                {
                    AimSeqNumber = 2,
                    LearnAimRef = "ZESF0001",
                    ConRefNumber = conRefNumber2
                }
            };

            NewRule().DuplicateConRefNumbersForLearnAimRef(learningDeliveries).Should().Be(string.Empty);
        }

        [Fact]
        public void DuplicateConRefNumbersForLearnAimRef_LearnAimRefMisMatch()
        {
            var conRefNumber1 = "ConRef1";

            var learningDeliveries = new List<TestLearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    AimSeqNumber = 1,
                    LearnAimRef = "ZESF0001",
                    ConRefNumber = conRefNumber1
                },
                new TestLearningDelivery()
                {
                    AimSeqNumber = 2,
                    LearnAimRef = "AimRef",
                    ConRefNumber = conRefNumber1
                }
            };

            NewRule().DuplicateConRefNumbersForLearnAimRef(learningDeliveries).Should().Be(string.Empty);
        }

        [Fact]
        public void Validate_Error_OneDuplicate()
        {
            var conRefNumber1 = "ConRef1";
            var conRefNumber2 = "ConRef2";

            var learningDeliveries = new List<TestLearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    AimSeqNumber = 1,
                    LearnAimRef = "ZESF0001",
                    ConRefNumber = conRefNumber1
                },
                new TestLearningDelivery()
                {
                    AimSeqNumber = 2,
                    LearnAimRef = "ZESF0001",
                    ConRefNumber = conRefNumber1
                },
                new TestLearningDelivery()
                {
                    AimSeqNumber = 3,
                    LearnAimRef = "ZESF0001",
                    ConRefNumber = conRefNumber2
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = learningDeliveries
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Error_ManyDuplicates()
        {
            var conRefNumber1 = "ConRef1";
            var conRefNumber2 = "ConRef2";

            var learningDeliveries = new List<TestLearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    AimSeqNumber = 1,
                    LearnAimRef = "ZESF0001",
                    ConRefNumber = conRefNumber1
                },
                new TestLearningDelivery()
                {
                    AimSeqNumber = 2,
                    LearnAimRef = "ZESF0001",
                    ConRefNumber = conRefNumber2
                },
                new TestLearningDelivery()
                {
                    AimSeqNumber = 3,
                    LearnAimRef = "ZESF0001",
                    ConRefNumber = conRefNumber1
                },
                new TestLearningDelivery()
                {
                    AimSeqNumber = 4,
                    LearnAimRef = "ZESF0001",
                    ConRefNumber = conRefNumber2
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = learningDeliveries
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_NoLearningDeliveries()
        {
            var learner = new TestLearner();

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_NoDuplicates()
        {
            var conRefNumber1 = "ConRef1";
            var conRefNumber2 = "ConRef2";

            var learningDeliveries = new List<TestLearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    AimSeqNumber = 1,
                    LearnAimRef = "ZESF0001",
                    ConRefNumber = conRefNumber1
                },
                new TestLearningDelivery()
                {
                    AimSeqNumber = 2,
                    LearnAimRef = "ZESF0001",
                    ConRefNumber = conRefNumber2
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = learningDeliveries
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_LearnAimRefMisMatch()
        {
            var conRefNumber1 = "ConRef1";
            var conRefNumber2 = "ConRef2";

            var learningDeliveries = new List<TestLearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    AimSeqNumber = 1,
                    LearnAimRef = "ZESF0001",
                    ConRefNumber = conRefNumber1
                },
                new TestLearningDelivery()
                {
                    AimSeqNumber = 2,
                    LearnAimRef = "ZESF0001",
                    ConRefNumber = conRefNumber2
                },
                new TestLearningDelivery()
                {
                    AimSeqNumber = 3,
                    LearnAimRef = "AimRef",
                    ConRefNumber = conRefNumber2
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = learningDeliveries
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnAimRef", "ZESF0001")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("ConRefNumber", "ConRef1")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters("ZESF0001", "ConRef1");

            validationErrorHandlerMock.Verify();
        }

        private R92Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new R92Rule(validationErrorHandler);
        }
    }
}
