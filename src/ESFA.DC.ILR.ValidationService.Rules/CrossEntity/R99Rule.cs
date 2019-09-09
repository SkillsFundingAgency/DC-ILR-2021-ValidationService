using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    /// <summary>
    /// cross record rule 99
    /// </summary>
    /// <seealso cref="AbstractRule" />
    /// <seealso cref="Interface.IRule{ILearner}" />
    public class R99Rule :
        AbstractRule,
        IRule<ILearner>
    {
        /// <summary>
        /// The check(er) rule common operations provider
        /// </summary>
        private readonly IProvideRuleCommonOperations _check;

        /// <summary>
        /// Initializes a new instance of the <see cref="R99Rule"/> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="commonOps">The common rule operations provider.</param>
        public R99Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideRuleCommonOperations commonOps)
            : base(validationErrorHandler, RuleNameConstants.R99)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(commonOps)
                .AsGuard<ArgumentNullException>(nameof(commonOps));

            _check = commonOps;
        }

        /// <summary>
        /// Validates the specified the learner.
        /// </summary>
        /// <param name="theLearner">The learner.</param>
        public void Validate(ILearner theLearner)
        {
            It.IsNull(theLearner)
                .AsGuard<ArgumentNullException>(nameof(theLearner));

            var candidates = GetCandidateDeliveries(theLearner.LearningDeliveries);

            if (HasViableCount(candidates))
            {
                var learnRefNumber = theLearner.LearnRefNumber;

                candidates.ForAny(
                    x => IsNotValid(x, AgainstOtherDeliveries(x, candidates)),
                    x => RaiseValidationMessage(learnRefNumber, x));
            }
        }

        /// <summary>
        /// Gets the candidate deliveries.
        /// </summary>
        /// <param name="candidates">The candidates.</param>
        /// <returns>returns those matching the restriction criteria</returns>
        public IReadOnlyCollection<ILearningDelivery> GetCandidateDeliveries(IReadOnlyCollection<ILearningDelivery> candidates) =>
            candidates
                .SafeWhere(IsProgrammeAim)
                .AsSafeReadOnlyList();

        /// <summary>
        /// Determines whether [is programme aim] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is programe aim] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsProgrammeAim(ILearningDelivery theDelivery) =>
            _check.InAProgramme(theDelivery);

        /// <summary>
        /// Determines whether [has viable count] [the specified candidates].
        /// </summary>
        /// <param name="candidates">The candidates.</param>
        /// <returns>
        ///   <c>true</c> if [has viable count] [the specified candidates]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasViableCount(IReadOnlyCollection<ILearningDelivery> candidates) =>
            candidates.Count > 1;

        /// <summary>
        /// Against other deliveries.
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <param name="candidates">The other candidates.</param>
        /// <returns>takes the input and returns those matching the restriction criteria</returns>
        public IReadOnlyCollection<ILearningDelivery> AgainstOtherDeliveries(ILearningDelivery theDelivery, IReadOnlyCollection<ILearningDelivery> candidates) =>
            candidates
                .SafeWhere(x => IsNotSelf(theDelivery, x))
                .AsSafeReadOnlyList();

        /// <summary>
        /// Determines whether [is not self] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if [is not self] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotSelf(ILearningDelivery theDelivery, ILearningDelivery candidate) =>
            theDelivery.AimSeqNumber != candidate.AimSeqNumber;

        /// <summary>
        /// Determines whether [is not valid] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <param name="candidates">The other candidates.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearningDelivery theDelivery, IReadOnlyCollection<ILearningDelivery> candidates) =>
            (IsOpenAim(theDelivery)
                && HasOpenAim(candidates))
            || HasOverlappingAimEndDates(theDelivery, candidates)
            || HasOverlappingAimAchievementDates(theDelivery, AgainstStandardApprencticeshipDeliveries(candidates));

        /// <summary>
        /// Determines whether [is open aim] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is open aim] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsOpenAim(ILearningDelivery theDelivery) =>
            It.IsEmpty(theDelivery.LearnActEndDateNullable);

        /// <summary>
        /// Determines whether [has open aim] [the specified candidates].
        /// </summary>
        /// <param name="candidates">The candidates.</param>
        /// <returns>
        ///   <c>true</c> if [has open aim] [the specified candidates]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasOpenAim(IReadOnlyCollection<ILearningDelivery> candidates) =>
            candidates.SafeAny(IsOpenAim);

        /// <summary>
        /// Determines whether [has overlapping aim end dates] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <param name="candidates">The other candidates.</param>
        /// <returns>
        ///   <c>true</c> if [has overlapping aim end dates] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasOverlappingAimEndDates(ILearningDelivery theDelivery, IReadOnlyCollection<ILearningDelivery> candidates) =>
            candidates
                .SafeAny(x => HasOverlappingAimEndDates(theDelivery, x));

        /// <summary>
        /// Determines whether [has overlapping aim end dates] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if [has overlapping aim end dates] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasOverlappingAimEndDates(ILearningDelivery theDelivery, ILearningDelivery candidate) =>
            It.IsBetween(theDelivery.LearnStartDate, candidate.LearnStartDate, candidate.LearnActEndDateNullable ?? DateTime.MaxValue);

        /// <summary>
        /// Determines whether [has qualifying model] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying model] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingModel(ILearningDelivery theDelivery) =>
            _check.HasQualifyingFunding(theDelivery, TypeOfFunding.ApprenticeshipsFrom1May2017);

        /// <summary>
        /// Determines whether [is standard apprencticeship] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is standard apprencticeship] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsStandardApprencticeship(ILearningDelivery theDelivery) =>
            _check.IsStandardApprencticeship(theDelivery);

        /// <summary>
        /// Against standard apprencticeship deliveries.
        /// </summary>
        /// <param name="candidates">The candidates.</param>
        /// <returns>takes the input and returns those matching the restriction criteria</returns>
        public IReadOnlyCollection<ILearningDelivery> AgainstStandardApprencticeshipDeliveries(IReadOnlyCollection<ILearningDelivery> candidates) =>
            candidates
                .SafeWhere(x => HasQualifyingModel(x) && IsStandardApprencticeship(x))
                .AsSafeReadOnlyList();

        /// <summary>
        /// Determines whether [has overlapping aim achievement dates] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <param name="candidates">The other candidates.</param>
        /// <returns>
        ///   <c>true</c> if [has overlapping aim achievement dates] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasOverlappingAimAchievementDates(ILearningDelivery theDelivery, IReadOnlyCollection<ILearningDelivery> candidates) =>
            candidates
                .SafeAny(x => HasOverlappingAimAchievementDates(theDelivery, x));

        /// <summary>
        /// Determines whether [has overlapping aim achievement dates] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if [has overlapping aim achievement dates] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasOverlappingAimAchievementDates(ILearningDelivery theDelivery, ILearningDelivery candidate) =>
            It.IsBetween(theDelivery.LearnStartDate, candidate.LearnStartDate, candidate.AchDateNullable ?? DateTime.MaxValue);

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="theDelivery">The delivery.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery theDelivery) =>
            HandleValidationError(learnRefNumber, theDelivery.AimSeqNumber, BuildMessageParametersFor(theDelivery));

        /// <summary>
        /// Builds the message parameters for.
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns></returns>
        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery theDelivery) => new[]
        {
            BuildErrorMessageParameter(PropertyNameConstants.AimType, theDelivery.AimType),
            BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, theDelivery.LearnStartDate),
            BuildErrorMessageParameter(PropertyNameConstants.LearnActEndDate, theDelivery.LearnActEndDateNullable),
            BuildErrorMessageParameter(PropertyNameConstants.FundModel, theDelivery.FundModel),
            BuildErrorMessageParameter(PropertyNameConstants.ProgType, theDelivery.ProgTypeNullable),
            BuildErrorMessageParameter(PropertyNameConstants.AchDate, theDelivery.AchDateNullable)
        };
    }
}
