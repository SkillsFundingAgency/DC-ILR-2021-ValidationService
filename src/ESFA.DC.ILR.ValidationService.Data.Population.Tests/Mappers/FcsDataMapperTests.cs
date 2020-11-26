using System;
using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Model;
using ESFA.DC.ILR.ValidationService.Data.Population.Mappers;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Tests.Mappers
{
    public class FcsDataMapperTests
    {
        [Fact]
        public void MapFcsContractAllocations()
        {
            var fcsContractAllocations = TestFcsContractAllocations();

            var expectedFcsContractAllocations = new Dictionary<string, FcsContractAllocation>
            {
                {
                    "ConRef1",
                    new FcsContractAllocation
                    {
                        ContractAllocationNumber = "ConRef1",
                        DeliveryUKPRN = 1,
                        StartDate = new DateTime(2018, 8, 1),
                        StopNewStartsFromDate = null,
                        FundingStreamPeriodCode = "FundingStreamPeriodCode",
                        TenderSpecReference = "TenderSpecReference",
                        LotReference = "LotReference",
                        EsfEligibilityRule = new EsfEligibilityRule
                        {
                            Benefits = true,
                            TenderSpecReference = "TenderSpecReference",
                            LotReference = "LotReference",
                            MaxAge = 1,
                            MaxLengthOfUnemployment = 1,
                            MaxPriorAttainment = "MaxPriorAttainment",
                            MinAge = 1,
                            MinLengthOfUnemployment = 1,
                            MinPriorAttainment = "MinPriorAttainment",
                            EmploymentStatuses = new List<EsfEligibilityRuleEmploymentStatus>
                            {
                                new EsfEligibilityRuleEmploymentStatus
                                {
                                    Code = 1,
                                    TenderSpecReference = "TenderSpecReference",
                                    LotReference = "LotReference",
                                },
                            },
                            LocalAuthorities = new List<EsfEligibilityRuleLocalAuthority>
                            {
                                new EsfEligibilityRuleLocalAuthority
                                {
                                    Code = "1",
                                    TenderSpecReference = "TenderSpecReference",
                                    LotReference = "LotReference",
                                },
                            },
                            LocalEnterprisePartnerships = new List<EsfEligibilityRuleLocalEnterprisePartnership>
                            {
                                new EsfEligibilityRuleLocalEnterprisePartnership
                                {
                                    Code = "1",
                                    TenderSpecReference = "TenderSpecReference",
                                    LotReference = "LotReference",
                                },
                                new EsfEligibilityRuleLocalEnterprisePartnership
                                {
                                    Code = "2",
                                    TenderSpecReference = "TenderSpecReference",
                                    LotReference = "LotReference",
                                },
                            },
                            SectorSubjectAreaLevels = new List<EsfEligibilityRuleSectorSubjectAreaLevel>
                            {
                                new EsfEligibilityRuleSectorSubjectAreaLevel
                                {
                                    MaxLevelCode = "1",
                                    MinLevelCode = "1",
                                    SectorSubjectAreaCode = 1.0m,
                                    TenderSpecReference = "TenderSpecReference",
                                    LotReference = "LotReference",
                                },
                            },
                        },
                    }
                },
                {
                    "ConRef2",
                    new FcsContractAllocation
                    {
                        ContractAllocationNumber = "ConRef2",
                        DeliveryUKPRN = 1,
                        StartDate = new DateTime(2018, 8, 1),
                        StopNewStartsFromDate = null,
                        FundingStreamPeriodCode = "FundingStreamPeriodCode",
                        TenderSpecReference = "TenderSpecReference",
                        LotReference = "LotReference",
                        EsfEligibilityRule = new EsfEligibilityRule
                        {
                            Benefits = true,
                            TenderSpecReference = "TenderSpecReference",
                            LotReference = "LotReference",
                            MaxAge = 1,
                            MaxLengthOfUnemployment = 1,
                            MaxPriorAttainment = "MaxPriorAttainment",
                            MinAge = 1,
                            MinLengthOfUnemployment = 1,
                            MinPriorAttainment = "MinPriorAttainment",
                            EmploymentStatuses = new List<EsfEligibilityRuleEmploymentStatus>
                            {
                                new EsfEligibilityRuleEmploymentStatus
                                {
                                    Code = 1,
                                    TenderSpecReference = "TenderSpecReference",
                                    LotReference = "LotReference",
                                },
                            },
                            LocalAuthorities = new List<EsfEligibilityRuleLocalAuthority>
                            {
                                new EsfEligibilityRuleLocalAuthority
                                {
                                    Code = "1",
                                    TenderSpecReference = "TenderSpecReference",
                                    LotReference = "LotReference",
                                },
                            },
                            LocalEnterprisePartnerships = new List<EsfEligibilityRuleLocalEnterprisePartnership>
                            {
                                new EsfEligibilityRuleLocalEnterprisePartnership
                                {
                                    Code = "1",
                                    TenderSpecReference = "TenderSpecReference",
                                    LotReference = "LotReference",
                                },
                            },
                            SectorSubjectAreaLevels = new List<EsfEligibilityRuleSectorSubjectAreaLevel>
                            {
                                new EsfEligibilityRuleSectorSubjectAreaLevel
                                {
                                    MaxLevelCode = "1",
                                    MinLevelCode = "1",
                                    SectorSubjectAreaCode = 1.0m,
                                    TenderSpecReference = "TenderSpecReference",
                                    LotReference = "LotReference",
                                },
                            },
                        },
                    }
                },
                {
                    "ConRef3",
                    new FcsContractAllocation
                    {
                        ContractAllocationNumber = "ConRef3",
                        DeliveryUKPRN = 1,
                        StartDate = new DateTime(2018, 8, 1),
                        StopNewStartsFromDate = null,
                        FundingStreamPeriodCode = "FundingStreamPeriodCode",
                        TenderSpecReference = "TenderSpecReference",
                        LotReference = "LotReference",
                    }
                },
            };

            NewMapper().MapFcsContractAllocations(fcsContractAllocations).Should().BeEquivalentTo(expectedFcsContractAllocations);
        }

        private IReadOnlyCollection<ReferenceDataService.Model.FCS.FcsContractAllocation> TestFcsContractAllocations()
        {
            return new List<ReferenceDataService.Model.FCS.FcsContractAllocation>
            {
                new ReferenceDataService.Model.FCS.FcsContractAllocation
                {
                    ContractAllocationNumber = "ConRef1",
                    DeliveryUKPRN = 1,
                    StartDate = new DateTime(2018, 8, 1),
                    StopNewStartsFromDate = null,
                    FundingStreamPeriodCode = "FundingStreamPeriodCode",
                    TenderSpecReference = "TenderSpecReference",
                    LotReference = "LotReference",
                    EsfEligibilityRule = new ReferenceDataService.Model.FCS.EsfEligibilityRule
                    {
                        Benefits = true,
                        TenderSpecReference = "TenderSpecReference",
                        LotReference = "LotReference",
                        MaxAge = 1,
                        MaxLengthOfUnemployment = 1,
                        MaxPriorAttainment = "MaxPriorAttainment",
                        MinAge = 1,
                        MinLengthOfUnemployment = 1,
                        MinPriorAttainment = "MinPriorAttainment",
                        EmploymentStatuses = new List<ReferenceDataService.Model.FCS.EsfEligibilityRuleEmploymentStatus>
                        {
                            new ReferenceDataService.Model.FCS.EsfEligibilityRuleEmploymentStatus
                            {
                                Code = 1,
                            },
                        },
                        LocalAuthorities = new List<ReferenceDataService.Model.FCS.EsfEligibilityRuleLocalAuthority>
                        {
                            new ReferenceDataService.Model.FCS.EsfEligibilityRuleLocalAuthority
                            {
                                Code = "1",
                            },
                        },
                        LocalEnterprisePartnerships = new List<ReferenceDataService.Model.FCS.EsfEligibilityRuleLocalEnterprisePartnership>
                        {
                            new ReferenceDataService.Model.FCS.EsfEligibilityRuleLocalEnterprisePartnership
                            {
                                Code = "1",
                            },
                            new ReferenceDataService.Model.FCS.EsfEligibilityRuleLocalEnterprisePartnership
                            {
                                Code = "2",
                            },
                        },
                        SectorSubjectAreaLevels = new List<ReferenceDataService.Model.FCS.EsfEligibilityRuleSectorSubjectAreaLevel>
                        {
                            new ReferenceDataService.Model.FCS.EsfEligibilityRuleSectorSubjectAreaLevel
                            {
                                MaxLevelCode = "1",
                                MinLevelCode = "1",
                                SectorSubjectAreaCode = 1.0m,
                            },
                        },
                    },
                },
                new ReferenceDataService.Model.FCS.FcsContractAllocation
                {
                    ContractAllocationNumber = "ConRef2",
                    DeliveryUKPRN = 1,
                    StartDate = new DateTime(2018, 8, 1),
                    StopNewStartsFromDate = null,
                    FundingStreamPeriodCode = "FundingStreamPeriodCode",
                    TenderSpecReference = "TenderSpecReference",
                    LotReference = "LotReference",
                    EsfEligibilityRule = new ReferenceDataService.Model.FCS.EsfEligibilityRule
                    {
                        Benefits = true,
                        TenderSpecReference = "TenderSpecReference",
                        LotReference = "LotReference",
                        MaxAge = 1,
                        MaxLengthOfUnemployment = 1,
                        MaxPriorAttainment = "MaxPriorAttainment",
                        MinAge = 1,
                        MinLengthOfUnemployment = 1,
                        MinPriorAttainment = "MinPriorAttainment",
                        EmploymentStatuses = new List<ReferenceDataService.Model.FCS.EsfEligibilityRuleEmploymentStatus>
                        {
                            new ReferenceDataService.Model.FCS.EsfEligibilityRuleEmploymentStatus
                            {
                                Code = 1,
                            },
                        },
                        LocalAuthorities = new List<ReferenceDataService.Model.FCS.EsfEligibilityRuleLocalAuthority>
                        {
                            new ReferenceDataService.Model.FCS.EsfEligibilityRuleLocalAuthority
                            {
                                Code = "1",
                            },
                        },
                        LocalEnterprisePartnerships = new List<ReferenceDataService.Model.FCS.EsfEligibilityRuleLocalEnterprisePartnership>
                        {
                            new ReferenceDataService.Model.FCS.EsfEligibilityRuleLocalEnterprisePartnership
                            {
                                Code = "1",
                            },
                        },
                        SectorSubjectAreaLevels = new List<ReferenceDataService.Model.FCS.EsfEligibilityRuleSectorSubjectAreaLevel>
                        {
                            new ReferenceDataService.Model.FCS.EsfEligibilityRuleSectorSubjectAreaLevel
                            {
                                MaxLevelCode = "1",
                                MinLevelCode = "1",
                                SectorSubjectAreaCode = 1.0m,
                            },
                        },
                    },
                },
                new ReferenceDataService.Model.FCS.FcsContractAllocation
                {
                    ContractAllocationNumber = "ConRef3",
                    DeliveryUKPRN = 1,
                    StartDate = new DateTime(2018, 8, 1),
                    StopNewStartsFromDate = null,
                    FundingStreamPeriodCode = "FundingStreamPeriodCode",
                    TenderSpecReference = "TenderSpecReference",
                    LotReference = "LotReference",
                },
            };
        }

        private FcsDataMapper NewMapper()
        {
            return new FcsDataMapper();
        }
    }
}
