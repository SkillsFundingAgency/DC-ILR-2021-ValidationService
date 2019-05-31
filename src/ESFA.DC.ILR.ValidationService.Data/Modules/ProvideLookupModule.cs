using Autofac;
using ESFA.DC.ILR.ValidationService.Data.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Modules
{
    public class ProvideLookupModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LookupDetailsProvider>().As<IProvideLookupDetails>();
        }
    }
}
