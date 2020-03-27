using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpStat
{
    public class EmpStat_15Rule :
        IRule<ILearner>
    {
        /// <summary>
        /// Gets the name of the message property.
        /// </summary>
        public const string MessagePropertyName = PropertyNameConstants.EmpStat;

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = RuleNameConstants.EmpStat_15;

        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// The derived data 07 (rule)
        /// </summary>
        private readonly IDerivedData_07Rule _derivedData07;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmpStat_15Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="derivedData07">The derived data 07 rule.</param>
        public EmpStat_15Rule(
            IValidationErrorHandler validationErrorHandler,
            IDerivedData_07Rule derivedData07)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(derivedData07)
                .AsGuard<ArgumentNullException>(nameof(derivedData07));

            _messageHandler = validationErrorHandler;
            _derivedData07 = derivedData07;
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
        /// Determines whether the specified delivery is apprenticeship.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if the specified delivery is apprenticeship; otherwise, <c>false</c>.
        /// </returns>
        public bool IsApprenticeship(ILearningDelivery delivery) =>
            _derivedData07.IsApprenticeship(delivery.ProgTypeNullable);

        /// <summary>
        /// Determines whether [in a programme] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [in a programme] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool InAProgramme(ILearningDelivery delivery) =>
            It.IsInRange(delivery.AimType, TypeOfAim.ProgrammeAim);

        /// <summary>
        /// Determines whether [is qualifying aim] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is qualifying aim] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsQualifyingAim(ILearningDelivery delivery) =>
            delivery.LearnStartDate > LastInviableDate;

        /// <summary>
        /// Determines whether [has qualifying employment status] [the specified employment status].
        /// </summary>
        /// <param name="eStatus">The e status.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying employment status] [the specified employment status]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingEmploymentStatus(ILearnerEmploymentStatus eStatus) =>
            It.IsOutOfRange(eStatus?.EmpStat, TypeOfEmploymentStatus.NotKnownProvided);

        /// <summary>
        /// Gets the qualifying employment status.
        /// </summary>
        /// <param name="learner">The learner.</param>
        /// <param name="delivery">The delivery.</param>
        /// <returns>returns the latest applicable employment status</returns>
        public ILearnerEmploymentStatus GetQualifyingEmploymentStatus(ILearner learner, ILearningDelivery delivery) =>
            learner.LearnerEmploymentStatuses
                .SafeWhere(x => x.DateEmpStatApp <= delivery.LearnStartDate)
                .OrderByDescending(x => x.DateEmpStatApp)
                .FirstOrDefault();

        /// <summary>
        /// Determines whether [has qualifying employment status] [the specified learner].
        /// </summary>
        /// <param name="learner">The learner.</param>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying employment status] [the specified learner]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingEmploymentStatus(ILearner learner, ILearningDelivery delivery) =>
            HasQualifyingEmploymentStatus(GetQualifyingEmploymentStatus(learner, delivery));

        /// <summary>
        /// Determines whether [is not valid] [the specified learner].
        /// </summary>
        /// <param name="learner">The learner.</param>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified learner]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearner learner, ILearningDelivery delivery) =>
            IsApprenticeship(delivery)
                && InAProgramme(delivery)
                && IsQualifyingAim(delivery)
                && !HasQualifyingEmploymentStatus(learner, delivery);

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearningDeliveries
                .SafeWhere(x => IsNotValid(objectToValidate, x))
                .ForEach(x => RaiseValidationMessage(learnRefNumber, x));
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="thisDelivery">this delivery.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            var parameters = Collection.Empty<IErrorMessageParameter>();
            parameters.Add(_messageHandler.BuildErrorMessageParameter(MessagePropertyName, TypeOfEmploymentStatus.NotKnownProvided));
            parameters.Add(_messageHandler.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, thisDelivery.LearnStartDate));

            _messageHandler.Handle(RuleName, learnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}
