using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    public class DerivedData_18Rule : IDerivedData_18Rule
    {
        public DerivedData_18Rule()
        {
        }

        public bool HasMatchingStandardCode(ILearningDelivery delivery, ILearningDelivery candidate) =>
            delivery != null
            && delivery.StdCodeNullable.HasValue
            && delivery.StdCodeNullable == candidate.StdCodeNullable;

        public bool HasRestrictionsMatch(ILearningDelivery candidate, ILearningDelivery andDelivery) =>
            candidate.ProgTypeNullable == TypeOfLearningProgramme.ApprenticeshipStandard
                && candidate.AimType == TypeOfAim.ProgrammeAim
                && HasMatchingStandardCode(candidate, andDelivery);

        public DateTime? GetApprenticeshipStandardProgrammeStartDateFor(ILearningDelivery thisDelivery, IReadOnlyCollection<ILearningDelivery> usingSources)
        {
            /*
              LearningDelivery.ProgType = 25
              and the earliest value of LearningDelivery.LearnStartDate for all programme aims with LearningDelivery.AimType = 1
              and the same value of Learner.LearnRefNumber, LearningDelivery.ProgType and LearningDelivery.StdCode.
              Set to NULL if there are no such programme aims
           */

            var candidate = usingSources
                .NullSafeWhere(x => HasRestrictionsMatch(x, thisDelivery))
                .OrderBy(x => x.LearnStartDate)
                .FirstOrDefault();

            return candidate?.LearnStartDate;
        }
    }
}
