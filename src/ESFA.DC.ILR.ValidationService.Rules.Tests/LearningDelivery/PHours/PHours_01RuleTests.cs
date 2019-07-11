using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LSDPostcode;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.PHours;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.PHours
{
    public class PHours_01RuleTests : AbstractRuleTests<LSDPostcode_01Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("PHours_01");
        }

        [Fact]
        public void StartDateCondition_Pass_AsStartDateIsEqual()
        {
            var startDate = new DateTime(2019, 8, 1);

            var academicStartDate = new DateTime(2019, 8, 1);
            var mockAcademicYearDataService = new Mock<IAcademicYearDataService>();
            mockAcademicYearDataService.Setup(x => x.Start()).Returns(academicStartDate);

            var rule = NewRule(academicYearDataService: mockAcademicYearDataService.Object).StartDateConditionMet(startDate);
            rule.Should().BeTrue();
        }

        [Fact]
        public void StartDateCondition_Pass_AsStartDateIsGreater()
        {
            var startDate = new DateTime(2019, 12, 1);

            var academicStartDate = new DateTime(2019, 8, 1);
            var mockAcademicYearDataService = new Mock<IAcademicYearDataService>();
            mockAcademicYearDataService.Setup(x => x.Start()).Returns(academicStartDate);

            var rule = NewRule(academicYearDataService: mockAcademicYearDataService.Object).StartDateConditionMet(startDate);
            rule.Should().BeTrue();
        }

        [Fact]
        public void StartDateCondition_Fails_AsStartDateIsLessThan()
        {
            var startDate = new DateTime(2019, 6, 1);

            var academicStartDate = new DateTime(2019, 8, 1);
            var mockAcademicYearDataService = new Mock<IAcademicYearDataService>();
            mockAcademicYearDataService.Setup(x => x.Start()).Returns(academicStartDate);

            var rule = NewRule(academicYearDataService: mockAcademicYearDataService.Object).StartDateConditionMet(startDate);
            rule.Should().BeFalse();
        }

        [Fact]
        public void PlannedHoursConditionMet_Fails_AsPHrsNotNull()
        {
            int? pHours = 40;
            NewRule().PlannedHoursConditionMet(pHours).Should().BeFalse();
        }

        [Fact]
        public void PlannedHoursConditionMet_Pass_AsNull()
        {
            int? pHours = null;
            NewRule().PlannedHoursConditionMet(pHours).Should().BeTrue();
        }

        [Fact]
        public void FundModelConditionMet_Pass()
        {
            var fundModel = 36;
            NewRule().FundModelConditionMet(fundModel).Should().BeTrue();
        }

        [Fact]
        public void FundModelConditionMet_Fails()
        {
            var fundModel = 81;
            NewRule().FundModelConditionMet(fundModel).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_Pass()
        {
            var startDate = new DateTime(2019, 12, 1);
            int? pHours = null;
            int fundModel = 36;

            var academicStartDate = new DateTime(2019, 8, 1);
            var mockAcademicYearDataService = new Mock<IAcademicYearDataService>();
            mockAcademicYearDataService.Setup(x => x.Start()).Returns(academicStartDate);

            var rule = NewRule(academicYearDataService: mockAcademicYearDataService.Object).ConditionMet(startDate, pHours, fundModel);
            rule.Should().BeTrue();
        }

        [Theory]
        [InlineData("01/06/2019", 200, 36)]
        [InlineData("01/08/2019", 250, 36)]
        [InlineData("01/12/2019", 300, 81)]
        public void ConditionMet_Fails(string startingDate, int? pHours, int fundModel)
        {
            var startDate = DateTime.Parse(startingDate);
            var academicStartDate = new DateTime(2019, 8, 1);

            var mockAcademicYearDataService = new Mock<IAcademicYearDataService>();
            mockAcademicYearDataService.Setup(x => x.Start()).Returns(academicStartDate);

            var rule = NewRule(academicYearDataService: mockAcademicYearDataService.Object).ConditionMet(startDate, pHours, fundModel);
            rule.Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learnStartDate = new DateTime(2019, 12, 01);
            var academicStartDate = new DateTime(2019, 8, 1);

            var learningDeliveries = new List<TestLearningDelivery>()
            {
                 new TestLearningDelivery
                 {
                     FundModel = 36,
                     AimType = 1,
                     PHoursNullable = null,
                     LearnStartDate = learnStartDate
                 }
            };

            var learner = new TestLearner()
            {
                LearnRefNumber = "LearnRefNumber",
                LearningDeliveries = learningDeliveries
            };

            var mockAcademicYearDataService = new Mock<IAcademicYearDataService>();
            mockAcademicYearDataService.Setup(x => x.Start()).Returns(academicStartDate);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(mockAcademicYearDataService.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learnStartDate = new DateTime(2019, 12, 01);
            var academicStartDate = new DateTime(2019, 8, 1);

            var learningDeliveries = new List<TestLearningDelivery>()
            {
                 new TestLearningDelivery
                 {
                     FundModel = 81, // fundModel doesn't meet the condition, hence no Error!
                     AimType = 1,
                     PHoursNullable = 200,
                     LearnStartDate = learnStartDate
                 }
            };

            var learner = new TestLearner()
            {
                LearnRefNumber = "LearnRefNumber",
                LearningDeliveries = learningDeliveries
            };

            var mockAcademicYearDataService = new Mock<IAcademicYearDataService>();
            mockAcademicYearDataService.Setup(x => x.Start()).Returns(academicStartDate);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(mockAcademicYearDataService.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var fundModel = 36;
            var pHours = 200;
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(x => x.BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel)).Verifiable();
            validationErrorHandlerMock.Setup(x => x.BuildErrorMessageParameter(PropertyNameConstants.PHours, pHours)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(fundModel, pHours);

            validationErrorHandlerMock.Verify();
        }

        public PHours_01Rule NewRule(IAcademicYearDataService academicYearDataService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new PHours_01Rule(academicYearDataService, validationErrorHandler);
        }
    }
}
