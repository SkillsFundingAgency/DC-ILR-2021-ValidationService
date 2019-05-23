using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    /// <summary>
    /// derived date 04 rule
    /// for any learning delivery in the set it gets
    /// the earliest start date matching the search criteria: main aim, programme type, framework code and pathway code
    /// from the set
    /// </summary>
    /// <seealso cref="IDerivedData_04Rule" />
    public class DerivedData_04Rule :
        IDerivedData_04Rule
    {
        /// <summary>
        /// Gets the earlies start date for.
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <param name="usingSources">using sources.</param>
        /// <returns>the earliest start date matching the search criteria: main aim, programme type, framework code and pathway code</returns>
        public DateTime? GetEarliesStartDateFor(ILearningDelivery thisDelivery, IReadOnlyCollection<ILearningDelivery> usingSources) =>
            GetEarliesStartDateFor(TypeOfAim.ProgrammeAim, thisDelivery.ProgTypeNullable, thisDelivery.FworkCodeNullable, thisDelivery.PwayCodeNullable, usingSources);

        /// <summary>
        /// Gets the earlies start date for.
        /// </summary>
        /// <param name="aimType">Type of the aim.</param>
        /// <param name="progType">Type of the prog.</param>
        /// <param name="fworkCode">The fwork code.</param>
        /// <param name="pwayCode">The pway code.</param>
        /// <param name="usingSources">The using sources.</param>
        /// <returns>the earliest start date matching the search criteria: main aim, programme type, framework code and pathway code</returns>
        public DateTime? GetEarliesStartDateFor(long aimType, long? progType, long? fworkCode, long? pwayCode, IEnumerable<ILearningDelivery> usingSources) =>
            usingSources
                .SafeWhere(ld => ld.AimType == aimType
                    && ld.ProgTypeNullable == progType
                    && ld.FworkCodeNullable == fworkCode
                    && ld.PwayCodeNullable == pwayCode)
                .Min(ld => (DateTime?)ld.LearnStartDate);
    }
}
