using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.FileProvider.Abstract;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ValidationService.Data.Population.FileProvider
{
    public class XmlFileProviderService<T> : BaseFileProvider, IFileProvider<T>
    {
        public XmlFileProviderService(IXmlSerializationService xmlSerializationService, IFileService fileService)
            : base(xmlSerializationService, fileService)
        {
        }

        public async Task<T> ProvideAsync(string fileKey, string container, CancellationToken cancellationToken)
        {
            return await ProvideFileAsync<T>(fileKey, container, cancellationToken);
        }
    }
}
