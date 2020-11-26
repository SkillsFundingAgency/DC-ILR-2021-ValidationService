using System;
using System.Collections.Generic;
using System.Linq;
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
    public class DateOfBirth_36RuleTests : AbstractRuleTests<DateOfBirth_36Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("DateOfBirth_36");
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(FundModels.ApprenticeshipsFrom1May2017).Should().BeFalse();
        }

        [Theory]
        [InlineData(FundModels.AdultSkills)]
        [InlineData(FundModels.OtherAdult)]
        public void FundModelConditionMet_True(int fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeTrue();
        }

        [Theory]
        [InlineData(null, "2012-01-01")]
        [InlineData("1994-01-01", "2012-01-01")]
        public void DateOfBirthConditionMet_False(string dateOfBirthString, string learnStartDateString)
        {
            DateTime? dateOfBirth = string.IsNullOrEmpty(dateOfBirthString) ? (DateTime?)null : DateTime.Parse(dateOfBirthString);
            DateTime learnStartDate = DateTime.Parse(learnStartDateString);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(dd => dd.YearsBetween(dateOfBirth ?? new DateTime(2010, 05, 01), learnStartDate)).Returns(18);

            NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object).DateOfBirthConditionMet(dateOfBirth, learnStartDate).Should().BeFalse();
        }

        [Fact]
        public void DateOfBirthConditionMet_True()
        {
            DateTime dateOfBirth = new DateTime(1999, 05, 01);
            DateTime learnStartDate = new DateTime(2017, 05, 01);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(dd => dd.YearsBetween(dateOfBirth, learnStartDate)).Returns(20);

            NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object).DateOfBirthConditionMet(dateOfBirth, learnStartDate).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(25)]
        public void DD07ConditionMet_False(int? progType)
        {
            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(true);

            NewRule(dd07: dd07Mock.Object).DD07ConditionMet(progType).Should().BeFalse();
        }

        [Fact]
        public void DD07ConditionMet_True()
        {
            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dd => dd.IsApprenticeship(23)).Returns(false);

            NewRule(dd07: dd07Mock.Object).DD07ConditionMet(23).Should().BeFalse();
        }

        [Fact]
        public void AimTypeConditionMet_False()
        {
            NewRule().AimTypeConditionMet(3).Should().BeFalse();
        }

        [Fact]
        public void AimTypeConditionMet_True()
        {
            NewRule().AimTypeConditionMet(1).Should().BeTrue();
        }

        [Fact]
        public void ApprenticeshipDurationConditionMet_False()
        {
            DateTime learnStartDate = new DateTime(2017, 06, 01);
            DateTime learnPlanEndDate = new DateTime(2018, 07, 01);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(dd => dd.MonthsBetween(learnStartDate, learnPlanEndDate)).Returns(20);

            NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object).ApprenticeshipDurationConditionMet(learnStartDate, learnPlanEndDate).Should().BeFalse();
        }

        [Fact]
        public void ApprenticeshipDurationConditionMet_True()
        {
            DateTime learnStartDate = new DateTime(2016, 05, 01);
            DateTime learnPlanEndDate = new DateTime(2016, 09, 01);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(dd => dd.MonthsBetween(learnStartDate, learnPlanEndDate)).Returns(5);

            NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object).ApprenticeshipDurationConditionMet(learnStartDate, learnPlanEndDate).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryFAMTypeConditionMet_False()
        {
            IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT },
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.RES }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMsQueryServiceMock.Setup(dd => dd.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.RES)).Returns(true);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).LearningDeliveryFAMTypeConditionMet(learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMTypeConditionMet_True()
        {
            IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT },
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.ADL }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMsQueryServiceMock.Setup(dd => dd.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.RES)).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).LearningDeliveryFAMTypeConditionMet(learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryFAMTypeConditionMet_True_Null()
        {
            IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT },
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.ADL }
            };

            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMsQueryServiceMock.Setup(dd => dd.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.RES)).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).LearningDeliveryFAMTypeConditionMet(null).Should().BeTrue();
        }

        [Theory]
        [InlineData(FundModels.ApprenticeshipsFrom1May2017, null, "2016-01-12", null, 2, "2016-01-10")]
        [InlineData(FundModels.ApprenticeshipsFrom1May2017, "2002-05-01", "2016-12-01", null, 3, "2016-10-01")]
        [InlineData(FundModels.ApprenticeshipsFrom1May2017, "2002-05-01", "2016-12-01", 25, 3, "2016-10-01")]
        public void ConditionMet_False(
            int fundModel,
            string dateOfBirthString,
            string learnStartDateString,
            int? progType,
            int aimType,
            string learnPlanEndDateString)
        {
            IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT },
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.RES }
            };

            DateTime learnStartDate = DateTime.Parse(learnStartDateString);
            DateTime? dateOfBirth = string.IsNullOrEmpty(dateOfBirthString) ? (DateTime?)null : DateTime.Parse(dateOfBirthString);
            DateTime learnPlanEndDate = DateTime.Parse(learnPlanEndDateString);

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);
            dateTimeQueryServiceMock.Setup(dd => dd.YearsBetween(dateOfBirth ?? new DateTime(2000, 06, 01), learnStartDate)).Returns(18);
            dateTimeQueryServiceMock.Setup(dd => dd.MonthsBetween(learnStartDate, learnPlanEndDate)).Returns(13);
            learningDeliveryFAMsQueryServiceMock.Setup(dd => dd.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.RES)).Returns(true);

            NewRule(
                dd07: dd07Mock.Object,
                dateTimeQueryService: dateTimeQueryServiceMock.Object,
                learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).ConditionMet(fundModel, dateOfBirth, learnStartDate, progType, aimType, learnPlanEndDate, learningDeliveryFAMs).Should().BeFalse();
        }

        [Theory]
        [InlineData(FundModels.AdultSkills, "2001-01-01", "2018-01-01", 23, 1, "2018-03-01")]
        [InlineData(FundModels.OtherAdult, "2001-01-05", "2018-01-01", 23, 1, "2018-03-01")]
        public void ConditionMet_True(
            int fundModel,
            string dateOfBirthString,
            string learnStartDateString,
            int? progType,
            int aimType,
            string learnPlanEndDateString)
        {
            IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT },
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.ADL }
            };

            DateTime learnStartDate = DateTime.Parse(learnStartDateString);
            DateTime? dateOfBirth = string.IsNullOrEmpty(dateOfBirthString) ? (DateTime?)null : DateTime.Parse(dateOfBirthString);
            DateTime learnPlanEndDate = DateTime.Parse(learnPlanEndDateString);

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(true);
            dateTimeQueryServiceMock.Setup(dd => dd.YearsBetween(dateOfBirth ?? new DateTime(2000, 06, 01), learnStartDate)).Returns(20);
            dateTimeQueryServiceMock.Setup(dd => dd.MonthsBetween(learnStartDate, learnPlanEndDate)).Returns(5);
            learningDeliveryFAMsQueryServiceMock.Setup(dd => dd.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.RES)).Returns(false);

            NewRule(
                dd07: dd07Mock.Object,
                dateTimeQueryService: dateTimeQueryServiceMock.Object,
                learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).ConditionMet(fundModel, dateOfBirth, learnStartDate, progType, aimType, learnPlanEndDate, learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT },
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.ADL }
            };

            ILearner learner = new TestLearner()
            {
                DateOfBirthNullable = new DateTime(1998, 01, 01),
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        LearnStartDate = new DateTime(2018, 01, 01),
                        LearnPlanEndDate = new DateTime(2018, 05, 01),
                        FundModel = FundModels.AdultSkills,
                        ProgTypeNullable = 23,
                        LearningDeliveryFAMs = learningDeliveryFAMs.ToList()
                    }
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(23)).Returns(true);
            dateTimeQueryServiceMock.Setup(dd => dd.YearsBetween(new DateTime(1998, 01, 01), new DateTime(2018, 01, 01))).Returns(20);
            dateTimeQueryServiceMock.Setup(dd => dd.MonthsBetween(new DateTime(2018, 01, 01), new DateTime(2018, 05, 01))).Returns(5);
            learningDeliveryFAMsQueryServiceMock.Setup(dd => dd.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.RES)).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    dd07: dd07Mock.Object,
                    dateTimeQueryService: dateTimeQueryServiceMock.Object,
                    learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object,
                    validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT },
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.RES }
            };

            ILearner learner = new TestLearner()
            {
                DateOfBirthNullable = new DateTime(1996, 01, 01),
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        LearnStartDate = new DateTime(2016, 06, 01),
                        LearnPlanEndDate = new DateTime(2018, 01, 01),
                        FundModel = FundModels.ApprenticeshipsFrom1May2017,
                        OutcomeNullable = 2,
                        ProgTypeNullable = 25,
                        LearningDeliveryFAMs = learningDeliveryFAMs.ToList()
                    }
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(25)).Returns(false);
            dateTimeQueryServiceMock.Setup(dd => dd.YearsBetween(new DateTime(1996, 01, 01), new DateTime(2016, 06, 01))).Returns(18);
            dateTimeQueryServiceMock.Setup(dd => dd.MonthsBetween(new DateTime(2016, 06, 01), new DateTime(2018, 01, 01))).Returns(15);
            learningDeliveryFAMsQueryServiceMock.Setup(dd => dd.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.RES)).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    dd07: dd07Mock.Object,
                    dateTimeQueryService: dateTimeQueryServiceMock.Object,
                    learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object,
                    validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();
            validationErrorHandlerMock.Setup(dd => dd.BuildErrorMessageParameter(PropertyNameConstants.DateOfBirth, "01/01/2001")).Verifiable();
            validationErrorHandlerMock.Setup(dd => dd.BuildErrorMessageParameter(PropertyNameConstants.AimType, 1)).Verifiable();
            validationErrorHandlerMock.Setup(dd => dd.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, "01/06/2011")).Verifiable();
            validationErrorHandlerMock.Setup(dd => dd.BuildErrorMessageParameter(PropertyNameConstants.LearnPlanEndDate, "01/06/2014")).Verifiable();
            validationErrorHandlerMock.Setup(dd => dd.BuildErrorMessageParameter(PropertyNameConstants.FundModel, FundModels.AdultSkills)).Verifiable();
            validationErrorHandlerMock.Setup(dd => dd.BuildErrorMessageParameter(PropertyNameConstants.ProgType, 23)).Verifiable();
            validationErrorHandlerMock.Setup(dd => dd.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.RES)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(
                new DateTime(2001, 01, 01),
                1,
                new DateTime(2011, 06, 01),
                new DateTime(2014, 06, 01),
                FundModels.AdultSkills,
                23,
                LearningDeliveryFAMTypeConstants.RES);
            validationErrorHandlerMock.Verify();
        }

        public DateOfBirth_36Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            IDerivedData_07Rule dd07 = null,
            IDateTimeQueryService dateTimeQueryService = null,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null)
        {
            return new DateOfBirth_36Rule(
                validationErrorHandler: validationErrorHandler,
                dd07: dd07,
                dateTimeQueryService: dateTimeQueryService,
                learningDeliveryFAMQueryService: learningDeliveryFAMQueryService);
        }
    }
}
