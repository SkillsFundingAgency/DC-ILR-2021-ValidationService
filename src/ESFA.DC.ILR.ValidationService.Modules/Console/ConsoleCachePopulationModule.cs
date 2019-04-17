using Autofac;
using ESFA.DC.Data.ILR.ValidationErrors.Model;
using ESFA.DC.Data.ILR.ValidationErrors.Model.Interfaces;
using ESFA.DC.Data.LARS.Model;
using ESFA.DC.Data.LARS.Model.Interfaces;
using ESFA.DC.Data.Postcodes.Model;
using ESFA.DC.Data.Postcodes.Model.Interfaces;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population;
using ESFA.DC.ILR.ValidationService.Data.Population.Configuration.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ReferenceData.FCS.Model;
using ESFA.DC.ReferenceData.FCS.Model.Interface;
using ESFA.DC.ReferenceData.Organisations.Model;
using ESFA.DC.ReferenceData.Organisations.Model.Interface;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.ILR.ValidationService.Modules.Console
{
    public class ConsoleCachePopulationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PreValidationPopulationService>().As<IPopulationService>().InstancePerLifetimeScope();

            builder.RegisterType<InternalDataCachePopulationService>().As<IInternalDataCachePopulationService>().InstancePerLifetimeScope();
            builder.RegisterType<InternalDataCachePopulationService>().As<ICreateInternalDataCache>().InstancePerLifetimeScope();

            builder.RegisterType<FileDataCachePopulationService>().As<IFileDataCachePopulationService>().InstancePerLifetimeScope();
            builder.RegisterType<MessageCachePopulationService>().As<IMessageCachePopulationService>().InstancePerLifetimeScope();

            builder.RegisterType<LARS>().As<ILARS>().InstancePerLifetimeScope();
            builder.RegisterType<Postcodes>().As<IPostcodes>().InstancePerLifetimeScope();
            builder.Register(c =>
            {
                DbContextOptions<OrganisationsContext> options = new DbContextOptionsBuilder<OrganisationsContext>()
            .UseSqlServer(c.Resolve<IReferenceDataOptions>().OrganisationsConnectionString).Options;

                return new OrganisationsContext(options);
            }).As<IOrganisationsContext>().InstancePerLifetimeScope();
            builder.RegisterType<FcsContext>().As<IFcsContext>().InstancePerLifetimeScope();
            builder.RegisterType<ValidationErrors>().As<IValidationErrors>().InstancePerLifetimeScope();

            builder.RegisterModule<ExternalDataCachePopulationServiceModule>();
        }
    }
}
