using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReferenceDataService.Model.Learner;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Desktop
{
    public class DesktopLearnerReferenceDataFileProviderService : IFileProvider<LearnerReferenceData>
    {
        public Task<LearnerReferenceData> ProvideAsync(string fileKey, string container, CancellationToken cancellationToken)
        {
            return Task.FromResult(new LearnerReferenceData());
        }
    }
}
