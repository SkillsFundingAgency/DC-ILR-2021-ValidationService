using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinType
{
    public class AFinType_01Rule : AbstractRule, IRule<ILearner>
    {
        private const string _afinType = ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice;
        private const int _aFinCode = 1;

        private readonly ILearningDeliveryAppFinRecordQueryService _learningDeliveryAppFinRecordQueryService;

        public AFinType_01Rule(
            ILearningDeliveryAppFinRecordQueryService learningDeliveryAppFinRecordQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.AFinType_01)
        {
            _learningDeliveryAppFinRecordQueryService = learningDeliveryAppFinRecordQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.FundModel,
                    learningDelivery.ProgTypeNullable,
                    learningDelivery.AimType,
                    learningDelivery.AppFinRecords))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        errorMessageParameters: BuildErrorMessageParameters(_afinType, _aFinCode));
                }
            }
        }

        public bool ConditionMet(int fundModel, int? progType, int aimType, IEnumerable<IAppFinRecord> appFinRecords)
        {
            return FundModelConditionMet(fundModel)
                   && ProgTypeConditionMet(progType)
                   && AimTypeConditionMet(aimType)
                   && AppFinConditionMet(appFinRecords);
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return fundModel == FundModels.OtherAdult;
        }

        public bool ProgTypeConditionMet(int? progType)
        {
            return progType == ProgTypes.ApprenticeshipStandard;
        }

        public bool AimTypeConditionMet(int aimType)
        {
            return aimType == AimTypes.ProgrammeAim;
        }

        public bool AppFinConditionMet(IEnumerable<IAppFinRecord> appFinRecords)
        {
            return !_learningDeliveryAppFinRecordQueryService.HasAnyLearningDeliveryAFinCodeForType(appFinRecords, _afinType, _aFinCode);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string aFinType, int aFinCode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AFinType, aFinType),
                BuildErrorMessageParameter(PropertyNameConstants.AFinCode, aFinCode)
            };
        }
    }
}
