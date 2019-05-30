using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Cache;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Tests
{
    public class MessageCachePopulationServiceTests
    {
        [Fact]
        public void Populate()
        {
            var message = new Mock<IMessage>().Object;

            var messageCacheMock = new Mock<Cache<IMessage>>();

            messageCacheMock.SetupSet(mc => mc.Item = message).Verifiable();

            NewService(messageCacheMock.Object).Populate(message);

            messageCacheMock.Verify();
        }

        private MessageCachePopulationService NewService(ICache<IMessage> messageCache = null)
        {
            return new MessageCachePopulationService(messageCache);
        }
    }
}
