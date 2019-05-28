using System;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Desktop.Interface;

namespace ESFA.DC.ILR.ValidationService.Desktop
{
    public class ValidationServiceDesktopTask : IDesktopTask
    {
        public Task<IDesktopContext> ExecuteAsync(IDesktopContext desktopContext, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
