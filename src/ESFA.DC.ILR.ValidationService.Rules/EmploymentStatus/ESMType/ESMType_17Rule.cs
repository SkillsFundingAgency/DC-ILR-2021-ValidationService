using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.ESMType
{
    public class ESMType_17Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<int> _esmCodes = new HashSet<int>
        {
            LearnerEmploymentStatusConstants.ESMCodes.BSI_ReceiptOfJSA,
            LearnerEmploymentStatusConstants.ESMCodes.BSI_ReceiptOfUniversalCredit,
            LearnerEmploymentStatusConstants.ESMCodes.BSI_ReceiptOfEmploymentAndSupport
        };

        public ESMType_17Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.ESMType_17)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null || objectToValidate?.LearnerEmploymentStatuses == null)
            {
                return;
            }

            if (!LearningDeliveryConditionMet(objectToValidate.LearningDeliveries))
            {
                return;
            }

            if (ConditionMet(objectToValidate.LearnerEmploymentStatuses))
            {
                HandleValidationError(objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters());
            }
        }

        public bool LearningDeliveryConditionMet(IEnumerable<ILearningDelivery> learningDeliveries)
        {
            return learningDeliveries?
                       .Any(ld => ld.LearningDeliveryFAMs?.Any(fam => fam.LearnDelFAMType == LearningDeliveryFAMTypeConstants.LDM && fam.LearnDelFAMCode == LearningDeliveryFAMCodeConstants.LDM_375) ?? false) ?? false;
        }

        public bool ConditionMet(IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses)
        {
            var allESMs = learnerEmploymentStatuses?.Where(les => les.EmploymentStatusMonitorings != null).SelectMany(s => s.EmploymentStatusMonitorings);

            return allESMs?.Any(esm => esm?.ESMType == Monitoring.EmploymentStatus.Types.BenefitStatusIndicator && !_esmCodes.Contains(esm.ESMCode)) ?? false;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters()
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.LDM),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, LearningDeliveryFAMCodeConstants.LDM_375),
                BuildErrorMessageParameter(PropertyNameConstants.ESMType, Monitoring.EmploymentStatus.Types.BenefitStatusIndicator)
            };
        }
    }
}
