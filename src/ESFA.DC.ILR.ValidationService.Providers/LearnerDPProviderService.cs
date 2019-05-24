using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Providers
{
    public class LearnerDPProviderService : IValidationItemProviderService<IEnumerable<ILearnerDestinationAndProgression>>
    {
        private readonly ICache<IMessage> _messageCache;

        public LearnerDPProviderService(ICache<IMessage> messageCache)
        {
            _messageCache = messageCache;
        }

        public async Task<IEnumerable<ILearnerDestinationAndProgression>> ProvideAsync(IValidationContext validationContext, CancellationToken cancellationToken)
        {
            return _messageCache.Item.LearnerDestinationAndProgressions;
        }
    }
}
