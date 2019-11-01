using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_77Rule : AbstractRule, IRule<ILearner>
    {
        private const int MaxOccurences = 4;

        public LearnDelFAMType_77Rule(
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_77)
        {
        }

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="learner">The object to validate.</param>
        public void Validate(ILearner learner)
        {
            if (learner?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in learner.LearningDeliveries)
            {
                if (learningDelivery.LearningDeliveryFAMs == null)
                {
                    continue;
                }

                var count = learningDelivery.LearningDeliveryFAMs
                    .Count(ldf => ldf.LearnDelFAMType.CaseInsensitiveEquals(LearningDeliveryFAMTypeConstants.DAM));

                if (count > MaxOccurences)
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
