using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.PlanLearnHours
{
    /// <summary>
    /// If returned, the sum of the Planned learning hours and the Planned employability, enrichment and pastoral hours must not be greater than 4000 hours
    /// </summary>
    public class PlanLearnHours_05Rule : AbstractRule, IRule<ILearner>
    {
        public PlanLearnHours_05Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.PlanLearnHours_05)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (!Excluded(learningDelivery.FundModel, learningDelivery.ProgTypeNullable) && ConditionMet(objectToValidate.PlanLearnHoursNullable, objectToValidate.PlanEEPHoursNullable))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(objectToValidate.PlanLearnHoursNullable, objectToValidate.PlanEEPHoursNullable));
                }
            }
        }

        public bool ConditionMet(int? planLearnHours, int? planEEPHours)
        {
            return (planLearnHours ?? 0) + (planEEPHours ?? 0) > 4000;
        }

        public bool Excluded(int fundModel, int? progType)
        {
            return progType.HasValue
                   && fundModel == FundModels.Age16To19ExcludingApprenticeships
                   && (progType == ProgTypes.TLevel || progType == ProgTypes.TLevelTransition);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int? planLearnHours, int? planEEPHours)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.PlanLearnHours, planLearnHours),
                BuildErrorMessageParameter(PropertyNameConstants.PlanEEPHours, planEEPHours)
            };
        }
    }
}