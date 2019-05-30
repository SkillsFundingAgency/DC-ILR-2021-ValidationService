using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Desktop.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Desktop
{
    public class ValidationServiceDesktopTask : IDesktopTask
    {
        private readonly IValidationContextFactory<IDesktopContext> _validationContextFactory;

        public ValidationServiceDesktopTask(IValidationContextFactory<IDesktopContext> validationContextFactory)
        {
            _validationContextFactory = validationContextFactory;
        }

        public Task<IDesktopContext> ExecuteAsync(IDesktopContext desktopContext, CancellationToken cancellationToken)
        {
            var context = _validationContextFactory.Build(desktopContext);

            return Task.FromResult(desktopContext);
        }
    }
}
