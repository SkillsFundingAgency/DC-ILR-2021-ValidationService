using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived.Interface
{
    public interface IDerivedData_25Rule : IDerivedDataRule
    {
        int? GetLengthOfUnemployment(
            ILearner learner,
            string conRefNumber);
    }
}
