using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Autofac.Features.AttributeFilters;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
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
using ESFA.DC.ILR.ValidationService.Data.Population.Mappers;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Providers;
using ESFA.DC.ILR.ValidationService.Providers.Output;
using ESFA.DC.ILR.ValidationService.RuleSet;
using ESFA.DC.ILR.ValidationService.RuleSet.ErrorHandler;
using Module = Autofac.Module;

namespace ESFA.DC.ILR.ValidationService.Stateless.Modules
{
    public class PreValidationServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PreValidationOrchestrationSfService>().As<IPreValidationOrchestrationService>().InstancePerLifetimeScope();

            builder.RegisterType<MessageFileProviderService>().As<IProvider<IMessage>>().InstancePerLifetimeScope();
            builder.RegisterType<IlrReferenceDataFileProviderService>().As<IProvider<ReferenceDataRoot>>().InstancePerLifetimeScope();
            builder.RegisterType<ValidationOutputService>().As<IValidationOutputService>().WithAttributeFiltering().InstancePerLifetimeScope();
            builder.RegisterType<ValidIlrFileOutputService>().As<IValidIlrFileOutputService>().InstancePerLifetimeScope();
            builder.RegisterType<LearnerPerActorProviderService>().As<ILearnerPerActorProviderService>().InstancePerLifetimeScope();
            builder.RegisterType<LearnerDPPerActorProviderService>().As<ILearnerDPPerActorProviderService>().InstancePerLifetimeScope();
            builder.RegisterType<ValidationErrorCache>().As<IValidationErrorCache>().InstancePerLifetimeScope();
            builder.RegisterType<PreValidationPopulationService>().As<IPopulationService>().InstancePerLifetimeScope();
            builder.RegisterType<RuleSetOrchestrationService<IMessage>>().As<IRuleSetOrchestrationService<IMessage>>();
            builder.RegisterType<AutoFacRuleSetResolutionService<IMessage>>().As<IRuleSetResolutionService<IMessage>>();
            builder.RegisterType<RuleSetExecutionService<IMessage>>().As<IRuleSetExecutionService<IMessage>>();
            builder.RegisterType<ValidationErrorHandler>().As<IValidationErrorHandler>().InstancePerLifetimeScope();
            builder.RegisterType<ActorValidationExecutionProvider>().As<IValidationExecutionProvider>().InstancePerLifetimeScope();
            builder.RegisterType<EnabledRulesProvider>().As<IEnabledRulesProvider>().InstancePerLifetimeScope();

            builder.RegisterType<PreValidationPopulationService>().As<IPopulationService>().InstancePerLifetimeScope();
            builder.RegisterModule<MessageRuleSetModule>();
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

            builder.RegisterType<LookupsDataMapper>().As<ILookupsDataMapper>().InstancePerLifetimeScope();
            builder.RegisterType<EmployersDataMapper>().As<IEmployersDataMapper>().InstancePerLifetimeScope();
            builder.RegisterType<EpaOrgDataMapper>().As<IEpaOrgDataMapper>().InstancePerLifetimeScope();
            builder.RegisterType<FcsDataMapper>().As<IFcsDataMapper>().InstancePerLifetimeScope();
            builder.RegisterType<LarsDataMapper>().As<ILarsDataMapper>().InstancePerLifetimeScope();
            builder.RegisterType<OrganisationsDataMapper>().As<IOrganisationsDataMapper>().InstancePerLifetimeScope();
            builder.RegisterType<PostcodesDataMapper>().As<IPostcodesDataMapper>().InstancePerLifetimeScope();
            builder.RegisterType<UlnDataMapper>().As<IUlnDataMapper>().InstancePerLifetimeScope();
            builder.RegisterType<ValidationErrorsDataMapper>().As<IValidationErrorsDataMapper>().InstancePerLifetimeScope();
            builder.RegisterType<ValidationRulesDataMapper>().As<IValidationRulesDataMapper>().InstancePerLifetimeScope();
        }
    }
}
