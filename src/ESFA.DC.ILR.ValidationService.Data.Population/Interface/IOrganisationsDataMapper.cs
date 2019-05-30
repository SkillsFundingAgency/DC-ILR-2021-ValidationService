using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Model;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Interface
{
    public interface IOrganisationsDataMapper : IMapper
    {
        IReadOnlyDictionary<long, Organisation> MapOrganisations(IReadOnlyCollection<ReferenceDataService.Model.Organisations.Organisation> organisations);

        IReadOnlyCollection<ICampusIdentifier> MapCampusIdentifiers(IReadOnlyCollection<ReferenceDataService.Model.Organisations.Organisation> organisations);
    }
}
