using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinDate;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
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

        [Theory]
        [InlineData(3, 1)]
        [InlineData(3, 3)]
        [InlineData(1, 3)]
        public void TNP3Exists_True(int aFinCode1, int aFinCode2)
        {
            var appFinRecords = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = aFinCode1,
                    AFinDate = new DateTime(2018, 9, 1)
                },
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = aFinCode2,
                    AFinDate = new DateTime(2018, 9, 1)
                }
            };

            NewRule().TNP3Exists(appFinRecords).Should().BeTrue();
        }

        [Theory]
        [InlineData("TNP", "PMR", 3)]
        [InlineData("PMR", "PMR", 3)]
        [InlineData("TNP", "TNP", 1)]
        public void TNP3Exists_False(string aFinType1, string aFinType2, int aFinCode)
        {
            var appFinRecords = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = aFinType1,
                    AFinCode = 1,
                    AFinDate = new DateTime(2018, 9, 1)
                },
                new TestAppFinRecord
                {
                    AFinType = aFinType2,
                    AFinCode = aFinCode,
                    AFinDate = new DateTime(2018, 9, 1)
                }
            };

            NewRule().TNP3Exists(appFinRecords).Should().BeFalse();
        }

        [Fact]
        public void TNP3Exists_False_Null()
        {
            NewRule().TNP3Exists(null).Should().BeFalse();
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

            var tnp3Entity = new TestAppFinRecord
            {
                AFinType = "TNP",
                AFinCode = 3,
                AFinDate = new DateTime(2018, 9, 1)
            };

            var appFinRecords = new List<TestAppFinRecord>
            {
                tnp1Entity,
                tnp3Entity
            };

            NewRule().TNP1RecordLaterThanTNP3Record(appFinRecords).Should().Be(tnp1Entity);
        }

        [Fact]
        public void TNP1RecordLaterThanTNP3Record_MultipleTNP3ReturnsNull()
        {
            var tnp1Entity = new TestAppFinRecord
            {
                AFinType = "TNP",
                AFinCode = 1,
                AFinDate = new DateTime(2018, 10, 1)
            };

            var tnp3EntityOne = new TestAppFinRecord
            {
                AFinType = "TNP",
                AFinCode = 3,
                AFinDate = new DateTime(2018, 9, 1)
            };

            var tnp3EntityTwo = new TestAppFinRecord
            {
                AFinType = "TNP",
                AFinCode = 3,
                AFinDate = new DateTime(2018, 10, 1)
            };

            var appFinRecords = new List<TestAppFinRecord>
            {
                tnp1Entity,
                tnp3EntityOne,
                tnp3EntityTwo
            };

            NewRule().TNP1RecordLaterThanTNP3Record(appFinRecords).Should().BeNull();
        }

        [Fact]
        public void TNP1RecordLaterThanTNP3Record_MultipleTNP3ReturnsEntity()
        {
            var tnp1Entity = new TestAppFinRecord
            {
                AFinType = "TNP",
                AFinCode = 1,
                AFinDate = new DateTime(2018, 11, 1)
            };

            var tnp3EntityOne = new TestAppFinRecord
            {
                AFinType = "TNP",
                AFinCode = 3,
                AFinDate = new DateTime(2018, 9, 1)
            };

            var tnp3EntityTwo = new TestAppFinRecord
            {
                AFinType = "TNP",
                AFinCode = 3,
                AFinDate = new DateTime(2018, 10, 1)
            };

            var appFinRecords = new List<TestAppFinRecord>
            {
                tnp1Entity,
                tnp3EntityOne,
                tnp3EntityTwo
            };

            NewRule().TNP1RecordLaterThanTNP3Record(appFinRecords).Should().Be(tnp1Entity);
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
                },
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 3,
                    AFinDate = new DateTime(2018, 9, 1)
                }
            };

            NewRule().TNP1RecordLaterThanTNP3Record(appFinRecords).Should().BeNull();
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
                },
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 3,
                    AFinDate = new DateTime(2018, 9, 1)
                }
            };

            NewRule().TNP1RecordLaterThanTNP3Record(appFinRecords).Should().BeNull();
        }

        [Fact]
        public void TNP1RecordLaterThanTNP3Record_NoEntitiesReturnsNull()
        {
            NewRule().TNP1RecordLaterThanTNP3Record(null).Should().BeNull();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AppFinRecords = new List<TestAppFinRecord>
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
                        }
                    },
                    new TestLearningDelivery()
                    {
                        AppFinRecords = new List<TestAppFinRecord>
                        {
                            new TestAppFinRecord
                            {
                                AFinType = "TNP",
                                AFinCode = 1,
                                AFinDate = new DateTime(2018, 11, 1)
                            },
                            new TestAppFinRecord
                            {
                                AFinType = "TNP",
                                AFinCode = 3,
                                AFinDate = new DateTime(2018, 10, 1)
                            },
                            new TestAppFinRecord
                            {
                                AFinType = "PMR",
                                AFinCode = 1,
                                AFinDate = new DateTime(2018, 9, 1)
                            }
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Error_MultipleDeliveriesTrigger()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AppFinRecords = new List<TestAppFinRecord>
                        {
                            new TestAppFinRecord
                            {
                                AFinType = "TNP",
                                AFinCode = 3,
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
                        }
                    },
                    new TestLearningDelivery()
                    {
                        AppFinRecords = new List<TestAppFinRecord>
                        {
                            new TestAppFinRecord
                            {
                                AFinType = "TNP",
                                AFinCode = 1,
                                AFinDate = new DateTime(2018, 11, 1)
                            },
                            new TestAppFinRecord
                            {
                                AFinType = "TNP",
                                AFinCode = 3,
                                AFinDate = new DateTime(2018, 10, 1)
                            },
                            new TestAppFinRecord
                            {
                                AFinType = "PMR",
                                AFinCode = 1,
                                AFinDate = new DateTime(2018, 9, 1)
                            }
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
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
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_NullAppFinRecords()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                    },
                    new TestLearningDelivery
                    {
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_AppFinRecordsNoTNP3()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AppFinRecords = new List<TestAppFinRecord>
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
                        }
                    },
                    new TestLearningDelivery()
                    {
                        AppFinRecords = new List<TestAppFinRecord>
                        {
                            new TestAppFinRecord
                            {
                                AFinType = "TNP",
                                AFinCode = 1,
                                AFinDate = new DateTime(2018, 11, 1)
                            },
                            new TestAppFinRecord
                            {
                                AFinType = "PMR",
                                AFinCode = 3,
                                AFinDate = new DateTime(2018, 10, 1)
                            },
                            new TestAppFinRecord
                            {
                                AFinType = "PMR",
                                AFinCode = 1,
                                AFinDate = new DateTime(2018, 9, 1)
                            }
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_AppFinRecordsDateMisMatch()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AppFinRecords = new List<TestAppFinRecord>
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
                        }
                    },
                    new TestLearningDelivery()
                    {
                        AppFinRecords = new List<TestAppFinRecord>
                        {
                            new TestAppFinRecord
                            {
                                AFinType = "TNP",
                                AFinCode = 1,
                                AFinDate = new DateTime(2018, 10, 1)
                            },
                            new TestAppFinRecord
                            {
                                AFinType = "TNP",
                                AFinCode = 3,
                                AFinDate = new DateTime(2018, 10, 1)
                            },
                            new TestAppFinRecord
                            {
                                AFinType = "PMR",
                                AFinCode = 1,
                                AFinDate = new DateTime(2018, 9, 1)
                            }
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        private AFinDate_05Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new AFinDate_05Rule(validationErrorHandler);
        }
    }
}
