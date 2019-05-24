using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Providers
{
    public class MessageProviderService : IValidationItemProviderService<IEnumerable<IMessage>>
    {
        private readonly ICache<IMessage> _messageCache;

        public MessageProviderService(ICache<IMessage> messageCache)
        {
            _messageCache = messageCache;
        }

        public async Task<IEnumerable<IMessage>> ProvideAsync(IValidationContext validationContext, CancellationToken cancellationToken)
        {
            return new List<IMessage>
            {
                _messageCache.Item
            };
        }
    }
}
