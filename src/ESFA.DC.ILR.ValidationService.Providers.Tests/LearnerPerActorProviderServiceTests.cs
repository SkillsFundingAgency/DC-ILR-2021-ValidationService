using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.Tests.Model;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Providers.Tests
{
    public class LearnerPerActorProviderServiceTests
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
                Learner = testLearners,
                LearnerDestinationandProgression = testLearnerDP,
                LearningProvider = new MessageLearningProvider { UKPRN = 12345678 },
            };
            
            var messages = new List<IMessage>
            {
                message
            };
            
            var learnerPerActorProviderService = new LearnerPerActorProviderService();

            learnerPerActorProviderService.Provide(message).Should().BeEquivalentTo(messages);
        }

        [Fact]
        public async Task ProvideAsync_LearnerDPMismatch()
        {
            var testLearners = new MessageLearner[]
            {
                new MessageLearner { LearnRefNumber = "Learner1" },
                new MessageLearner { LearnRefNumber = "Learner2" },
            };

            var testLearnerDP = new MessageLearnerDestinationandProgression[]
            {
                new MessageLearnerDestinationandProgression { LearnRefNumber = "Learner1" },
                new MessageLearnerDestinationandProgression { LearnRefNumber = "Learner3" },
            };

            IMessage message = new Message
            {
                Header = new MessageHeader(),
                Learner = testLearners,
                LearnerDestinationandProgression = testLearnerDP,
                LearningProvider = new MessageLearningProvider { UKPRN = 12345678 },
            };

            var learnerPerActorProviderService = new LearnerPerActorProviderService();

            var lpa = learnerPerActorProviderService.Provide(message);

            lpa.Select(m => m).Should().HaveCount(1);
            lpa.SelectMany(m => m.Learners).Should().HaveCount(2);
            lpa.SelectMany(m => m.LearnerDestinationAndProgressions).Should().HaveCount(1);
        }

        [Fact]
        public async Task ProvideAsync_LearnerDPNoMatch()
        {
            var testLearners = new MessageLearner[]
            {
                new MessageLearner { LearnRefNumber = "Learner1" },
                new MessageLearner { LearnRefNumber = "Learner2" },
            };

            var testLearnerDP = new MessageLearnerDestinationandProgression[]
            {
                new MessageLearnerDestinationandProgression { LearnRefNumber = "Learner3" },
                new MessageLearnerDestinationandProgression { LearnRefNumber = "Learner4" },
            };

            IMessage message = new Message
            {
                Header = new MessageHeader(),
                Learner = testLearners,
                LearnerDestinationandProgression = testLearnerDP,
                LearningProvider = new MessageLearningProvider { UKPRN = 12345678 },
            };
            
            var learnerPerActorProviderService = new LearnerPerActorProviderService();

            var lpa = learnerPerActorProviderService.Provide(message);

            lpa.Select(m => m).Should().HaveCount(1);
            lpa.SelectMany(m => m.Learners).Should().HaveCount(2);
            lpa.SelectMany(m => m.LearnerDestinationAndProgressions).Should().HaveCount(0);
        }

        [Fact]
        public async Task ProvideAsync_ZeroDPRecords()
        {
            var testLearners = new MessageLearner[]
            {
                new MessageLearner { LearnRefNumber = "Learner1" },
                new MessageLearner { LearnRefNumber = "Learner2" },
            };

            IMessage message = new Message
            {
                Header = new MessageHeader(),
                Learner = testLearners,
                LearningProvider = new MessageLearningProvider { UKPRN = 12345678 },
            };

            var learnerPerActorProviderService = new LearnerPerActorProviderService();

            var lpa = learnerPerActorProviderService.Provide(message);

            lpa.Select(m => m).Should().HaveCount(1);
            lpa.SelectMany(m => m.Learners).Should().HaveCount(2);
            lpa.SelectMany(m => m.LearnerDestinationAndProgressions).Should().HaveCount(0);
        }


        [Fact]
        public async Task ProvideAsync_MultipleShards()
        {
            var testLearners = new List<MessageLearner>();

            for (var i = 0; i < 1001; i++)
            {
                testLearners.Add(new MessageLearner());
            };

            var testLearnerDP = new MessageLearnerDestinationandProgression[]
            {
                new MessageLearnerDestinationandProgression { LearnRefNumber = "Learner1" },
                new MessageLearnerDestinationandProgression { LearnRefNumber = "Learner2" },
            };

            IMessage message = new Message
            {
                Header = new MessageHeader(),
                Learner = testLearners.ToArray(),
                LearnerDestinationandProgression = testLearnerDP,
                LearningProvider = new MessageLearningProvider { UKPRN = 12345678 },
            };
            
            var messageCacheMock = new Mock<ICache<IMessage>>();
            messageCacheMock.SetupGet(mc => mc.Item).Returns(message);
            
            var lpa = new LearnerPerActorProviderService().Provide(message).ToArray();

            lpa.Select(m => m).Should().HaveCount(2);
            lpa[0].Learners.Should().HaveCount(1000);
            lpa[1].Learners.Should().HaveCount(1);
        }
    }
}
