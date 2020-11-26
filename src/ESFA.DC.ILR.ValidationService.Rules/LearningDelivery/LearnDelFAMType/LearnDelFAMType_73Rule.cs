using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnDelFAMType
{
    public class LearnDelFAMType_73Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFamQueryService;
        private readonly IEnumerable<int> _fundModels = new HashSet<int>() { FundModels.OtherAdult, FundModels.NotFundedByESFA };

        public LearnDelFAMType_73Rule(
            IValidationErrorHandler validationErrorHandler,
            ILearningDeliveryFAMQueryService learningDeliveryFamQueryService)
            : base(validationErrorHandler, RuleNameConstants.LearnDelFAMType_73)
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
                if (ConditionMet(learningDelivery.FundModel, learningDelivery.LearningDeliveryFAMs))
                {
                    HandleValidationError(
                        objectToValidate.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learningDelivery.FundModel, LearningDeliveryFAMTypeConstants.LDM, LearningDeliveryFAMCodeConstants.LDM_CareerLearningPilot));
                }
            }
        }

        public bool ConditionMet(int fundModel, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return LearningDeliveryFAMsConditionMet(learningDeliveryFAMs)
                                            && FundModelConditionMet(fundModel);
        }

        public bool LearningDeliveryFAMsConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return _learningDeliveryFamQueryService.HasLearningDeliveryFAMCodeForType(
                                                            learningDeliveryFAMs,
                                                            LearningDeliveryFAMTypeConstants.LDM,
                                                            LearningDeliveryFAMCodeConstants.LDM_CareerLearningPilot);
        }

        public bool FundModelConditionMet(int fundModel)
        {
            return !_fundModels.Contains(fundModel);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int fundModel, string learnDelFAMType, string learnDelFamCode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, learnDelFAMType),
                BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMCode, learnDelFamCode)
            };
        }
    }
}
