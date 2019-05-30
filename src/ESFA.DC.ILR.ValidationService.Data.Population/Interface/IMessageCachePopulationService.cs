using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Interface
{
    public interface IMessageCachePopulationService
    {
        void Populate(IMessage message);
    }
}
