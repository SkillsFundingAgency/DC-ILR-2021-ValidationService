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
            builder.RegisterType<ValidationErrorCache>().As<IValidationErrorCache>();
            builder.RegisterType<ValidationErrorHandler>().As<IValidationErrorHandler>();

            builder.RegisterType<ValidationOutputService>().As<IValidationOutputService>().InstancePerLifetimeScope();
        }
    }
}
