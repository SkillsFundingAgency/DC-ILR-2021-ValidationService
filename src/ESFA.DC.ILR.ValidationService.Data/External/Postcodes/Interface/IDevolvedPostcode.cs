using System;

namespace ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface
{
    public interface IDevolvedPostcode
    {
        string Postcode { get; }
        string Area { get; }
        string SourceOfFunding { get; }
        DateTime EffectiveFrom { get; }
        DateTime? EffectiveTo { get; }
    }
}
