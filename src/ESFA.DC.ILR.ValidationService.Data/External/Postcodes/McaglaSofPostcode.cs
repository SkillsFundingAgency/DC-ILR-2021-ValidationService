using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.ILR.ValidationService.Data.External.Postcodes
{
    public class McaglaSOFPostcode : IMcaglaSOFPostcode
    {
        /// <summary>
        /// Gets the postcode.
        /// </summary>
        public string Postcode { get; set; }
        
        /// <summary>
        /// Gets the SofCode.
        /// </summary>
        public string SofCode { get; set; }

        /// <summary>
        /// Gets the effective from (date).
        /// </summary>
        public DateTime EffectiveFrom { get; set; }

        /// <summary>
        /// Gets the effective to.
        /// </summary>
        public DateTime? EffectiveTo { get; set; }
    }
}
