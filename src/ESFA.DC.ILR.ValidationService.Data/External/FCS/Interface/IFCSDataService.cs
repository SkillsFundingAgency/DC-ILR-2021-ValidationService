using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface
{
    /// <summary>
    /// the FCS data service definition
    /// </summary>
    public interface IFCSDataService : IDataService
    {
        /// <summary>
        /// Gets the allocation for (this contract reference).
        /// </summary>
        /// <param name="thisContractReference">this contract reference.</param>
        /// <returns>
        /// a contract allocation (if found)
        /// </returns>
        IFcsContractAllocation GetContractAllocationFor(string thisContractReference);

        /// <summary>
        /// Gets the contract allocations for.
        /// </summary>
        /// <param name="thisProviderID">this provider identifier.</param>
        /// <returns>
        /// a collection of contract allocations for the provider
        /// </returns>
        IReadOnlyCollection<IFcsContractAllocation> GetContractAllocationsFor(int thisProviderID);

        /// <summary>
        /// Gets the eligibility rule for (this contract reference).
        /// </summary>
        /// <param name="thisContractReference">this contract reference.</param>
        /// <returns>
        /// an eligibility rule (if found)
        /// </returns>
        IEsfEligibilityRule GetEligibilityRuleFor(string thisContractReference);

        /// <summary>
        /// Gets the eligibility rule employment statuses for (this contract reference).
        /// </summary>
        /// <param name="thisContractReference">this contract reference.</param>
        /// <returns>
        /// a collection of employment statuses (if found) or an empty list
        /// </returns>
        IEnumerable<IEsfEligibilityRuleEmploymentStatus> GetEligibilityRuleEmploymentStatusesFor(string thisContractReference);

        /// <summary>
        /// Gets the eligibility rule local authorities for (this contract reference).
        /// </summary>
        /// <param name="thisContractReference">this contract reference.</param>
        /// <returns>
        /// a collection of local authorities (if found) or an empty list
        /// </returns>
        IEnumerable<IEsfEligibilityRuleLocalAuthority> GetEligibilityRuleLocalAuthoritiesFor(string thisContractReference);

        /// <summary>
        /// Gets the eligibility rule enterprise partnerships for (this contract reference).
        /// </summary>
        /// <param name="thisContractReference">this contract reference.</param>
        /// <returns>
        /// a collection of enterprise partnerships (if found) or an empty list
        /// </returns>
        IEnumerable<IEsfEligibilityRuleLocalEnterprisePartnership> GetEligibilityRuleEnterprisePartnershipsFor(string thisContractReference);

        /// <summary>
        /// Gets the eligibility rule sector subject area levels for (this contract reference).
        /// </summary>
        /// <param name="thisContractReference">this contract reference.</param>
        /// <returns>
        /// a collection of sector subject area levels (if found) or an empty list
        /// </returns>
        IEnumerable<IEsfEligibilityRuleSectorSubjectAreaLevel> GetEligibilityRuleSectorSubjectAreaLevelsFor(string thisContractReference);

        /// <summary>
        /// Contract reference number exists.
        /// </summary>
        /// <param name="conRefNumber">The con reference number.</param>
        /// <returns>true if it does</returns>
        bool ConRefNumberExists(string conRefNumber);

        /// <summary>
        /// Fundings the relationship FCT exists.
        /// </summary>
        /// <param name="fundingStreamPeriodCodes">The funding stream period codes.</param>
        /// <returns>true if it does</returns>
        bool FundingRelationshipFCTExists(IEnumerable<string> fundingStreamPeriodCodes);
    }
}
