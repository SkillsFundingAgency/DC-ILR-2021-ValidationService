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
    public class DateOfBirth_47RuleTests : AbstractRuleTests<DateOfBirth_47Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("DateOfBirth_47");
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
        public void CompStatusConditionMet_True()
        {
            NewRule().CompStatusConditionMet(2).Should().BeTrue();
        }

        [Fact]
        public void CompStatusConditionMet_False()
        {
            NewRule().CompStatusConditionMet(1).Should().BeFalse();
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
        public void DateOfBirthConditionMet_False_Null()
        {
            var dateOfBirth = new DateTime(2010, 01, 01);
            var learnStartDate = new DateTime(2018, 08, 01);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(8);

            NewRule(dateTimeQueryServiceMock.Object).DateOfBirthConditionMet(null, learnStartDate).Should().BeFalse();
        }

        [Fact]
        public void LearnActEndDateConditionMet_True()
        {
            var learnStartDate = new DateTime(2018, 08, 01);
            var learnActEndDate = new DateTime(2019, 08, 01);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.WholeDaysBetween(learnStartDate, learnActEndDate)).Returns(366);

            NewRule(dateTimeQueryServiceMock.Object).LearnActEndDateConditionMet(learnStartDate, learnActEndDate).Should().BeTrue();
        }

        [Theory]
        [InlineData("2018-08-01", "2019-08-31", 396)]
        [InlineData("2017-07-28", "2018-08-03", 372)]
        public void LearnActEndDateConditionMet_False(string startDate, string endDate, int days)
        {
            var learnStartDate = DateTime.Parse(startDate);
            var learnActEndDate = DateTime.Parse(endDate);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.WholeDaysBetween(learnStartDate, learnActEndDate)).Returns(days);

            NewRule(dateTimeQueryServiceMock.Object).LearnActEndDateConditionMet(learnStartDate, learnActEndDate).Should().BeFalse();
        }

        [Fact]
        public void LearnActEndDateConditionMet_False_Null()
        {
            var learnStartDate = new DateTime(2018, 08, 01);
            var learnActEndDate = new DateTime(2019, 08, 01);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.WholeDaysBetween(learnStartDate, learnActEndDate)).Returns(366);

            NewRule(dateTimeQueryServiceMock.Object).LearnActEndDateConditionMet(learnStartDate, null).Should().BeFalse();
        }

        [Fact]
        public void LearningDeliveryFAMConditionMet_True()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = "034",
                    LearnDelFAMType = "SOF"
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES")).Returns(false);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMConditionMet(learningDeliveryFAMs).Should().BeTrue();
        }

        [Fact]
        public void LearningDeliveryFAMConditionMet_False()
        {
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = "100",
                    LearnDelFAMType = "RES"
                }
            };

            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES")).Returns(true);

            NewRule(learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object).LearningDeliveryFAMConditionMet(learningDeliveryFAMs).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var dateOfBirth = new DateTime(2000, 01, 01);
            var learnStartDate = new DateTime(2018, 08, 01);
            var learnActEndDate = new DateTime(2019, 08, 01);
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
                CompStatus = 2,
                LearnStartDate = learnStartDate,
                LearnActEndDateNullable = learnActEndDate,
                LearningDeliveryFAMs = learningDeliveryFAMs
            };

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(18);
            dateTimeQueryServiceMock.Setup(qs => qs.WholeDaysBetween(learnStartDate, learnActEndDate)).Returns(366);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES")).Returns(false);

            NewRule(dateTimeQueryServiceMock.Object, learningDeliveryFAMQueryServiceMock.Object)
                .ConditionMet(
                        learningDelivery.ProgTypeNullable,
                        learningDelivery.FundModel,
                        learningDelivery.CompStatus,
                        learningDelivery.LearnStartDate,
                        learningDelivery.LearnActEndDateNullable,
                        learningDelivery.AimType,
                        dateOfBirth,
                        learningDelivery.LearningDeliveryFAMs)
                .Should().BeTrue();
        }

        [Theory]
        [InlineData(99, "2018-08-01", 1, 25, 2, 18, 366, false)]
        [InlineData(36, "2015-08-01", 1, 25, 2, 18, 366, false)]
        [InlineData(36, "2018-08-01", 2, 25, 2, 18, 366, false)]
        [InlineData(36, "2018-08-01", 1, 20, 2, 18, 366, false)]
        [InlineData(36, "2018-08-01", 1, 25, 1, 18, 366, false)]
        [InlineData(36, "2018-08-01", 1, 25, 2, 15, 366, false)]
        [InlineData(36, "2018-08-01", 1, 25, 2, 15, 381, false)]
        [InlineData(36, "2018-08-01", 1, 25, 2, 15, 366, true)]
        public void ConditionMet_False(int fundModel, string learnStartDateString, int aimType, int? progType, int compStatus, int age, int days, bool famMock)
        {
            var dateOfBirth = new DateTime(2000, 01, 01);
            var learnStartDate = DateTime.Parse(learnStartDateString);
            var learnActEndDate = new DateTime(2019, 08, 01);
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
                CompStatus = compStatus,
                LearnStartDate = learnStartDate,
                LearnActEndDateNullable = learnActEndDate,
                LearningDeliveryFAMs = learningDeliveryFAMs
            };

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(age);
            dateTimeQueryServiceMock.Setup(qs => qs.WholeDaysBetween(learnStartDate, learnActEndDate)).Returns(days);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES")).Returns(famMock);

            NewRule(dateTimeQueryServiceMock.Object, learningDeliveryFAMQueryServiceMock.Object)
                .ConditionMet(
                        learningDelivery.ProgTypeNullable,
                        learningDelivery.FundModel,
                        learningDelivery.CompStatus,
                        learningDelivery.LearnStartDate,
                        learningDelivery.LearnActEndDateNullable,
                        learningDelivery.AimType,
                        dateOfBirth,
                        learningDelivery.LearningDeliveryFAMs)
                .Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var dateOfBirth = new DateTime(2000, 01, 01);
            var learnStartDate = new DateTime(2018, 08, 01);
            var learnActEndDate = new DateTime(2019, 08, 01);
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
                    CompStatus = 2,
                    LearnStartDate = learnStartDate,
                    LearnActEndDateNullable = learnActEndDate,
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

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(18);
            dateTimeQueryServiceMock.Setup(qs => qs.WholeDaysBetween(learnStartDate, learnActEndDate)).Returns(366);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES")).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(dateTimeQueryServiceMock.Object, learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var dateOfBirth = new DateTime(2000, 01, 01);
            var learnStartDate = new DateTime(2018, 08, 01);
            var learnActEndDate = new DateTime(2019, 08, 01);
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
                    CompStatus = 2,
                    LearnStartDate = learnStartDate,
                    LearnActEndDateNullable = learnActEndDate,
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

            dateTimeQueryServiceMock.Setup(qs => qs.YearsBetween(dateOfBirth, learnStartDate)).Returns(15);
            dateTimeQueryServiceMock.Setup(qs => qs.WholeDaysBetween(learnStartDate, learnActEndDate)).Returns(366);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES")).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(dateTimeQueryServiceMock.Object, learningDeliveryFAMQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
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

        private DateOfBirth_47Rule NewRule(IDateTimeQueryService dateTimeQueryService = null, ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new DateOfBirth_47Rule(dateTimeQueryService, learningDeliveryFAMQueryService, validationErrorHandler);
        }
    }
}
