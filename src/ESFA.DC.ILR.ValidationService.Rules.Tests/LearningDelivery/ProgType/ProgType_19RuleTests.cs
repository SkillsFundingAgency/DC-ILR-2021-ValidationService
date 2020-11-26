using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.ProgType;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.ProgType
{
    public class ProgType_19RuleTests : AbstractRuleTests<ProgType_19Rule>
    {
        private const string _larsLearnAimRefType = "1468";
        private const string _larsLearnAimReference = "lars";
        private const string _nonLarsLearnAimReference = "non-lars";

        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("ProgType_19");
        }

        [Fact]
        public void Validate_NoError()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = 31,
                    },
                    new TestLearningDelivery()
                    {
                        LearnAimRef = _larsLearnAimReference,
                        AimType = 5
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Error_Incorrect_NoMatch()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = 31,
                        LearnAimRef = _larsLearnAimReference
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_No_TLevel()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = 30,
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_No_TLevel_WithLars()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = 30,
                    },
                    new TestLearningDelivery()
                    {
                        LearnAimRef = _larsLearnAimReference,
                        AimType = 5
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var learnAimRefType = "lars";
            var progType = 30;

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LarsLearnAimRefType", learnAimRefType)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("ProgType", progType)).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(learnAimRefType, progType);

            validationErrorHandlerMock.Verify();
        }

        private ProgType_19Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            ILARSLearningDelivery larsLearningDelivery = new Data.External.LARS.Model.LearningDelivery()
            {
                LearnAimRefType = _larsLearnAimRefType
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(x => x.GetDeliveryFor(_larsLearnAimReference))
                .Returns(larsLearningDelivery);

            larsDataServiceMock.Setup(x => x.GetDeliveryFor(_nonLarsLearnAimReference))
                .Returns((ILARSLearningDelivery)null);

            return new ProgType_19Rule(validationErrorHandler, larsDataServiceMock.Object);
        }
    }
}
