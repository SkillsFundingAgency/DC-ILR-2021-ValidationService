using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_77Rule : AbstractRule, IRule<ILearner>
    {
        private const int MaxOccurences = 6;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public LearnDelFAMType_77Rule(
            IValidationErrorHandler validationErrorHandler, ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_77)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner learner)
        {
            if (learner?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in learner.LearningDeliveries)
            {
                var famCount = _learningDeliveryFAMQueryService.GetLearningDeliveryFAMsCountByFAMType(learningDelivery.LearningDeliveryFAMs, LearningDeliveryFAMTypeConstants.DAM);

                if (famCount > MaxOccurences)
                {
                    RaiseValidationMessage(learner.LearnRefNumber, learningDelivery);
                }
            }
        }

        private void RaiseValidationMessage(string learnRefNum, ILearningDelivery learningDelivery)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.DAM)
            };

            HandleValidationError(learnRefNum, learningDelivery.AimSeqNumber, parameters);
        }
    }
}
