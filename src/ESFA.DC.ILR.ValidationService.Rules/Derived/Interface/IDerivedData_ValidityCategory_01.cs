using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived.Interface
{
    /// <summary>
    /// Gets Validity Category for LearningDelivery based on Table1 worksheet in ILR Validation Rules
    /// </summary>
    public interface IDerivedData_ValidityCategory_01 : IDerivedDataRule
    {
        string Derive(ILearningDelivery learningDelivery, IReadOnlyCollection<ILearnerEmploymentStatus> learnerEmploymentStatuses);
    }
}
