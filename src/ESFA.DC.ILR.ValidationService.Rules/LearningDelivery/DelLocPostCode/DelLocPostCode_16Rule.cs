using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.DelLocPostCode
{
    public class DelLocPostCode_16Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IPostcodesDataService _postcodesDataService;

        public DelLocPostCode_16Rule(IPostcodesDataService postcodesDataService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.DelLocPostCode_16)
        {
            _postcodesDataService = postcodesDataService;
        }

        public DelLocPostCode_16Rule()
            : base(null, null)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.FundModel, learningDelivery.DelLocPostCode))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.DelLocPostCode));
                }
            }
        }

        public bool ConditionMet(int fundModel, string delLocPostCode)
        {
            return FundModelConditionMet(fundModel)
                && PostcodeConditionMet(delLocPostCode);
        }

        public virtual bool PostcodeConditionMet(string delLocPostCode)
        {
            return delLocPostCode != ValidationConstants.TemporaryPostCode &&
                   !_postcodesDataService.PostcodeExists(delLocPostCode);
        }

        public virtual bool FundModelConditionMet(int fundModel)
        {
            return fundModel == FundModels.EuropeanSocialFund;
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
