using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Stubs
{
    public class FileSystemFileContentStringProviderService : IFileSystemFileContentStringProviderService
    {
        public async Task<Stream> Provide(IValidationContext validationContext, CancellationToken cancellationToken)
        {
            return File.Open(validationContext.Filename, FileMode.Open);
        }
    }
}
