using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.CompStatus;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.LearningDelivery.CompStatus
{
    public class CompStatus_07RuleTests : AbstractRuleTests<CompStatus_06Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("CompStatus_07");
        }

        [Fact]
        public void AimTypeCondition_Pass()
        {
            var aimType = 1;
            NewRule().AimTypeConditionMet(aimType).Should().BeTrue();
        }

        [Fact]
        public void AimTypeCondition_Fails()
        {
            var aimType = 2;
            NewRule().AimTypeConditionMet(aimType).Should().BeFalse();
        }

        [Fact]
        public void FundModelContion_Pass()
        {
            var fundModel = 36;
            NewRule().FundModelConditionMet(fundModel).Should().BeTrue();
        }

        [Fact]
        public void FundModelCondition_Fails()
        {
            var fundModel = 81;
            NewRule().FundModelConditionMet(fundModel).Should().BeFalse();
        }

        [Fact]
        public void ProgTypeCondition_Pass()
        {
            int progType = 25;
            NewRule().ProgTypeConditionMet(progType).Should().BeTrue();
        }

        [Theory]
        [InlineData(21)]
        [InlineData(null)]
        public void ProgTypeCondition_Fails(int? progType)
        {
            NewRule().ProgTypeConditionMet(progType).Should().BeFalse();
        }

        [Theory]
        [InlineData("01/08/2019")]
        [InlineData("12/12/2019")]
        public void LearnActEndDateCondition_Pass(string actEndDate)
        {
            var learnActEndDate = DateTime.Parse(actEndDate);
            NewRule().LearnActEndDateConditionMet(learnActEndDate).Should().BeTrue();
        }

        [Theory]
        [InlineData("01/07/2019")]
        [InlineData(null)]
        public void LearnActEndDateCondition_Fails(string strActEndDate)
        {
            DateTime? learnActEndDate = string.IsNullOrEmpty(strActEndDate) ? (DateTime?)null : DateTime.Parse(strActEndDate);

            NewRule().LearnActEndDateConditionMet(learnActEndDate).Should().BeFalse();
        }

        [Fact]
        public void AchDateCondition_Pass()
        {
            DateTime achDate = new DateTime(2019, 08, 01);
            NewRule().AchDateConditionMet(achDate).Should().BeTrue();
        }

        [Fact]
        public void AchDateCondition_Fails_IsNull()
        {
            DateTime? achDate = null;
            NewRule().AchDateConditionMet(achDate).Should().BeFalse();
        }

        [Fact]
        public void CompStatusCondition_Pass()
        {
            var compStatus = 3;
            NewRule().CompStatusConditionMet(compStatus).Should().BeTrue();
        }

        [Fact]
        public void CompStatusCondition_Fail()
        {
            var compStatus = 2;
            NewRule().CompStatusConditionMet(compStatus).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_Pass()
        {
            var aimType = 1;
            var fundModel = 36;
            var progType = 25;
            var compStatus = 3;
            var learnActEndDate = new DateTime(2019, 08, 01);
            var achDate = learnActEndDate.AddMonths(2);

            var rule = NewRule().ConditionMet(aimType, fundModel, progType, compStatus, learnActEndDate, achDate);

            rule.Should().BeTrue();
        }

        [Theory]
        [InlineData(2, 36, 25, 3, "01/08/2019", "01/08/2019")] // aimType condition returns FALSE
        [InlineData(1, 81, 25, 3, "01/08/2019", "01/08/2019")] // fundModel condition returns FALSE
        [InlineData(1, 36, 24, 3, "01/08/2019", "01/08/2019")] // progType condition returns FALSE
        [InlineData(1, 36, 25, 2, "01/08/2019", "01/08/2019")] // compStatus condition returns FALSE
        [InlineData(1, 36, 25, 3, null, "01/08/2019")] // LearnActEndDateConditionMet condition is NULL returns FALSE
        [InlineData(1, 36, 25, 3, "01/07/2019", "01/10/2019")] // LearnActEndDateCondition FALSE as date is Lower than 01/08/2019
        [InlineData(1, 36, 25, 3, "01/12/2019", null)] // AchDate condition is NULL returns FALSE
        public void ConditionMet_Fails(int aimType, int fundModel, int? progType, int compStatus, string strLearnActEndDate, string strAchDate)
        {
            DateTime? learnActEndDate = string.IsNullOrEmpty(strLearnActEndDate) ? (DateTime?)null : DateTime.Parse(strLearnActEndDate);
            DateTime? achDate = string.IsNullOrEmpty(strAchDate) ? (DateTime?)null : DateTime.Parse(strAchDate);

            var rule = NewRule().ConditionMet(aimType, fundModel, progType, compStatus, learnActEndDate, achDate);

            rule.Should().BeFalse();
        }

        [Fact]
        public void Validate_Error()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = 1,
                        FundModel = 36,
                        ProgTypeNullable = 25,
                        CompStatus = 3,
                        LearnActEndDateNullable = new DateTime(2019, 09, 21),
                        AchDateNullable = new DateTime(2019, 09, 21),
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoErrors()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        AimType = 2,
                        FundModel = 81,
                        LearnActEndDateNullable = new DateTime(2019, 06, 21),
                        AchDateNullable = null
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            var fundModel = 36;
            var progType = 25;
            var compStatus = 3;
            var learnActEndDate = new DateTime(2019, 07, 01);
            var achDate = learnActEndDate.AddMonths(1);

            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("FundModel", fundModel)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("ProgType", progType)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("CompStatus", compStatus)).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("LearnStartDate", "01/07/2019")).Verifiable();
            validationErrorHandlerMock.Setup(veh => veh.BuildErrorMessageParameter("AchDate", "01/08/2019")).Verifiable();

            NewRule(validationErrorHandlerMock.Object).BuildErrorMessageParameters(fundModel, progType, compStatus, learnActEndDate, achDate);

            validationErrorHandlerMock.Verify();
        }

        private CompStatus_07Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new CompStatus_07Rule(validationErrorHandler);
        }
    }
}
