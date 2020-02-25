using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.Model.Interface;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Providers.Tests
{
    public class LearnerDPPerActorProviderServiceTests
    {
        [Fact]
        public async Task ProvideAsync()
        {
            var testLearners = new MessageLearner[]
            {
                new MessageLearner { LearnRefNumber = "Learner1" },
                new MessageLearner { LearnRefNumber = "Learner2" },
            };

            var testLearnerDP = new MessageLearnerDestinationandProgression[]
            {
                new MessageLearnerDestinationandProgression { LearnRefNumber = "Learner1" },
                new MessageLearnerDestinationandProgression { LearnRefNumber = "Learner2" },
            };

            IMessage message = new Message
            {
                Header = new MessageHeader(),
                LearnerDestinationandProgression = testLearnerDP,
                LearningProvider = new MessageLearningProvider { UKPRN = 12345678 },
            };


            var messages = new List<IMessage>
            {
                message
            };

            var learnerDPPerActorProviderServiceMock = new LearnerDPPerActorProviderService();

            learnerDPPerActorProviderServiceMock.Provide(message).Should().BeEquivalentTo(messages);
        }

        [Fact]
        public async Task ProvideAsync_MultipleShards()
        {
            var testLearnerDP = new List<MessageLearnerDestinationandProgression>();

            for (var i = 0; i < 1001; i++)
            {
                testLearnerDP.Add(new MessageLearnerDestinationandProgression());
            }

            IMessage message = new Message
            {
                Header = new MessageHeader(),
                LearnerDestinationandProgression = testLearnerDP.ToArray(),
                LearningProvider = new MessageLearningProvider { UKPRN = 12345678 },
            };
            
            var lpa = new LearnerDPPerActorProviderService().Provide(message).ToArray();

            lpa.Select(m => m).Should().HaveCount(2);
            lpa[0].Learners.Should().BeNullOrEmpty();
            lpa[1].Learners.Should().BeNullOrEmpty();
            lpa[0].LearnerDestinationAndProgressions.Should().HaveCount(1000);
            lpa[1].LearnerDestinationAndProgressions.Should().HaveCount(1);
        }
    }
}
