using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.LearningDelivery.LearnStartDate
{
    public class LearnStartDate_15Rule :
        AbstractRule,
        IRule<ILearner>
    {
        /// <summary>
        /// The derived date (rule) 22
        /// </summary>
        private readonly IDerivedData_22Rule _derivedData22;

        /// <summary>
        /// The check (rule common operations provider)
        /// </summary>
        private readonly IProvideRuleCommonOperations _check;

        /// <summary>
        /// Initializes a new instance of the <see cref="LearnStartDate_15Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="derivedData22">The derived date18.</param>
        /// <param name="commonOperations">The common operations.</param>
        public LearnStartDate_15Rule(
            IValidationErrorHandler validationErrorHandler,
            IDerivedData_22Rule derivedData22,
            IProvideRuleCommonOperations commonOperations)
            : base(validationErrorHandler, RuleNameConstants.LearnStartDate_15)
        {
            // this check should be in the base class
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(derivedData22)
                .AsGuard<ArgumentNullException>(nameof(derivedData22));
            It.IsNull(commonOperations)
                .AsGuard<ArgumentNullException>(nameof(commonOperations));

            _derivedData22 = derivedData22;
            _check = commonOperations;
        }

        /// <summary>
        /// Gets the start for.
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <param name="usingSources">using sources.</param>
        /// <returns>a start date for a standard apprecticehsip aim (or null for others)</returns>
        public DateTime? GetStartFor(ILearningDelivery thisDelivery, IReadOnlyCollection<ILearningDelivery> usingSources) =>
            _derivedData22.GetLatestLearningStartForESFContract(thisDelivery, usingSources);

        /// <summary>
        /// Determines whether [has qualifying start] [the specified this delivery].
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <param name="requiredStart">required start.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying start] [the specified this delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingStart(ILearningDelivery thisDelivery, DateTime? requiredStart) =>
            It.IsEmpty(requiredStart)
            || _check.HasQualifyingStart(thisDelivery, requiredStart.Value);

        /// <summary>
        /// Determines whether [is not valid] [the specified delivery].
        /// </summary>
        /// <param name="thisDelivery">this delivery.</param>
        /// <param name="usingSources">using sources.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified delivery]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearningDelivery thisDelivery, IReadOnlyCollection<ILearningDelivery> usingSources) =>
            !HasQualifyingStart(thisDelivery, GetStartFor(thisDelivery, usingSources));

        /// <summary>
        /// Validates this learner.
        /// </summary>
        /// <param name="thisLearner">this learner.</param>
        public void Validate(ILearner thisLearner)
        {
            It.IsNull(thisLearner)
                .AsGuard<ArgumentNullException>(nameof(thisLearner));

            var learnRefNumber = thisLearner.LearnRefNumber;
            var deliveries = thisLearner.LearningDeliveries.ToReadOnlyCollection();

            deliveries
                .ForAny(x => IsNotValid(x, deliveries), x => RaiseValidationMessage(learnRefNumber, x));
        }

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
        /// Builds the error message parameters.
        /// </summary>
        /// <param name="thisDelivery">The this delivery.</param>
        /// <returns>
        /// returns a list of message parameters
        /// </returns>
        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(ILearningDelivery thisDelivery)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.LearnStartDate, thisDelivery.LearnStartDate)
            };
        }
    }
}
