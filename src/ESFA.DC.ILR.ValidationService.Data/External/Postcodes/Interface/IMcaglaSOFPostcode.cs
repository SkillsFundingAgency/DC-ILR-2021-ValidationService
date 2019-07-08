using System;

namespace ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface
{
    public interface IMcaglaSOFPostcode
    {
        /// <summary>
        /// Gets the postcode.
        /// </summary>
        string Postcode { get; }
        
        /// <summary>
        /// Gets the SofCode.
        /// </summary>
        string SofCode { get; }

        /// <summary>
        /// Gets the effective from (date).
        /// </summary>
        DateTime EffectiveFrom { get; }

        /// <summary>
        /// Gets the effective to.
        /// </summary>
        DateTime? EffectiveTo { get; }
    }
}
