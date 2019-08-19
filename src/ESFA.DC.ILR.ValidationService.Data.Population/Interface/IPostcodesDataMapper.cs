using System.Collections.Generic;
using ESFA.DC.ILR.ReferenceDataService.Model.Postcodes;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using DevolvedPostcodeRDS = ESFA.DC.ILR.ReferenceDataService.Model.PostcodesDevolution.DevolvedPostcode;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Interface
{
    public interface IPostcodesDataMapper : IMapper
    {
        IReadOnlyCollection<string> MapPostcodes(IReadOnlyCollection<Postcode> postcodes);

        IReadOnlyCollection<ONSPostcode> MapONSPostcodes(IReadOnlyCollection<Postcode> postcodes);

        IReadOnlyDictionary<string, List<DevolvedPostcode>> MapDevolvedPostcodes(IReadOnlyCollection<DevolvedPostcodeRDS> postcodes);
    }
}
