using Autofac;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.FileService.Interface;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using Moq;

namespace ESFA.DC.ILR.ValidationService.Desktop.Tests
{
    public static class ContainerBuilderExtensions
    {
        public static void RegisterCommonServiceStubs(this ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterMock<IDateTimeProvider>();
            containerBuilder.RegisterMock<ILogger>();
            containerBuilder.RegisterMock<IFileService>();
            containerBuilder.RegisterMock<IKeyValuePersistenceService>();
            containerBuilder.RegisterMock<IStreamableKeyValuePersistenceService>();
            containerBuilder.RegisterMock<IDecompressionService>();
            containerBuilder.RegisterMock<IXmlSerializationService>();
            containerBuilder.RegisterMock<IJsonSerializationService>();
            containerBuilder.RegisterMock<ISerializationService>();
        }

        private static void RegisterMock<T>(this ContainerBuilder containerBuilder)
            where T : class
        {
            var mock = Mock.Of<T>();

            containerBuilder.RegisterInstance(mock).As<T>();
        }
    }
}
