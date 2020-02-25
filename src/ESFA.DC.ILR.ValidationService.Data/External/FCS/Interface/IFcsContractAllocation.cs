using System;

namespace ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface
{
    /// <summary>
    /// the FCS contract allocation definition
    /// </summary>
    public interface IFcsContractAllocation
    {
        /// <summary>
        /// Gets the tender spec reference.
        /// </summary>
        string TenderSpecReference { get; }

        /// <summary>
        /// Gets the lot reference.
        /// </summary>
        string LotReference { get; }

        /// <summary>
        /// Gets the contract allocation number.
        /// </summary>
        string ContractAllocationNumber { get; }

        /// <summary>
        /// Gets the funding stream period code.
        /// </summary>
        string FundingStreamPeriodCode { get; }

        /// <summary>
        /// Gets the Start Date
        /// </summary>
        DateTime? StartDate { get; }

        /// <summary>
        /// Gets or sets the StopNewStartsFromDate
        /// </summary>
        DateTime? StopNewStartsFromDate { get; set; }

        /// <summary>
        /// Gets the delivery ukprn.
        /// </summary>
        int DeliveryUKPRN { get; }

        /// <summary>
        ///  Gets Esf Eligibility Rule
        /// </summary>
        IEsfEligibilityRule EsfEligibilityRule { get; }
    }
}
