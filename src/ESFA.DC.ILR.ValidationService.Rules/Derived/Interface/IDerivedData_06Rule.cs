using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived.Interface
{
    public interface IDerivedData_06Rule : IDerivedDataRule
    {
        DateTime Derive(IEnumerable<ILearningDelivery> learningDeliveries);
    }
}
