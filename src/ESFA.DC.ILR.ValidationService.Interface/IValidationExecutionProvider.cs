using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IValidationExecutionProvider<U>
    {
        Task ExecuteAsync(IValidationContext validationContext, IMessage message, CancellationToken cancellationToken);
    }
}
