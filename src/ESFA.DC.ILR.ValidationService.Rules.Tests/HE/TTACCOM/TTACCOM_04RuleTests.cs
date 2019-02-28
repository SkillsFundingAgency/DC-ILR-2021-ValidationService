using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.HE.TTACCOM;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.HE.TTACCOM
{
    public class TTACCOM_04RuleTests : AbstractRuleTests<TTACCOM_04Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("TTACCOM_04");
        }

        [Fact]
        public void LearnerHEConditionMet_True_NullTTACCOM()
        {
            NewRule().LearnerHEConditionMet(new TestLearnerHE()).Should().BeTrue();
        }

        [Fact]
        public void LearnerHEConditionMet_False()
        {
            TestLearnerHE learningDeliveryHE = new TestLearnerHE()
            {
                TTACCOMNullable = 1
            };

            NewRule().LearnerHEConditionMet(learningDeliveryHE).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryHEConditionMet_False()
        {
            TestLearningDeliveryHE learningDeliveryHE = new TestLearningDeliveryHE()
            {
                MODESTUD = 2
            };

            NewRule().LearningDeliveryHEConditionMet(learningDeliveryHE).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryHEConditionMet_True()
        {
            TestLearningDeliveryHE learningDeliveryHE = new TestLearningDeliveryHE()
            {
                MODESTUD = 1
            };

            NewRule().LearningDeliveryHEConditionMet(learningDeliveryHE).Should().BeTrue();
        }

        [Fact]
        public void LearnStartDateConditionMet_True()
        {
            var learningDeliveries = new TestLearningDelivery[]
           {
                new TestLearningDelivery()
                {
                    LearnStartDate = new DateTime(2013, 08, 01)
                },
                new TestLearningDelivery()
                {
                    LearnStartDate = new DateTime(2014, 11, 10)
                },
           };
            NewRule(NewDD06()).LearnStartDateConditionMet(learningDeliveries).Should().BeTrue();
        }

        [Fact]
        public void LearnStartDateConditionMet_False()
        {
            var learningDeliveries = new TestLearningDelivery[]
           {
                new TestLearningDelivery()
                {
                    LearnStartDate = new DateTime(2009, 08, 01)
                },
                new TestLearningDelivery()
                {
                    LearnStartDate = new DateTime(2011, 11, 10)
                },
           };
            NewRule(NewDD06()).LearnStartDateConditionMet(learningDeliveries).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            TestLearningDeliveryHE learningDeliveryHE = new TestLearningDeliveryHE()
            {
                MODESTUD = 1
            };

            NewRule(NewDD06()).ConditionMet(learningDeliveryHE).Should().BeTrue();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(2)]
        public void ConditionMet_False(int modeStud)
        {
            TestLearningDeliveryHE learningDeliveryHE = new TestLearningDeliveryHE()
            {
                MODESTUD = modeStud
            };

            NewRule().ConditionMet(learningDeliveryHE).Should().BeFalse();
        }

        [Fact]
        public void Validate_Errors()
        {
            var learner = new TestLearner()
            {
                LearnerHEEntity = new TestLearnerHE() { TTACCOMNullable = null },
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = new DateTime(2013, 08, 1),
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE() { MODESTUD = 1 }
                    }
                }
            };

            var dd06Mock = new Mock<IDerivedData_06Rule>();

            dd06Mock.Setup(dd => dd.Derive(learner.LearningDeliveries)).Returns(new DateTime(2013, 08, 01));
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(dd06Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Theory]
        [InlineData(1, 2, "2013-8-1")]
        [InlineData(null, 2, "2013-8-1")]
        [InlineData(null, 2, "2013-7-31")]
        public void Validate_NoErrors(int? ttACCOM, int mODESTUD, string learnStartDateString)
        {
            DateTime learnStartDate = DateTime.Parse(learnStartDateString);

            var learner = new TestLearner()
            {
                LearnerHEEntity = new TestLearnerHE() { TTACCOMNullable = ttACCOM },
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = learnStartDate,
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE() { MODESTUD = mODESTUD }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(NewDD06(), validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoErrors_NoLearnerHE()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = new DateTime(2013, 08, 1),
                        LearningDeliveryHEEntity = new TestLearningDeliveryHE() { MODESTUD = 1 }
                    }
                }
            };

            var dd06Mock = new Mock<IDerivedData_06Rule>();

            dd06Mock.Setup(dd => dd.Derive(learner.LearningDeliveries)).Returns(new DateTime(2013, 08, 01));
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dd06Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoErrors_NoLearningDeliveryHE()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = new DateTime(2013, 08, 1),
                        LearningDeliveryHEEntity = null
                    }
                }
            };

            var dd06Mock = new Mock<IDerivedData_06Rule>();

            dd06Mock.Setup(dd => dd.Derive(learner.LearningDeliveries)).Returns(new DateTime(2013, 08, 01));
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dd06Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("MODESTUD", 1)).Verifiable();
            NewRule(NewDD06(), validationErrorHandlerMock.Object).BuildErrorMessageParameters(1);

            validationErrorHandlerMock.Verify();
        }

        private TTACCOM_04Rule NewRule(
            IDerivedData_06Rule dd06 = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new TTACCOM_04Rule(dd06, validationErrorHandler);
        }

        private DerivedData_06Rule NewDD06()
        {
            return new DerivedData_06Rule();
        }
    }
}
