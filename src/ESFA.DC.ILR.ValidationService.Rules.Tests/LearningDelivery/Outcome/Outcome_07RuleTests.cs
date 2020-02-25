using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.Outcome;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.Outcome
{
    public class Outcome_07RuleTests : AbstractRuleTests<Outcome_07Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("Outcome_07");
        }

        [Theory]
        [InlineData(1, 24, 2)]
        [InlineData(1, 24, 3)]
        public void LearningDeliveryConditionMet_True(int aimType, int? progType, int? outcome)
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError();

            var testLearningDelivery = new TestLearningDelivery()
            {
                AimType = aimType,
                ProgTypeNullable = progType,
                OutcomeNullable = outcome
            };

            NewRule(learnerDpQueryService: null, validationErrorHandler: validationErrorHandlerMock.Object).LearningDeliveryConditionMet(testLearningDelivery).Should().BeTrue();
        }

        [Theory]
        [InlineData(2, 24, 2)]
        [InlineData(1, 25, 2)]
        [InlineData(1, 24, 4)]
        [InlineData(1, 24, null)]
        [InlineData(1, null, 2)]
        [InlineData(2, 24, 3)]
        [InlineData(1, 25, 3)]
        [InlineData(1, null, 3)]
        public void LearningDeliveryConditionMet_False(int aimType, int? progType, int? outcome)
        {
            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var testLearningDelivery = new TestLearningDelivery()
            {
                AimType = aimType,
                ProgTypeNullable = progType,
                OutcomeNullable = outcome
            };

            NewRule(learnerDpQueryService: null, validationErrorHandler: validationErrorHandlerMock.Object).LearningDeliveryConditionMet(testLearningDelivery).Should().BeFalse();
        }

        [Fact]
        public void DpOutcomeConditionMet_False()
        {
            var matchingDpOutcome = new TestLearnerDestinationAndProgression
            {
                LearnRefNumber = "123456",
                DPOutcomes = new List<TestDPOutcome>
                {
                    new TestDPOutcome
                    {
                        OutStartDate = new DateTime(2018, 9, 1),
                        OutType = "type1",
                        OutCode = 1
                    }
                }
            };

            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError();

            var learnerDPQueryServiceMock = new Mock<ILearnerDPQueryService>();

            learnerDPQueryServiceMock
                .Setup(m => m.GetDestinationAndProgressionForLearner(It.IsAny<string>()))
                .Returns(matchingDpOutcome);

            NewRule(learnerDpQueryService: learnerDPQueryServiceMock.Object, validationErrorHandler: validationErrorHandlerMock.Object).DpOutcomeConditionMet("123456").Should().BeFalse();
        }

        [Fact]
        public void DpOutcomeConditionMet_True()
        {
            var matchingDpOutcome = new TestLearnerDestinationAndProgression
            {
                LearnRefNumber = "123456"
            };

            var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError();

            var learnerDPQueryServiceMock = new Mock<ILearnerDPQueryService>();

            learnerDPQueryServiceMock
                .Setup(m => m.GetDestinationAndProgressionForLearner("123456"))
                .Returns(matchingDpOutcome);

            NewRule(learnerDpQueryService: learnerDPQueryServiceMock.Object, validationErrorHandler: validationErrorHandlerMock.Object).DpOutcomeConditionMet("123456").Should().BeTrue();
        }

        private Outcome_07Rule NewRule(IValidationErrorHandler validationErrorHandler = null, ILearnerDPQueryService learnerDpQueryService = null)
        {
            return new Outcome_07Rule(learnerDpQueryService, validationErrorHandler);
        }
    }
}
