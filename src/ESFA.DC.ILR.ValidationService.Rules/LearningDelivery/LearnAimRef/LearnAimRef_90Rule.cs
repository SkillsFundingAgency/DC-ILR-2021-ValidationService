using System;
using System.Collections.Generic;
using System.Text;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnAimRef
{
    public class LearnAimRef_90Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<string> learningAimRefs = new HashSet<string>
        {
            AimTypes.References.WorkPlacement0To49Hours,
            AimTypes.References.WorkPlacement50To99Hours,
            AimTypes.References.WorkPlacement100To199Hours,
            AimTypes.References.WorkPlacement200To499Hours,
            AimTypes.References.WorkPlacement500PlusHours,
            AimTypes.References.SupportedInternship16To19,
            AimTypes.References.WorkExperience,
            AimTypes.References.IndustryPlacement
        }.ToCaseInsensitiveHashSet();

        public LearnAimRef_90Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnAimRef_90)
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
                if (ConditionMet(learningDelivery.LearnAimRef, learningDelivery.ProgTypeNullable))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.LearnAimRef, learningDelivery.ProgTypeNullable));
                }
            }
        }

        public bool ConditionMet(string learnAimRef, int? progType)
        {
            return progType != null
                   && progType == ProgTypes.TLevel
                   && learningAimRefs.Contains(learnAimRef);
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
