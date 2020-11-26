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
    public class AFinDate_08RuleTests : AbstractRuleTests<AFinDate_08Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("AFinDate_08");
        }

        [Fact]
        public void TNP2DateEqualToTNP4Date_ReturnsEntity()
        {
            var tnp2Entity = new TestAppFinRecord
            {
                AFinType = "TNP",
                AFinCode = 2,
                AFinDate = new DateTime(2018, 9, 1)
            };

            var tnp4Entity = new TestAppFinRecord
            {
                AFinType = "TNP",
                AFinCode = 4,
                AFinDate = new DateTime(2018, 9, 1)
            };

            var appFinRecords = new List<TestAppFinRecord>
            {
                tnp2Entity,
                tnp4Entity
            };

            NewRule().TNP2DateEqualToTNP4Date(appFinRecords, new List<DateTime> { new DateTime(2018, 9, 1) }).Should().Be(tnp2Entity);
        }

        [Fact]
        public void TNP2DateEqualToTNP4Date_MultipleTNP4ReturnsNull()
        {
            var tnp2Entity = new TestAppFinRecord
            {
                AFinType = "TNP",
                AFinCode = 2,
                AFinDate = new DateTime(2018, 10, 1)
            };

            var appFinRecords = new List<TestAppFinRecord>
            {
                tnp2Entity
            };

            NewRule().TNP2DateEqualToTNP4Date(appFinRecords, new List<DateTime> { new DateTime(2018, 9, 1), new DateTime(2018, 11, 1) }).Should().BeNull();
        }

        [Fact]
        public void TNP2DateEqualToTNP4Date_MultipleTNP4ReturnsEntity()
        {
            var tnp2Entity = new TestAppFinRecord
            {
                AFinType = "TNP",
                AFinCode = 2,
                AFinDate = new DateTime(2018, 9, 1)
            };

            var appFinRecords = new List<TestAppFinRecord>
            {
                tnp2Entity
            };

            NewRule().TNP2DateEqualToTNP4Date(appFinRecords, new List<DateTime> { new DateTime(2018, 9, 1), new DateTime(2018, 10, 1) }).Should().Be(tnp2Entity);
        }

        [Fact]
        public void Validate_Error()
        {
            var appFinRecordsOne = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "PMR",
                    AFinCode = 2,
                    AFinDate = new DateTime(2018, 9, 1)
                }
            };

            var tnp2RecordsOne = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 2,
                    AFinDate = new DateTime(2018, 9, 1)
                },
            };

            var tnp4RecordsOne = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 4,
                    AFinDate = new DateTime(2018, 9, 1)
                }
            };

            appFinRecordsOne.AddRange(tnp2RecordsOne);
            appFinRecordsOne.AddRange(tnp4RecordsOne);

            var appFinRecordsTwo = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "PMR",
                    AFinCode = 2,
                    AFinDate = new DateTime(2018, 9, 1)
                }
            };

            var tnp2RecordsTwo = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                     AFinType = "TNP",
                     AFinCode = 2,
                     AFinDate = new DateTime(2018, 11, 1)
                },
            };
            var tnp4RecordsTwo = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 4,
                    AFinDate = new DateTime(2018, 10, 1)
                },
            };

            appFinRecordsTwo.AddRange(tnp2RecordsTwo);
            appFinRecordsTwo.AddRange(tnp4RecordsTwo);

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
            appFinRecordQueryServiceMock.Setup(x => x.GetAppFinRecordsForTypeAndCode(appFinRecordsOne, "TNP", 2)).Returns(tnp2RecordsOne);
            appFinRecordQueryServiceMock.Setup(x => x.GetAppFinRecordsForTypeAndCode(appFinRecordsOne, "TNP", 4)).Returns(tnp4RecordsOne);
            appFinRecordQueryServiceMock.Setup(x => x.GetAppFinRecordsForTypeAndCode(appFinRecordsTwo, "TNP", 2)).Returns(tnp2RecordsTwo);
            appFinRecordQueryServiceMock.Setup(x => x.GetAppFinRecordsForTypeAndCode(appFinRecordsTwo, "TNP", 4)).Returns(tnp4RecordsTwo);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(appFinRecordQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Error_MultipleDeliveriesTrigger()
        {
            var appFinRecordsOne = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "PMR",
                    AFinCode = 2,
                    AFinDate = new DateTime(2018, 9, 1)
                }
            };

            var tnp2RecordsOne = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 2,
                    AFinDate = new DateTime(2018, 9, 1)
                },
            };

            var tnp4RecordsOne = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 4,
                    AFinDate = new DateTime(2018, 9, 1)
                }
            };

            appFinRecordsOne.AddRange(tnp2RecordsOne);
            appFinRecordsOne.AddRange(tnp4RecordsOne);

            var appFinRecordsTwo = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "PMR",
                    AFinCode = 2,
                    AFinDate = new DateTime(2018, 9, 1)
                }
            };

            var tnp2RecordsTwo = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                     AFinType = "TNP",
                     AFinCode = 2,
                     AFinDate = new DateTime(2018, 10, 1)
                },
            };
            var tnp4RecordsTwo = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 4,
                    AFinDate = new DateTime(2018, 10, 1)
                },
            };

            appFinRecordsTwo.AddRange(tnp2RecordsTwo);
            appFinRecordsTwo.AddRange(tnp4RecordsTwo);

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
            appFinRecordQueryServiceMock.Setup(x => x.GetAppFinRecordsForTypeAndCode(appFinRecordsOne, "TNP", 2)).Returns(tnp2RecordsOne);
            appFinRecordQueryServiceMock.Setup(x => x.GetAppFinRecordsForTypeAndCode(appFinRecordsOne, "TNP", 4)).Returns(tnp4RecordsOne);
            appFinRecordQueryServiceMock.Setup(x => x.GetAppFinRecordsForTypeAndCode(appFinRecordsTwo, "TNP", 2)).Returns(tnp2RecordsTwo);
            appFinRecordQueryServiceMock.Setup(x => x.GetAppFinRecordsForTypeAndCode(appFinRecordsTwo, "TNP", 4)).Returns(tnp4RecordsTwo);

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
            appFinRecordQueryServiceMock.Setup(x => x.GetAppFinRecordsForTypeAndCode(learningDeliveryOne.AppFinRecords, "TNP", 2)).Returns(Enumerable.Empty<IAppFinRecord>());
            appFinRecordQueryServiceMock.Setup(x => x.GetAppFinRecordsForTypeAndCode(learningDeliveryTwo.AppFinRecords, "TNP", 4)).Returns(Enumerable.Empty<IAppFinRecord>());

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(appFinRecordQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_AppFinRecordsNoTNP4()
        {
            var appFinRecordsOne = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "PMR",
                    AFinCode = 2,
                    AFinDate = new DateTime(2018, 9, 1)
                }
            };

            var tnp2RecordsOne = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 2,
                    AFinDate = new DateTime(2018, 9, 1)
                },
            };

            var tnp4RecordsOne = new List<TestAppFinRecord>();
            appFinRecordsOne.AddRange(tnp2RecordsOne);
            appFinRecordsOne.AddRange(tnp4RecordsOne);

            var appFinRecordsTwo = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "PMR",
                    AFinCode = 2,
                    AFinDate = new DateTime(2018, 9, 1)
                }
            };

            var tnp2RecordsTwo = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                     AFinType = "TNP",
                     AFinCode = 2,
                     AFinDate = new DateTime(2018, 10, 1)
                },
            };
            var tnp4RecordsTwo = new List<TestAppFinRecord>();
            appFinRecordsTwo.AddRange(tnp2RecordsTwo);
            appFinRecordsTwo.AddRange(tnp4RecordsTwo);

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
            appFinRecordQueryServiceMock.Setup(x => x.GetAppFinRecordsForTypeAndCode(appFinRecordsOne, "TNP", 2)).Returns(tnp2RecordsOne);
            appFinRecordQueryServiceMock.Setup(x => x.GetAppFinRecordsForTypeAndCode(appFinRecordsOne, "TNP", 4)).Returns(Enumerable.Empty<IAppFinRecord>());
            appFinRecordQueryServiceMock.Setup(x => x.GetAppFinRecordsForTypeAndCode(appFinRecordsTwo, "TNP", 2)).Returns(tnp2RecordsTwo);
            appFinRecordQueryServiceMock.Setup(x => x.GetAppFinRecordsForTypeAndCode(appFinRecordsTwo, "TNP", 4)).Returns(Enumerable.Empty<IAppFinRecord>());

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(appFinRecordQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_AppFinRecordsDateMisMatch()
        {
            var appFinRecordsOne = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "PMR",
                    AFinCode = 2,
                    AFinDate = new DateTime(2018, 9, 1)
                }
            };

            var tnp2RecordsOne = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 2,
                    AFinDate = new DateTime(2018, 8, 1)
                },
            };

            var tnp4RecordsOne = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 4,
                    AFinDate = new DateTime(2018, 9, 1)
                }
            };

            appFinRecordsOne.AddRange(tnp2RecordsOne);
            appFinRecordsOne.AddRange(tnp4RecordsOne);

            var appFinRecordsTwo = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "PMR",
                    AFinCode = 2,
                    AFinDate = new DateTime(2018, 9, 1)
                }
            };

            var tnp2RecordsTwo = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                     AFinType = "TNP",
                     AFinCode = 2,
                     AFinDate = new DateTime(2018, 9, 1)
                },
            };
            var tnp4RecordsTwo = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 4,
                    AFinDate = new DateTime(2018, 10, 1)
                },
            };

            appFinRecordsTwo.AddRange(tnp2RecordsTwo);
            appFinRecordsTwo.AddRange(tnp4RecordsTwo);

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
            appFinRecordQueryServiceMock.Setup(x => x.GetAppFinRecordsForTypeAndCode(appFinRecordsOne, "TNP", 2)).Returns(tnp2RecordsOne);
            appFinRecordQueryServiceMock.Setup(x => x.GetAppFinRecordsForTypeAndCode(appFinRecordsOne, "TNP", 4)).Returns(tnp4RecordsOne);
            appFinRecordQueryServiceMock.Setup(x => x.GetAppFinRecordsForTypeAndCode(appFinRecordsTwo, "TNP", 2)).Returns(tnp2RecordsTwo);
            appFinRecordQueryServiceMock.Setup(x => x.GetAppFinRecordsForTypeAndCode(appFinRecordsTwo, "TNP", 4)).Returns(tnp4RecordsTwo);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(appFinRecordQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        private AFinDate_08Rule NewRule(ILearningDeliveryAppFinRecordQueryService appFinRecordQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new AFinDate_08Rule(validationErrorHandler, appFinRecordQueryService ?? Mock.Of<ILearningDeliveryAppFinRecordQueryService>());
        }
    }
}
