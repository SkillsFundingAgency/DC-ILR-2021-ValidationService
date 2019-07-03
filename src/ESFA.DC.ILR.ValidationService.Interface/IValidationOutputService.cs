using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IValidationOutputService
    {
        Task ProcessAsync(IValidationContext validationContext, IMessage message, IEnumerable<IValidationError> validationErrors, CancellationToken cancellationToken);
    }
}
