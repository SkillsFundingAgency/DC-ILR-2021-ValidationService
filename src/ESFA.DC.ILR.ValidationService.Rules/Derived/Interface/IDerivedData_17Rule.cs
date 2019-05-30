using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived.Interface
{
    public interface IDerivedData_17Rule : IDerivedDataRule
    {
        bool IsTotalNegotiatedPriceMoreThanCapForStandard(IEnumerable<ILearningDelivery> learningDeliveries, int standardCode);
    }
}
