using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_76Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFamQueryService;

        public LearnDelFAMType_76Rule(
            ILearningDeliveryFAMQueryService learningDeliveryFamQueryService,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_76)
        {
            _learningDeliveryFamQueryService = learningDeliveryFamQueryService;
        }

        public void Validate(ILearner objectToValidate)
       {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries.Where(d => d.LearningDeliveryFAMs != null))
            {
                if (LearnDelFamTypeLDMConditionMet(learningDelivery.LearningDeliveryFAMs))
                {
                    foreach (var learningDeliveryFam in learningDelivery.LearningDeliveryFAMs)
                    {
                        if (LearnDelFamTypeSOFConditionMet(learningDeliveryFam))
                        {
                            HandleValidationError(
                                objectToValidate.LearnRefNumber,
                                learningDelivery.AimSeqNumber,
                                BuildErrorMessageParameters(learningDeliveryFam.LearnDelFAMType, learningDeliveryFam.LearnDelFAMCode));
                        }
                    }
                }
            }
        }

        public bool LearnDelFamTypeLDMConditionMet(IReadOnlyCollection<ILearningDeliveryFAM> learningDeliveryFams)
        {
            return _learningDeliveryFamQueryService.HasLearningDeliveryFAMCodeForType(
                learningDeliveryFams,
                LearningDeliveryFAMTypeConstants.LDM,
                LearningDeliveryFAMCodeConstants.LDM_OLASS);
        }

        public bool LearnDelFamTypeSOFConditionMet(ILearningDeliveryFAM learningDeliveryFam)
        {
            return
                learningDeliveryFam.LearnDelFAMType.CaseInsensitiveEquals(LearningDeliveryFAMTypeConstants.SOF) &&
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
