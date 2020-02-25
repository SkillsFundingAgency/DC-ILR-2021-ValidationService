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
    public class R61RuleTests : AbstractRuleTests<R61Rule>
    {
        private readonly string _famTypeLSF = Monitoring.Delivery.Types.LearningSupportFunding;

        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R61");
        }

        [Fact]
        public void Validate_Error()
        {
            var learningDeliveryFamOne = new TestLearningDeliveryFAM
            {
                LearnDelFAMType = "LSF",
                LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                LearnDelFAMDateToNullable = new DateTime(2018, 8, 31)
            };

            var learningDeliveryFamTwo = new TestLearningDeliveryFAM
            {
                LearnDelFAMType = "LSF",
                LearnDelFAMDateFromNullable = new DateTime(2018, 9, 1),
                LearnDelFAMDateToNullable = new DateTime(2018, 9, 2)
            };

            var learningDeliveryFamThree = new TestLearningDeliveryFAM
            {
                LearnDelFAMType = "LSF",
                LearnDelFAMDateFromNullable = new DateTime(2018, 9, 2),
            };

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
               learningDeliveryFamOne,
               learningDeliveryFamTwo,
               learningDeliveryFamThree
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

            learningDeliveryFAMQueryServiceMock
                .Setup(qs => qs.GetOverLappingLearningDeliveryFAMsForType(learningDeliveryFAMs, _famTypeLSF))
                .Returns(new List<TestLearningDeliveryFAM> { learningDeliveryFamThree });

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
                validationErrorHandlerMock.Verify(h => h.BuildErrorMessageParameter(It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(3));
            }
        }

        [Fact]
        public void Validate_Error_MultipleDelivieries()
        {
            var learningDeliveryFamOne = new TestLearningDeliveryFAM
            {
                LearnDelFAMType = "LSF",
                LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                LearnDelFAMDateToNullable = new DateTime(2018, 8, 31)
            };

            var learningDeliveryFamTwo = new TestLearningDeliveryFAM
            {
                LearnDelFAMType = "LSF",
                LearnDelFAMDateFromNullable = new DateTime(2018, 9, 1),
                LearnDelFAMDateToNullable = new DateTime(2018, 9, 2)
            };

            var learningDeliveryFamThree = new TestLearningDeliveryFAM
            {
                LearnDelFAMType = "LSF",
                LearnDelFAMDateFromNullable = new DateTime(2018, 9, 2),
            };

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
               learningDeliveryFamOne,
               learningDeliveryFamTwo,
               learningDeliveryFamThree
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
                        LearnStartDate = new DateTime(2018, 9, 1),
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock
                .Setup(qs => qs.GetOverLappingLearningDeliveryFAMsForType(learningDeliveryFAMs, _famTypeLSF))
                .Returns(new List<TestLearningDeliveryFAM> { learningDeliveryFamThree });

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
                validationErrorHandlerMock.Verify(h => h.BuildErrorMessageParameter(It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(6));
            }
        }

        [Fact]
        public void Validate_Error_NoFAMDateTo()
        {
            var learningDeliveryFamOne = new TestLearningDeliveryFAM
            {
                LearnDelFAMType = "LSF",
                LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1)
            };

            var learningDeliveryFamTwo = new TestLearningDeliveryFAM
            {
                LearnDelFAMType = "LSF",
                LearnDelFAMDateFromNullable = new DateTime(2018, 9, 1)
            };

            var learningDeliveryFamThree = new TestLearningDeliveryFAM
            {
                LearnDelFAMType = "LSF",
                LearnDelFAMDateFromNullable = new DateTime(2018, 9, 2),
            };

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
               learningDeliveryFamOne,
               learningDeliveryFamTwo,
               learningDeliveryFamThree
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

            learningDeliveryFAMQueryServiceMock
                .Setup(qs => qs.GetOverLappingLearningDeliveryFAMsForType(learningDeliveryFAMs, _famTypeLSF))
                .Returns(new List<TestLearningDeliveryFAM> { learningDeliveryFamTwo, learningDeliveryFamThree });

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
                validationErrorHandlerMock.Verify(h => h.BuildErrorMessageParameter(It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(6));
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

            learningDeliveryFAMQueryServiceMock
                .Setup(qs => qs.GetOverLappingLearningDeliveryFAMsForType(It.IsAny<IReadOnlyCollection<TestLearningDeliveryFAM>>(), _famTypeLSF))
                .Returns(new List<TestLearningDeliveryFAM>());

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
                                LearnDelFAMType = "LSF"
                            }
                        }
                    }
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock
                .Setup(qs => qs.GetOverLappingLearningDeliveryFAMsForType(It.IsAny<IReadOnlyCollection<TestLearningDeliveryFAM>>(), _famTypeLSF))
                .Returns(new List<TestLearningDeliveryFAM>());

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
                                LearnDelFAMType = "LSF",
                                LearnDelFAMDateFromNullable = new DateTime(2018, 8, 1),
                                LearnDelFAMDateToNullable = new DateTime(2018, 8, 31)
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = "LSF",
                                LearnDelFAMDateFromNullable = new DateTime(2018, 9, 1),
                                LearnDelFAMDateToNullable = new DateTime(2018, 9, 2)
                            },
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = "LSF",
                                LearnDelFAMDateFromNullable = new DateTime(2018, 9, 3),
                            }
                        }
                    }
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock
                .Setup(qs => qs.GetOverLappingLearningDeliveryFAMsForType(It.IsAny<IReadOnlyCollection<TestLearningDeliveryFAM>>(), _famTypeLSF))
                .Returns(new List<TestLearningDeliveryFAM>());

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var famType = Monitoring.Delivery.Types.LearningSupportFunding;
            var learnDelFamDateFrom = new DateTime(2018, 8, 1);
            var learnDelFamDateTo = new DateTime(2018, 8, 1);

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnDelFAMType", "LSF")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnDelFAMDateFrom", "01/08/2018")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnDelFAMDateTo", "01/08/2018")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(famType, learnDelFamDateFrom, learnDelFamDateTo);

            validationErrorHandlerMock.Verify();
        }

        public R61Rule NewRule(ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new R61Rule(learningDeliveryFAMQueryService, validationErrorHandler);
        }
    }
}
