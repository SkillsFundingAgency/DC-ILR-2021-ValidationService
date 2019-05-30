using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived.Interface
{
    public interface IDerivedData_19Rule : IDerivedDataRule
    {
        DateTime? Derive(IEnumerable<ILearningDelivery> learningDeliveries, ILearningDelivery learningDelivery);
    }
}
