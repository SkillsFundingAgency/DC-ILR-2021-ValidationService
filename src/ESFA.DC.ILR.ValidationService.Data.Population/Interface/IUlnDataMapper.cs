using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Interface
{
    public interface IUlnDataMapper
    {
        IReadOnlyCollection<long> MapUlns(IReadOnlyCollection<long> ulns);
    }
}
