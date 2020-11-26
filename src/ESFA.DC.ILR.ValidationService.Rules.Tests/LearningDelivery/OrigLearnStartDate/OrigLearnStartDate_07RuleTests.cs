using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.OrigLearnStartDate;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.OrigLearnStartDate
{
    public class OrigLearnStartDate_07RuleTests : AbstractRuleTests<OrigLearnStartDate_07Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("OrigLearnStartDate_07");
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
        [InlineData(81, 100)]
        [InlineData(81, null)]
        [InlineData(36, 25)]
        public void FundModelConditionMet_True(int fundModel, int? progType)
        {
            NewRule().FundModelConditionMet(fundModel, progType).Should().BeTrue();
        }

        [Theory]
        [InlineData(35, null)]
        [InlineData(99, null)]
        [InlineData(10, null)]
        [InlineData(36, 10)]
        public void FundModelConditionMet_False(int fundModel, int? progType)
        {
            NewRule().FundModelConditionMet(fundModel, progType).Should().BeFalse();
        }

        [Fact]
        public void LARSConditionMet_True()
        {
            var origLearnStartDate = new DateTime(2018, 10, 01);
            var learnAimRef = "123456789";

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.OrigLearnStartDateBetweenStartAndEndDateForValidityCategory(origLearnStartDate, learnAimRef, LARSConstants.Validities.Any))
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
                .Setup(ldsm => ldsm.OrigLearnStartDateBetweenStartAndEndDateForValidityCategory(origLearnStartDate, learnAimRef, LARSConstants.Validities.Any))
                .Returns(true);

            NewRule(larsDataServiceMock.Object).LARSConditionMet(origLearnStartDate, learnAimRef).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var origLearnStartDate = new DateTime(2018, 10, 01);
            var fundModel = 36;
            var progType = 25;
            var learnAimRef = "123456789";

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.OrigLearnStartDateBetweenStartAndEndDateForValidityCategory(origLearnStartDate, learnAimRef, LARSConstants.Validities.Any))
                .Returns(false);

            NewRule(larsDataServiceMock.Object).ConditionMet(origLearnStartDate, fundModel, progType, learnAimRef)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void ConditionMet_FalseOrigLearnStartDate()
        {
            NewRule().ConditionMet(null, 35, 0, It.IsAny<string>())
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseFundModel()
        {
            var origLearnStartDate = new DateTime(2018, 10, 01);
            NewRule().ConditionMet(origLearnStartDate, 100, It.IsAny<int>(), It.IsAny<string>())
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseLars()
        {
            var origLearnStartDate = new DateTime(2018, 10, 01);
            var fundModel = 35;
            var progType = 22;
            var learnAimRef = "123456789";

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.OrigLearnStartDateBetweenStartAndEndDateForValidityCategory(origLearnStartDate, learnAimRef, LARSConstants.Validities.Any))
                .Returns(false);

            NewRule(larsDataServiceMock.Object).ConditionMet(origLearnStartDate, fundModel, progType, learnAimRef)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var origLearnStartDate = new DateTime(2018, 10, 01);
            var fundModel = 36;
            var progType = 25;
            var learnAimRef = "123456789";

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        OrigLearnStartDateNullable = origLearnStartDate,
                        FundModel = fundModel,
                        ProgTypeNullable = progType,
                        LearnAimRef = learnAimRef
                    }
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(false);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.OrigLearnStartDateBetweenStartAndEndDateForValidityCategory(origLearnStartDate, learnAimRef, LARSConstants.Validities.Any))
                .Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(larsDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError()
        {
            var origLearnStartDate = new DateTime(2018, 10, 01);
            var fundModel = 36;
            var progType = 25;
            var learnAimRef = "123456789";

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        OrigLearnStartDateNullable = origLearnStartDate,
                        FundModel = fundModel,
                        ProgTypeNullable = progType,
                        LearnAimRef = learnAimRef
                    }
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(false);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.OrigLearnStartDateBetweenStartAndEndDateForValidityCategory(origLearnStartDate, learnAimRef, LARSConstants.Validities.Any))
                .Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(larsDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
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

        private OrigLearnStartDate_07Rule NewRule(
            ILARSDataService larsDataService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new OrigLearnStartDate_07Rule(larsDataService, validationErrorHandler);
        }
    }
}
