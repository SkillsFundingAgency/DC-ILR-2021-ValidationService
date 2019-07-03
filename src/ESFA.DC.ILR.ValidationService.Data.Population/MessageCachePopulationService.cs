using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Cache;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population
{
    public class MessageCachePopulationService : IMessageCachePopulationService
    {
        private readonly ICache<IMessage> _messageCache;

        public MessageCachePopulationService(
            ICache<IMessage> messageCache)
        {
            _messageCache = messageCache;
        }

        public void Populate(IMessage message)
        {
            var messageCache = (Cache<IMessage>)_messageCache;
            messageCache.Item = message;
        }
    }
}
