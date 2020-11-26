using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived.Interface
{
    public interface IDerivedData_39Rule : IDerivedDataRule
    {
        ILearnerReferenceData GetMatchingLearningAimFromPreviousYear(ILearner learner, ILearningDelivery learningDelivery);
    }
}
