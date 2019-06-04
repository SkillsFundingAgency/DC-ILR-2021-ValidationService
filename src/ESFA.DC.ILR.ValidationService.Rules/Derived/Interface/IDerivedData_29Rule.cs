using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived.Interface
{
    /// <summary>
    /// derived data rule 21
    /// Adult skills funded unemployed learner on other state benefits on learning aim start date
    /// </summary>
    public interface IDerivedData_29Rule : IDerivedDataRule
    {
        bool IsInflexibleElementOfTrainingAim(ILearner candidate);
    }
}
