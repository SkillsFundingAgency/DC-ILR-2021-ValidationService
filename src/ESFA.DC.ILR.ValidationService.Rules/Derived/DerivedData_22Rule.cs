using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    public class DerivedData_22Rule :
        IDerivedData_22Rule
    {
        /// <summary>
        /// Gets the latest learning start for esf contract.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="usingSources">The using sources.</param>
        /// <returns>the latest start date or null</returns>
        public DateTime? GetLatestLearningStartForESFContract(
            ILearningDelivery candidate,
            IReadOnlyCollection<ILearningDelivery> usingSources)
        {
            It.IsNull(candidate)
                .AsGuard<ArgumentNullException>(nameof(candidate));
            It.IsNull(usingSources)
                .AsGuard<ArgumentNullException>(nameof(usingSources));

            var latest = usingSources
                .Where(x => IsCompletedQualifyingAim(x) && HasMatchingContractReference(x, candidate))
                .OrderByDescending(x => x.LearnStartDate)
                .FirstOrDefault();

            return latest?.LearnStartDate;
        }

        /// <summary>
        /// Determines whether [is completed qualifying aim] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is completed qualifying aim] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsCompletedQualifyingAim(ILearningDelivery delivery) =>
            delivery.LearnAimRef == TypeOfAim.References.ESFLearnerStartandAssessment
            && delivery.CompStatus == CompletionState.HasCompleted;

        /// <summary>
        /// Determines whether [has matching contract reference] [the specified source and candidate].
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if [has matching contract reference] [the specified source]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasMatchingContractReference(ILearningDelivery source, ILearningDelivery candidate) =>
            It.Has(source.ConRefNumber) && source.ConRefNumber.CaseInsensitiveEquals(candidate.ConRefNumber);
    }
}
