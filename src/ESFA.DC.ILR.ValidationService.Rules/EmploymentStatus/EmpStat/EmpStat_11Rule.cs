using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpStat
{
    public class EmpStat_11Rule :
        IRule<ILearner>
    {
        /// <summary>
        /// The pastoral hours threshold
        /// </summary>
        public const int PastoralHoursThreshold = 540;

        /// <summary>
        /// Gets the name of the message property.
        /// </summary>
        public const string MessagePropertyName = PropertyNameConstants.EmpStat;

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = RuleNameConstants.EmpStat_11;

        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmpStat_11Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="derivedData22">The derived data 22 rule.</param>
        /// <param name="yearData">The year data.</param>
        public EmpStat_11Rule(IValidationErrorHandler validationErrorHandler)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));

            _messageHandler = validationErrorHandler;
        }

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public string RuleName => Name;

        /// <summary>
        /// Gets the last inviable date.
        /// </summary>
        public DateTime LastInviableDate => new DateTime(2014, 07, 31);

        /// <summary>
        /// Determines whether [in training] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [in training] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool InTraining(ILearningDelivery delivery) =>
            It.IsInRange(delivery.ProgTypeNullable, TypeOfLearningProgramme.Traineeship);

        /// <summary>
        /// Determines whether [is viable start] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is viable start] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsViableStart(ILearningDelivery delivery) =>
            delivery.LearnStartDate > LastInviableDate;

        /// <summary>
        /// Determines whether [in training] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [in training] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingFunding(ILearningDelivery delivery) =>
            It.IsInRange(delivery.FundModel, TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfFunding.Other16To19);

        /// <summary>
        /// Gets the matching employment.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="usingEmployments">using employments.</param>
        /// <returns>the latest employment date for the learning aim (if there is one)</returns>
        public bool HasAQualifyingEmploymentStatus(ILearningDelivery delivery, IReadOnlyCollection<ILearnerEmploymentStatus> usingEmployments) =>
            usingEmployments
                .SafeAny(x => x.DateEmpStatApp < delivery.LearnStartDate);

        /// <summary>
        /// Determines whether [is not valid] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="usingEmployments">The using employments.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearningDelivery delivery, IReadOnlyCollection<ILearnerEmploymentStatus> usingEmployments) =>
            !InTraining(delivery)
                && IsViableStart(delivery)
                && HasQualifyingFunding(delivery)
                && !HasAQualifyingEmploymentStatus(delivery, usingEmployments);

        /// <summary>
        /// Gets the qualifying hours.
        /// </summary>
        /// <param name="learner">The learner.</param>
        /// <returns>the total of hours study</returns>
        public int GetQualifyingHours(ILearner learner) =>
            (learner.PlanEEPHoursNullable ?? 0) + (learner.PlanLearnHoursNullable ?? 0);

        /// <summary>
        /// Determines whether [has qualifying hours] [the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying hours] [the specified candidate]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingHours(int candidate) =>
            candidate < PastoralHoursThreshold;

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            if (HasQualifyingHours(GetQualifyingHours(objectToValidate)))
            {
                var learnRefNumber = objectToValidate.LearnRefNumber;
                var employments = objectToValidate.LearnerEmploymentStatuses.AsSafeReadOnlyList();

                objectToValidate.LearningDeliveries
                    .Where(x => IsNotValid(x, employments))
                    .ForEach(x => RaiseValidationMessage(learnRefNumber, x));
            }
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="thisDelivery">The this delivery.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            var parameters = Collection.Empty<IErrorMessageParameter>();
            parameters.Add(_messageHandler.BuildErrorMessageParameter(MessagePropertyName, TypeOfEmploymentStatus.NotKnownProvided));
            parameters.Add(_messageHandler.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, thisDelivery.LearnStartDate));

            _messageHandler.Handle(RuleName, learnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}
