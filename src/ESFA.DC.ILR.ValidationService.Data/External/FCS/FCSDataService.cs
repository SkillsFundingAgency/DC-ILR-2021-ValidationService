﻿using ESFA.DC.ILR.ValidationService.Data.Extensions;
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
        /// The employment statuses
        /// </summary>
        private readonly IReadOnlyCollection<IEsfEligibilityRuleEmploymentStatus> _employmentStatuses;

        /// <summary>
        /// The contract allocations
        /// </summary>
        private readonly IReadOnlyCollection<IFcsContractAllocation> _contractAllocations;

        /// <summary>
        /// The Sector Subject Area Levels
        /// </summary>
        private readonly IReadOnlyCollection<IEsfEligibilityRuleSectorSubjectAreaLevel> _sectorSubjectAreaLevels;

        public FCSDataService(IExternalDataCache externalDataCache)
        {
            _employmentStatuses = externalDataCache.ESFEligibilityRuleEmploymentStatuses.AsSafeReadOnlyList();
            _contractAllocations = externalDataCache.FCSContractAllocations.AsSafeReadOnlyList();
            _sectorSubjectAreaLevels = externalDataCache.EsfEligibilityRuleSectorSubjectAreaLevels;
        }

        /// <summary>
        /// Contract reference number exists.
        /// </summary>
        /// <param name="conRefNumber">The con reference number.</param>
        /// <returns>true if it does</returns>
        public bool ConRefNumberExists(string conRefNumber)
        {
            return _contractAllocations
                .Where(ca => ca.ContractAllocationNumber.CaseInsensitiveEquals(conRefNumber))
                .Any();
        }

        /// <summary>
        /// Fundings the relationship FCT exists.
        /// </summary>
        /// <param name="fundingStreamPeriodCodes">The funding stream period codes.</param>
        /// <returns>true if it does</returns>
        public bool FundingRelationshipFCTExists(IEnumerable<string> fundingStreamPeriodCodes)
        {
            var fsCodes = fundingStreamPeriodCodes.AsSafeReadOnlyList().ToCaseInsensitiveHashSet();

            return _contractAllocations
               .Where(ca => fsCodes.Contains(ca.FundingStreamPeriodCode))
               .Any();
        }

        /// <summary>
        /// Gets the eligibility rule employment status.
        /// 2018-11-08 CME: this routine may require refinement as i'm not convinced i'm filtering with all of the correct criteria
        /// </summary>
        /// <param name="forContractReference">For contract reference.</param>
        /// <returns>the eligibility rule employment status (should there be one)</returns>
        public IEsfEligibilityRuleEmploymentStatus GetEligibilityRuleEmploymentStatus(string forContractReference)
        {
            var allocation = _contractAllocations.FirstOrDefault(x => x.ContractAllocationNumber.CaseInsensitiveEquals(forContractReference));
            return It.Has(allocation)
                ? _employmentStatuses.FirstOrDefault(x => x.TenderSpecReference.ComparesWith(allocation.TenderSpecReference))
                : null;
        }

        public IReadOnlyCollection<IEsfEligibilityRuleSectorSubjectAreaLevel> GetSectorSubjectAreaLevelsForContract(string conRefNumber)
        {
            return _sectorSubjectAreaLevels?
                .Join(
                    _contractAllocations?.Where(ca => ca.ContractAllocationNumber.CaseInsensitiveEquals(conRefNumber)).ToList(),
                    ers => new { ers.TenderSpecReference, ers.LotReference },
                    ca => new { ca.TenderSpecReference, ca.LotReference },
                    (ers, ca) => ers).ToList();
        }

        public bool IsSectorSubjectAreaCodeExistsForContract(string conRefNumber)
        {
            return GetSectorSubjectAreaLevelsForContract(conRefNumber)?
                .Any(
                s => s.SectorSubjectAreaCode.HasValue
                && (string.IsNullOrEmpty(s.MinLevelCode)
                && string.IsNullOrEmpty(s.MaxLevelCode))) ?? false;
        }
    }
}
