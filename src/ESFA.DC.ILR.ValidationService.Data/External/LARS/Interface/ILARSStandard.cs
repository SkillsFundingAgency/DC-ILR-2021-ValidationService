using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface
{
    /// <summary>
    /// lars standard
    /// dates removed to ensure validity is determined using the right classes
    /// </summary>
    public interface ILARSStandard
    {
        /// <summary>
        /// Gets the standard code.
        /// </summary>
        int StandardCode { get; }

        /// <summary>
        /// Gets the standard sector code.
        /// </summary>
        string StandardSectorCode { get; }

        /// <summary>
        /// Gets the notional end level.
        /// </summary>
        string NotionalEndLevel { get; }

        IEnumerable<ILARSStandardFunding> StandardsFunding { get; }

        /// <summary>
        /// Gets the effective from date.
        /// </summary>
        DateTime? EffectiveFrom { get; }

        /// <summary>
        /// Gets the effective to date.
        /// </summary>
        DateTime? EffectiveTo { get; }
    }
}
