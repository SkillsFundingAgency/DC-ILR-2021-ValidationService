using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.ProgType
{
    public class ProgType_14Rule : AbstractRule, IRule<ILearner>
    {
        public ProgType_14Rule(IValidationErrorHandler validationErrorHandler)
             : base(validationErrorHandler, RuleNameConstants.ProgType_14)
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
                if (ConditionMet(learningDelivery.LearnAimRef, learningDelivery.ProgTypeNullable))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learningDelivery.LearnAimRef, learningDelivery.ProgTypeNullable));
                }
            }
        }

        public bool ConditionMet(string learnAimRef, int? progType)
        {
            return progType == ProgTypes.Traineeship
                && (learnAimRef.CaseInsensitiveEquals(AimTypes.References.IndustryPlacement)
                || learnAimRef.CaseInsensitiveEquals(AimTypes.References.TLevelWorkExperience));
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string learnAimRef, int? progType)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnAimRef, learnAimRef),
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, progType)
            };
        }
    }
}
