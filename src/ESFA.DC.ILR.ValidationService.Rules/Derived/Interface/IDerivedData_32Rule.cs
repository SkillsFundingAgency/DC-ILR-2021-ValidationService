using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived.Interface
{
    public interface IDerivedData_32Rule : IDerivedDataRule
    {
        bool IsOpenApprenticeshipFundedProgramme(ILearningDelivery learningDelivery);
    }
}
