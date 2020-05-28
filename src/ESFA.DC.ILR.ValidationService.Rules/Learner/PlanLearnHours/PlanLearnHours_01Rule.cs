using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.PlanLearnHours
{
    public class PlanLearnHours_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IDerivedData_07Rule _dd07;

        public PlanLearnHours_01Rule(IDerivedData_07Rule dd07, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.PlanLearnHours_01)
        {
            _dd07 = dd07;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (LearnerConditionMet(objectToValidate.PlanLearnHoursNullable, objectToValidate.LearningDeliveries))
            {
                foreach (var learningDelivery in objectToValidate.LearningDeliveries)
                {
                    if (LearningDeliveryConditionMet(learningDelivery.FundModel, learningDelivery.ProgTypeNullable))
                    {
                        HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, errorMessageParameters: BuildErrorMessageParameters(objectToValidate.PlanLearnHoursNullable, learningDelivery.FundModel));
                        return;
                    }
                }
            }
        }

        public bool LearnerConditionMet(int? planLearnHours, IEnumerable<ILearningDelivery> learningDeliveries)
        {
            return !planLearnHours.HasValue
                && !learningDeliveries.All(ld => ld.LearnActEndDateNullable.HasValue);
        }

        public bool PlanLearnHoursConditionMet(int? planLearnHours)
        {
            return !planLearnHours.HasValue;
        }

        public bool LearnActEndDateConditionMet(IEnumerable<ILearningDelivery> learningDeliveries)
        {
            return !learningDeliveries.All(ld => ld.LearnActEndDateNullable.HasValue);
        }

        public bool LearningDeliveryConditionMet(int fundModel, int? progType)
        {
            return !Excluded(fundModel, progType)
                   && FundModelConditionMet(fundModel)
                   && DD07ConditionMet(progType);
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return fundModel != 70;
        }

        public bool DD07ConditionMet(int? progType)
        {
            return !_dd07.IsApprenticeship(progType);
        }

        public bool Excluded(int fundModel, int? progType)
        {
            return progType.HasValue
                   && fundModel == FundModels.Age16To19ExcludingApprenticeships
                   && (progType == ProgTypes.TLevel || progType == ProgTypes.TLevelTransition);
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