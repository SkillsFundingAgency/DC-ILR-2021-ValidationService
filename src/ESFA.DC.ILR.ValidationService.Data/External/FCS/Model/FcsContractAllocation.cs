using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using System;

namespace ESFA.DC.ILR.ValidationService.Data.External.FCS.Model
{
    /// <summary>
    /// the FCS Contract Allocation implementation
    /// </summary>
    /// <seealso cref="IFcsContractAllocation" />
    public class FcsContractAllocation : IFcsContractAllocation
    {
        /// <summary>
        /// Gets or sets the contract allocation number.
        /// </summary>
        public string ContractAllocationNumber { get; set; }

        /// <summary>
        /// Gets or sets the tender spec reference.
        /// </summary>
        public string TenderSpecReference { get; set; }

        /// <summary>
        /// Gets or sets the lot reference.
        /// </summary>
        public string LotReference { get; set; }

        /// <summary>
        /// Gets or sets the funding stream period code.
        /// </summary>
        public string FundingStreamPeriodCode { get; set; }

        /// <summary>
        /// Gets or sets the Start Date
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the StopNewStartsFromDate
        /// </summary>
        public DateTime? StopNewStartsFromDate { get; set;  }

        public IEsfEligibilityRule EsfEligibilityRule { get; set; }

        /// <summary>
        /// Gets or sets the delivery ukprn.
        /// </summary>
        public int DeliveryUKPRN { get; set; }
    }
}
