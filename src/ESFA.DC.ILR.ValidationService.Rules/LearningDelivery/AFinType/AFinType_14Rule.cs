using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AFinType
{
    public class AFinType_14Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryAppFinRecordQueryService _learningDeliveryAppFinRecordQueryService;
        private readonly int _aFinCodeForError = 1;

        public AFinType_14Rule(ILearningDeliveryAppFinRecordQueryService learningDeliveryAppFinRecordQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.AFinType_14)
        {
            _learningDeliveryAppFinRecordQueryService = learningDeliveryAppFinRecordQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.AimType,
                    learningDelivery.AppFinRecords))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(ApprenticeshipFinancialRecord.Types.PaymentRecord, _aFinCodeForError));
                }
            }
        }

        public bool ConditionMet(int aimType, IEnumerable<IAppFinRecord> appFinRecords)
        {
            return AimTypeConditionMet(aimType)
                && PMRConditionMet(appFinRecords)
                && TNPConditionMet(appFinRecords);
        }

        public bool AimTypeConditionMet(int aimType)
        {
            return aimType == 1;
        }

        public bool PMRConditionMet(IEnumerable<IAppFinRecord> appFinRecords)
        {
            return _learningDeliveryAppFinRecordQueryService
                .HasAnyLearningDeliveryAFinCodeForType(appFinRecords, ApprenticeshipFinancialRecord.Types.PaymentRecord, 1);
        }

        public bool TNPConditionMet(IEnumerable<IAppFinRecord> appFinRecords)
        {
            var aFinCodes = new int[] { 1, 3 };

            return
            !_learningDeliveryAppFinRecordQueryService
            .HasAnyLearningDeliveryAFinCodesForType(appFinRecords, ApprenticeshipFinancialRecord.Types.TotalNegotiatedPrice, aFinCodes);
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
