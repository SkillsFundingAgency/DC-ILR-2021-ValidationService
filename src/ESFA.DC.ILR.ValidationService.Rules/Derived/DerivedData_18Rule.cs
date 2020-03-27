using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    /// <summary>
    /// derived data rule 18
    /// Apprenticeship standard programme start date (held at aim level, calculated from matching programme aims)
    /// </summary>
    public class DerivedData_18Rule :
        IDerivedData_18Rule
    {
        /// <summary>
        /// The check (common operations provider)
        /// </summary>
        private readonly IProvideRuleCommonOperations _check;

        /// <summary>
        /// Initializes a new instance of the <see cref="DerivedData_18Rule"/> class.
        /// </summary>
        /// <param name="commonOperations">The common operations (provider).</param>
        public DerivedData_18Rule(IProvideRuleCommonOperations commonOperations)
        {
            It.IsNull(commonOperations)
                .AsGuard<ArgumentNullException>(nameof(commonOperations));

            _check = commonOperations;
        }

        /// <summary>
        /// Determines whether [has matching standard code] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if [has matching standard code] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasMatchingStandardCode(ILearningDelivery delivery, ILearningDelivery candidate) =>
            It.Has(delivery?.StdCodeNullable)
                && delivery.StdCodeNullable == candidate.StdCodeNullable;

        /// <summary>
        /// Determines whether [has restrictions match] [the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="andDelivery">The and delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has restrictions match] [the specified candidate]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasRestrictionsMatch(ILearningDelivery candidate, ILearningDelivery andDelivery) =>
            _check.IsStandardApprenticeship(candidate)
                && _check.InAProgramme(candidate)
                && HasMatchingStandardCode(candidate, andDelivery);

        /// <summary>
        /// Gets the apprenticeship standard programme start date for.
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <param name="usingSources">using sources.</param>
        /// <returns>the programme start date or null</returns>
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
                .SafeWhere(x => HasRestrictionsMatch(x, thisDelivery))
                .OrderBy(x => x.LearnStartDate)
                .FirstOrDefault();

            return candidate?.LearnStartDate;
        }
    }
}
