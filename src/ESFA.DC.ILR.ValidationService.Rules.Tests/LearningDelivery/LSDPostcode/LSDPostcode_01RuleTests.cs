using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Learner.Postcode;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LSDPostcode;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LSDPostcode
{
    public class LSDPostcode_01RuleTests : AbstractRuleTests<LSDPostcode_01Rule>
    {
        private DateTime _firstAugust2019 = new DateTime(2019, 08, 01);

        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LSDPostcode_01");
        }

        [Fact]
        public void LearnStartDate_Passes_AsStartDateisEqual()
        {
            var startDate = new DateTime(2019, 08, 01);
            NewRule().LearnStartDateConditionMet(startDate).Should().BeTrue();
        }

        [Fact]
        public void LearnStartDate_True_AsStartIsGreater()
        {
            var startDate = new DateTime(2019, 12, 01);
            NewRule().LearnStartDateConditionMet(startDate).Should().BeTrue();
        }

        [Fact]
        public void LearnStartDate_Fails_AsStartisLessThan()
        {
            var startDate = new DateTime(2019, 07, 01);
            NewRule().LearnStartDateConditionMet(startDate).Should().BeFalse();
        }

        [Theory]
        [InlineData(TypeOfFunding.AdultSkills, true)]
        [InlineData(TypeOfFunding.OtherAdult, false)]
        [InlineData(15, false)]
        public void FundModelConditionMet(int fundModel, bool asExpected)
        {
            NewRule().FundModelConditionMet(fundModel).Should().Be(asExpected);
        }

        [Fact]
        public void PostcodeNullCondition_Passes()
        {
            NewRule().PostCodeNullConditionMet("postcode").Should().BeTrue();
        }

        [Fact]
        public void TemporaryPostcodeConditionMet_False()
        {
            NewRule().TemporaryPostcodeConditionMet("ZZ99 9ZZ").Should().BeFalse();
        }

        [Fact]
        public void TemporaryPostcodeConditionMet_True()
        {
            NewRule().TemporaryPostcodeConditionMet("Postcode").Should().BeTrue();
        }

        [Theory]
        [InlineData("postcode", true)]
        [InlineData("ZZ99 9ZZ", true)]
        public void ValidPostcodeConditionMet(string postCode, bool asExpected)
        {
            var mockPostcodesService = new Mock<IPostcodesDataService>();
            mockPostcodesService.Setup(x => x.PostcodeExists(postCode)).Returns(true);

            var ruleLSDPostcode = NewRule(postcodesDataService: mockPostcodesService.Object).ValidPostcodeConditionMet(postCode);
            ruleLSDPostcode.Should().Be(asExpected);

            mockPostcodesService.Verify(x => x.PostcodeExists(postCode), Times.AtLeastOnce);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData(23, true)]
        [InlineData(TypeOfLearningProgramme.Traineeship, false)]
        public void ProgtypeConditionMet(int? progType, bool asExpected)
        {
            NewRule().ProTypeConditionMet(progType).Should().Be(asExpected);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData(" ", false)]
        public void PostcodeNullCondition_Fails(string postcode, bool asExpected)
        {
            NewRule().PostCodeNullConditionMet(postcode).Should().Be(asExpected);
        }

        [Fact]
        public void ConditionMet_True()
        {
            var progType = 25;
            var fundModel = TypeOfFunding.AdultSkills;
            var lsdPostcode = "lsdPostcode";
            var startDate = new DateTime(2019, 09, 01);

            var mockPostcodesDataService = new Mock<IPostcodesDataService>();
            mockPostcodesDataService.Setup(x => x.PostcodeExists(lsdPostcode)).Returns(true);

            var lsdRule = NewRule(postcodesDataService: mockPostcodesDataService.Object).ConditionMet(progType, fundModel, lsdPostcode, startDate);
            lsdRule.Should().BeTrue();
            mockPostcodesDataService.Verify(x => x.PostcodeExists(lsdPostcode), Times.AtLeastOnce);
        }

        [Theory]
        [InlineData(24, TypeOfFunding.AdultSkills, "lsdPostCode", "2018-09-11", true)] // progType condition Fails
        [InlineData(25, TypeOfFunding.OtherAdult, "lsdPostcode", "2019-09-11", true)] // fundModel condition Fails
        [InlineData(25, TypeOfFunding.AdultSkills, null, "2019-09-11", true)] // postcode nullable condition Fails
        [InlineData(25, TypeOfFunding.AdultSkills, "ZZ99 9ZZ", "2019-09-11", true)] // temp postcode condition Fails
        [InlineData(25, TypeOfFunding.AdultSkills, "wrongPostCode", "2019-09-11", false)] // valid postcode condition Fails
        [InlineData(25, TypeOfFunding.AdultSkills, "lsdPostCode", "2018-09-11", true)] // startDate condition Fails
        public void ConditionMet_False(int progType, int fundModel, string lsdPostcode, string startDate, bool mockPostcodeResult)
        {
            var learnStartDate = DateTime.Parse(startDate);

            var mockPostcodesDataService = new Mock<IPostcodesDataService>();
            mockPostcodesDataService.Setup(x => x.PostcodeExists(lsdPostcode)).Returns(mockPostcodeResult);

            var lsdRule = NewRule(postcodesDataService: mockPostcodesDataService.Object).ConditionMet(progType, fundModel, lsdPostcode, learnStartDate);
            lsdRule.Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var lsdPostcode = "LSDPostcode";
            var learnStartDate = new DateTime(2019, 09, 01);

            var learningDeliveries = new List<TestLearningDelivery>()
            {
                 new TestLearningDelivery
                 {
                     FundModel = 35,
                     ProgTypeNullable = 25,
                     AimType = 1,
                     LearnStartDate = learnStartDate,
                     LSDPostcode = lsdPostcode
                 }
            };

            var learner = new TestLearner()
            {
                Postcode = lsdPostcode,
                LearnRefNumber = "LearnRefNumber",
                LearningDeliveries = learningDeliveries
            };

            var mockPostcodesDataService = new Mock<IPostcodesDataService>();
            mockPostcodesDataService.Setup(ds => ds.PostcodeExists(lsdPostcode)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(mockPostcodesDataService.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var lsdPostcode = "LSDPostcode";
            var learnStartDate = new DateTime(2019, 09, 01);

            var learningDeliveries = new List<TestLearningDelivery>()
            {
                 new TestLearningDelivery
                 {
                     FundModel = 35,
                     ProgTypeNullable = 24, // no error for ProgType = 24
                     AimType = 1,
                     LearnStartDate = learnStartDate,
                     LSDPostcode = lsdPostcode
                 }
            };

            var learner = new TestLearner()
            {
                Postcode = lsdPostcode,
                LearnRefNumber = "LearnRefNumber",
                LearningDeliveries = learningDeliveries
            };

            var mockPostcodesDataService = new Mock<IPostcodesDataService>();
            mockPostcodesDataService.Setup(ds => ds.PostcodeExists(lsdPostcode)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(mockPostcodesDataService.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var learnStartDate = new DateTime(2018, 09, 01);
            var fundModel = TypeOfFunding.AdultSkills;
            var lsdPostcode = "lsdPostCode";
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(x => x.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, "01/09/2018")).Verifiable();
            validationErrorHandlerMock.Setup(x => x.BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel)).Verifiable();
            validationErrorHandlerMock.Setup(x => x.BuildErrorMessageParameter(PropertyNameConstants.LSDPostcode, lsdPostcode)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(learnStartDate, fundModel, lsdPostcode);

            validationErrorHandlerMock.Verify();
        }

        private LSDPostcode_01Rule NewRule(IPostcodesDataService postcodesDataService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new LSDPostcode_01Rule(postcodesDataService, validationErrorHandler);
        }
    }
}
