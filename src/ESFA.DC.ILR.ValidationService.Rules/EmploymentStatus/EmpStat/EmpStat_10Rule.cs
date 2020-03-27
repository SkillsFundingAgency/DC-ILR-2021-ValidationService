using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpStat
{
    public class EmpStat_10Rule :
        IRule<ILearner>
    {
        /// <summary>
        /// Gets the name of the message property.
        /// </summary>
        public const string MessagePropertyName = "DateEmpStatApp";

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = RuleNameConstants.EmpStat_10;

        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// The derived data 22 (rule)
        /// </summary>
        private readonly IDerivedData_22Rule _derivedData22;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmpStat_10Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="derivedData22">The derived data 22 rule.</param>
        /// <param name="yearData">The year data.</param>
        public EmpStat_10Rule(
            IValidationErrorHandler validationErrorHandler,
            IDerivedData_22Rule derivedData22)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(derivedData22)
                .AsGuard<ArgumentNullException>(nameof(derivedData22));

            _messageHandler = validationErrorHandler;
            _derivedData22 = derivedData22;
        }

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public string RuleName => Name;

        /// <summary>
        /// Gets the contract completion date.
        /// </summary>
        /// <param name="delivery">The delivery.</param>
        /// <param name="usingSources">The using sources.</param>
        /// <returns>the latest completion date for the contract (if there is one)</returns>
        public DateTime? GetContractCompletionDate(ILearningDelivery delivery, IReadOnlyCollection<ILearningDelivery> usingSources) =>
            _derivedData22.GetLatestLearningStartForESFContract(delivery, usingSources);

        /// <summary>
        /// Gets the latest contract completion date.
        /// </summary>
        /// <param name="usingSources">The using sources.</param>
        /// <returns>the latest completion date for all the contracts (if there is one)</returns>
        public DateTime? GetLatestContractCompletionDate(IReadOnlyCollection<ILearningDelivery> usingSources)
        {
            var candidates = Collection.Empty<DateTime?>();
            usingSources.ForEach(source => candidates.Add(GetContractCompletionDate(source, usingSources)));

            return candidates.Max();
        }

        /// <summary>
        /// Determines whether [has a qualifying employment status] [the specified e status].
        /// </summary>
        /// <param name="thisEmployment">this employment.</param>
        /// <param name="thresholdDate">The learning start date.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying employment status] [the specified e status]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasAQualifyingEmploymentStatus(ILearnerEmploymentStatus thisEmployment, DateTime thresholdDate) =>
            thisEmployment.DateEmpStatApp < thresholdDate;

        /// <summary>
        /// Determines whether [is not valid] [the specified learner].
        /// </summary>
        /// <param name="learner">The learner.</param>
        /// <param name="referenceDate">The reference date.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified learner]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearner learner, DateTime referenceDate) =>
            !learner.LearnerEmploymentStatuses.SafeAny(x => HasAQualifyingEmploymentStatus(x, referenceDate));

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;
            var forDeliveries = objectToValidate.LearningDeliveries.AsSafeReadOnlyList();
            var completionDate = GetLatestContractCompletionDate(forDeliveries);

            if (It.Has(completionDate) && IsNotValid(objectToValidate, completionDate.Value))
            {
                RaiseValidationMessage(learnRefNumber, completionDate.Value);
            }
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="learnStartDate">The learn start date.</param>
        public void RaiseValidationMessage(string learnRefNumber, DateTime learnStartDate)
        {
            var parameters = Collection.Empty<IErrorMessageParameter>();
            parameters.Add(_messageHandler.BuildErrorMessageParameter(MessagePropertyName, learnStartDate));
            parameters.Add(_messageHandler.BuildErrorMessageParameter(PropertyNameConstants.LearnAimRef, TypeOfAim.References.ESFLearnerStartandAssessment));

            _messageHandler.Handle(RuleName, learnRefNumber, null, parameters);
        }
    }
}
