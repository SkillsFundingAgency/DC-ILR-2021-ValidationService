using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.AgreeId
{
    public class AgreeId_02Rule : AbstractRule, IRule<ILearner>
    {
        private const string _famCode = "1";
        private const string _famType = LearningDeliveryFAMTypeConstants.ACT;

        public AgreeId_02Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.AgreeId_02)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearnerEmploymentStatuses == null)
            {
                return;
            }

            var learningDeliveryFAMs = objectToValidate.LearningDeliveries?
                .Where(ld => ld.LearningDeliveryFAMs != null)
                .SelectMany(ld => ld.LearningDeliveryFAMs);

            foreach (var learnerEmploymentStatus in objectToValidate.LearnerEmploymentStatuses)
            {
                if (ConditionMet(learnerEmploymentStatus, learningDeliveryFAMs))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        errorMessageParameters: BuildErrorMessageParameters(learnerEmploymentStatus.DateEmpStatApp, learnerEmploymentStatus.AgreeId));
                }
            }
        }

        public bool ConditionMet(ILearnerEmploymentStatus learnerEmploymentStatus, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return AgreeIdExists(learnerEmploymentStatus.AgreeId)
                && ACTOneDoesNotExistOnOrAfterDateEmpStat(learningDeliveryFAMs, learnerEmploymentStatus.DateEmpStatApp);
        }

        public bool AgreeIdExists(string agreeId)
        {
            return !string.IsNullOrWhiteSpace(agreeId);
        }

        public bool ACTOneDoesNotExistOnOrAfterDateEmpStat(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs, DateTime dateEmpStat)
        {
            if (learningDeliveryFAMs != null)
            {
                return
                    !learningDeliveryFAMs
                    .Any(f =>
                        f.LearnDelFAMType.CaseInsensitiveEquals(_famType)
                    && f.LearnDelFAMCode.CaseInsensitiveEquals(_famCode)
                    && f.LearnDelFAMDateFromNullable >= dateEmpStat);
            }

            return true;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(DateTime dateEmpStatApp, string agreeId)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.DateEmpStatApp, dateEmpStatApp),
                BuildErrorMessageParameter(PropertyNameConstants.AgreeId, agreeId),
            };
        }
    }
}
