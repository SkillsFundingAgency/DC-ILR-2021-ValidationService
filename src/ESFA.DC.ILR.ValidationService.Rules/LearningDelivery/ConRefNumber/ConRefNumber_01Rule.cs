using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.ConRefNumber
{
    public class ConRefNumber_01Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IFCSDataService _fcsDataService;

        public ConRefNumber_01Rule(IFCSDataService fcsDataService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.ConRefNumber_01)
        {
            _fcsDataService = fcsDataService;
        }

        public ConRefNumber_01Rule()
            : base(null, RuleNameConstants.ConRefNumber_01)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.FundModel, learningDelivery.ConRefNumber))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.FundModel, learningDelivery.ConRefNumber));
                }
            }
        }

        public bool ConditionMet(int fundModel, string conRefNumber)
        {
            return FundModelConditionMet(fundModel) && ConRefNumberConditionMet(conRefNumber);
        }

        public virtual bool FundModelConditionMet(int fundModel)
        {
            return fundModel == FundModels.EuropeanSocialFund;
        }

        public virtual bool ConRefNumberConditionMet(string conRefNumber)
        {
            return string.IsNullOrWhiteSpace(conRefNumber)
                   || !_fcsDataService.ConRefNumberExists(conRefNumber);
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int fundModel, string conRefNumber)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, fundModel),
                BuildErrorMessageParameter(PropertyNameConstants.ConRefNumber, conRefNumber),
            };
        }
    }
}
