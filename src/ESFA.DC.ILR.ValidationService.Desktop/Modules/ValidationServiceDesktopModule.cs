using Autofac;
using ESFA.DC.ILR.ValidationService.Modules;

namespace ESFA.DC.ILR.ValidationService.Desktop.Modules
{
    public class ValidationServiceDesktopModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<ValidationServicesModule>();
        }
    }
}
