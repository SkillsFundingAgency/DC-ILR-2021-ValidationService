using System.Collections.Generic;
using ESFA.DC.ILR.ReferenceDataService.Model.EPAOrganisations;
using ESFA.DC.ILR.ValidationService.Data.External.EPAOrganisation.Model;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Interface
{
    public interface IEpaOrgDataMapper
    {
        IReadOnlyDictionary<string, List<EPAOrganisations>> MapEpaOrganisations(IReadOnlyCollection<EPAOrganisation> epaOrganisations);
    }
}
