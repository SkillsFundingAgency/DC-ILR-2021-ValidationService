using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.WithdrawReason
{
    public class WithdrawReason_05Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public WithdrawReason_05Rule(ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.WithdrawReason_05)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
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
                        learnRefNumber: objectToValidate.LearnRefNumber,
                        aimSequenceNumber: learningDelivery.AimSeqNumber,
                        errorMessageParameters: BuildErrorMessageParameters(learningDelivery.WithdrawReasonNullable));
                }
            }
        }

        public bool ConditionMet(ILearningDelivery learningDelivery)
        {
            return WithdrawReasonConditionMet(learningDelivery.WithdrawReasonNullable)
                && LearningDeliveryFAMConditionMet(learningDelivery.LearningDeliveryFAMs);
        }

        public bool WithdrawReasonConditionMet(int? withdrawReason)
        {
            return withdrawReason.HasValue && withdrawReason.Value == ReasonForWithdrawal.OLASSLearnerOutsideProvidersControl;
        }

        public bool LearningDeliveryFAMConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return !_learningDeliveryFAMQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, LearningDeliveryFAMCodeConstants.LDM_OLASS);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int? withdrawReason)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.WithdrawReason, withdrawReason)
            };
        }
    }
}
