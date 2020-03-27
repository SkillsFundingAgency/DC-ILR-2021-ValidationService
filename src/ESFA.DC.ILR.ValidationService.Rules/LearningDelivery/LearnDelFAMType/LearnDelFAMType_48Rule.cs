using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_48Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        private readonly string[] validFamCodes = { "1", "3" };

        public LearnDelFAMType_48Rule(
            IValidationErrorHandler validationErrorHandler,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_48)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries.Where(d => d.LearningDeliveryFAMs != null))
            {
                foreach (var learnDelFam in learningDelivery.LearningDeliveryFAMs)
                {
                    if (ConditionMet(learnDelFam, learningDelivery.LearningDeliveryFAMs))
                    {
                        HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learnDelFam));
                    }
                }
            }
        }

        public bool ConditionMet(ILearningDeliveryFAM learningDeliveryFam, IReadOnlyCollection<ILearningDeliveryFAM> learningDeliveryFaMs)
        {
            var learningDeliveryFaMsHhsCount = _learningDeliveryFAMQueryService.GetLearningDeliveryFAMsCountByFAMType(learningDeliveryFaMs, LearningDeliveryFAMTypeConstants.HHS);

            if (learningDeliveryFam?.LearnDelFAMType == LearningDeliveryFAMTypeConstants.HHS &&
                learningDeliveryFaMsHhsCount == 2 &&
                !validFamCodes.Contains(learningDeliveryFam.LearnDelFAMCode))
            {
                return true;
            }

            return false;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(ILearningDeliveryFAM learnDelFam)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, learnDelFam.LearnDelFAMType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, learnDelFam.LearnDelFAMCode)
            };
        }
    }
}
