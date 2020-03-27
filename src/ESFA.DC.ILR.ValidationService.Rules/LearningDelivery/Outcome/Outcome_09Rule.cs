using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.Outcome
{
    public class Outcome_09Rule : AbstractRule, IRule<ILearner>
    {
        public Outcome_09Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.Outcome_09)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                             learningDelivery.OutcomeNullable,
                             learningDelivery.CompStatus,
                             learningDelivery.FundModel,
                             learningDelivery.ProgTypeNullable))
                {
                    HandleValidationError(
                             objectToValidate.LearnRefNumber,
                             learningDelivery.AimSeqNumber,
                             BuildErrorMessageParameters(learningDelivery.OutcomeNullable, learningDelivery.CompStatus));
                }
            }
        }

        public bool ConditionMet(int? outcome, int compStatus, int fundModel, int? progType)
        {
            return OutcomeConditionMet(outcome)
                   && CompStatusConditionMet(compStatus)
                   && ExceptionConditionMet(fundModel, progType);
        }

        public bool OutcomeConditionMet(int? outcome)
        {
            return outcome.HasValue
                   && outcome == OutcomeConstants.LearningActivitiesCompleteButOutcomeNotKnown;
        }

        public bool CompStatusConditionMet(int compStatus)
        {
            return compStatus != CompletionState.HasCompleted;
        }

        public bool ExceptionConditionMet(int fundModel, int? progType)
        {
            return (!progType.HasValue || progType != TypeOfLearningProgramme.ApprenticeshipStandard)
                && fundModel != TypeOfFunding.ApprenticeshipsFrom1May2017;
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
