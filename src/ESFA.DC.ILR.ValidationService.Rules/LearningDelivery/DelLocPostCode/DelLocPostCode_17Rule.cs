using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.DelLocPostCode
{
    public class DelLocPostCode_17Rule :
        DelLocPostCode_EligibilityRule<IEsfEligibilityRuleLocalAuthority>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DelLocPostCode_17Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="commonChecks">The common checks.</param>
        /// <param name="fcsData">The FCS data.</param>
        /// <param name="postcodesData">The postcodes data.</param>
        /// <param name="derivedData22">The derived data22.</param>
        public DelLocPostCode_17Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideRuleCommonOperations commonChecks,
            IFCSDataService fcsData,
            IPostcodesDataService postcodesData)
            : base(validationErrorHandler, commonChecks, fcsData, postcodesData, RuleNameConstants.DelLocPostCode_17)
        {
        }

        /// <summary>
        /// Gets the eligibility item.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>the enterprise partnership (if found)</returns>
        public override IReadOnlyCollection<IEsfEligibilityRuleLocalAuthority> GetEligibilityItemsFor(ILearningDelivery delivery) =>
            FcsData.GetEligibilityRuleLocalAuthoritiesFor(delivery.ConRefNumber).AsSafeReadOnlyList();

        /// <summary>
        /// Determines whether [has any qualifying eligibility] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="postcodes">The postcodes.</param>
        /// <param name="eligibilityCodes">The eligibility codes.</param>
        /// <returns>
        ///   <c>true</c> if [has any qualifying eligibility] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public override bool HasAnyQualifyingEligibility(ILearningDelivery delivery, IReadOnlyCollection<IONSPostcode> postcodes, IContainThis<string> eligibilityCodes) =>
            postcodes.Any(x => eligibilityCodes.Contains(x.LocalAuthority) && InQualifyingPeriod(delivery, x));
    }
}
