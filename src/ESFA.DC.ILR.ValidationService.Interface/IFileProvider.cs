using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IFileProvider<T>
    {
        Task<T> ProvideAsync(string fileKey, string container, CancellationToken cancellationToken);
    }
}
