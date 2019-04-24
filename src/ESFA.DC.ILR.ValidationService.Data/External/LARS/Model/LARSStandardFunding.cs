using System;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.External.LARS.Model
{
    public class LARSStandardFunding : ILARSStandardFunding
    {
        public DateTime EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }

        public decimal? CoreGovContributionCap { get; set; }
    }
}
