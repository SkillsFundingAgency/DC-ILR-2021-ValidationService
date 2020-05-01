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
    public class OrigLearnStartDate_06RuleTests : AbstractRuleTests<OrigLearnStartDate_06Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("OrigLearnStartDate_06");
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

        [Fact]
        public void FundModelConditionMet_True()
        {
            NewRule().FundModelConditionMet(35).Should().BeTrue();
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(36).Should().BeFalse();
        }

        [Fact]
        public void DD07ConditionMet_True()
        {
            var progType = 22;

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            NewRule(dd07Mock.Object).Excluded(progType).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        public void DD07ConditionMet_False(int? progType)
        {
            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(false);

            NewRule(dd07Mock.Object).Excluded(progType).Should().BeFalse();
        }

        [Fact]
        public void LARSConditionMet_True()
        {
            var origLearnStartDate = new DateTime(2018, 10, 01);
            var learnAimRef = "123456789";

            var larsValidityCategories = new HashSet<string>()
            {
                LARSConstants.Validities.AdultSkills,
                LARSConstants.Validities.Unemployed,
                LARSConstants.Validities.OLASSAdult
            };

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.OrigLearnStartDateBetweenStartAndEndDateForAnyValidityCategory(origLearnStartDate, learnAimRef, larsValidityCategories))
                .Returns(false);

            NewRule(larsDataService: larsDataServiceMock.Object).LARSConditionMet(origLearnStartDate, learnAimRef).Should().BeTrue();
        }

        [Fact]
        public void LARSConditionMet_False()
        {
            var origLearnStartDate = new DateTime(2018, 10, 01);
            var learnAimRef = "XXX";

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.OrigLearnStartDateBetweenStartAndEndDateForAnyValidityCategory(origLearnStartDate, learnAimRef, It.IsAny<HashSet<string>>()))
                .Returns(true);

            NewRule(larsDataService: larsDataServiceMock.Object).LARSConditionMet(origLearnStartDate, learnAimRef).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var origLearnStartDate = new DateTime(2018, 10, 01);
            var fundModel = 35;
            var progType = 22;
            var learnAimRef = "123456789";

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(false);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.OrigLearnStartDateBetweenStartAndEndDateForAnyValidityCategory(origLearnStartDate, learnAimRef, It.IsAny<HashSet<string>>()))
                .Returns(false);

            NewRule(dd07Mock.Object, larsDataServiceMock.Object).ConditionMet(origLearnStartDate, fundModel, progType, learnAimRef)
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

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.OrigLearnStartDateBetweenStartAndEndDateForAnyValidityCategory(origLearnStartDate, learnAimRef, It.IsAny<HashSet<string>>()))
                .Returns(false);

            NewRule(dd07Mock.Object, larsDataServiceMock.Object).ConditionMet(origLearnStartDate, fundModel, progType, learnAimRef)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ExcludeConditionMet_True()
        {
            var origLearnStartDate = new DateTime(2018, 10, 01);

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(It.IsAny<int>())).Returns(true);

            NewRule(dd07Mock.Object, new Mock<ILARSDataService>().Object).ConditionMet(origLearnStartDate, 35, It.IsAny<int>(), It.IsAny<string>())
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var origLearnStartDate = new DateTime(2018, 10, 01);
            var fundModel = 35;
            var progType = 22;
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
                .Setup(ldsm => ldsm.OrigLearnStartDateBetweenStartAndEndDateForAnyValidityCategory(origLearnStartDate, learnAimRef, It.IsAny<HashSet<string>>()))
                .Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(dd07Mock.Object, larsDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError()
        {
            var origLearnStartDate = new DateTime(2018, 10, 01);
            var fundModel = 35;
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
                .Setup(ldsm => ldsm.OrigLearnStartDateBetweenStartAndEndDateForAnyValidityCategory(origLearnStartDate, learnAimRef, It.IsAny<HashSet<string>>()))
                .Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dd07Mock.Object, larsDataServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
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

        private OrigLearnStartDate_06Rule NewRule(
            IDerivedData_07Rule dd07 = null,
            ILARSDataService larsDataService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new OrigLearnStartDate_06Rule(dd07, larsDataService, validationErrorHandler);
        }
    }
}
