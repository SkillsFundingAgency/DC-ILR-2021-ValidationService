using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.DateOfBirth
{
    public class DateOfBirth_35RuleTests : AbstractRuleTests<DateOfBirth_35Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("DateOfBirth_35");
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            NewRule().FundModelConditionMet(35).Should().BeTrue();
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(99).Should().BeFalse();
        }

        [Fact]
        public void LearnStartDateConditionMet_True()
        {
            NewRule().LearnStartDateConditionMet(new DateTime(2018, 08, 01)).Should().BeTrue();
        }

        [Fact]
        public void LearnStartDateConditionMet_False()
        {
            NewRule().LearnStartDateConditionMet(new DateTime(2014, 07, 01)).Should().BeFalse();
        }

        [Fact]
        public void AimTypeConditionMet_True()
        {
            NewRule().AimTypeConditionMet(1).Should().BeTrue();
        }

        [Fact]
        public void AimTypeConditionMet_False()
        {
            NewRule().AimTypeConditionMet(2).Should().BeFalse();
        }

        [Fact]
        public void DateOfBirthConditionMet_True()
        {
            var dateOfBirth = new DateTime(2000, 01, 01);
            var learnStartDate = new DateTime(2018, 08, 01);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(18);

            NewRule(dateTimeQueryServiceMock.Object).DateOfBirthConditionMet(dateOfBirth, learnStartDate).Should().BeTrue();
        }

        [Fact]
        public void DateOfBirthConditionMet_False()
        {
            var dateOfBirth = new DateTime(1990, 01, 01);
            var learnStartDate = new DateTime(2018, 08, 01);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(28);

            NewRule(dateTimeQueryServiceMock.Object).DateOfBirthConditionMet(dateOfBirth, learnStartDate).Should().BeFalse();
        }

        [Fact]
        public void DateOfBirthConditionMet_False_Null()
        {
            var dateOfBirth = new DateTime(1990, 01, 01);
            var learnStartDate = new DateTime(2018, 08, 01);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(28);

            NewRule(dateTimeQueryServiceMock.Object).DateOfBirthConditionMet(null, learnStartDate).Should().BeFalse();
        }

        [Fact]
        public void ApprenticeshipConditionMet_True()
        {
            var progType = 24;

            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(true);

            NewRule(dd07: dd07Mock.Object).ApprenticeshipConditionMet(progType).Should().BeTrue();
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData(25, true)]
        [InlineData(99, false)]
        public void ApprenticeshipConditionMet_False(int? progType, bool mockBool)
        {
            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(mockBool);

            NewRule(dd07: dd07Mock.Object).ApprenticeshipConditionMet(progType).Should().BeFalse();
        }

        [Theory]
        [InlineData("2016-07-31", "2017-07-29", true)] // triggers due to 364 Days
        public void ApprenticeshipDurationConditionMet_True(string learnStartDate, string learnplanEndDate, bool expectedResult)
        {
            DateTime startDate = DateTime.Parse(learnStartDate);
            DateTime planEndDate = DateTime.Parse(learnplanEndDate);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            var days = (planEndDate - startDate).TotalDays;
            double totalWholeDays = Math.Abs(days) + 1;
            dateTimeQueryServiceMock.Setup(x => x.WholeDaysBetween(startDate, planEndDate)).Returns(totalWholeDays);

            var rule35 = NewRule(dateTimeQueryServiceMock.Object).ApprenticeshipDurationConditionMet(startDate, planEndDate);
            rule35.Should().Be(expectedResult);

            dateTimeQueryServiceMock.Verify(x => x.WholeDaysBetween(startDate, planEndDate), Times.Exactly(1));
        }

        [Theory]
        [InlineData("2016-07-31", "2017-07-30", false)] // doesn't trigger due to 365 days
        [InlineData("2016-07-31", "2017-07-31", false)] // doesn't trigger due to 366 days
        [InlineData("2014-08-01", "2015-07-31", false)] // TestData: doesn't trigger due to 365 days
        public void ApprenticeshipDurationConditionMet_False(string learnStartDate, string learnplanEndDate, bool expectedResult)
        {
            var startDate = DateTime.Parse(learnStartDate);
            var planEndDate = DateTime.Parse(learnplanEndDate);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var days = (planEndDate - startDate).TotalDays;
            double totalWholeDays = Math.Abs(days) + 1;
            dateTimeQueryServiceMock.Setup(x => x.WholeDaysBetween(startDate, planEndDate)).Returns(totalWholeDays);

            var rule35 = NewRule(dateTimeQueryServiceMock.Object).ApprenticeshipDurationConditionMet(startDate, planEndDate);
            rule35.Should().BeFalse();

            dateTimeQueryServiceMock.Verify(x => x.WholeDaysBetween(startDate, planEndDate), Times.Exactly(1));
        }

        [Fact]
        public void LearningDeliveryFAMConditionMet_True()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "SOF",
                    LearnDelFAMCode = "100"
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES")).Returns(false);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMConditionMet(learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryFAMConditionMet_False()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "RES",
                    LearnDelFAMCode = "100"
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES")).Returns(true);

            NewRule(learningDeliveryFamQueryService: learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMConditionMet(learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learnStartDate = new DateTime(2018, 08, 01);
            var learnPlanEndDate = new DateTime(2019, 01, 01);
            var dateOfBirth = new DateTime(2000, 01, 01);
            var progType = 24;

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "LDM",
                    LearnDelFAMCode = "100"
                }
            };

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(18);
            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(true);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES")).Returns(false);

            NewRule(dateTimeQueryServiceMock.Object, dd07Mock.Object, learningDeliveryFAMQueryServiceMock.Object).
                ConditionMet(24, 35, learnStartDate, learnPlanEndDate, 1, dateOfBirth, learningDeliveryFAMs).Should().BeTrue();
        }

        [Theory]
        [InlineData(25, 35, "2018-08-01", "2019-01-01", 1, "2000-01-01", 18, "LDM", true, false)]
        [InlineData(null, 35, "2018-08-01", "2019-01-01", 1, "2000-01-01", 18, "LDM", false, false)]
        [InlineData(24, 99, "2018-08-01", "2019-01-01", 1, "2000-01-01", 18, "LDM", true, false)]
        [InlineData(24, 35, "2013-08-01", "2019-01-01", 1, "2000-01-01", 18, "LDM", true, false)]
        [InlineData(24, 35, "2018-08-01", "2019-01-01", 2, "2000-01-01", 18, "LDM", true, false)]
        [InlineData(24, 35, "2018-08-01", "2019-01-01", 1, "1990-01-01", 28, "LDM", true, false)]
        [InlineData(99, 35, "2018-08-01", "2019-01-01", 1, "2000-01-01", 18, "LDM", false, false)]
        [InlineData(24, 35, "2018-08-01", "2019-01-01", 1, "2000-01-01", 18, "RES", true, true)]
        public void ConditionMet_False(int? progType, int fundModel, string startDate, string planEndDate, int aimType, string dob, int mockYears, string famType, bool dd07Bool, bool famMock)
        {
            DateTime dateOfBirthMock = dob == null ? new DateTime(1900, 01, 01) : DateTime.Parse(dob);
            DateTime? dateOfBirth = dob == null ? (DateTime?)null : DateTime.Parse(dob);

            DateTime learnStartDate = DateTime.Parse(startDate);
            DateTime learnPlanEndDate = DateTime.Parse(planEndDate);

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = famType,
                    LearnDelFAMCode = "100"
                }
            };

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirthMock, learnStartDate)).Returns(mockYears);
            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(dd07Bool);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES")).Returns(famMock);

            NewRule(dateTimeQueryServiceMock.Object, dd07Mock.Object, learningDeliveryFAMQueryServiceMock.Object).
                ConditionMet(progType, fundModel, learnStartDate, learnPlanEndDate, aimType, dateOfBirth, learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learnStartDate = new DateTime(2014, 08, 01);
            var learnPlanEndDate = new DateTime(2015, 07, 28);
            var dateOfBirth = new DateTime(1995, 08, 02);
            var progType = 24;
            var fundModel = 35;

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "SOF",
                    LearnDelFAMCode = "105"
                }
            };

            var learner = new TestLearner
            {
                LearnRefNumber = "Learner1",
                DateOfBirthNullable = dateOfBirth,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        AimSeqNumber = 1,
                        AimType = 1,
                        FundModel = fundModel,
                        LearnStartDate = learnStartDate,
                        LearnPlanEndDate = learnPlanEndDate,
                        ProgTypeNullable = progType,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                }
            };

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            var days = (learnPlanEndDate - learnStartDate).TotalDays;
            double totalWholeDays = Math.Abs(days) + 1;
            dateTimeQueryServiceMock.Setup(x => x.WholeDaysBetween(learnStartDate, learnPlanEndDate)).Returns(totalWholeDays);

            // DOB
            var years = learnStartDate.Year - dateOfBirth.Year;
            var dob = dateOfBirth < learnStartDate.AddYears(years) ? years - 1 : years;
            dateTimeQueryServiceMock.Setup(x => x.YearsBetween(dateOfBirth, learnStartDate)).Returns(dob);

           // dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(18);
            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(true);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "SOF")).Returns(false); // Results TRUE

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    dateTimeQueryServiceMock.Object,
                    dd07Mock.Object,
                    learningDeliveryFAMQueryServiceMock.Object,
                    validationErrorHandlerMock.Object)
                    .Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learnStartDate = new DateTime(2014, 08, 01);
            var learnPlanEndDate = new DateTime(2015, 07, 31);
            var dateOfBirth = new DateTime(1995, 08, 02);
            var progType = 24;
            var fundModel = 35;

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = "SOF",
                    LearnDelFAMCode = "105"
                }
            };

            var learner = new TestLearner
            {
                LearnRefNumber = "Learner1",
                DateOfBirthNullable = dateOfBirth,
                LearningDeliveries = new List<TestLearningDelivery>
                {
                    new TestLearningDelivery
                    {
                        AimSeqNumber = 1,
                        AimType = 1,
                        FundModel = fundModel,
                        LearnStartDate = learnStartDate,
                        LearnPlanEndDate = learnPlanEndDate,
                        ProgTypeNullable = progType,
                        LearningDeliveryFAMs = learningDeliveryFAMs
                    }
                }
            };

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            var days = (learnPlanEndDate - learnStartDate).TotalDays;
            double totalWholeDays = Math.Abs(days) + 1;
            dateTimeQueryServiceMock.Setup(x => x.WholeDaysBetween(learnStartDate, learnPlanEndDate)).Returns(totalWholeDays);

            // DOB
            var years = learnStartDate.Year - dateOfBirth.Year;
            var dob = dateOfBirth < learnStartDate.AddYears(years) ? years - 1 : years;
            dateTimeQueryServiceMock.Setup(x => x.YearsBetween(dateOfBirth, learnStartDate)).Returns(dob);

            // dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(18);
            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(true);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "SOF")).Returns(false); // Results TRUE

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    dateTimeQueryServiceMock.Object,
                    dd07Mock.Object,
                    learningDeliveryFAMQueryServiceMock.Object,
                    validationErrorHandlerMock.Object)
                .Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("DateOfBirth", "01/01/2000")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("ProgType", 1)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2000, 01, 01), 1);

            validationErrorHandlerMock.Verify();
        }

        private DateOfBirth_35Rule NewRule(IDateTimeQueryService dateTimeQueryService = null, IDerivedData_07Rule dd07 = null, ILearningDeliveryFAMQueryService learningDeliveryFamQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new DateOfBirth_35Rule(dateTimeQueryService, dd07, learningDeliveryFamQueryService, validationErrorHandler);
        }
    }
}
