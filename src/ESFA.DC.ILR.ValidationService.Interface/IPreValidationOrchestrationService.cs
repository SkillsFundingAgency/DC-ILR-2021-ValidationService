using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IPreValidationOrchestrationService<U>
    {
        Task ExecuteAsync(IValidationContext validationContext, CancellationToken cancellationToken);
    }
}