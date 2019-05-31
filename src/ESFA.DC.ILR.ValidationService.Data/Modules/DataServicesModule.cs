using System.Reflection;
using Autofac;
using ESFA.DC.ILR.ValidationService.Interface;
using Module = Autofac.Module;

namespace ESFA.DC.ILR.ValidationService.Data.Modules
{
    public class DataServicesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(t => t.IsAssignableTo<IDataService>())
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}
