using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IValidationExecutionProvider
    {
        Task ExecuteAsync(IValidationContext validationContext, IMessage message, CancellationToken cancellationToken);
    }
}
