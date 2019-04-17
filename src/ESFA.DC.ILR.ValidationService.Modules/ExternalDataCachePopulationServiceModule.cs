using Autofac;
using ESFA.DC.ILR.ValidationService.Data.Population.External;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;

namespace ESFA.DC.ILR.ValidationService.Modules
{
    public class ExternalDataCachePopulationServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ExternalDataCachePopulationService>().As<IExternalDataCachePopulationService>().InstancePerLifetimeScope();

            builder.RegisterType<LARSStandardDataRetrievalService>().As<ILARSStandardDataRetrievalService>().InstancePerLifetimeScope();
            builder.RegisterType<LARSStandardValidityDataRetrievalService>().As<ILARSStandardValidityDataRetrievalService>().InstancePerLifetimeScope();
            builder.RegisterType<LARSLearningDeliveryDataRetrievalService>().As<ILARSLearningDeliveryDataRetrievalService>().InstancePerLifetimeScope();
            builder.RegisterType<LARSFrameworkDataRetrievalService>().As<ILARSFrameworkDataRetrievalService>().InstancePerLifetimeScope();
        }
    }
}
