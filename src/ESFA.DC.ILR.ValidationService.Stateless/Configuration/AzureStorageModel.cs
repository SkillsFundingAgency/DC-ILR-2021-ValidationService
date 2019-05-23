using ESFA.DC.IO.AzureStorage.Config.Interfaces;

namespace ESFA.DC.ILR.ValidationService.Stateless.Models
{
    // TODO: refactor this and use the storage config properly everywhere
    public class AzureStorageModel : IAzureStorageKeyValuePersistenceServiceConfig
    {
        public string AzureBlobConnectionString { get; set; }

        public string AzureContainerReference { get; set; }

        public string ConnectionString => AzureBlobConnectionString;

        public string ContainerName => AzureContainerReference;
    }
}
