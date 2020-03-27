using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Providers
{
    public class LearnerPerActorProviderService : ILearnerPerActorProviderService
    {
        private const int LearnersPerActor = 1000;
        
        public IEnumerable<IMessage> Provide(IMessage fullMessage)
        {
            if (fullMessage?.Learners == null)
            {
                return null;
            }

            var learnerShards = fullMessage.Learners.SplitList(LearnersPerActor);

            // create IMessage shards with learners
            var messageShards = new List<IMessage>();
            var msg = fullMessage as Message;
            foreach (var learnerShard in learnerShards)
            {
                var learnRefNumbers = learnerShard.Select(l => l.LearnRefNumber).ToCaseInsensitiveHashSet();

                // shallow duplication is sufficient except for the learners
                Message messageShard = new Message
                {
                    Header = msg.Header,
                    LearnerDestinationandProgression = msg.LearnerDestinationandProgression?
                    .Where(ldp => learnRefNumbers.Contains(ldp.LearnRefNumber)).ToArray() ?? Array.Empty<MessageLearnerDestinationandProgression>(),
                    LearningProvider = msg.LearningProvider,
                    SourceFiles = msg.SourceFiles,
                    Learner = learnerShard.Cast<MessageLearner>().ToArray()
                };

                messageShards.Add(messageShard);
            }

            return messageShards;
        }
    }
}
