using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.Outcome
{
    public class Outcome_05Rule : AbstractRule, IRule<ILearner>
    {
        public Outcome_05Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.Outcome_05)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(
                    learningDelivery.OutcomeNullable,
                    learningDelivery.LearnActEndDateNullable))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learningDelivery.OutcomeNullable));
                }
            }
        }

        public bool ConditionMet(int? outcome, DateTime? learnActEndDate)
        {
            return OutcomeConditionMet(outcome)
                   && LearnActEndDateConditionMet(learnActEndDate);
        }

        public bool OutcomeConditionMet(int? outcome)
        {
            return outcome == 1;
        }

        public bool LearnActEndDateConditionMet(DateTime? learnActEndDate)
        {
            return !learnActEndDate.HasValue;
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
