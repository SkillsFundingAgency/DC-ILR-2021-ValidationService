using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.DelLocPostCode
{
    public class DelLocPostCode_17Rule : AbstractRule, IRule<ILearner>
    {
        private const int FundModel = TypeOfFunding.EuropeanSocialFund;

        private readonly IFCSDataService _fcsDataService;
        private readonly IPostcodesDataService _postcodeService;
        private readonly IDerivedData_22Rule _derivedData22;

        private readonly DateTime _ruleEndDate = new DateTime(2017, 8, 1);

        public DelLocPostCode_17Rule(
            IFCSDataService fcsDataService,
            IPostcodesDataService postcodeService,
            IDerivedData_22Rule derivedData22,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.DelLocPostCode_17)
        {
            _fcsDataService = fcsDataService;
            _postcodeService = postcodeService;
            _derivedData22 = derivedData22;
        }

        public void Validate(ILearner learner)
        {
            if (learner?.LearningDeliveries == null)
            {
                return;
            }

            foreach (var learningDelivery in learner.LearningDeliveries)
            {

                var latestLearningStart = _derivedData22.GetLatestLearningStartForESFContract(learningDelivery, learner.LearningDeliveries);

                var localAuthorities = _fcsDataService.GetEligibilityRuleLocalAuthoritiesFor(learningDelivery.ConRefNumber);

                if (!localAuthorities.Any())
                {
                    break;
                }

                var allOnsPostCodes = _postcodeService.GetONSPostcodes(learningDelivery.DelLocPostCode);
                var onsPostcodesMatchinglocalAuthorities = allOnsPostCodes.Where(pc => localAuthorities.Any(la => la.Code.CaseInsensitiveEquals(pc.LocalAuthority)));

                if (ConditionMetDD22Exists(latestLearningStart)
                    && ConditionMetStartDate(learningDelivery.LearnStartDate)
                    && ConditionMetFundModel(learningDelivery.FundModel)
                    && (ConditionMetONSPostcode(latestLearningStart, onsPostcodesMatchinglocalAuthorities)
                        || ConditionMetLocalAuthority(localAuthorities, allOnsPostCodes)
                    ))
                {
                    HandleValidationError(
                        learner.LearnRefNumber,
                        learningDelivery.AimSeqNumber,
                        BuildErrorMessageParameters(learningDelivery));
                }
            }
        }

        public bool ConditionMetDD22Exists(DateTime? latestLearningStart) =>
            latestLearningStart.HasValue;

        public bool ConditionMetStartDate(DateTime learnStartDate) =>
            learnStartDate >= _ruleEndDate;

        public bool ConditionMetFundModel(int fundModel) =>
            fundModel == FundModel;

        public bool ConditionMetONSPostcode(DateTime? latestLearningStart, IEnumerable<IONSPostcode> onsPostcodes) =>
            onsPostcodes != null
                   && !onsPostcodes.Any(vp => latestLearningStart >= vp.EffectiveFrom
                                             && latestLearningStart <= (vp.EffectiveTo ?? DateTime.MaxValue)
                                             && latestLearningStart < (vp.Termination ?? DateTime.MaxValue));

        public bool ConditionMetLocalAuthority(IEnumerable<IEsfEligibilityRuleLocalAuthority> eligibilityRulesLocalAuthorities, IEnumerable<IONSPostcode> onsPostcodes) =>
            eligibilityRulesLocalAuthorities != null
                && (onsPostcodes == null
                    || !eligibilityRulesLocalAuthorities.Join(onsPostcodes, eli => eli.Code.ToUpper(), pc => pc.LocalAuthority.ToUpper(), (eli, pc) => pc).Any());

        private IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(ILearningDelivery learningDelivery)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.ConRefNumber, learningDelivery.ConRefNumber)
            };
        }
    }
}
