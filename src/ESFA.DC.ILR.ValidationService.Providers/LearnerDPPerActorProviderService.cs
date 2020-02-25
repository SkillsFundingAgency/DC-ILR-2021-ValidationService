using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Providers
{
    public class LearnerDPPerActorProviderService : ILearnerDPPerActorProviderService
    {
        private const int LearnersPerActor = 1000;

        public IEnumerable<IMessage> Provide(IMessage message)
        {
            if (message?.LearnerDestinationAndProgressions == null)
            {
                return null;
            }

            var learnerDPShards = message.LearnerDestinationAndProgressions.SplitList(LearnersPerActor);

            // create IMessage shards with learners
            var messageShards = new List<IMessage>();

            var localMessage = message as Message;

            foreach (var learnerDPShard in learnerDPShards)
            {
                // shallow duplication is sufficient except for the learners
                Message shardedMessage = new Message
                {
                    Header = localMessage.Header,
                    LearnerDestinationandProgression = learnerDPShard.Cast<MessageLearnerDestinationandProgression>().ToArray(),
                    LearningProvider = localMessage.LearningProvider,
                    SourceFiles = localMessage.SourceFiles
                };

                messageShards.Add(shardedMessage);
            }

            return messageShards;
        }
    }
}
