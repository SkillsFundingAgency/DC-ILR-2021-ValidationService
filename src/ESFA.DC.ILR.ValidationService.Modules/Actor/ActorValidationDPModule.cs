using Autofac;
using ESFA.DC.ILR.ValidationService.RuleSet.Modules;

namespace ESFA.DC.ILR.ValidationService.Modules.Actor
{
    public class ActorValidationDPModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<ActorValidationOrchestrationModule>();
            builder.RegisterModule<BaseDataModule>();
            builder.RegisterModule<LearnerDPRuleSetModule>();

            builder.RegisterModule<DataServiceModule>();
        }
    }
}
