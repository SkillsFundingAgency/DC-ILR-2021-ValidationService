using Autofac;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.FileService;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.IO.Dictionary;
using ESFA.DC.IO.Interfaces;

namespace ESFA.DC.ILR.ValidationService.Modules
{
    public class BaseDataModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<InternalDataCachePopulationService>().As<IInternalDataCachePopulationService>().InstancePerLifetimeScope();
            builder.RegisterType<ExternalDataCachePopulationService>().As<IExternalDataCachePopulationService>().InstancePerLifetimeScope();
            builder.RegisterType<FileDataCachePopulationService>().As<IFileDataCachePopulationService>().InstancePerLifetimeScope();
            builder.RegisterType<ReferenceDataCachePopulationService>().As<IReferenceDataCachePopulationService>().InstancePerLifetimeScope();
            builder.RegisterType<MessageCachePopulationService>().As<IMessageCachePopulationService>().InstancePerLifetimeScope();

            builder.RegisterType<DateTimeProvider.DateTimeProvider>().As<IDateTimeProvider>().InstancePerLifetimeScope();

            builder.RegisterType<DictionaryKeyValuePersistenceService>().As<IKeyValuePersistenceService>().InstancePerLifetimeScope();

            builder.RegisterType<AzureStorageFileService>().As<IFileService>().InstancePerLifetimeScope();

            builder.RegisterModule<CacheModule>();
            builder.RegisterModule<DataServiceModule>();
        }
    }
}
