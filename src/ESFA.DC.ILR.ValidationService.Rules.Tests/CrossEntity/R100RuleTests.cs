using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
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
            var appFinRecords = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 2,
                    AFinDate = new DateTime(2018, 9, 1)
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = ProgTypes.ApprenticeshipStandard,
                        AimType = AimTypes.ProgrammeAim,
                        CompStatus = CompletionState.HasCompleted,
                        AppFinRecords = appFinRecords
                    }
                }
            };

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            appFinRecordQueryServiceMock.Setup(x => x.HasAnyLearningDeliveryAFinCodesForType(appFinRecords, "TNP", new HashSet<int> { 2, 4 })).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(appFinRecordQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var appFinRecords = new List<TestAppFinRecord>
            {
                new TestAppFinRecord
                {
                    AFinType = "TNP",
                    AFinCode = 2,
                    AFinDate = new DateTime(2018, 9, 1)
                }
            };

            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = ProgTypes.ApprenticeshipStandard,
                        AimType = AimTypes.ProgrammeAim,
                        CompStatus = CompletionState.HasCompleted,
                        AppFinRecords = appFinRecords
                    }
                }
            };

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            appFinRecordQueryServiceMock.Setup(x => x.HasAnyLearningDeliveryAFinCodesForType(appFinRecords, "TNP", new HashSet<int> { 2, 4 })).Returns(true);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(appFinRecordQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
            }
        }

        [Fact]
        public void Validate_Error_NoAppFinRecords()
        {
            var learner = new TestLearner()
            {
                LearningDeliveries = new List<ILearningDelivery>()
                {
                    new TestLearningDelivery()
                    {
                        ProgTypeNullable = ProgTypes.ApprenticeshipStandard,
                        AimType = AimTypes.ProgrammeAim,
                        CompStatus = CompletionState.HasCompleted
                    }
                }
            };

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            appFinRecordQueryServiceMock.Setup(x => x.HasAnyLearningDeliveryAFinCodesForType(null, "TNP", new HashSet<int> { 2, 4 })).Returns(false);

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(appFinRecordQueryServiceMock.Object, validationErrorHandlerMock.Object).Validate(learner);
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
                FundModel = FundModels.NotFundedByESFA,
                ProgTypeNullable = ProgTypes.ApprenticeshipStandard
            };

            NewRule().IsNonFundedApprenticeshipStandard(learningDelivery).Should().BeTrue();
        }

        [Theory]
        [InlineData(0, ProgTypes.ApprenticeshipStandard)]
        [InlineData(FundModels.NotFundedByESFA, 0)]
        [InlineData(0, 0)]
        [InlineData(FundModels.NotFundedByESFA, null)]
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
                ProgTypeNullable = ProgTypes.ApprenticeshipStandard,
                AimType = AimTypes.ProgrammeAim,
                CompStatus = CompletionState.HasCompleted
            };

            NewRule().IsCompletedApprenticeshipStandardAim(learningDelivery).Should().BeTrue();
        }

        [Theory]
        [InlineData(0, AimTypes.ProgrammeAim, CompletionState.HasCompleted)]
        [InlineData(null, AimTypes.ProgrammeAim, CompletionState.HasCompleted)]
        [InlineData(ProgTypes.ApprenticeshipStandard, 0, CompletionState.HasCompleted)]
        [InlineData(ProgTypes.ApprenticeshipStandard, AimTypes.ProgrammeAim, 0)]
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

        [Fact]
        public void HasAssessmentPrice_True()
        {
            var appFinRecords = new List<IAppFinRecord>
            {
                new TestAppFinRecord()
                {
                    AFinType = ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice,
                    AFinCode = 2
                },
                new TestAppFinRecord()
            };

            var learningDelivery = new TestLearningDelivery()
            {
                AppFinRecords = appFinRecords
            };

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            appFinRecordQueryServiceMock.Setup(x => x.HasAnyLearningDeliveryAFinCodesForType(appFinRecords, "TNP", new HashSet<int> { 2, 4 })).Returns(true);

            NewRule(appFinRecordQueryServiceMock.Object).HasAssessmentPrice(learningDelivery).Should().BeTrue();
        }

        [Fact]
        public void HasAssessmentPrice_False()
        {
            var appFinRecords = new List<IAppFinRecord>
            {
                new TestAppFinRecord()
                {
                    AFinType = ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice,
                    AFinCode = 1
                },
                new TestAppFinRecord()
            };

            var learningDelivery = new TestLearningDelivery()
            {
                AppFinRecords = appFinRecords
            };

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            appFinRecordQueryServiceMock.Setup(x => x.HasAnyLearningDeliveryAFinCodesForType(appFinRecords, "TNP", new HashSet<int> { 2, 4 })).Returns(false);

            NewRule(appFinRecordQueryServiceMock.Object).HasAssessmentPrice(learningDelivery).Should().BeFalse();
        }

        [Fact]
        public void HasAssessmentPrice_False_Empty()
        {
            var learningDelivery = new TestLearningDelivery();

            var appFinRecordQueryServiceMock = new Mock<ILearningDeliveryAppFinRecordQueryService>();
            appFinRecordQueryServiceMock.Setup(x => x.HasAnyLearningDeliveryAFinCodesForType(null, "TNP", new HashSet<int> { 2, 4 })).Returns(false);

            NewRule(appFinRecordQueryServiceMock.Object).HasAssessmentPrice(learningDelivery).Should().BeFalse();
        }

        private R100Rule NewRule(ILearningDeliveryAppFinRecordQueryService appFinRecordQueryService = null, IValidationErrorHandler validationErrorHandler = null)
        {
            return new R100Rule(validationErrorHandler, appFinRecordQueryService ?? Mock.Of<ILearningDeliveryAppFinRecordQueryService>());
        }
    }
}
