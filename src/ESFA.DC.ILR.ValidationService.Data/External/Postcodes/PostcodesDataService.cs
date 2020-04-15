using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Utility;

namespace ESFA.DC.ILR.ValidationService.Data.External.Postcodes
{
    public class PostcodesDataService :
        IPostcodesDataService
    {
        private readonly IExternalDataCache _externalDataCache;

        private readonly IReadOnlyCollection<IONSPostcode> _onsPostcodes;

        private readonly IReadOnlyDictionary<string, IReadOnlyCollection<IDevolvedPostcode>> _devolvedPostcodes;

        public PostcodesDataService(IExternalDataCache externalDataCache)
        {
            It.IsNull(externalDataCache)
                .AsGuard<ArgumentNullException>(nameof(externalDataCache));

            _externalDataCache = externalDataCache;
            _onsPostcodes = _externalDataCache.ONSPostcodes.ToReadOnlyCollection();
            _devolvedPostcodes = _externalDataCache.DevolvedPostcodes;
        }

        public bool PostcodeExists(string postcode)
        {
            return !string.IsNullOrWhiteSpace(postcode)
                   && _externalDataCache.Postcodes.Contains(postcode);
        }

        public IReadOnlyCollection<IONSPostcode> GetONSPostcodes(string fromPostcode) =>
            _onsPostcodes?.Where(x => x.Postcode.CaseInsensitiveEquals(fromPostcode)).ToArray()
            ?? Array.Empty<IONSPostcode>();

        public IReadOnlyCollection<IDevolvedPostcode> GetDevolvedPostcodes(string fromPostcode) =>
            _devolvedPostcodes.GetValueOrDefault(fromPostcode);
    }
}
