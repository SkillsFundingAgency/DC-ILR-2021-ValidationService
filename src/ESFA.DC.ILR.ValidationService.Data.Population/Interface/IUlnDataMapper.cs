using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Interface
{
    public interface IUlnDataMapper : IMapper
    {
        IReadOnlyCollection<long> MapUlns(IReadOnlyCollection<long> ulns);
    }
}
