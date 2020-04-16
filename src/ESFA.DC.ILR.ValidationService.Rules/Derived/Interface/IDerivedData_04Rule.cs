using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived.Interface
{
    public interface IDerivedData_04Rule : IDerivedDataRule
    {
        DateTime? GetEarliesStartDateFor(ILearningDelivery thisDelivery, IReadOnlyCollection<ILearningDelivery> usingSources);
    }
}
