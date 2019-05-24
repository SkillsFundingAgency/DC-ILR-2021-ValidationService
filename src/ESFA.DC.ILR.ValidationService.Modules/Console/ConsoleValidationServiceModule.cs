using Autofac;
using ESFA.DC.ILR.ValidationService.Modules.Actor;
using ESFA.DC.ILR.ValidationService.RuleSet.Modules;

namespace ESFA.DC.ILR.ValidationService.Modules.Console
{
    public class ConsoleValidationServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<ConsoleValidationOrchestrationModule>();
            builder.RegisterModule<BaseDataModule>();
            builder.RegisterModule<LearnerRuleSetModule>();
            builder.RegisterModule<MessageRuleSetModule>();

            builder.RegisterModule<DataServiceModule>();
            builder.RegisterModule<ConsoleCachePopulationModule>();
            builder.RegisterModule<CacheModule>();
        }
    }
}
