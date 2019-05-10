using System;

namespace ESFA.DC.ILR.ValidationService.Data.Internal.Model
{
    /// <summary>
    /// lookup validity periods
    /// </summary>
    public struct ValidityPeriods
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidityPeriods"/> struct.
        /// </summary>
        /// <param name="validFrom">The valid from.</param>
        /// <param name="validTo">The valid to.</param>
        public ValidityPeriods(DateTime? validFrom, DateTime? validTo)
        {
            ValidFrom = validFrom;
            ValidTo = validTo;
        }

        /// <summary>
        /// Gets valid from.
        /// </summary>
        public DateTime? ValidFrom { get; }

        /// <summary>
        /// Gets valid to.
        /// </summary>
        public DateTime? ValidTo { get; }
    }
}
