using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R126RuleTests : AbstractRuleTests<R126Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R126");
        }

        [Fact]
        public void FilterAims_Empty()
        {
            var learningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery(),
                new TestLearningDelivery(),
            };

            NewRule().FilterAims(learningDeliveries).Should().BeEmpty();
        }

        [Fact]
        public void FilterAims_Matched()
        {
            var learningDeliveryOne = new TestLearningDelivery()
            {
                ProgTypeNullable = 30,
                AimType = 1,
            };
            var learningDeliveryTwo = new TestLearningDelivery()
            {
                ProgTypeNullable = 31,
                AimType = 1,
            };
            var learningDeliveryThree = new TestLearningDelivery()
            {
                ProgTypeNullable = 31,
                AimType = 3,
            };
            var learningDeliveryFour = new TestLearningDelivery()
            {
                ProgTypeNullable = null,
                AimType = 1,
            };

            var learningDeliveries = new List<ILearningDelivery>()
            {
                learningDeliveryOne,
                learningDeliveryTwo,
                learningDeliveryThree,
                learningDeliveryFour
            };

            var matches = NewRule().FilterAims(learningDeliveries).ToList();

            matches.Should().HaveCount(2);
            matches.SingleOrDefault(x => x.AimType == 1 && x.ProgTypeNullable == 30).Should().NotBeNull();
            matches.SingleOrDefault(x => x.AimType == 1 && x.ProgTypeNullable == 31).Should().NotBeNull();
            matches.SingleOrDefault(x => x.AimType == 3).Should().BeNull();
            matches.SingleOrDefault(x => x.AimType == 1 && !x.ProgTypeNullable.HasValue).Should().BeNull();
        }

        [Fact]
        public void FilterAims_None_Matched()
        {
            var learningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    ProgTypeNullable = 32,
                    AimType = 1,
                },
                new TestLearningDelivery()
                {
                    ProgTypeNullable = null,
                    AimType = 1,
                },
                new TestLearningDelivery()
                {
                    ProgTypeNullable = 31,
                    AimType = 3,
                },
                new TestLearningDelivery()
                {
                    ProgTypeNullable = 30,
                    AimType = 5,
                },
            };

            var matches = NewRule().FilterAims(learningDeliveries).ToList();
            matches.Should().BeEmpty();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var mainLearningDelivery = new TestLearningDelivery()
            {
                ProgTypeNullable = 30,
                AimType = 1,
                FundModel = 36,
            };

            var learningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    ProgTypeNullable = 30,
                    AimType = 3,
                    FundModel = 32
                },
                new TestLearningDelivery()
                {
                    ProgTypeNullable = 31,
                    AimType = 3,
                    FundModel = 36,
                },
                new TestLearningDelivery()
                {
                    ProgTypeNullable = null,
                    AimType = 5,
                    FundModel = 36
                },
                new TestLearningDelivery()
                {
                    ProgTypeNullable = 32,
                    AimType = 5,
                    FundModel = 36
                },
            };

            NewRule().ConditionMet(mainLearningDelivery, learningDeliveries).Should().BeTrue();
        }

        public void ConditionMet_False()
        {
            var mainLearningDelivery = new TestLearningDelivery()
            {
                ProgTypeNullable = 30,
                AimType = 1,
                FundModel = 36,
            };

            var learningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    ProgTypeNullable = 30,
                    AimType = 3,
                    FundModel = 32
                },
                new TestLearningDelivery()
                {
                    ProgTypeNullable = 30,
                    AimType = 3,
                    FundModel = 36,
                },
                new TestLearningDelivery()
                {
                    ProgTypeNullable = null,
                    AimType = 5,
                    FundModel = 36
                },
            };

            NewRule().ConditionMet(mainLearningDelivery, learningDeliveries).Should().BeFalse();
        }

        [Fact]
        public void Validate_Valid_Null_LearningDeliveries()
        {
            var learner = new TestLearner();
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Theory]
        [InlineData(36, 30, 3)]
        [InlineData(36, 31, 3)]
        [InlineData(36, 30, 5)]
        [InlineData(36, 31, 5)]
        [InlineData(99, 30, 5)]
        [InlineData(77, 30, 3)]
        public void Validate_Valid_LearningDeliveries(int fundModel, int progType, int aimType)
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = progType,
                        AimType = 1,
                        FundModel = fundModel
                    },
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = progType,
                        AimType = aimType,
                        FundModel = fundModel
                    },
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = null,
                        AimType = 3,
                        FundModel = fundModel,
                    },
                }
            };
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Theory]
        [InlineData(10)]
        [InlineData(20)]
        [InlineData(null)]
        public void Validate_Valid_LearningDeliveries_No_MatchingProgType(int? progType)
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = progType,
                        AimType = 1,
                        FundModel = 36
                    },
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = 30,
                        AimType = 4,
                        FundModel = 36
                    },
                }
            };
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_InValid_FundModel_LearningDeliveries()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = 30,
                        AimType = 1,
                        FundModel = 36
                    },
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = 30,
                        AimType = 3,
                        FundModel = 50
                    },
                }
            };
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_InValid_ProgType_LearningDeliveries()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = 30,
                        AimType = 1,
                        FundModel = 36
                    },
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = 33,
                        AimType = 3,
                        FundModel = 36
                    },
                }
            };
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Theory]
        [InlineData(30)]
        [InlineData(31)]
        public void Validate_InValid_AimType_LearningDeliveries(int progType)
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = progType,
                        AimType = 1,
                        FundModel = 36
                    },
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = progType,
                        AimType = 4,
                        FundModel = 36
                    },
                }
            };
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        private R126Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new R126Rule(validationErrorHandler ?? Mock.Of<IValidationErrorHandler>());
        }
    }
}
