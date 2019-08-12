using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ValidationService.Data.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.WithdrawReason
{
    public class WithdrawReason_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IProvideLookupDetails _lookupDetails;

        public WithdrawReason_02Rule(IValidationErrorHandler validationErrorHandler, IProvideLookupDetails lookupDetails)
            : base(validationErrorHandler, RuleNameConstants.WithdrawReason_02)
        {
            _lookupDetails = lookupDetails;
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.WithdrawReasonNullable))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learningDelivery.WithdrawReasonNullable));
                }
            }
        }

        public bool ConditionMet(int? withdrawReason)
        {
            return withdrawReason.HasValue
                   && !_lookupDetails.Contains(TypeOfIntegerCodedLookup.WithdrawReason, withdrawReason.Value);
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
