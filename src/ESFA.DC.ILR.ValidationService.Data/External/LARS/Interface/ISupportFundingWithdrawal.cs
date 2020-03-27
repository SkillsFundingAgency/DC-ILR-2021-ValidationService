using System;

namespace ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface
{
    /// <summary>
    /// i support funding withdrawal
    /// for the use-case where LARS 'effective to' dates are set
    /// one day before the 'effective from' dates as a mechanism to remove funding. 'start
    ///  date' and 'end date' have been used over 'effective from' and 'effective to'
    /// for clarity and consistency
    /// </summary>
    public interface ISupportFundingWithdrawal
    {
        /// <summary>
        /// Gets the effective from (date).
        /// </summary>
        DateTime StartDate { get; }

        /// <summary>
        /// Gets the effective to (date).
        /// </summary>
        DateTime? EndDate { get; }
    }
}
