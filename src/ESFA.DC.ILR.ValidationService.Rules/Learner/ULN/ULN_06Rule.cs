using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.ULN
{
    /// <summary>
    /// the unique learner number rule 10
    /// </summary>
    /// <seealso cref="AbstractRule" />
    /// <seealso cref="Interface.IRule{ILearner}" />
    public class ULN_06Rule :
        AbstractRule,
        IRule<ILearner>
    {
        /// <summary>
        /// The date time query (service)
        /// </summary>
        private readonly IDateTimeQueryService _dateTimeQuery;

        /// <summary>
        /// The check(er, common rule operations provider)
        /// </summary>
        private readonly IProvideRuleCommonOperations _check;

        /// <summary>
        /// Initializes a new instance of the <see cref="ULN_06Rule"/> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="academicDataQueryService">The academic data query service.</param>
        /// <param name="dateTimeQueryService">The date time query service.</param>
        /// <param name="fileDataService">The file data service.</param>
        /// <param name="commonOps">The common ops.</param>
        public ULN_06Rule(
            IValidationErrorHandler validationErrorHandler,
            IAcademicYearDataService academicDataQueryService,
            IDateTimeQueryService dateTimeQueryService,
            IFileDataService fileDataService,
            IProvideRuleCommonOperations commonOps)
            : base(validationErrorHandler, RuleNameConstants.ULN_06)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(academicDataQueryService)
                .AsGuard<ArgumentNullException>(nameof(academicDataQueryService));
            It.IsNull(dateTimeQueryService)
                .AsGuard<ArgumentNullException>(nameof(dateTimeQueryService));
            It.IsNull(fileDataService)
                .AsGuard<ArgumentNullException>(nameof(fileDataService));
            It.IsNull(commonOps)
                .AsGuard<ArgumentNullException>(nameof(commonOps));

            FilePreparationDate = fileDataService.FilePreparationDate();
            FirstJanuary = academicDataQueryService.JanuaryFirst();

            _dateTimeQuery = dateTimeQueryService;
            _check = commonOps;
        }

        /// <summary>
        /// The minimum course duration
        /// </summary>
        public const int MinimumCourseDuration = 5; // (days)

        /// <summary>
        /// The rule leniency period
        /// </summary>
        public const int RuleLeniencyPeriod = 60; // (days)

        /// <summary>
        /// Gets the file preparation date.
        /// </summary>
        public DateTime FilePreparationDate { get; }

        /// <summary>
        /// Gets the first january (for the current educational year).
        /// </summary>
        public DateTime FirstJanuary { get; }

        /// <summary>
        /// Validates the specified learner.
        /// </summary>
        /// <param name="theLearner">The learner.</param>
        public void Validate(ILearner theLearner)
        {
            It.IsNull(theLearner)
                .AsGuard<ArgumentNullException>(nameof(theLearner));

            if (IsOutsideQualifyingPeriod() || IsValidULN(theLearner))
            {
                return;
            }

            var learnRefNumber = theLearner.LearnRefNumber;

            theLearner.LearningDeliveries
                .ForAny(IsNotValid, x => RaiseValidationMessage(learnRefNumber, x));
        }

        /// <summary>
        /// Determines whether [is outside qualifying period].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is outside qualifying period]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsOutsideQualifyingPeriod() =>
            FirstJanuary > FilePreparationDate;

        /// <summary>
        /// Determines whether [is valid uln] [for the specified learner].
        /// </summary>
        /// <param name="theLearner">The learner.</param>
        /// <returns>
        ///   <c>true</c> if [is valid uln] [for the specified learner]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValidULN(ILearner theLearner) =>
            theLearner.ULN != ValidationConstants.TemporaryULN;

        /// <summary>
        /// Determines whether [is not valid] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearningDelivery theDelivery) =>
            !IsExcluded(theDelivery)
            && (HasQualifyingModel(theDelivery)
                || (IsNotFundedByESFA(theDelivery) && HasAdvancedLearnerLoan(theDelivery)))
            && (HasQualifyingPlannedDuration(theDelivery)
                || (HasActualEndDate(theDelivery) && HasQualifyingActualDuration(theDelivery)))
            && IsInsideLeniencyPeriod(theDelivery);

        /// <summary>
        /// Determines whether the specified the delivery is excluded.
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if the specified the delivery is excluded; otherwise, <c>false</c>.
        /// </returns>
        public bool IsExcluded(ILearningDelivery theDelivery) =>
            IsLearnerInCustody(theDelivery)
            || IsLevyFundedApprenticeship(theDelivery);

        /// <summary>
        /// Determines whether [is learner in custody] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is learner in custody] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsLearnerInCustody(ILearningDelivery theDelivery) =>
            _check.IsLearnerInCustody(theDelivery);

        /// <summary>
        /// Determines whether [is levy funded apprenticeship] [the specified monitor].
        /// </summary>
        /// <param name="theMonitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [is levy funded apprenticeship] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsLevyFundedApprenticeship(ILearningDeliveryFAM theMonitor) =>
            It.IsInRange($"{theMonitor.LearnDelFAMType}{theMonitor.LearnDelFAMCode}", Monitoring.Delivery.ApprenticeshipFundedThroughAContractForServicesWithEmployer);

        /// <summary>
        /// Determines whether [is levy funded apprenticeship] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is levy funded apprenticeship] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsLevyFundedApprenticeship(ILearningDelivery theDelivery) =>
            _check.CheckDeliveryFAMs(theDelivery, IsLevyFundedApprenticeship);

        /// <summary>
        /// Determines whether [has qualifying model] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying model] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingModel(ILearningDelivery theDelivery) =>
            _check.HasQualifyingFunding(
                theDelivery,
                TypeOfFunding.Age16To19ExcludingApprenticeships,
                TypeOfFunding.AdultSkills,
                TypeOfFunding.ApprenticeshipsFrom1May2017,
                TypeOfFunding.EuropeanSocialFund,
                TypeOfFunding.OtherAdult,
                TypeOfFunding.Other16To19);

        /// <summary>
        /// Determines whether [is not funded by esfa] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is not funded by esfa] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotFundedByESFA(ILearningDelivery theDelivery) =>
            _check.HasQualifyingFunding(
                theDelivery,
                TypeOfFunding.NotFundedByESFA);

        /// <summary>
        /// Determines whether [is financed by advanced learner loans] [the specified monitor].
        /// </summary>
        /// <param name="theMonitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [is financed by advanced learner loans] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsFinancedByAdvancedLearnerLoans(ILearningDeliveryFAM theMonitor) =>
            It.IsInRange($"{theMonitor.LearnDelFAMType}{theMonitor.LearnDelFAMCode}", Monitoring.Delivery.FinancedByAdvancedLearnerLoans);

        /// <summary>
        /// Determines whether [has advanced learner loan] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has advanced learner loan] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasAdvancedLearnerLoan(ILearningDelivery theDelivery) =>
            _check.CheckDeliveryFAMs(theDelivery, IsFinancedByAdvancedLearnerLoans);

        /// <summary>
        /// Determines whether [has qualifying planned duration] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying planned duration] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingPlannedDuration(ILearningDelivery theDelivery) =>
            _dateTimeQuery.DaysBetween(theDelivery.LearnStartDate, theDelivery.LearnPlanEndDate) >= MinimumCourseDuration;

        /// <summary>
        /// Determines whether [has actual end date] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has actual end date] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasActualEndDate(ILearningDelivery theDelivery) =>
            It.Has(theDelivery.LearnActEndDateNullable);

        /// <summary>
        /// Determines whether [has qualifying actual duration] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying actual duration] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingActualDuration(ILearningDelivery theDelivery) =>
            _dateTimeQuery.DaysBetween(theDelivery.LearnStartDate, (DateTime)theDelivery.LearnActEndDateNullable) >= MinimumCourseDuration;

        /// <summary>
        /// Determines whether [is inside leniency period] [the specified delivery].
        /// </summary>
        /// <param name="theDelivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is inside leniency period] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsInsideLeniencyPeriod(ILearningDelivery theDelivery) =>
            _dateTimeQuery.DaysBetween(theDelivery.LearnStartDate, FilePreparationDate) <= RuleLeniencyPeriod;

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
        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery theDelivery) =>
            new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.ULN, ValidationConstants.TemporaryULN),
                BuildErrorMessageParameter(PropertyNameConstants.FilePreparationDate, FilePreparationDate),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, theDelivery.LearnStartDate),
            };
    }
}