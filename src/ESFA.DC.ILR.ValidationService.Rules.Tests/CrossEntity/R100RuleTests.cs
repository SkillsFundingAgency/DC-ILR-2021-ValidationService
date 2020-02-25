using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R100RuleTests : AbstractRuleTests<R100Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R100");
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
                        ProgTypeNullable = TypeOfLearningProgramme.ApprenticeshipStandard,
                        AimType = TypeOfAim.ProgrammeAim,
                        CompStatus = CompletionState.HasCompleted,
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = TypeOfLearningProgramme.ApprenticeshipStandard,
                        AimType = TypeOfAim.ProgrammeAim,
                        CompStatus = CompletionState.HasCompleted,
                        AppFinRecords = new List<IAppFinRecord>()
                        {
                            new TestAppFinRecord()
                            {
                                AFinType = ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice,
                                AFinCode = 2,
                            }
                        }
                    }
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learningDelivery = new TestLearningDelivery();

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.IsNonFundedApprenticeshipStandard(learningDelivery)).Returns(false);
            ruleMock.Setup(r => r.IsCompletedApprenticeshipStandardAim(learningDelivery)).Returns(true);
            ruleMock.Setup(r => r.HasAssessmentPrice(learningDelivery)).Returns(false);

            ruleMock.Object.ConditionMet(learningDelivery).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False_FundedApprenticeshipStandard()
        {
            var learningDelivery = new TestLearningDelivery();

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.IsNonFundedApprenticeshipStandard(learningDelivery)).Returns(true);

            ruleMock.Object.ConditionMet(learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_NotCompletedApprenticeshipStandardAim()
        {
            var learningDelivery = new TestLearningDelivery();

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.IsNonFundedApprenticeshipStandard(learningDelivery)).Returns(false);
            ruleMock.Setup(r => r.IsCompletedApprenticeshipStandardAim(learningDelivery)).Returns(false);

            ruleMock.Object.ConditionMet(learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_False_HasAssessmentPrice()
        {
            var learningDelivery = new TestLearningDelivery();

            var ruleMock = NewRuleMock();

            ruleMock.Setup(r => r.IsNonFundedApprenticeshipStandard(learningDelivery)).Returns(false);
            ruleMock.Setup(r => r.IsCompletedApprenticeshipStandardAim(learningDelivery)).Returns(true);
            ruleMock.Setup(r => r.HasAssessmentPrice(learningDelivery)).Returns(true);

            ruleMock.Object.ConditionMet(learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void IsNonFundedApprenticeshipStandard_True()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = TypeOfFunding.NotFundedByESFA,
                ProgTypeNullable = TypeOfLearningProgramme.ApprenticeshipStandard
            };

            NewRule().IsNonFundedApprenticeshipStandard(learningDelivery).Should().BeTrue();
        }

        [Theory]
        [InlineData(0, TypeOfLearningProgramme.ApprenticeshipStandard)]
        [InlineData(TypeOfFunding.NotFundedByESFA, 0)]
        [InlineData(0, 0)]
        [InlineData(TypeOfFunding.NotFundedByESFA, null)]
        public void IsNonFundedApprenticeshipStandard_False(int fundModel, int? progType)
        {
            var learningDelivery = new TestLearningDelivery()
            {
                FundModel = fundModel,
                ProgTypeNullable = progType
            };

            NewRule().IsNonFundedApprenticeshipStandard(learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void IsCompletedApprenticeshipStandardAim_True()
        {
            var learningDelivery = new TestLearningDelivery()
            {
                ProgTypeNullable = TypeOfLearningProgramme.ApprenticeshipStandard,
                AimType = TypeOfAim.ProgrammeAim,
                CompStatus = CompletionState.HasCompleted
            };

            NewRule().IsCompletedApprenticeshipStandardAim(learningDelivery).Should().BeTrue();
        }

        [Theory]
        [InlineData(0, TypeOfAim.ProgrammeAim, CompletionState.HasCompleted)]
        [InlineData(null, TypeOfAim.ProgrammeAim, CompletionState.HasCompleted)]
        [InlineData(TypeOfLearningProgramme.ApprenticeshipStandard, 0, CompletionState.HasCompleted)]
        [InlineData(TypeOfLearningProgramme.ApprenticeshipStandard, TypeOfAim.ProgrammeAim, 0)]
        public void IsCompletedApprenticeshipStandardAim_False(int? progType, int aimType, int compStatus)
        {
            var learningDelivery = new TestLearningDelivery()
            {
                ProgTypeNullable = progType,
                AimType = aimType,
                CompStatus = compStatus
            };

            NewRule().IsCompletedApprenticeshipStandardAim(learningDelivery).Should().BeFalse();
        }

        [Theory]
        [InlineData(ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice, 2)]
        [InlineData(ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice, 4)]
        public void IsAssessmentPrice_True(string type, int code)
        {
            var appFinRecord = new TestAppFinRecord()
            {
                AFinType = type, AFinCode = code
            };

            NewRule().IsAssessmentPrice(appFinRecord).Should().BeTrue();
        }

        [Theory]
        [InlineData(ApprenticeshipFinancialRecord.Types.PaymentRecord, 1)]
        [InlineData(ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice, 3)]
        public void IsAssessmentPrice_False(string type, int code)
        {
            var appFinRecord = new TestAppFinRecord()
            {
                AFinType = type,
                AFinCode = code
            };

            NewRule().IsAssessmentPrice(appFinRecord).Should().BeFalse();
        }

        [Fact]
        public void HasAssessmentPrice_True()
        {
            var assessmentPriceAppFinRecord = new TestAppFinRecord()
            {
                AFinType = ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice,
                AFinCode = 2
            };
            var nonAssesssmentPriceAppFinRecord = new TestAppFinRecord();

            var learningDelivery = new TestLearningDelivery()
            {
                AppFinRecords = new List<IAppFinRecord>()
                {
                    assessmentPriceAppFinRecord,
                    nonAssesssmentPriceAppFinRecord,
                }
            };

            NewRule().HasAssessmentPrice(learningDelivery).Should().BeTrue();
        }

        [Fact]
        public void HasAssessmentPrice_False()
        {
            var nonAssesssmentPriceAppFinRecordOne = new TestAppFinRecord();
            var nonAssesssmentPriceAppFinRecordTwo = new TestAppFinRecord();

            var learningDelivery = new TestLearningDelivery()
            {
                AppFinRecords = new List<IAppFinRecord>()
                {
                    nonAssesssmentPriceAppFinRecordOne,
                    nonAssesssmentPriceAppFinRecordTwo,
                }
            };

            NewRule().HasAssessmentPrice(learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void HasAssessmentPrice_False_Empty()
        {
            var learningDelivery = new TestLearningDelivery();

            NewRule().HasAssessmentPrice(learningDelivery).Should().BeFalse();
        }

        private R100Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new R100Rule(validationErrorHandler);
        }
    }
}
