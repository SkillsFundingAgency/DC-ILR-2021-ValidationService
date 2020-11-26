using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_91Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFamQueryService;

        public LearnDelFAMType_91Rule(IValidationErrorHandler validationErrorHandler, ILearningDeliveryFAMQueryService learningDeliveryFamQueryService)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_91)
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
                if (learningDelivery.LearningDeliveryFAMs == null)
                {
                    continue;
                }

                if (ConditionMet(learningDelivery.FundModel, learningDelivery.LearningDeliveryFAMs))
                {
                    HandleValidationError(learner.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters());
                }
            }
        }

        public bool ConditionMet(int fundModel, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return FundModelConditionMet(fundModel) && LearningDeliveryFAMConditionMet(learningDeliveryFAMs);
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return fundModel == FundModels.NotFundedByESFA;
        }

        public bool LearningDeliveryFAMConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return _learningDeliveryFamQueryService.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.ADL)
                   && _learningDeliveryFamQueryService.HasLearningDeliveryFAMType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.SOF);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters()
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, FundModels.NotFundedByESFA),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, LearningDeliveryFAMTypeConstants.SOF),
            };
        }
    }
}
