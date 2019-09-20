using System;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Cache;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;

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
            var messageRoot = BuildMessage(message);
            var messageCache = (Cache<IMessage>)_messageCache;

            messageCache.Item = messageRoot;
        }

        public virtual IMessage BuildMessage(IMessage message)
        {
            var messageRoot = (Message)message;
            return new Message
            {
                Header = messageRoot.Header,
                SourceFiles = messageRoot.SourceFiles,
                LearningProvider = messageRoot.LearningProvider,
                Learner = messageRoot.Learner ?? Array.Empty<MessageLearner>(),
                LearnerDestinationandProgression = messageRoot.LearnerDestinationandProgression ?? Array.Empty<MessageLearnerDestinationandProgression>()
            };
        }
    }
}
