using Autofac;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Mappers;

namespace ESFA.DC.ILR.ValidationService.Modules
{
    public class DataCacheMapperModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LookupsDataMapper>().As<ILookupsDataMapper>().InstancePerLifetimeScope();

            builder.RegisterType<EmployersDataMapper>().As<IEmployersDataMapper>().InstancePerLifetimeScope();
            builder.RegisterType<EpaOrgDataMapper>().As<IEpaOrgDataMapper>().InstancePerLifetimeScope();
            builder.RegisterType<FcsDataMapper>().As<IFcsDataMapper>().InstancePerLifetimeScope();
            builder.RegisterType<LarsDataMapper>().As<ILarsDataMapper>().InstancePerLifetimeScope();
            builder.RegisterType<OrganisationsDataMapper>().As<IOrganisationsDataMapper>().InstancePerLifetimeScope();
            builder.RegisterType<PostcodesDataMapper>().As<IPostcodesDataMapper>().InstancePerLifetimeScope();
            builder.RegisterType<UlnDataMapper>().As<IUlnDataMapper>().InstancePerLifetimeScope();
            builder.RegisterType<ValidationErrorsDataMapper>().As<IValidationErrorsDataMapper>().InstancePerLifetimeScope();
        }
    }
}
