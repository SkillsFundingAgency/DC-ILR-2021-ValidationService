using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpId
{
    public class EmpStat_07Rule :
        AbstractRule,
        IRule<ILearner>
    {
        /// <summary>
        /// The check (rule common operations provider)
        /// </summary>
        private readonly IProvideRuleCommonOperations _check;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmpStat_07Rule"/> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="commonOperations">The common operations.</param>
        public EmpStat_07Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideRuleCommonOperations commonOperations)
            : base(validationErrorHandler, RuleNameConstants.EmpStat_07)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(commonOperations)
                .AsGuard<ArgumentNullException>(nameof(commonOperations));

            _check = commonOperations;
        }

        /// <summary>
        /// Gets the last viable date.
        /// </summary>
        public static DateTime LastViableDate => new DateTime(2013, 07, 31);

        /// <summary>
        /// Gets the planned total qualifying hours
        /// </summary>
        public static int PlannedTotalQualifyingHours => 540;

        /// <summary>
        /// Validates this learner.
        /// </summary>
        /// <param name="thisLearner">this learner.</param>
        public void Validate(ILearner thisLearner)
        {
            It.IsNull(thisLearner)
                .AsGuard<ArgumentNullException>(nameof(thisLearner));

            if (!HasQualifyingLearningHours(GetLearningHoursTotal(thisLearner)))
            {
                return;
            }

            var employments = thisLearner.LearnerEmploymentStatuses.ToReadOnlyCollection();

            thisLearner.LearningDeliveries
                .ForAny(
                    x => IsNotValid(x, GetEmploymentStatusOn(x.LearnStartDate, employments)),
                    x => RaiseValidationMessage(thisLearner, x));
        }

        /// <summary>
        /// Gets the usable value.
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>the int or zero</returns>
        public int GetUsableValue(int? candidate) =>
            candidate ?? 0;

        /// <summary>
        /// Gets the learning hours total.
        /// </summary>
        /// <param name="thisLearner">The this learner.</param>
        /// <returns>the total learning hours</returns>
        public int GetLearningHoursTotal(ILearner thisLearner) =>
            GetUsableValue(thisLearner.PlanLearnHoursNullable)
            + GetUsableValue(thisLearner.PlanEEPHoursNullable);

        /// <summary>
        /// Determines whether [has qualifying learning hours] [the specified candidate].
        /// </summary>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying learning hours] [the specified candidate]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingLearningHours(int candidate) =>
            candidate < PlannedTotalQualifyingHours;

        /// <summary>
        /// Gets the employment status on (this date) (from employments).
        /// </summary>
        /// <param name="thisDate">this date.</param>
        /// <param name="fromEmployments">from employments.</param>
        /// <returns>the closest learner employmentstatus for the learn start date</returns>
        public ILearnerEmploymentStatus GetEmploymentStatusOn(DateTime thisDate, IReadOnlyCollection<ILearnerEmploymentStatus> fromEmployments) =>
            _check.GetEmploymentStatusOn(thisDate, fromEmployments);

        /// <summary>
        /// Determines whether [is not valid] [this delivery].
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <param name="thisEmployment">this employment.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [this delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearningDelivery thisDelivery, ILearnerEmploymentStatus thisEmployment) =>
            HasQualifyingFunding(thisDelivery)
            && HasQualifyingStart(thisDelivery)
            && !HasQualifyingEmployment(thisEmployment);

        /// <summary>
        /// Determines whether [has qualifying funding] [this delivery].
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying funding] [this delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingFunding(ILearningDelivery thisDelivery) =>
            _check.HasQualifyingFunding(thisDelivery, TypeOfFunding.Age16To19ExcludingApprenticeships, TypeOfFunding.Other16To19);

        /// <summary>
        /// Determines whether [has qualifying start] [this delivery].
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying start] [this delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingStart(ILearningDelivery thisDelivery) =>
            _check.HasQualifyingStart(thisDelivery, DateTime.MinValue, LastViableDate);

        /// <summary>
        /// Determines whether [has qualifying employment] [this employment].
        /// </summary>
        /// <param name="thisEmployment">this employment.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying employment] [this employment]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingEmployment(ILearnerEmploymentStatus thisEmployment) =>
            It.Has(thisEmployment);

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="thisLearner">The this learner.</param>
        /// <param name="thisDelivery">this delivery.</param>
        public void RaiseValidationMessage(ILearner thisLearner, ILearningDelivery thisDelivery)
        {
            HandleValidationError(thisLearner.LearnRefNumber, thisDelivery.AimSeqNumber, BuildMessageParametersFor(thisLearner, thisDelivery));
        }

        /// <summary>
        /// Builds the message parameters for.
        /// </summary>
        /// <param name="thisLearner">The this learner.</param>
        /// <param name="thisDelivery">this delivery.</param>
        /// <returns>
        /// a collection of message parameters
        /// </returns>
        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearner thisLearner, ILearningDelivery thisDelivery)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.PlanLearnHours, thisLearner.PlanLearnHoursNullable),
                BuildErrorMessageParameter(PropertyNameConstants.PlanEEPHours, thisLearner.PlanEEPHoursNullable),
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, thisDelivery.LearnStartDate)
            };
        }
    }
}
