using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_86RuleTests : AbstractRuleTests<LearnAimRef_86Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnAimRef_86");
        }

        [Fact]
        public void FundModelCondition_True()
        {
            NewRule().FundModelCondition(35).Should().BeTrue();
        }

        [Fact]
        public void FundModelCondition_False()
        {
            NewRule().FundModelCondition(25).Should().BeFalse();
        }

        [Fact]
        public void ProgTypeCondition_False()
        {
            NewRule().ProgTypeCondition(24).Should().BeFalse();
        }

        [Theory]
        [InlineData(25)]
        [InlineData(null)]
        public void ProgTypeCondition_True(int? progType)
        {
            NewRule().ProgTypeCondition(progType).Should().BeTrue();
        }

        [Theory]
        [InlineData(AimTypes.References.ESFLearnerStartandAssessment, false)]
        [InlineData(AimTypes.References.SupportedInternship16To19, false)]
        [InlineData(AimTypes.References.WorkExperience, true)]
        [InlineData(AimTypes.References.IndustryPlacement, true)]
        [InlineData(AimTypes.References.TLevelWorkExperience, true)]
        [InlineData(AimTypes.References.WorkPlacement0To49Hours, false)]
        [InlineData(AimTypes.References.WorkPlacement100To199Hours, false)]
        [InlineData(AimTypes.References.WorkPlacement200To499Hours, false)]
        [InlineData(AimTypes.References.WorkPlacement500PlusHours, false)]
        [InlineData(AimTypes.References.WorkPlacement50To99Hours, false)]
        public void IsWorkExperienceMeetsExpectation(string candidate, bool expectation)
        {
            var sut = NewRule();
            var mockDelivery = new Mock<ILearningDelivery>();
            mockDelivery
                .SetupGet(y => y.LearnAimRef)
                .Returns(candidate);

            var result = sut.IsWorkExperience(mockDelivery.Object);

            Assert.Equal(expectation, result);
        }

        [Fact]
        public void LearningDeliveryFAMExclusion_True()
        {
            var learningDeliveryFamQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(
                It.IsAny<IEnumerable<ILearningDeliveryFAM>>(),
                LearningDeliveryFAMTypeConstants.LDM,
                LearningDeliveryFAMCodeConstants.LDM_SteelRedundancy)).Returns(true);

            NewRule(learningDeliveryFamQSMock.Object).LearningDeliveryFAMExclusion(It.IsAny<IEnumerable<ILearningDeliveryFAM>>()).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryFAMExclusion_False()
        {
            var learningDeliveryFamQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(
                It.IsAny<IEnumerable<ILearningDeliveryFAM>>(),
                LearningDeliveryFAMTypeConstants.LDM,
                LearningDeliveryFAMCodeConstants.LDM_SteelRedundancy)).Returns(false);

            NewRule(learningDeliveryFamQSMock.Object).LearningDeliveryFAMExclusion(It.IsAny<IEnumerable<ILearningDeliveryFAM>>()).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                    LearnDelFAMCode = "034"
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "ZWRKX001",
                        FundModel = 35,
                        ProgTypeNullable = 25,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                }
            };

            var learningDeliveryFamQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(
                learningDeliveryFAMs,
                LearningDeliveryFAMTypeConstants.LDM,
                LearningDeliveryFAMCodeConstants.LDM_SteelRedundancy)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(learningDeliveryFamQSMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_Exclusion()
        {
            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                    LearnDelFAMCode = "347"
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "ZWRKX001",
                        FundModel = 35,
                        ProgTypeNullable = 25,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                }
            };

            var learningDeliveryFamQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(
                learningDeliveryFAMs,
                LearningDeliveryFAMTypeConstants.LDM,
                LearningDeliveryFAMCodeConstants.LDM_SteelRedundancy)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learningDeliveryFamQSMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_ProgType()
        {
            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.LDM,
                    LearnDelFAMCode = "047"
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnAimRef = "ZWRKX001",
                        FundModel = 35,
                        ProgTypeNullable = 24,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                }
            };

            var learningDeliveryFamQSMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQSMock.Setup(x => x.HasLearningDeliveryFAMCodeForType(
                learningDeliveryFAMs,
                LearningDeliveryFAMTypeConstants.LDM,
                LearningDeliveryFAMCodeConstants.LDM_SteelRedundancy)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(learningDeliveryFamQSMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_NoDeliveries()
        {
            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(handler: validationErrorHandlerMock.Object).Validate(new TestLearner());
            }
        }

        public LearnAimRef_86Rule NewRule(ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null, IValidationErrorHandler handler = null)
        {
            return new LearnAimRef_86Rule(handler, learningDeliveryFAMQueryService);
        }
    }
}
