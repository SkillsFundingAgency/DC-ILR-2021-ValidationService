using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
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
            if (!LearningDeliveryConditionMet(objectToValidate.LearningDeliveries))
            {
                return;
            }

            if (objectToValidate.LearnerEmploymentStatuses == null)
            {
                return;
            }

            var learnerEmploymentStatusMonitorings = GetLearnerEmploymentStatusesMonitorings(objectToValidate.LearnerEmploymentStatuses);

            foreach (var employmentStatusMonitoring in learnerEmploymentStatusMonitorings)
            {
                if (ConditionMet(employmentStatusMonitoring))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        errorMessageParameters: BuildErrorMessageParameters(employmentStatusMonitoring.ESMCode));
                }
            }
        }

        public bool LearningDeliveryConditionMet(IEnumerable<ILearningDelivery> learningDeliveries)
        {
            return learningDeliveries?
                       .Any(ld => ld.LearningDeliveryFAMs?.Any(fam => fam.LearnDelFAMType == LearningDeliveryFAMTypeConstants.LDM && fam.LearnDelFAMCode == LearningDeliveryFAMCodeConstants.LDM_375) ?? false) ?? false;
        }

        public bool ConditionMet(IEmploymentStatusMonitoring employmentStatusMonitoring)
        {
            return !_esmCodes.Contains(employmentStatusMonitoring.ESMCode);
        }

        public IEnumerable<IEmploymentStatusMonitoring> GetLearnerEmploymentStatusesMonitorings(IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses)
        {
            return learnerEmploymentStatuses?
                .Where(x => x.EmploymentStatusMonitorings.Any(e => e.ESMType == Monitoring.EmploymentStatus.Types.BenefitStatusIndicator))
                .SelectMany(s => s.EmploymentStatusMonitorings);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int? esmCode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.LDM),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, LearningDeliveryFAMCodeConstants.LDM_375),
                BuildErrorMessageParameter(PropertyNameConstants.ESMType, Monitoring.EmploymentStatus.Types.BenefitStatusIndicator),
                BuildErrorMessageParameter(PropertyNameConstants.ESMCode, esmCode),
            };
        }
    }
}
