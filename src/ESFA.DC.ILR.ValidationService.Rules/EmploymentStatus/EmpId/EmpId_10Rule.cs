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
    public class EmpId_10Rule :
        AbstractRule,
        IRule<ILearner>
    {
        /// <summary>
        /// The check (rule common operations provider)
        /// </summary>
        private readonly IProvideRuleCommonOperations _check;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmpId_10Rule"/> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="commonOperations">The common operations.</param>
        public EmpId_10Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideRuleCommonOperations commonOperations)
            : base(validationErrorHandler, RuleNameConstants.EmpId_10)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(commonOperations)
                .AsGuard<ArgumentNullException>(nameof(commonOperations));

            _check = commonOperations;
        }

        /// <summary>
        /// Validates this learner.
        /// </summary>
        /// <param name="thisLearner">this learner.</param>
        public void Validate(ILearner thisLearner)
        {
            It.IsNull(thisLearner)
                .AsGuard<ArgumentNullException>(nameof(thisLearner));

            var employments = thisLearner.LearnerEmploymentStatuses.ToReadOnlyCollection();

            thisLearner.LearningDeliveries
                .ForAny(
                    x => IsNotValid(x, GetEmploymentStatusOn(x.LearnStartDate, employments)),
                    x => RaiseValidationMessage(thisLearner.LearnRefNumber, x));
        }

        /// <summary>
        /// Determines whether [is not valid] [the specified thisdelivery].
        /// </summary>
        /// <param name="thisdelivery">The thisdelivery.</param>
        /// <param name="thisEmployment">The this employment.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified thisdelivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearningDelivery thisdelivery, ILearnerEmploymentStatus thisEmployment) =>
            IsPrimaryLearningAim(thisdelivery)
            && HasQualifyingEmploymentStatus(thisEmployment)
            && HasDisqualifyingEmployerID(thisEmployment);

        /// <summary>
        /// Determines whether [is primary learning aim] [this delivery].
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <returns>
        ///   <c>true</c> if [is primary learning aim] [this delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsPrimaryLearningAim(ILearningDelivery thisDelivery) =>
            _check.InApprenticeship(thisDelivery)
            && _check.InAProgramme(thisDelivery);

        /// <summary>
        /// Determines whether [has qualifying employment status] [the specified this employment].
        /// </summary>
        /// <param name="thisEmployment">The this employment.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying employment status] [the specified this employment]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingEmploymentStatus(ILearnerEmploymentStatus thisEmployment) =>
            It.Has(thisEmployment)
                && It.IsInRange(thisEmployment.EmpStat, TypeOfEmploymentStatus.InPaidEmployment);

        /// <summary>
        /// Determines whether [has disqualifying employer identifier] [the specified this employment].
        /// </summary>
        /// <param name="thisEmployment">The this employment.</param>
        /// <returns>
        ///   <c>true</c> if [has disqualifying employer identifier] [the specified this employment]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasDisqualifyingEmployerID(ILearnerEmploymentStatus thisEmployment) =>
            It.IsEmpty(thisEmployment.EmpIdNullable);

        /// <summary>
        /// Gets the employment status on (this date) (from employments).
        /// </summary>
        /// <param name="thisDate">this date.</param>
        /// <param name="fromEmployments">from employments.</param>
        /// <returns>the closest learner employmentstatus for the learn start date</returns>
        public ILearnerEmploymentStatus GetEmploymentStatusOn(DateTime thisDate, IReadOnlyCollection<ILearnerEmploymentStatus> fromEmployments) =>
            _check.GetEmploymentStatusOn(thisDate, fromEmployments);

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="thisDelivery">this delivery.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery)
        {
            HandleValidationError(learnRefNumber, thisDelivery.AimSeqNumber, BuildMessageParametersFor(thisDelivery));
        }

        /// <summary>
        /// Builds the message parameters for.
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <returns>a collection of message parameters</returns>
        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery thisDelivery)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, thisDelivery.LearnStartDate),
                BuildErrorMessageParameter(PropertyNameConstants.EmpStat, TypeOfEmploymentStatus.InPaidEmployment),
                BuildErrorMessageParameter(PropertyNameConstants.EmpId, "(missing)")
            };
        }
    }
}
