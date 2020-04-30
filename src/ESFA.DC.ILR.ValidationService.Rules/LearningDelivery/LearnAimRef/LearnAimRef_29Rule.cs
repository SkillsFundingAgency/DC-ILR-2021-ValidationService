using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_29Rule : AbstractRule, IRule<ILearner>
    {
        public LearnAimRef_29Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnAimRef_29)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.LearnAimRef, learningDelivery.FundModel))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.LearnAimRef, learningDelivery.FundModel));
                }
            }
        }

        public bool ConditionMet(string learnAimRef, int fundModel)
        {
            return learnAimRef.CaseInsensitiveEquals(ValidationConstants.ZESF0001) && fundModel != FundModels.EuropeanSocialFund;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string learnAimRef, int fundModel)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnAimRef, learnAimRef),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel)
            };
        }
    }
}
