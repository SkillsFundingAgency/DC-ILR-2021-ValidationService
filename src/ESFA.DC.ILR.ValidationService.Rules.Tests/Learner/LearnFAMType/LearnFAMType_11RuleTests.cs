using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Learner.LearnFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Mocks;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.LearnFAMType
{
    public class LearnFAMType_11RuleTests : AbstractRuleTests<LearnFAMType_11Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnFAMType_11");
        }

        [Theory]
        [InlineData(LearnerFAMTypeConstants.HNS, 0, false)]
        [InlineData(LearnerFAMTypeConstants.NLM, 2, false)]
        [InlineData(LearnerFAMTypeConstants.EDF, 2, false)]
        [InlineData("edF", 2, false)]
        [InlineData(LearnerFAMTypeConstants.LSR, 4, false)]
        [InlineData(LearnerFAMTypeConstants.SEN, 1, false)]
        [InlineData(LearnerFAMTypeConstants.MCF, 1, false)]
        [InlineData(LearnerFAMTypeConstants.ECF, 1, false)]
        [InlineData(LearnerFAMTypeConstants.FME, 1, false)]
        [InlineData("ABC", 2, false)]
        [InlineData(null, 1, false)]
        [InlineData(LearnerFAMTypeConstants.NLM, 3, true)]
        [InlineData("nlm", 3, true)]
        [InlineData(LearnerFAMTypeConstants.EDF, 3, true)]
        public void ConditionMetMeetsExpectation(string learnFamType, int count, bool expectation)
        {
            var learnerFAMQueryServiceMock = new Mock<ILearnerFAMQueryService>();

            learnerFAMQueryServiceMock.Setup(s => s.GetLearnerFAMsCountByFAMType(It.IsAny<List<ILearnerFAM>>(), learnFamType)).Returns(count);

            NewRule(learnerFAMQueryServiceMock.Object).ConditionMet(learnFamType, It.IsAny<List<ILearnerFAM>>()).Should().Be(expectation);
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 35
                    }
                },
                LearnerFAMs = new List<TestLearnerFAM>()
                {
                    new TestLearnerFAM()
                    {
                        LearnFAMType = LearnerFAMTypeConstants.NLM
                    },
                    new TestLearnerFAM()
                    {
                        LearnFAMType = LearnerFAMTypeConstants.DLA
                    },
                }
            };

            var learnerFAMQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            learnerFAMQueryServiceMock
                .Setup(x => x.GetLearnerFAMsCountByFAMType(It.IsAny<IEnumerable<ILearnerFAM>>(), It.IsAny<string>()))
                .Returns(3);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learnerFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Error_CalledOnceForEachFamType()
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 35
                    }
                },
                LearnerFAMs = new List<TestLearnerFAM>()
                {
                    new TestLearnerFAM { LearnFAMType = LearnerFAMTypeConstants.NLM },
                    new TestLearnerFAM { LearnFAMType = LearnerFAMTypeConstants.NLM },
                    new TestLearnerFAM { LearnFAMType = LearnerFAMTypeConstants.EDF },
                    new TestLearnerFAM { LearnFAMType = LearnerFAMTypeConstants.EDF },
                    new TestLearnerFAM { LearnFAMType = LearnerFAMTypeConstants.EDF },
                    new TestLearnerFAM { LearnFAMType = LearnerFAMTypeConstants.HNS },
                }
            };

            var learnerFAMQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            learnerFAMQueryServiceMock
                .Setup(x => x.GetLearnerFAMsCountByFAMType(It.IsAny<IEnumerable<ILearnerFAM>>(), It.IsAny<string>()))
                .Returns(3);

            var validationErrorHandlerMock = new ValidationErrorHandlerMock(true);
            NewRule(learnerFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);

            validationErrorHandlerMock.Verify(dd => dd.BuildErrorMessageParameter(PropertyNameConstants.LearnFAMType, LearnerFAMTypeConstants.NLM), Times.Once);
            validationErrorHandlerMock.Verify(dd => dd.BuildErrorMessageParameter(PropertyNameConstants.LearnFAMType, LearnerFAMTypeConstants.EDF), Times.Once);
            validationErrorHandlerMock.Verify(dd => dd.BuildErrorMessageParameter(PropertyNameConstants.LearnFAMType, LearnerFAMTypeConstants.HNS), Times.Never);
        }

        [Fact]
        public void Validate_WithNoLearnerFAMS_Returns_NoError()
        {
            var learner = new TestLearner();

            var learnerFAMQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            learnerFAMQueryServiceMock
                .Setup(x => x.GetLearnerFAMsCountByFAMType(It.IsAny<IEnumerable<ILearnerFAM>>(), It.IsAny<string>()))
                .Returns(1);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learnerFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 35
                    }
                },
                LearnerFAMs = new List<TestLearnerFAM>()
                {
                    new TestLearnerFAM()
                    {
                        LearnFAMType = LearnerFAMTypeConstants.NLM
                    },
                    new TestLearnerFAM()
                    {
                        LearnFAMType = LearnerFAMTypeConstants.DLA
                    },
                }
            };

            var learnerFAMQueryServiceMock = new Mock<ILearnerFAMQueryService>();
            learnerFAMQueryServiceMock
                .Setup(x => x.GetLearnerFAMsCountByFAMType(It.IsAny<IEnumerable<ILearnerFAM>>(), It.IsAny<string>()))
                .Returns(2);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learnerFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuilderErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            validationErrorHandlerMock.Setup(dd => dd.BuildErrorMessageParameter(PropertyNameConstants.LearnFAMType, LearnerFAMTypeConstants.ECF)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(LearnerFAMTypeConstants.ECF);

            validationErrorHandlerMock.Verify();
        }

        private LearnFAMType_11Rule NewRule(
            ILearnerFAMQueryService learnerFAMQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnFAMType_11Rule(learnerFAMQueryService, validationErrorHandler);
        }
    }
}
