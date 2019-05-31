using Autofac;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Modules;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Modules;
using ESFA.DC.ILR.ValidationService.Rules.Modules;

namespace ESFA.DC.ILR.ValidationService.Desktop.Modules
{
    public class ValidationServiceDesktopModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<ValidationServicesModule>();
            builder.RegisterModule<DataServicesModule>();
            builder.RegisterModule<RuleSetModule<IMessage>>();
            builder.RegisterModule<RuleSetModule<ILearner>>();
            builder.RegisterModule<RuleSetModule<ILearnerDestinationAndProgression>>();
            builder.RegisterModule<DataCacheModule>();
            builder.RegisterModule<QueryServiceModule>();
            builder.RegisterModule<DerivedDataModule>();
            builder.RegisterModule<ProvideLookupModule>();
        }
    }
}
