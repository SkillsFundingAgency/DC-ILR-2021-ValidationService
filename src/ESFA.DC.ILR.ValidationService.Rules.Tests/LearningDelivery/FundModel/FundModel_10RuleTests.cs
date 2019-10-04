using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.FundModel;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.FundModel
{
    public class FundModel_10RuleTests : AbstractRuleTests<FundModel_10Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("FundModel_10");
        }

        [Theory]
        [InlineData(10, false)]
        [InlineData(35, false)]
        [InlineData(36, true)]
        [InlineData(70, true)]
        [InlineData(25, true)]
        public void FundModelConditionMet(int fundModel, bool expectedResult)
        {
            var actualResult = NewRule().FundModelConditionMet(fundModel);
            Assert.Equal(expectedResult, actualResult);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(35)]
        public void FundModelConditionMet_False(int fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeFalse();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(36)]
        [InlineData(70)]
        public void FundModelConditionMet_True(int fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeTrue();
        }

        [Fact]
        public void DD35ConditionMet_True()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                {
                    new TestLearningDeliveryFAM
                    {
                        LearnDelFAMType = "SOF",
                        LearnDelFAMCode = "112"
                    }
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    learningDelivery
                }
            };

            var dd35Mock = new Mock<IDerivedData_35Rule>();

            dd35Mock.Setup(dd => dd.IsCombinedAuthorities(learningDelivery)).Returns(true);

            NewRule(dd35Mock.Object).DD35ConditionMet(learningDelivery).Should().BeTrue();
        }

        [Fact]
        public void DD35ConditionMet_False()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                {
                    new TestLearningDeliveryFAM
                    {
                        LearnDelFAMType = "SOF",
                        LearnDelFAMCode = "105"
                    }
                }
            };

            var dd35Mock = new Mock<IDerivedData_35Rule>();

            dd35Mock.Setup(dd => dd.IsCombinedAuthorities(learningDelivery)).Returns(false);

            NewRule(dd35Mock.Object).DD35ConditionMet(learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void DD35ConditionMet_False_No_LDFAM()
        {
            var learningDelivery = new TestLearningDelivery()
            {
            };

            var dd35Mock = new Mock<IDerivedData_35Rule>();

            dd35Mock.Setup(dd => dd.IsCombinedAuthorities(learningDelivery)).Returns(false);

            NewRule(dd35Mock.Object).DD35ConditionMet(learningDelivery).Should().BeFalse();
        }

        [Theory]
        [InlineData(1, "112")]
        [InlineData(36, "111")]
        [InlineData(70, "113")]
        public void ConditionMet_True(int fundModel, string learnDelFAMCode)
        {
            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = fundModel,
                LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                {
                    new TestLearningDeliveryFAM
                    {
                        LearnDelFAMType = "SOF",
                        LearnDelFAMCode = learnDelFAMCode
                    }
                }
            };

            var dd35Mock = new Mock<IDerivedData_35Rule>();

            dd35Mock.Setup(dd => dd.IsCombinedAuthorities(learningDelivery)).Returns(true);

            NewRule(dd35Mock.Object).ConditionMet(learningDelivery).Should().BeTrue();
        }

        [Theory]
        [InlineData(35, "112")]
        [InlineData(10, "111")]
        [InlineData(36, "105")]
        public void ConditionMet_False(int fundModel, string learnDelFAMCode)
        {
            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = fundModel,
                LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                {
                    new TestLearningDeliveryFAM
                    {
                        LearnDelFAMType = "SOF",
                        LearnDelFAMCode = learnDelFAMCode
                    }
                }
            };

            var dd35Mock = new Mock<IDerivedData_35Rule>();

            dd35Mock.Setup(dd => dd.IsCombinedAuthorities(learningDelivery)).Returns(false);

            NewRule(dd35Mock.Object).ConditionMet(learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error_When_Condition_Met()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = 36,
                LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                {
                    new TestLearningDeliveryFAM
                    {
                        LearnDelFAMType = "SOF",
                        LearnDelFAMCode = "112"
                    }
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    learningDelivery
                }
            };

            var dd35Mock = new Mock<IDerivedData_35Rule>();

            dd35Mock.Setup(dd => dd.IsCombinedAuthorities(learningDelivery)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(dd35Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Error_False_FundModel()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = 35,
                LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                {
                    new TestLearningDeliveryFAM
                    {
                        LearnDelFAMType = "SOF",
                        LearnDelFAMCode = "112"
                    }
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    learningDelivery
                }
            };

            var dd35Mock = new Mock<IDerivedData_35Rule>();

            dd35Mock.Setup(dd => dd.IsCombinedAuthorities(learningDelivery)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dd35Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Error_False_SOF()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = 35,
                LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                {
                    new TestLearningDeliveryFAM
                    {
                        LearnDelFAMType = "SOF",
                        LearnDelFAMCode = "105"
                    }
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    learningDelivery
                }
            };

            var dd35Mock = new Mock<IDerivedData_35Rule>();

            dd35Mock.Setup(dd => dd.IsCombinedAuthorities(learningDelivery)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dd35Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Error_False_SOF_MISSING()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = 35,
                LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                {
                    new TestLearningDeliveryFAM
                    {
                        LearnDelFAMType = "RES",
                        LearnDelFAMCode = "1"
                    }
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    learningDelivery
                }
            };

            var dd35Mock = new Mock<IDerivedData_35Rule>();

            dd35Mock.Setup(dd => dd.IsCombinedAuthorities(learningDelivery)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dd35Mock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnDelFAMType", "SOF")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnDelFAMCode", "105")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("FundModel", 10)).Verifiable();

            NewRule(new Mock<IDerivedData_35Rule>().Object, validationErrorHandlerMock.Object).BuildErrorMessageParameters(10, "105");

            validationErrorHandlerMock.Verify();
        }

        private FundModel_10Rule NewRule(
            IDerivedData_35Rule dd35 = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new FundModel_10Rule(dd35, validationErrorHandler);
        }
    }
}
