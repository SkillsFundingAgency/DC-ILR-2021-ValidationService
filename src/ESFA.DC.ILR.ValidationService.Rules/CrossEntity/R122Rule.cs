using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    /// <summary>
    /// cross record rule 122
    /// </summary>
    /// <seealso cref="AbstractRule" />
    /// <seealso cref="Interface.IRule{ILearner}" />
    public class R122Rule :
        AbstractRule,
        IRule<ILearner>
    {
        /// <summary>
        /// The check(er, rule common operations provider)
        /// </summary>
        private readonly IProvideRuleCommonOperations _check;

        /// <summary>
        /// Initializes a new instance of the <see cref="R122Rule"/> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="commonOps">The common ops.</param>
        public R122Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideRuleCommonOperations commonOps)
            : base(validationErrorHandler, RuleNameConstants.R122)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(commonOps)
                .AsGuard<ArgumentNullException>(nameof(commonOps));

            _check = commonOps;
        }

        /// <summary>
        /// Validates the specified learner.
        /// </summary>
        /// <param name="theLearner">The learner.</param>
        public void Validate(ILearner theLearner)
        {
            It.IsNull(theLearner)
                .AsGuard<ArgumentNullException>(nameof(theLearner));

            var learnRefNumber = theLearner.LearnRefNumber;
            var fromFams = GetFAMsFrom(theLearner);
            var latestFAMDateTo = GetLatestFAMDateTo(fromFams);

            if (It.Has(latestFAMDateTo))
            {
                theLearner.LearningDeliveries
                    .ForAny(MatchesFilter, x => RaiseValidationMessage(learnRefNumber, x, latestFAMDateTo));
            }
        }

        /// <summary>
        /// Gets the fams from.
        /// </summary>
        /// <param name="theLearner">The learner.</param>
        /// <returns>a flattened collection of FAMS</returns>
        public IReadOnlyCollection<ILearningDeliveryFAM> GetFAMsFrom(ILearner theLearner) =>
            theLearner.LearningDeliveries
                .SelectMany(x => x.LearningDeliveryFAMs)
                .AsSafeReadOnlyList();

        /// <summary>
        /// Gets the latest fam date to.
        /// </summary>
        /// <param name="fromMonitors">From monitors.</param>
        /// <returns>the latest FAM date to (if there is one)</returns>
        public DateTime? GetLatestFAMDateTo(IReadOnlyCollection<ILearningDeliveryFAM> fromMonitors) =>
            fromMonitors
                .Where(IsQualifyingMonitoringType)
                .OrderByDescending(x => x.LearnDelFAMDateFromNullable)
                .FirstOrDefault()?.LearnDelFAMDateToNullable;

        /// <summary>
        /// Determines whether [is qualifying monitoring type] [the specified monitor].
        /// </summary>
        /// <param name="theMonitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [is qualifying monitoring type] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsQualifyingMonitoringType(ILearningDeliveryFAM theMonitor) =>
            theMonitor.LearnDelFAMType.ComparesWith(Monitoring.Delivery.Types.ApprenticeshipContract);

        /// <summary>
        /// Matches the filter.
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>true if it does...</returns>
        public bool MatchesFilter(ILearningDelivery theDelivery) =>
            HasQualifyingModel(theDelivery)
            && IsStandardApprenticeship(theDelivery)
            && HasApprenticeshipContract(theDelivery)
            && !HasAchievementDate(theDelivery);

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
        /// Determines whether [is standard apprenticeship] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is standard apprenticeship] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsStandardApprenticeship(ILearningDelivery theDelivery) =>
            _check.IsStandardApprenticeship(theDelivery);

        /// <summary>
        /// Determines whether [has apprenticeship contract] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has apprenticeship contract] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasApprenticeshipContract(ILearningDelivery theDelivery) =>
            _check.CheckDeliveryFAMs(theDelivery, IsApprenticeshipContract);

        /// <summary>
        /// Determines whether [is apprenticeship contract] [the specified monitor].
        /// </summary>
        /// <param name="theMonitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [is apprenticeship contract] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsApprenticeshipContract(ILearningDeliveryFAM theMonitor) =>
            It.IsInRange(theMonitor.LearnDelFAMType, Monitoring.Delivery.Types.ApprenticeshipContract);

        /// <summary>
        /// Determines whether [has achievement date] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has achievement date] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasAchievementDate(ILearningDelivery theDelivery) =>
            It.Has(theDelivery.AchDateNullable);

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="theDelivery">The delivery.</param>
        /// <param name="latestFamDateTo">The latest fam date to.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery theDelivery, DateTime? latestFamDateTo) =>
            HandleValidationError(learnRefNumber, theDelivery.AimSeqNumber, BuildMessageParametersFor(theDelivery, latestFamDateTo));

        /// <summary>
        /// Builds the message parameters for.
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <param name="latestFamDateTo">The latest fam date to.</param>
        /// <returns>a message parameter collection</returns>
        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery theDelivery, DateTime? latestFamDateTo) => new[]
        {
            BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMType, Monitoring.Delivery.Types.ApprenticeshipContract),
            BuildErrorMessageParameter(PropertyNameConstants.LearnDelFAMDateTo, latestFamDateTo),
            BuildErrorMessageParameter(PropertyNameConstants.AchDate, theDelivery.AchDateNullable)
        };
    }
}
