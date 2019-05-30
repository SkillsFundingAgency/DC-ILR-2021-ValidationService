using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query.Interface
{
    public interface ILearnerQueryService : IQueryService
    {
        bool HasLearningDeliveryFAMCodeForType(ILearner learner, string famType, string famCode);
    }
}
