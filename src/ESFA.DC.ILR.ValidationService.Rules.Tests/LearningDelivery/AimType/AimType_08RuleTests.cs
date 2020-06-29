using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AimType;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.AimType
{
    public class AimType_08RuleTests : AbstractRuleTests<AimType_08Rule>
    {
        private const string _larsLearnAimRefType = "1468";
        private const string _larsLearnAimReference = "lars";
        private const string _nonLarsLearnAimReference = "non-lars";

        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("AimType_08");
        }

        [Fact]
        public void ConditionMet_False()
        {
            NewRule().ConditionMet(_larsLearnAimReference, 31, 5).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_ProgType()
        {
            NewRule().ConditionMet(_larsLearnAimReference, 30, 5).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_NonLarsLearnAimRef()
        {
            NewRule().ConditionMet(_nonLarsLearnAimReference, 31, 5).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True_AimType()
        {
            NewRule().ConditionMet(_larsLearnAimReference, 31, 6).Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                ProgTypeNullable = 31,
                AimType = 6,
                LearnAimRef = _larsLearnAimReference
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    learningDelivery
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_SuccessfulLars()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                ProgTypeNullable = 31,
                AimType = 5,
                LearnAimRef = _larsLearnAimReference
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    learningDelivery
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_NonLars()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                ProgTypeNullable = 31,
                AimType = 6,
                LearnAimRef = _nonLarsLearnAimReference
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    learningDelivery
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnAimRef", _larsLearnAimReference)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("ProgType", 31)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("AimType", 6)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(_larsLearnAimReference, 31, 6);

            validationErrorHandlerMock.Verify();
        }

        private AimType_08Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            ILARSLearningDelivery larsLearningDelivery = new Data.External.LARS.Model.LearningDelivery()
            {
                LearnAimRefType = _larsLearnAimRefType
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();

            larsDataServiceMock.Setup(x => x.LearnAimRefExists(_larsLearnAimReference))
                .Returns(true);
            larsDataServiceMock.Setup(x => x.LearnAimRefExists(_nonLarsLearnAimReference))
                .Returns(false);
            larsDataServiceMock.Setup(x => x.GetDeliveryFor(_larsLearnAimReference))
                .Returns(larsLearningDelivery);

            return new AimType_08Rule(validationErrorHandler, larsDataServiceMock.Object);
        }
    }
}
