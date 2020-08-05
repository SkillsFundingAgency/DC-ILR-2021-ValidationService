using System;
using System.Collections.Generic;
using System.Globalization;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_86RuleTests : AbstractRuleTests<LearnDelFamType_86Rule>
    {
        private readonly DateTime _cutOffDate = new DateTime(2022, 03, 31);

        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMType_86");
        }

        [Fact]
        public void Validate_Error()
        {
            var ldmLearnDelFams = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                    LearnDelFAMCode = LearningDeliveryFAMCodeConstants.LDM_376
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = FundModels.AdultSkills,
                        LearnPlanEndDate = _cutOffDate.AddDays(1)
                    }
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock.Setup(f => f.GetLearningDeliveryFAMsForTypeAndCodes(It.IsAny<IReadOnlyCollection<ILearningDeliveryFAM>>(), It.IsAny<string>(), It.IsAny<HashSet<string>>()))
                .Returns(ldmLearnDelFams);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learningDeliveryFamQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var ldmLearnDelFams = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                    LearnDelFAMCode = LearningDeliveryFAMCodeConstants.LDM_376
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = FundModels.AdultSkills,
                        LearnPlanEndDate = _cutOffDate
                    }
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock.Setup(f => f.GetLearningDeliveryFAMsForTypeAndCodes(It.IsAny<IReadOnlyCollection<ILearningDeliveryFAM>>(), It.IsAny<string>(), It.IsAny<HashSet<string>>()))
                .Returns(ldmLearnDelFams);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learningDeliveryFamQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            var learnPlanEndDate = _cutOffDate.AddDays(1);

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.LDM)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, LearningDeliveryFAMCodeConstants.LDM_376)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.FundModel, FundModels.AdultSkills)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnPlanEndDate, learnPlanEndDate.ToString(CultureInfo.CurrentUICulture))).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(learnPlanEndDate);

            validationErrorHandlerMock.Verify();
        }

        private LearnDelFamType_86Rule NewRule(
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnDelFamType_86Rule(
                learningDeliveryFAMQueryService,
                validationErrorHandler);
        }
    }
}
