using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.GROSSFEE
{
    public class GROSSFEE_03Rule : AbstractRule, IRule<ILearner>
    {
        public GROSSFEE_03Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.GROSSFEE_03)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.LearningDeliveryHEEntity?.GROSSFEENullable))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.LearningDeliveryHEEntity?.GROSSFEENullable));
                }
            }
        }

        public bool ConditionMet(int? gROSSFEENullable)
        {
            return gROSSFEENullable.HasValue
                && gROSSFEENullable > 30000;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int? gROSSFEENullable)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.GrossFee, gROSSFEENullable)
            };
        }
    }
}
