using Autofac;
using ESFA.DC.ILR.ValidationService.Rules.Query;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.ValidationActor.Modules
{
    internal class QueryServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AcademicYearQueryService>().As<IAcademicYearQueryService>().InstancePerLifetimeScope();
            builder.RegisterType<DateTimeQueryService>().As<IDateTimeQueryService>().InstancePerLifetimeScope();
            builder.RegisterType<LearningDeliveryQueryService>().As<ILearningDeliveryQueryService>().InstancePerLifetimeScope();
            builder.RegisterType<LearnerQueryService>().As<ILearnerQueryService>().InstancePerLifetimeScope();
            builder.RegisterType<LearnerDPQueryService>().As<ILearnerDPQueryService>().InstancePerLifetimeScope();
            builder.RegisterType<LearnerFAMQueryService>().As<ILearnerFAMQueryService>().InstancePerLifetimeScope();
            builder.RegisterType<LearningDeliveryFAMQueryService>().As<ILearningDeliveryFAMQueryService>().InstancePerLifetimeScope();
            builder.RegisterType<LearningDeliveryAppFinRecordQueryService>().As<ILearningDeliveryAppFinRecordQueryService>().InstancePerLifetimeScope();
            builder.RegisterType<LearnerEmploymentStatusQueryService>().As<ILearnerEmploymentStatusQueryService>().InstancePerLifetimeScope();
            builder.RegisterType<LearnerEmploymentStatusMonitoringQueryService>().As<ILearnerEmploymentStatusMonitoringQueryService>().InstancePerLifetimeScope();
            builder.RegisterType<PostcodeQueryService>().As<IPostcodeQueryService>().InstancePerLifetimeScope();
            builder.RegisterType<LearningDeliveryWorkPlacementQueryService>().As<ILearningDeliveryWorkPlacementQueryService>().InstancePerLifetimeScope();

            builder.RegisterType<RuleCommonOperationsProvider>().As<IProvideRuleCommonOperations>().InstancePerLifetimeScope();
            builder.RegisterType<LearnAimRefRuleActionProvider>().As<IProvideLearnAimRefRuleActions>().InstancePerLifetimeScope();
        }
    }
}
