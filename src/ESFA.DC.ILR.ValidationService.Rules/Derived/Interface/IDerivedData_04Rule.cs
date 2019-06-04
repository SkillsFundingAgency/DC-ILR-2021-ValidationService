using ESFA.DC.ILR.Model.Interface;
using System;
using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived.Interface
{
    public interface IDerivedData_04Rule : IDerivedDataRule
    {
        /// <summary>
        /// Gets the earlies start date for.
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <param name="usingSources">using sources.</param>
        /// <returns>the earliest start date matching the search criteria: main aim, programme type, framework code and pathway code</returns>
        DateTime? GetEarliesStartDateFor(ILearningDelivery thisDelivery, IReadOnlyCollection<ILearningDelivery> usingSources);
    }
}
