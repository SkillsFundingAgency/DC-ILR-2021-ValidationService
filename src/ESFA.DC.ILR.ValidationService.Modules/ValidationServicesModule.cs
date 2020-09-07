using Autofac;
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
            builder.RegisterGeneric(typeof(RuleSetOrchestrationService<>)).As(typeof(IRuleSetOrchestrationService<>));
            builder.RegisterGeneric(typeof(AutoFacRuleSetResolutionService<>)).As(typeof(IRuleSetResolutionService<>));
            builder.RegisterGeneric(typeof(RuleSetExecutionService<>)).As(typeof(IRuleSetExecutionService<>));
            builder.RegisterGeneric(typeof(CrossYearRuleSetOrchestrationService<>)).As(typeof(ICrossYearRuleSetOrchestrationService<>));
            builder.RegisterGeneric(typeof(CrossYearAutoFacRuleSetResolutionService<>)).As(typeof(ICrossYearRuleSetResolutionService<>));
            builder.RegisterGeneric(typeof(CrossYearRuleSetExecutionService<>)).As(typeof(ICrossYearRuleSetExecutionService<>));
            builder.RegisterType<ValidationErrorCache>().As<IValidationErrorCache>().InstancePerLifetimeScope();
            builder.RegisterType<ValidationErrorHandler>().As<IValidationErrorHandler>().InstancePerLifetimeScope();

            builder.RegisterType<ValidationOutputService>().As<IValidationOutputService>().InstancePerLifetimeScope();
            builder.RegisterType<ValidIlrFileOutputService>().As<IValidIlrFileOutputService>().InstancePerLifetimeScope();
        }
    }
}
