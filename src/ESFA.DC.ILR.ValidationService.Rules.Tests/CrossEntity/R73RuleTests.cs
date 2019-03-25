using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
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
    public class R73RuleTests : AbstractRuleTests<R73Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R73");
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
                    LearnStartDate = new DateTime(2017, 10, 10),
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
                    LearnStartDate = new DateTime(2017, 10, 10)
                }
            };

            learningDeliveries.AddRange(standardLearningDeliveries);

            var dd17Mock = new Mock<IDerivedData_17Rule>();
            dd17Mock.Setup(x => x.IsTotalNegotiatedPriceMoreThanCapForStandard(standardLearningDeliveries, 1)).Returns(true);

            var learningDeliveryAppFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            learningDeliveryAppFinRecordQueryServiceMock.Setup(x =>
                x.GetTotalTNPPriceForLatestAppFinRecordsForLearning(standardLearningDeliveries)).Returns(10);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(x => x.GetStandardFundingForCodeOnDate(1, new DateTime(2017, 10, 10)))
                .Returns(new LARSStandardFunding()
                {
                    CoreGovContributionCap = 4.5m
                });

            NewRule(null, learningDeliveryAppFinRecordQueryServiceMock.Object, dd17Mock.Object, larsDataServiceMock.Object)
                .ConditionMet(1, 20, learningDeliveries).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_NullPMR()
        {
            NewRule().ConditionMet(1, null, null).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_DD17False()
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
            dd17Mock.Setup(x => x.IsTotalNegotiatedPriceMoreThanCapForStandard(standardLearningDeliveries, 1)).Returns(false);

            NewRule(null, null, dd17Mock.Object).ConditionMet(1, 20, standardLearningDeliveries).Should().BeFalse();
            dd17Mock.Verify(x => x.IsTotalNegotiatedPriceMoreThanCapForStandard(standardLearningDeliveries, 1), Times.Once);
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
                    LearnStartDate = new DateTime(2017, 10, 10),
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
            dd17Mock.Setup(x => x.IsTotalNegotiatedPriceMoreThanCapForStandard(standardLearningDeliveries, 1)).Returns(true);

            var learningDeliveryAppFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            learningDeliveryAppFinRecordQueryServiceMock.Setup(x =>
                x.GetTotalTNPPriceForLatestAppFinRecordsForLearning(standardLearningDeliveries)).Returns(15);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(x => x.GetStandardFundingForCodeOnDate(1, new DateTime(2017, 10, 10)))
                .Returns(new LARSStandardFunding()
                {
                    CoreGovContributionCap = 4.5m
                });

            NewRule(null, learningDeliveryAppFinRecordQueryServiceMock.Object, dd17Mock.Object, larsDataServiceMock.Object)
                .ConditionMet(1, 10, standardLearningDeliveries).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_NullFundingCap()
        {
            var standardLearningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    StdCodeNullable = 1,
                    ProgTypeNullable = 25,
                    FundModel = 81,
                    AimType = 1,
                    LearnStartDate = new DateTime(2017, 10, 10),
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
                x.GetTotalTNPPriceForLatestAppFinRecordsForLearning(standardLearningDeliveries)).Returns(15);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(x => x.GetStandardFundingForCodeOnDate(1, new DateTime(2017, 10, 10)))
                .Returns(new LARSStandardFunding()
                {
                    CoreGovContributionCap = null
                });

            NewRule(null, learningDeliveryAppFinRecordQueryServiceMock.Object, dd17Mock.Object, larsDataServiceMock.Object)
                .ConditionMet(1, int.MaxValue, standardLearningDeliveries).Should().BeFalse();

            larsDataServiceMock.Verify(x => x.GetStandardFundingForCodeOnDate(1, new DateTime(2017, 10, 10)), Times.Once);
        }

        [Fact]
        public void GetApplicableDateForCapChecking_Success_EarlierStartDate()
        {
            var standardLearningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    LearnStartDate = new DateTime(2017, 10, 11),
                    OrigLearnStartDateNullable = new DateTime(2017, 10, 20),
                },
                new TestLearningDelivery()
                {
                    LearnStartDate = new DateTime(2017, 10, 10),
                    OrigLearnStartDateNullable = null
                },
                new TestLearningDelivery()
                {
                    LearnStartDate = new DateTime(2017, 10, 12),
                    OrigLearnStartDateNullable = new DateTime(2017, 10, 11),
                }
            };

            NewRule().GetApplicableDateForCapChecking(standardLearningDeliveries).Should().Be(new DateTime(2017, 10, 10));
        }

        [Fact]
        public void GetApplicableDateForCapChecking_Success_EarlierOrigStartDate()
        {
            var standardLearningDeliveries = new List<ILearningDelivery>()
            {
                new TestLearningDelivery()
                {
                    LearnStartDate = new DateTime(2017, 10, 11),
                    OrigLearnStartDateNullable = null
                },
                new TestLearningDelivery()
                {
                    LearnStartDate = new DateTime(2017, 10, 10),
                    OrigLearnStartDateNullable = new DateTime(2017, 10, 09),
                },
                new TestLearningDelivery()
                {
                    LearnStartDate = new DateTime(2017, 10, 12),
                    OrigLearnStartDateNullable = new DateTime(2017, 10, 10),
                }
            };

            NewRule().GetApplicableDateForCapChecking(standardLearningDeliveries).Should().Be(new DateTime(2017, 10, 09));
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
                    LearnStartDate = new DateTime(2017, 10, 12),
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
                    LearnStartDate = new DateTime(2017, 10, 10),
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
            dd17Mock.Setup(x => x.IsTotalNegotiatedPriceMoreThanCapForStandard(It.IsAny<List<ILearningDelivery>>(), 1)).Returns(true);

            var learningDeliveryAppFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            learningDeliveryAppFinRecordQueryServiceMock.Setup(x =>
                x.GetTotalTNPPriceForLatestAppFinRecordsForLearning(It.IsAny<List<ILearningDelivery>>())).Returns(16);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(x => x.GetStandardFundingForCodeOnDate(1, new DateTime(2017, 10, 10)))
                .Returns(new LARSStandardFunding()
                {
                    CoreGovContributionCap = 10m
                });

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, learningDeliveryAppFinRecordQueryServiceMock.Object, dd17Mock.Object, larsDataServiceMock.Object).Validate(learner);
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
                    LearnStartDate = new DateTime(2017, 10, 11),
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
                    LearnStartDate = new DateTime(2017, 10, 10),
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
                    LearnStartDate = new DateTime(2017, 10, 12),
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
                LearnRefNumber = "test",
                LearningDeliveries = standardLearningDeliveries
            };

            var dd17Mock = new Mock<IDerivedData_17Rule>();
            dd17Mock.Setup(x => x.IsTotalNegotiatedPriceMoreThanCapForStandard(It.IsAny<List<ILearningDelivery>>(), 1)).Returns(true);

            var learningDeliveryAppFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            learningDeliveryAppFinRecordQueryServiceMock.Setup(x =>
                x.GetTotalTNPPriceForLatestAppFinRecordsForLearning(It.IsAny<List<ILearningDelivery>>())).Returns(15);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock.Setup(x => x.GetStandardFundingForCodeOnDate(1, It.IsAny<DateTime>()))
                .Returns(new LARSStandardFunding()
                {
                    CoreGovContributionCap = 10.5m
                });

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object, learningDeliveryAppFinRecordQueryServiceMock.Object, dd17Mock.Object, larsDataServiceMock.Object)
                    .Validate(learner);
                dd17Mock.Verify(x => x.IsTotalNegotiatedPriceMoreThanCapForStandard(It.IsAny<IReadOnlyCollection<ILearningDelivery>>(), It.IsAny<int>()), Times.AtLeastOnce);
                learningDeliveryAppFinRecordQueryServiceMock.Verify(x => x.GetTotalTNPPriceForLatestAppFinRecordsForLearning(It.IsAny<List<ILearningDelivery>>()), Times.AtLeastOnce);
                larsDataServiceMock.Verify(x => x.GetStandardFundingForCodeOnDate(1, new DateTime(2017, 10, 10)), Times.AtLeastOnce());
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

        private R73Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            ILearningDeliveryAppFinRecordQueryService learningDeliveryAppFinRecordQueryService = null,
            IDerivedData_17Rule dd17 = null,
            ILARSDataService larsDataService = null)
        {
            return new R73Rule(validationErrorHandler, learningDeliveryAppFinRecordQueryService, larsDataService, dd17);
        }
    }
}
