using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Mappers
{
    public class UlnDataMapper : IUlnDataMapper
    {
        public UlnDataMapper()
        {
        }

        public IReadOnlyCollection<long> MapUlns(IReadOnlyCollection<long> ulns)
        {
            return ulns ?? new List<long>();
        }
    }
}
