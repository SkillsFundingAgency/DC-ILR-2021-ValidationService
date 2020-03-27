using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.PHours;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.PHours
{
    public class PHours_02RuleTests : AbstractRuleTests<PHours_02Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("PHours_02");
        }

        [Fact]
        public void PlannedHoursConditionMet_Pass_AsLessPHours()
        {
            int? pHours = 250;
            NewRule().PlannedHoursConditionMet(pHours).Should().BeTrue();
        }

        [Fact]
        public void PlannedHoursConditionMet_Fails_AsPHrsNull()
        {
            int? pHours = null;
            NewRule().PlannedHoursConditionMet(pHours).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMsCondition_Pass()
        {
            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>();
            var famType = "RES";
            var mockFAMQueryService = new Mock<ILearningDeliveryFAMQueryService>();
            mockFAMQueryService.Setup(x => x.HasLearningDeliveryFAMType(learningDeliveryFAMs, famType)).Returns(false);

            var rule = NewRule(learningDeliveryFamQueryService: mockFAMQueryService.Object).LearningDeliveryFAMsConditionMet(learningDeliveryFAMs);
            rule.Should().BeTrue();
            mockFAMQueryService.Verify(x => x.HasLearningDeliveryFAMType(learningDeliveryFAMs, famType), Times.AtLeastOnce);
        }

        [Fact]
        public void LearningDeliveryFAMsCondition_Fail()
        {
            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>();
            var famType = "RES";
            var mockFAMQueryService = new Mock<ILearningDeliveryFAMQueryService>();
            mockFAMQueryService.Setup(x => x.HasLearningDeliveryFAMType(learningDeliveryFAMs, famType)).Returns(true);

            var rule = NewRule(learningDeliveryFamQueryService: mockFAMQueryService.Object).LearningDeliveryFAMsConditionMet(learningDeliveryFAMs);

            rule.Should().BeFalse();
            mockFAMQueryService.Verify(x => x.HasLearningDeliveryFAMType(learningDeliveryFAMs, famType), Times.AtLeastOnce);
        }

        [Fact]
        public void ConditionMet_Pass()
        {
            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>();
            int? pHours = 250;
            var famType = "RES";
            var mockFAMQueryService = new Mock<ILearningDeliveryFAMQueryService>();
            mockFAMQueryService.Setup(x => x.HasLearningDeliveryFAMType(learningDeliveryFAMs, famType)).Returns(false);

            var rule = NewRule(learningDeliveryFamQueryService: mockFAMQueryService.Object).ConditionMet(pHours, learningDeliveryFAMs);

            rule.Should().BeTrue();
            mockFAMQueryService.Verify(x => x.HasLearningDeliveryFAMType(learningDeliveryFAMs, famType), Times.AtLeastOnce);
        }

        [Theory]
        [InlineData(false, true)]
        [InlineData(true, false)]
        public void ConditionMet_Fails(bool pHoursCondition, bool delFAMCondition)
        {
            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>();
            int? pHours = 250;

            var mockRule = NewRuleMock();
            mockRule.Setup(x => x.PlannedHoursConditionMet(pHours)).Returns(pHoursCondition);
            mockRule.Setup(x => x.LearningDeliveryFAMsConditionMet(learningDeliveryFAMs)).Returns(delFAMCondition);
            mockRule.Object
                .ConditionMet(pHours, learningDeliveryFAMs)
                .Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            int? pHours = 250;

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "RES"
                }
            };

            var learningDeliveries = new List<TestLearningDelivery>()
            {
                 new TestLearningDelivery
                 {
                     AimType = 1,
                     AimSeqNumber = 123,
                     PHoursNullable = pHours,
                     LearningDeliveryFAMs = learningDeliveryFAMs
                 }
            };

            var learner = new TestLearner()
            {
                LearnRefNumber = "LearnRefNumber",
                LearningDeliveries = learningDeliveries
            };

            var mockFAMQueryService = new Mock<ILearningDeliveryFAMQueryService>();
            mockFAMQueryService.Setup(x => x.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES")).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    learningDeliveryFamQueryService: mockFAMQueryService.Object,
                    validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            int? pHours = 250;

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "RES"
                }
            };

            var learningDeliveries = new List<TestLearningDelivery>()
            {
                 new TestLearningDelivery
                 {
                     AimType = 1,
                     AimSeqNumber = 123,
                     PHoursNullable = pHours,
                     LearningDeliveryFAMs = learningDeliveryFAMs
                 }
            };

            var learner = new TestLearner()
            {
                LearnRefNumber = "LearnRefNumber",
                LearningDeliveries = learningDeliveries
            };

            var mockFAMQueryService = new Mock<ILearningDeliveryFAMQueryService>();
            mockFAMQueryService.Setup(x => x.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES")).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    learningDeliveryFamQueryService: mockFAMQueryService.Object,
                    validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var pHours = 200;
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            validationErrorHandlerMock.Setup(x => x.BuildErrorMessageParameter(PropertyNameConstants.PHours, pHours)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(pHours);

            validationErrorHandlerMock.Verify();
        }

        public PHours_02Rule NewRule(ILearningDeliveryFAMQueryService learningDeliveryFamQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new PHours_02Rule(learningDeliveryFamQueryService, validationErrorHandler);
        }
    }
}
