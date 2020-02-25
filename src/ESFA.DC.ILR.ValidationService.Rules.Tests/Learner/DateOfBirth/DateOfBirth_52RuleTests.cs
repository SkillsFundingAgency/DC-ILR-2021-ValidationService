using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.DateOfBirth
{
    public class DateOfBirth_52RuleTests : AbstractRuleTests<DateOfBirth_52Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("DateOfBirth_52");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learnStartDate = new DateTime(2017, 12, 02);
            var learnPlanEndDate = new DateTime(2018, 08, 01);
            var fundModel = TypeOfFunding.ApprenticeshipsFrom1May2017;
            var progType = TypeOfLearningProgramme.AdvancedLevelApprenticeship;
            var aimType = TypeOfAim.ProgrammeAim;

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.WholeDaysBetween(learnStartDate, learnPlanEndDate)).Returns(242);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.RES))
                .Returns(false);

            NewRule(dd07Mock.Object, learningDeliveryFamQueryServiceMock.Object, dateTimeQueryServiceMock.Object)
                .ConditionMet(fundModel, progType, aimType, learnStartDate, learnPlanEndDate, It.IsAny<IEnumerable<ILearningDeliveryFAM>>())
                .Should()
                .BeTrue();
        }

        [Theory]
        [InlineData(0, true)]
        [InlineData(25, false)]
        public void ConditionMet_False_IsExcluded(int? progType, bool learningDeliveryMockResult)
        {
            var learnStartDate = new DateTime(2017, 08, 01);
            var learnPlanEndDate = new DateTime(2018, 01, 01);
            var fundModel = TypeOfFunding.ApprenticeshipsFrom1May2017;
            var aimType = TypeOfAim.ProgrammeAim;

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.WholeDaysBetween(learnStartDate, learnPlanEndDate)).Returns(5);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.RES))
                .Returns(learningDeliveryMockResult);

            NewRule(dd07Mock.Object, learningDeliveryFamQueryServiceMock.Object, dateTimeQueryServiceMock.Object)
                .ConditionMet(fundModel, progType, aimType, learnStartDate, learnPlanEndDate, It.IsAny<IEnumerable<ILearningDeliveryFAM>>())
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FundModel()
        {
            var learnStartDate = new DateTime(2017, 08, 01);
            var learnPlanEndDate = new DateTime(2018, 01, 01);
            var fundModel = 0;
            var progType = TypeOfLearningProgramme.AdvancedLevelApprenticeship;
            var aimType = TypeOfAim.ProgrammeAim;

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.WholeDaysBetween(learnStartDate, learnPlanEndDate)).Returns(5);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.RES))
                .Returns(false);

            NewRule(dd07Mock.Object, learningDeliveryFamQueryServiceMock.Object, dateTimeQueryServiceMock.Object)
                .ConditionMet(fundModel, progType, aimType, learnStartDate, learnPlanEndDate, It.IsAny<IEnumerable<ILearningDeliveryFAM>>())
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ConditionMet_False_LearnStartDateLessThanMayFirst2017()
        {
            var learnStartDate = new DateTime(2016, 08, 01);
            var learnPlanEndDate = new DateTime(2018, 01, 01);
            var fundModel = TypeOfFunding.ApprenticeshipsFrom1May2017;
            var progType = TypeOfLearningProgramme.AdvancedLevelApprenticeship;
            var aimType = TypeOfAim.ProgrammeAim;

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.WholeDaysBetween(learnStartDate, learnPlanEndDate)).Returns(5);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.RES))
                .Returns(false);

            NewRule(dd07Mock.Object, learningDeliveryFamQueryServiceMock.Object, dateTimeQueryServiceMock.Object)
                .ConditionMet(fundModel, progType, aimType, learnStartDate, learnPlanEndDate, It.IsAny<IEnumerable<ILearningDeliveryFAM>>())
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ConditionMet_False_NotApprenticeship()
        {
            var learnStartDate = new DateTime(2017, 08, 01);
            var learnPlanEndDate = new DateTime(2018, 01, 01);
            var fundModel = TypeOfFunding.ApprenticeshipsFrom1May2017;
            var progType = 0;
            var aimType = TypeOfAim.ProgrammeAim;

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(false);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.WholeDaysBetween(learnStartDate, learnPlanEndDate)).Returns(5);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.RES))
                .Returns(false);

            NewRule(dd07Mock.Object, learningDeliveryFamQueryServiceMock.Object, dateTimeQueryServiceMock.Object)
                .ConditionMet(fundModel, progType, aimType, learnStartDate, learnPlanEndDate, It.IsAny<IEnumerable<ILearningDeliveryFAM>>())
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ConditionMet_False_AimType()
        {
            var learnStartDate = new DateTime(2017, 08, 01);
            var learnPlanEndDate = new DateTime(2018, 01, 01);
            var fundModel = TypeOfFunding.ApprenticeshipsFrom1May2017;
            var progType = TypeOfLearningProgramme.AdvancedLevelApprenticeship;
            var aimType = 0;

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.WholeDaysBetween(learnStartDate, learnPlanEndDate)).Returns(5);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.RES))
                .Returns(false);

            NewRule(dd07Mock.Object, learningDeliveryFamQueryServiceMock.Object, dateTimeQueryServiceMock.Object)
                .ConditionMet(fundModel, progType, aimType, learnStartDate, learnPlanEndDate, It.IsAny<IEnumerable<ILearningDeliveryFAM>>())
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ConditionMet_False_DaysEqualTo365()
        {
            var learnStartDate = new DateTime(2018, 08, 01);
            var learnPlanEndDate = new DateTime(2019, 08, 02);
            var fundModel = TypeOfFunding.ApprenticeshipsFrom1May2017;
            var progType = TypeOfLearningProgramme.AdvancedLevelApprenticeship;
            var aimType = TypeOfAim.ProgrammeAim;

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.WholeDaysBetween(learnStartDate, learnPlanEndDate)).Returns(365);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.RES))
                .Returns(false);

            NewRule(dd07Mock.Object, learningDeliveryFamQueryServiceMock.Object, dateTimeQueryServiceMock.Object)
                .ConditionMet(fundModel, progType, aimType, learnStartDate, learnPlanEndDate, It.IsAny<IEnumerable<ILearningDeliveryFAM>>())
                .Should()
                .BeFalse();

            dateTimeQueryServiceMock.Verify(qs => qs.WholeDaysBetween(learnStartDate, learnPlanEndDate), Times.AtLeastOnce);
        }

        [Fact]
        public void ConditionMet_False_DaysGreaterThan365()
        {
            var learnStartDate = new DateTime(2018, 08, 01);
            var learnPlanEndDate = new DateTime(2019, 08, 12);
            var fundModel = TypeOfFunding.ApprenticeshipsFrom1May2017;
            var progType = TypeOfLearningProgramme.AdvancedLevelApprenticeship;
            var aimType = TypeOfAim.ProgrammeAim;

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.WholeDaysBetween(learnStartDate, learnPlanEndDate)).Returns(370);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.RES))
                .Returns(false);

            NewRule(dd07Mock.Object, learningDeliveryFamQueryServiceMock.Object, dateTimeQueryServiceMock.Object)
                .ConditionMet(fundModel, progType, aimType, learnStartDate, learnPlanEndDate, It.IsAny<IEnumerable<ILearningDeliveryFAM>>())
                .Should()
                .BeFalse();

            dateTimeQueryServiceMock.Verify(qs => qs.WholeDaysBetween(learnStartDate, learnPlanEndDate), Times.AtLeastOnce);
        }

        [Fact]
        public void ValidateError()
        {
            var learnStartDate = new DateTime(2017, 08, 01);
            var learnPlanEndDate = new DateTime(2018, 01, 01);
            var progType = TypeOfLearningProgramme.AdvancedLevelApprenticeship;

            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = learnStartDate,
                        LearnPlanEndDate = learnPlanEndDate,
                        FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                        ProgTypeNullable = progType,
                        AimType = TypeOfAim.ProgrammeAim
                    }
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.DaysBetween(learnStartDate, learnPlanEndDate)).Returns(5);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.RES))
                .Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(dd07Mock.Object, learningDeliveryFamQueryServiceMock.Object, dateTimeQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError()
        {
            var learnStartDate = new DateTime(2017, 08, 01);
            var learnPlanEndDate = new DateTime(2018, 01, 01);
            var progType = TypeOfLearningProgramme.AdvancedLevelApprenticeship;

            var learner = new TestLearner
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = learnStartDate,
                        LearnPlanEndDate = learnPlanEndDate,
                        FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                        ProgTypeNullable = progType,
                        AimType = TypeOfAim.ProgrammeAim
                    }
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dm => dm.IsApprenticeship(progType)).Returns(true);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.DaysBetween(learnStartDate, learnPlanEndDate)).Returns(5);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.RES))
                .Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dd07Mock.Object, learningDeliveryFamQueryServiceMock.Object, dateTimeQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        private DateOfBirth_52Rule NewRule(
            IDerivedData_07Rule dd07 = null,
            ILearningDeliveryFAMQueryService learningDeliveryFamQueryService = null,
            IDateTimeQueryService dateTimeQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new DateOfBirth_52Rule(dd07, learningDeliveryFamQueryService, dateTimeQueryService, validationErrorHandler);
        }
    }
}
