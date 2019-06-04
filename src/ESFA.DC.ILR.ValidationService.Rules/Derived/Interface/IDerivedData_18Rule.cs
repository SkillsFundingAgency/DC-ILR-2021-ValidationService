using ESFA.DC.ILR.Model.Interface;
using System;
using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived.Interface
{
    /// <summary>
    /// derived data rule 18
    /// Apprenticeship standard programme start date (held at aim level, calculated from matching programme aims)
    /// </summary>
    public interface IDerivedData_18Rule : IDerivedDataRule
    {
        /// <summary>
        /// Gets the apprenticeship standard programme start date for.
        /// </summary>
        /// <param name="thisDelivery">The this delivery.</param>
        /// <param name="usingSources">The using sources.</param>
        /// <returns>the programme start date or null</returns>
        DateTime? GetApprenticeshipStandardProgrammeStartDateFor(ILearningDelivery thisDelivery, IReadOnlyCollection<ILearningDelivery> usingSources);
    }
}
