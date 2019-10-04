using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_76Rule : AbstractRule, IRule<ILearner>
    {
        public LearnDelFAMType_76Rule(
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_76)
        {
        }

        public void Validate(ILearner objectToValidate)
       {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries.Where(d => d.LearningDeliveryFAMs != null))
            {
                foreach (var learningDeliveryFam in learningDelivery.LearningDeliveryFAMs)
                {
                    if (ConditionMet(learningDeliveryFam))
                    {
                        HandleValidationError(
                            objectToValidate.LearnRefNumber,
                            learningDelivery.AimSeqNumber,
                            BuildErrorMessageParameters(LearningDeliveryFAMTypeConstants.LDM, learningDeliveryFam.LearnDelFAMCode));
                    }
                }
                
            }
        }

        public bool ConditionMet(ILearningDeliveryFAM learningDeliveryFam)
        {
            return learningDeliveryFam.LearnDelFAMType.CaseInsensitiveEquals(LearningDeliveryFAMTypeConstants.SOF) &&
                   !learningDeliveryFam.LearnDelFAMCode.CaseInsensitiveEquals(LearningDeliveryFAMCodeConstants.SOF_ESFA_Adult);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string learnDelFamType, string learnDelFamCode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, learnDelFamType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, learnDelFamCode)
            };
        }
    }
}
