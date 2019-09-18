using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.Query
{
    /// <summary>
    /// the rule common operations provider implementation
    /// </summary>
    /// <seealso cref="IProvideRuleCommonOperations" />
    public class RuleCommonOperationsProvider :
        IProvideRuleCommonOperations
    {
        /// <summary>
        /// the derived data (rule) 07
        /// </summary>
        private readonly IDerivedData_07Rule _derivedData07;

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleCommonOperationsProvider" /> class.
        /// </summary>
        /// <param name="derivedData07">The derived data (rule) 07.</param>
        public RuleCommonOperationsProvider(
            IDerivedData_07Rule derivedData07)
        {
            It.IsNull(derivedData07)
                .AsGuard<ArgumentNullException>(nameof(derivedData07));

            _derivedData07 = derivedData07;
        }

        /// <summary>
        /// Checks the delivery fams.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="matchCondition">The match condition.</param>
        /// <returns>true if any of the delivery fams match the condition</returns>
        public bool CheckDeliveryFAMs(ILearningDelivery delivery, Func<ILearningDeliveryFAM, bool> matchCondition) =>
            delivery.LearningDeliveryFAMs.SafeAny(matchCondition);

        /// <summary>
        /// Determines whether the specified learning delivery FAM is a restart.
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if the specified monitor is restart; otherwise, <c>false</c>.
        /// </returns>
        public bool IsRestart(ILearningDeliveryFAM monitor) =>
            It.IsInRange(monitor.LearnDelFAMType, Monitoring.Delivery.Types.Restart);

        /// <summary>
        /// Determines whether the specified learning delivery is a restart.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if the specified delivery is restart; otherwise, <c>false</c>.
        /// </returns>
        public bool IsRestart(ILearningDelivery delivery) =>
            CheckDeliveryFAMs(delivery, IsRestart);

        /// <summary>
        /// Determines whether [is advanced learner loan] [the specified monitor].
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [is advanced learner loan] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsAdvancedLearnerLoan(ILearningDeliveryFAM monitor) =>
            It.IsInRange(monitor.LearnDelFAMType, Monitoring.Delivery.Types.AdvancedLearnerLoan);

        /// <summary>
        /// Determines whether [is advanced learner loan] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is advanced learner loan] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsAdvancedLearnerLoan(ILearningDelivery delivery) =>
            CheckDeliveryFAMs(delivery, IsAdvancedLearnerLoan);

        /// <summary>
        /// Determines whether [is loans bursary] [the specified monitor].
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [is loans bursary] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsLoansBursary(ILearningDeliveryFAM monitor) =>
            It.IsInRange(monitor.LearnDelFAMType, Monitoring.Delivery.Types.AdvancedLearnerLoansBursaryFunding);

        /// <summary>
        /// Determines whether [is loans bursary] [this delivery].
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is loans bursary] [this delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsLoansBursary(ILearningDelivery thisDelivery) =>
            CheckDeliveryFAMs(thisDelivery, IsLoansBursary);

        /// <summary>
        /// Determines whether [is learner in custody] [the specified monitor].
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [is learner in custody] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsLearnerInCustody(ILearningDeliveryFAM monitor) =>
            It.IsInRange($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}", Monitoring.Delivery.OLASSOffendersInCustody);

        /// <summary>
        /// Determines whether the specified learning delivery is a learner in custody
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is learner in custody] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsLearnerInCustody(ILearningDelivery delivery) =>
            CheckDeliveryFAMs(delivery, IsLearnerInCustody);

        /// <summary>
        /// Determines whether [is steel worker] [the specified monitor].
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [is steel worker] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsSteelWorkerRedundancyTraining(ILearningDeliveryFAM monitor) =>
            It.IsInRange($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}", Monitoring.Delivery.SteelIndustriesRedundancyTraining);

        /// <summary>
        /// Determines whether the specified learning delivery is steel worker redundancy training
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is steel worker redundancy training] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsSteelWorkerRedundancyTraining(ILearningDelivery delivery) =>
            CheckDeliveryFAMs(delivery, IsSteelWorkerRedundancyTraining);

        /// <summary>
        /// Determines whether [is released on temporary licence] [the specified monitor].
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [is released on temporary licence] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsReleasedOnTemporaryLicence(ILearningDeliveryFAM monitor) =>
            It.IsInRange($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}", Monitoring.Delivery.ReleasedOnTemporaryLicence);

        /// <summary>
        /// Determines whether the specified learning delivery is released on temporary licence
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is released on temporary licence] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsReleasedOnTemporaryLicence(ILearningDelivery delivery) =>
            CheckDeliveryFAMs(delivery, IsReleasedOnTemporaryLicence);

        /// <summary>
        /// Determines whether the specified learning delivery is in an apprenticeship
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [in apprenticeship] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool InApprenticeship(ILearningDelivery delivery) =>
            _derivedData07.IsApprenticeship(delivery.ProgTypeNullable);

        /// <summary>
        /// Determines whether the specified learning delivery aim is in a programme
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [in a programme] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool InAProgramme(ILearningDelivery delivery) =>
            It.IsInRange(delivery.AimType, TypeOfAim.ProgrammeAim);

        /// <summary>
        /// Determines whether [is component of a program] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is component of a program] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsComponentOfAProgram(ILearningDelivery delivery) =>
            It.IsInRange(delivery.AimType, TypeOfAim.ComponentAimInAProgramme);

        /// <summary>
        /// Determines whether the specified delivery is traineeship.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if the specified delivery is traineeship; otherwise, <c>false</c>.
        /// </returns>
        public bool IsTraineeship(ILearningDelivery delivery) =>
            It.IsInRange(delivery.ProgTypeNullable, TypeOfLearningProgramme.Traineeship);

        /// <summary>
        /// Determines whether [is standard Apprenticeship] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is standard Apprenticeship] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsStandardApprenticeship(ILearningDelivery delivery) =>
            It.IsInRange(delivery.ProgTypeNullable, TypeOfLearningProgramme.ApprenticeshipStandard);

        /// <summary>
        /// Determines whether the specified learning delivery has qualifying funding
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="desiredFundings">The desired fundings.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying funding] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingFunding(ILearningDelivery delivery, params int[] desiredFundings) =>
            It.IsInRange(delivery.FundModel, desiredFundings);

        /// <summary>
        /// Determines whether the specified learning delivery has qualifying start date
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="minStart">The minimum start.</param>
        /// <param name="maxStart">The maximum start (if null sets to today).</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying start] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingStart(ILearningDelivery delivery, DateTime minStart, DateTime? maxStart = null) =>
            It.Has(delivery)
            && It.IsBetween(delivery.LearnStartDate, minStart, maxStart ?? DateTime.MaxValue);

        /// <summary>
        /// Determines whether the specified employment status record has qualifying start date
        /// </summary>
        /// <param name="employment">The employment.</param>
        /// <param name="minStart">The minimum start.</param>
        /// <param name="maxStart">The maximum start (if null sets to today).</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying start] [the specified employment]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingStart(ILearnerEmploymentStatus employment, DateTime minStart, DateTime? maxStart = null) =>
            It.Has(employment)
            && It.IsBetween(employment.DateEmpStatApp, minStart, maxStart ?? DateTime.MaxValue);

        /// <summary>
        /// Gets the (closest) qualifying employment status to the learner start date.
        /// </summary>
        /// <param name="thisStartDate">this start date.</param>
        /// <param name="usingSources">using sources.</param>
        /// <returns>
        /// returns the latest applicable employment status
        /// </returns>
        public ILearnerEmploymentStatus GetEmploymentStatusOn(DateTime? thisStartDate, IReadOnlyCollection<ILearnerEmploymentStatus> usingSources) =>
            usingSources
                .SafeWhere(x => x.DateEmpStatApp <= thisStartDate)
                .OrderByDescending(x => x.DateEmpStatApp)
                .FirstOrDefault();
    }
}
