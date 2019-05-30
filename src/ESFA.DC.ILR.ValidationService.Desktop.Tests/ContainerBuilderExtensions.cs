using Autofac;
using ESFA.DC.DateTimeProvider.Interface;

namespace ESFA.DC.ILR.ValidationService.Desktop.Tests
{
    public static class ContainerBuilderExtensions
    {
        public static void RegisterCommonServiceStubs(this ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterMock<IDateTimeProvider>();
            containerBuilder.RegisterMock<ILogger>();
            //Logger
            //containerBuilder.RegisterType<FileSystemFileService>().As<IFileService>();
            //containerBuilder.RegisterType<FileSystemKeyValuePersistenceService>()
            //    .As<IKeyValuePersistenceService>()
            //    .As<IStreamableKeyValuePersistenceService>();

            //containerBuilder.RegisterType<DecompressionService>().As<IDecompressionService>();

            //var fileSystemKeyValuePersistenceServiceConfiguration = new FileSystemKeyValuePersistenceServiceConfigStub()
            //{
            //    Directory = "Sandbox"
            //};

            //containerBuilder.RegisterInstance(fileSystemKeyValuePersistenceServiceConfiguration).As<IFileSystemKeyValuePersistenceServiceConfig>();

            //containerBuilder.RegisterType<XmlSerializationService>().As<IXmlSerializationService>();
            //containerBuilder.RegisterType<JsonSerializationService>().As<IJsonSerializationService>().As<ISerializationService>();
        }

        public static void RegisterMock<T>(this ContainerBuilder containerBuilder)
            where T : class
        {
            var mock = Mock.Of<T>();

            containerBuilder.RegisterInstance(mock).As<T>();
        }
    }
}
