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
    /// <summary>
    /// the delivery location postcode rule 18 implementation
    /// </summary>
    /// <seealso cref="DelLocPostCode.DelLocPostCode_EligibilityRule{IEsfEligibilityRuleLocalEnterprisePartnership}" />
    public class DelLocPostCode_18Rule :
        DelLocPostCode_EligibilityRule<IEsfEligibilityRuleLocalEnterprisePartnership>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DelLocPostCode_18Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="commonChecks">The common checks.</param>
        /// <param name="fcsData">The FCS data.</param>
        /// <param name="postcodesData">The postcodes data.</param>
        /// <param name="derivedData22">The derived data22.</param>
        public DelLocPostCode_18Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideRuleCommonOperations commonChecks,
            IFCSDataService fcsData,
            IPostcodesDataService postcodesData)
            : base(validationErrorHandler, commonChecks, fcsData, postcodesData, RuleNameConstants.DelLocPostCode_18)
        {
        }

        /// <summary>
        /// Gets the eligibility items for...
        /// </summary>
        /// <param name="delivery">This delivery.</param>
        /// <returns>the enterprise partnership (if found)</returns>
        public override IReadOnlyCollection<IEsfEligibilityRuleLocalEnterprisePartnership> GetEligibilityItemsFor(ILearningDelivery delivery) =>
            FcsData.GetEligibilityRuleEnterprisePartnershipsFor(delivery?.ConRefNumber).AsSafeReadOnlyList();

        /// <summary>
        /// Determines whether [has qualifying eligibility] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="postcodes">The postcodes.</param>
        /// <param name="eligibilityCodes">The eligibility codes.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying eligibility] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public override bool HasAnyQualifyingEligibility(ILearningDelivery delivery, IReadOnlyCollection<IONSPostcode> postcodes, IContainThis<string> eligibilityCodes) =>
            postcodes.Any(x => (ContainsCode(eligibilityCodes, x.Lep1) || ContainsCode(eligibilityCodes, x.Lep2)) && InQualifyingPeriod(delivery, x));

        /// <summary>
        /// Determines whether the specified eligibility codes contains code.
        /// </summary>
        /// <param name="eligibilityCodes">The eligibility codes.</param>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if the specified eligibility codes contains code; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsCode(IContainThis<string> eligibilityCodes, string candidate) =>
            eligibilityCodes.Contains(candidate);
    }
}
