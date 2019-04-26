using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ReferenceDataService.Model.EPAOrganisations;
using ESFA.DC.ILR.ValidationService.Data.External.EPAOrganisation.Model;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Mappers
{
    public class EpaOrgDataMapper : IEpaOrgDataMapper
    {
        public EpaOrgDataMapper()
        {
        }

        public IReadOnlyDictionary<string, List<EPAOrganisations>> MapEpaOrganisations(IReadOnlyCollection<EPAOrganisation> epaOrganisations)
        {
            return epaOrganisations?
                .GroupBy(e => e.ID)
                .ToDictionary(
                k => k.Key,
                v => v.Select(epa => new EPAOrganisations
                {
                    ID = epa.ID,
                    Standard = epa.Standard,
                    EffectiveFrom = epa.EffectiveFrom,
                    EffectiveTo = epa.EffectiveTo
                }).ToList(), StringComparer.OrdinalIgnoreCase);
        }
    }
}
