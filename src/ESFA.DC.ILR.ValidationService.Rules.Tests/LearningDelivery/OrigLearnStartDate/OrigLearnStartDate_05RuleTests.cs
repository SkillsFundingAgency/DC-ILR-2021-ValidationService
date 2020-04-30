using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.OrigLearnStartDate;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.OrigLearnStartDate
{
    public class OrigLearnStartDate_05RuleTests : AbstractRuleTests<OrigLearnStartDate_05Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("OrigLearnStartDate_05");
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
            DateTime? origLearnStartDate = null;

            NewRule().OrigLearnStartDateConditionMet(origLearnStartDate).Should().BeFalse();
        }

        [Theory]
        [InlineData(35)]
        [InlineData(36)]
        public void FundModelConditionMet_True(int fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeTrue();
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            var fundModel = 0;

            NewRule().FundModelConditionMet(fundModel).Should().BeFalse();
        }

        [Fact]
        public void DD07ConditionMet_True()
        {
            var progType = 22;

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            NewRule(dd07Mock.Object).DD07ConditionMet(progType).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        public void DD07ConditionMet_False(int? progType)
        {
            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(false);

            NewRule(dd07Mock.Object).DD07ConditionMet(progType).Should().BeFalse();
        }

        [Fact]
        public void AimTypeConditionMet_True()
        {
            var aimType = 3;

            NewRule().AimTypeConditionMet(aimType).Should().BeTrue();
        }

        [Fact]
        public void AimTypeConditionMet_False()
        {
            var aimType = 0;

            NewRule().AimTypeConditionMet(aimType).Should().BeFalse();
        }

        [Fact]
        public void LARSConditionMet_True()
        {
            var origLearnStartDate = new DateTime(2018, 10, 01);
            var learnAimRef = "123456789";

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.OrigLearnStartDateBetweenStartAndEndDateForValidityCategory(origLearnStartDate, learnAimRef, LARSValidities.Apprenticeships))
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
                .Setup(ldsm => ldsm.OrigLearnStartDateBetweenStartAndEndDateForValidityCategory(origLearnStartDate, learnAimRef, LARSValidities.Apprenticeships))
                .Returns(true);

            NewRule(larsDataService: larsDataServiceMock.Object).LARSConditionMet(origLearnStartDate, learnAimRef).Should().BeFalse();
        }

        [Fact]
        public void Excluded_True()
        {
            var progType = 25;

            NewRule().Excluded(progType).Should().BeTrue();
        }

        [Fact]
        public void Excluded_False()
        {
            var progType = 0;

            NewRule().Excluded(progType).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var origLearnStartDate = new DateTime(2018, 10, 01);
            var fundModel = 35;
            var progType = 22;
            var aimType = 3;
            var learnAimRef = "123456789";

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.OrigLearnStartDateBetweenStartAndEndDateForValidityCategory(origLearnStartDate, learnAimRef, LARSValidities.Apprenticeships))
                .Returns(false);

            NewRule(dd07Mock.Object, larsDataServiceMock.Object).ConditionMet(origLearnStartDate, fundModel, progType, aimType, learnAimRef)
                .Should()
                .BeTrue();
        }

        [Fact]
        public void ConditionMet_FalseFundModel()
        {
            var origLearnStartDate = new DateTime(2018, 10, 01);
            var fundModel = 1;
            var progType = 22;
            var aimType = 3;
            var learnAimRef = "123456789";

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.OrigLearnStartDateBetweenStartAndEndDateForValidityCategory(origLearnStartDate, learnAimRef, LARSValidities.Apprenticeships))
                .Returns(false);

            NewRule(dd07Mock.Object, larsDataServiceMock.Object).ConditionMet(origLearnStartDate, fundModel, progType, aimType, learnAimRef)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseDD07()
        {
            var origLearnStartDate = new DateTime(2018, 10, 01);
            var fundModel = 35;
            var progType = 22;
            var aimType = 3;
            var learnAimRef = "123456789";

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(false);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.OrigLearnStartDateBetweenStartAndEndDateForValidityCategory(origLearnStartDate, learnAimRef, LARSValidities.Apprenticeships))
                .Returns(false);

            NewRule(dd07Mock.Object, larsDataServiceMock.Object).ConditionMet(origLearnStartDate, fundModel, progType, aimType, learnAimRef)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseAimType()
        {
            var origLearnStartDate = new DateTime(2018, 10, 01);
            var fundModel = 35;
            var progType = 22;
            var aimType = 1;
            var learnAimRef = "123456789";

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.OrigLearnStartDateBetweenStartAndEndDateForValidityCategory(origLearnStartDate, learnAimRef, LARSValidities.Apprenticeships))
                .Returns(false);

            NewRule(dd07Mock.Object, larsDataServiceMock.Object).ConditionMet(origLearnStartDate, fundModel, progType, aimType, learnAimRef)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseLars()
        {
            var origLearnStartDate = new DateTime(2018, 10, 01);
            var fundModel = 35;
            var progType = 22;
            var aimType = 3;
            var learnAimRef = "123456789";

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.OrigLearnStartDateBetweenStartAndEndDateForValidityCategory(origLearnStartDate, learnAimRef, LARSValidities.Apprenticeships))
                .Returns(true);

            NewRule(dd07Mock.Object, larsDataServiceMock.Object).ConditionMet(origLearnStartDate, fundModel, progType, aimType, learnAimRef)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ConditionMet_FalseExcluded()
        {
            var origLearnStartDate = new DateTime(2018, 10, 01);
            var fundModel = 35;
            var progType = 25;
            var aimType = 3;
            var learnAimRef = "123456789";

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.OrigLearnStartDateBetweenStartAndEndDateForValidityCategory(origLearnStartDate, learnAimRef, LARSValidities.Apprenticeships))
                .Returns(false);

            NewRule(dd07Mock.Object, larsDataServiceMock.Object).ConditionMet(origLearnStartDate, fundModel, progType, aimType, learnAimRef)
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var origLearnStartDate = new DateTime(2018, 10, 01);
            var fundModel = 35;
            var progType = 22;
            var aimType = 3;
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
                        AimType = aimType,
                        LearnAimRef = learnAimRef
                    }
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.OrigLearnStartDateBetweenStartAndEndDateForValidityCategory(origLearnStartDate, learnAimRef, LARSValidities.Apprenticeships))
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
            var aimType = 3;
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
                        AimType = aimType,
                        LearnAimRef = learnAimRef
                    }
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            var larsDataServiceMock = new Mock<ILARSDataService>();
            larsDataServiceMock
                .Setup(ldsm => ldsm.OrigLearnStartDateBetweenStartAndEndDateForValidityCategory(origLearnStartDate, learnAimRef, LARSValidities.Apprenticeships))
                .Returns(false);

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
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("FundModel", 1)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("AimType", 1)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2017, 1, 1), 1, 1);

            validationErrorHandlerMock.Verify();
        }

        private OrigLearnStartDate_05Rule NewRule(
            IDerivedData_07Rule dd07 = null,
            ILARSDataService larsDataService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new OrigLearnStartDate_05Rule(dd07, larsDataService, validationErrorHandler);
        }
    }
}
