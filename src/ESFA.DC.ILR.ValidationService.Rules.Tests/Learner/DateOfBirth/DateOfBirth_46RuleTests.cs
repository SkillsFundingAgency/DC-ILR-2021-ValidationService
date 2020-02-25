using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Learner.DateOfBirth;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.Learner.DateOfBirth
{
    public class DateOfBirth_46RuleTests : AbstractRuleTests<DateOfBirth_46Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("DateOfBirth_46");
        }

        [Theory]
        [InlineData(36)]
        [InlineData(81)]
        public void FundModelConditionMet_True(int fundModel)
        {
            NewRule().FundModelConditionMet(fundModel).Should().BeTrue();
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
        public void ProgTypeConditionMet_True()
        {
            NewRule().ProgTypeConditionMet(25).Should().BeTrue();
        }

        [Fact]
        public void ProgTypeConditionMet_False()
        {
            NewRule().ProgTypeConditionMet(2).Should().BeFalse();
        }

        [Fact]
        public void ProgTypeConditionMet_False_Null()
        {
            NewRule().ProgTypeConditionMet(null).Should().BeFalse();
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
            var dateOfBirth = new DateTime(2010, 01, 01);
            var learnStartDate = new DateTime(2018, 08, 01);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(8);

            NewRule(dateTimeQueryServiceMock.Object).DateOfBirthConditionMet(dateOfBirth, learnStartDate).Should().BeFalse();
        }

        [Fact]
        public void DateOfBirthConditionMet_False_DobIsNull()
        {
            DateTime? dateOfBirth = null;
            var learnStartDate = new DateTime(2018, 08, 01);
            var result = NewRule().DateOfBirthConditionMet(dateOfBirth, learnStartDate);

            result.Should().BeFalse();
        }

        [Fact]
        public void LearnPlanEndDateConditionMet_False_AsDaysEqualsTo365()
        {
            var learnStartDate = new DateTime(2014, 08, 01);
            var learnPlanEndDate = new DateTime(2015, 07, 31);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            var totalWholeDays = GetWholeDays(learnStartDate, learnPlanEndDate);
            dateTimeQueryServiceMock.Setup(x => x.WholeDaysBetween(learnStartDate, learnPlanEndDate)).Returns(totalWholeDays);

            var rule46 = NewRule(dateTimeQueryServiceMock.Object).LearnPlanEndDateConditionMet(learnStartDate, learnPlanEndDate);

            rule46.Should().BeFalse();
            dateTimeQueryServiceMock.Verify(x => x.WholeDaysBetween(learnStartDate, learnPlanEndDate), Times.Exactly(1));
        }

        [Fact]
        public void LearnPlanEndDateConditionMet_True_DaysAreLessThan365()
        {
            var learnStartDate = new DateTime(2014, 08, 01);
            var learnPlanEndDate = new DateTime(2015, 07, 29);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            var totalWholeDays = GetWholeDays(learnStartDate, learnPlanEndDate);
            dateTimeQueryServiceMock.Setup(qs => qs.WholeDaysBetween(learnStartDate, learnPlanEndDate)).Returns(totalWholeDays);

            var rule46 = NewRule(dateTimeQueryServiceMock.Object).LearnPlanEndDateConditionMet(learnStartDate, learnPlanEndDate);

            rule46.Should().BeTrue();
            dateTimeQueryServiceMock.Verify(x => x.WholeDaysBetween(learnStartDate, learnPlanEndDate), Times.Exactly(1));
        }

        [Theory]
        [InlineData("2014-08-01", "2015-08-01")] // doesn't trigger due to 366 days
        [InlineData("2014-08-01", "2015-08-05")] // doesn't trigger due to 70 days
        public void LearnPlanEndDateConditionMet_False(string startDate, string planEndDate)
        {
            DateTime learnStartDate = DateTime.Parse(startDate);
            DateTime learnPlanEndDate = DateTime.Parse(planEndDate);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            var days = (learnPlanEndDate - learnStartDate).TotalDays;
            double totalWholeDays = Math.Abs(days) + 1;
            dateTimeQueryServiceMock.Setup(qs => qs.WholeDaysBetween(learnStartDate, learnPlanEndDate)).Returns(totalWholeDays);

            var rule46 = NewRule(dateTimeQueryServiceMock.Object).LearnPlanEndDateConditionMet(learnStartDate, learnPlanEndDate);

            rule46.Should().BeFalse();
            dateTimeQueryServiceMock.Verify(qs => qs.WholeDaysBetween(learnStartDate, learnPlanEndDate), Times.Exactly(1));
        }

        [Fact]
        public void LearningDeliveryFAMConditionMet_True()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>();

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "SOF")).Returns(false);

            var result = NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMConditionMet(learningDeliveryFAMs);
            result.Should().BeTrue();
            learningDeliveryFAMQueryServiceMock.Verify(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES"), Times.AtLeastOnce);
        }

        [Fact]
        public void LearningDeliveryFAMConditionMet_False()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>();

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES")).Returns(true);

            var result = NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMConditionMet(learningDeliveryFAMs);

            result.Should().BeFalse();
            learningDeliveryFAMQueryServiceMock.Verify(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES"), Times.AtLeastOnce);
        }

        [Fact]
        public void ConditionMet_True()
        {
            var dateOfBirth = new DateTime(2000, 01, 01);
            var learnStartDate = new DateTime(2018, 08, 01);
            var learnPlanEndDate = new DateTime(2019, 08, 01);
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = "034",
                    LearnDelFAMType = "LDM"
                }
            };

            var learningDelivery = new TestLearningDelivery
            {
                FundModel = 36,
                ProgTypeNullable = 25,
                AimType = 1,
                LearnStartDate = learnStartDate,
                LearnPlanEndDate = learnPlanEndDate,
                LearningDeliveryFAMs = learningDeliveryFAMs
            };

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(18);
            dateTimeQueryServiceMock.Setup(qs => qs.WholeDaysBetween(learnStartDate, learnPlanEndDate)).Returns(360);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES")).Returns(false);

            NewRule(dateTimeQueryServiceMock.Object, learningDeliveryFAMQueryServiceMock.Object)
                .ConditionMet(
                        learningDelivery.ProgTypeNullable,
                        learningDelivery.FundModel,
                        learningDelivery.LearnStartDate,
                        learningDelivery.LearnPlanEndDate,
                        learningDelivery.AimType,
                        dateOfBirth,
                        learningDelivery.LearningDeliveryFAMs)
                .Should().BeTrue();
        }

        [Theory]
        [InlineData(99, "2018-08-01", 1, 25, 18, 365, false)]
        [InlineData(36, "2015-08-01", 1, 25, 18, 365, false)]
        [InlineData(36, "2018-08-01", 2, 25, 18, 365, false)]
        [InlineData(36, "2018-08-01", 1, 20, 18, 365, false)]
        [InlineData(36, "2018-08-01", 1, 25, 15, 365, false)]
        [InlineData(36, "2018-08-01", 1, 25, 15, 380, false)]
        [InlineData(36, "2018-08-01", 1, 25, 15, 365, true)]
        public void ConditionMet_False(int fundModel, string learnStartDateString, int aimType, int? progType, int age, int days, bool famMock)
        {
            var dateOfBirth = new DateTime(2000, 01, 01);
            var learnStartDate = DateTime.Parse(learnStartDateString);
            var learnPlanEndDate = new DateTime(2019, 08, 01);
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = "034",
                    LearnDelFAMType = "LDM"
                }
            };

            var learningDelivery = new TestLearningDelivery
            {
                FundModel = fundModel,
                ProgTypeNullable = progType,
                AimType = aimType,
                LearnStartDate = learnStartDate,
                LearnPlanEndDate = learnPlanEndDate,
                LearningDeliveryFAMs = learningDeliveryFAMs
            };

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(age);
            dateTimeQueryServiceMock.Setup(qs => qs.WholeDaysBetween(learnStartDate, learnPlanEndDate)).Returns(days);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES")).Returns(famMock);

            NewRule(dateTimeQueryServiceMock.Object, learningDeliveryFAMQueryServiceMock.Object)
                .ConditionMet(
                        learningDelivery.ProgTypeNullable,
                        learningDelivery.FundModel,
                        learningDelivery.LearnStartDate,
                        learningDelivery.LearnPlanEndDate,
                        learningDelivery.AimType,
                        dateOfBirth,
                        learningDelivery.LearningDeliveryFAMs)
                .Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var dateOfBirth = new DateTime(1997, 09, 30);
            var learnStartDate = new DateTime(2016, 08, 01);
            var learnPlanEndDate = new DateTime(2017, 07, 30);

            List<TestLearningDeliveryFAM> deliveryFAMs = LearningDeliveryFAM_Without_RES();

            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    LearnAimRef = "ZPROG001",
                    FundModel = 81,
                    ProgTypeNullable = 25,
                    AimType = 1,
                    LearnStartDate = learnStartDate,
                    LearnPlanEndDate = learnPlanEndDate,
                    LearningDeliveryFAMs = deliveryFAMs
                }
            };

            var learner = new TestLearner
            {
                DateOfBirthNullable = dateOfBirth,
                LearningDeliveries = learningDeliveries
            };

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            // DOB
            var dob = GetDOB(dateOfBirth, learnStartDate);
            dateTimeQueryServiceMock.Setup(x => x.YearsBetween(dateOfBirth, learnStartDate)).Returns(dob);

            // Days
            var totalWholeDays = GetWholeDays(learnStartDate, learnPlanEndDate);
            dateTimeQueryServiceMock.Setup(qs => qs.WholeDaysBetween(learnStartDate, learnPlanEndDate)).Returns(totalWholeDays);

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(deliveryFAMs, "RES")).Returns(false); // Results TRUE

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    dateTimeQueryServiceMock.Object,
                    learningDeliveryFAMQueryServiceMock.Object,
                    validationErrorHandlerMock.Object)
                .Validate(learner);
            }
        }

        [Theory]
        [InlineData("2016-07-31", "2017-08-01")] // doesn't trigger due to 367 days
        [InlineData("2016-07-31", "2017-07-31")] // doesn't trigger due to 366 days
        public void Validate_NoError(string learnStartDate, string learnPlanEndDate)
        {
            var dateOfBirth = new DateTime(1996, 5, 30);
            DateTime startDate = DateTime.Parse(learnStartDate);
            DateTime planEndDate = DateTime.Parse(learnPlanEndDate);

            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = "034",
                    LearnDelFAMType = "LDM"
                }
            };

            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    FundModel = 36,
                    ProgTypeNullable = 25,
                    AimType = 1,
                    LearnStartDate = startDate,
                    LearnPlanEndDate = planEndDate,
                    LearningDeliveryFAMs = learningDeliveryFAMs
                }
            };

            var learner = new TestLearner
            {
                DateOfBirthNullable = dateOfBirth,
                LearningDeliveries = learningDeliveries
            };

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, startDate)).Returns(16);

            var totalWholeDays = GetWholeDays(startDate, planEndDate);
            dateTimeQueryServiceMock.Setup(qs => qs.WholeDaysBetween(startDate, planEndDate)).Returns(totalWholeDays);

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES")).Returns(false); // results TRUE

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    dateTimeQueryServiceMock.Object,
                    learningDeliveryFAMQueryServiceMock.Object,
                    validationErrorHandlerMock.Object)
                .Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError_DueToRESTART()
        {
            var dateOfBirth = new DateTime(1997, 09, 30);
            var learnStartDate = new DateTime(2016, 08, 01);
            var learnPlanEndDate = new DateTime(2017, 07, 30);

            List<TestLearningDeliveryFAM> deliveryFAMs = LearningDelFAMWith_RES();

            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    LearnAimRef = "50098184",
                    FundModel = 81,
                    ProgTypeNullable = 25,
                    AimType = 1,
                    LearnStartDate = learnStartDate,
                    LearnPlanEndDate = learnPlanEndDate,
                    LearningDeliveryFAMs = deliveryFAMs
                }
            };

            var learner = new TestLearner
            {
                DateOfBirthNullable = dateOfBirth,
                LearningDeliveries = learningDeliveries
            };

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            // DOB
            var dob = GetDOB(dateOfBirth, learnStartDate);
            dateTimeQueryServiceMock.Setup(x => x.YearsBetween(dateOfBirth, learnStartDate)).Returns(dob);

            // Days
            var totalWholeDays = GetWholeDays(learnStartDate, learnPlanEndDate);
            dateTimeQueryServiceMock.Setup(qs => qs.WholeDaysBetween(learnStartDate, learnPlanEndDate)).Returns(totalWholeDays);

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(deliveryFAMs, "RES")).Returns(true); // Results FALSE

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    dateTimeQueryServiceMock.Object,
                    learningDeliveryFAMQueryServiceMock.Object,
                    validationErrorHandlerMock.Object)
                .Validate(learner);
            }
        }

        [Theory]
        [InlineData(81)]
        [InlineData(36)]
        public void Validate_NoError_AsDaysAre365(int fundModel)
        {
            var dateOfBirth = new DateTime(1997, 09, 30);
            var learnStartDate = new DateTime(2016, 08, 01);
            var learnPlanEndDate = new DateTime(2017, 07, 31);

            List<TestLearningDeliveryFAM> deliveryFAMs = LearningDeliveryFAM_Without_RES();

            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    LearnAimRef = "ZPROG001",
                    FundModel = fundModel,
                    ProgTypeNullable = 25,
                    AimType = 1,
                    LearnStartDate = learnStartDate,
                    LearnPlanEndDate = learnPlanEndDate,
                    LearningDeliveryFAMs = deliveryFAMs
                }
            };

            var learner = new TestLearner
            {
                DateOfBirthNullable = dateOfBirth,
                LearningDeliveries = learningDeliveries
            };

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            // DOB
            var dob = GetDOB(dateOfBirth, learnStartDate);
            dateTimeQueryServiceMock.Setup(x => x.YearsBetween(dateOfBirth, learnStartDate)).Returns(dob);

            // Days
            var totalWholeDays = GetWholeDays(learnStartDate, learnPlanEndDate);
            dateTimeQueryServiceMock.Setup(qs => qs.WholeDaysBetween(learnStartDate, learnPlanEndDate)).Returns(totalWholeDays);

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(deliveryFAMs, "RES")).Returns(false); // Results TRUE

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    dateTimeQueryServiceMock.Object,
                    learningDeliveryFAMQueryServiceMock.Object,
                    validationErrorHandlerMock.Object)
                .Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnPlanEndDate", "01/01/2008")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnStartDate", "01/01/2007")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2008, 01, 01), new DateTime(2007, 01, 01));

            validationErrorHandlerMock.Verify();
        }

        private DateOfBirth_46Rule NewRule(IDateTimeQueryService dateTimeQueryService = null, ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new DateOfBirth_46Rule(dateTimeQueryService, learningDeliveryFAMQueryService, validationErrorHandler);
        }

        private List<TestLearningDeliveryFAM> LearningDeliveryFAM_Without_RES()
        {
            return new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM { LearnDelFAMCode = "1", LearnDelFAMType = "FFI" },
                new TestLearningDeliveryFAM { LearnDelFAMCode = "105", LearnDelFAMType = "SOF" },
                new TestLearningDeliveryFAM { LearnDelFAMCode = "1", LearnDelFAMType = "HHS" }
            };
        }

        private List<TestLearningDeliveryFAM> LearningDelFAMWith_RES()
        {
            return new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM { LearnDelFAMCode = "1", LearnDelFAMType = "RES" },
                new TestLearningDeliveryFAM { LearnDelFAMCode = "105", LearnDelFAMType = "SOF" },
                new TestLearningDeliveryFAM { LearnDelFAMCode = "1", LearnDelFAMType = "HHS" },
                new TestLearningDeliveryFAM { LearnDelFAMCode = "1", LearnDelFAMType = "FFI" }
            };
        }

        private int GetDOB(DateTime dateOfBirth, DateTime learnStartDate)
        {
            var year = learnStartDate.Year - dateOfBirth.Year;
            var dob = learnStartDate < dateOfBirth.AddYears(year) ? year - 1 : year;
            return dob;
        }

        private double GetWholeDays(DateTime learnStartDate, DateTime learnPlanEndDate)
        {
            var days = (learnPlanEndDate - learnStartDate).TotalDays;
            double totalWholeDays = Math.Abs(days) + 1;
            return totalWholeDays;
        }
    }
}
