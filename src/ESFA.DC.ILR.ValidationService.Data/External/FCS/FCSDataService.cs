using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Utility;

namespace ESFA.DC.ILR.ValidationService.Data.External.FCS
{
    public class FCSDataService : IFCSDataService
    {
        private readonly IReadOnlyDictionary<string, IFcsContractAllocation> _contractAllocations;

        public FCSDataService(IExternalDataCache externalDataCache)
        {
            _contractAllocations = externalDataCache.FCSContractAllocations.ToCaseInsensitiveDictionary();
        }

        public IFcsContractAllocation GetContractAllocationFor(string thisContractReference) =>
            _contractAllocations.GetValueOrDefault(thisContractReference);

        public IReadOnlyCollection<IFcsContractAllocation> GetContractAllocationsFor(int thisProviderID)
        {
            return _contractAllocations.Values
                .NullSafeWhere(ca => ca.DeliveryUKPRN == thisProviderID)
                .ToReadOnlyCollection();
        }

        public IEsfEligibilityRule GetEligibilityRuleFor(string thisContractReference)
        {
            return GetContractAllocationFor(thisContractReference)?.EsfEligibilityRule;
        }

        public IEnumerable<IEsfEligibilityRuleEmploymentStatus> GetEligibilityRuleEmploymentStatusesFor(string thisContractReference)
        {
            return GetEligibilityRuleFor(thisContractReference)?
                .EmploymentStatuses ?? Array.Empty<IEsfEligibilityRuleEmploymentStatus>();
        }

        public IEnumerable<IEsfEligibilityRuleLocalAuthority> GetEligibilityRuleLocalAuthoritiesFor(string thisContractReference)
        {
            return GetEligibilityRuleFor(thisContractReference)?
                .LocalAuthorities ?? Array.Empty<IEsfEligibilityRuleLocalAuthority>();
        }

        public IEnumerable<IEsfEligibilityRuleLocalEnterprisePartnership> GetEligibilityRuleEnterprisePartnershipsFor(string thisContractReference)
        {
            return GetEligibilityRuleFor(thisContractReference)?
               .LocalEnterprisePartnerships ?? Array.Empty<IEsfEligibilityRuleLocalEnterprisePartnership>();
        }

        public IEnumerable<IEsfEligibilityRuleSectorSubjectAreaLevel> GetEligibilityRuleSectorSubjectAreaLevelsFor(string thisContractReference)
        {
            return GetEligibilityRuleFor(thisContractReference)?
              .SectorSubjectAreaLevels ?? Array.Empty<IEsfEligibilityRuleSectorSubjectAreaLevel>();
        }

        public bool ConRefNumberExists(string conRefNumber)
        {
            return !string.IsNullOrEmpty(conRefNumber) && _contractAllocations.ContainsKey(conRefNumber);
        }

        public bool FundingRelationshipFCTExists(IEnumerable<string> fundingStreamPeriodCodes)
        {
            var fsCodes = fundingStreamPeriodCodes.ToCaseInsensitiveHashSet();

            return _contractAllocations.Values.Any(ca => fsCodes.Contains(ca.FundingStreamPeriodCode));
        }
    }
}
