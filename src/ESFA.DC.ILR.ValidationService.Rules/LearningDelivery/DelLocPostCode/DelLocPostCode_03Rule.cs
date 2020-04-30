using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.DelLocPostCode
{
    public class DelLocPostCode_03Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IPostcodesDataService _postcodesDataService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFamQueryService;

        public DelLocPostCode_03Rule(IPostcodesDataService postcodesDataService, ILearningDeliveryFAMQueryService learningDeliveryFamQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.DelLocPostCode_03)
        {
            _postcodesDataService = postcodesDataService;
            _learningDeliveryFamQueryService = learningDeliveryFamQueryService;
        }

        public DelLocPostCode_03Rule()
            : base(null, RuleNameConstants.DelLocPostCode_03)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.FundModel, learningDelivery.DelLocPostCode, learningDelivery.LearningDeliveryFAMs))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.DelLocPostCode));
                }
            }
        }

        public bool ConditionMet(int fundModel, string delLocPostCode, IEnumerable<ILearningDeliveryFAM> learningDeliveryFams)
        {
            return FundModelConditionMet(fundModel)
                && PostcodeConditionMet(delLocPostCode)
                && LearningDeliveryFAMConditionMet(learningDeliveryFams);
        }

        public virtual bool PostcodeConditionMet(string delLocPostCode)
        {
            return delLocPostCode != ValidationConstants.TemporaryPostCode &&
                   !_postcodesDataService.PostcodeExists(delLocPostCode);
        }

        public virtual bool FundModelConditionMet(int fundModel)
        {
            return fundModel != FundModels.EuropeanSocialFund;
        }

        public virtual bool LearningDeliveryFAMConditionMet(IEnumerable<ILearningDeliveryFAM> learningDeliveryFams)
        {
            return !_learningDeliveryFamQueryService.HasLearningDeliveryFAMCodeForType(learningDeliveryFams, LearningDeliveryFAMTypeConstants.LDM, "034");
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(string delLocPostCode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.DelLocPostCode, delLocPostCode)
            };
        }
    }
}
