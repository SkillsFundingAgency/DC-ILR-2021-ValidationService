using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LSDPostcode;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LSDPostcode
{
    public class LSDPostcode_01RuleTests : AbstractRuleTests<LSDPostcode_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LSDPostcode_01");
        }

        [Fact]
        public void LearnSartDateConditionMet_True()
        {
            NewRule().LearnSartDateConditionMet(new DateTime(2019, 8, 1)).Should().BeTrue();
        }

        [Fact]
        public void LearnSartDateConditionMet_False()
        {
            NewRule().LearnSartDateConditionMet(new DateTime(2018, 8, 1)).Should().BeFalse();
        }

        [Theory]
        [InlineData(10)]
        [InlineData(35)]
        public void FundModelConditionMet_True(int fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeTrue();
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(70).Should().BeFalse();
        }

        [Fact]
        public void PostcodeConditionMet_False()
        {
            var postcode = "Postcode";

            var postcodeDataServiceMock = new Mock<IPostcodesDataService>();

            postcodeDataServiceMock.Setup(p => p.PostcodeExists(postcode)).Returns(true);

            NewRule(postcodesDataService: postcodeDataServiceMock.Object).PostcodeConditionMet(postcode).Should().BeFalse();
        }

        [Theory]
        [InlineData("Postcode")]
        [InlineData(null)]
        public void PostcodeConditionMet_True(string postcode)
        {
            var postcodeDataServiceMock = new Mock<IPostcodesDataService>();

            postcodeDataServiceMock.Setup(p => p.PostcodeExists(postcode)).Returns(false);

            NewRule(postcodesDataService: postcodeDataServiceMock.Object).PostcodeConditionMet(postcode).Should().BeTrue();
        }

        [Theory]
        [InlineData("LDM", null, null, true)]
        [InlineData("ACT", "ZZ99 9ZZ", null, false)]
        [InlineData("ACT", null, 1, false)]
        public void IsExcluded_True(string famType, string postcode, int? progType, bool mockResult)
        {
            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = famType,
                    LearnDelFAMCode = "034"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMCode = "034"
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFamQueryServiceMock.Setup(ldf => ldf.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, LearningDeliveryFAMCodeConstants.LDM_OLASS)).Returns(mockResult);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFamQueryServiceMock.Object).IsExcluded(progType, postcode, learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void IsExcluded_False()
        {
            var learningDeliveryFAMs = new List<ILearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMCode = "034"
                },
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMCode = "034"
                }
            };

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFamQueryServiceMock.Setup(ldf => ldf.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, LearningDeliveryFAMCodeConstants.LDM_OLASS)).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFamQueryServiceMock.Object).IsExcluded(null, null, learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learnStartDate = new DateTime(2019, 8, 1);
            var fundModel = 35;
            var lsdPostcode = "Postcode";

            var ruleMock = NewRuleMock();

            ruleMock.Setup(rm => rm.LearnSartDateConditionMet(learnStartDate)).Returns(true);
            ruleMock.Setup(rm => rm.FundModelConditionMet(fundModel)).Returns(true);
            ruleMock.Setup(rm => rm.PostcodeConditionMet(lsdPostcode)).Returns(true);
            ruleMock.Setup(rm => rm.IsExcluded(null, lsdPostcode, It.IsAny<IEnumerable<ILearningDeliveryFAM>>())).Returns(false);

            ruleMock.Object.ConditionMet(learnStartDate, fundModel, null, lsdPostcode, It.IsAny<IEnumerable<ILearningDeliveryFAM>>()).Should().BeTrue();
        }

        [Theory]
        [InlineData(false, true, true, false)]
        [InlineData(true, false, true, false)]
        [InlineData(true, true, false, false)]
        [InlineData(true, true, true, true)]
        [InlineData(false, true, true, true)]
        [InlineData(false, false, true, true)]
        [InlineData(false, true, false, true)]
        [InlineData(true, false, false, false)]
        [InlineData(false, false, false, false)]
        public void ConditionMet_False(bool mock1, bool mock2, bool mock3, bool mock4)
        {
            var learnStartDate = new DateTime(2019, 8, 1);
            var fundModel = 35;
            var lsdPostcode = "Postcode";

            var ruleMock = NewRuleMock();

            ruleMock.Setup(rm => rm.LearnSartDateConditionMet(learnStartDate)).Returns(mock1);
            ruleMock.Setup(rm => rm.FundModelConditionMet(fundModel)).Returns(mock2);
            ruleMock.Setup(rm => rm.PostcodeConditionMet(lsdPostcode)).Returns(mock3);
            ruleMock.Setup(rm => rm.IsExcluded(null, lsdPostcode, It.IsAny<IEnumerable<ILearningDeliveryFAM>>())).Returns(mock4);

            ruleMock.Object.ConditionMet(learnStartDate, fundModel, null, lsdPostcode, It.IsAny<IEnumerable<ILearningDeliveryFAM>>()).Should().BeFalse();
        }

       [Fact]
        public void ValidateError()
        {
            var learnStartDate = new DateTime(2019, 8, 1);
            var fundModel = 35;
            var lsdPostcode = "Postcode";
            var learningDeliveryFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMCode = "034"
                },
            };
            var learningDelivery = new TestLearningDelivery
            {
                LearnStartDate = learnStartDate,
                FundModel = fundModel,
                LSDPostcode = lsdPostcode,
                LearningDeliveryFAMs = learningDeliveryFams
            };

            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    learningDelivery,
                    learningDelivery,
                }
            };

            var postcodeDataServiceMock = new Mock<IPostcodesDataService>();
            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            postcodeDataServiceMock.Setup(p => p.PostcodeExists(lsdPostcode)).Returns(false);
            learningDeliveryFamQueryServiceMock.Setup(ldf => ldf.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, LearningDeliveryFAMTypeConstants.LDM, LearningDeliveryFAMCodeConstants.LDM_OLASS)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object, postcodeDataServiceMock.Object, learningDeliveryFamQueryServiceMock.Object).Validate(learner);
                validationErrorHandlerMock.Verify(h => h.BuildErrorMessageParameter(It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(6));
            }
        }

        [Fact]
        public void ValidateNoError()
        {
            var learnStartDate = new DateTime(2019, 8, 1);
            var fundModel = 35;
            var lsdPostcode = "Postcode";
            var learningDeliveryFams = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "ACT",
                    LearnDelFAMCode = "034"
                },
            };
            var learningDelivery = new TestLearningDelivery
            {
                LearnStartDate = learnStartDate,
                FundModel = fundModel,
                LSDPostcode = lsdPostcode,
                LearningDeliveryFAMs = learningDeliveryFams
            };

            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    learningDelivery,
                    learningDelivery,
                }
            };

            var postcodeDataServiceMock = new Mock<IPostcodesDataService>();
            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            postcodeDataServiceMock.Setup(p => p.PostcodeExists(lsdPostcode)).Returns(true);
            learningDeliveryFamQueryServiceMock.Setup(ldf => ldf.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, LearningDeliveryFAMTypeConstants.LDM, LearningDeliveryFAMCodeConstants.LDM_OLASS)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object, postcodeDataServiceMock.Object, learningDeliveryFamQueryServiceMock.Object).Validate(learner);
                validationErrorHandlerMock.Verify(h => h.BuildErrorMessageParameter(It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(0));
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var learsStartDate = new DateTime(2019, 8, 1);
            var fundModel = 36;
            var lsdPostcode = "Postcode";

            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(x => x.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, "01/08/2019")).Verifiable();
            validationErrorHandlerMock.Setup(x => x.BuildErrorMessageParameter(PropertyNameConstants.FundModel, 36)).Verifiable();
            validationErrorHandlerMock.Setup(x => x.BuildErrorMessageParameter(PropertyNameConstants.LSDPostcode, "Postcode")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(learsStartDate, fundModel, lsdPostcode);

            validationErrorHandlerMock.Verify();
        }

        private LSDPostcode_01Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            IPostcodesDataService postcodesDataService = null,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null)
        {
            return new LSDPostcode_01Rule(validationErrorHandler, postcodesDataService, learningDeliveryFAMQueryService);
        }
    }
}
