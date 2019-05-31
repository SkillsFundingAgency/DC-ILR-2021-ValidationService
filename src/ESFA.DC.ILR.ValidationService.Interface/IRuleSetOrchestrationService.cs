using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IRuleSetOrchestrationService<T>
        where T : class
    {
        Task<IEnumerable<IValidationError>> ExecuteAsync(IValidationContext validationContext, IEnumerable<T> validationItems, CancellationToken cancellationToken);

        Task<IEnumerable<IValidationError>> ExecuteAsync(IValidationContext validationContext, T validationItem, CancellationToken cancellationToken);
    }
}
