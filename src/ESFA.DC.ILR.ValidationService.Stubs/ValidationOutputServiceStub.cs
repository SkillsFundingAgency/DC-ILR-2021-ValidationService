using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Stubs
{
    public class ValidationOutputServiceStub : IValidationOutputService
    {
        public Task ProcessAsync(IValidationContext validationContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
