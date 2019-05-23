using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.PlanLearnHours
{
    public class PlanLearnHours_03Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<long> _fundModels = new HashSet<long>
        {
            TypeOfFunding.Age16To19ExcludingApprenticeships
        };

        public PlanLearnHours_03Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.PlanLearnHours_03)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (
                objectToValidate?.PlanLearnHoursNullable == null
                || !LearnerConditionMet(
                    objectToValidate.PlanLearnHoursNullable,
                    objectToValidate.PlanEEPHoursNullable)
                || objectToValidate.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.FundModel))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(
                            objectToValidate.PlanLearnHoursNullable,
                            objectToValidate.PlanEEPHoursNullable,
                            learningDelivery.FundModel));
                }
            }
        }

        public bool ConditionMet(int fundModel) => FundModelConditionMet(fundModel);

        public bool LearnerConditionMet(int? planLearnHours, int? planEEPHours)
            => (planLearnHours ?? 0) + (planEEPHours ?? 0) == 0;

        public bool FundModelConditionMet(int fundModel) => _fundModels.Contains(fundModel);

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(
            int? planLearnHours,
            int? planEEPHours,
            int fundModel)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.PlanLearnHours, planLearnHours),
                BuildErrorMessageParameter(PropertyNameConstants.PlanEEPHours, planEEPHours),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel)
            };
        }
    }
}
