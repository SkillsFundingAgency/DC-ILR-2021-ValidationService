using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.Outcome
{
    public class Outcome_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly int[] _validOutcomes =
        {
            OutcomeConstants.Achieved,
            OutcomeConstants.PartialAchievement,
            OutcomeConstants.NoAchievement,
            OutcomeConstants.LearningActivitiesCompleteButOutcomeNotKnown
        };

        public Outcome_01Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.Outcome_01)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.OutcomeNullable))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learningDelivery.OutcomeNullable));
                }
            }
        }

        public bool ConditionMet(int? outcome)
        {
            return outcome.HasValue && !_validOutcomes.Contains(outcome.Value);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int? outcome)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.Outcome, outcome)
            };
        }
    }
}
