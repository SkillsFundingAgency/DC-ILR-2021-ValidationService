using ESFA.DC.ILR.ReferenceDataService.Model.Learner;

namespace ESFA.DC.ILR.ValidationService.Data.Population.Interface
{
    public interface ILearnerReferenceDataCachePopulationService
    {
        void Populate(LearnerReferenceData learnerReferenceData);
    }
}
