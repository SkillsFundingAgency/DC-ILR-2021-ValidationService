using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Mappers
{
    public class UlnDataMapper
    {
        public UlnDataMapper()
        {
        }

        public IReadOnlyCollection<long> MapUlns(IReadOnlyCollection<long> ulns)
        {
            return ulns;
        }
    }
}
