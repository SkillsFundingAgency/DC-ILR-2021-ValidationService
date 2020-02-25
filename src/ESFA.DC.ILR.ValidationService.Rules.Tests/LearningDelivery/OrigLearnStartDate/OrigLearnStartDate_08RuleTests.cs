using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.OrigLearnStartDate;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.OrigLearnStartDate
{
    public class OrigLearnStartDate_08RuleTests : AbstractRuleTests<OrigLearnStartDate_08Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("OrigLearnStartDate_08");
        }

        [Fact]
        public void OrigLearnStartDateConditionMet_True()
        {
            var origLearnStartDate = new DateTime(2018, 10, 01);

            NewRule().OrigLearnStartDateConditionMet(origLearnStartDate).Should().BeTrue();
        }

        [Fact]
        public void OrigLearnStartDateConditionMet_False()
        {
            NewRule().OrigLearnStartDateConditionMet(null).Should().BeFalse();
        }

        [Theory]
        [InlineData(99)]
        public void FundModelConditionMet_True(int fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeTrue();
        }

        [Theory]
        [InlineData(35)]
        [InlineData(10)]
        [InlineData(36)]
        public void FundModelConditionMet_False(int fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeFalse();
        }

        [Fact]
        public void LARSConditionMet_True()
        {
            var origLearnStartDate = new DateTime(2018, 10, 01);
            var learnAimRef = "123456789";

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.OrigLearnStartDateBetweenStartAndEndDateForValidityCategory(origLearnStartDate, learnAimRef, TypeOfLARSValidity.AdvancedLearnerLoan))
                .Returns(false);

            NewRule(larsDataServiceMock.Object).LARSConditionMet(origLearnStartDate, learnAimRef).Should().BeTrue();
        }

        [Fact]
        public void LARSConditionMet_False()
        {
            var origLearnStartDate = new DateTime(2018, 10, 01);
            var learnAimRef = "XXX";

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.OrigLearnStartDateBetweenStartAndEndDateForValidityCategory(origLearnStartDate, learnAimRef, TypeOfLARSValidity.AdvancedLearnerLoan))
                .Returns(true);

            NewRule(larsDataServiceMock.Object).LARSConditionMet(origLearnStartDate, learnAimRef).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var origLearnStartDate = new DateTime(2018, 10, 01);
            var fundModel = 99;
            var learnAimRef = "123456789";

            var famQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            famQueryServiceMock
                .Setup(x => x.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), It.IsAny<string>()))
                .Returns(true);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.OrigLearnStartDateBetweenStartAndEndDateForValidityCategory(origLearnStartDate, learnAimRef, TypeOfLARSValidity.AdvancedLearnerLoan))
                .Returns(false);

            var learningDeliveryFams = new List<ILearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = "ADL"
                },
                new TestLearningDeliveryFAM()
                {
                    LearnDelFAMType = "XYZ"
                }
            };

            NewRule(larsDataServiceMock.Object, famQueryServiceMock.Object).ConditionMet(origLearnStartDate, fundModel, learningDeliveryFams, learnAimRef)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void ConditionMet_FalseOrigLearnStartDate()
        {
            NewRule().ConditionMet(null, It.IsAny<int>(), It.IsAny<IReadOnlyCollection<ILearningDeliveryFAM>>(), It.IsAny<string>())
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseFundModel()
        {
            var origLearnStartDate = new DateTime(2018, 10, 01);
            NewRule().ConditionMet(origLearnStartDate, 100, It.IsAny<IReadOnlyCollection<ILearningDeliveryFAM>>(), It.IsAny<string>())
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseLars()
        {
            var origLearnStartDate = new DateTime(2018, 10, 01);
            var fundModel = 99;
            var learnAimRef = "123456789";

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.OrigLearnStartDateBetweenStartAndEndDateForValidityCategory(origLearnStartDate, learnAimRef, TypeOfLARSValidity.AdvancedLearnerLoan))
                .Returns(true);

            var famQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            famQueryServiceMock
                .Setup(x => x.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), It.IsAny<string>()))
                .Returns(true);

            NewRule(larsDataServiceMock.Object, famQueryServiceMock.Object).ConditionMet(origLearnStartDate, fundModel, It.IsAny<IReadOnlyCollection<ILearningDeliveryFAM>>(), learnAimRef)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var origLearnStartDate = new DateTime(2018, 10, 01);
            var fundModel = 99;
            var learnAimRef = "123456789";

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        OrigLearnStartDateNullable = origLearnStartDate,
                        FundModel = fundModel,
                        LearnAimRef = learnAimRef,
                        LearningDeliveryFAMs = new List<ILearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = "ADL"
                            }
                        }
                    }
                }
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.OrigLearnStartDateBetweenStartAndEndDateForValidityCategory(origLearnStartDate, learnAimRef, TypeOfLARSValidity.AdvancedLearnerLoan))
                .Returns(false);

            var famQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            famQueryServiceMock
                .Setup(x => x.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), It.IsAny<string>()))
                .Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(larsDataServiceMock.Object, famQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError()
        {
            var origLearnStartDate = new DateTime(2018, 10, 01);
            var fundModel = 99;
            var learnAimRef = "123456789";

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        OrigLearnStartDateNullable = origLearnStartDate,
                        FundModel = fundModel,
                        LearnAimRef = learnAimRef,
                        LearningDeliveryFAMs = new List<ILearningDeliveryFAM>
                        {
                            new TestLearningDeliveryFAM()
                            {
                                LearnDelFAMType = "ADL"
                            }
                        }
                    }
                }
            };

            var famQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            famQueryServiceMock
                .Setup(x => x.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), It.IsAny<string>()))
                .Returns(true);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.OrigLearnStartDateBetweenStartAndEndDateForValidityCategory(origLearnStartDate, learnAimRef, TypeOfLARSValidity.AdvancedLearnerLoan))
                .Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(larsDataServiceMock.Object, famQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("OrigLearnStartDate", "01/01/2017")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2017, 1, 1));

            validationErrorHandlerMock.Verify();
        }

        private OrigLearnStartDate_08Rule NewRule(
            ILARSDataService larsDataService = null,
            ILearningDeliveryFAMQueryService learningDeliveryFamQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new OrigLearnStartDate_08Rule(larsDataService, learningDeliveryFamQueryService, validationErrorHandler);
        }
    }
}
