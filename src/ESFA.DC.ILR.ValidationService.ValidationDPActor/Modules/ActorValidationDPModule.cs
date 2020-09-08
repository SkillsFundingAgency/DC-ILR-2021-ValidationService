using Autofac;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data;
using ESFA.DC.ILR.ValidationService.Data.Cache;
using ESFA.DC.ILR.ValidationService.Data.External;
using ESFA.DC.ILR.ValidationService.Data.External.EDRS;
using ESFA.DC.ILR.ValidationService.Data.External.EDRS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.EPAOrganisation;
using ESFA.DC.ILR.ValidationService.Data.External.EPAOrganisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.FCS;
using ESFA.DC.ILR.ValidationService.Data.External.FCS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.LARS;
using ESFA.DC.ILR.ValidationService.Data.External.LARS.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation;
using ESFA.DC.ILR.ValidationService.Data.External.Organisation.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes;
using ESFA.DC.ILR.ValidationService.Data.External.Postcodes.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.ULN;
using ESFA.DC.ILR.ValidationService.Data.External.ULN.Interface;
using ESFA.DC.ILR.ValidationService.Data.External.ValidationErrors;
using ESFA.DC.ILR.ValidationService.Data.File;
using ESFA.DC.ILR.ValidationService.Data.File.FileData;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Providers;
using ESFA.DC.ILR.ValidationService.Rules.Modules;
using ESFA.DC.ILR.ValidationService.RuleSet;
using ESFA.DC.ILR.ValidationService.RuleSet.ErrorHandler;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Serialization.Xml;

namespace ESFA.DC.ILR.ValidationService.ValidationDPActor.Modules
{
    public class ActorValidationDPModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RuleSetOrchestrationService<IRule<ILearner>, ILearner>>().As<IRuleSetOrchestrationService<IRule<ILearner>, ILearner>>();
            builder.RegisterType<RuleSetOrchestrationService<IRule<ILearnerDestinationAndProgression>, ILearnerDestinationAndProgression>>().As<IRuleSetOrchestrationService<IRule<ILearnerDestinationAndProgression>, ILearnerDestinationAndProgression>>();

            builder.RegisterType<RuleSetResolutionService<IRule<IMessage>, IMessage>>().As<IRuleSetResolutionService<IRule<IMessage>, IMessage>>();
            builder.RegisterType<RuleSetResolutionService<IRule<ILearner>, ILearner>>().As<IRuleSetResolutionService<IRule<ILearner>, ILearner>>();
            builder.RegisterType<RuleSetResolutionService<IRule<ILearnerDestinationAndProgression>, ILearnerDestinationAndProgression>>().As<IRuleSetResolutionService<IRule<ILearnerDestinationAndProgression>, ILearnerDestinationAndProgression>>();
            builder.RegisterType<RuleSetExecutionService<IRule<IMessage>, IMessage>>().As<IRuleSetExecutionService<IRule<IMessage>, IMessage>>();
            builder.RegisterType<RuleSetExecutionService<IRule<ILearner>, ILearner>>().As<IRuleSetExecutionService<IRule<ILearner>, ILearner>>();
            builder.RegisterType<RuleSetExecutionService<IRule<ILearnerDestinationAndProgression>, ILearnerDestinationAndProgression>>().As<IRuleSetExecutionService<IRule<ILearnerDestinationAndProgression>, ILearnerDestinationAndProgression>>();
            builder.RegisterType<ValidationErrorHandler>().As<IValidationErrorHandler>().InstancePerLifetimeScope();
            builder.RegisterType<ValidationErrorCache>().As<IValidationErrorCache>().InstancePerLifetimeScope();
            builder.RegisterType<EnabledRulesProvider>().As<IEnabledRulesProvider>().InstancePerLifetimeScope();

            builder.RegisterType<JsonSerializationService>().As<IJsonSerializationService>().InstancePerLifetimeScope();
            builder.RegisterType<XmlSerializationService>().As<IXmlSerializationService>().InstancePerLifetimeScope();

            builder.RegisterModule<RuleSetModule<IRule<ILearnerDestinationAndProgression>, ILearnerDestinationAndProgression>>();

            builder.RegisterModule<DerivedDataModule>();
            builder.RegisterModule<QueryServiceModule>();

            builder.RegisterType<InternalDataCachePopulationService>().As<IInternalDataCachePopulationService>().InstancePerLifetimeScope();
            builder.RegisterType<ExternalDataCachePopulationService>().As<IExternalDataCachePopulationService>().InstancePerLifetimeScope();
            builder.RegisterType<FileDataCachePopulationService>().As<IFileDataCachePopulationService>().InstancePerLifetimeScope();
            builder.RegisterType<MessageCachePopulationService>().As<IMessageCachePopulationService>().InstancePerLifetimeScope();

            builder.RegisterType<DateTimeProvider.DateTimeProvider>().As<IDateTimeProvider>().InstancePerLifetimeScope();

            builder.RegisterType<ExternalDataCache>().As<IExternalDataCache>().InstancePerLifetimeScope();
            builder.RegisterType<InternalDataCache>().As<IInternalDataCache>().InstancePerLifetimeScope();
            builder.RegisterType<FileDataCache>().As<IFileDataCache>().InstancePerLifetimeScope();

            builder.RegisterType<Cache<IMessage>>().As<ICache<IMessage>>().InstancePerLifetimeScope();

            builder.RegisterType<FileDataService>().As<IFileDataService>().InstancePerLifetimeScope();
            builder.RegisterType<LARSDataService>().As<ILARSDataService>().InstancePerLifetimeScope();
            builder.RegisterType<OrganisationDataService>().As<IOrganisationDataService>().InstancePerLifetimeScope();
            builder.RegisterType<EPAOrganisationDataService>().As<IEPAOrganisationDataService>().InstancePerLifetimeScope();
            builder.RegisterType<ULNDataService>().As<IULNDataService>().InstancePerLifetimeScope();
            builder.RegisterType<PostcodesDataService>().As<IPostcodesDataService>();
            builder.RegisterType<ValidationErrorsDataService>().As<IValidationErrorsDataService>();
            builder.RegisterType<FCSDataService>().As<IFCSDataService>().InstancePerLifetimeScope();
            builder.RegisterType<EmployersDataService>().As<IEmployersDataService>().InstancePerLifetimeScope();

            builder.RegisterType<AcademicYearDataService>().As<IAcademicYearDataService>().InstancePerLifetimeScope();
            builder.RegisterType<LookupDetailsProvider>().As<IProvideLookupDetails>().InstancePerLifetimeScope();
        }
    }
}
