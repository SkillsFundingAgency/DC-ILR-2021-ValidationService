using Autofac;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Mappers;

namespace ESFA.DC.ILR.ValidationService.Modules
{
    public class DataCacheMapperModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EmployersDataMapper>().As<IEmployersDataMapper>().InstancePerLifetimeScope();
            builder.RegisterType<PostcodesDataMapper>().As<IPostcodesDataMapper>().InstancePerLifetimeScope();
            builder.RegisterType<UlnDataMapper>().As<IUlnDataMapper>().InstancePerLifetimeScope();
        }
    }
}
