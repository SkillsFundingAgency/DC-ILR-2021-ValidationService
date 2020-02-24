using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.ESMType
{
    public class ESMType_07Rule :
        IRule<ILearner>
    {
        /// <summary>
        /// Gets the name of the message property.
        /// </summary>
        public const string MessagePropertyName = "ESMType";

        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = RuleNameConstants.ESMType_07;

        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="ESMType_07Rule"/> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        public ESMType_07Rule(
            IValidationErrorHandler validationErrorHandler)
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
        /// Determines whether [is qualifying employment] [the specified employment status].
        /// </summary>
        /// <param name="employmentStatus">The employment status.</param>
        /// <returns>
        ///   <c>true</c> if [is qualifying employment] [the specified employment status]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsQualifyingEmployment(ILearnerEmploymentStatus employmentStatus) =>
            It.IsInRange(employmentStatus.EmpStat, TypeOfEmploymentStatus.InPaidEmployment);

        /// <summary>
        /// Determines whether [has qualifying indicator] [the specified monitor].
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying indicator] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingIndicator(IEmploymentStatusMonitoring monitor) =>
            $"{monitor.ESMType}{monitor.ESMCode}".CaseInsensitiveEquals(Monitoring.EmploymentStatus.SelfEmployed);

        /// <summary>
        /// Determines whether [has qualifying indicator] [the specified employment status].
        /// </summary>
        /// <param name="employmentStatus">The employment status.</param>
        /// <returns>
        ///   <c>true</c> if [has qualifying indicator] [the specified employment status]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasQualifyingIndicator(ILearnerEmploymentStatus employmentStatus) =>
            employmentStatus.EmploymentStatusMonitorings.SafeAny(HasQualifyingIndicator);

        /// <summary>
        /// Determines whether [is not valid] [the specified employment status].
        /// </summary>
        /// <param name="employmentStatus">The employment status.</param>
        /// <returns>
        ///   <c>true</c> if [is not valid] [the specified employment status]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsNotValid(ILearnerEmploymentStatus employmentStatus) =>
            HasQualifyingIndicator(employmentStatus) && !IsQualifyingEmployment(employmentStatus);

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
            parameters.Add(_messageHandler.BuildErrorMessageParameter(MessagePropertyName, Monitoring.EmploymentStatus.Types.SelfEmploymentIndicator));
            parameters.Add(_messageHandler.BuildErrorMessageParameter(nameof(thisEmployment.EmpStat), thisEmployment.EmpStat));

            _messageHandler.Handle(RuleName, learnRefNumber, null, parameters);
        }
    }
}