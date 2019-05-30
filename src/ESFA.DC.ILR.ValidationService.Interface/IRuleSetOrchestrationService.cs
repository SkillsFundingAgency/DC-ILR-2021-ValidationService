using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IRuleSetOrchestrationService<T, U>
        where T : class
    {
        Task<IEnumerable<U>> ExecuteAsync(IValidationContext validationContext, IEnumerable<T> validationItems, CancellationToken cancellationToken);

        Task<IEnumerable<U>> ExecuteAsync(IValidationContext validationContext, T validationItem, CancellationToken cancellationToken);
    }
}
