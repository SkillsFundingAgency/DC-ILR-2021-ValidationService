using System;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.External.EPAOrganisation.Interface
{
    public interface IEPAOrganisationDataService : IDataService
    {
        bool IsValidEpaOrg(string epaOrgId, int? stdCode, DateTime learnPlanEndDate);
    }
}
