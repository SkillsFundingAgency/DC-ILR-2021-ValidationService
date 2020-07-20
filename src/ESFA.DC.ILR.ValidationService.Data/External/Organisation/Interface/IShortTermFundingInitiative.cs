using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface
{
    public interface IShortTermFundingInitiative
    {
        long UKPRN { get; set; }

        string LdmCode { get; set; }

        string Reason { get; set; }

        DateTime EffectiveFrom { get; set; }

        DateTime? EffectiveTo { get; set; }
    }
}
