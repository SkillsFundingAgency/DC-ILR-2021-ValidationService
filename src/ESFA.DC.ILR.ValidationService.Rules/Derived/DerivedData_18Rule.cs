using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    public class DerivedData_18Rule :
        IDerivedData_18Rule
    {
        private readonly IProvideRuleCommonOperations _check;

        public DerivedData_18Rule(IProvideRuleCommonOperations commonOperations)
        {
            It.IsNull(commonOperations)
                .AsGuard<ArgumentNullException>(nameof(commonOperations));

            _check = commonOperations;
        }

        public bool HasMatchingStandardCode(ILearningDelivery delivery, ILearningDelivery candidate) =>
            It.Has(delivery?.StdCodeNullable)
                && delivery.StdCodeNullable == candidate.StdCodeNullable;

        public bool HasRestrictionsMatch(ILearningDelivery candidate, ILearningDelivery andDelivery) =>
            _check.IsStandardApprenticeship(candidate)
                && _check.InAProgramme(candidate)
                && HasMatchingStandardCode(candidate, andDelivery);

        public DateTime? GetApprenticeshipStandardProgrammeStartDateFor(ILearningDelivery thisDelivery, IReadOnlyCollection<ILearningDelivery> usingSources)
        {
            It.IsNull(thisDelivery)
                .AsGuard<ArgumentNullException>(nameof(thisDelivery));
            It.IsEmpty(usingSources)
                .AsGuard<ArgumentNullException>(nameof(usingSources));

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
