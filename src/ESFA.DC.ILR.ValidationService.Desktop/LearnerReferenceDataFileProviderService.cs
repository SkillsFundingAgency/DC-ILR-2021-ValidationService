using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ReferenceDataService.Model.Learner;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Desktop
{
    public class LearnerReferenceDataFileProviderService : IProvider<LearnerReferenceData>
    {
        public Task<LearnerReferenceData> ProvideAsync(IValidationContext validationContext, CancellationToken cancellationToken)
        {
            return Task.FromResult(new LearnerReferenceData());
        }
    }
}
