using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived.Interface
{
    public interface IDerivedData_31Rule : IDerivedDataRule
    {
        bool IsAdultSkillsFundedEnglishOrMathsAim(ILearningDelivery learningDelivery);
    }
}
