using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Desktop
{
    public class ValidationExecutionProvider : IValidationExecutionProvider
    {
        public async Task ExecuteAsync(IValidationContext validationContext, IMessage message, CancellationToken cancellationToken)
        {
        }
    }
}
