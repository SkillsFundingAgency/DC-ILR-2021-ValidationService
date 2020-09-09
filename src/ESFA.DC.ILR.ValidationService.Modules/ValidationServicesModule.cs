using Autofac;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Providers.Output;
using ESFA.DC.ILR.ValidationService.RuleSet;
using ESFA.DC.ILR.ValidationService.RuleSet.ErrorHandler;

namespace ESFA.DC.ILR.ValidationService.Modules
{
    public class ValidationServicesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {

            builder.RegisterType<RuleSetOrchestrationService<ICrossYearRule<ILearner>, ILearner>>().As<IRuleSetOrchestrationService<ICrossYearRule<ILearner>, ILearner>>();
            builder.RegisterType<RuleSetOrchestrationService<IRule<IMessage>, IMessage>>().As<IRuleSetOrchestrationService<IRule<IMessage>, IMessage>>();
            builder.RegisterType<RuleSetOrchestrationService<IRule<ILearner>, ILearner>>().As<IRuleSetOrchestrationService<IRule<ILearner>, ILearner>>();
            builder.RegisterType<RuleSetOrchestrationService<IRule<ILearnerDestinationAndProgression>, ILearnerDestinationAndProgression>>().As<IRuleSetOrchestrationService<IRule<ILearnerDestinationAndProgression>, ILearnerDestinationAndProgression>>();

            builder.RegisterType<RuleSetResolutionService<ICrossYearRule<ILearner>, ILearner>>().As<IRuleSetResolutionService<ICrossYearRule<ILearner>, ILearner>>();
            builder.RegisterType<RuleSetResolutionService<IRule<IMessage>, IMessage>>().As<IRuleSetResolutionService<IRule<IMessage>, IMessage>>();
            builder.RegisterType<RuleSetResolutionService<IRule<ILearner>, ILearner>>().As<IRuleSetResolutionService<IRule<ILearner>, ILearner>>();
            builder.RegisterType<RuleSetResolutionService<IRule<ILearnerDestinationAndProgression>, ILearnerDestinationAndProgression>>().As<IRuleSetResolutionService<IRule<ILearnerDestinationAndProgression>, ILearnerDestinationAndProgression>>();

            builder.RegisterType<RuleSetExecutionService<ICrossYearRule<ILearner>, ILearner>>().As<IRuleSetExecutionService<ICrossYearRule<ILearner>, ILearner>>();
            builder.RegisterType<RuleSetExecutionService<IRule<IMessage>, IMessage>>().As<IRuleSetExecutionService<IRule<IMessage>, IMessage>>();
            builder.RegisterType<RuleSetExecutionService<IRule<ILearner>, ILearner>>().As<IRuleSetExecutionService<IRule<ILearner>, ILearner>>();
            builder.RegisterType<RuleSetExecutionService<IRule<ILearnerDestinationAndProgression>, ILearnerDestinationAndProgression>>().As<IRuleSetExecutionService<IRule<ILearnerDestinationAndProgression>, ILearnerDestinationAndProgression>>();

            builder.RegisterType<ValidationErrorCache>().As<IValidationErrorCache>().InstancePerLifetimeScope();
            builder.RegisterType<ValidationErrorHandler>().As<IValidationErrorHandler>().InstancePerLifetimeScope();

            builder.RegisterType<ValidationOutputService>().As<IValidationOutputService>().InstancePerLifetimeScope();
            builder.RegisterType<ValidIlrFileOutputService>().As<IValidIlrFileOutputService>().InstancePerLifetimeScope();
        }
    }
}
