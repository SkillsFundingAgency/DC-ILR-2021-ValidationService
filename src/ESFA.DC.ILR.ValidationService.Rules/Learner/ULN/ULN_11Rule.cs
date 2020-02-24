using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.File.FileData.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;

namespace ESFA.DC.ILR.ValidationService.Rules.Learner.ULN
{
    public class ULN_11Rule :
        IRule<ILearner>
    {
        /// <summary>
        /// Gets the name of the message property.
        /// </summary>
        public const string MessagePropertyName = "ULN";

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = RuleNameConstants.ULN_11;

        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// The file data service
        /// </summary>
        private readonly IFileDataService _fileDataService;

        /// <summary>
        /// The year service
        /// </summary>
        private readonly IAcademicYearDataService _yearService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ULN_11Rule"/> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="fileDataService">The file data service.</param>
        /// <param name="yearService">The validation data service.</param>
        public ULN_11Rule(
            IValidationErrorHandler validationErrorHandler,
            IFileDataService fileDataService,
            IAcademicYearDataService yearService)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(fileDataService)
                .AsGuard<ArgumentNullException>(nameof(fileDataService));
            It.IsNull(yearService)
                .AsGuard<ArgumentNullException>(nameof(yearService));

            _messageHandler = validationErrorHandler;
            _fileDataService = fileDataService;
            _yearService = yearService;
        }

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public string RuleName => Name;

        /// <summary>
        /// Gets five days.
        /// </summary>
        public TimeSpan FiveDays => new TimeSpan(5, 0, 0, 0); // 5 days

        /// <summary>
        /// Gets sixty days.
        /// </summary>
        public TimeSpan SixtyDays => new TimeSpan(60, 0, 0, 0); // 60 days

        /// <summary>
        /// Checks the delivery fams.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="matchCondition">The match condition.</param>
        /// <returns>true if any of the delivery fams match the condition</returns>
        public bool CheckDeliveryFAMs(ILearningDelivery delivery, Func<ILearningDeliveryFAM, bool> matchCondition) =>
            delivery.LearningDeliveryFAMs.SafeAny(matchCondition);

        /// <summary>
        /// Checks the learning deliveries.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <param name="matchCondition">The matching condition.</param>
        /// <returns>true if any of the deliveries match the condition</returns>
        public bool CheckLearningDeliveries(ILearner candidate, Func<ILearningDelivery, bool> matchCondition) =>
            candidate.LearningDeliveries.SafeAny(matchCondition);

        /// <summary>
        /// Determines whether [is externally funded] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is externally funded] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsExternallyFunded(ILearningDelivery delivery) =>
            It.IsInRange(delivery.FundModel, TypeOfFunding.NotFundedByESFA);

        /// <summary>
        /// Determines whether [is hefce funded] [the specified monitor].
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [is hefce funded] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsHEFCEFunded(ILearningDeliveryFAM monitor) =>
            It.IsInRange($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}", Monitoring.Delivery.HigherEducationFundingCouncilEngland);

        /// <summary>
        /// Determines whether [is hefce funded] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is hefce funded] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsHEFCEFunded(ILearningDelivery delivery) =>
            CheckDeliveryFAMs(delivery, IsHEFCEFunded);

        /// <summary>
        /// Determines whether [is short course] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is short course] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsShortCourse(ILearningDelivery delivery) =>
            IsPlannedShortCourse(delivery) || IsCompletedShortCourse(delivery);

        /// <summary>
        /// Determines whether [is planned short course] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is planned short course] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsPlannedShortCourse(ILearningDelivery delivery) =>
            (delivery.LearnPlanEndDate - delivery.LearnStartDate) < FiveDays;

        /// <summary>
        /// Determines whether [is completed short course] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is completed short course] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsCompletedShortCourse(ILearningDelivery delivery) =>
            It.Has(delivery.LearnActEndDateNullable)
                && ((delivery.LearnActEndDateNullable.Value - delivery.LearnStartDate) < FiveDays);

        /// <summary>
        /// Determines whether [has exceed registration period] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has exceed registration period] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasExceedRegistrationPeriod(ILearningDelivery delivery) =>
            (_fileDataService.FilePreparationDate() - delivery.LearnStartDate) > SixtyDays;

        /// <summary>
        /// Determines whether [is inside general registration threshold].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is inside general registration threshold]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsInsideGeneralRegistrationThreshold() => _fileDataService.FilePreparationDate() < _yearService.JanuaryFirst();

        /// <summary>
        /// Determines whether [is registered learner] [the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if [is registered learner] [the specified candidate]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsRegisteredLearner(ILearner candidate) =>
            candidate.ULN != 9999999999;

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
        /// Determines whether the specified candidate is excluded.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if the specified candidate is excluded; otherwise, <c>false</c>.
        /// </returns>
        public bool IsExcluded(ILearner candidate)
        {
            return IsInsideGeneralRegistrationThreshold()
                || IsRegisteredLearner(candidate)
                || CheckLearningDeliveries(candidate, x => CheckDeliveryFAMs(x, IsLearnerInCustody));
        }

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            if (IsExcluded(objectToValidate))
            {
                return;
            }

            ValidateDeliveries(objectToValidate);
        }

        public void ValidateDeliveries(ILearner objectToValidate)
        {
            var learnRefNumber = objectToValidate.LearnRefNumber;

            /*
                the file preparation date >= 1 january of the current teaching year             <= has exceeded the general registration threshold
                and Learner.ULN = 9999999999                                                    <= and is unregistered
                and LearningDelivery.FundModel = 99                                             <= and is externally funded
                and (LearningDeliveryFAM.LearnDelFAMType = SOF
                    and LearningDeliveryFAM.LearnDelFAMCode = 1)                                <= and is HEFCE funded
                and (LearningDelivery.LearnPlanEndDate - LearningDelivery.LearnStartDate >= 5
                    OR LearningDelivery.LearnActEndDate - LearningDelivery.LearnStartDate >= 5) <= and is not a (planned or completed) short course (< 5 days)
                and (file preparation date - LearningDelivery.LearnStartDate) > 60 days         <= and has exceeded delivery registration threshold
             */

            objectToValidate.LearningDeliveries
                .SafeWhere(x => IsExternallyFunded(x) && IsHEFCEFunded(x) && !IsShortCourse(x))
                .ForEach(x =>
                {
                    var failedValidation = HasExceedRegistrationPeriod(x);

                    if (failedValidation)
                    {
                        RaiseValidationMessage(learnRefNumber, x);
                    }
                });
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="thisDelivery">this delivery.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            var parameters = Collection.Empty<IErrorMessageParameter>();
            parameters.Add(_messageHandler.BuildErrorMessageParameter(MessagePropertyName, thisDelivery));

            _messageHandler.Handle(RuleName, learnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}