using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ReferenceDataService.Model;
using ESFA.DC.ILR.ReferenceDataService.Model.Learner;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Interface
{
    public interface IPopulationService
    {
        void Populate(IValidationContext validationContext, IMessage message, ReferenceDataRoot referenceDataRoot, LearnerReferenceData learnerReferenceData);
    }
}
