using System.Collections.Generic;
using ESFA.DC.ILR.ReferenceDataService.Model.Postcodes;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Interface
{
    public interface IPostcodesDataMapper : IMapper
    {
        IReadOnlyCollection<string> MapPostcodes(IReadOnlyCollection<Postcode> postcodes);

        IReadOnlyCollection<ONSPostcode> MapONSPostcodes(IReadOnlyCollection<Postcode> postcodes);

        IReadOnlyCollection<McaglaSOFPostcode> MapMcaglaSOFPostcodes(IReadOnlyCollection<Postcode> postcodes);
    }
}
