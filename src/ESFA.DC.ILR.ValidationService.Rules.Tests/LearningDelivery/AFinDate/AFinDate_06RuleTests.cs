using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinDate;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.AFinDate
{
    public class AFinDate_06RuleTests : AbstractRuleTests<AFinDate_06Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("AFinDate_06");
        }

        [Fact]
        public void TNP2RecordLaterThanTNP4Record_ReturnsEntity()
        {
            var tnp2Entity = new TestAppFinRecord
            {
                AFinType = "TNP",
                AFinCode = 2,
                AFinDate = new DateTime(2018, 10, 1)
            };

            var tnp4Date = new DateTime(2018, 9, 1);

            var appFinRecords = new List<TestAppFinRecord>
            {
                tnp2Entity,
            };

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            appFinRecordQueryServiceMock.Setup(x => x.GetAppFinRecordsForTypeAndCode(appFinRecords, "TNP", 2)).Returns(appFinRecords);

            NewRule(appFinRecordQueryServiceMock.Object).TNP2RecordLaterThanTNP4Record(appFinRecords, tnp4Date).Should().Be(tnp2Entity);
        }

        [Fact]
        public void TNP2RecordLaterThanTNP4Record_MultipleTNP4ReturnsNull()
        {
            var tnp2Entity = new TestAppFinRecord
            {
                AFinType = "TNP",
                AFinCode = 2,
                AFinDate = new DateTime(2018, 10, 1)
            };

            var tnp4EntityOne = new TestAppFinRecord
            {
                AFinType = "TNP",
                AFinCode = 4,
                AFinDate = new DateTime(2018, 9, 1)
            };

            var tnp4EntityTwo = new TestAppFinRecord
            {
                AFinType = "TNP",
                AFinCode = 4,
                AFinDate = new DateTime(2018, 10, 1)
            };

            var appFinRecords = new List<TestAppFinRecord>
            {
                tnp2Entity,
                tnp4EntityOne,
                tnp4EntityTwo
            };

            var tnp2Records = new List<TestAppFinRecord>
            {
                tnp2Entity,
            };

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            appFinRecordQueryServiceMock.Setup(x => x.GetAppFinRecordsForTypeAndCode(appFinRecords, "TNP", 2)).Returns(tnp2Records);

            NewRule(appFinRecordQueryServiceMock.Object).TNP2RecordLaterThanTNP4Record(appFinRecords, new DateTime(2018, 10, 1)).Should().BeNull();
        }

        [Fact]
        public void TNP2RecordLaterThanTNP4Record_MultipleTNP4ReturnsEntity()
        {
            var tnp2Entity = new TestAppFinRecord
            {
                AFinType = "TNP",
                AFinCode = 2,
                AFinDate = new DateTime(2018, 11, 1)
            };

            var tnp4EntityOne = new TestAppFinRecord
            {
                AFinType = "TNP",
                AFinCode = 4,
                AFinDate = new DateTime(2018, 9, 1)
            };

            var tnp4EntityTwo = new TestAppFinRecord
            {
                AFinType = "TNP",
                AFinCode = 4,
                AFinDate = new DateTime(2018, 10, 1)
            };

            var appFinRecords = new List<TestAppFinRecord>
            {
                tnp2Entity,
                tnp4EntityOne,
                tnp4EntityTwo
            };

            var tnp2Records = new List<TestAppFinRecord>
            {
                tnp2Entity,
            };

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            appFinRecordQueryServiceMock.Setup(x => x.GetAppFinRecordsForTypeAndCode(appFinRecords, "TNP", 2)).Returns(tnp2Records);

            NewRule(appFinRecordQueryServiceMock.Object).TNP2RecordLaterThanTNP4Record(appFinRecords, new DateTime(2018, 10, 1)).Should().Be(tnp2Entity);
        }

        [Fact]
        public void TNP2RecordLaterThanTNP4Record_MisMatchTypesReturnsNull()
        {
            var appFinRecords = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "PMR",
                    AFinCode = 1,
                    AFinDate = new DateTime(2018, 9, 1)
                },
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 4,
                    AFinDate = new DateTime(2018, 9, 1)
                }
            };

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            appFinRecordQueryServiceMock.Setup(x => x.GetAppFinRecordsForTypeAndCode(appFinRecords, "TNP", 2)).Returns(Enumerable.Empty<IAppFinRecord>());

            NewRule(appFinRecordQueryServiceMock.Object).TNP2RecordLaterThanTNP4Record(appFinRecords, new DateTime(2018, 9, 1)).Should().BeNull();
        }

        [Fact]
        public void TNP2RecordLaterThanTNP4Record_MisMatchDatesReturnsNull()
        {
            var tnp2Entity = new TestAppFinRecord
            {
                AFinType = "TNP",
                AFinCode = 2,
                AFinDate = new DateTime(2018, 9, 1)
            };

            var tnp4EntityOne = new TestAppFinRecord
            {
                AFinType = "TNP",
                AFinCode = 4,
                AFinDate = new DateTime(2018, 9, 1)
            };

            var appFinRecords = new List<TestAppFinRecord>
            {
                tnp2Entity,
                tnp4EntityOne
            };

            var tnp2Records = new List<TestAppFinRecord>
            {
                tnp2Entity,
            };

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            appFinRecordQueryServiceMock.Setup(x => x.GetAppFinRecordsForTypeAndCode(appFinRecords, "TNP", 2)).Returns(tnp2Records);

            NewRule(appFinRecordQueryServiceMock.Object).TNP2RecordLaterThanTNP4Record(appFinRecords, new DateTime(2018, 9, 1)).Should().BeNull();
        }

        [Fact]
        public void TNP2RecordLaterThanTNP4Record_NoEntitiesReturnsNull()
        {
            NewRule().TNP2RecordLaterThanTNP4Record(null, new DateTime(2018, 9, 1)).Should().BeNull();
        }

        [Fact]
        public void TNP2RecordLaterThanTNP4Record_NullEntityAndDateReturnsNull()
        {
            NewRule().TNP2RecordLaterThanTNP4Record(null, null).Should().BeNull();
        }

        [Fact]
        public void Validate_Error()
        {
            var tnp2ListOne = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 2,
                    AFinDate = new DateTime(2018, 9, 1)
                },
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 2,
                    AFinDate = new DateTime(2018, 10, 1)
                },
            };

            var appFinListOne = new List<TestAppFinRecord>();

            appFinListOne.Union(tnp2ListOne);
            appFinListOne.Add(
                new TestAppFinRecord
                {
                    AFinType = "PMR",
                    AFinCode = 2,
                    AFinDate = new DateTime(2018, 9, 1)
                });

            var tnp2ListTwo = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 2,
                    AFinDate = new DateTime(2018, 11, 1)
                }
            };

            var tnp4Two = new TestAppFinRecord
            {
                AFinType = "TNP",
                AFinCode = 4,
                AFinDate = new DateTime(2018, 10, 1)
            };

            var appFinListTwo = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "PMR",
                    AFinCode = 2,
                    AFinDate = new DateTime(2018, 9, 1)
                }
            };

            appFinListTwo.Union(tnp2ListTwo);
            appFinListTwo.Add(tnp4Two);

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AppFinRecords = appFinListOne
                    },
                    new TestLearningDelivery()
                    {
                        AppFinRecords = appFinListTwo
                    }
                }
            };

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            appFinRecordQueryServiceMock.Setup(x => x.GetLatestAppFinRecord(appFinListOne, "TNP", 4)).Returns((IAppFinRecord)null);
            appFinRecordQueryServiceMock.Setup(x => x.GetLatestAppFinRecord(appFinListTwo, "TNP", 4)).Returns(tnp4Two);
            appFinRecordQueryServiceMock.Setup(x => x.GetAppFinRecordsForTypeAndCode(appFinListOne, "TNP", 2)).Returns(tnp2ListOne);
            appFinRecordQueryServiceMock.Setup(x => x.GetAppFinRecordsForTypeAndCode(appFinListTwo, "TNP", 2)).Returns(tnp2ListTwo);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(appFinRecordQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Error_MultipleDeliveriesTrigger()
        {
            var tnp2List = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 2,
                    AFinDate = new DateTime(2018, 11, 1)
                }
            };

            var tnp4 = new TestAppFinRecord
            {
                AFinType = "TNP",
                AFinCode = 4,
                AFinDate = new DateTime(2018, 10, 1)
            };

            var appFinList = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "PMR",
                    AFinCode = 2,
                    AFinDate = new DateTime(2018, 9, 1)
                }
            };

            appFinList.Union(tnp2List);
            appFinList.Add(tnp4);

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AppFinRecords = appFinList
                    },
                    new TestLearningDelivery()
                    {
                        AppFinRecords = appFinList
                    }
                }
            };

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            appFinRecordQueryServiceMock.Setup(x => x.GetLatestAppFinRecord(appFinList, "TNP", 4)).Returns(tnp4);
            appFinRecordQueryServiceMock.Setup(x => x.GetAppFinRecordsForTypeAndCode(appFinList, "TNP", 2)).Returns(tnp2List);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(appFinRecordQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_NullLearningDeliveries()
        {
            var learner = new TestLearner()
            {
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_NullAppFinRecords()
        {
            var learningDeliveryOne = new TestLearningDelivery();
            var learningDeliveryTwo = new TestLearningDelivery();

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    learningDeliveryOne,
                    learningDeliveryTwo
                }
            };

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            appFinRecordQueryServiceMock.Setup(x => x.GetLatestAppFinRecord(learningDeliveryOne.AppFinRecords, "TNP", 4)).Returns((IAppFinRecord)null);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(appFinRecordQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_AppFinRecordsNoTNP4()
        {
            var appFinRecords = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 2,
                    AFinDate = new DateTime(2018, 9, 1)
                },
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 2,
                    AFinDate = new DateTime(2018, 10, 1)
                },
                new TestAppFinRecord
                {
                    AFinType = "PMR",
                    AFinCode = 2,
                    AFinDate = new DateTime(2018, 9, 1)
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AppFinRecords = appFinRecords
                    },
                    new TestLearningDelivery()
                    {
                        AppFinRecords = appFinRecords
                    }
                }
            };

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            appFinRecordQueryServiceMock.Setup(x => x.GetLatestAppFinRecord(appFinRecords, "TNP", 4)).Returns((IAppFinRecord)null);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(appFinRecordQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_AppFinRecordsDateMisMatch()
        {
            var tnp2ListOne = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 2,
                    AFinDate = new DateTime(2018, 9, 1)
                },
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 2,
                    AFinDate = new DateTime(2018, 10, 1)
                }
            };

            var appFinRecordsOne = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "PMR",
                    AFinCode = 2,
                    AFinDate = new DateTime(2018, 9, 1)
                }
            };

            appFinRecordsOne.AddRange(tnp2ListOne);

            var tnp4 = new TestAppFinRecord
            {
                AFinType = "TNP",
                AFinCode = 4,
                AFinDate = new DateTime(2018, 10, 1)
            };

            var tnp2ListTwo = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 2,
                    AFinDate = new DateTime(2018, 10, 1)
                }
            };

            var appFinRecordsTwo = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "PMR",
                    AFinCode = 2,
                    AFinDate = new DateTime(2018, 9, 1)
                }
            };

            appFinRecordsTwo.AddRange(tnp2ListTwo);
            appFinRecordsTwo.Add(tnp4);

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AppFinRecords = appFinRecordsOne
                    },
                    new TestLearningDelivery()
                    {
                        AppFinRecords = appFinRecordsTwo
                    }
                }
            };

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            appFinRecordQueryServiceMock.Setup(x => x.GetLatestAppFinRecord(appFinRecordsOne, "TNP", 4)).Returns((IAppFinRecord)null);
            appFinRecordQueryServiceMock.Setup(x => x.GetLatestAppFinRecord(appFinRecordsTwo, "TNP", 4)).Returns(tnp4);
            appFinRecordQueryServiceMock.Setup(x => x.GetAppFinRecordsForTypeAndCode(appFinRecordsOne, "TNP", 2)).Returns(tnp2ListOne);
            appFinRecordQueryServiceMock.Setup(x => x.GetAppFinRecordsForTypeAndCode(appFinRecordsTwo, "TNP", 2)).Returns(tnp2ListTwo);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(appFinRecordQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        private AFinDate_06Rule NewRule(ILearningDeliveryAppFinRecordQueryService appFinRecordQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new AFinDate_06Rule(validationErrorHandler, appFinRecordQueryService ?? Mock.Of<ILearningDeliveryAppFinRecordQueryService>());
        }
    }
}
