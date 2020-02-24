using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpStat
{
    public class EmpStat_17Rule :
        IRule<ILearner>
    {
        /// <summary>
        /// Gets the name of the message property.
        /// </summary>
        public const string MessagePropertyName = PropertyNameConstants.EmpStat;

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = RuleNameConstants.EmpStat_17;

        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmpStat_17Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="derivedData22">The derived data 22 rule.</param>
        /// <param name="yearData">The year data.</param>
        public EmpStat_17Rule(IValidationErrorHandler validationErrorHandler)
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
        public DateTime LastInviableDate => new DateTime(2016, 07, 31);

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
        public bool InTraining(ILearningDelivery delivery) =>
            It.IsInRange(delivery.ProgTypeNullable, TypeOfLearningProgramme.Traineeship);

        /// <summary>
        /// Determines whether [is right fund model] [for the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is right fund model] [for the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsInAProgramme(ILearningDelivery delivery) =>
            It.IsInRange(delivery.AimType, TypeOfAim.ProgrammeAim);

        /// <summary>
        /// Determines whether [has a qualifying employment status] [the specified this employment].
        /// </summary>
        /// <param name="thisEmployment">The this employment.</param>
        /// <returns>
        ///   <c>true</c> if [has a qualifying employment status] [the specified this employment]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasAQualifyingEmploymentStatus(ILearnerEmploymentStatus thisEmployment) =>
                It.IsOutOfRange(thisEmployment?.EmpStat, TypeOfEmploymentStatus.NotKnownProvided);

        /// <summary>
        /// Gets the matching employment.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="usingEmployments">using employments.</param>
        /// <returns>the latest employment date for the learning aim (if there is one)</returns>
        public ILearnerEmploymentStatus GetMatchingEmployment(ILearningDelivery delivery, IReadOnlyCollection<ILearnerEmploymentStatus> usingEmployments) =>
            usingEmployments?
                .Where(x => x.DateEmpStatApp <= delivery.LearnStartDate)
                .OrderByDescending(x => x.DateEmpStatApp)
                .FirstOrDefault();

        /// <summary>
        /// Determines whether [is not valid] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="usingEmployments">The using employments.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearningDelivery delivery, IReadOnlyCollection<ILearnerEmploymentStatus> usingEmployments) =>
            IsViableStart(delivery)
                && InTraining(delivery)
                && IsInAProgramme(delivery)
                && !HasAQualifyingEmploymentStatus(GetMatchingEmployment(delivery, usingEmployments));

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;
            var employments = objectToValidate.LearnerEmploymentStatuses.AsSafeReadOnlyList();

            objectToValidate.LearningDeliveries
                .Where(x => IsNotValid(x, employments))
                .ForEach(x => RaiseValidationMessage(learnRefNumber, x));
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
