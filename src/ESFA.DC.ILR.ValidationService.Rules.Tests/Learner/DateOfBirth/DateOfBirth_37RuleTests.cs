using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.DateOfBirth
{
    public class DateOfBirth_37RuleTests : AbstractRuleTests<DateOfBirth_37Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("DateOfBirth_37");
        }

        [Fact]
        public void ConditionMet_True()
        {
            var dateOfBirth = new DateTime(1995, 08, 01);
            var learnStartDate = new DateTime(2015, 08, 01);
            var learnPlanEndDate = new DateTime(2016, 01, 01);
            var fundModel = FundModels.AdultSkills;
            var progType = TypeOfLearningProgramme.ApprenticeshipStandard;
            var aimType = TypeOfAim.ProgrammeAim;

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(20);
            dateTimeQueryServiceMock.Setup(qs => qs.DaysBetween(learnStartDate, learnPlanEndDate)).Returns(5);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.RES))
                .Returns(false);

            NewRule(dateTimeQueryServiceMock.Object, learningDeliveryFamQueryServiceMock.Object)
                .ConditionMet(learnStartDate, dateOfBirth, fundModel, progType, aimType, learnPlanEndDate, It.IsAny<IEnumerable<ILearningDeliveryFAM>>())
                .Should()
                .BeTrue();
        }

        [Fact]
        public void ConditionMet_False_IsExcluded()
        {
            var dateOfBirth = new DateTime(1995, 08, 01);
            var learnStartDate = new DateTime(2015, 08, 01);
            var learnPlanEndDate = new DateTime(2016, 01, 01);
            var fundModel = FundModels.AdultSkills;
            var progType = TypeOfLearningProgramme.ApprenticeshipStandard;
            var aimType = TypeOfAim.ProgrammeAim;

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(20);
            dateTimeQueryServiceMock.Setup(qs => qs.DaysBetween(learnStartDate, learnPlanEndDate)).Returns(5);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.RES))
                .Returns(true);

            NewRule(dateTimeQueryServiceMock.Object, learningDeliveryFamQueryServiceMock.Object)
                .ConditionMet(learnStartDate, dateOfBirth, fundModel, progType, aimType, learnPlanEndDate, It.IsAny<IEnumerable<ILearningDeliveryFAM>>())
                .Should()
                .BeFalse();
        }

        [Theory]
        [InlineData("2016-08-02")]
        [InlineData("2014-07-31")]
        public void ConditionMet_False_LearnStartDateNotInRange(string learnStartDateInput)
        {
            var dateOfBirth = new DateTime(1995, 08, 01);
            var learnStartDate = DateTime.Parse(learnStartDateInput);
            var learnPlanEndDate = new DateTime(2016, 01, 01);
            var fundModel = FundModels.AdultSkills;
            var progType = TypeOfLearningProgramme.ApprenticeshipStandard;
            var aimType = TypeOfAim.ProgrammeAim;

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(20);
            dateTimeQueryServiceMock.Setup(qs => qs.DaysBetween(learnStartDate, learnPlanEndDate)).Returns(5);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.RES))
                .Returns(false);

            NewRule(dateTimeQueryServiceMock.Object, learningDeliveryFamQueryServiceMock.Object)
                .ConditionMet(learnStartDate, dateOfBirth, fundModel, progType, aimType, learnPlanEndDate, It.IsAny<IEnumerable<ILearningDeliveryFAM>>())
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ConditionMet_False_FundModel()
        {
            var dateOfBirth = new DateTime(1995, 08, 01);
            var learnStartDate = new DateTime(2015, 08, 01);
            var learnPlanEndDate = new DateTime(2016, 01, 01);
            var fundModel = 0;
            var progType = TypeOfLearningProgramme.ApprenticeshipStandard;
            var aimType = TypeOfAim.ProgrammeAim;

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(20);
            dateTimeQueryServiceMock.Setup(qs => qs.DaysBetween(learnStartDate, learnPlanEndDate)).Returns(5);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.RES))
                .Returns(false);

            NewRule(dateTimeQueryServiceMock.Object, learningDeliveryFamQueryServiceMock.Object)
                .ConditionMet(learnStartDate, dateOfBirth, fundModel, progType, aimType, learnPlanEndDate, It.IsAny<IEnumerable<ILearningDeliveryFAM>>())
                .Should()
                .BeFalse();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(null)]
        public void ConditionMet_False_ProgType(int? progType)
        {
            var dateOfBirth = new DateTime(1995, 08, 01);
            var learnStartDate = new DateTime(2015, 08, 01);
            var learnPlanEndDate = new DateTime(2016, 01, 01);
            var fundModel = FundModels.AdultSkills;
            var aimType = TypeOfAim.ProgrammeAim;

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(20);
            dateTimeQueryServiceMock.Setup(qs => qs.DaysBetween(learnStartDate, learnPlanEndDate)).Returns(5);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.RES))
                .Returns(false);

            NewRule(dateTimeQueryServiceMock.Object, learningDeliveryFamQueryServiceMock.Object)
                .ConditionMet(learnStartDate, dateOfBirth, fundModel, progType, aimType, learnPlanEndDate, It.IsAny<IEnumerable<ILearningDeliveryFAM>>())
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ConditionMet_False_AimType()
        {
            var dateOfBirth = new DateTime(1995, 08, 01);
            var learnStartDate = new DateTime(2015, 08, 01);
            var learnPlanEndDate = new DateTime(2016, 01, 01);
            var fundModel = FundModels.AdultSkills;
            var progType = TypeOfLearningProgramme.ApprenticeshipStandard;
            var aimType = 0;

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(20);
            dateTimeQueryServiceMock.Setup(qs => qs.DaysBetween(learnStartDate, learnPlanEndDate)).Returns(5);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.RES))
                .Returns(false);

            NewRule(dateTimeQueryServiceMock.Object, learningDeliveryFamQueryServiceMock.Object)
                .ConditionMet(learnStartDate, dateOfBirth, fundModel, progType, aimType, learnPlanEndDate, It.IsAny<IEnumerable<ILearningDeliveryFAM>>())
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ConditionMet_False_YearsBetweenLessThan19()
        {
            var dateOfBirth = new DateTime(1997, 08, 01);
            var learnStartDate = new DateTime(2015, 08, 01);
            var learnPlanEndDate = new DateTime(2016, 01, 01);
            var fundModel = FundModels.AdultSkills;
            var progType = TypeOfLearningProgramme.ApprenticeshipStandard;
            var aimType = TypeOfAim.ProgrammeAim;

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(18);
            dateTimeQueryServiceMock.Setup(qs => qs.DaysBetween(learnStartDate, learnPlanEndDate)).Returns(5);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.RES))
                .Returns(false);

            NewRule(dateTimeQueryServiceMock.Object, learningDeliveryFamQueryServiceMock.Object)
                .ConditionMet(learnStartDate, dateOfBirth, fundModel, progType, aimType, learnPlanEndDate, It.IsAny<IEnumerable<ILearningDeliveryFAM>>())
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ConditionMet_True_DaysBetweenLessThan365()
        {
            var dateOfBirth = new DateTime(1995, 08, 01);
            var learnStartDate = new DateTime(2015, 08, 01);
            var learnPlanEndDate = new DateTime(2016, 09, 01);
            var fundModel = FundModels.AdultSkills;
            var progType = TypeOfLearningProgramme.ApprenticeshipStandard;
            var aimType = TypeOfAim.ProgrammeAim;

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(20);
            dateTimeQueryServiceMock.Setup(qs => qs.DaysBetween(learnStartDate, learnPlanEndDate)).Returns(15);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.RES))
                .Returns(false);

            NewRule(dateTimeQueryServiceMock.Object, learningDeliveryFamQueryServiceMock.Object)
                .ConditionMet(learnStartDate, dateOfBirth, fundModel, progType, aimType, learnPlanEndDate, It.IsAny<IEnumerable<ILearningDeliveryFAM>>())
                .Should()
                .BeTrue();
        }

        [Fact]
        public void ConditionMet_False_DaysBetweenEqualTo365()
        {
            var dateOfBirth = new DateTime(1995, 08, 01);
            var learnStartDate = new DateTime(2015, 08, 01);
            var learnPlanEndDate = new DateTime(2016, 09, 01);
            var fundModel = FundModels.AdultSkills;
            var progType = TypeOfLearningProgramme.ApprenticeshipStandard;
            var aimType = TypeOfAim.ProgrammeAim;

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(20);
            dateTimeQueryServiceMock.Setup(qs => qs.DaysBetween(learnStartDate, learnPlanEndDate)).Returns(365);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.RES))
                .Returns(false);

            NewRule(dateTimeQueryServiceMock.Object, learningDeliveryFamQueryServiceMock.Object)
                .ConditionMet(learnStartDate, dateOfBirth, fundModel, progType, aimType, learnPlanEndDate, It.IsAny<IEnumerable<ILearningDeliveryFAM>>())
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ConditionMet_False_DaysBetweenGreaterThan365()
        {
            var dateOfBirth = new DateTime(1995, 08, 01);
            var learnStartDate = new DateTime(2015, 08, 01);
            var learnPlanEndDate = new DateTime(2016, 09, 01);
            var fundModel = FundModels.AdultSkills;
            var progType = TypeOfLearningProgramme.ApprenticeshipStandard;
            var aimType = TypeOfAim.ProgrammeAim;

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(20);
            dateTimeQueryServiceMock.Setup(qs => qs.DaysBetween(learnStartDate, learnPlanEndDate)).Returns(367);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.RES))
                .Returns(false);

            NewRule(dateTimeQueryServiceMock.Object, learningDeliveryFamQueryServiceMock.Object)
                .ConditionMet(learnStartDate, dateOfBirth, fundModel, progType, aimType, learnPlanEndDate, It.IsAny<IEnumerable<ILearningDeliveryFAM>>())
                .Should()
                .BeFalse();
        }

        [Fact]
        public void ValidateError()
        {
            var dateOfBirth = new DateTime(1995, 08, 01);
            var learnStartDate = new DateTime(2015, 08, 01);
            var learnPlanEndDate = new DateTime(2016, 01, 01);

            var learner = new TestLearner()
            {
                DateOfBirthNullable = dateOfBirth,
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = learnStartDate,
                        LearnPlanEndDate = learnPlanEndDate,
                        FundModel = FundModels.AdultSkills,
                        ProgTypeNullable = TypeOfLearningProgramme.ApprenticeshipStandard,
                        AimType = TypeOfAim.ProgrammeAim
                    }
                }
            };

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(20);
            dateTimeQueryServiceMock.Setup(qs => qs.DaysBetween(learnStartDate, learnPlanEndDate)).Returns(5);

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.RES))
                .Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(dateTimeQueryServiceMock.Object, learningDeliveryFamQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ValidateNoError()
        {
            var dateOfBirth = new DateTime(1995, 08, 01);
            var learnStartDate = new DateTime(2015, 08, 01);
            var learnPlanEndDate = new DateTime(2016, 09, 01);

            var learner = new TestLearner()
            {
                DateOfBirthNullable = dateOfBirth,
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        LearnStartDate = learnStartDate,
                        LearnPlanEndDate = learnPlanEndDate,
                        FundModel = FundModels.AdultSkills,
                        ProgTypeNullable = TypeOfLearningProgramme.ApprenticeshipStandard,
                        AimType = TypeOfAim.ProgrammeAim
                    }
                }
            };

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(20);
            dateTimeQueryServiceMock.Setup(qs => qs.DaysBetween(learnStartDate, learnPlanEndDate)).Returns(367); // Failed Condition

            var learningDeliveryFamQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFamQueryServiceMock
                .Setup(qs => qs.HasLearningDeliveryFAMType(It.IsAny<IEnumerable<ILearningDeliveryFAM>>(), LearningDeliveryFAMTypeConstants.RES))
                .Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dateTimeQueryServiceMock.Object, learningDeliveryFamQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, "01/08/2016")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter(PropertyNameConstants.LearnPlanEndDate, "01/08/2017")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2016, 08, 01), new DateTime(2017, 08, 01));

            validationErrorHandlerMock.Verify();
        }

        private DateOfBirth_37Rule NewRule(
            IDateTimeQueryService dateTimeQueryService = null,
            ILearningDeliveryFAMQueryService learningDeliveryFamQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new DateOfBirth_37Rule(dateTimeQueryService, learningDeliveryFamQueryService, validationErrorHandler);
        }
    }
}
