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
    public class R106RuleTests : AbstractRuleTests<R106Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R106");
        }

        [Fact]
        public void GetApplicableLearningDeliveryFAMs_NoDeliveriesReturnsEmpty()
        {
            NewRule().GetApplicableLearningDeliveryFAMs(new TestLearner()).Should().BeNullOrEmpty();
        }

        [Fact]
        public void GetApplicableLearningDeliveryFAMs_NoLearningDeliveryFamsReturnsEmpty()
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery()
                }
            };

            NewRule().GetApplicableLearningDeliveryFAMs(learner).Should().BeNullOrEmpty();
        }

        [Fact]
        public void GetApplicableLearningDeliveryFAMs_LearningDeliveryFamsReturnsEmpty_DateMismatch()
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = "LSF"
                            }
                        }
                    },
                    new TestLearningDelivery
                    {
                        LearningDeliveryFAMs = new List<TestLearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM
                            {
                                LearnDelFAMType = "LSF",
                                LearnDelFAMDateFromNullable = new DateTime(2015, 08, 01)
                            }
                        }
                    }
                }
            };

            NewRule().GetApplicableLearningDeliveryFAMs(learner).Should().BeNullOrEmpty();
        }

        [Fact]
        public void GetApplicableLearningDeliveryFAMs_LearningDeliveryFamsReturnsCollection()
        {
            var learningDeliveryFAMsOne = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LSF",
                    LearnDelFAMDateFromNullable = new DateTime(2018, 08, 01)
                }
            };

            var learningDeliveryFAMsTwo = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LSF",
                    LearnDelFAMDateFromNullable = new DateTime(2015, 08, 01)
                }
            };

            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        LearningDeliveryFAMs = learningDeliveryFAMsOne
                    },
                    new TestLearningDelivery
                    {
                        LearningDeliveryFAMs = learningDeliveryFAMsTwo
                    }
                }
            };

            NewRule().GetApplicableLearningDeliveryFAMs(learner).Should().BeEquivalentTo(learningDeliveryFAMsOne);
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

            var learningDeliveryFAMsOne = new List<TestLearningDeliveryFAM>
            {
               learningDeliveryFamOne,
               learningDeliveryFamTwo
            };

            var learningDeliveryFAMsTwo = new List<TestLearningDeliveryFAM>
            {
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
                        LearningDeliveryFAMs = learningDeliveryFAMsOne
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 2,
                        AimType = 5,
                        LearningDeliveryFAMs = learningDeliveryFAMsTwo
                    }
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock
                .Setup(qs => qs.GetOverLappingLearningDeliveryFAMsForType(
                    new List<TestLearningDeliveryFAM>
                    {
                       learningDeliveryFamOne,
                       learningDeliveryFamTwo,
                       learningDeliveryFamThree
                    }, "LSF"))
                .Returns(learningDeliveryFAMsTwo);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
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
                LearnDelFAMDateFromNullable = new DateTime(2018, 9, 3),
            };

            var learningDeliveryFAMsOne = new List<TestLearningDeliveryFAM>
            {
               learningDeliveryFamOne,
               learningDeliveryFamTwo
            };

            var learningDeliveryFAMsTwo = new List<TestLearningDeliveryFAM>
            {
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
                        LearningDeliveryFAMs = learningDeliveryFAMsOne
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 2,
                        AimType = 5,
                        LearningDeliveryFAMs = learningDeliveryFAMsTwo
                    }
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock
                .Setup(qs => qs.GetOverLappingLearningDeliveryFAMsForType(It.IsAny<IEnumerable<TestLearningDeliveryFAM>>(), "LSF"))
                .Returns(new List<TestLearningDeliveryFAM>());

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_NoFAMs()
        {
            var learner = new TestLearner()
            {
                LearnRefNumber = "00100309",
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 1
                    },
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 2,
                        AimType = 5
                    }
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock
                .Setup(qs => qs.GetOverLappingLearningDeliveryFAMsForType(It.IsAny<IEnumerable<TestLearningDeliveryFAM>>(), "LSF"))
                .Returns(new List<TestLearningDeliveryFAM>());

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_NoDeliveries()
        {
            var learner = new TestLearner();

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock
                .Setup(qs => qs.GetOverLappingLearningDeliveryFAMsForType(It.IsAny<IEnumerable<TestLearningDeliveryFAM>>(), "LSF"))
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

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnDelFAMType", "LSF")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(famType);

            validationErrorHandlerMock.Verify();
        }

        public R106Rule NewRule(ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new R106Rule(learningDeliveryFAMQueryService, validationErrorHandler);
        }
    }
}
