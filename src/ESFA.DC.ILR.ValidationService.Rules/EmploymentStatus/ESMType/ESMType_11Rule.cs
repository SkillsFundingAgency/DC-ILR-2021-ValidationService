using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.ESMType
{
    public class ESMType_11Rule :
        IRule<ILearner>
    {
        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = RuleNameConstants.ESMType_11;

        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// The lookup (details provider)
        /// </summary>
        private readonly IProvideLookupDetails _lookups;

        /// <summary>
        /// Initializes a new instance of the <see cref="ESMType_11Rule" /> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        /// <param name="lookups">The lookups.</param>
        public ESMType_11Rule(
            IValidationErrorHandler validationErrorHandler,
            IProvideLookupDetails lookups)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));
            It.IsNull(lookups)
                .AsGuard<ArgumentNullException>(nameof(lookups));

            _messageHandler = validationErrorHandler;
            _lookups = lookups;
        }

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public string RuleName => Name;

        /// <summary>
        /// Determines whether [in qualifying period] [the specified monitor].
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <param name="candidate">The candidate.</param>
        /// <returns>
        ///   <c>true</c> if [is qualifying period] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool InQualifyingPeriod(IEmploymentStatusMonitoring monitor, DateTime candidate) =>
            _lookups.IsCurrent(TypeOfLimitedLifeLookup.ESMType, $"{monitor.ESMType}{monitor.ESMCode}", candidate);

        /// <summary>
        /// Determines whether [is not valid] [the specified employment status].
        /// </summary>
        /// <param name="employmentStatus">The employment status.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified employment status]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearnerEmploymentStatus employmentStatus) =>
            employmentStatus.EmploymentStatusMonitorings.SafeAny(x => !InQualifyingPeriod(x, employmentStatus.DateEmpStatApp));

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearnerEmploymentStatuses
                .SafeWhere(IsNotValid)
                .ForEach(x => RaiseValidationMessage(learnRefNumber, x));
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="thisEmployment">The this employment.</param>
        public void RaiseValidationMessage(string learnRefNumber, ILearnerEmploymentStatus thisEmployment)
        {
            var parameters = Collection.Empty<IErrorMessageParameter>();
            parameters.Add(_messageHandler.BuildErrorMessageParameter(nameof(thisEmployment.DateEmpStatApp), thisEmployment.DateEmpStatApp));

            _messageHandler.Handle(RuleName, learnRefNumber, null, parameters);
        }
    }
}