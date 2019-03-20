using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    /// <summary>
    /// derived data rule 28
    /// Adult skills funded learner unemployed with benefits on learning aim start date
    /// IsAdultFundedUnemployedWithBenefits
    /// </summary>
    public class DerivedData_28Rule :
        IDerivedData_28Rule
    {
        /// <summary>
        /// The check (rule common operations provider)
        /// </summary>
        private IProvideRuleCommonOperations _check;

        /// <summary>
        /// Initializes a new instance of the <see cref="DerivedData_28Rule"/> class.
        /// </summary>
        /// <param name="commonOperations">The common operations.</param>
        public DerivedData_28Rule(IProvideRuleCommonOperations commonOperations)
        {
            It.IsNull(commonOperations)
                .AsGuard<ArgumentNullException>(nameof(commonOperations));

            _check = commonOperations;
        }

        /// <summary>
        /// In receipt of employment support.
        /// </summary>
        /// <param name="employmentMonitoring">The employment monitoring.</param>
        /// <returns>true if the condition is met</returns>
        public bool InReceiptOfEmploymentSupport(IEmploymentStatusMonitoring employmentMonitoring) =>
            It.IsInRange(
                $"{employmentMonitoring.ESMType}{employmentMonitoring.ESMCode}",
                Monitoring.EmploymentStatus.InReceiptOfJobSeekersAllowance,
                Monitoring.EmploymentStatus.InReceiptOfEmploymentAndSupportAllowance);

        /// <summary>
        /// Determines whether [is in receipt of employment support] [the specified employment monitorings].
        /// </summary>
        /// <param name="employmentMonitorings">The employment monitorings.</param>
        /// <returns>
        ///   <c>true</c> if [is in receipt of employment support] [the specified employment monitorings]; otherwise, <c>false</c>.
        /// </returns>
        public bool InReceiptOfEmploymentSupport(IReadOnlyCollection<IEmploymentStatusMonitoring> employmentMonitorings) =>
            employmentMonitorings.SafeAny(InReceiptOfEmploymentSupport);

        /// <summary>
        /// Determines whether [has valid employment status] [the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if [has valid employment status] [the specified candidate]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasValidEmploymentStatus(ILearnerEmploymentStatus candidate) =>
            It.IsInRange(
                candidate?.EmpStat,
                TypeOfEmploymentStatus.InPaidEmployment,
                TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable,
                TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable,
                TypeOfEmploymentStatus.NotKnownProvided);

        /// <summary>
        /// Determines whether [is valid with employment support] [the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if [is valid with employment support] [the specified candidate]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValidWithEmploymentSupport(ILearnerEmploymentStatus candidate)
        {
            return HasValidEmploymentStatus(candidate)
                && InReceiptOfEmploymentSupport(candidate.EmploymentStatusMonitorings);
        }

        /// <summary>
        /// In receipt of (other state) benefits / credits.
        /// </summary>
        /// <param name="employmentMonitoring">The employment monitoring.</param>
        /// <returns>true if the condition is met</returns>
        public bool InReceiptOfCredits(IEmploymentStatusMonitoring employmentMonitoring) =>
            It.IsInRange(
                $"{employmentMonitoring.ESMType}{employmentMonitoring.ESMCode}",
                Monitoring.EmploymentStatus.InReceiptOfAnotherStateBenefit,
                Monitoring.EmploymentStatus.InReceiptOfUniversalCredit);

        /// <summary>
        /// Determines whether [is in receipt of credits] [the specified employment monitorings].
        /// </summary>
        /// <param name="employmentMonitorings">The employment monitorings.</param>
        /// <returns>
        ///   <c>true</c> if [is in receipt of credits] [the specified employment monitorings]; otherwise, <c>false</c>.
        /// </returns>
        public bool InReceiptOfCredits(IReadOnlyCollection<IEmploymentStatusMonitoring> employmentMonitorings) =>
            employmentMonitorings.SafeAny(InReceiptOfCredits);

        /// <summary>
        /// Determines whether [is not employed] [the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if [is not employed] [the specified candidate]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotEmployed(ILearnerEmploymentStatus candidate) =>
            It.IsInRange(
                candidate.EmpStat,
                TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable,
                TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable);

        /// <summary>
        /// Determines whether [is not employed with benefits] [the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if [is not employed with benefits] [the specified candidate]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotEmployedWithBenefits(ILearnerEmploymentStatus candidate)
        {
            return IsNotEmployed(candidate)
                && InReceiptOfCredits(candidate.EmploymentStatusMonitorings);
        }

        /// <summary>
        /// Determines whether [is working short hours] [the specified monitor].
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [is working short hours] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsWorkingShortHours(IEmploymentStatusMonitoring monitor) =>
            It.IsInRange(
                $"{monitor.ESMType}{monitor.ESMCode}",
                Monitoring.EmploymentStatus.EmployedForLessThan16HoursPW,
                Monitoring.EmploymentStatus.EmployedFor0To10HourPW,
                Monitoring.EmploymentStatus.EmployedFor11To20HoursPW);

        /// <summary>
        /// Determines whether [is working short hours] [the specified employment monitorings].
        /// </summary>
        /// <param name="employmentMonitorings">The employment monitorings.</param>
        /// <returns>
        ///   <c>true</c> if [is working short hours] [the specified employment monitorings]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsWorkingShortHours(IReadOnlyCollection<IEmploymentStatusMonitoring> employmentMonitorings) =>
            employmentMonitorings.SafeAny(IsWorkingShortHours);

        /// <summary>
        /// Determines whether the specified candidate is employed.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if the specified candidate is employed; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEmployed(ILearnerEmploymentStatus candidate) =>
            It.IsInRange(candidate.EmpStat, TypeOfEmploymentStatus.InPaidEmployment);

        /// <summary>
        /// Determines whether [is employed with support] [the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if [is employed with support] [the specified candidate]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEmployedWithSupport(ILearnerEmploymentStatus candidate)
        {
            return IsEmployed(candidate)
                && IsWorkingShortHours(candidate.EmploymentStatusMonitorings)
                && InReceiptOfCredits(candidate.EmploymentStatusMonitorings);
        }

        /// <summary>
        /// Determines whether [is adult skills unemployed with benefits] [this delivery].
        /// </summary>
        /// <param name="thisDelivery">This delivery.</param>
        /// <param name="forThisCandidate">For this candidate.</param>
        /// <returns>
        ///   <c>true</c> if [is adult skills unemployed with benefits] [this delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsAdultFundedUnemployedWithBenefits(ILearningDelivery thisDelivery, ILearner forThisCandidate)
        {
            It.IsNull(thisDelivery)
                .AsGuard<ArgumentNullException>(nameof(thisDelivery));
            It.IsNull(forThisCandidate)
                .AsGuard<ArgumentNullException>(nameof(forThisCandidate));

            /*
                if
                    // is adult skills
                    LearningDelivery.FundModel = 35
                    // and has valid employment status
                    and LearnerEmploymentStatus.EmpStat = 10, 11, 12 or 98
                    // and in receipt of support at the time of starting the learning aim
                    and (Monitoring.EmploymentStatus.ESMType = BSI and Monitoring.EmploymentStatus.ESMCode = 1 or 2)
                        (for the learner's Employment status on the LearningDelivery.LearnStartDate of the learning aim)
                or
                    // or is not employed, and in receipt of benefits
                    LearnerEmploymentStatus.EmpStat = 11 or 12
                    and (Monitoring.EmploymentStatus.ESMType = BSI and Monitoring.EmploymentStatus.ESMCode = 3 or 4)
                or
                    // or is employed with workng short hours and in receipt of support
                    LearnerEmploymentStatus.EmpStat = 10
                    and (Monitoring.EmploymentStatus.ESMType = EII and Monitoring.EmploymentStatus.ESMCode = 2, 5 or 6)
                    and (Monitoring.EmploymentStatus.ESMType = BSI and Monitoring.EmploymentStatus.ESMCode = 3 or 4)
                        set to Y,
                        otherwise set to N
             */

            var employment = _check.GetEmploymentStatusOn(thisDelivery.LearnStartDate, forThisCandidate.LearnerEmploymentStatuses);

            return _check.HasQualifyingFunding(thisDelivery, TypeOfFunding.AdultSkills)
                && It.Has(employment)
                && (IsValidWithEmploymentSupport(employment)
                || IsNotEmployedWithBenefits(employment)
                || IsEmployedWithSupport(employment));
        }
    }
}
