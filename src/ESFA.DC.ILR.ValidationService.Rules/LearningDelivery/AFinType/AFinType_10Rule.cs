using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinType
{
    public class AFinType_10Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<int> _tnpCodes = new HashSet<int> { 2, 4 };
        private readonly ILearningDeliveryAppFinRecordQueryService _appFinRecordQueryService;

        public AFinType_10Rule(IValidationErrorHandler validationErrorHandler, ILearningDeliveryAppFinRecordQueryService appFinRecordQueryService)
              : base(validationErrorHandler, RuleNameConstants.AFinType_10)
        {
            _appFinRecordQueryService = appFinRecordQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery))
                {
                    HandleValidationError(
                             objectToValidate.LearnRefNumber,
                             learningDelivery.AimSeqNumber,
                             BuildErrorMessageParameters(
                                 learningDelivery.AimType,
                                 learningDelivery.FundModel,
                                 learningDelivery.ProgTypeNullable));
                }
            }
        }

        public bool ConditionMet(ILearningDelivery learningDelivery)
        {
            return !Exclusion(learningDelivery.FundModel, learningDelivery.ProgTypeNullable)
                && learningDelivery.ProgTypeNullable == ProgTypes.ApprenticeshipStandard
                && AppFinRecordConditionMet(learningDelivery);
        }

        public bool AppFinRecordConditionMet(ILearningDelivery learningDelivery)
        {
            return !_appFinRecordQueryService.HasAnyLearningDeliveryAFinCodesForType(
                learningDelivery.AppFinRecords,
                ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice,
                _tnpCodes);
        }

        public bool Exclusion(int fundModel, int? progTypeNullable)
        {
            return fundModel == FundModels.NotFundedByESFA && progTypeNullable == ProgTypes.ApprenticeshipStandard;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int aimType, int fundModel, int? progType)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AimType, aimType),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, progType)
            };
        }
    }
}
