using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Stubs
{
    public class FileSystemFileContentStringProviderService : IFileSystemFileContentStringProviderService
    {
        private readonly IPreValidationContext _preValidationContext;

        public FileSystemFileContentStringProviderService(IPreValidationContext preValidationContext)
        {
            _preValidationContext = preValidationContext;
        }

        public async Task<Stream> Provide(CancellationToken cancellationToken)
        {
            return File.Open(_preValidationContext.Input, FileMode.Open);
        }
    }
}
