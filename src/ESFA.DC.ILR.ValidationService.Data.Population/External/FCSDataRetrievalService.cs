using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ReferenceData.FCS.Model.Interface;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ValidationService.Data.Population.External
{
    /// <summary>
    /// the FCS data service implementation
    /// </summary>
    /// <seealso cref="AbstractDataRetrievalService" />
    /// <seealso cref="IFCSDataRetrievalService" />
    public class FCSDataRetrievalService :
        AbstractDataRetrievalService,
        IFCSDataRetrievalService
    {
        /// <summary>
        /// The FCS (DB Context)
        /// </summary>
        private readonly IFcsContext _fcs;

        /// <summary>
        /// Initializes a new instance of the <see cref="FCSDataRetrievalService"/> class.
        /// </summary>
        /// <param name="fcs">The FCS.</param>
        /// <param name="messageCache">The message cache.</param>
        public FCSDataRetrievalService(IFcsContext fcs, ICache<IMessage> messageCache)
            : base(messageCache)
        {
            _fcs = fcs;
        }

        /// <summary>
        /// Retrieves the (FCS Contracts) asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// a task running the collection builder
        /// </returns>
        public async Task<IReadOnlyDictionary<string, IFcsContractAllocation>> RetrieveAsync(CancellationToken cancellationToken)
        {
            var ukprn = UKPRNFromMessage(_messageCache.Item);

            var contractAllocations = await _fcs.ContractAllocations
                .Where(ca => ca.DeliveryUKPRN == ukprn)
                .Select(ca => new FcsContractAllocation
                {
                    ContractAllocationNumber = ca.ContractAllocationNumber,
                    FundingStreamPeriodCode = ca.FundingStreamPeriodCode,
                    LotReference = ca.LotReference,
                    TenderSpecReference = ca.TenderSpecReference,
                    StartDate = ca.StartDate,
                    DeliveryUKPRN = (int)ca.DeliveryUKPRN // <= this cannot be null
                })
                .ToListAsync(cancellationToken);

            // No Foreign key Relationship on Model for Eligibility Rules, do two queries and overlay in Memory, only around 300 Rules, not worth filter at this point.
            var eligibilityRules = await _fcs.EsfEligibilityRules
                .Select(r => new EsfEligibilityRule
                {
                    LotReference = r.LotReference,
                    TenderSpecReference = r.TenderSpecReference,
                    MinAge = r.MinAge,
                    MaxAge = r.MaxAge,
                    Benefits = r.Benefits,
                    MinLengthOfUnemployment = r.MinLengthOfUnemployment,
                    MaxLengthOfUnemployment = r.MaxLengthOfUnemployment,
                    EmploymentStatuses = r.EsfEligibilityRuleEmploymentStatuses
                        .Select(s => new EsfEligibilityRuleEmploymentStatus
                        {
                            Code = s.Code,
                            LotReference = s.LotReference,
                            TenderSpecReference = s.TenderSpecReference,
                        })
                        .ToList(),
                    LocalAuthorities = r.EsfEligibilityRuleLocalAuthorities
                        .Select(a => new EsfEligibilityRuleLocalAuthority
                        {
                            Code = a.Code,
                            LotReference = a.LotReference,
                            TenderSpecReference = a.TenderSpecReference,
                        })
                        .ToList(),
                    LocalEnterprisePartnerships = r.EsfEligibilityRuleLocalEnterprisePartnerships
                        .Select(p => new EsfEligibilityRuleLocalEnterprisePartnership
                        {
                            Code = p.Code,
                            LotReference = p.LotReference,
                            TenderSpecReference = p.TenderSpecReference
                        })
                        .ToList(),
                    SectorSubjectAreaLevels = r.EsfEligibilityRuleSectorSubjectAreaLevel
                        .Select(l => new EsfEligibilityRuleSectorSubjectAreaLevel
                        {
                            MaxLevelCode = l.MaxLevelCode,
                            MinLevelCode = l.MinLevelCode,
                            SectorSubjectAreaCode = l.SectorSubjectAreaCode,
                            LotReference = l.LotReference,
                            TenderSpecReference = l.TenderSpecReference
                        })
                        .ToList(),
                    MinPriorAttainment = r.MinPriorAttainment,
                    MaxPriorAttainment = r.MaxPriorAttainment
                }).ToListAsync(cancellationToken);

            foreach (var contractAllocation in contractAllocations)
            {
                contractAllocation.EsfEligibilityRule = eligibilityRules
                    .SingleOrDefault(
                        r => r.LotReference.CaseInsensitiveEquals(contractAllocation.LotReference)
                        && r.TenderSpecReference.CaseInsensitiveEquals(contractAllocation.TenderSpecReference));
            }

            return contractAllocations.ToCaseInsensitiveDictionary<IFcsContractAllocation, IFcsContractAllocation>(ca => ca.ContractAllocationNumber, ca => ca);
        }

        public IEnumerable<string> ConRefNumbersFromMessage(IMessage message)
        {
            return message
                ?.Learners
                ?.Where(l => l.LearningDeliveries != null)
                .SelectMany(l => l.LearningDeliveries)
                .Where(ld => !string.IsNullOrEmpty(ld.ConRefNumber))
                .Select(ld => ld.ConRefNumber)
                .Distinct();
        }

        public int UKPRNFromMessage(IMessage message) =>
            message.LearningProviderEntity.UKPRN;
    }
}
