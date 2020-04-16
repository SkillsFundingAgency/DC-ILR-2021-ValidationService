using System;
using System.Collections.Generic;
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
    public class R101RuleTests : AbstractRuleTests<R104Rule>
    {
        private readonly string _famTypeACT = Monitoring.Delivery.Types.ApprenticeshipContract;

        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R101");
        }

        [Fact]
        public void DoesNotHaveMultipleACTFams_True()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT"
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.GetLearningDeliveryFAMsCountByFAMType(learningDeliveryFAMs, _famTypeACT)).Returns(1);

            NewRule(learningDeliveryFAMQueryServiceMock.Object).DoesNotHaveMultipleACTFams(learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void DoesNotHaveMultipleACTFams_True_Null()
        {
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.GetLearningDeliveryFAMsCountByFAMType(null, _famTypeACT)).Returns(0);

            NewRule(learningDeliveryFAMQueryServiceMock.Object).DoesNotHaveMultipleACTFams(null).Should().BeTrue();
        }

        [Fact]
        public void DoesNotHaveMultipleACTFams_False()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT"
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.GetLearningDeliveryFAMsCountByFAMType(learningDeliveryFAMs, _famTypeACT)).Returns(2);

            NewRule(learningDeliveryFAMQueryServiceMock.Object).DoesNotHaveMultipleACTFams(learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFamForOverlappingACTTypes_ReturnsEntity()
        {
            var learningDeliveryFamOne = new TestLearningDeliveryFAM
            {
                LearnDelFAMType = "ACT",
                LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                LearnDelFAMDateToNullable = new DateTime(2018, 8, 31)
            };

            var learningDeliveryFamTwo = new TestLearningDeliveryFAM
            {
                LearnDelFAMType = "ACT",
                LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                LearnDelFAMDateToNullable = new DateTime(2018, 9, 1)
            };

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
               learningDeliveryFamOne,
               learningDeliveryFamTwo
            };

            NewRule().LearningDeliveryFamForOverlappingACTTypes(learningDeliveryFAMs).Should().BeEquivalentTo(new List<TestLearningDeliveryFAM> { learningDeliveryFamOne });
        }

        [Fact]
        public void LearningDeliveryFamForOverlappingACTTypes_ReturnsDateMultiple()
        {
            var learningDeliveryFamOne = new TestLearningDeliveryFAM
            {
                LearnDelFAMType = "ACT",
                LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                LearnDelFAMDateToNullable = new DateTime(2018, 8, 31)
            };

            var learningDeliveryFamTwo = new TestLearningDeliveryFAM
            {
                LearnDelFAMType = "ACT",
                LearnDelFAMDateFromNullable = new DateTime(2018, 9, 1),
                LearnDelFAMDateToNullable = new DateTime(2018, 9, 2)
            };

            var learningDeliveryFamThree = new TestLearningDeliveryFAM
            {
                LearnDelFAMType = "ACT",
                LearnDelFAMDateFromNullable = new DateTime(2018, 9, 2),
            };

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
               learningDeliveryFamOne,
               learningDeliveryFamTwo,
               learningDeliveryFamThree
            };

            NewRule().LearningDeliveryFamForOverlappingACTTypes(learningDeliveryFAMs).Should().BeEquivalentTo(new List<TestLearningDeliveryFAM> { learningDeliveryFamTwo });
        }

        [Fact]
        public void LearningDeliveryFamForOverlappingACTTypes_ReturnsDate_NoEndDate()
        {
            var learningDeliveryFamOne = new TestLearningDeliveryFAM
            {
                LearnDelFAMType = "ACT",
                LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1)
            };

            var learningDeliveryFamTwo = new TestLearningDeliveryFAM
            {
                LearnDelFAMType = "ACT",
                LearnDelFAMDateFromNullable = new DateTime(2018, 9, 1)
            };

            var learningDeliveryFamThree = new TestLearningDeliveryFAM
            {
                LearnDelFAMType = "ACT",
                LearnDelFAMDateFromNullable = new DateTime(2018, 9, 3),
            };

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
               learningDeliveryFamOne,
               learningDeliveryFamTwo,
               learningDeliveryFamThree
            };

            NewRule().LearningDeliveryFamForOverlappingACTTypes(learningDeliveryFAMs).Should().BeEquivalentTo(new List<TestLearningDeliveryFAM> { learningDeliveryFamOne, learningDeliveryFamTwo });
        }

        [Fact]
        public void LearningDeliveryFamForOverlappingACTTypes_ReturnsNull_NoDeliveries()
        {
            NewRule().LearningDeliveryFamForOverlappingACTTypes(null).Should().BeNullOrEmpty();
        }

        [Fact]
        public void LearningDeliveryFamForOverlappingACTTypes_ReturnsNull_EmptyCollection()
        {
            NewRule().LearningDeliveryFamForOverlappingACTTypes(new List<TestLearningDeliveryFAM>()).Should().BeNullOrEmpty();
        }

        [Fact]
        public void LearningDeliveryFamForOverlappingACTTypes_ReturnsNull_NoOverlap()
        {
            var learningDeliveryFamOne = new TestLearningDeliveryFAM
            {
                LearnDelFAMType = "ACT",
                LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                LearnDelFAMDateToNullable = new DateTime(2018, 8, 31)
            };

            var learningDeliveryFamTwo = new TestLearningDeliveryFAM
            {
                LearnDelFAMType = "ACT",
                LearnDelFAMDateFromNullable = new DateTime(2018, 9, 1),
                LearnDelFAMDateToNullable = new DateTime(2018, 9, 2)
            };

            var learningDeliveryFamThree = new TestLearningDeliveryFAM
            {
                LearnDelFAMType = "ACT",
                LearnDelFAMDateFromNullable = new DateTime(2018, 9, 3),
            };

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
               learningDeliveryFamOne,
               learningDeliveryFamTwo,
               learningDeliveryFamThree
            };

            NewRule().LearningDeliveryFamForOverlappingACTTypes(learningDeliveryFAMs).Should().BeNullOrEmpty();
        }

        [Fact]
        public void Validate_Error()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
               new TestLearningDeliveryFAM
               {
                   LearnDelFAMType = "ACT",
                   LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                   LearnDelFAMDateToNullable = new DateTime(2018, 8, 31)
               },
               new TestLearningDeliveryFAM
               {
                   LearnDelFAMType = "ACT",
                   LearnDelFAMDateFromNullable = new DateTime(2018, 9, 1),
                   LearnDelFAMDateToNullable = new DateTime(2018, 9, 2)
               },
               new TestLearningDeliveryFAM
               {
                   LearnDelFAMType = "ACT",
                   LearnDelFAMDateFromNullable = new DateTime(2018, 9, 2),
               }
            };

            var learner = new TestLearner()
            {
                LearnRefNumber = "00100309",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 1,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 2,
                        AimType = 5,
                        LearnStartDate = new DateTime(2018, 9, 1)
                    }
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.GetLearningDeliveryFAMsForType(learningDeliveryFAMs, _famTypeACT)).Returns(learningDeliveryFAMs);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.GetLearningDeliveryFAMsCountByFAMType(learningDeliveryFAMs, _famTypeACT)).Returns(3);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Error_MultipleDelivieries()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
               new TestLearningDeliveryFAM
               {
                   LearnDelFAMType = "ACT",
                   LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                   LearnDelFAMDateToNullable = new DateTime(2018, 8, 31)
               },
               new TestLearningDeliveryFAM
               {
                   LearnDelFAMType = "ACT",
                   LearnDelFAMDateFromNullable = new DateTime(2018, 9, 1),
                   LearnDelFAMDateToNullable = new DateTime(2018, 9, 2)
               },
               new TestLearningDeliveryFAM
               {
                   LearnDelFAMType = "ACT",
                   LearnDelFAMDateFromNullable = new DateTime(2018, 9, 2),
               }
            };

            var learner = new TestLearner()
            {
                LearnRefNumber = "00100309",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 1,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    },
                    new TestLearningDelivery()
                     {
                        AimSeqNumber = 2,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                     },
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.GetLearningDeliveryFAMsForType(learningDeliveryFAMs, _famTypeACT)).Returns(learningDeliveryFAMs);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.GetLearningDeliveryFAMsCountByFAMType(learningDeliveryFAMs, _famTypeACT)).Returns(3);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Error_NoFAMDateTo()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
               new TestLearningDeliveryFAM
               {
                   LearnDelFAMType = "ACT",
                   LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
               },
               new TestLearningDeliveryFAM
               {
                   LearnDelFAMType = "ACT",
                   LearnDelFAMDateFromNullable = new DateTime(2018, 9, 1)
               },
               new TestLearningDeliveryFAM
               {
                   LearnDelFAMType = "ACT",
                   LearnDelFAMDateFromNullable = new DateTime(2018, 9, 2),
               }
            };

            var learner = new TestLearner()
            {
                LearnRefNumber = "00100309",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 1,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 2,
                        AimType = 5,
                        LearnStartDate = new DateTime(2018, 9, 1)
                    }
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.GetLearningDeliveryFAMsForType(learningDeliveryFAMs, _famTypeACT)).Returns(learningDeliveryFAMs);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.GetLearningDeliveryFAMsCountByFAMType(learningDeliveryFAMs, _famTypeACT)).Returns(3);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_NoDeliveries()
        {
            var learner = new TestLearner();

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_NoACTFams()
        {
            var learner = new TestLearner()
            {
                LearnRefNumber = "00100309",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 1,
                    },
                    new TestLearningDelivery()
                    {
                    }
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.GetLearningDeliveryFAMsCountByFAMType(It.IsAny<IReadOnlyCollection<TestLearningDeliveryFAM>>(), _famTypeACT)).Returns(0);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.GetLearningDeliveryFAMsForType(null, _famTypeACT)).Returns(new List<TestLearningDeliveryFAM>());

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_SingleCoreAims()
        {
            var learner = new TestLearner()
            {
                LearnRefNumber = "00100309",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 1,
                        AimType = 5,
                        LearnStartDate = new DateTime(2018, 9, 1),
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = "ACT"
                            }
                        }
                    }
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.GetLearningDeliveryFAMsCountByFAMType(It.IsAny<IReadOnlyCollection<TestLearningDeliveryFAM>>(), _famTypeACT)).Returns(1);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_NoOverlap()
        {
            var learner = new TestLearner()
            {
                LearnRefNumber = "00100309",
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = "ACT",
                                LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                                LearnDelFAMDateToNullable = new DateTime(2018, 8, 31)
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = "ACT",
                                LearnDelFAMDateFromNullable = new DateTime(2018, 9, 1),
                                LearnDelFAMDateToNullable = new DateTime(2018, 9, 2)
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = "ACT",
                                LearnDelFAMDateFromNullable = new DateTime(2018, 9, 3),
                            }
                        }
                    }
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.GetLearningDeliveryFAMsCountByFAMType(It.IsAny<IReadOnlyCollection<TestLearningDeliveryFAM>>(), _famTypeACT)).Returns(3);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var famType = Monitoring.Delivery.Types.ApprenticeshipContract;
            var dateFrom = new DateTime(2018, 8, 1);
            var dateTo = new DateTime(2018, 8, 1);

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnDelFAMType", "ACT")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnDelFAMDateFrom", "01/08/2018")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnDelFAMDateTo", "01/08/2018")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(famType, dateFrom, dateTo);

            validationErrorHandlerMock.Verify();
        }

        public R101Rule NewRule(ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new R101Rule(learningDeliveryFAMQueryService, validationErrorHandler);
        }
    }
}
