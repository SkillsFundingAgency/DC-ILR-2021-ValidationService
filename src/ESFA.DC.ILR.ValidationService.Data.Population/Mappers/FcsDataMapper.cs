using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Model;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Mappers
{
    public class FcsDataMapper : IFcsDataMapper
    {
        public IReadOnlyDictionary<string, IFcsContractAllocation> MapFcsContractAllocations(IReadOnlyCollection<ReferenceDataService.Model.FCS.FcsContractAllocation> fcsContractAllocations)
        {
            return fcsContractAllocations?
                .ToDictionary(
                ca => ca.ContractAllocationNumber,
                f => new FcsContractAllocation
                {
                    ContractAllocationNumber = f.ContractAllocationNumber,
                    DeliveryUKPRN = f.DeliveryUKPRN,
                    StartDate = f.StartDate,
                    StopNewStartsFromDate = f.StopNewStartsFromDate,
                    FundingStreamPeriodCode = f.FundingStreamPeriodCode,
                    TenderSpecReference = f.TenderSpecReference,
                    LotReference = f.LotReference,                    
                    EsfEligibilityRule = EsfEligibilityRuleFromEntity(f)
                } as IFcsContractAllocation, StringComparer.OrdinalIgnoreCase);
        }

        private EsfEligibilityRule EsfEligibilityRuleFromEntity(ReferenceDataService.Model.FCS.FcsContractAllocation fcsContractAllocation)
        {
            if (fcsContractAllocation.EsfEligibilityRule == null)
            {
                return null;
            }

            return new EsfEligibilityRule
            {
                Benefits = fcsContractAllocation.EsfEligibilityRule.Benefits,
                TenderSpecReference = fcsContractAllocation.EsfEligibilityRule.TenderSpecReference,
                LotReference = fcsContractAllocation.EsfEligibilityRule.LotReference,
                MaxAge = fcsContractAllocation.EsfEligibilityRule.MaxAge,
                MaxLengthOfUnemployment = fcsContractAllocation.EsfEligibilityRule.MaxLengthOfUnemployment,
                MaxPriorAttainment = fcsContractAllocation.EsfEligibilityRule.MaxPriorAttainment,
                MinAge = fcsContractAllocation.EsfEligibilityRule.MinAge,
                MinLengthOfUnemployment = fcsContractAllocation.EsfEligibilityRule.MinLengthOfUnemployment,
                MinPriorAttainment = fcsContractAllocation.EsfEligibilityRule.MinPriorAttainment,
                EmploymentStatuses = fcsContractAllocation.EsfEligibilityRule.EmploymentStatuses.Select(es => new EsfEligibilityRuleEmploymentStatus
                {
                    Code = es.Code,
                    LotReference = fcsContractAllocation.LotReference,
                    TenderSpecReference = fcsContractAllocation.TenderSpecReference
                }).ToList(),
                LocalAuthorities = fcsContractAllocation.EsfEligibilityRule.LocalAuthorities.Select(la => new EsfEligibilityRuleLocalAuthority
                {
                    Code = la.Code,
                    LotReference = fcsContractAllocation.LotReference,
                    TenderSpecReference = fcsContractAllocation.TenderSpecReference
                }).ToList(),
                LocalEnterprisePartnerships = fcsContractAllocation.EsfEligibilityRule.LocalEnterprisePartnerships.Select(lep => new EsfEligibilityRuleLocalEnterprisePartnership
                {
                    Code = lep.Code,
                    LotReference = fcsContractAllocation.LotReference,
                    TenderSpecReference = fcsContractAllocation.TenderSpecReference
                }).ToList(),
                SectorSubjectAreaLevels = fcsContractAllocation.EsfEligibilityRule.SectorSubjectAreaLevels.Select(ssa => new EsfEligibilityRuleSectorSubjectAreaLevel
                {
                    MaxLevelCode = ssa.MaxLevelCode,
                    MinLevelCode = ssa.MinLevelCode,
                    SectorSubjectAreaCode = ssa.SectorSubjectAreaCode,
                    LotReference = fcsContractAllocation.LotReference,
                    TenderSpecReference = fcsContractAllocation.TenderSpecReference
                }).ToList()
            };
        }
    }
}
