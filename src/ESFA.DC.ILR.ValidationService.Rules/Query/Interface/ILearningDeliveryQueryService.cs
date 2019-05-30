using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query.Interface
{
    public interface ILearningDeliveryQueryService : IQueryService
    {
        double? AverageAddHoursPerLearningDay(ILearningDelivery learningDelivery);
    }
}
