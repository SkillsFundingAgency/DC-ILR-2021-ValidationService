using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface
{
    public interface ILARSStandard
    {
        int StandardCode { get; }

        string StandardSectorCode { get; }

        string NotionalEndLevel { get; }

        DateTime? LastDateStarts { get; }

        IEnumerable<ILARSStandardFunding> StandardsFunding { get; }

        DateTime? EffectiveFrom { get; }

        DateTime? EffectiveTo { get; }
    }
}
