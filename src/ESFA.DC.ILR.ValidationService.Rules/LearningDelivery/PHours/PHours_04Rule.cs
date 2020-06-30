using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.PHours
{
    public class PHours_04Rule : AbstractRule, IRule<ILearner>
    {
        public PHours_04Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.PHours_04)
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
                if (ConditionMet(learningDelivery.AimType, learningDelivery.FundModel, learningDelivery.ProgTypeNullable, learningDelivery.PHoursNullable))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learningDelivery.AimType, learningDelivery.FundModel, learningDelivery.ProgTypeNullable, learningDelivery.PHoursNullable));
                    return;
                }
            }
        }

        public bool ConditionMet(int aimType, int fundModel, int? progType, int? plannedHours)
        {
            return aimType == AimTypes.ProgrammeAim
                   && fundModel == FundModels.Age16To19ExcludingApprenticeships
                   && progType.HasValue && progType == ProgTypes.TLevel
                   && plannedHours.HasValue && plannedHours <= 0;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int aimType, int fundModel, int? progType, int? pHours)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AimType, aimType),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, progType),
                BuildErrorMessageParameter(PropertyNameConstants.PHours, pHours),
            };
        }
    }
}
