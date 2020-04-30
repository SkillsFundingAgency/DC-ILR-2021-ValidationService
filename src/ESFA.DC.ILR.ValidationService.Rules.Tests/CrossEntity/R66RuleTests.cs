using System.Linq;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.CrossEntity;
using ESFA.DC.ILR.ValidationService.Rules.Tests.Abstract;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Rules.Tests.CrossEntity
{
    public class R66RuleTests : AbstractRuleTests<R66Rule>
    {
        [Fact]
        public void RuleName()
        {
            NewRule().RuleName.Should().Be("R66");
        }

        [Fact]
        public void FundModelConditionMet_False()
        {
            NewRule().FundModelConditionMet(FundModels.NotFundedByESFA).Should().BeFalse();
        }

        [Fact]
        public void FundModelConditionMet_True()
        {
            NewRule().FundModelConditionMet(FundModels.ApprenticeshipsFrom1May2017).Should().BeTrue();
        }

        [Fact]
        public void AimTypeConditionMet_False()
        {
            var learningDeliveriesProgrammingAimType = new[]
            {
                new TestLearningDelivery()
                {
                    LearnAimRef = "101",
                    AimType = TypeOfAim.ComponentAimInAProgramme,
                    FundModel = FundModels.ApprenticeshipsFrom1May2017,
                    ProgTypeNullable = TypeOfLearningProgramme.ApprenticeshipStandard,
                    FworkCodeNullable = 1,
                    PwayCodeNullable = 2,
                    StdCodeNullable = 3
                }
            };

            NewRule().AimTypeConditionMet(
                aimType: TypeOfAim.AimNotPartOfAProgramme,
                fundModel: FundModels.ApprenticeshipsFrom1May2017,
                progTypeNullable: TypeOfLearningProgramme.ApprenticeshipStandard,
                fworkCodeNullable: 1,
                pwayCodeNullable: 2,
                stdCodeNullable: 3,
                learningDeliveriesProgrammingAimType: learningDeliveriesProgrammingAimType).Should().BeFalse();
        }

        [Fact]
        public void AimTypeConditionMet_LearningDeliveriesProgramAim_Match_False()
        {
            var learningDeliveriesProgrammingAimType = new[]
            {
                new TestLearningDelivery()
                {
                    LearnAimRef = "101",
                    AimType = TypeOfAim.ComponentAimInAProgramme,
                    FundModel = FundModels.ApprenticeshipsFrom1May2017,
                    ProgTypeNullable = TypeOfLearningProgramme.ApprenticeshipStandard,
                    FworkCodeNullable = 1,
                    PwayCodeNullable = 2,
                    StdCodeNullable = 3
                }
            };

            NewRule().AimTypeConditionMet(
                aimType: TypeOfAim.ComponentAimInAProgramme,
                fundModel: FundModels.ApprenticeshipsFrom1May2017,
                progTypeNullable: TypeOfLearningProgramme.ApprenticeshipStandard,
                fworkCodeNullable: 1,
                pwayCodeNullable: 2,
                stdCodeNullable: 3,
                learningDeliveriesProgrammingAimType: learningDeliveriesProgrammingAimType).Should().BeFalse();
        }

        [Fact]
        public void AimTypeConditionMet_True()
        {
            var learningDeliveriesProgrammingAimType = new[]
            {
                new TestLearningDelivery()
                {
                    LearnAimRef = "101",
                    AimType = TypeOfAim.ProgrammeAim,
                    FundModel = FundModels.NotFundedByESFA,
                    ProgTypeNullable = TypeOfLearningProgramme.ApprenticeshipStandard,
                    FworkCodeNullable = 2,
                    PwayCodeNullable = 3,
                    StdCodeNullable = 5
                }
            };

            NewRule().AimTypeConditionMet(
                aimType: TypeOfAim.ComponentAimInAProgramme,
                fundModel: FundModels.ApprenticeshipsFrom1May2017,
                progTypeNullable: TypeOfLearningProgramme.ApprenticeshipStandard,
                fworkCodeNullable: 4,
                pwayCodeNullable: 5,
                stdCodeNullable: 6,
                learningDeliveriesProgrammingAimType: learningDeliveriesProgrammingAimType).Should().BeTrue();
        }

        [Fact]
        public void AimTypeConditionMet_LearningDeliveriesProgrammAim_Null_True()
        {
            NewRule().AimTypeConditionMet(
                aimType: TypeOfAim.ComponentAimInAProgramme,
                fundModel: FundModels.ApprenticeshipsFrom1May2017,
                progTypeNullable: TypeOfLearningProgramme.ApprenticeshipStandard,
                fworkCodeNullable: 4,
                pwayCodeNullable: 5,
                stdCodeNullable: 6,
                learningDeliveriesProgrammingAimType: null).Should().BeTrue();
        }

        [Fact]
        public void ConditionMet_False()
        {
            var learningDeliveriesProgrammingAimType = new[]
           {
                new TestLearningDelivery()
                {
                    LearnAimRef = "101",
                    AimType = TypeOfAim.ComponentAimInAProgramme,
                    FundModel = FundModels.ApprenticeshipsFrom1May2017,
                    ProgTypeNullable = TypeOfLearningProgramme.ApprenticeshipStandard,
                    FworkCodeNullable = 1,
                    PwayCodeNullable = 2,
                    StdCodeNullable = 3
                }
           };

            NewRule().ConditionMet(
                aimType: TypeOfAim.AimNotPartOfAProgramme,
                fundModel: FundModels.ApprenticeshipsFrom1May2017,
                progTypeNullable: TypeOfLearningProgramme.ApprenticeshipStandard,
                fworkCodeNullable: 1,
                pwayCodeNullable: 2,
                stdCodeNullable: 3,
                learningDeliveriesProgrammingAimType: learningDeliveriesProgrammingAimType).Should().BeFalse();
        }

        [Fact]
        public void ConditionMet_True()
        {
            var learningDeliveriesProgrammingAimType = new[]
            {
                new TestLearningDelivery()
                {
                    LearnAimRef = "101",
                    AimType = TypeOfAim.ProgrammeAim,
                    FundModel = FundModels.NotFundedByESFA,
                    ProgTypeNullable = TypeOfLearningProgramme.ApprenticeshipStandard,
                    FworkCodeNullable = 2,
                    PwayCodeNullable = 3,
                    StdCodeNullable = 5
                }
            };

            NewRule().AimTypeConditionMet(
                aimType: TypeOfAim.ComponentAimInAProgramme,
                fundModel: FundModels.ApprenticeshipsFrom1May2017,
                progTypeNullable: TypeOfLearningProgramme.ApprenticeshipStandard,
                fworkCodeNullable: 4,
                pwayCodeNullable: 5,
                stdCodeNullable: 6,
                learningDeliveriesProgrammingAimType: learningDeliveriesProgrammingAimType).Should().BeTrue();
        }

        [Fact]
        public void Validate_Error()
        {
            var learningDeliveriesProgrammingAimType = new[]
            {
                new TestLearningDelivery()
                {
                    LearnAimRef = "101",
                    AimType = TypeOfAim.ProgrammeAim,
                    FundModel = FundModels.NotFundedByESFA,
                    ProgTypeNullable = TypeOfLearningProgramme.ApprenticeshipStandard,
                    FworkCodeNullable = 2,
                    PwayCodeNullable = 3,
                    StdCodeNullable = 5
                }
            };

            var testLearner = new TestLearner()
            {
                LearnRefNumber = "101Learner",
                LearningDeliveries = new[]
                {
                    new TestLearningDelivery()
                    {
                        AimType = TypeOfAim.ComponentAimInAProgramme,
                        FundModel = FundModels.ApprenticeshipsFrom1May2017,
                        ProgTypeNullable = TypeOfLearningProgramme.ApprenticeshipStandard,
                        FworkCodeNullable = 2,
                        PwayCodeNullable = 3,
                        StdCodeNullable = 5
                    },
                    learningDeliveriesProgrammingAimType.FirstOrDefault()
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void Validate_NoError()
        {
            var learningDeliveriesProgrammingAimType = new[]
            {
                new TestLearningDelivery()
                {
                    LearnAimRef = "101",
                    AimType = TypeOfAim.ProgrammeAim,
                    FundModel = FundModels.NotFundedByESFA,
                    ProgTypeNullable = TypeOfLearningProgramme.ApprenticeshipStandard,
                    FworkCodeNullable = 2,
                    PwayCodeNullable = 3,
                    StdCodeNullable = 5
                }
            };

            var testLearner = new TestLearner()
            {
                LearnRefNumber = "101Learner",
                LearningDeliveries = new[]
                {
                    new TestLearningDelivery()
                    {
                        AimType = TypeOfAim.ComponentAimInAProgramme,
                        FundModel = FundModels.NotFundedByESFA,
                        ProgTypeNullable = TypeOfLearningProgramme.ApprenticeshipStandard,
                        FworkCodeNullable = 2,
                        PwayCodeNullable = 3,
                        StdCodeNullable = 5
                    },
                    learningDeliveriesProgrammingAimType.FirstOrDefault()
                }
            };

            using (var validationErrorHandlerMock = BuildValidationErrorHandlerMockForNoError())
            {
                NewRule(validationErrorHandler: validationErrorHandlerMock.Object).Validate(testLearner);
            }
        }

        [Fact]
        public void BuildErrorMessageParameters()
        {
            var validationErrorHandlerMock = new Mock<IValidationErrorHandler>();

            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.AimType, TypeOfAim.ComponentAimInAProgramme)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.FundModel, FundModels.NotFundedByESFA)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.ProgType, TypeOfLearningProgramme.AdvancedLevelApprenticeship)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.FworkCode, 1)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.PwayCode, 2)).Verifiable();
            validationErrorHandlerMock.Setup(v => v.BuildErrorMessageParameter(PropertyNameConstants.StdCode, 3)).Verifiable();

            NewRule(validationErrorHandler: validationErrorHandlerMock.Object).BuildErrorMessageParameters(
                aimType: TypeOfAim.ComponentAimInAProgramme,
                fundModel: FundModels.NotFundedByESFA,
                progTypeNullable: TypeOfLearningProgramme.AdvancedLevelApprenticeship,
                fworkCodeNullable: 1,
                pwayCodeNullable: 2,
                stdCodeNullable: 3);

            validationErrorHandlerMock.Verify();
        }

        public R66Rule NewRule(IValidationErrorHandler validationErrorHandler = null)
        {
            return new R66Rule(validationErrorHandler: validationErrorHandler);
        }
    }
}
