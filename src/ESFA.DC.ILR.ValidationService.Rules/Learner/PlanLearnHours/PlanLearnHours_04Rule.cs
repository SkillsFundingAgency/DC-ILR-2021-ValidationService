using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.PlanLearnHours
{
    public class PlanLearnHours_04Rule : AbstractRule, IRule<ILearner>
    {
        public PlanLearnHours_04Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.PlanLearnHours_04)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (!Excluded(learningDelivery.FundModel, learningDelivery.ProgTypeNullable) && ConditionMet(objectToValidate.PlanLearnHoursNullable, objectToValidate.PlanEEPHoursNullable))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(objectToValidate.PMUKPRNNullable, objectToValidate.PlanEEPHoursNullable));
                }
            }
        }

        public bool ConditionMet(int? planLearnHours, int? planEEPHours)
        {
            return (PlanLearnHoursValue(planLearnHours) + PlanEEPHoursValue(planEEPHours)) > 1000;
        }

        public bool Excluded(int fundModel, int? progType)
        {
            return progType.HasValue
                   && fundModel == FundModels.Age16To19ExcludingApprenticeships
                   && (progType == ProgTypes.TLevel || progType == ProgTypes.TLevelTransition);
        }

        public int PlanLearnHoursValue(int? planLearnHours)
        {
            return planLearnHours ?? 0;
        }

        public int PlanEEPHoursValue(int? planEEPHours)
        {
            return planEEPHours ?? 0;
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