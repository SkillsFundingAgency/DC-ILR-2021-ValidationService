using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinType
{
    public class AFinType_07Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryAppFinRecordQueryService _learningDeliveryAppFinRecordQueryService;

        public AFinType_07Rule(
            ILearningDeliveryAppFinRecordQueryService learningDeliveryAppFinRecordQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.AFinType_07)
        {
            _learningDeliveryAppFinRecordQueryService = learningDeliveryAppFinRecordQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.ProgTypeNullable,
                    learningDelivery.AimType,
                    learningDelivery.AppFinRecords))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(ApprenticeshipFinancialRecord.Types.PaymentRecord, 2));
                }
            }
        }

        public bool ConditionMet(int? progType, int aimType, IEnumerable<IAppFinRecord> appFinRecords)
        {
            return ProgTypeConditionMet(progType)
                   && AimTypeConditionMet(aimType)
                   && AppFinConditionMet(appFinRecords);
        }

        public bool ProgTypeConditionMet(int? progType)
        {
            return progType == 25;
        }

        public bool AimTypeConditionMet(int aimType)
        {
            return aimType == 1;
        }

        public bool AppFinConditionMet(IEnumerable<IAppFinRecord> appFinRecords)
        {
            var aFinCodes = new[] { 2, 4 };

            return _learningDeliveryAppFinRecordQueryService.HasAnyLearningDeliveryAFinCodeForType(appFinRecords, ApprenticeshipFinancialRecord.Types.PaymentRecord, 2)
                && !_learningDeliveryAppFinRecordQueryService.HasAnyLearningDeliveryAFinCodesForType(appFinRecords, ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice, aFinCodes);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string aFinType, int? aFinCode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AFinType, aFinType),
                BuildErrorMessageParameter(PropertyNameConstants.AFinCode, aFinCode)
            };
        }
    }
}
