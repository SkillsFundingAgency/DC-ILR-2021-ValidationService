using System;
using System.Collections.Generic;
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
    public class DateOfBirth_53RuleTests : AbstractRuleTests<DateOfBirth_53Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("DateOfBirth_53");
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            NewRule().FundModelConditionMet(TypeOfFunding.ApprenticeshipsFrom1May2017).Should().BeTrue();
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(99).Should().BeFalse();
        }

        [Fact]
        public void LearnStartDateConditionMet_True()
        {
            NewRule().LearnStartDateConditionMet(new DateTime(2017, 05, 01)).Should().BeTrue();
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
        public void DD07ConditionMet_True()
        {
            var progType = 23;

            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(true);

            NewRule(dd07: dd07Mock.Object).DD07ConditionMet(progType).Should().BeTrue();
        }

        [Fact]
        public void DD07ConditionMet_False()
        {
            var progType = 25;

            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);

            NewRule(dd07: dd07Mock.Object).DD07ConditionMet(progType).Should().BeFalse();
        }

        [Fact]
        public void DD07ConditionMet_Null()
        {
            var dd07Mock = new Mock<IDerivedData_07Rule>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(null)).Returns(false);

            NewRule(dd07: dd07Mock.Object).DD07ConditionMet(null).Should().BeFalse();
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

        [Theory]
        [InlineData("2017-05-01", null, double.MaxValue)]
        [InlineData("2017-05-01", "2019-05-01", 730)]
        [InlineData("2017-09-04", "2018-09-03", 365)]
        public void LearnActEndDateConditionMet_False(string learnStartDate, string learnActEndDate, double totalDays)
        {
            DateTime learnStartDateParam = DateTime.Parse(learnStartDate);
            DateTime? learnActEndDateParam = string.IsNullOrEmpty(learnActEndDate) ? (DateTime?)null : DateTime.Parse(learnActEndDate);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.WholeDaysBetween(learnStartDateParam, learnActEndDateParam ?? DateTime.MaxValue)).Returns(totalDays);

            NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object).LearnActEndDateConditionMet(learnStartDateParam, learnActEndDateParam).Should().BeFalse();
        }

        [Theory]
        [InlineData("2017-05-01", "2018-04-30", 364)]
        [InlineData("2017-09-01", "2017-09-01", 1)]
        public void LearnActEndDateConditionMet_True(string learnStartDate, string learnActEndDate, double totalDays)
        {
            DateTime learnStartDateParam = DateTime.Parse(learnStartDate);
            DateTime learnActEndDateParam = DateTime.Parse(learnActEndDate);

            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();

            dateTimeQueryServiceMock.Setup(qs => qs.WholeDaysBetween(learnStartDateParam, learnActEndDateParam)).Returns(totalDays);

            NewRule(dateTimeQueryService: dateTimeQueryServiceMock.Object).LearnActEndDateConditionMet(learnStartDateParam, learnActEndDateParam).Should().BeTrue();
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
            var learnStartDate = new DateTime(2017, 05, 01);
            var learnActEndDate = new DateTime(2017, 06, 01);
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
                FundModel = TypeOfFunding.ApprenticeshipsFrom1May2017,
                ProgTypeNullable = 2,
                AimType = 1,
                CompStatus = 2,
                LearnStartDate = learnStartDate,
                LearnActEndDateNullable = learnActEndDate,
                LearningDeliveryFAMs = learningDeliveryFAMs
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(2)).Returns(true);
            dateTimeQueryServiceMock.Setup(qs => qs.WholeDaysBetween(learnStartDate, learnActEndDate)).Returns(270);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES")).Returns(false);

            NewRule(
                dd07: dd07Mock.Object,
                dateTimeQueryService: dateTimeQueryServiceMock.Object,
                learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object)
                .ConditionMet(
                    learningDelivery.FundModel,
                    learningDelivery.ProgTypeNullable,
                    learningDelivery.LearnStartDate,
                    learningDelivery.AimType,
                    learningDelivery.LearnActEndDateNullable,
                    learningDelivery.CompStatus,
                    learningDelivery.LearningDeliveryFAMs)
                .Should().BeTrue();
        }

        [Theory]
        [InlineData(99, "2018-05-01", "2020-08-01", 2, 3, 4, false)]
        [InlineData(36, "2017-05-01", "2018-09-01", 1, 2, 2, false)]
        [InlineData(36, "2017-05-01", null, 1, 2, 2, false)]
        [InlineData(36, "2018-05-01", "2018-06-01", 2, 2, 2, false)]
        [InlineData(36, "2018-05-01", "2018-06-01", 1, 25, 2, false)]
        [InlineData(36, "2018-05-01", "2018-06-01", 1, 2, 4, false)]
        [InlineData(36, "2018-05-01", "2018-06-01", 1, 2, 2, true)]
        public void ConditionMet_False(int fundModel, string learnStartDateString, string learnActEndDateString, int aimType, int? progType, int compStatus, bool famMock)
        {
            var learnStartDate = DateTime.Parse(learnStartDateString);
            var learnActEndDate = string.IsNullOrEmpty(learnActEndDateString) ? (DateTime?)null : DateTime.Parse(learnStartDateString);

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

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(progType)).Returns(false);
            dateTimeQueryServiceMock.Setup(qs => qs.WholeDaysBetween(learnStartDate, learnActEndDate ?? new DateTime(2017, 5, 1))).Returns(370);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES")).Returns(famMock);

            NewRule(
                dd07: dd07Mock.Object,
                dateTimeQueryService: dateTimeQueryServiceMock.Object,
                learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object)
                .ConditionMet(
                    learningDelivery.FundModel,
                    learningDelivery.ProgTypeNullable,
                    learningDelivery.LearnStartDate,
                    learningDelivery.AimType,
                    learningDelivery.LearnActEndDateNullable,
                    learningDelivery.CompStatus,
                    learningDelivery.LearningDeliveryFAMs)
                .Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learnStartDate = new DateTime(2017, 05, 01);
            var learnActEndDate = new DateTime(2018, 04, 01);
            var learningDeliveryFAMs = new List<TestLearningDeliveryFAM>
            {
                new TestLearningDeliveryFAM
                {
                    LearnDelFAMCode = "1",
                    LearnDelFAMType = "RES"
                }
            };

            var learningDeliveries = new List<TestLearningDelivery>
            {
                new TestLearningDelivery
                {
                    FundModel = 36,
                    ProgTypeNullable = 2,
                    AimType = 1,
                    CompStatus = 2,
                    LearnStartDate = learnStartDate,
                    LearnActEndDateNullable = learnActEndDate,
                    LearningDeliveryFAMs = learningDeliveryFAMs
                }
            };

            var learner = new TestLearner
            {
                LearnRefNumber = "123456",
                LearningDeliveries = learningDeliveries
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(2)).Returns(true);
            dateTimeQueryServiceMock.Setup(qs => qs.WholeDaysBetween(learnStartDate, learnActEndDate)).Returns(360);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES")).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(
                    dd07: dd07Mock.Object,
                    dateTimeQueryService: dateTimeQueryServiceMock.Object,
                    learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object,
                    validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
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
                LearnRefNumber = "1234567",
                LearningDeliveries = learningDeliveries
            };

            var dd07Mock = new Mock<IDerivedData_07Rule>();
            var dateTimeQueryServiceMock = new Mock<IDateTimeQueryService>();
            var learningDeliveryFAMQueryServiceMock = new Mock<ILearningDeliveryFAMQueryService>();

            dd07Mock.Setup(dd => dd.IsApprenticeship(25)).Returns(false);
            dateTimeQueryServiceMock.Setup(qs => qs.WholeDaysBetween(learnStartDate, learnActEndDate)).Returns(365);
            learningDeliveryFAMQueryServiceMock.Setup(qs => qs.HasLearningDeliveryFAMType(learningDeliveryFAMs, "RES")).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(
                    dd07: dd07Mock.Object,
                    dateTimeQueryService: dateTimeQueryServiceMock.Object,
                    learningDeliveryFAMQueryService: learningDeliveryFAMQueryServiceMock.Object,
                    validationErrorHandler: validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnStartDate", "01/05/2017")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnActEndDate", "01/05/2018")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(new DateTime(2017, 05, 01), new DateTime(2018, 5, 1));

            validationErrorHandlerMock.Verify();
        }

        public DateOfBirth_53Rule NewRule(
            IDerivedData_07Rule dd07 = null,
            IDateTimeQueryService dateTimeQueryService = null,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService = null,
            IValidationErrorHandler validationErrorHandler = null)
        {
            return new DateOfBirth_53Rule(
                dd07: dd07,
                dateTimeQueryService: dateTimeQueryService,
                learningDeliveryFAMQueryService: learningDeliveryFAMQueryService,
                validationErrorHandler: validationErrorHandler);
        }
    }
}
