using Autofac;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ServiceFabric.Helpers;

namespace ESFA.DC.ILR.ValidationService.Modules.Stateless
{
    public class PreValidationDataModule : BaseDataModule
    {
        protected override void Load(ContainerBuilder builder)
        {
            var configHelper = new ConfigurationHelper();

            builder.RegisterType<PreValidationPopulationService>().As<IPopulationService>().InstancePerLifetimeScope();

            base.Load(builder);
        }
    }
}
