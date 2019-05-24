using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Providers
{
    public class LearnerProviderService : IValidationItemProviderService<IEnumerable<ILearner>>
    {
        private readonly ICache<IMessage> _messageCache;

        public LearnerProviderService(ICache<IMessage> messageCache)
        {
            _messageCache = messageCache;
        }

        public async Task<IEnumerable<ILearner>> ProvideAsync(IValidationContext validationContext, CancellationToken cancellationToken)
        {
            return _messageCache.Item.Learners;
        }
    }
}
