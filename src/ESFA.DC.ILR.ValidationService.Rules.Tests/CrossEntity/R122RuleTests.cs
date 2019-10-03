using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R122RuleTests : AbstractRuleTests<R122Rule>
    {
        [Fact]
        public void RuleName1()
        {
            NewRule().RuleName.Should().Be("R122");
        }

        [Fact]
        public void RuleName2()
        {
            NewRule().RuleName.Should().Be(RuleNameConstants.R122);
        }

        public DateTime? GetNullableDate(string candidate) =>
            string.IsNullOrWhiteSpace(candidate) ? (DateTime?)null : DateTime.Parse(candidate);

        [Fact]
        public void FundModelConditionMet_True()
        {
            NewRule().FundModelConditionMet(36).Should().BeTrue();
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(35).Should().BeFalse();
        }

        [Fact]
        public void ProgTypeConditionMet_False()
        {
            NewRule().ProgTypeConditionMet(10).Should().BeFalse();
        }

        [Fact]
        public void ProgTypeConditionMet_True()
        {
            NewRule().ProgTypeConditionMet(25).Should().BeTrue();
        }

        [Fact]
        public void CompStatusConditionMet_False()
        {
            NewRule().CompletionStatusConditionMet(1).Should().BeFalse();
        }

        [Fact]
        public void CompStatusConditionMet_True()
        {
            NewRule().CompletionStatusConditionMet(2).Should().BeTrue();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void HasApprenticeshipContractMeetsExpectation(bool expectation)
        {
            var fams = new List<ILearningDeliveryFAM>();

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFamQueryServiceMock.Setup(s => s.HasLearningDeliveryFAMType(fams,  "ACT")).Returns(expectation);

            var sut = NewRule(learningDeliveryFamQueryService: learningDeliveryFamQueryServiceMock.Object);

            sut.HasApprenticeshipContract(fams).Should().Be(expectation);
        }

        [Fact]
        public void AchievementDateConditionMet_True()
        {
            NewRule().AchievementDateConditionMet(null).Should().BeTrue();
        }

        [Fact]
        public void AchievementDateConditionMet_False()
        {
            NewRule().AchievementDateConditionMet(new DateTime(2019, 1, 2)).Should().BeFalse();
        }

        [Fact]
        public void Validate_Invalid()
        {
            var learningDeliveryFams = new List<ILearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMDateToNullable = new DateTime(2019, 1, 1)
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 36,
                        ProgTypeNullable = 25,
                        CompStatus = 2,
                        LearnActEndDateNullable = new DateTime(2018, 1, 1),
                        LearningDeliveryFAMs = learningDeliveryFams,
                    }
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFamQueryServiceMock.Setup(s => s.HasLearningDeliveryFAMType(learningDeliveryFams, "ACT")).Returns(true);
            learningDeliveryFamQueryServiceMock.Setup(s => s.GetLearningDeliveryFAMsForType(learningDeliveryFams, "ACT")).Returns(learningDeliveryFams);

            using (var validationErrorHandler = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandler.Object, learningDeliveryFamQueryServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Valid()
        {
            var learningDeliveryFams = new List<ILearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMDateToNullable = new DateTime(2019, 1, 1)
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = 35,
                        ProgTypeNullable = 25,
                        CompStatus = 2,
                        LearnActEndDateNullable = new DateTime(2018, 1, 1),
                        LearningDeliveryFAMs = learningDeliveryFams,
                    }
                }
            };

            using (var validationErrorHandler = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var compStatus = 1;
            var learnDelFamDateTo = new DateTime(2019, 8, 1);
            var achDate = new DateTime(2020, 2, 1);
            var learnDelFamType = "ACT";
            var learnActEndDate = new DateTime(2021, 3, 1);

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParametersFor(compStatus, learnDelFamDateTo, achDate, learnActEndDate);

            validationErrorHandlerMock.Verify(x => x.BuildErrorMessageParameter(PropertyNameConstants.CompStatus, compStatus));
            validationErrorHandlerMock.Verify(x => x.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMDateTo, "01/08/2019"));
            validationErrorHandlerMock.Verify(x => x.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, learnDelFamType));
            validationErrorHandlerMock.Verify(x => x.BuildErrorMessageParameter(PropertyNameConstants.AchDate, "01/02/2020"));
            validationErrorHandlerMock.Verify(x => x.BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, "01/03/2021"));
        }

        private R122Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            ILearningDeliveryFAMQueryService learningDeliveryFamQueryService = null)
        {
            return new R122Rule(
                validationErrorHandler ?? Mock.Of<IValidationErrorHandler>(),
                learningDeliveryFamQueryService ?? Mock.Of<ILearningDeliveryFAMQueryService>());
        }
    }
}
