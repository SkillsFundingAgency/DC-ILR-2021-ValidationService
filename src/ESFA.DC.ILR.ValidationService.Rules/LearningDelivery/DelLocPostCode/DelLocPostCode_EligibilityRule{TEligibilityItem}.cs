using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.DelLocPostCode
{
    public abstract class DelLocPostCode_EligibilityRule<TEligibilityItem> :
        AbstractRule,
        IRule<ILearner>
        where TEligibilityItem : class, IEsfEligibilityRuleCode<string>
    {
        private readonly IProvideRuleCommonOperations _check;

        private readonly IPostcodesDataService _postcodesData;

        public DelLocPostCode_EligibilityRule(
            IValidationErrorHandler validationErrorHandler,
            IProvideRuleCommonOperations commonChecks,
            IFCSDataService fcsData,
            IPostcodesDataService postcodesData,
            string ruleName)
            : base(validationErrorHandler, ruleName)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(commonChecks)
                .AsGuard<ArgumentNullException>(nameof(commonChecks));
            It.IsNull(fcsData)
                .AsGuard<ArgumentNullException>(nameof(fcsData));
            It.IsNull(postcodesData)
                .AsGuard<ArgumentNullException>(nameof(postcodesData));

            _check = commonChecks;
            FcsData = fcsData;
            _postcodesData = postcodesData;
        }

        public static DateTime FirstViableDate => new DateTime(2017, 08, 01);

        protected IFCSDataService FcsData { get; }

        public ILearningDelivery GetQualifyingAim(IReadOnlyCollection<ILearningDelivery> usingSources) =>
            usingSources
                .NullSafeWhere(x => x.FundModel == TypeOfFunding.EuropeanSocialFund
                    && x.LearnAimRef == TypeOfAim.References.ESFLearnerStartandAssessment
                    && x.CompStatus == CompletionState.HasCompleted)
                .OrderByDescending(x => x.LearnStartDate)
                .FirstOrDefault();

        public abstract IReadOnlyCollection<TEligibilityItem> GetEligibilityItemsFor(ILearningDelivery delivery);

        public IReadOnlyCollection<IONSPostcode> GetONSPostcodes(ILearningDelivery delivery) =>
            _postcodesData.GetONSPostcodes(delivery.DelLocPostCode);

        public bool HasQualifyingEligibility(ILearningDelivery delivery, IReadOnlyCollection<IONSPostcode> postcodes, IReadOnlyCollection<TEligibilityItem> eligibilities) =>
            It.HasValues(postcodes)
            && It.HasValues(eligibilities)
            && It.Has(delivery)
            && HasAnyQualifyingEligibility(delivery, postcodes, eligibilities.Select(x => x.Code));

        public abstract bool HasAnyQualifyingEligibility(ILearningDelivery delivery, IReadOnlyCollection<IONSPostcode> postcodes, IEnumerable<string> eligibilityCodes);

        public bool InQualifyingPeriod(ILearningDelivery delivery, IONSPostcode onsPostcode) =>
            delivery.LearnStartDate < onsPostcode.EffectiveFrom
                || (onsPostcode.EffectiveTo.HasValue && delivery.LearnStartDate > onsPostcode.EffectiveTo)
                || (onsPostcode.Termination.HasValue && delivery.LearnStartDate >= onsPostcode.Termination);

        public bool IsNotValid(ILearningDelivery delivery)
        {
            var eligibilities = GetEligibilityItemsFor(delivery);

            return _check.HasQualifyingStart(delivery, FirstViableDate)
                   && eligibilities.Any(x => !string.IsNullOrEmpty(x.Code))
                   && HasQualifyingEligibility(delivery, GetONSPostcodes(delivery), eligibilities);
        }

        public void Validate(ILearner thisLearner)
        {
            It.IsNull(thisLearner)
                .AsGuard<ArgumentNullException>(nameof(thisLearner));

            var learnRefNumber = thisLearner.LearnRefNumber;

            var candidate = GetQualifyingAim(thisLearner.LearningDeliveries);
            if (It.IsNull(candidate))
            {
                return;
            }

            if (IsNotValid(candidate))
            {
                RaiseValidationMessage(learnRefNumber, candidate);
            }
        }

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            HandleValidationError(learnRefNumber, null, BuildMessageParametersFor(thisDelivery));
        }

        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery thisDelivery)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnAimRef, thisDelivery.LearnAimRef),
                BuildErrorMessageParameter(PropertyNameConstants.FundModel, thisDelivery.FundModel),
                BuildErrorMessageParameter(PropertyNameConstants.DelLocPostCode, thisDelivery.DelLocPostCode),
                BuildErrorMessageParameter(PropertyNameConstants.ConRefNumber, thisDelivery.ConRefNumber)
            };
        }
    }
}
