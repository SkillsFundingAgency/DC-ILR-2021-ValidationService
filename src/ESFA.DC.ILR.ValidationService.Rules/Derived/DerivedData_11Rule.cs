using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    /// <summary>
    /// derived data rule 11
    /// Adult skills funded learner on benefits at the start of the learning aim
    /// </summary>
    public class DerivedData_11Rule :
        IDerivedData_11Rule
    {
        /// <summary>
        /// The check (common operations provider)
        /// </summary>
        private readonly IProvideRuleCommonOperations _check;

        /// <summary>
        /// Initializes a new instance of the <see cref="DerivedData_11Rule"/> class.
        /// </summary>
        /// <param name="commonOps">The common ops.</param>
        public DerivedData_11Rule(IProvideRuleCommonOperations commonOps)
        {
            It.IsNull(commonOps)
                .AsGuard<ArgumentNullException>(nameof(commonOps));

            _check = commonOps;
        }

        /// <summary>
        /// In receipt of benefits.
        /// </summary>
        /// <param name="learnerEmploymentStatus">The learner employment status.</param>
        /// <param name="startDate">The start date.</param>
        /// <returns>
        /// true, if on state benefits at start of learning aim
        /// </returns>
        public bool InReceiptOfBenefits(IReadOnlyCollection<ILearnerEmploymentStatus> learnerEmploymentStatus, DateTime startDate)
        {
            var candidate = _check.GetEmploymentStatusOn(startDate, learnerEmploymentStatus);

            return InReceiptOfBenefits(candidate?.EmploymentStatusMonitorings);
        }

        /// <summary>
        /// In receipt of benefits.
        /// </summary>
        /// <param name="monitors">The monitors.</param>
        /// <returns>if any item in the set matches the required criteria</returns>
        public bool InReceiptOfBenefits(IReadOnlyCollection<IEmploymentStatusMonitoring> monitors)
        {
            return monitors.SafeAny(InReceiptOfBenefits);
        }

        /// <summary>
        /// In receipt of benefits.
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <returns>true if the 'typed code' matches one from the set</returns>
        public bool InReceiptOfBenefits(IEmploymentStatusMonitoring monitor) =>
            It.IsInRange(
                $"{monitor.ESMType}{monitor.ESMCode}",
                Monitoring.EmploymentStatus.InReceiptOfUniversalCredit,
                Monitoring.EmploymentStatus.InReceiptOfAnotherStateBenefit,
                Monitoring.EmploymentStatus.InReceiptOfEmploymentAndSupportAllowance,
                Monitoring.EmploymentStatus.InReceiptOfJobSeekersAllowance);

        /// <summary>
        /// Determines whether [is adult funded on benefits at start of aim] [the specified candidate].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="learnerEmployments">The learner employments.</param>
        /// <returns>
        ///   <c>true</c> if [is adult funded on benefits at start of aim] [the specified candidate]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsAdultFundedOnBenefitsAtStartOfAim(ILearningDelivery delivery, IReadOnlyCollection<ILearnerEmploymentStatus> learnerEmployments)
        {
            It.IsNull(delivery)
                .AsGuard<ArgumentNullException>(nameof(delivery));
            var employments = learnerEmployments.AsSafeReadOnlyList();

            /*
                if
                    LearningDelivery.FundModel = 35
                    and the learner's Employment status on the LearningDelivery.LearnStartDate of the learning aim
                    is (EmploymentStatusMonitoring.ESMType = BSI and EmploymentStatusMonitoring.ESMCode = 1, 2, 3 or 4)
                        set to Y,
                        otherwise set to N
             */

            return _check.HasQualifyingFunding(delivery, TypeOfFunding.AdultSkills)
                && InReceiptOfBenefits(employments, delivery.LearnStartDate);
        }
    }
}
