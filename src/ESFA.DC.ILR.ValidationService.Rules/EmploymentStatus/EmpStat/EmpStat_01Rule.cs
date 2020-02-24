using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal.AcademicYear.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpStat
{
    public class EmpStat_01Rule :
        IRule<ILearner>
    {
        /// <summary>
        /// Gets the name of the message property.
        /// </summary>
        public const string MessagePropertyName = PropertyNameConstants.EmpStat;

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = RuleNameConstants.EmpStat_01;

        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// The derived data 07 (rule)
        /// </summary>
        private readonly IDerivedData_07Rule _derivedData07;

        /// <summary>
        /// The (academic) year data (service)
        /// </summary>
        private readonly IAcademicYearDataService _yearData;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmpStat_01Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="derivedData07">The derived data 07 rule.</param>
        /// <param name="yearData">The year data.</param>
        public EmpStat_01Rule(
            IValidationErrorHandler validationErrorHandler,
            IDerivedData_07Rule derivedData07,
            IAcademicYearDataService yearData)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(derivedData07)
                .AsGuard<ArgumentNullException>(nameof(derivedData07));
            It.IsNull(yearData)
                .AsGuard<ArgumentNullException>(nameof(yearData));

            _messageHandler = validationErrorHandler;
            _derivedData07 = derivedData07;
            _yearData = yearData;
        }

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public string RuleName => Name;

        /// <summary>
        /// Gets the first viable date.
        /// </summary>
        public DateTime FirstViableDate => new DateTime(2012, 08, 01);

        /// <summary>
        /// Gets the last viable date.
        /// </summary>
        public DateTime LastViableDate => new DateTime(2014, 07, 31);

        /// <summary>
        /// Gets the last inviable age.
        /// </summary>
        public TimeSpan LastInviableAge => new TimeSpan(6939, 0, 0, 0); // 1 full day under 19 years

        /// <summary>
        /// Checks the delivery fams.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="matchCondition">The match condition.</param>
        /// <returns>true if any of the delivery fams match the condition</returns>
        public bool CheckDeliveryFAMs(ILearningDelivery delivery, Func<ILearningDeliveryFAM, bool> matchCondition) =>
            delivery.LearningDeliveryFAMs.SafeAny(matchCondition);

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
        /// Determines whether [is learner in custody] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is learner in custody] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsLearnerInCustody(ILearningDelivery delivery) =>
            CheckDeliveryFAMs(delivery, IsLearnerInCustody);

        /// <summary>
        /// Determines whether [is comunity learning fund] [the specified monitor].
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [is comunity learning fund] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsComunityLearningFund(ILearningDeliveryFAM monitor) =>
            It.IsInRange($"{monitor.LearnDelFAMType}{monitor.LearnDelFAMCode}", Monitoring.Delivery.LocalAuthorityCommunityLearningFunds);

        /// <summary>
        /// Determines whether [is comunity learning fund] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is comunity learning fund] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsComunityLearningFund(ILearningDelivery delivery) =>
            CheckDeliveryFAMs(delivery, IsComunityLearningFund);

        /// <summary>
        /// Determines whether [is not funded by esfa] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is not funded by esfa] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotFundedByESFA(ILearningDelivery delivery) =>
            It.IsInRange(delivery.FundModel, TypeOfFunding.NotFundedByESFA);

        /// <summary>
        /// Determines whether [is not qualifying funding] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is not qualifying funding] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotQualifiedFunding(ILearningDelivery delivery) =>
            IsNotFundedByESFA(delivery) && IsComunityLearningFund(delivery);

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
        /// Determines whether [in training] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [in training] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool InTraining(ILearningDelivery delivery) =>
            It.IsInRange(delivery.ProgTypeNullable, TypeOfLearningProgramme.Traineeship);

        /// <summary>
        /// Determines whether the specified delivery is excluded.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if the specified delivery is excluded; otherwise, <c>false</c>.
        /// </returns>
        public bool IsExcluded(ILearningDelivery delivery) =>
            IsLearnerInCustody(delivery)
            || IsNotQualifiedFunding(delivery)
            || IsApprenticeship(delivery)
            || InTraining(delivery);

        /// <summary>
        /// Determines whether the specified delivery is funded.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if the specified delivery is funded; otherwise, <c>false</c>.
        /// </returns>
        public bool IsQualifyingFunding(ILearningDelivery delivery) =>
            It.IsInRange(delivery.FundModel, TypeOfFunding.AdultSkills, TypeOfFunding.OtherAdult, TypeOfFunding.NotFundedByESFA);

        /// <summary>
        /// Determines whether [is qualifying aim] [the specified delivery].
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is qualifying aim] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsQualifyingAim(ILearningDelivery delivery) =>
            It.IsBetween(delivery.LearnStartDate, FirstViableDate, LastViableDate);

        /// <summary>
        /// Gets the year of learning commentment date.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>a date time representing the 31 august for the year of learning</returns>
        public DateTime GetYearOfLearningCommencementDate(DateTime candidate) =>
            _yearData.GetAcademicYearOfLearningDate(candidate, AcademicYearDates.PreviousYearEnd);

        /// <summary>
        /// Determines whether [is qualifying age] [the specified learner].
        /// </summary>
        /// <param name="learner">The learner.</param>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is qualifying age] [the specified learner]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsQualifyingAge(ILearner learner, ILearningDelivery delivery) =>
            It.Has(learner.DateOfBirthNullable) && ((GetYearOfLearningCommencementDate(delivery.LearnStartDate) - learner.DateOfBirthNullable.Value) > LastInviableAge);

        /// <summary>
        /// Determines whether [has qualifying employment status] [the specified e status].
        /// </summary>
        /// <param name="eStatus">The e status.</param>
        /// <param name="learningStartDate">The learning start date.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying employment status] [the specified e status]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingEmploymentStatus(ILearnerEmploymentStatus eStatus, DateTime learningStartDate) =>
            eStatus.DateEmpStatApp <= learningStartDate;

        /// <summary>
        /// Determines whether [has qualifying employment status] [the specified learner].
        /// </summary>
        /// <param name="learner">The learner.</param>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying employment status] [the specified learner]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingEmploymentStatus(ILearner learner, ILearningDelivery delivery) =>
            learner.LearnerEmploymentStatuses.SafeAny(x => HasQualifyingEmploymentStatus(x, delivery.LearnStartDate));

        /// <summary>
        /// Determines whether [is not valid] [the specified learner].
        /// </summary>
        /// <param name="learner">The learner.</param>
        /// <param name="delivery">The delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified learner]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearner learner, ILearningDelivery delivery) =>
            !IsExcluded(delivery)
                && IsQualifyingAge(learner, delivery)
                && IsQualifyingFunding(delivery)
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
                .ForEach(x => RaiseValidationMessage(objectToValidate, x));
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learner">The learner.</param>
        /// <param name="thisDelivery">this delivery.</param>
        public void RaiseValidationMessage(ILearner learner, ILearningDelivery thisDelivery)
        {
            var parameters = Collection.Empty<IErrorMessageParameter>();
            parameters.Add(_messageHandler.BuildErrorMessageParameter(MessagePropertyName, "(missing)"));
            parameters.Add(_messageHandler.BuildErrorMessageParameter(PropertyNameConstants.FundModel, thisDelivery.FundModel));
            parameters.Add(_messageHandler.BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, thisDelivery.LearnStartDate));
            parameters.Add(_messageHandler.BuildErrorMessageParameter(PropertyNameConstants.DateOfBirth, learner.DateOfBirthNullable));

            _messageHandler.Handle(RuleName, learner.LearnRefNumber, thisDelivery.AimSeqNumber, parameters);
        }
    }
}
