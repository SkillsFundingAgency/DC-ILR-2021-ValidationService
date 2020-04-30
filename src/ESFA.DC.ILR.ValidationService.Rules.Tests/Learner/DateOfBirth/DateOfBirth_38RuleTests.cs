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
    public class DateOfBirth_38RuleTests : AbstractRuleTests<DateOfBirth_38Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("DateOfBirth_38");
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
            dateTimeQueryServiceMock.Setup(dd => dd.YearsBetween(dateOfBirth ?? new DateTime(2010, 05, 01), learnStartDate)).Returns(20);

            NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object).DateOfBirthConditionMet(dateOfBirth, learnStartDate).Should().BeFalse();
        }

        [Fact]
        public void DateOfBirthConditionMet_True()
        {
            DateTime dateOfBirth = new DateTime(1999, 05, 01);
            DateTime learnStartDate = new DateTime(2017, 05, 01);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(dd => dd.YearsBetween(dateOfBirth, learnStartDate)).Returns(18);

            NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object).DateOfBirthConditionMet(dateOfBirth, learnStartDate).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(25)]
        public void DD07ConditionMet_False(int? progType)
        {
            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);

            NewRule(dd07: dd07Mock.Object).DD07ConditionMet(progType).Should().BeFalse();
        }

        [Fact]
        public void DD07ConditionMet_True()
        {
            var dd07Mock = new Mock<IDerivedData_07Rule>();
            dd07Mock.Setup(dd => dd.IsApprenticeship(23)).Returns(true);

            NewRule(dd07: dd07Mock.Object).DD07ConditionMet(23).Should().BeTrue();
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

        [Theory]
        [InlineData("2017-06-01", null)]
        [InlineData("2017-06-01", "2018-07-01")]
        public void ApprenticeshipDurationConditionMet_False(string learnStartDateString, string learnActEndDateString)
        {
            DateTime learnStartDate = DateTime.Parse(learnStartDateString);
            DateTime? learnActEndDate = string.IsNullOrEmpty(learnActEndDateString) ? (DateTime?)null : DateTime.Parse(learnActEndDateString);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(dd => dd.DaysBetween(learnStartDate, learnActEndDate ?? new DateTime(2018, 07, 01))).Returns(367);

            var rule38 = NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object).ApprenticeshipDurationConditionMet(learnStartDate, learnActEndDate);
            rule38.Should().BeFalse();
        }

        [Fact]
        public void ApprenticeshipDurationConditionMet_True()
        {
            DateTime learnStartDate = new DateTime(2016, 05, 01);
            DateTime learnActEndDate = new DateTime(2016, 04, 01);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            dateTimeQueryServiceMock.Setup(dd => dd.DaysBetween(learnStartDate, learnActEndDate)).Returns(11);

            var rule38 = NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object).ApprenticeshipDurationConditionMet(learnStartDate, learnActEndDate);
            rule38.Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(4)]
        public void OutComeConditionMet_False(int? outCome)
        {
            NewRule().OutComeConditionMet(outCome).Should().BeFalse();
        }

        [Fact]
        public void OutComeConditionMet_True()
        {
            NewRule().OutComeConditionMet(1).Should().BeTrue();
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
        [InlineData(FundModels.ApprenticeshipsFrom1May2017, null, "2016-12-01", null, 2, null, null)]
        [InlineData(FundModels.ApprenticeshipsFrom1May2017, "2002-05-01", "2016-12-01", null, 3, null, null)]
        [InlineData(FundModels.ApprenticeshipsFrom1May2017, "2002-05-01", "2016-12-01", 25, 3, null, null)]
        [InlineData(FundModels.ApprenticeshipsFrom1May2017, "2002-05-01", "2016-12-01", 25, 3, "2016-10-01", null)]
        [InlineData(FundModels.ApprenticeshipsFrom1May2017, "2002-05-01", "2016-12-01", 25, 3, "2016-10-01", 3)]
        public void ConditionMet_False(
            int fundModel,
            string dateOfBirthString,
            string learnStartDateString,
            int? progType,
            int aimType,
            string learnActEndDateString,
            int? outCome)
        {
            IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT },
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.RES }
            };

            DateTime learnStartDate = DateTime.Parse(learnStartDateString);
            DateTime? dateOfBirth = string.IsNullOrEmpty(dateOfBirthString) ? (DateTime?)null : DateTime.Parse(dateOfBirthString);
            DateTime? learnActEndDate = string.IsNullOrEmpty(learnActEndDateString) ? (DateTime?)null : DateTime.Parse(learnActEndDateString);

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);
            dateTimeQueryServiceMock.Setup(dd => dd.YearsBetween(dateOfBirth ?? new DateTime(2000, 06, 01), learnStartDate)).Returns(20);
            dateTimeQueryServiceMock.Setup(dd => dd.DaysBetween(learnStartDate, learnActEndDate ?? new DateTime(2017, 02, 01))).Returns(13);
            learningDeliveryFAMsQueryServiceMock.Setup(dd => dd.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.RES)).Returns(true);

            NewRule(
                dd07: dd07Mock.Object,
                dateTimeQueryService: dateTimeQueryServiceMock.Object,
                learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).ConditionMet(fundModel, dateOfBirth, learnStartDate, progType, aimType, learnActEndDate, outCome, learningDeliveryFAMs).Should().BeFalse();
        }

        [Theory]
        [InlineData(FundModels.AdultSkills, "2001-01-01", "2016-12-01", 23, 1, "2017-03-01", 1)]
        [InlineData(FundModels.OtherAdult, "2001-05-01", "2016-12-01", 23, 1, "2017-03-01", 1)]
        public void ConditionMet_True(
            int fundModel,
            string dateOfBirthString,
            string learnStartDateString,
            int? progType,
            int aimType,
            string learnActEndDateString,
            int? outCome)
        {
            IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.ACT },
                new TestLearningDeliveryFAM() { LearnDelFAMType = LearningDeliveryFAMTypeConstants.ADL }
            };

            DateTime learnStartDate = DateTime.Parse(learnStartDateString);
            DateTime? dateOfBirth = string.IsNullOrEmpty(dateOfBirthString) ? (DateTime?)null : DateTime.Parse(dateOfBirthString);
            DateTime? learnActEndDate = string.IsNullOrEmpty(learnActEndDateString) ? (DateTime?)null : DateTime.Parse(learnActEndDateString);

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(true);
            dateTimeQueryServiceMock.Setup(dd => dd.YearsBetween(dateOfBirth ?? new DateTime(2000, 06, 01), learnStartDate)).Returns(15);
            dateTimeQueryServiceMock.Setup(dd => dd.DaysBetween(learnStartDate, learnActEndDate ?? new DateTime(2017, 02, 01))).Returns(10);
            learningDeliveryFAMsQueryServiceMock.Setup(dd => dd.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.RES)).Returns(false);

            NewRule(
                dd07: dd07Mock.Object,
                dateTimeQueryService: dateTimeQueryServiceMock.Object,
                learningDeliveryFAMQueryService: learningDeliveryFAMsQueryServiceMock.Object).ConditionMet(fundModel, dateOfBirth, learnStartDate, progType, aimType, learnActEndDate, outCome, learningDeliveryFAMs).Should().BeTrue();
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
                DateOfBirthNullable = new DateTime(2001, 01, 01),
                LearningDeliveries = new TestLearningDelivery[]
                {
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        LearnStartDate = new DateTime(2016, 06, 01),
                        LearnActEndDateNullable = new DateTime(2017, 01, 01),
                        FundModel = FundModels.AdultSkills,
                        OutcomeNullable = 1,
                        ProgTypeNullable = 23,
                        LearningDeliveryFAMs = learningDeliveryFAMs.ToList()
                    }
                }
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var learningDeliveryFAMsQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(23)).Returns(true);
            dateTimeQueryServiceMock.Setup(dd => dd.YearsBetween(new DateTime(2001, 01, 01), new DateTime(2016, 06, 01))).Returns(15);
            dateTimeQueryServiceMock.Setup(dd => dd.DaysBetween(new DateTime(2016, 06, 01), new DateTime(2017, 01, 01))).Returns(10);
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
                        LearnActEndDateNullable = new DateTime(2018, 01, 01),
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
            dateTimeQueryServiceMock.Setup(dd => dd.YearsBetween(new DateTime(1996, 01, 01), new DateTime(2016, 06, 01))).Returns(20);
            dateTimeQueryServiceMock.Setup(dd => dd.DaysBetween(new DateTime(2016, 06, 01), new DateTime(2018, 01, 01))).Returns(15);
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
            validationErrorHandlerMock.Setup(dd => dd.BuildErrorMessageParameter(PropertyNameConstants.FundModel, FundModels.AdultSkills)).Verifiable();
            validationErrorHandlerMock.Setup(dd => dd.BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.RES)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2001, 01, 01), 1, new DateTime(2011, 06, 01), FundModels.AdultSkills, LearningDeliveryFAMTypeConstants.RES);
            validationErrorHandlerMock.Verify();
        }

        public DateOfBirth_38Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            IDerivedData_07Rule dd07 = null,
            IDateTimeQueryService dateTimeQueryService = null,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null)
        {
            return new DateOfBirth_38Rule(
                validationErrorHandler: validationErrorHandler,
                dd07: dd07,
                dateTimeQueryService: dateTimeQueryService,
                learningDeliveryFAMQueryService: learningDeliveryFAMQueryService);
        }
    }
}
