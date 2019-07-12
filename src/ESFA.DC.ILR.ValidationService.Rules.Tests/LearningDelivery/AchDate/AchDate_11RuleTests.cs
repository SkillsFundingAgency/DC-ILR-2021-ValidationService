using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AchDate;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.AchDate
{
    public class AchDate_11RuleTests : AbstractRuleTests<AchDate_11Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("AchDate_11");
        }

        [Fact]
        public void AimTypeConditionMet_Pass()
        {
            var aimType = 1;
            NewRule().AimTypeConditionMet(aimType).Should().BeTrue();
        }

        [Fact]
        public void AimTypeConditionMet_Fails()
        {
            var aimType = 2;
            NewRule().AimTypeConditionMet(aimType).Should().BeFalse();
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
        public void ProgTypeConditionMet_Pass()
        {
            var progType = 25;
            NewRule().ProgTypeConditionMet(progType).Should().BeTrue();
        }

        [Fact]
        public void ProgTypeConditionMet_Fails()
        {
            var progType = 24;
            NewRule().ProgTypeConditionMet(progType).Should().BeFalse();
        }

        [Fact]
        public void LearnActEndDate_Pass_AsGreaterThanFirstAug2019()
        {
            var learnActEndDate = new DateTime(2019, 09, 01);
            var academicStartDate = new DateTime(2019, 8, 1);

            var mockAcademicYearDataService = new Mock<IAcademicYearDataService>();
            mockAcademicYearDataService.Setup(x => x.Start()).Returns(academicStartDate);

            NewRule(mockAcademicYearDataService.Object)
                .LearnActEndDateConditionMet(learnActEndDate)
                .Should().BeTrue();
        }

        [Fact]
        public void LearnActEndDate_Pass_AsEqualsToFirstAug2019()
        {
            var learnActEndDate = new DateTime(2019, 08, 01);
            var academicStartDate = new DateTime(2019, 8, 1);

            var mockAcademicYearDataService = new Mock<IAcademicYearDataService>();
            mockAcademicYearDataService.Setup(x => x.Start()).Returns(academicStartDate);

            NewRule(mockAcademicYearDataService.Object)
                .LearnActEndDateConditionMet(learnActEndDate)
                .Should().BeTrue();
        }

        [Fact]
        public void LearnActEndDate_Fails_AsLessThanFirstAug2019()
        {
            var learnActEndDate = new DateTime(2019, 07, 01);
            var academicStartDate = new DateTime(2019, 8, 1);

            var mockAcademicYearDataService = new Mock<IAcademicYearDataService>();
            mockAcademicYearDataService.Setup(x => x.Start()).Returns(academicStartDate);

            NewRule(mockAcademicYearDataService.Object).LearnActEndDateConditionMet(learnActEndDate).Should().BeFalse();
        }

        [Fact]
        public void LearnActEndDate_Fails_AslearnActDateNull()
        {
            DateTime? learnActEndDate = null;
            var academicStartDate = new DateTime(2019, 8, 1);

            var mockAcademicYearDataService = new Mock<IAcademicYearDataService>();
            mockAcademicYearDataService.Setup(x => x.Start()).Returns(academicStartDate);

            NewRule(mockAcademicYearDataService.Object).LearnActEndDateConditionMet(learnActEndDate).Should().BeFalse();
        }

        [Fact]
        public void AchDateCondition_Pass()
        {
            var learnActEndDate = new DateTime(2019, 09, 15);
            var achDate = new DateTime(2019, 09, 05);
            NewRule().AchDateConditionMet(achDate, learnActEndDate).Should().BeTrue();
        }

        [Fact]
        public void AchDateCondition_Fails()
        {
            var learnActEndDate = new DateTime(2019, 09, 15);
            var achDate = new DateTime(2019, 10, 08);
            NewRule().AchDateConditionMet(achDate, learnActEndDate).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_Pass()
        {
            var aimType = 1;
            var fundModel = 36;
            var progType = 25;
            var learnActEndDate = new DateTime(2019, 09, 15);
            var achDate = new DateTime(2019, 09, 05);
            var academicStartDate = new DateTime(2019, 8, 1);

            var mockAcademicYearDataService = new Mock<IAcademicYearDataService>();
            mockAcademicYearDataService.Setup(x => x.Start()).Returns(academicStartDate);

            NewRule(mockAcademicYearDataService.Object).ConditionMet(aimType, fundModel, progType, learnActEndDate, achDate).Should().BeTrue();
        }

        [Theory]
        [InlineData(false, true, true, true, true)]
        [InlineData(true, false, true, true, true)]
        [InlineData(true, true, false, true, true)]
        [InlineData(true, true, true, false, true)]
        [InlineData(true, true, true, true, false)]
        public void ConditionMet_Fails(bool aimCondition, bool fundCondition, bool progCondition, bool learnActDateCondition, bool achDateCondition)
        {
            var aimType = 1;
            var fundModel = 36;
            var progType = 25;
            var learnActEndDate = new DateTime(2019, 09, 15);
            var achDate = new DateTime(2019, 09, 05);

            var mockedRule = NewRuleMock();

            mockedRule.Setup(x => x.AimTypeConditionMet(aimType)).Returns(aimCondition);
            mockedRule.Setup(x => x.FundModelConditionMet(fundModel)).Returns(fundCondition);
            mockedRule.Setup(x => x.ProgTypeConditionMet(progType)).Returns(progCondition);
            mockedRule.Setup(x => x.LearnActEndDateConditionMet(learnActEndDate)).Returns(learnActDateCondition);
            mockedRule.Setup(x => x.AchDateConditionMet(achDate, learnActEndDate)).Returns(achDateCondition);

            var expectedResult = mockedRule.Object.ConditionMet(aimType, fundModel, progType, learnActEndDate, achDate);

            expectedResult.Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var academicStartDate = new DateTime(2019, 8, 1);

            var learner = new TestLearner()
            {
                LearnRefNumber = "123",
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimSeqNumber = 555,
                        AimType = 1,
                        FundModel = 36,
                        ProgTypeNullable = 25,
                        LearnActEndDateNullable = new DateTime(2019, 9, 1),
                        AchDateNullable = new DateTime(2019, 8, 20)
                    }
                }
            };

            var mockAcademicYearDataService = new Mock<IAcademicYearDataService>();
            mockAcademicYearDataService.Setup(x => x.Start()).Returns(academicStartDate);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(mockAcademicYearDataService.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var academicStartDate = new DateTime(2019, 8, 1);
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<TestLearningDelivery>()
                {
                    new TestLearningDelivery()
                }
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
            var aimType = 1;
            var fundModel = 36;
            var progType = 25;
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("AimType", aimType)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("FundModel", fundModel)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("ProgType", progType)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnActEndDate", "21/08/2019")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("AchDate", "25/06/2019")).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(aimType, fundModel, progType, new DateTime(2019, 08, 21), new DateTime(2019, 06, 25));

            validationErrorHandlerMock.Verify();
        }

        public AchDate_11Rule NewRule(IAcademicYearDataService academicYearDataService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new AchDate_11Rule(academicYearDataService, validationErrorHandler);
        }
    }
}
