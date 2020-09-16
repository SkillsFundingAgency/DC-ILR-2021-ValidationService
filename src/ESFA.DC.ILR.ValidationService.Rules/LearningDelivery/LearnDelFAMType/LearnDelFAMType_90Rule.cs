using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_90Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<string> _famCodes = new HashSet<string> { LearningDeliveryFAMCodeConstants.SOF_ESFA_Adult, LearningDeliveryFAMCodeConstants.SOF_ESFA_1619 };
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFamQueryService;

        public LearnDelFAMType_90Rule(IValidationErrorHandler validationErrorHandler, ILearningDeliveryFAMQueryService learningDeliveryFamQueryService)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_90)
        {
            _learningDeliveryFamQueryService = learningDeliveryFamQueryService;
        }

        public void Validate(ILearner learner)
        {
            if (learner?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in learner.LearningDeliveries)
            {
                if (ProgTypeConditionMet(learningDelivery.ProgTypeNullable))
                {
                    if (learningDelivery.LearningDeliveryFAMs == null)
                    {
                        continue;
                    }

                    foreach (var deliveryFam in _learningDeliveryFamQueryService.GetLearningDeliveryFAMsForType(learningDelivery.LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.SOF))
                    {
                        if (!_famCodes.Contains(deliveryFam.LearnDelFAMCode))
                        {
                            HandleValidationError(learner.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.ProgTypeNullable, deliveryFam.LearnDelFAMCode));
                        }
                    }
                }
            }
        }

        public bool ProgTypeConditionMet(int? progType)
        {
            return progType == ProgTypes.Traineeship;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int? progType, string famCode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, progType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.SOF),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, famCode)
            };
        }
    }
}
