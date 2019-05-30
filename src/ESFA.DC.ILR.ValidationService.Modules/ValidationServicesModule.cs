using System.Collections.Generic;
using Autofac;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.RuleSet;
using ESFA.DC.ILR.ValidationService.RuleSet.ErrorHandler;

namespace ESFA.DC.ILR.ValidationService.Modules
{
    public class ValidationServicesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(RuleSetOrchestrationService<,>)).As(typeof(IRuleSetOrchestrationService<,>));
            builder.RegisterGeneric(typeof(AutoFacRuleSetResolutionService<>)).As(typeof(IRuleSetResolutionService<>));
            builder.RegisterGeneric(typeof(RuleSetExecutionService<>)).As(typeof(IRuleSetExecutionService<>));
            builder.RegisterGeneric(typeof(ValidationErrorCache<>)).As(typeof(IValidationErrorCache<>));

            builder.RegisterMock<IValidationItemProviderService<IEnumerable<IMessage>>>();
        }
    }
}
