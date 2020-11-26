using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived.Interface
{
    public interface IDerivedData_17Rule :
        IDerivedDataRule
    {
        bool IsTNPMoreThanContributionCapFor(int theStandard, IReadOnlyCollection<ILearningDelivery> theDeliveries);

        int GetTotalTNPPriceFor(IReadOnlyCollection<ILearningDelivery> theDeliveries);

        decimal? GetFundingContributionCapFor(int theStandard, IReadOnlyCollection<ILearningDelivery> theDeliveries);
    }
}
