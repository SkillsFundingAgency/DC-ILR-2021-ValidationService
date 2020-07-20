using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived.Interface
{
    /// <summary>
    /// Gets Category Ref for LearningDelivery based on Table2 worksheet in ILR Validation Rules
    /// </summary>
    public interface IDerivedData_CategoryRef_02 : IDerivedDataRule
    {
        int? Derive(ILearningDelivery learningDelivery);
    }
}
