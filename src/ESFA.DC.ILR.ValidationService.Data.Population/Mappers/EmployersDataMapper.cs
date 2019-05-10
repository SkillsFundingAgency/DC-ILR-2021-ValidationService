using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.ReferenceDataService.Model.Employers;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Mappers
{
    public class EmployersDataMapper : IEmployersDataMapper
    {
        public IReadOnlyCollection<int> MapEmployers(IReadOnlyCollection<Employer> employers)
        {
            return employers?.Select(e => e.ERN).ToList();
        }
    }
}
