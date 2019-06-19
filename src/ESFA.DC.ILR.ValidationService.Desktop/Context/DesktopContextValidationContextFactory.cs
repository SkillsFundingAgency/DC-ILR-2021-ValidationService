using ESFA.DC.ILR.Desktop.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Desktop.Context
{
    public class DesktopContextValidationContextFactory : IValidationContextFactory<IDesktopContext>
    {
        public IValidationContext Build(IDesktopContext context)
        {
            return new DesktopContextValidationContext(context);
        }
    }
}
