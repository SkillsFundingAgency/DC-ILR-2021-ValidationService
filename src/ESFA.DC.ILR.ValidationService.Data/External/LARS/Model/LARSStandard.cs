using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.External.LARS.Model
{
    public class LARSStandard : ILARSStandard
    {
        public int StandardCode { get; set; }

        public string StandardSectorCode { get; set; }

        public string NotionalEndLevel { get; set; }

        public DateTime? LastDateStarts { get; set; }

        public DateTime? EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }

        public IEnumerable<ILARSStandardFunding> StandardsFunding { get; set; }
    }
}
