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
    public class AFinDate_05RuleTests : AbstractRuleTests<AFinDate_05Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("AFinDate_05");
        }

        [Fact]
        public void TNP1RecordLaterThanTNP3Record_ReturnsEntity()
        {
            var tnp1Entity = new TestAppFinRecord
            {
                AFinType = "TNP",
                AFinCode = 1,
                AFinDate = new DateTime(2018, 10, 1)
            };

            var tnp3Date = new DateTime(2018, 9, 1);

            var appFinRecords = new List<TestAppFinRecord>
            {
                tnp1Entity
            };

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            appFinRecordQueryServiceMock.Setup(x => x.GetAppFinRecordsForTypeAndCode(appFinRecords, "TNP", 1)).Returns(appFinRecords);

            NewRule(appFinRecordQueryServiceMock.Object).TNP1RecordLaterThanTNP3Record(appFinRecords, tnp3Date).Should().Be(tnp1Entity);
        }

        [Fact]
        public void TNP1RecordLaterThanTNP3Record_MisMatchTypesReturnsNull()
        {
            var appFinRecords = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "PMR",
                    AFinCode = 1,
                    AFinDate = new DateTime(2018, 9, 1)
                }
            };

            var tnp3Date = new DateTime(2018, 9, 1);

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            appFinRecordQueryServiceMock.Setup(x => x.GetAppFinRecordsForTypeAndCode(appFinRecords, "TNP", 1)).Returns(Enumerable.Empty<IAppFinRecord>());

            NewRule(appFinRecordQueryServiceMock.Object).TNP1RecordLaterThanTNP3Record(appFinRecords, tnp3Date).Should().BeNull();
        }

        [Fact]
        public void TNP1RecordLaterThanTNP3Record_MisMatchDatesReturnsNull()
        {
            var appFinRecords = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 1,
                    AFinDate = new DateTime(2018, 9, 1)
                }
            };

            var tnp3Date = new DateTime(2018, 9, 1);

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            appFinRecordQueryServiceMock.Setup(x => x.GetAppFinRecordsForTypeAndCode(appFinRecords, "TNP", 1)).Returns(appFinRecords);

            NewRule(appFinRecordQueryServiceMock.Object).TNP1RecordLaterThanTNP3Record(appFinRecords, tnp3Date).Should().BeNull();
        }

        [Fact]
        public void TNP1RecordLaterThanTNP3Record_NoEntitiesReturnsNull()
        {
            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            appFinRecordQueryServiceMock.Setup(x => x.GetAppFinRecordsForTypeAndCode(null, "TNP", 1)).Returns(Enumerable.Empty<IAppFinRecord>());

            NewRule(appFinRecordQueryServiceMock.Object).TNP1RecordLaterThanTNP3Record(null, new DateTime(2018, 9, 1)).Should().BeNull();
        }

        [Fact]
        public void TNP1RecordLaterThanTNP3Record_NullEntityAndDateReturnsNull()
        {
            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            appFinRecordQueryServiceMock.Setup(x => x.GetAppFinRecordsForTypeAndCode(null, "TNP", 1)).Returns(Enumerable.Empty<IAppFinRecord>());

            NewRule(appFinRecordQueryServiceMock.Object).TNP1RecordLaterThanTNP3Record(null, null).Should().BeNull();
        }

        [Fact]
        public void TNP1RecordLaterThanTNP3Record_NullDate()
        {
            var tnp1Entity = new TestAppFinRecord
            {
                AFinType = "TNP",
                AFinCode = 1,
                AFinDate = new DateTime(2018, 10, 1)
            };

            var appFinRecords = new List<TestAppFinRecord>
            {
                tnp1Entity
            };

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            appFinRecordQueryServiceMock.Setup(x => x.GetAppFinRecordsForTypeAndCode(appFinRecords, "TNP", 1)).Returns(appFinRecords);

            NewRule(appFinRecordQueryServiceMock.Object).TNP1RecordLaterThanTNP3Record(appFinRecords, null).Should().BeNull();
        }

        [Fact]
        public void Validate_Error()
        {
            var tnp1ListOne = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 1,
                    AFinDate = new DateTime(2018, 9, 1)
                },
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 1,
                    AFinDate = new DateTime(2018, 10, 1)
                },
            };

            var appFinListOne = new List<TestAppFinRecord>();

            appFinListOne.Union(tnp1ListOne);
            appFinListOne.Add(
                new TestAppFinRecord
                {
                    AFinType = "PMR",
                    AFinCode = 1,
                    AFinDate = new DateTime(2018, 9, 1)
                });

            var tnp1ListTwo = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 1,
                    AFinDate = new DateTime(2018, 11, 1)
                }
            };

            var tnp3Two = new TestAppFinRecord
            {
                AFinType = "TNP",
                AFinCode = 3,
                AFinDate = new DateTime(2018, 10, 1)
            };

            var appFinListTwo = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "PMR",
                    AFinCode = 1,
                    AFinDate = new DateTime(2018, 9, 1)
                }
            };

            appFinListTwo.Union(tnp1ListTwo);
            appFinListTwo.Add(tnp3Two);

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
            appFinRecordQueryServiceMock.Setup(x => x.GetLatestAppFinRecord(appFinListOne, "TNP", 3)).Returns((IAppFinRecord)null);
            appFinRecordQueryServiceMock.Setup(x => x.GetLatestAppFinRecord(appFinListTwo, "TNP", 3)).Returns(tnp3Two);
            appFinRecordQueryServiceMock.Setup(x => x.GetAppFinRecordsForTypeAndCode(appFinListOne, "TNP", 1)).Returns(tnp1ListOne);
            appFinRecordQueryServiceMock.Setup(x => x.GetAppFinRecordsForTypeAndCode(appFinListTwo, "TNP", 1)).Returns(tnp1ListTwo);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(appFinRecordQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Error_MultipleDeliveriesTrigger()
        {
            var tnp1List = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 1,
                    AFinDate = new DateTime(2018, 11, 1)
                }
            };

            var tnp3 = new TestAppFinRecord
            {
                AFinType = "TNP",
                AFinCode = 3,
                AFinDate = new DateTime(2018, 10, 1)
            };

            var appFinList = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "PMR",
                    AFinCode = 1,
                    AFinDate = new DateTime(2018, 9, 1)
                }
            };

            appFinList.Union(tnp1List);
            appFinList.Add(tnp3);

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
            appFinRecordQueryServiceMock.Setup(x => x.GetLatestAppFinRecord(appFinList, "TNP", 3)).Returns(tnp3);
            appFinRecordQueryServiceMock.Setup(x => x.GetAppFinRecordsForTypeAndCode(appFinList, "TNP", 1)).Returns(tnp1List);

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
            appFinRecordQueryServiceMock.Setup(x => x.GetLatestAppFinRecord(learningDeliveryOne.AppFinRecords, "TNP", 3)).Returns((IAppFinRecord)null);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(appFinRecordQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_AppFinRecordsNoTNP3()
        {
            var appFinRecords = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 1,
                    AFinDate = new DateTime(2018, 9, 1)
                },
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 1,
                    AFinDate = new DateTime(2018, 10, 1)
                },
                new TestAppFinRecord
                {
                    AFinType = "PMR",
                    AFinCode = 1,
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
            appFinRecordQueryServiceMock.Setup(x => x.GetLatestAppFinRecord(appFinRecords, "TNP", 3)).Returns((IAppFinRecord)null);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(appFinRecordQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_AppFinRecordsDateMisMatch()
        {
            var tnp1ListOne = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 1,
                    AFinDate = new DateTime(2018, 9, 1)
                },
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 1,
                    AFinDate = new DateTime(2018, 10, 1)
                }
            };

            var appFinRecordsOne = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "PMR",
                    AFinCode = 1,
                    AFinDate = new DateTime(2018, 9, 1)
                }
            };

            appFinRecordsOne.AddRange(tnp1ListOne);

            var tnp3 = new TestAppFinRecord
            {
                AFinType = "TNP",
                AFinCode = 3,
                AFinDate = new DateTime(2018, 10, 1)
            };

            var tnp1ListTwo = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 1,
                    AFinDate = new DateTime(2018, 10, 1)
                }
            };

            var appFinRecordsTwo = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "PMR",
                    AFinCode = 1,
                    AFinDate = new DateTime(2018, 9, 1)
                }
            };

            appFinRecordsTwo.AddRange(tnp1ListTwo);
            appFinRecordsTwo.Add(tnp3);

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
            appFinRecordQueryServiceMock.Setup(x => x.GetLatestAppFinRecord(appFinRecordsOne, "TNP", 3)).Returns((IAppFinRecord)null);
            appFinRecordQueryServiceMock.Setup(x => x.GetLatestAppFinRecord(appFinRecordsTwo, "TNP", 3)).Returns(tnp3);
            appFinRecordQueryServiceMock.Setup(x => x.GetAppFinRecordsForTypeAndCode(appFinRecordsOne, "TNP", 1)).Returns(tnp1ListOne);
            appFinRecordQueryServiceMock.Setup(x => x.GetAppFinRecordsForTypeAndCode(appFinRecordsTwo, "TNP", 1)).Returns(tnp1ListTwo);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(appFinRecordQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        private AFinDate_05Rule NewRule(ILearningDeliveryAppFinRecordQueryService appFinRecordQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new AFinDate_05Rule(validationErrorHandler, appFinRecordQueryService ?? Mock.Of<ILearningDeliveryAppFinRecordQueryService>());
        }
    }
}
