using ESFA.DC.ILR.Model.Interface;
using System;
using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived.Interface
{
    /// <summary>
    /// derived data rule 22
    /// </summary>
    public interface IDerivedData_22Rule : IDerivedDataRule
    {
        /// <summary>
        /// Gets the latest learning start for esf contract.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="usingSources">The using sources.</param>
        /// <returns>the latest start date or null</returns>
        DateTime? GetLatestLearningStartForESFContract(ILearningDelivery candidate, IReadOnlyCollection<ILearningDelivery> usingSources);
    }
}
