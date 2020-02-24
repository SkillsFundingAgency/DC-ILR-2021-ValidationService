using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.ESMType
{
    public class ESMType_01Rule :
        IRule<ILearner>
    {
        /// <summary>
        /// Gets the name of the rule.
        /// </summary>
        public const string Name = RuleNameConstants.ESMType_01;

        /// <summary>
        /// The message handler
        /// </summary>
        private readonly IValidationErrorHandler _messageHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="ESMType_01Rule"/> class.
        /// </summary>
        /// <param name="validationErrorHandler">The validation error handler.</param>
        public ESMType_01Rule(
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
        /// Determines whether [is invalid domain item] [the specified monitor].
        /// </summary>
        /// <param name="monitor">The monitor.</param>
        /// <returns>
        ///   <c>true</c> if [is invalid domain item] [the specified monitor]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsInvalidDomainItem(IEmploymentStatusMonitoring monitor) =>
            It.IsOutOfRange($"{monitor.ESMType}{monitor.ESMCode}", Monitoring.EmploymentStatus.AsASet);

        /// <summary>
        /// Validates the specified object.
        /// </summary>
        /// <param name="objectToValidate">The object to validate.</param>
        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearnerEmploymentStatuses?
                .SelectMany(x => x.EmploymentStatusMonitorings.AsSafeReadOnlyList())
                .SafeWhere(IsInvalidDomainItem)
                .ForEach(x => RaiseValidationMessage(learnRefNumber, x));
        }

        /// <summary>
        /// Raises the validation message.
        /// </summary>
        /// <param name="learnRefNumber">The learn reference number.</param>
        /// <param name="thisMonitor">this monitor.</param>
        public void RaiseValidationMessage(string learnRefNumber, IEmploymentStatusMonitoring thisMonitor)
        {
            var parameters = Collection.Empty<IErrorMessageParameter>();
            parameters.Add(_messageHandler.BuildErrorMessageParameter(nameof(thisMonitor.ESMType), thisMonitor.ESMType));
            parameters.Add(_messageHandler.BuildErrorMessageParameter(nameof(thisMonitor.ESMCode), thisMonitor.ESMCode));

            _messageHandler.Handle(RuleName, learnRefNumber, null, parameters);
        }
    }
}