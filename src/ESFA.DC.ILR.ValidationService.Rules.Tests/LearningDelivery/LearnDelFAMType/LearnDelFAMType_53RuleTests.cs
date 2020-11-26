using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_53RuleTests : AbstractRuleTests<LearnDelFAMType_53Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("LearnDelFAMType_53");
        }

        [Theory]
        [InlineData(LearningDeliveryFAMTypeConstants.ALB, "1", false)]
        [InlineData(LearningDeliveryFAMTypeConstants.ALB, "3", false)]
        public void ConditionMet_False(string learDelFamType, string famCode, bool hasFundingStreamPeriodCode)
        {
            var testLearningDeliveryFam = new TestLearningDeliveryFAM()
            {
                LearnDelFAMType = learDelFamType,
                LearnDelFAMCode = famCode
            };

            var fcsDataServiceMock = new Mock<IFCSDataService>();
            fcsDataServiceMock.Setup(x => x.FundingRelationshipFCTExists(It.IsAny<List<string>>())).Returns(hasFundingStreamPeriodCode);

            NewRule(fcsDataServiceMock.Object).ConditionMet(testLearningDeliveryFam).Should().BeFalse();
        }

        [Theory]
        [InlineData(LearningDeliveryFAMTypeConstants.ALB, "1", true)]
        [InlineData(LearningDeliveryFAMTypeConstants.ALB, "3", true)]
        public void ConditionMet_True(string learDelFamType, string famCode, bool hasFundingStreamPeriodCode)
        {
            var testLearningDeliveryFam = new TestLearningDeliveryFAM()
            {
                LearnDelFAMType = learDelFamType,
                LearnDelFAMCode = famCode
            };

            var fcsDataServiceMock = new Mock<IFCSDataService>();
            fcsDataServiceMock.Setup(x => x.FundingRelationshipFCTExists(It.IsAny<List<string>>())).Returns(hasFundingStreamPeriodCode);

            NewRule(fcsDataServiceMock.Object).ConditionMet(testLearningDeliveryFam).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_NullCheck()
        {
            NewRule().ConditionMet(null).Should().BeFalse();
        }

        [Fact]
        public void FCTFundingConditionMet_Pass()
        {
            var fundingStreamPeriodCodes = new List<string>()
            {
                 FundingStreamPeriodCodeConstants.ALLBC2021
            };

            var mockFCSDataService = new Mock<IFCSDataService>();
            mockFCSDataService.Setup(x => x.FundingRelationshipFCTExists(fundingStreamPeriodCodes)).Returns(true);

            NewRule(fcsDataService: mockFCSDataService.Object).FCTFundingConditionMet().Should().BeTrue();

            mockFCSDataService.Verify(x => x.FundingRelationshipFCTExists(fundingStreamPeriodCodes), Times.AtLeastOnce);
        }

        [Fact]
        public void FCTFundingConditionMet_Fails()
        {
            var fundingStreamPeriodCodes = new List<string>()
            {
                 FundingStreamPeriodCodeConstants.ALLBC2021
            };

            var mockFCSDataService = new Mock<IFCSDataService>();
            mockFCSDataService.Setup(x => x.FundingRelationshipFCTExists(fundingStreamPeriodCodes)).Returns(false);

            NewRule(fcsDataService: mockFCSDataService.Object).FCTFundingConditionMet().Should().BeFalse();

            mockFCSDataService.Verify(x => x.FundingRelationshipFCTExists(fundingStreamPeriodCodes), Times.AtLeastOnce);
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            var testLearningDeliveryFam = new TestLearningDeliveryFAM()
            {
                LearnDelFAMType = LearningDeliveryFAMTypeConstants.ALB,
                LearnDelFAMCode = "1"
            };

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.ALB)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, "1")).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.UKPRN, 123456)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(testLearningDeliveryFam, 123456);

            validationErrorHandlerMock.Verify();
        }

        private LearnDelFAMType_53Rule NewRule(
            IFCSDataService fcsDataService = null,
            IValidationErrorHandler validationErrorHandler = null,
            IFileDataService fileDataService = null)
        {
            return new LearnDelFAMType_53Rule(fcsDataService, validationErrorHandler, fileDataService);
        }
    }
}
