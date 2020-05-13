using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R100Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<int> _tnpCodes = new HashSet<int> { 2, 4 };
        private readonly ILearningDeliveryAppFinRecordQueryService _appFinRecordQueryService;

        public R100Rule(IValidationErrorHandler validationErrorHandler, ILearningDeliveryAppFinRecordQueryService appFinRecordQueryService)
            : base(validationErrorHandler, RuleNameConstants.R100)
        {
            _appFinRecordQueryService = appFinRecordQueryService;
        }

        public R100Rule()
            : base(null, RuleNameConstants.R100)
        {
        }

        public void Validate(ILearner learner)
        {
            if (learner.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in learner.LearningDeliveries.Where(ConditionMet))
            {
                HandleValidationError(
                    learner.LearnRefNumber,
                    learningDelivery.AimSeqNumber,
                    BuildErrorMessageParameters(
                        learningDelivery.AimType,
                        learningDelivery.FundModel,
                        learningDelivery.ProgTypeNullable,
                        learningDelivery.CompStatus));
            }
        }

        public bool ConditionMet(ILearningDelivery learningDelivery)
        {
            return !IsNonFundedApprenticeshipStandard(learningDelivery)
                   && IsCompletedApprenticeshipStandardAim(learningDelivery)
                   && !HasAssessmentPrice(learningDelivery);
        }

        public virtual bool IsNonFundedApprenticeshipStandard(ILearningDelivery learningDelivery)
        {
            return learningDelivery.FundModel == FundModels.NotFundedByESFA
                   && learningDelivery.ProgTypeNullable == ProgTypes.ApprenticeshipStandard;
        }

        public virtual bool IsCompletedApprenticeshipStandardAim(ILearningDelivery learningDelivery)
        {
            return learningDelivery.ProgTypeNullable == ProgTypes.ApprenticeshipStandard
                   && learningDelivery.AimType == AimTypes.ProgrammeAim
                   && learningDelivery.CompStatus == CompletionState.HasCompleted;
        }

        public virtual bool HasAssessmentPrice(ILearningDelivery learningDelivery)
        {
            return _appFinRecordQueryService.HasAnyLearningDeliveryAFinCodesForType(
                learningDelivery?.AppFinRecords,
                ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice,
                _tnpCodes);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int aimType, int fundModel, int? progType, int compStatus)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AimType, aimType),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, progType),
                BuildErrorMessageParameter(PropertyNameConstants.CompStatus, compStatus)
            };
        }
    }
}
