/*
 * CME 2018-10-09
 * this derived date rule is not used as it is only used in emp stat 14
 * and this rule effectively embodies the requirements of emp stat 14
 * so it serves no purpose

using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived.Interface
{
    /// <summary>
    /// derived data rule 24
    /// Learner's employment status on the start date of the completed ZESF0001 aim for that contract
    /// </summary>
    public interface IDerivedData_24Rule
    {
        /// <summary>
        /// Gets the employment status for completed esf contract.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>the emp stat</returns>
        int? GetEmploymentStatusForCompletedESFContract(ILearner candidate);
    }
}

using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    /// <summary>
    /// derived data rule 24
    /// Learner's employment status on the start date of the completed ZESF0001 aim for that contract
    /// </summary>
    public class DerivedData_24Rule :
        IDerivedData_24Rule
    {
        /// <summary>
        /// The derived data 22 (rule)
        /// </summary>
        private readonly IDerivedData_22Rule _derivedData22;

        /// <summary>
        /// Initializes a new instance of the <see cref="DerivedData_24Rule " /> class.
        /// </summary>
        /// <param name="derivedData22">The derived data 22.</param>
        public DerivedData_24Rule(IDerivedData_22Rule derivedData22)
        {
            It.IsNull(derivedData22)
                .AsGuard<ArgumentNullException>(nameof(derivedData22));

            _derivedData22 = derivedData22;
        }

        /// <summary>
        /// Gets the contract completion date.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="usingSources">The using sources.</param>
        /// <returns>the latest completion date for the contract (if there is one)</returns>
        public DateTime? GetContractCompletionDate(ILearningDelivery delivery, IReadOnlyCollection<ILearningDelivery> usingSources) =>
            _derivedData22.GetLatestLearningStartForESFContract(delivery, usingSources);

        /// <summary>
        /// Gets the latest contract completion date.
        /// </summary>
        /// <param name="usingSources">The using sources.</param>
        /// <returns>the latest completion date for all the contracts (if there is one)</returns>
        public DateTime? GetLatestContractCompletionDate(IReadOnlyCollection<ILearningDelivery> usingSources)
        {
            var candidates = Collection.Empty<DateTime?>();
            usingSources.ForEach(source => candidates.Add(GetContractCompletionDate(source, usingSources)));

            return candidates.Max();
        }

        /// <summary>
        /// Gets the closest employment.
        /// </summary>
        /// <param name="usingSources">The using sources.</param>
        /// <param name="toThisDate">To this date.</param>
        /// <returns>the closest employment record to this date (if there is one)</returns>
        public ILearnerEmploymentStatus GetClosestEmployment(IReadOnlyCollection<ILearnerEmploymentStatus> usingSources, DateTime? toThisDate) =>
            usingSources
                .SafeWhere(x => x.DateEmpStatApp <= toThisDate)
                .OrderByDescending(x => x.DateEmpStatApp)
                .FirstOrDefault();

        /// <summary>
        /// Gets the employment status for completed esf contract.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        /// the emp stat
        /// </returns>
        public int? GetEmploymentStatusForCompletedESFContract(ILearner candidate)
        {
            var fromDeliveries = candidate.LearningDeliveries.AsSafeReadOnlyList();
            var startDate = GetLatestContractCompletionDate(fromDeliveries);
            var employment = GetClosestEmployment(candidate.LearnerEmploymentStatuses, startDate);

            return employment?.EmpStat;
        }
    }
}
*/