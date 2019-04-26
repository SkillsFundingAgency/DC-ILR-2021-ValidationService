using Autofac;
using ESFA.DC.FileService;
using ESFA.DC.FileService.Config.Interface;
using ESFA.DC.FileService.Interface;
using ESFA.DC.IO.AzureStorage;
using ESFA.DC.IO.AzureStorage.Config.Interfaces;
using ESFA.DC.IO.Interfaces;

namespace ESFA.DC.ILR.ReferenceDataService.Modules
{
    public class IOModule : Module
    {
        private readonly IAzureStorageFileServiceConfiguration _azureStorageFileServiceConfig;
        private readonly IAzureStorageKeyValuePersistenceServiceConfig _azureStorageKeyValuePersistenceServiceConfig;

        public IOModule(IAzureStorageFileServiceConfiguration azureStorageFileServiceConfig, IAzureStorageKeyValuePersistenceServiceConfig azureStorageKeyValuePersistenceServiceConfig)
        {
            _azureStorageFileServiceConfig = azureStorageFileServiceConfig;
            _azureStorageKeyValuePersistenceServiceConfig = azureStorageKeyValuePersistenceServiceConfig;
        }

        protected override void Load(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterInstance(_azureStorageFileServiceConfig).As<IAzureStorageFileServiceConfiguration>();
            containerBuilder.RegisterInstance(_azureStorageKeyValuePersistenceServiceConfig).As<IAzureStorageKeyValuePersistenceServiceConfig>();

            containerBuilder.RegisterType<AzureStorageFileService>().As<IFileService>();
            containerBuilder.RegisterType<DecompressionService>().As<IDecompressionService>();
            containerBuilder.RegisterType<AzureStorageKeyValuePersistenceService>().As<IKeyValuePersistenceService>();
        }
    }
}
