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
    public class DateOfBirth_40RuleTests : AbstractRuleTests<DateOfBirth_40Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("DateOfBirth_40");
        }

        [Theory]
        [InlineData(35)]
        [InlineData(81)]
        public void FundModelCondition_Pass(int fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeTrue();
        }

        [Fact]
        public void AimStartConditionMet_Pass_asLessThanDate()
        {
            var startDate = new DateTime(2016, 5, 31);
            NewRule().AimsStartDateConditionMet(startDate).Should().BeTrue();
        }

        [Fact]
        public void AimStartConditionMet_Pass_asEqualDate()
        {
            var startDate = new DateTime(2016, 7, 31);
            NewRule().AimsStartDateConditionMet(startDate).Should().BeTrue();
        }

        [Fact]
        public void AimStartConditionMet_Fails_asGreaterThanDate()
        {
            var startDate = new DateTime(2016, 9, 18);
            NewRule().AimsStartDateConditionMet(startDate).Should().BeFalse();
        }

        [Theory]
        [InlineData(31)]
        [InlineData(19)]
        public void FundModelCondition_Fails(int fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeFalse();
        }

        [Fact]
        public void AimTypeCondition_Pass()
        {
            NewRule().AimTypeConditionMet(1).Should().BeTrue();
        }

        [Fact]
        public void AimTypeCondition_Fails()
        {
            NewRule().AimTypeConditionMet(2).Should().BeFalse();
        }

        [Fact]
        public void ProgtypeCondition_Pass()
        {
            NewRule().ProgTypeConditionMet(25).Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(19)]
        public void ProgtypeCondition_Fails(int? progType)
        {
            NewRule().ProgTypeConditionMet(progType).Should().BeFalse();
        }

        [Fact]
        public void OutcomeCondition_Pass()
        {
            NewRule().OutcomeConditionMet(1).Should().BeTrue();
        }

        [Fact]
        public void OutcomeCondition_Fails()
        {
            NewRule().OutcomeConditionMet(2).Should().BeFalse();
        }

        [Fact]
        public void AgeConditionMet_Pass()
        {
            var dateOfBirth = new DateTime(1996, 5, 30);
            var startDate = new DateTime(2016, 07, 31);

            var mockDateTimeService = new Mock<IDateTimeQueryService>();

            // DOB
            var years = startDate.Year - dateOfBirth.Year;
            var dob = dateOfBirth < startDate.AddYears(years) ? years - 1 : years;
            mockDateTimeService.Setup(x => x.YearsBetween(dateOfBirth, startDate)).Returns(dob);

            var rule = NewRule(dateTimeQueryService: mockDateTimeService.Object);

            var result = rule.AgeConditionMet(dateOfBirth, startDate);
            result.Should().BeTrue();

            mockDateTimeService.Verify(x => x.YearsBetween(dateOfBirth, startDate), Times.Exactly(1));
        }

        [Fact]
        public void AgeConditionMet_Fails()
        {
            var dateOfBirth = new DateTime(1997, 5, 30);
            var startDate = new DateTime(2016, 07, 31);

            var mockDateTimeService = new Mock<IDateTimeQueryService>();

            // DOB
            var years = startDate.Year - dateOfBirth.Year;
            var dob = dateOfBirth < startDate.AddYears(years) ? years - 1 : years;
            mockDateTimeService.Setup(x => x.YearsBetween(dateOfBirth, startDate)).Returns(dob);

            var rule = NewRule(dateTimeQueryService: mockDateTimeService.Object);

            var result = rule.AgeConditionMet(dateOfBirth, startDate);
            result.Should().BeFalse();

            mockDateTimeService.Verify(x => x.YearsBetween(dateOfBirth, startDate), Times.Exactly(1));
        }

        [Theory]
        [InlineData("2016-07-31", "2017-07-29", true)] // triggers due to 364 Days
        [InlineData("2016-07-31", "2017-07-30", false)] // doesn't trigger due to 365 days
        [InlineData("2016-07-31", "2017-07-31", false)] // doesn't trigger due to 366 days
        public void DurationConditionMet_ExpectedResult(string learnStartDate, string learnActEndDate, bool expectedResult)
        {
            DateTime startDate = DateTime.Parse(learnStartDate);
            DateTime actEndDateNullable = DateTime.Parse(learnActEndDate);

            var mockDateTimeService = new Mock<IDateTimeQueryService>();

            var days = (actEndDateNullable - startDate).TotalDays;
            double totalWholeDays = Math.Abs(days) + 1;
            mockDateTimeService.Setup(x => x.WholeDaysBetween(startDate, actEndDateNullable)).Returns(totalWholeDays);

            var rule = NewRule(dateTimeQueryService: mockDateTimeService.Object);

            var result = rule.DurationConditionMet(startDate, actEndDateNullable);
            result.Should().Be(expectedResult);

            mockDateTimeService.Verify(x => x.WholeDaysBetween(startDate, actEndDateNullable), Times.Exactly(1));
        }

        [Fact]
        public void DurationConditionMet_Fails_DuetoNull()
        {
            DateTime startDate = new DateTime(2016, 07, 31);
            DateTime? actEndDateNullable = null;

            NewRule().DurationConditionMet(startDate, actEndDateNullable).Should().BeFalse();
        }

        [Fact]
        public void RestartConditionMet_Pass()
        {
            IEnumerable<ILearningDeliveryFAM> deliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = "ADL" }
            };

            var mockFamQuerySrvc = new Mock<ILearningDeliveryFAMQueryService>();
            mockFamQuerySrvc.Setup(x => x.HasLearningDeliveryFAMType(deliveryFAMs, "RES")).Returns(false); // results True

            var result = NewRule(learningDeliveryFAMQueryService: mockFamQuerySrvc.Object).RestartConditionMet(deliveryFAMs);

            result.Should().BeTrue();
            mockFamQuerySrvc.Verify(x => x.HasLearningDeliveryFAMType(deliveryFAMs, "RES"), Times.Exactly(1));
        }

        [Fact]
        public void RestartConditionMet_Fails()
        {
            IEnumerable<ILearningDeliveryFAM> deliveryFAMs = new List<TestLearningDeliveryFAM>()
            {
                new TestLearningDeliveryFAM() { LearnDelFAMType = "RES" }
            };

            var mockFamQuerySrvc = new Mock<ILearningDeliveryFAMQueryService>();
            mockFamQuerySrvc.Setup(x => x.HasLearningDeliveryFAMType(deliveryFAMs, "RES")).Returns(true); // results False

            var result = NewRule(learningDeliveryFAMQueryService: mockFamQuerySrvc.Object).RestartConditionMet(deliveryFAMs);

            result.Should().BeFalse();
            mockFamQuerySrvc.Verify(x => x.HasLearningDeliveryFAMType(deliveryFAMs, "RES"), Times.Exactly(1));
        }

        [Fact]
        public void ConditionMet_Pass()
        {
            int fundModel = 36;
            var progType = 25;
            var aimType = 1;
            var outcome = 1;

            var dateOfBirth = new DateTime(1996, 5, 30);
            var startDate = new DateTime(2016, 07, 31);
            var actEndDate = new DateTime(2017, 07, 29);

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.RES,
                    LearnDelFAMCode = "105"
                }
            };

            var rule = NewRuleMock();
            rule.Setup(x => x.AimsStartDateConditionMet(startDate)).Returns(true);
            rule.Setup(x => x.FundModelConditionMet(fundModel)).Returns(true);
            rule.Setup(x => x.AimTypeConditionMet(aimType)).Returns(true);
            rule.Setup(x => x.ProgTypeConditionMet(progType)).Returns(true);
            rule.Setup(x => x.OutcomeConditionMet(outcome)).Returns(true);
            rule.Setup(x => x.AgeConditionMet(dateOfBirth, startDate)).Returns(true);
            rule.Setup(x => x.DurationConditionMet(startDate, actEndDate)).Returns(true);
            rule.Setup(x => x.RestartConditionMet(learningDeliveryFAMs)).Returns(true);

            var result = rule.Object.ConditionMet(fundModel, aimType, progType, outcome, startDate, dateOfBirth, actEndDate, learningDeliveryFAMs);

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("2016-07-31", "2017-07-30")] // doesn't trigger due to 365 days
        [InlineData("2016-07-31", "2017-07-31")] // doesn't trigger due to 366 days
        public void Validate_NoError(string learnStartDate, string learnActEndDate)
        {
            int fundModel = 81;
            var progType = 25;
            var aimType = 1;
            var outcome = 1;

            var dateOfBirth = new DateTime(1996, 5, 30);

            DateTime startDate = DateTime.Parse(learnStartDate);
            DateTime actEndDate = DateTime.Parse(learnActEndDate);

            var deliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.RES,
                    LearnDelFAMCode = "105"
                }
            };

            var learner = new TestLearner
            {
                DateOfBirthNullable = dateOfBirth,
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = fundModel,
                        AimType = aimType,
                        ProgTypeNullable = progType,
                        OutcomeNullable = outcome,
                        CompStatus = 1,
                        LearnActEndDateNullable = actEndDate,
                        LearnStartDate = startDate,
                        LearningDeliveryFAMs = deliveryFAMs
                    }
                }
            };

            var mockDateTimeService = new Mock<IDateTimeQueryService>();

            var days = (actEndDate - startDate).TotalDays;
            double totalWholeDays = Math.Abs(days) + 1;
            mockDateTimeService.Setup(x => x.WholeDaysBetween(startDate, actEndDate)).Returns(totalWholeDays);

            // DOB
            var years = startDate.Year - dateOfBirth.Year;
            var dob = dateOfBirth < startDate.AddYears(years) ? years - 1 : years;
            mockDateTimeService.Setup(x => x.YearsBetween(dateOfBirth, startDate)).Returns(dob);

            var mockFamQuerySrvc = new Mock<ILearningDeliveryFAMQueryService>();
            mockFamQuerySrvc.Setup(x => x.HasLearningDeliveryFAMType(deliveryFAMs, "RES")).Returns(false); // results True

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                        validationErrorHandlerMock.Object,
                        mockFamQuerySrvc.Object,
                        mockDateTimeService.Object)
                .Validate(learner);
            }
        }

        [Theory]
        [InlineData("2016-07-31", "2017-07-29")] // triggers due to 364 Days
        public void Validate_Error(string learnStartDate, string learnActEndDate)
        {
            int fundModel = 81;
            var progType = 25;
            var aimType = 1;
            var outcome = 1;

            var dateOfBirth = new DateTime(1996, 5, 30);

            DateTime startDate = DateTime.Parse(learnStartDate);
            DateTime actEndDate = DateTime.Parse(learnActEndDate);

            var deliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMType = LearningDeliveryFAMTypeConstants.ALB,
                    LearnDelFAMCode = "105"
                }
            };

            var learner = new TestLearner
            {
                DateOfBirthNullable = dateOfBirth,
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        FundModel = fundModel,
                        AimType = aimType,
                        ProgTypeNullable = progType,
                        OutcomeNullable = outcome,
                        CompStatus = 1,
                        LearnActEndDateNullable = actEndDate,
                        LearnStartDate = startDate,
                        LearningDeliveryFAMs = deliveryFAMs
                    }
                }
            };

            var mockDateTimeService = new Mock<IDateTimeQueryService>();

            var days = (actEndDate - startDate).TotalDays;
            double totalWholeDays = Math.Abs(days) + 1;
            mockDateTimeService.Setup(x => x.WholeDaysBetween(startDate, actEndDate)).Returns(totalWholeDays);

            // DOB
            var years = startDate.Year - dateOfBirth.Year;
            var dob = dateOfBirth < startDate.AddYears(years) ? years - 1 : years;
            mockDateTimeService.Setup(x => x.YearsBetween(dateOfBirth, startDate)).Returns(dob);

            var mockFamQuerySrvc = new Mock<ILearningDeliveryFAMQueryService>();
            mockFamQuerySrvc.Setup(x => x.HasLearningDeliveryFAMType(deliveryFAMs, "ALB")).Returns(false); // results True

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                        validationErrorHandlerMock.Object,
                        mockFamQuerySrvc.Object,
                        mockDateTimeService.Object)
                .Validate(learner);
            }
        }

        private DateOfBirth_40Rule NewRule(
            IValidationErrorHandler validationErrorHandler = null,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null,
            IDateTimeQueryService dateTimeQueryService = null)
        {
            return new DateOfBirth_40Rule(
                dateTimeQueryService: dateTimeQueryService,
                learningDeliveryFAMQueryService: learningDeliveryFAMQueryService,
                validationErrorHandler: validationErrorHandler);
        }
    }
}
