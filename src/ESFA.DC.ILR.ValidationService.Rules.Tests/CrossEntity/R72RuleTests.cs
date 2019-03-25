using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R72RuleTests : AbstractRuleTests<R72Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R72");
        }

        [Fact]
        public void Validate_Null_LearningDelivery_True()
        {
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(new TestLearner());
            }
        }

        [Fact]
        public void ConditionMet_True()
        {
            var standardLearningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    StdCodeNullable = 1,
                    AppFinRecords = new List<IAppFinRecord>()
                    {
                        new TestAppFinRecord()
                        {
                            AFinCode = 1,
                            AFinType = "TNP",
                            AFinAmount = 10,
                        },
                        new TestAppFinRecord()
                        {
                            AFinCode = 2,
                            AFinType = "TNP",
                            AFinAmount = 5,
                        }
                    }
                }
            };

            var learningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    StdCodeNullable = 20
                }
            };

            learningDeliveries.AddRange(standardLearningDeliveries);

            var dd17Mock = new Mock<IDerivedData_17Rule>();
            dd17Mock.Setup(x => x.IsTotalNegotiatedPriceMoreThanCapForStandard(standardLearningDeliveries, 1)).Returns(false);

            var learningDeliveryAppFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            learningDeliveryAppFinRecordQueryServiceMock.Setup(x =>
                x.GetTotalTNPPriceForLatestAppFinRecordsForLearning(standardLearningDeliveries)).Returns(10);

            NewRule(null, learningDeliveryAppFinRecordQueryServiceMock.Object, dd17Mock.Object).ConditionMet(1, 20, learningDeliveries).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_NullPMR()
        {
            NewRule().ConditionMet(1, null, null).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_DD17True()
        {
            var standardLearningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    StdCodeNullable = 1,
                    ProgTypeNullable = 25,
                    FundModel = 81,
                    AimType = 1,
                    AppFinRecords = new List<IAppFinRecord>()
                    {
                        new TestAppFinRecord()
                        {
                            AFinCode = 1,
                            AFinType = "TNP",
                            AFinAmount = 10,
                        },
                        new TestAppFinRecord()
                        {
                            AFinCode = 2,
                            AFinType = "TNP",
                            AFinAmount = 5,
                        }
                    }
                }
            };

            var dd17Mock = new Mock<IDerivedData_17Rule>();
            dd17Mock.Setup(x => x.IsTotalNegotiatedPriceMoreThanCapForStandard(standardLearningDeliveries, 1)).Returns(true);

            var learningDeliveryAppFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            learningDeliveryAppFinRecordQueryServiceMock.Setup(x =>
                x.GetTotalTNPPriceForLatestAppFinRecordsForLearning(standardLearningDeliveries)).Returns(10);

            NewRule(null, learningDeliveryAppFinRecordQueryServiceMock.Object, dd17Mock.Object).ConditionMet(1, 20, standardLearningDeliveries).Should().BeFalse();
            learningDeliveryAppFinRecordQueryServiceMock.Verify(x => x.GetTotalTNPPriceForLatestAppFinRecordsForLearning(It.IsAny<List<ILearningDelivery>>()), Times.Never);
        }

        [Fact]
        public void ConditionMet_False_Amount()
        {
            var standardLearningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    StdCodeNullable = 1,
                    ProgTypeNullable = 25,
                    FundModel = 81,
                    AimType = 1,
                    AppFinRecords = new List<IAppFinRecord>()
                    {
                        new TestAppFinRecord()
                        {
                            AFinCode = 1,
                            AFinType = "TNP",
                            AFinAmount = 10,
                        },
                        new TestAppFinRecord()
                        {
                            AFinCode = 2,
                            AFinType = "TNP",
                            AFinAmount = 5,
                        }
                    }
                }
            };

            var learningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    StdCodeNullable = 20,
                    ProgTypeNullable = 25,
                    FundModel = 81,
                    AimType = 1,
                    AppFinRecords = new List<IAppFinRecord>()
                    {
                        new TestAppFinRecord()
                        {
                            AFinCode = 1,
                            AFinType = "TNP",
                            AFinAmount = 10,
                        }
                    }
                }
            };

            learningDeliveries.AddRange(standardLearningDeliveries);

            var dd17Mock = new Mock<IDerivedData_17Rule>();
            dd17Mock.Setup(x => x.IsTotalNegotiatedPriceMoreThanCapForStandard(standardLearningDeliveries, 1)).Returns(false);

            var learningDeliveryAppFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            learningDeliveryAppFinRecordQueryServiceMock.Setup(x =>
                x.GetTotalTNPPriceForLatestAppFinRecordsForLearning(standardLearningDeliveries)).Returns(15);

            NewRule(null, learningDeliveryAppFinRecordQueryServiceMock.Object, dd17Mock.Object).ConditionMet(1, 4, standardLearningDeliveries).Should().BeFalse();
        }

        [Theory]
        [InlineData(20, 100, 81, 1)]
        [InlineData(20, 25, 100, 1)]
        [InlineData(20, 25, 81, 3)]
        public void Validate_False_NoMatchingData(int standardCode, int? progType, int fundModel, int aimType)
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        StdCodeNullable = 20,
                        AimType = TypeOfAim.CoreAim16To19ExcludingApprenticeships,
                        FundModel = 81,
                        ProgTypeNullable = 25
                    },

                    new TestLearningDelivery()
                    {
                        StdCodeNullable = null,
                        AimType = TypeOfAim.ProgrammeAim,
                        FundModel = 81,
                        ProgTypeNullable = 25
                    },
                    new TestLearningDelivery()
                    {
                        StdCodeNullable = standardCode,
                        AimType = aimType,
                        ProgTypeNullable = progType,
                        FundModel = fundModel
                    },
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_True()
        {
            var standardLearningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    StdCodeNullable = 1,
                    ProgTypeNullable = 25,
                    FundModel = 81,
                    AimType = 1,
                    AppFinRecords = new List<IAppFinRecord>()
                    {
                        new TestAppFinRecord()
                        {
                            AFinCode = 1,
                            AFinType = "TNP",
                            AFinAmount = 11,
                        },
                        new TestAppFinRecord()
                        {
                            AFinCode = 2,
                            AFinType = "TNP",
                            AFinAmount = 5,
                        },
                        new TestAppFinRecord()
                        {
                            AFinCode = 3,
                            AFinType = "PMR",
                            AFinAmount = 10,
                        },
                    }
                },

                new TestLearningDelivery()
                {
                    StdCodeNullable = 1,
                    ProgTypeNullable = 25,
                    FundModel = 81,
                    AimType = 1,
                    AppFinRecords = new List<IAppFinRecord>()
                    {
                        new TestAppFinRecord()
                        {
                            AFinCode = 1,
                            AFinType = "PMR",
                            AFinAmount = 10,
                        },
                        new TestAppFinRecord()
                        {
                            AFinCode = 2,
                            AFinType = "pmr",
                            AFinAmount = 5,
                        }
                    }
                },

                new TestLearningDelivery()
                {
                    StdCodeNullable = 100,
                    ProgTypeNullable = 25,
                    FundModel = 81,
                    AimType = 1,
                    AppFinRecords = new List<IAppFinRecord>()
                    {
                        new TestAppFinRecord()
                        {
                            AFinCode = 1,
                            AFinType = "TNP",
                            AFinAmount = 10,
                        },
                        new TestAppFinRecord()
                        {
                            AFinCode = 2,
                            AFinType = "TNP",
                            AFinAmount = 5,
                        }
                    }
                },
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = standardLearningDeliveries
            };

            var dd17Mock = new Mock<IDerivedData_17Rule>();
            dd17Mock.Setup(x => x.IsTotalNegotiatedPriceMoreThanCapForStandard(It.IsAny<List<ILearningDelivery>>(), 1)).Returns(false);

            var learningDeliveryAppFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            learningDeliveryAppFinRecordQueryServiceMock.Setup(x =>
                x.GetTotalTNPPriceForLatestAppFinRecordsForLearning(It.IsAny<List<ILearningDelivery>>())).Returns(16);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, learningDeliveryAppFinRecordQueryServiceMock.Object, dd17Mock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_False()
        {
            var standardLearningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    StdCodeNullable = 1,
                    ProgTypeNullable = 25,
                    FundModel = 81,
                    AimType = 1,
                    AppFinRecords = new List<IAppFinRecord>()
                    {
                        new TestAppFinRecord()
                        {
                            AFinCode = 1,
                            AFinType = "TNP",
                            AFinAmount = 11,
                        },
                        new TestAppFinRecord()
                        {
                            AFinCode = 2,
                            AFinType = "TNP",
                            AFinAmount = 5,
                        },
                        new TestAppFinRecord()
                        {
                            AFinCode = 3,
                            AFinType = "PMR",
                            AFinAmount = 10,
                        },
                    }
                },

                new TestLearningDelivery()
                {
                    StdCodeNullable = 1,
                    ProgTypeNullable = 25,
                    FundModel = 81,
                    AimType = 1,
                    AppFinRecords = new List<IAppFinRecord>()
                    {
                        new TestAppFinRecord()
                        {
                            AFinCode = 1,
                            AFinType = "PMR",
                            AFinAmount = 10,
                        },
                        new TestAppFinRecord()
                        {
                            AFinCode = 2,
                            AFinType = "pmr",
                            AFinAmount = 5,
                        }
                    }
                },

                new TestLearningDelivery()
                {
                    StdCodeNullable = 100,
                    ProgTypeNullable = 25,
                    FundModel = 81,
                    AimType = 1,
                    AppFinRecords = new List<IAppFinRecord>()
                    {
                        new TestAppFinRecord()
                        {
                            AFinCode = 1,
                            AFinType = "TNP",
                            AFinAmount = 5,
                        },
                        new TestAppFinRecord()
                        {
                            AFinCode = 2,
                            AFinType = "TNP",
                            AFinAmount = 5,
                        }
                    }
                },
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = standardLearningDeliveries
            };

            var dd17Mock = new Mock<IDerivedData_17Rule>();
            dd17Mock.Setup(x => x.IsTotalNegotiatedPriceMoreThanCapForStandard(It.IsAny<List<ILearningDelivery>>(), 1)).Returns(false);

            var learningDeliveryAppFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            learningDeliveryAppFinRecordQueryServiceMock.Setup(x =>
                x.GetTotalTNPPriceForLatestAppFinRecordsForLearning(It.IsAny<List<ILearningDelivery>>())).Returns(10);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object, learningDeliveryAppFinRecordQueryServiceMock.Object, dd17Mock.Object).Validate(learner);
                dd17Mock.Verify(x => x.IsTotalNegotiatedPriceMoreThanCapForStandard(It.IsAny<IReadOnlyCollection<ILearningDelivery>>(), It.IsAny<int>()), Times.AtLeastOnce);
                learningDeliveryAppFinRecordQueryServiceMock.Verify(x => x.GetTotalTNPPriceForLatestAppFinRecordsForLearning(It.IsAny<List<ILearningDelivery>>()), Times.AtLeastOnce);
            }
        }

        [Fact]
        public void GetPMRTotalsDictionary_Success()
        {
            var standardLearningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    StdCodeNullable = 1,
                    ProgTypeNullable = 25,
                    FundModel = 81,
                    AimType = 1,
                    AppFinRecords = new List<IAppFinRecord>()
                    {
                        new TestAppFinRecord()
                        {
                            AFinCode = 3,
                            AFinType = "PMR",
                            AFinAmount = 10,
                        },
                        new TestAppFinRecord()
                        {
                            AFinCode = 1,
                            AFinType = "PMR",
                            AFinAmount = 20,
                        },
                    }
                },

                new TestLearningDelivery()
                {
                    StdCodeNullable = 1,
                    ProgTypeNullable = 25,
                    FundModel = 81,
                    AimType = 1,
                    AppFinRecords = new List<IAppFinRecord>()
                    {
                        new TestAppFinRecord()
                        {
                            AFinCode = 1,
                            AFinType = "PMR",
                            AFinAmount = 10,
                        },
                        new TestAppFinRecord()
                        {
                            AFinCode = 2,
                            AFinType = "pmr",
                            AFinAmount = 5,
                        }
                    }
                },

                new TestLearningDelivery()
                {
                    StdCodeNullable = 100,
                    ProgTypeNullable = 25,
                    FundModel = 81,
                    AimType = 1,
                },
            };

            var result = NewRule().GetPMRTotalsDictionary(standardLearningDeliveries);
            result.Should().NotBeEmpty();
            result[1].Should().Be(25);
            result[100].Should().Be(0);
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnRefNumber, "Test1")).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.AimType, 1)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.ProgType, 25)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.StdCode, 3)).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters("Test1", 3);
            validationErrorHandlerMock.Verify();
        }

        private R72Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            ILearningDeliveryAppFinRecordQueryService learningDeliveryAppFinRecordQueryService = null,
            IDerivedData_17Rule dd17 = null)
        {
            return new R72Rule(validationErrorHandler, learningDeliveryAppFinRecordQueryService, dd17);
        }
    }
}
