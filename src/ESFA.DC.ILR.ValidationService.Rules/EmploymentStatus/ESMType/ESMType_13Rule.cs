using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.ESMType
{
    public class ESMType_13Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IFCSDataService _fcsData;
        private readonly IDerivedData_25Rule _ddrule25;

        public ESMType_13Rule(
            IValidationErrorHandler validationErrorHandler,
            IDerivedData_25Rule ddrule25,
            IFCSDataService fcsData)
            : base(validationErrorHandler, RuleNameConstants.ESMType_13)
        {
            _ddrule25 = ddrule25;
            _fcsData = fcsData;
        }

        public IEsfEligibilityRule GetEligibilityRuleFor(ILearningDelivery thisDelivery) =>
            _fcsData.GetEligibilityRuleFor(thisDelivery.ConRefNumber);

        public int? GetDerivedRuleLOUIndicatorFor(ILearner thisLearner, ILearningDelivery thisDelivery) =>
            _ddrule25.GetLengthOfUnemployment(thisLearner, thisDelivery.ConRefNumber);

        public bool HasDisqualifyingMinLOUIndicator(IEsfEligibilityRule eligibility, int derivedRuleResult) =>
            eligibility.MinLengthOfUnemployment.HasValue
            && derivedRuleResult < eligibility.MinLengthOfUnemployment;

        public bool HasDisqualifyingMaxLOUIndicator(IEsfEligibilityRule eligibility, int derivedRuleResult) =>
            eligibility.MaxLengthOfUnemployment.HasValue
            && derivedRuleResult > eligibility.MaxLengthOfUnemployment;

        public bool HasDisqualifyingLOUIndicator(IEsfEligibilityRule eligibility, int? derivedRuleResult) =>
            eligibility != null
            && derivedRuleResult != null
            && (HasDisqualifyingMinLOUIndicator(eligibility, derivedRuleResult.Value)
                || HasDisqualifyingMaxLOUIndicator(eligibility, derivedRuleResult.Value));

        public bool HasDisqualifyingLOUIndicator(ILearningDelivery thisDelivery, Func<int?> derivedRuleAction) =>
            HasDisqualifyingLOUIndicator(GetEligibilityRuleFor(thisDelivery), derivedRuleAction());

        public bool IsNotValid(ILearningDelivery thisDelivery, Func<int?> derivedRuleAction) =>
            thisDelivery.FundModel == FundModels.EuropeanSocialFund
                && HasDisqualifyingLOUIndicator(thisDelivery, derivedRuleAction);

        public void Validate(ILearner thisLearner)
        {
            var learnRefNumber = thisLearner.LearnRefNumber;

            thisLearner.LearningDeliveries
                .ForAny(x => IsNotValid(x, () => GetDerivedRuleLOUIndicatorFor(thisLearner, x)), x => RaiseValidationMessage(learnRefNumber, x));
        }

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            HandleValidationError(learnRefNumber, null, BuildMessageParametersFor(thisDelivery));
        }

        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery thisDelivery)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.ConRefNumber, thisDelivery.ConRefNumber)
            };
        }
    }
}