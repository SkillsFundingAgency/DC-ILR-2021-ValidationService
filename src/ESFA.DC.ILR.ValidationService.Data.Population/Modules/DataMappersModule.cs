using System.Reflection;
using Autofac;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using Module = Autofac.Module;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Modules
{
    public class DataMappersModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(t => t.IsAssignableTo<IMapper>())
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}
