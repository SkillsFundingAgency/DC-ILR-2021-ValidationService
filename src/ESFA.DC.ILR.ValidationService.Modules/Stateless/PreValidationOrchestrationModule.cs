using System.Collections.Generic;
using Autofac;
using Autofac.Features.AttributeFilters;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ValidationService.Data.Population;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Providers;
using ESFA.DC.ILR.ValidationService.Providers.Output;
using ESFA.DC.ILR.ValidationService.RuleSet;
using ESFA.DC.ILR.ValidationService.RuleSet.ErrorHandler;
using ESFA.DC.ILR.ValidationService.Stubs;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Serialization.Xml;

namespace ESFA.DC.ILR.ValidationService.Modules.Stateless
{
    public class PreValidationOrchestrationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PreValidationOrchestrationSfService<IValidationError>>().As<IPreValidationOrchestrationService<IValidationError>>().InstancePerLifetimeScope();

            builder.RegisterType<MessageFileProviderService>().As<IValidationItemProviderService<IMessage>>().InstancePerLifetimeScope();
            builder.RegisterType<IlrReferenceDataFileProviderService>().As<IValidationItemProviderService<ReferenceDataRoot>>().InstancePerLifetimeScope();
            builder.RegisterType<ValidationOutputService>().As<IValidationOutputService>().WithAttributeFiltering().InstancePerLifetimeScope();
            builder.RegisterType<LearnerPerActorProviderService>().As<ILearnerPerActorProviderService>().InstancePerLifetimeScope();
            builder.RegisterType<LearnerDPPerActorProviderService>().As<ILearnerDPPerActorProviderService>().InstancePerLifetimeScope();
            builder.RegisterType<ValidationErrorCache>().As<IValidationErrorCache<IValidationError>>().InstancePerLifetimeScope();
            builder.RegisterType<PreValidationPopulationService>().As<IPopulationService>().InstancePerLifetimeScope();
            builder.RegisterType<JsonSerializationService>().As<IJsonSerializationService>().InstancePerLifetimeScope();
            builder.RegisterType<XmlSerializationService>().As<IXmlSerializationService>().InstancePerLifetimeScope();
            builder.RegisterType<RuleSetOrchestrationService<IMessage, IValidationError>>().As<IRuleSetOrchestrationService<IMessage, IValidationError>>();
            builder.RegisterType<AutoFacRuleSetResolutionService<IMessage>>().As<IRuleSetResolutionService<IMessage>>();
            builder.RegisterType<RuleSetExecutionService<IMessage>>().As<IRuleSetExecutionService<IMessage>>();
            builder.RegisterType<MessageProviderService>().As<IValidationItemProviderService<IEnumerable<IMessage>>>().InstancePerLifetimeScope();
            builder.RegisterType<LearnerProviderService>().As<IValidationItemProviderService<IEnumerable<ILearner>>>().InstancePerLifetimeScope();
            builder.RegisterType<LearnerDPProviderService>().As<IValidationItemProviderService<IEnumerable<ILearnerDestinationAndProgression>>>().InstancePerLifetimeScope();
            builder.RegisterType<ValidationErrorHandler>().As<IValidationErrorHandler>().InstancePerLifetimeScope();
        }
    }
}
