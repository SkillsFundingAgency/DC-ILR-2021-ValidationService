using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_08Rule : AbstractRule, IRule<ILearner>
    {
        private readonly int _fundModels = 99;
        private readonly IEnumerable<string> _learnDelFAMCodes = new HashSet<string>() { "105", "107" };

        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public LearnDelFAMType_08Rule(
            IValidationErrorHandler validationErrorHandler,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_08)
        {
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.FundModel, learningDelivery.LearningDeliveryFAMs))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.FundModel, LearningDeliveryFAMTypeConstants.SOF, "105,107"));
                }
            }
        }

        public bool ConditionMet(int fundModel, IReadOnlyCollection<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return FundModelConditionMet(fundModel)
                && LearningDeliveryFAMsConditionMet(learningDeliveryFAMs);
        }

        public bool LearningDeliveryFAMsConditionMet(IReadOnlyCollection<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return _learningDeliveryFAMQueryService.HasAnyLearningDeliveryFAMCodesForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.SOF, _learnDelFAMCodes);
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return fundModel == _fundModels;
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int fundModel, string learnDelFAMType, string learnDelFAMCode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, learnDelFAMType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, learnDelFAMCode)
            };
        }
    }
}
