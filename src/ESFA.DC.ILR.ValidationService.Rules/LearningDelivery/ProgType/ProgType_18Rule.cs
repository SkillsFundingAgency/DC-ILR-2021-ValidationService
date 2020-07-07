using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.ProgType
{
    public class ProgType_18Rule : AbstractRule, IRule<ILearner>
    {
        public ProgType_18Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.ProgType_18)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearningDeliveries == null)
            {
                return;
            }

            var aim = objectToValidate.LearningDeliveries.FirstOrDefault(l => l.ProgTypeNullable.GetValueOrDefault() == ProgTypes.TLevelTransition);

            if (aim != null)
            {
                if (!objectToValidate.LearningDeliveries.Any(l =>
                    l.AimType == AimTypes.CoreAim16To19ExcludingApprenticeships &&
                    l.LearnAimRef.CaseInsensitiveStartsWith(AimTypes.References.TLevelTransitionAimClassCode)))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, aim.AimSeqNumber, BuildErrorMessageParameters(aim.LearnAimRef, aim.ProgTypeNullable));
                }
            }
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
