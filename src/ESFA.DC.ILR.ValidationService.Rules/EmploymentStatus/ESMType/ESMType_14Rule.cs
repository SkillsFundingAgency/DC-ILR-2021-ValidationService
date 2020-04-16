using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.ESMType
{
    public class ESMType_14Rule :
        AbstractRule,
        IRule<ILearner>
    {
        private readonly IProvideRuleCommonOperations _check;

        private readonly IFCSDataService _fcsData;

        private readonly IDerivedData_26Rule _ddrule26;

        public ESMType_14Rule(
            IValidationErrorHandler validationErrorHandler,
            IDerivedData_26Rule ddrule26,
            IFCSDataService fcsData,
            IProvideRuleCommonOperations commonOperations)
            : base(validationErrorHandler, RuleNameConstants.ESMType_14)
        {
            _ddrule26 = ddrule26;
            _fcsData = fcsData;
            _check = commonOperations;
        }

        public IEsfEligibilityRule GetEligibilityRuleFor(ILearningDelivery thisDelivery) =>
            _fcsData.GetEligibilityRuleFor(thisDelivery.ConRefNumber);

        public bool GetDerivedRuleBenefitsIndicatorFor(ILearner thisLearner, ILearningDelivery thisDelivery) =>
            _ddrule26.LearnerOnBenefitsAtStartOfCompletedZESF0001AimForContract(thisLearner, thisDelivery.ConRefNumber);

        public bool HasMatchingBenefitsIndicator(IEsfEligibilityRule eligibility, bool derivedRuleResult) =>
            eligibility == null || eligibility.Benefits == null || eligibility.Benefits == derivedRuleResult;

        public bool HasMatchingBenefitsIndicator(ILearningDelivery thisDelivery, Func<bool> derivedRuleAction) =>
            HasMatchingBenefitsIndicator(GetEligibilityRuleFor(thisDelivery), derivedRuleAction());

        public bool IsNotValid(ILearningDelivery thisDelivery, Func<bool> derivedRuleAction) =>
            _check.HasQualifyingFunding(thisDelivery, TypeOfFunding.EuropeanSocialFund)
                && !HasMatchingBenefitsIndicator(thisDelivery, derivedRuleAction);

        public void Validate(ILearner thisLearner)
        {
            var learnRefNumber = thisLearner.LearnRefNumber;

            thisLearner.LearningDeliveries
                .ForAny(x => IsNotValid(x, () => GetDerivedRuleBenefitsIndicatorFor(thisLearner, x)), x => RaiseValidationMessage(learnRefNumber, x));
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