using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Cache;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Tests
{
    public class MessageCachePopulationServiceTests
    {
        [Fact]
        public void Populate()
        {
            IMessage message = new Message();
            var castMessage = (Message)message;

            var messageCacheMock = new Mock<Cache<IMessage>>();
            messageCacheMock.SetupSet(mc => mc.Item = castMessage).Verifiable();

            var service = NewMockService(messageCacheMock.Object);
            service.Setup(x => x.BuildMessage(message)).Returns(castMessage).Verifiable();

            service.Object.Populate(message);

            messageCacheMock.VerifyAll();
        }

        private Mock<MessageCachePopulationService> NewMockService(ICache<IMessage> messageCache = null)
        {
            return new Mock<MessageCachePopulationService>(messageCache);
        }
    }
}
