using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.HE.GROSSFEE
{
    public class GROSSFEE_01Rule : AbstractRule, IRule<ILearner>
    {
        public GROSSFEE_01Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.GROSSFEE_01)
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
                if (learningDelivery.LearningDeliveryHEEntity != null
                    && ConditionMet(
                        learningDelivery.LearningDeliveryHEEntity.NETFEENullable,
                        learningDelivery.LearningDeliveryHEEntity.GROSSFEENullable))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(
                            learningDelivery.LearningDeliveryHEEntity?.GROSSFEENullable));
                }
            }
        }

        public bool ConditionMet(int? nETFEENullable, int? gROSSFEENullable) =>
            nETFEENullable.HasValue && !gROSSFEENullable.HasValue;

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int? gROSSFEENullable)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.GrossFee, gROSSFEENullable)
            };
        }
    }
}
