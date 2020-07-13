using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.Outcome
{
    public class Outcome_10Rule : AbstractRule, IRule<ILearner>
    {
        public Outcome_10Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.Outcome_10)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                             learningDelivery.OutcomeNullable,
                             learningDelivery.CompStatus))
                {
                    HandleValidationError(
                             objectToValidate.LearnRefNumber,
                             learningDelivery.AimSeqNumber,
                             BuildErrorMessageParameters(learningDelivery.OutcomeNullable, learningDelivery.CompStatus));
                }
            }
        }

        public bool ConditionMet(int? outcome, int compStatus)
        {
            return OutcomeConditionMet(outcome)
                   && CompStatusConditionMet(compStatus);
        }

        public bool OutcomeConditionMet(int? outcome)
        {
            return outcome.HasValue
                   && outcome == OutcomeConstants.NoAchievement;
        }

        public bool CompStatusConditionMet(int compStatus)
        {
            return compStatus == CompletionState.IsOngoing;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int? outcome, int compStatus)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.Outcome, outcome),
                BuildErrorMessageParameter(PropertyNameConstants.CompStatus, compStatus),
            };
        }
    }
}
