using System;
using System.Collections.Generic;
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
    public class LearnDelFAMType_74RuleTests : AbstractRuleTests<LearnDelFAMType_74Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be(RuleNameConstants.LearnDelFAMType_74);
        }

        [Fact]
        public void ValidateWithNullLearnerThrows()
        {
            Action act = () => NewRule().Validate(null);

            act.Should().Throw<ArgumentNullException>();
        }

       [Theory]
       [InlineData("ACT", "105")]
       [InlineData("SOF", "110")]
       [InlineData("ACT", "110")]
        public void HasDisqualifyingMonitor_True(string famType, string famCode)
        {
            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = famType,
                    LearnDelFAMCode = famCode
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(m => m.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "SOF", "105")).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).HasDisqualifyingMonitor(learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void HasDisqualifyingMonitor_False()
        {
            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "SOF",
                    LearnDelFAMCode = "105"
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(m => m.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "SOF", "105")).Returns(true);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).HasDisqualifyingMonitor(learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void HasDisqualifyingMonitor_True_NullFams()
        {
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(m => m.HasLearningDeliveryFAMCodeForType(null, "SOF", "105")).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).HasDisqualifyingMonitor(Array.Empty<ILearningDeliveryFAM>()).Should().BeTrue();
        }

        [Fact]
        public void LastInviableDateMeetsExpectation()
        {
            DateTime.Parse("2019-07-31").Should().BeSameDateAs(LearnDelFAMType_74Rule.LastInviableDate);
        }

        [Theory]
        [InlineData(36)]
        [InlineData(81)]
        [InlineData(70)]
        public void HasQualifyingFunding_True(int fundModel)
        {
            NewRule().HasQualifyingFunding(fundModel).Should().BeTrue();
        }

        [Fact]
        public void HasQualifyingFunding_False()
        {
            NewRule().HasQualifyingFunding(35).Should().BeFalse();
        }

        [Fact]
        public void HasTraineeshipFunding_True()
        {
            NewRule().HasTraineeshipFunding(35, 1).Should().BeTrue();
        }

        [Theory]
        [InlineData(36, 1)]
        [InlineData(35, null)]
        [InlineData(36, null)]
        public void HasTraineeshipFunding_False(int fundModel, int? progType)
        {
            NewRule().HasTraineeshipFunding(fundModel, progType).Should().BeFalse();
        }

        [Theory]
        [InlineData(35, 1)]
        [InlineData(70, null)]
        public void ConditionMet_True(int fundModel, int? progType)
        {
            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMCode = "1"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "SOF",
                    LearnDelFAMCode = "110"
                }
            };

            var delivery = new TestLearningDelivery
            {
                LearnStartDate = new DateTime(2019, 8, 1),
                ProgTypeNullable = progType,
                FundModel = fundModel,
                LearningDeliveryFAMs = learningDeliveryFAMs
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(m => m.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "SOF", "105")).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).ConditionMet(delivery).Should().BeTrue();
        }

        [Theory]
        [InlineData(2018, 70, 1, "110", false)]
        [InlineData(2019, 25, 1, "110", false)]
        [InlineData(2019, 35, null, "110", false)]
        [InlineData(2019, 70, 1, "105", true)]
        public void ConditionMet_False(int year, int fundModel, int? progType, string famCode, bool mockValue)
        {
            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMCode = "1"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "SOF",
                    LearnDelFAMCode = famCode
                }
            };

            var delivery = new TestLearningDelivery
            {
                LearnStartDate = new DateTime(year, 8, 1),
                ProgTypeNullable = progType,
                FundModel = fundModel,
                LearningDeliveryFAMs = learningDeliveryFAMs
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(m => m.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "SOF", "105")).Returns(mockValue);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).ConditionMet(delivery).Should().BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMCode = "1"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "SOF",
                    LearnDelFAMCode = "110"
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                   new TestLearningDelivery
                   {
                       LearnStartDate = new DateTime(2019, 8, 1),
                       FundModel = 70,
                       LearningDeliveryFAMs = learningDeliveryFAMs
                   }
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(m => m.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "SOF", "105")).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object, learningDeliveryFAMQueryServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError()
        {
            var learningDeliveryFAMs = new TestLearningDeliveryFAM[]
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMCode = "1"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "SOF",
                    LearnDelFAMCode = "105"
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                   new TestLearningDelivery
                   {
                       LearnStartDate = new DateTime(2019, 8, 1),
                       FundModel = 70,
                       LearningDeliveryFAMs = learningDeliveryFAMs
                   }
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(m => m.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, "SOF", "105")).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, learningDeliveryFAMQueryServiceMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var fundModel = 35;

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.FundModel, 35)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, "SOF")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, "105")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(fundModel);

            validationErrorHandlerMock.Verify();
        }

        private LearnDelFAMType_74Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null)
        {
            return new LearnDelFAMType_74Rule(validationErrorHandler, learningDeliveryFAMQueryService);
        }
    }
}
