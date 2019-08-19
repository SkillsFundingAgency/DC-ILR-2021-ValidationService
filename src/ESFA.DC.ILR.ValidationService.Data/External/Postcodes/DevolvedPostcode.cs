using System;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.External.Postcodes
{
    public class DevolvedPostcode : IDevolvedPostcode
    {
        public string Postcode { get; set; }
        public string Area { get; set; }
        public string SourceOfFunding { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
    }
}
