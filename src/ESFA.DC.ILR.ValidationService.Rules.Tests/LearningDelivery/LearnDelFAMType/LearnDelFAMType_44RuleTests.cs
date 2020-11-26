using System;
using System.Collections.Generic;
using System.Linq;
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
    public class LearnDelFAMType_44RuleTests : AbstractRuleTests<LearnDelFAMType_44Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMType_44");
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(FundModels.ApprenticeshipsFrom1May2017).Should().BeFalse();
        }

        [Theory]
        [InlineData(FundModels.AdultSkills)]
        [InlineData(FundModels.OtherAdult)]
        [InlineData(FundModels.EuropeanSocialFund)]
        public void FundModelConditionMet_True(int fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryStartDateConditionMet_False()
        {
            NewRule().LearningDeliveryStartDateConditionMet(new DateTime(2014, 08, 01)).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryStartDateConditionMet_True()
        {
            NewRule().LearningDeliveryStartDateConditionMet(new DateTime(2016, 06, 01)).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryFAMsConditionMet_False()
        {
            IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.HHS, LearnDelFAMCode = "45" },
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM, LearnDelFAMCode = "034" }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMsQueryServiceMock.Setup(dd => dd.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.HHS)).Returns(true);
            learningDeliveryFAMsQueryServiceMock.Setup(dd => dd.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, "034")).Returns(true);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).LearningDeliveryFAMsConditionMet(learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMsConditionMet_True()
        {
            IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT, LearnDelFAMCode = "22" },
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.ADL, LearnDelFAMCode = "104" }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMsQueryServiceMock.Setup(dd => dd.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.HHS)).Returns(false);
            learningDeliveryFAMsQueryServiceMock.Setup(dd => dd.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, "034")).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).LearningDeliveryFAMsConditionMet(learningDeliveryFAMs).Should().BeTrue();
        }

        [Theory]
        [InlineData(3)]
        [InlineData(5)]
        public void LearningDeliveryAimTypeConditionMet_False(int aimType)
        {
            NewRule().LearningDeliveryAimTypeConditionMet(aimType).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryAimTypeConditionMet_True()
        {
            NewRule().LearningDeliveryAimTypeConditionMet(7).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryProgTypeConditionMet_False()
        {
            NewRule().LearningDeliveryProgTypeConditionMet(25).Should().BeFalse();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(15)]
        public void LearningDeliveryProgTypeConditionMet_True(int? progType)
        {
            NewRule().LearningDeliveryProgTypeConditionMet(progType).Should().BeTrue();
        }

        [Theory]
        [InlineData(3)]
        [InlineData(5)]
        public void ConditionMet_False(int aimType)
        {
            IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.HHS, LearnDelFAMCode = "45" },
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM, LearnDelFAMCode = "034" }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMsQueryServiceMock.Setup(dd => dd.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.HHS)).Returns(true);
            learningDeliveryFAMsQueryServiceMock.Setup(dd => dd.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, "034")).Returns(true);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).ConditionMet(FundModels.ApprenticeshipsFrom1May2017, new DateTime(2015, 06, 01), learningDeliveryFAMs, aimType, 25).Should().BeFalse();
        }

        [Theory]
        [InlineData(FundModels.AdultSkills)]
        [InlineData(FundModels.OtherAdult)]
        [InlineData(FundModels.EuropeanSocialFund)]
        public void ConditionMet_True(int fundModel)
        {
            IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT, LearnDelFAMCode = "22" },
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.ADL, LearnDelFAMCode = "104" }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMsQueryServiceMock.Setup(dd => dd.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.HHS)).Returns(false);
            learningDeliveryFAMsQueryServiceMock.Setup(dd => dd.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, "034")).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).ConditionMet(fundModel, new DateTime(2015, 09, 01), learningDeliveryFAMs, 6, null).Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT, LearnDelFAMCode = "22" },
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.ADL, LearnDelFAMCode = "104" }
            };

            ILearner learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimType = 6,
                        LearnStartDate = new DateTime(2016, 09, 01),
                        FundModel = FundModels.AdultSkills,
                        ProgTypeNullable = null,
                        LearningDeliveryFAMs = learningDeliveryFAMs.ToList()
                    }
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMsQueryServiceMock.Setup(dd => dd.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.HHS)).Returns(false);
            learningDeliveryFAMsQueryServiceMock.Setup(dd => dd.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, "034")).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object,
                    validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.HHS, LearnDelFAMCode = "45" },
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM, LearnDelFAMCode = "034" }
            };

            ILearner learner = new TestLearner()
            {
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimType = 3,
                        LearnStartDate = new DateTime(2016, 06, 01),
                        FundModel = FundModels.ApprenticeshipsFrom1May2017,
                        ProgTypeNullable = 25,
                        LearningDeliveryFAMs = learningDeliveryFAMs.ToList()
                    }
                }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMsQueryServiceMock.Setup(dd => dd.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.HHS)).Returns(true);
            learningDeliveryFAMsQueryServiceMock.Setup(dd => dd.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, "034")).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object,
                    validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            validationErrorHandlerMock.Setup(dd => dd.BuildErrorMessageParameter(PropertyNameConstants.AimType, 3)).Verifiable();
            validationErrorHandlerMock.Setup(dd => dd.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, "01/08/2015")).Verifiable();
            validationErrorHandlerMock.Setup(dd => dd.BuildErrorMessageParameter(PropertyNameConstants.FundModel, FundModels.AdultSkills)).Verifiable();
            validationErrorHandlerMock.Setup(dd => dd.BuildErrorMessageParameter(PropertyNameConstants.ProgType, null)).Verifiable();
            validationErrorHandlerMock.Setup(dd => dd.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.LDM)).Verifiable();
            validationErrorHandlerMock.Setup(dd => dd.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, "034")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(3, new DateTime(2015, 08, 01), FundModels.AdultSkills, null, LearningDeliveryFAMTypeConstants.LDM, "034");
            validationErrorHandlerMock.Verify();
        }

        public LearnDelFAMType_44Rule NewRule(
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new LearnDelFAMType_44Rule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryService, validationErrorHandler: validationErrorHandler);
        }
    }
}
