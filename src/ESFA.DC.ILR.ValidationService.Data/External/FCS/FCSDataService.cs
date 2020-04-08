using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Data.External.FCS
{
    /// <summary>
    /// the FCS data service implementation
    /// </summary>
    /// <seealso cref="IFCSDataService" />
    public class FCSDataService :
        IFCSDataService
    {
        /// <summary>
        /// The contract allocations
        /// </summary>
        private readonly IReadOnlyDictionary<string, IFcsContractAllocation> _contractAllocations;

        /// <summary>
        /// Initializes a new instance of the <see cref="FCSDataService"/> class.
        /// </summary>
        /// <param name="externalDataCache">The external data cache.</param>
        public FCSDataService(IExternalDataCache externalDataCache)
        {
            _contractAllocations = externalDataCache.FCSContractAllocations.ToCaseInsensitiveDictionary();
        }

        /// <summary>
        /// Gets the allocation for (this contract reference).
        /// </summary>
        /// <param name="thisContractReference">this contract reference.</param>
        /// <returns>
        /// a contract allocation (if found)
        /// </returns>
        public IFcsContractAllocation GetContractAllocationFor(string thisContractReference) =>
            _contractAllocations.GetValueOrDefault(thisContractReference);

        /// <summary>
        /// Gets the contract allocations for.
        /// </summary>
        /// <param name="thisProviderID">this provider identifier.</param>
        /// <returns>
        /// a collection of contract allocations for the provider
        /// </returns>
        public IReadOnlyCollection<IFcsContractAllocation> GetContractAllocationsFor(int thisProviderID)
        {
            return _contractAllocations.Values
                .SafeWhere(ca => ca.DeliveryUKPRN == thisProviderID)
                .AsSafeReadOnlyList();
        }

        /// <summary>
        /// Gets the eligibility rule for (this contract reference).
        /// </summary>
        /// <param name="thisContractReference">this contract reference.</param>
        /// <returns>
        /// an eligibility rule (if found)
        /// </returns>
        public IEsfEligibilityRule GetEligibilityRuleFor(string thisContractReference)
        {
            var contractAllocation = GetContractAllocationFor(thisContractReference);

            return contractAllocation?.EsfEligibilityRule;
        }

        /// <summary>
        /// Gets the eligibility rule employment statuses for (this contract reference).
        /// </summary>
        /// <param name="thisContractReference">this contract reference.</param>
        /// <returns>
        /// a collection of employment statuses (if found) or an empty list
        /// </returns>
        public IEnumerable<IEsfEligibilityRuleEmploymentStatus> GetEligibilityRuleEmploymentStatusesFor(string thisContractReference)
        {
            var eligibility = GetEligibilityRuleFor(thisContractReference);

            return eligibility?.EmploymentStatuses
                ?? Collection.EmptyAndReadOnly<IEsfEligibilityRuleEmploymentStatus>();
        }

        /// <summary>
        /// Gets the eligibility rule local authorities for (this contract reference).
        /// </summary>
        /// <param name="thisContractReference">this contract reference.</param>
        /// <returns>
        /// a collection of local authorities (if found) or an empty list
        /// </returns>
        public IEnumerable<IEsfEligibilityRuleLocalAuthority> GetEligibilityRuleLocalAuthoritiesFor(string thisContractReference)
        {
            var eligibility = GetEligibilityRuleFor(thisContractReference);

            return eligibility?.LocalAuthorities
                ?? Collection.EmptyAndReadOnly<IEsfEligibilityRuleLocalAuthority>();
        }

        /// <summary>
        /// Gets the eligibility rule enterprise partnerships for (this contract reference).
        /// </summary>
        /// <param name="thisContractReference">this contract reference.</param>
        /// <returns>
        /// a collection of enterprise partnerships (if found) or an empty list
        /// </returns>
        public IEnumerable<IEsfEligibilityRuleLocalEnterprisePartnership> GetEligibilityRuleEnterprisePartnershipsFor(string thisContractReference)
        {
            var eligibility = GetEligibilityRuleFor(thisContractReference);

            return eligibility?.LocalEnterprisePartnerships
                ?? Collection.EmptyAndReadOnly<IEsfEligibilityRuleLocalEnterprisePartnership>();
        }

        /// <summary>
        /// Gets the eligibility rule sector subject area levels for (this contract reference).
        /// </summary>
        /// <param name="thisContractReference">this contract reference.</param>
        /// <returns>
        /// a collection of sector subject area levels (if found) or an empty list
        /// </returns>
        public IEnumerable<IEsfEligibilityRuleSectorSubjectAreaLevel> GetEligibilityRuleSectorSubjectAreaLevelsFor(string thisContractReference)
        {
            var eligibility = GetEligibilityRuleFor(thisContractReference);

            return eligibility?.SectorSubjectAreaLevels
                ?? Collection.EmptyAndReadOnly<IEsfEligibilityRuleSectorSubjectAreaLevel>();
        }

        /// <summary>
        /// Contract reference number exists.
        /// </summary>
        /// <param name="conRefNumber">The con reference number.</param>
        /// <returns>true if it does</returns>
        public bool ConRefNumberExists(string conRefNumber)
        {
            return !string.IsNullOrEmpty(conRefNumber) && _contractAllocations.ContainsKey(conRefNumber);
        }

        /// <summary>
        /// Fundings the relationship FCT exists.
        /// </summary>
        /// <param name="fundingStreamPeriodCodes">The funding stream period codes.</param>
        /// <returns>true if it does</returns>
        public bool FundingRelationshipFCTExists(IEnumerable<string> fundingStreamPeriodCodes)
        {
            var fsCodes = fundingStreamPeriodCodes.ToCaseInsensitiveHashSet();

            return _contractAllocations.Values.Any(ca => fsCodes.Contains(ca.FundingStreamPeriodCode));
        }
    }
}
