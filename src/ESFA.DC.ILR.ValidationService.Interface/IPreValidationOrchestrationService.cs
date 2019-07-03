using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IPreValidationOrchestrationService
    {
        Task ExecuteAsync(IValidationContext validationContext, CancellationToken cancellationToken);
    }
}