using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.OTJActHours
{
    public class OTJActHours_01Rule : AbstractRule, IRule<ILearner>
    {
        public const int _aimType = 1;

        public OTJActHours_01Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.OTJActHours_01)
        {
        }

        public void Validate(ILearner learner)
        {
            foreach (var learningDelivery in learner.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery))
                {
                    HandleValidationError(learner.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.OtjActHoursNullable.Value, learningDelivery.AimType));
                }
            }
        }

        public bool ConditionMet(ILearningDelivery learningDelivery) =>
            HasOTJActHours(learningDelivery)
            && !HasAimType(learningDelivery);

        public bool HasOTJActHours(ILearningDelivery learningDelivery) =>
            learningDelivery.OtjActHoursNullable.HasValue;

        public bool HasAimType(ILearningDelivery learningDelivery) =>
           learningDelivery.AimType == _aimType;

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int actHours, int aimType) =>
            new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.OtjActHours, actHours),
                BuildErrorMessageParameter(PropertyNameConstants.AimType, aimType)
            };
    }
}
