using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.ESMType
{
    public class ESMType_01Rule :
        IRule<ILearner>
    {
        public const string Name = RuleNameConstants.ESMType_01;

        private readonly IValidationErrorHandler _messageHandler;

        public ESMType_01Rule(
            IValidationErrorHandler validationErrorHandler)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));

            _messageHandler = validationErrorHandler;
        }

        public string RuleName => Name;

        public bool IsInvalidDomainItem(IEmploymentStatusMonitoring monitor) =>
            !Monitoring.EmploymentStatus.AsASet.Contains($"{monitor.ESMType}{monitor.ESMCode}");

        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearnerEmploymentStatuses?
                .SelectMany(x => x.EmploymentStatusMonitorings.ToReadOnlyCollection())
                .NullSafeWhere(IsInvalidDomainItem)
                .ForEach(x => RaiseValidationMessage(learnRefNumber, x));
        }

        public void RaiseValidationMessage(string learnRefNumber, IEmploymentStatusMonitoring thisMonitor)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                _messageHandler.BuildErrorMessageParameter(nameof(thisMonitor.ESMType), thisMonitor.ESMType),
                _messageHandler.BuildErrorMessageParameter(nameof(thisMonitor.ESMCode), thisMonitor.ESMCode)
            };

            _messageHandler.Handle(RuleName, learnRefNumber, null, parameters);
        }
    }
}