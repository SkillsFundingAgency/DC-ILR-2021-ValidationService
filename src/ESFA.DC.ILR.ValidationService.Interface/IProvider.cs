using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IProvider<T>
    {
        Task<T> ProvideAsync(IValidationContext validationContext, CancellationToken cancellationToken);
    }
}
