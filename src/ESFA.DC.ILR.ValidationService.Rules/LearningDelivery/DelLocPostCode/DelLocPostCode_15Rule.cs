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
    public class DelLocPostCode_15Rule : AbstractRule, IRule<ILearner>
    {
        private const int FundModel = TypeOfFunding.EuropeanSocialFund;

        private readonly IFCSDataService _fcsDataService;
        private readonly IPostcodesDataService _postcodeService;
        private readonly IDerivedData_22Rule _derivedData22;

        private readonly DateTime _ruleEndDate = new DateTime(2017, 7, 31);

        public DelLocPostCode_15Rule(
            IFCSDataService fcsDataService,
            IPostcodesDataService postcodeService,
            IDerivedData_22Rule derivedData22,
            IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.DelLocPostCode_15)
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

                var partnerships = _fcsDataService.GetEligibilityRuleEnterprisePartnershipsFor(learningDelivery.ConRefNumber);

                if (!partnerships.Any())
                {
                    break;
                }

                var onsPostCode = _postcodeService.GetONSPostcodes(learningDelivery.DelLocPostCode);

                if (ConditionMetDD22Exists(latestLearningStart)
                    && ConditionMetStartDate(learningDelivery.LearnStartDate)
                    && ConditionMetFundModel(learningDelivery.FundModel)
                    && ConditionMetLearnAimRef(learningDelivery.LearnAimRef)
                    && ConditionMetTemporaryPostcode(learningDelivery.DelLocPostCode)
                    && (ConditionMetONSPostcode(latestLearningStart, onsPostCode)
                        || ConditionMetPartnership(partnerships, onsPostCode)))
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
            learnStartDate <= _ruleEndDate;

        public bool ConditionMetFundModel(int fundModel) =>
            fundModel == FundModel;

        public bool ConditionMetLearnAimRef(string learnAimRef) =>
            !learnAimRef.CaseInsensitiveEquals(TypeOfAim.References.ESFLearnerStartandAssessment);

        public bool ConditionMetTemporaryPostcode(string postcode) =>
            !postcode.CaseInsensitiveEquals(ValidationConstants.TemporaryPostCode);

        public bool ConditionMetONSPostcode(DateTime? latestLearningStart, IEnumerable<IONSPostcode> onsPostcodes) =>
            onsPostcodes != null
                   && !onsPostcodes.Any(vp => latestLearningStart >= vp.EffectiveFrom
                                             && latestLearningStart <= (vp.EffectiveTo ?? DateTime.MaxValue)
                                             && latestLearningStart < (vp.Termination ?? DateTime.MaxValue));

        public bool ConditionMetPartnership(IEnumerable<IEsfEligibilityRuleLocalEnterprisePartnership> eligibilityRulesPartnerships, IEnumerable<IONSPostcode> onsPostcodes) =>
            eligibilityRulesPartnerships != null
                && (onsPostcodes == null
                    || !eligibilityRulesPartnerships.Any(eli => onsPostcodes.Any(pc => pc.Lep1.CaseInsensitiveEquals(eli.Code) || pc.Lep2.CaseInsensitiveEquals(eli.Code))));

        private IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(ILearningDelivery learningDelivery)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnAimRef, learningDelivery.LearnAimRef),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, learningDelivery.FundModel),
                BuildErrorMessageParameter(PropertyNameConstants.DelLocPostCode, learningDelivery.DelLocPostCode),
                BuildErrorMessageParameter(PropertyNameConstants.ConRefNumber, learningDelivery.ConRefNumber)
            };
        }
    }
}
