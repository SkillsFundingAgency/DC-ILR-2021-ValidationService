using System;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.External.Organisation.Model
{
    public class ShortTermFundingInitiative : IShortTermFundingInitiative
    {
        public long UKPRN { get; set; }

        public string LdmCode { get; set; }

        public string Reason { get; set; }

        public DateTime EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }
    }
}
