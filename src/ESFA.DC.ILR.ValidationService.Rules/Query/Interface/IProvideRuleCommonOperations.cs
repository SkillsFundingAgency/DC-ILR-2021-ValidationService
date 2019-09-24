using ESFA.DC.ILR.Model.Interface;
using System;
using System.Collections.Generic;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query.Interface
{
    /// <summary>
    /// i provide common rule operations (definition)
    /// </summary>
    public interface IProvideRuleCommonOperations : IQueryService
    {
        /// <summary>
        /// Checks the delivery fams.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="matchCondition">The match condition.</param>
        /// <returns>true if any of the delivery fams match the condition</returns>
        bool CheckDeliveryFAMs(ILearningDelivery delivery, Func<ILearningDeliveryFAM, bool> matchCondition);

        /// <summary>
        /// Determines whether the specified learning delivery is a restart.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if the specified delivery is restart; otherwise, <c>false</c>.
        /// </returns>
        bool IsRestart(ILearningDelivery delivery);

        /// <summary>
        /// Determines whether [is advanced learner loan] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is advanced learner loan] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        bool IsAdvancedLearnerLoan(ILearningDelivery delivery);

        /// <summary>
        /// Determines whether [is loans bursary] [this delivery].
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is loans bursary] [this delivery]; otherwise, <c>false</c>.
        /// </returns>
        bool IsLoansBursary(ILearningDelivery thisDelivery);

        /// <summary>
        /// Determines whether the specified learning delivery is a learner in custody
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is learner in custody] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        bool IsLearnerInCustody(ILearningDelivery delivery);

        /// <summary>
        /// Determines whether the specified learning delivery is steel worker redundancy training
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is steel worker redundancy training] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        bool IsSteelWorkerRedundancyTraining(ILearningDelivery delivery);

        /// <summary>
        /// Determines whether the specified learning delivery is released on temporary licence
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is released on temporary licence] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        bool IsReleasedOnTemporaryLicence(ILearningDelivery delivery);

        /// <summary>
        /// Determines whether the specified learning delivery is in an apprenticeship
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [in apprenticeship] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        bool InApprenticeship(ILearningDelivery delivery);

        /// <summary>
        /// Determines whether the specified learning delivery aim is in a programme
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [in a programme] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        bool InAProgramme(ILearningDelivery delivery);

        /// <summary>
        /// Determines whether [is component of a program] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is component of a program] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        bool IsComponentOfAProgram(ILearningDelivery delivery);

        /// <summary>
        /// Determines whether the specified delivery is traineeship.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if the specified delivery is traineeship; otherwise, <c>false</c>.
        /// </returns>
        bool IsTraineeship(ILearningDelivery delivery);

        /// <summary>
        /// Determines whether [is standard apprenticeship] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is standard apprenticeship] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        bool IsStandardApprenticeship(ILearningDelivery delivery);

        /// <summary>
        /// Determines whether the specified learning delivery has qualifying funding
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="desiredFundings">The desired fundings.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying funding] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        bool HasQualifyingFunding(ILearningDelivery delivery, params int[] desiredFundings);

        /// <summary>
        /// Determines whether the specified learning delivery has qualifying start date
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="minStart">The minimum start.</param>
        /// <param name="maxStart">The maximum start (if null sets to today).</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying start] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        bool HasQualifyingStart(ILearningDelivery delivery, DateTime minStart, DateTime? maxStart = null);

        /// <summary>
        /// Determines whether the specified employment status record has qualifying start date
        /// </summary>
        /// <param name="employment">The employment.</param>
        /// <param name="minStart">The minimum start.</param>
        /// <param name="maxStart">The maximum start (if null sets to today).</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying start] [the specified employment]; otherwise, <c>false</c>.
        /// </returns>
        bool HasQualifyingStart(ILearnerEmploymentStatus employment, DateTime minStart, DateTime? maxStart = null);

        /// <summary>
        /// Gets the (closest) qualifying employment status to the start date.
        /// </summary>
        /// <param name="thisStartDate">this start date.</param>
        /// <param name="usingSources">The using sources.</param>
        /// <returns>
        /// returns the latest applicable employment status
        /// </returns>
        ILearnerEmploymentStatus GetEmploymentStatusOn(DateTime? thisStartDate, IReadOnlyCollection<ILearnerEmploymentStatus> usingSources);
    }
}
