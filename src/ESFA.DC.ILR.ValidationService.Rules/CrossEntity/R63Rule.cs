using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R63Rule : AbstractRule, IRule<ILearner>
    {
        public R63Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R63)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            bool bFundModelExists = false;
            bool bAimTypeExists = false;
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (FundModelConditionMet(learningDelivery.FundModel))
                {
                    bFundModelExists = true;
                    if (AimTypeConditionMet(learningDelivery.AimType))
                    {
                        bAimTypeExists = true;
                        break;
                    }
                }
            }

            if (bFundModelExists
                && !bAimTypeExists)
            {
                HandleValidationError(
                            learnRefNumber: objectToValidate.LearnRefNumber,
                            errorMessageParameters: BuildErrorMessageParameters(
                                TypeOfFunding.Age16To19ExcludingApprenticeships,
                                TypeOfAim.CoreAim16To19ExcludingApprenticeships));
            }
        }

        public bool FundModelConditionMet(int fundModel) => fundModel == TypeOfFunding.Age16To19ExcludingApprenticeships;

        public bool AimTypeConditionMet(int aimType) => aimType == TypeOfAim.CoreAim16To19ExcludingApprenticeships;

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int fundModel, int aimType)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.AimType, aimType)
            };
        }
    }
}
