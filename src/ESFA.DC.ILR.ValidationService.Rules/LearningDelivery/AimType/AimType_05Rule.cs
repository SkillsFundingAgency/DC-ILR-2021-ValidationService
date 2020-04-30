using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.AimType
{
    public class AimType_05Rule : AbstractRule, IRule<ILearner>
    {
        public AimType_05Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.AimType_05)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.AimType, learningDelivery.FundModel))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.AimType, learningDelivery.FundModel));
                }
            }
        }

        public bool ConditionMet(int aimType, int fundModel)
        {
            return aimType == 5 && fundModel != FundModels.Age16To19ExcludingApprenticeships && fundModel != FundModels.Other16To19;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int aimType, int fundModel)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.AimType, aimType),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel)
            };
        }
    }
}
