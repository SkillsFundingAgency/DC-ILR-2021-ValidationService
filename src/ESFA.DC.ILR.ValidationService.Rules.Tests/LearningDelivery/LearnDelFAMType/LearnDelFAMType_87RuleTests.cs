using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_87RuleTests : AbstractRuleTests<LearnDelFAMType_87Rule>
    {
        private readonly DateTime _cutOffDate = new DateTime(2022, 07, 31);
        private readonly HashSet<string> ldmCodes = new HashSet<string>()
        {
            "376"
        };

        [Fact]
        public void Validation_NoError()
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
                        LearnActEndDateNullable = _cutOffDate
                    }
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(ldm => ldm.GetLearningDeliveryFAMsForTypeAndCodes(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "LDM", ldmCodes))
                .Returns(ldmLearnDelFams);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learningDeliveryFamQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validation_Error()
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
                        LearnActEndDateNullable = _cutOffDate.AddDays(1)
                    }
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(ldm => ldm.GetLearningDeliveryFAMsForTypeAndCodes(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), "LDM", ldmCodes))
                .Returns(ldmLearnDelFams);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learningDeliveryFamQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = FundModels.AdultSkills,
                LearnActEndDateNullable = new DateTime(2022, 08, 01)
            };

            NewRule().ConditionMet(learningDelivery).Should().BeTrue();
        }

        [Theory]
        [InlineData(25, "2022-07-31")] // FundModel incorrect date correct
        [InlineData(25, "2022-08-30")] // FundModel incorrect date incorrect
        [InlineData(35, "2022-07-31")] // FundModel correct date incorrect
        public void ConditionMet_False(int fundModel, string endDate)
        {
            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = fundModel,
                LearnActEndDateNullable = DateTime.Parse(endDate)
            };

            NewRule().ConditionMet(learningDelivery).Should().BeFalse();
        }

        [Theory]
        [InlineData(25)]
        [InlineData(36)]
        [InlineData(70)]
        public void IsAdultSkillsFundingModel_False(int fundModel)
        {
            NewRule().IsAdultSkillsFundingModel(fundModel).Should().BeFalse();
        }

        [Fact]
        public void IsAdultSkillsFundingModel_True()
        {
            int fundModel = 35;
            NewRule().IsAdultSkillsFundingModel(fundModel).Should().BeTrue();
        }

        [Theory]
        [InlineData("2022-07-31")]
        [InlineData("2022-07-30")]
        public void EndDateCheck_False(string endDate)
        {
            NewRule().EndDateCheck(DateTime.Parse(endDate)).Should().BeFalse();
        }

        [Fact]
        public void EndDateCheck_True()
        {
            NewRule().EndDateCheck(new DateTime(2022, 08, 01)).Should().BeTrue();
        }

        private LearnDelFAMType_87Rule NewRule(
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnDelFAMType_87Rule(
                learningDeliveryFAMQueryService,
                validationErrorHandler);
        }
    }
}
