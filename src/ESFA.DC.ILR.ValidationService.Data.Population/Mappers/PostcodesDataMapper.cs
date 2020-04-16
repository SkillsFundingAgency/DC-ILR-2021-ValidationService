using ESFA.DC.ILR.ReferenceDataService.Model.Postcodes;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;

using System;
using System.Collections.Generic;
using System.Linq;
using DevolvedPostcodeRDS = ESFA.DC.ILR.ReferenceDataService.Model.PostcodesDevolution.DevolvedPostcode;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Mappers
{
    public class PostcodesDataMapper : IPostcodesDataMapper
    {
        public IReadOnlyCollection<string> MapPostcodes(IReadOnlyCollection<Postcode> postcodes)
        {
            return new HashSet<string>(postcodes?.Select(p => p.PostCode).ToList(), StringComparer.OrdinalIgnoreCase);
        }

        public IReadOnlyCollection<ONSPostcode> MapONSPostcodes(IReadOnlyCollection<Postcode> postcodes)
        {
            List<ONSPostcode> onsPostcodes = new List<ONSPostcode>();

            foreach (var postcode in postcodes.Where(o => o.ONSData != null))
            {
                onsPostcodes.AddRange(postcode?.ONSData?.Select(o => new ONSPostcode
                {
                    Postcode = postcode.PostCode,
                    EffectiveFrom = o.EffectiveFrom,
                    EffectiveTo = o.EffectiveTo,
                    Lep1 = o.Lep1,
                    Lep2 = o.Lep2,
                    LocalAuthority = o.LocalAuthority,
                    Nuts = o.Nuts,
                    Termination = o.Termination
                }));
            }

            return onsPostcodes;
        }

        public IReadOnlyDictionary<string, IReadOnlyCollection<IDevolvedPostcode>> MapDevolvedPostcodes(IReadOnlyCollection<DevolvedPostcodeRDS> postcodes)
        {
            return postcodes?
                .GroupBy(p => p.Postcode, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(
                    k => k.Key,
                    v => v.Select(dp => new DevolvedPostcode
                    {
                        Postcode = dp.Postcode,
                        Area = dp.Area,
                        SourceOfFunding = dp.SourceOfFunding,
                        EffectiveFrom = dp.EffectiveFrom,
                        EffectiveTo = dp.EffectiveTo
                    }).ToReadOnlyCollection<IDevolvedPostcode>(),
                    StringComparer.OrdinalIgnoreCase)
                        ?? new Dictionary<string, IReadOnlyCollection<IDevolvedPostcode>>(StringComparer.OrdinalIgnoreCase);
        }
    }
}
