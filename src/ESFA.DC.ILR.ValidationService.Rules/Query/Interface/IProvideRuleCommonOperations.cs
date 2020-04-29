using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query.Interface
{
    public interface IProvideRuleCommonOperations : IQueryService
    {
        bool HasQualifyingFunding(ILearningDelivery delivery, params int[] desiredFundings);
    }
}
