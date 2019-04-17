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
        public FcsDataMapper()
        {
        }

        public IReadOnlyDictionary<string, IFcsContractAllocation> MapFcsContractAllocations(IReadOnlyCollection<ReferenceDataService.Model.FCS.FcsContractAllocation> fcsContractAllocations)
        {
            return fcsContractAllocations
                .ToDictionary(
                ca => ca.ContractAllocationNumber,
                f => new FcsContractAllocation
                {
                    ContractAllocationNumber = f.ContractAllocationNumber,
                    DeliveryUKPRN = f.DeliveryUKPRN,
                    StartDate = f.StartDate,
                    FundingStreamPeriodCode = f.FundingStreamPeriodCode,
                    TenderSpecReference = f.TenderSpecReference,
                    LotReference = f.LotReference,
                    EsfEligibilityRule = new EsfEligibilityRule
                    {
                        Benefits = f.EsfEligibilityRule.Benefits,
                        TenderSpecReference = f.EsfEligibilityRule.TenderSpecReference,
                        LotReference = f.EsfEligibilityRule.LotReference,
                        MaxAge = f.EsfEligibilityRule.MaxAge,
                        MaxLengthOfUnemployment = f.EsfEligibilityRule.MaxLengthOfUnemployment,
                        MaxPriorAttainment = f.EsfEligibilityRule.MaxPriorAttainment,
                        MinAge = f.EsfEligibilityRule.MinAge,
                        MinLengthOfUnemployment = f.EsfEligibilityRule.MinLengthOfUnemployment,
                        MinPriorAttainment = f.EsfEligibilityRule.MinPriorAttainment,
                        EmploymentStatuses = f.EsfEligibilityRule.EmploymentStatuses.Select(es => new EsfEligibilityRuleEmploymentStatus
                        {
                            Code = es.Code,
                            LotReference = f.LotReference,
                            TenderSpecReference = f.TenderSpecReference
                        }).ToList(),
                        LocalAuthorities = f.EsfEligibilityRule.LocalAuthorities.Select(la => new EsfEligibilityRuleLocalAuthority
                        {
                            Code = la.Code,
                            LotReference = f.LotReference,
                            TenderSpecReference = f.TenderSpecReference
                        }).ToList(),
                        LocalEnterprisePartnerships = f.EsfEligibilityRule.LocalEnterprisePartnerships.Select(lep => new EsfEligibilityRuleLocalEnterprisePartnership
                        {
                            Code = lep.Code,
                            LotReference = f.LotReference,
                            TenderSpecReference = f.TenderSpecReference
                        }).ToList(),
                        SectorSubjectAreaLevels = f.EsfEligibilityRule.SectorSubjectAreaLevels.Select(ssa => new EsfEligibilityRuleSectorSubjectAreaLevel
                        {
                            MaxLevelCode = ssa.MaxLevelCode,
                            MinLevelCode = ssa.MinLevelCode,
                            SectorSubjectAreaCode = ssa.SectorSubjectAreaCode,
                            LotReference = f.LotReference,
                            TenderSpecReference = f.TenderSpecReference
                        }).ToList()
                    }
                } as IFcsContractAllocation, StringComparer.OrdinalIgnoreCase);
        }
    }
}
