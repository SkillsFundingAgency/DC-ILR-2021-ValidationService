using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.PlanLearnHours
{
    public class PlanLearnHours_02Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<long> _fundModels = new HashSet<long> { 10, 25, 35, 36, 81, 99 };

        public PlanLearnHours_02Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.PlanLearnHours_02)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(objectToValidate.PlanLearnHoursNullable, learningDelivery.FundModel, learningDelivery.ProgTypeNullable))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, errorMessageParameters: BuildErrorMessageParameters(objectToValidate.PMUKPRNNullable, learningDelivery.FundModel));
                    return;
                }
            }
        }

        public bool ConditionMet(int? planLearnHours, int fundModel, int? progType)
        {
            return !Excluded(fundModel, progType)
                   && PlanLearnHoursConditionMet(planLearnHours)
                   && FundModelConditionMet(fundModel);
        }

        public bool PlanLearnHoursConditionMet(int? planLearnHours)
        {
            return planLearnHours.HasValue
                && planLearnHours.Value == 0;
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return _fundModels.Contains(fundModel);
        }

        public bool Excluded(int fundModel, int? progType)
        {
            return fundModel == FundModels.Age16To19ExcludingApprenticeships
                   && progType == ProgTypes.TLevel;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int? planLearnHours, int fundModel)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.PlanLearnHours, planLearnHours),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel)
            };
        }
    }
}