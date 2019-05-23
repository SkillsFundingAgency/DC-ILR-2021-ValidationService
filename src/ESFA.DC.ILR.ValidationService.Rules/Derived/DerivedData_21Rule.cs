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
    /// derived data rule 21
    /// Adult skills funded unemployed learner on other state benefits on learning aim start date
    /// IsAdultFundedUnemployedWithOtherStateBenefits
    /// </summary>
    public class DerivedData_21Rule :
        IDerivedData_21Rule
    {
        /// <summary>
        /// The check (rule common operations provider)
        /// </summary>
        private IProvideRuleCommonOperations _check;

        /// <summary>
        /// Initializes a new instance of the <see cref="DerivedData_21Rule"/> class.
        /// </summary>
        /// <param name="commonOperations">The common operations.</param>
        public DerivedData_21Rule(IProvideRuleCommonOperations commonOperations)
        {
            It.IsNull(commonOperations)
                .AsGuard<ArgumentNullException>(nameof(commonOperations));

            _check = commonOperations;
        }

        /// <summary>
        /// Determines whether [is not employed] [the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if [is not employed] [the specified candidate]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotEmployed(ILearnerEmploymentStatus candidate) =>
            It.IsInRange(
                candidate?.EmpStat,
                TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable,
                TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable);

        /// <summary>
        /// In receipt of another benefit.
        /// </summary>
        /// <param name="employmentMonitoring">The employment monitoring.</param>
        /// <returns>true if the condition is met</returns>
        public bool InReceiptOfAnotherBenefit(IEmploymentStatusMonitoring employmentMonitoring) =>
            It.IsInRange(
                $"{employmentMonitoring.ESMType}{employmentMonitoring.ESMCode}",
                Monitoring.EmploymentStatus.InReceiptOfAnotherStateBenefit);

        /// <summary>
        /// In receipt of universal credit.
        /// </summary>
        /// <param name="employmentMonitoring">The employment monitoring.</param>
        /// <returns>true if the condition is met</returns>
        public bool InReceiptOfUniversalCredit(IEmploymentStatusMonitoring employmentMonitoring) =>
            It.IsInRange(
                $"{employmentMonitoring.ESMType}{employmentMonitoring.ESMCode}",
                Monitoring.EmploymentStatus.InReceiptOfUniversalCredit);

        /// <summary>
        /// Determines whether [in receipt of benefits] [the specified learner employment status].
        /// </summary>
        /// <param name="learnerEmploymentStatus">The learner employment status.</param>
        /// <returns>
        ///   <c>true</c> if [in receipt of benefits] [the specified learner employment status]; otherwise, <c>false</c>.
        /// </returns>
        public bool InReceiptOfBenefits(ILearnerEmploymentStatus learnerEmploymentStatus) =>
            learnerEmploymentStatus.EmploymentStatusMonitorings.SafeAny(InReceiptOfAnotherBenefit);

        /// <summary>
        /// Determines whether [in receipt of universal credits] [the specified learner employment status].
        /// </summary>
        /// <param name="learnerEmploymentStatus">The learner employment status.</param>
        /// <returns>
        ///   <c>true</c> if [in receipt of credites] [the specified learner employment status]; otherwise, <c>false</c>.
        /// </returns>
        public bool InReceiptOfCredits(ILearnerEmploymentStatus learnerEmploymentStatus) =>
            learnerEmploymentStatus.EmploymentStatusMonitorings.SafeAny(InReceiptOfUniversalCredit);

        /// <summary>
        /// Determines whether the specified FAM is (learning dleivery) monitored.
        /// </summary>
        /// <param name="fam">The fam.</param>
        /// <returns>
        ///   <c>true</c> if the specified fam is monitored; otherwise, <c>false</c>.
        /// </returns>
        public bool NotIsMonitored(ILearningDeliveryFAM fam) =>
            !It.IsInRange(fam.LearnDelFAMType, Monitoring.Delivery.Types.Learning);

        /// <summary>
        /// Determines whether (any of) the specified fams are monitored.
        /// </summary>
        /// <param name="fams">The fams.</param>
        /// <returns>
        ///   <c>true</c> if the specified fams are (learning delivery) monitored; otherwise, <c>false</c>.
        /// </returns>
        public bool NotIsMonitored(IReadOnlyCollection<ILearningDeliveryFAM> fams) =>
            fams.SafeAny(NotIsMonitored);

        /// <summary>
        /// Mandateds to skills training.
        /// </summary>
        /// <param name="fam">The fam.</param>
        /// <returns>true if the condition is met</returns>
        public bool MandatedToSkillsTraining(ILearningDeliveryFAM fam) =>
            It.IsInRange($"{fam.LearnDelFAMType}{fam.LearnDelFAMCode}", Monitoring.Delivery.MandationToSkillsTraining);

        /// <summary>
        /// Mandated to skills training.
        /// </summary>
        /// <param name="fams">The fams.</param>
        /// <returns>true if the condition is met</returns>
        public bool MandatedToSkillsTraining(IReadOnlyCollection<ILearningDeliveryFAM> fams) =>
            fams.SafeAny(MandatedToSkillsTraining);

        /// <summary>
        /// Determines whether [is adult funded unemployed with other state benefits] [this delivery].
        /// </summary>
        /// <param name="thisDelivery">This delivery.</param>
        /// <param name="forThisCandidate">For this candidate.</param>
        /// <returns>
        ///   <c>true</c> if [is adult funded unemployed with other state benefits] [this delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsAdultFundedUnemployedWithOtherStateBenefits(ILearningDelivery thisDelivery, ILearner forThisCandidate)
        {
            It.IsNull(thisDelivery)
                .AsGuard<ArgumentNullException>(nameof(thisDelivery));
            It.IsNull(forThisCandidate)
                .AsGuard<ArgumentNullException>(nameof(forThisCandidate));

            /*
                if
                    // is adult skills
                    LearningDelivery.FundModel = 35

                    //  is umemployed (not employed, seeking and available or otherwise)
                    and     LearnerEmploymentStatus.EmpStat = 11 or 12 for the latest Employment Status on (or before) the LearningDelivery.LearnStartDate

                            // in receipt of another benefit.
                    and     ((Monitoring.EmploymentStatus.ESMType = BSI and Monitoring.EmploymentStatus.ESMCode = 3)
                            or
                            // in receipt of universal credit.
                            (Monitoring.EmploymentStatus.ESMType = BSI and Monitoring.EmploymentStatus.ESMCode = 4
                            // is learning delivery monitored
                            and LearningDeliveryFAM.LearnDelFAMType = LDM
                            // and not mandated to skills training
                            and LearningDeliveryFAM.LearnDelFAMCode <> 318))

                        set to Y,
                        otherwise set to N
             */

            var employment = _check.GetEmploymentStatusOn(thisDelivery.LearnStartDate, forThisCandidate.LearnerEmploymentStatuses);

            return _check.HasQualifyingFunding(thisDelivery, TypeOfFunding.AdultSkills)
                && It.Has(employment)
                && IsNotEmployed(employment)
                && (InReceiptOfBenefits(employment)
                    || (InReceiptOfCredits(employment)
                        && (NotIsMonitored(thisDelivery.LearningDeliveryFAMs)
                            || !MandatedToSkillsTraining(thisDelivery.LearningDeliveryFAMs))));
        }
    }
}
