using ESFA.DC.ILR.Model.Interface;
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
        /// <summary>
        /// The common rule (operations provider)
        /// </summary>
        private readonly IProvideRuleCommonOperations _check;

        /// <summary>
        /// The postcodes data (service)
        /// </summary>
        private readonly IPostcodesDataService _postcodesData;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelLocPostCode_EligibilityRule{TEligibilityItem}" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="commonChecks">The common checks.</param>
        /// <param name="fcsData">The FCS data.</param>
        /// <param name="postcodesData">The postcodes data.</param>
        /// <param name="ruleName">Name of the rule.</param>
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

        /// <summary>
        /// Gets the first viable date.
        /// </summary>
        public static DateTime FirstViableDate => new DateTime(2017, 08, 01);

        /// <summary>
        /// Gets the FCS data (service)
        /// </summary>
        protected IFCSDataService FcsData { get; }

        /// <summary>
        /// Gets the qualifyingd aim.
        /// </summary>
        /// <param name="usingSources">The using sources.</param>
        /// <returns>the latest completed ZESF0001 aim</returns>
        public ILearningDelivery GetQualifyingAim(IReadOnlyCollection<ILearningDelivery> usingSources) =>
            usingSources
                .SafeWhere(x => x.FundModel == TypeOfFunding.EuropeanSocialFund
                    && x.LearnAimRef == TypeOfAim.References.ESFLearnerStartandAssessment
                    && x.CompStatus == CompletionState.HasCompleted)
                .OrderByDescending(x => x.LearnStartDate)
                .FirstOrDefault();

        /// <summary>
        /// Gets the (esf eligibility rule) enterprise partnership.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>the enterprise partnership (if found)</returns>
        public abstract IReadOnlyCollection<TEligibilityItem> GetEligibilityItemsFor(ILearningDelivery delivery);

        /// <summary>
        /// Gets the ons postcode.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>the ons postcodes (if found)</returns>
        public IReadOnlyCollection<IONSPostcode> GetONSPostcodes(ILearningDelivery delivery) =>
            _postcodesData.GetONSPostcodes(delivery.DelLocPostCode);

        /// <summary>
        /// Determines whether [has qualifying eligibility] [the specified postcode].
        /// </summary>
        /// <param name="delivery">The latest learnstartdate delivery.</param>
        /// <param name="postcodes">The postcode.</param>
        /// <param name="eligibilities">The eligibilities.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying eligibility] [the specified postcode]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingEligibility(ILearningDelivery delivery, IReadOnlyCollection<IONSPostcode> postcodes, IReadOnlyCollection<TEligibilityItem> eligibilities) =>
            It.HasValues(postcodes)
            && It.HasValues(eligibilities)
            && It.Has(delivery)
            && HasAnyQualifyingEligibility(delivery, postcodes, eligibilities.Select(x => x.Code).AsSafeDistinctKeySet());

        /// <summary>
        /// Determines whether [has any qualifying eligibility] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="postcodes">The postcodes.</param>
        /// <param name="eligibilityCodes">The eligibility codes.</param>
        /// <returns>
        ///   <c>true</c> if [has any qualifying eligibility] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool HasAnyQualifyingEligibility(ILearningDelivery delivery, IReadOnlyCollection<IONSPostcode> postcodes, IContainThis<string> eligibilityCodes);

        /// <summary>
        /// Determines whether [in qualifying period] [the specified delivery].
        /// TODO: needs to cater for 'termination' but this is currently a string of indeterminate quality
        /// It.IsBetween(delivery.LearnStartDate, onsPostcode.EffectiveFrom, onsPostcode.EffectiveTo ?? onsPostcode.Termination ?? DateTime.MaxValue);
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="onsPostcode">The ons postcode.</param>
        /// <returns>
        ///   <c>true</c> if [in qualifying period] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool InQualifyingPeriod(ILearningDelivery delivery, IONSPostcode onsPostcode) =>
            It.IsBetween(delivery.LearnStartDate, onsPostcode.EffectiveFrom, onsPostcode.Termination ?? onsPostcode.EffectiveTo ?? DateTime.MaxValue);

        /// <summary>
        /// Determines whether [is not valid] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearningDelivery delivery)
        {
            var eligibilities = GetEligibilityItemsFor(delivery);

            return _check.HasQualifyingStart(delivery, FirstViableDate)
                   && eligibilities.Any(x => !string.IsNullOrEmpty(x.Code))
                   && !HasQualifyingEligibility(delivery, GetONSPostcodes(delivery), eligibilities);
        }

        /// <summary>
        /// Validates this learner.
        /// </summary>
        /// <param name="thisLearner">this learner.</param>
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

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="thisDelivery">this delivery.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            HandleValidationError(learnRefNumber, null, BuildMessageParametersFor(thisDelivery));
        }

        /// <summary>
        /// Builds the message parameters for..
        /// </summary>
        /// <param name="thisDelivery">The this delivery.</param>
        /// <returns>
        /// returns a list of message parameters
        /// </returns>
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
