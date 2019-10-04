using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_75Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IDerivedData_35Rule _dd35;
        private readonly IEnumerable<string> _famCodes = new HashSet<string>()
        {
            LearningDeliveryFAMCodeConstants.LDM_OLASS,
            LearningDeliveryFAMCodeConstants.LDM_ProcuredAdultEducationBudget,
            LearningDeliveryFAMCodeConstants.LDM_LowWages
        };

        public LearnDelFAMType_75Rule(
            IValidationErrorHandler validationErrorHandler,
            IDerivedData_35Rule dd35)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_75)
        {
            _dd35 = dd35;
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
                    if (ConditionMet(learningDelivery, learningDeliveryFam))
                    {
                        HandleValidationError(
                            objectToValidate.LearnRefNumber,
                            learningDelivery.AimSeqNumber,
                            BuildErrorMessageParameters(LearningDeliveryFAMTypeConstants.LDM, learningDeliveryFam.LearnDelFAMCode));
                    }
                }
                
            }
        }

        public bool ConditionMet(ILearningDelivery learningDelivery, ILearningDeliveryFAM learningDeliveryFam)
        {
            return DD35ConditionMet(learningDelivery) && LearningDeliveryFamConditionMet(learningDeliveryFam);
        }

        public bool LearningDeliveryFamConditionMet(ILearningDeliveryFAM learningDeliveryFam)
        {
            return learningDeliveryFam.LearnDelFAMType.CaseInsensitiveEquals(LearningDeliveryFAMTypeConstants.LDM) &&
                _famCodes.Contains(learningDeliveryFam.LearnDelFAMCode);

        }

        public virtual bool DD35ConditionMet(ILearningDelivery learningDelivery)
        {
            return _dd35.IsCombinedAuthorities(learningDelivery);
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
