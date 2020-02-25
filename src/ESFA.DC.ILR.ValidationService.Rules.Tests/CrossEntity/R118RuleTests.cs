using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R118RuleTests : AbstractRuleTests<R118Rule>
    {
        private readonly string _famCode = "357";
        private readonly string _famType = LearningDeliveryFAMTypeConstants.LDM;

        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R118");
        }

        [Fact]
        public void ProgrammeAimConditionMet_False_NoFams()
        {
            var learningDelivery = new TestLearningDelivery();

            var learningeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDelivery.LearningDeliveryFAMs, _famType, _famCode)).Returns(false);

            NewRule(learningeliveryFAMQueryServiceMock.Object).ProgrammeAimConditionMet(learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void ProgrammeAimConditionMet_False()
        {
            var learningDelivery = new TestLearningDelivery
            {
                LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                {
                    new TestLearningDeliveryFAM
                    {
                        LearnDelFAMType = "LDM",
                        LearnDelFAMCode = "356"
                    }
                }.ToArray()
            };

            var learningeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDelivery.LearningDeliveryFAMs, _famType, _famCode)).Returns(false);

            NewRule(learningeliveryFAMQueryServiceMock.Object).ProgrammeAimConditionMet(learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void ProgrammeAimConditionMet_True()
        {
            var learningDelivery = new TestLearningDelivery
            {
                LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                {
                    new TestLearningDeliveryFAM
                    {
                        LearnDelFAMType = "LDM",
                        LearnDelFAMCode = "357"
                    }
                }.ToArray()
            };

            var learningeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDelivery.LearningDeliveryFAMs, _famType, _famCode)).Returns(true);

            NewRule(learningeliveryFAMQueryServiceMock.Object).ProgrammeAimConditionMet(learningDelivery).Should().BeTrue();
        }

        [Fact]
        public void ComponentAimConditionMet_True_NoAims()
        {
            var learningDeliveries = new List<TestLearningDelivery>();

            NewRule().ComponentAimConditionMet(35, 24, learningDeliveries).Should().BeTrue();
        }

        [Theory]
        [InlineData(2, 4, 24, 24, 35, 35, false)]
        [InlineData(2, 3, 24, 24, 35, 35, true)]
        [InlineData(3, 3, 24, 24, 35, 35, false)]
        [InlineData(3, 3, 25, 24, 35, 35, false)]
        [InlineData(3, 3, 25, 25, 35, 35, false)]
        [InlineData(3, 3, 24, 24, 36, 35, false)]
        [InlineData(3, 3, 24, 24, 35, 36, false)]
        public void ComponentAimConditioMet_True(int aimType1, int aimType2, int? progType1, int? progType2, int fundModel1, int fundModel2, bool mockSingle)
        {
            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    AimType = aimType1,
                    FundModel = fundModel1,
                    ProgTypeNullable = progType1,
                    LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                    {
                        new TestLearningDeliveryFAM
                        {
                            LearnDelFAMType = "LDM",
                            LearnDelFAMCode = "356"
                        }
                    }
                }
            };

            if (mockSingle == false)
            {
                learningDeliveries.Add(
                new TestLearningDelivery
                {
                    AimType = aimType2,
                    FundModel = fundModel2,
                    ProgTypeNullable = progType2,
                    LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                    {
                        new TestLearningDeliveryFAM
                        {
                            LearnDelFAMType = "LDM",
                            LearnDelFAMCode = "356"
                        }
                    }
                });
            }

            var learningDeliveryFams = learningDeliveries.SelectMany(ld => ld.LearningDeliveryFAMs);

            var learningeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, _famType, _famCode)).Returns(false);

            NewRule(learningeliveryFAMQueryServiceMock.Object).ComponentAimConditionMet(35, 24, learningDeliveries).Should().BeTrue();
        }

        [Theory]
        [InlineData(3, 4, 24, 24, 35, 35, true)]
        [InlineData(3, 3, 24, 24, 35, 35, false)]
        public void ComponentAimConditionMet_False(int aimType1, int aimType2, int? progType1, int? progType2, int fundModel1, int fundModel2, bool mockSingle)
        {
            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    AimType = aimType1,
                    FundModel = fundModel1,
                    ProgTypeNullable = progType1,
                    LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                    {
                        new TestLearningDeliveryFAM
                        {
                            LearnDelFAMType = "LDM",
                            LearnDelFAMCode = "357"
                        }
                    }
                }
            };

            if (mockSingle == false)
            {
                learningDeliveries.Add(
                new TestLearningDelivery
                {
                    AimType = aimType2,
                    FundModel = fundModel2,
                    ProgTypeNullable = progType2,
                    LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                    {
                        new TestLearningDeliveryFAM
                        {
                            LearnDelFAMType = "LDM",
                            LearnDelFAMCode = "357"
                        }
                    }
                });
            }

            var learningDeliveryFams = learningDeliveries.SelectMany(ld => ld.LearningDeliveryFAMs);

            var learningeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, _famType, _famCode)).Returns(true);

            NewRule(learningeliveryFAMQueryServiceMock.Object).ComponentAimConditionMet(35, 24, learningDeliveries).Should().BeFalse();
        }

        [Theory]
        [InlineData(1, 3)]
        [InlineData(3, 3)]
        public void ConditionMet_True(int aimType1, int aimType2)
        {
            var learningDelivery = new TestLearningDelivery
            {
                AimType = aimType1,
                FundModel = 35,
                ProgTypeNullable = 24
            };

            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    AimType = aimType2,
                    FundModel = 35,
                    ProgTypeNullable = 24,
                    LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                    {
                        new TestLearningDeliveryFAM
                        {
                            LearnDelFAMType = "LDM",
                            LearnDelFAMCode = "356"
                        }
                    }
                }
            };

            var rule = NewRuleMock();

            rule.Setup(r => r.ProgrammeAimConditionMet(learningDelivery)).Returns(true);
            rule.Setup(r => r.ComponentAimConditionMet(learningDelivery.FundModel, learningDelivery.ProgTypeNullable, learningDeliveries)).Returns(true);

            rule.Object.ConditionMet(learningDelivery, learningDeliveries).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var learningDelivery = new TestLearningDelivery
            {
                AimType = 1,
                FundModel = 35,
                ProgTypeNullable = 24
            };

            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    AimType = 3,
                    FundModel = 35,
                    ProgTypeNullable = 24,
                    LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                    {
                        new TestLearningDeliveryFAM
                        {
                            LearnDelFAMType = "LDM",
                            LearnDelFAMCode = "357"
                        }
                    }
                }
            };

            var rule = NewRuleMock();

            rule.Setup(r => r.ProgrammeAimConditionMet(learningDelivery)).Returns(true);
            rule.Setup(r => r.ComponentAimConditionMet(learningDelivery.FundModel, learningDelivery.ProgTypeNullable, learningDeliveries)).Returns(false);

            rule.Object.ConditionMet(learningDelivery, learningDeliveries).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_Null()
        {
            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    AimType = 3,
                    FundModel = 35,
                    ProgTypeNullable = 24,
                    LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                    {
                        new TestLearningDeliveryFAM
                        {
                            LearnDelFAMType = "LDM",
                            LearnDelFAMCode = "357"
                        }
                    }
                }
            };

            var rule = NewRuleMock();

            rule.Setup(r => r.ProgrammeAimConditionMet(null)).Returns(false);

            rule.Object.ConditionMet(null, learningDeliveries).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner
            {
                LearnRefNumber = "1",
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        AimSeqNumber = 1,
                        AimType = 1,
                        FundModel = 35,
                        ProgTypeNullable = 24,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = "LDM",
                                LearnDelFAMCode = "357"
                            }
                        }.ToArray()
                    },
                    new TestLearningDelivery
                    {
                        AimSeqNumber = 2,
                        AimType = 3,
                        FundModel = 35,
                        ProgTypeNullable = 24,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = "LDM",
                                LearnDelFAMCode = "356"
                            }
                        }.ToArray()
                    }
                }.ToArray()
            };

            var progAimFAMs = learner.LearningDeliveries.Where(ld => ld.AimSeqNumber == 1 && ld.LearningDeliveryFAMs != null).SelectMany(ld => ld.LearningDeliveryFAMs);
            var compAimFAMs = learner.LearningDeliveries.Where(ld => ld.AimSeqNumber == 2 && ld.LearningDeliveryFAMs != null).SelectMany(ld => ld.LearningDeliveryFAMs);

            var learningeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(progAimFAMs, _famType, _famCode)).Returns(true);
            learningeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(compAimFAMs, _famType, _famCode)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learningeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Error_NoFams()
        {
            var learner = new TestLearner
            {
                LearnRefNumber = "1",
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        AimSeqNumber = 1,
                        AimType = 1,
                        FundModel = 35,
                        ProgTypeNullable = 24,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = "LDM",
                                LearnDelFAMCode = "357"
                            }
                        }.ToArray()
                    },
                    new TestLearningDelivery
                    {
                        AimSeqNumber = 2,
                        AimType = 3,
                        FundModel = 35,
                        ProgTypeNullable = 24,
                    }
                }.ToArray()
            };

            var progAimFAMs = learner.LearningDeliveries.Where(ld => ld.AimSeqNumber == 1 && ld.LearningDeliveryFAMs != null).SelectMany(ld => ld.LearningDeliveryFAMs);

            var learningeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(progAimFAMs, _famType, _famCode)).Returns(true);
            learningeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(null, _famType, _famCode)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learningeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learner = new TestLearner
            {
                LearnRefNumber = "1",
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        AimSeqNumber = 1,
                        AimType = 1,
                        FundModel = 35,
                        ProgTypeNullable = 24,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = "LDM",
                                LearnDelFAMCode = "357"
                            }
                        }.ToArray()
                    },
                    new TestLearningDelivery
                    {
                        AimSeqNumber = 2,
                        AimType = 3,
                        FundModel = 35,
                        ProgTypeNullable = 24,
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = "LDM",
                                LearnDelFAMCode = "357"
                            }
                        }.ToArray()
                    }
                }.ToArray()
            };

            var progAimFAMs = learner.LearningDeliveries.Where(ld => ld.AimSeqNumber == 1 && ld.LearningDeliveryFAMs != null).SelectMany(ld => ld.LearningDeliveryFAMs);
            var compAimFAMs = learner.LearningDeliveries.Where(ld => ld.AimSeqNumber == 2 && ld.LearningDeliveryFAMs != null).SelectMany(ld => ld.LearningDeliveryFAMs);

            var learningeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(progAimFAMs, _famType, _famCode)).Returns(true);
            learningeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMCodeForType(compAimFAMs, _famType, _famCode)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learningeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            int aimType = 1;
            int fundModel = 35;
            int? progType = 24;

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("AimType", aimType)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("FundModel", fundModel)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("ProgType", progType)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnDelFAMCode", _famCode)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnDelFAMType", _famType)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(aimType, fundModel, progType, _famType, _famCode);

            validationErrorHandlerMock.Verify();
        }

        public R118Rule NewRule(ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new R118Rule(learningDeliveryFAMQueryService, validationErrorHandler);
        }
    }
}
