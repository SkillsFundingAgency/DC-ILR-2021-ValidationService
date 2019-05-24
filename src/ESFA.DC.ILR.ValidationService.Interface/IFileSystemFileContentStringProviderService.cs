using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface IFileSystemFileContentStringProviderService
    {
        Task<Stream> Provide(IValidationContext validationContext, CancellationToken cancellationToken);
    }
}
