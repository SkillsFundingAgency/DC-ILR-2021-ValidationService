using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    public class DerivedData_04Rule :
        IDerivedData_04Rule
    {
        public DateTime? GetEarliesStartDateFor(ILearningDelivery thisDelivery, IReadOnlyCollection<ILearningDelivery> usingSources) =>
            GetEarliesStartDateFor(TypeOfAim.ProgrammeAim, thisDelivery.ProgTypeNullable, thisDelivery.FworkCodeNullable, thisDelivery.PwayCodeNullable, usingSources);

        public DateTime? GetEarliesStartDateFor(long aimType, long? progType, long? fworkCode, long? pwayCode, IEnumerable<ILearningDelivery> usingSources) =>
            usingSources
                .NullSafeWhere(ld => ld.AimType == aimType
                    && ld.ProgTypeNullable == progType
                    && ld.FworkCodeNullable == fworkCode
                    && ld.PwayCodeNullable == pwayCode)
                .Min(ld => (DateTime?)ld.LearnStartDate);
    }
}
