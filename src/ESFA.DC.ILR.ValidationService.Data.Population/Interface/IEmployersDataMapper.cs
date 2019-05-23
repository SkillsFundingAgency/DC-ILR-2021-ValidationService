using System.Collections.Generic;
using ESFA.DC.ILR.ReferenceDataService.Model.Employers;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Interface
{
    public interface IEmployersDataMapper
    {
        IReadOnlyCollection<int> MapEmployers(IReadOnlyCollection<Employer> employers);
    }
}
