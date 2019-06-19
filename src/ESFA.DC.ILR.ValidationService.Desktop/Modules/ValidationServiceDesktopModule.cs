using Autofac;
using ESFA.DC.ILR.Desktop.Interface;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Modules;
using ESFA.DC.ILR.ValidationService.Data.Population.Modules;
using ESFA.DC.ILR.ValidationService.Desktop.Context;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Modules;
using ESFA.DC.ILR.ValidationService.Providers;
using ESFA.DC.ILR.ValidationService.Rules.Modules;

namespace ESFA.DC.ILR.ValidationService.Desktop.Modules
{
    public class ValidationServiceDesktopModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<DataCacheModule>();
            builder.RegisterModule<DataMappersModule>();
            builder.RegisterModule<DataPopulationModule>();
            builder.RegisterModule<DataServicesModule>();
            builder.RegisterModule<DerivedDataModule>();
            builder.RegisterModule<RuleSetModule<ILearner>>();
            builder.RegisterModule<RuleSetModule<ILearnerDestinationAndProgression>>();
            builder.RegisterModule<RuleSetModule<IMessage>>();
            builder.RegisterModule<QueryServiceModule>();
            builder.RegisterModule<ValidationServicesModule>();

            builder.RegisterType<PreValidationOrchestrationSfService>().As<IPreValidationOrchestrationService>().InstancePerLifetimeScope();
            builder.RegisterType<ValidationExecutionProvider>().As<IValidationExecutionProvider>().InstancePerLifetimeScope();
            builder.RegisterType<DesktopContextValidationContextFactory>().As<IValidationContextFactory<IDesktopContext>>();
            builder.RegisterType<EnabledRulesProvider>().As<IEnabledRulesProvider>().InstancePerLifetimeScope();
        }
    }
}
