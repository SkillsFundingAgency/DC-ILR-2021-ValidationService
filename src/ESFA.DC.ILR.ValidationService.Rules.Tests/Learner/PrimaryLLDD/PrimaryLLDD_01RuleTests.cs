using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.PrimaryLLDD;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.PrimaryLLDD
{
    public class PrimaryLLDD_01RuleTests : AbstractRuleTests<PrimaryLLDD_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("PrimaryLLDD_01");
        }

        [Fact]
        public void LLDDHealthProbConditionMet_True()
        {
            NewRule().LLDDHealthProbConditionMet(1).Should().BeTrue();
        }

        [Fact]
        public void LLDDHealthProbConditionMet_False()
        {
            NewRule().LLDDHealthProbConditionMet(20).Should().BeFalse();
        }

        [Fact]
        public void LearnStartDateConditionMet_True()
        {
            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    LearnStartDate = new DateTime(2018, 01, 01)
                },
                new TestLearningDelivery
                {
                    LearnStartDate = new DateTime(2015, 08, 01)
                }
            };

            var dd06Mock = new Mock<IDerivedData_06Rule>();

            dd06Mock.Setup(dd => dd.Derive(learningDeliveries)).Returns(new DateTime(2015, 08, 01));

            NewRule(dd06Mock.Object).LearnStartDateConditionMet(learningDeliveries).Should().BeTrue();
        }

        [Fact]
        public void LearnStartDateConditionMet_False()
        {
            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    LearnStartDate = new DateTime(2018, 01, 01)
                },
                new TestLearningDelivery
                {
                    LearnStartDate = new DateTime(2009, 08, 01)
                }
            };

            var dd06Mock = new Mock<IDerivedData_06Rule>();

            dd06Mock.Setup(dd => dd.Derive(learningDeliveries)).Returns(new DateTime(2009, 08, 01));

            NewRule(dd06Mock.Object).LearnStartDateConditionMet(learningDeliveries).Should().BeFalse();
        }

        [Fact]
        public void LLDDConditionMet_True()
        {
            var llddAndHealthProblems = new List<TestLLDDAndHealthProblem>
            {
                new TestLLDDAndHealthProblem
                {
                    LLDDCat = 20,
                },
                new TestLLDDAndHealthProblem
                {
                    LLDDCat = 98
                }
            };

            NewRule().LLDDConditionMet(llddAndHealthProblems).Should().BeTrue();
        }

        [Fact]
        public void LLDDConditionMet_False_Null()
        {
            NewRule().LLDDConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void LLDDConditionMet_False_AllExcluded()
        {
            var llddAndHealthProblems = new List<TestLLDDAndHealthProblem>
            {
                new TestLLDDAndHealthProblem
                {
                    LLDDCat = 98,
                },
                new TestLLDDAndHealthProblem
                {
                    LLDDCat = 99
                }
            };

            NewRule().LLDDConditionMet(llddAndHealthProblems).Should().BeFalse();
        }

        [Fact]
        public void LLDDConditionMet_False()
        {
            var llddAndHealthProblems = new List<TestLLDDAndHealthProblem>
            {
                new TestLLDDAndHealthProblem
                {
                    LLDDCat = 20,
                    PrimaryLLDDNullable = 1
                },
                new TestLLDDAndHealthProblem
                {
                    LLDDCat = 99
                }
            };

            NewRule().LLDDConditionMet(llddAndHealthProblems).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var lldHealthProb = 1;

            var llddAndHealthProblems = new List<TestLLDDAndHealthProblem>
            {
                new TestLLDDAndHealthProblem
                {
                    LLDDCat = 20,
                },
                new TestLLDDAndHealthProblem
                {
                    LLDDCat = 98
                }
            };

            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    LearnStartDate = new DateTime(2018, 01, 01)
                },
                new TestLearningDelivery
                {
                    LearnStartDate = new DateTime(2015, 08, 01)
                }
            };

            var dd06Mock = new Mock<IDerivedData_06Rule>();

            dd06Mock.Setup(dd => dd.Derive(learningDeliveries)).Returns(new DateTime(2015, 08, 01));

            NewRule(dd06Mock.Object).ConditionMet(lldHealthProb, llddAndHealthProblems, learningDeliveries).Should().BeTrue();
        }

        [Theory]
        [InlineData(2, 20, 98, null, 2018, 2015)]
        [InlineData(1, 20, 98, null, 2018, 2014)]
        [InlineData(1, 99, 98, null, 2018, 2015)]
        [InlineData(1, 20, 98, 1, 2018, 2015)]
        public void ConditionMet_False(int llddHealthProb, int llddCat1, int llddCat2, int? primaryLLDD, int year1, int year2)
        {
            var llddAndHealthProblems = new List<TestLLDDAndHealthProblem>
            {
                new TestLLDDAndHealthProblem
                {
                    LLDDCat = llddCat1,
                    PrimaryLLDDNullable = primaryLLDD
                },
                new TestLLDDAndHealthProblem
                {
                    LLDDCat = llddCat2
                }
            };

            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    LearnStartDate = new DateTime(year1, 01, 01)
                },
                new TestLearningDelivery
                {
                    LearnStartDate = new DateTime(year2, 08, 01)
                }
            };

            var dd06Mock = new Mock<IDerivedData_06Rule>();

            dd06Mock.Setup(dd => dd.Derive(learningDeliveries)).Returns(new DateTime(year2, 08, 01));

            NewRule(dd06Mock.Object).ConditionMet(llddHealthProb, llddAndHealthProblems, learningDeliveries).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var llddHealthProb = 1;
            var llddAndHealthProblems = new List<TestLLDDAndHealthProblem>
            {
                new TestLLDDAndHealthProblem
                {
                    LLDDCat = 20
                },
                new TestLLDDAndHealthProblem
                {
                    LLDDCat = 98
                }
            };

            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    LearnStartDate = new DateTime(2018, 01, 01)
                },
                new TestLearningDelivery
                {
                    LearnStartDate = new DateTime(2015, 08, 01)
                }
            };

            var learner = new TestLearner
            {
                LLDDHealthProb = llddHealthProb,
                LLDDAndHealthProblems = llddAndHealthProblems,
                LearningDeliveries = learningDeliveries
            };

            var dd06Mock = new Mock<IDerivedData_06Rule>();

            dd06Mock.Setup(dd => dd.Derive(learningDeliveries)).Returns(new DateTime(2015, 08, 01));

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(dd06Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var llddHealthProb = 1;
            var llddAndHealthProblems = new List<TestLLDDAndHealthProblem>
            {
                new TestLLDDAndHealthProblem
                {
                    LLDDCat = 99
                },
                new TestLLDDAndHealthProblem
                {
                    LLDDCat = 98
                }
            };

            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    LearnStartDate = new DateTime(2018, 01, 01)
                },
                new TestLearningDelivery
                {
                    LearnStartDate = new DateTime(2015, 08, 01)
                }
            };

            var learner = new TestLearner
            {
                LLDDHealthProb = llddHealthProb,
                LLDDAndHealthProblems = llddAndHealthProblems,
                LearningDeliveries = learningDeliveries
            };

            var dd06Mock = new Mock<IDerivedData_06Rule>();

            dd06Mock.Setup(dd => dd.Derive(learningDeliveries)).Returns(new DateTime(2015, 08, 01));

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dd06Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        private PrimaryLLDD_01Rule NewRule(IDerivedData_06Rule dd06 = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new PrimaryLLDD_01Rule(dd06, validationErrorHandler);
        }
    }
}