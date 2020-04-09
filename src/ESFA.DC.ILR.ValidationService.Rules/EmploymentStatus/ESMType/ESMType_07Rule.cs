using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.ESMType
{
    public class ESMType_07Rule :
        IRule<ILearner>
    {
        public const string MessagePropertyName = "ESMType";

        public const string Name = RuleNameConstants.ESMType_07;

        private readonly IValidationErrorHandler _messageHandler;

        public ESMType_07Rule(
            IValidationErrorHandler validationErrorHandler)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));

            _messageHandler = validationErrorHandler;
        }

        public string RuleName => Name;

        public bool IsQualifyingEmployment(ILearnerEmploymentStatus employmentStatus) =>
            It.IsInRange(employmentStatus.EmpStat, TypeOfEmploymentStatus.InPaidEmployment);

        public bool HasQualifyingIndicator(IEmploymentStatusMonitoring monitor) =>
            $"{monitor.ESMType}{monitor.ESMCode}".CaseInsensitiveEquals(Monitoring.EmploymentStatus.SelfEmployed);

        public bool HasQualifyingIndicator(ILearnerEmploymentStatus employmentStatus) =>
            employmentStatus.EmploymentStatusMonitorings.SafeAny(HasQualifyingIndicator);

        public bool IsNotValid(ILearnerEmploymentStatus employmentStatus) =>
            HasQualifyingIndicator(employmentStatus) && !IsQualifyingEmployment(employmentStatus);

        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearnerEmploymentStatuses
                .SafeWhere(IsNotValid)
                .ForEach(x => RaiseValidationMessage(learnRefNumber, x));
        }

        public void RaiseValidationMessage(string learnRefNumber, ILearnerEmploymentStatus thisEmployment)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                _messageHandler.BuildErrorMessageParameter(MessagePropertyName, Monitoring.EmploymentStatus.Types.SelfEmploymentIndicator),
                _messageHandler.BuildErrorMessageParameter(nameof(thisEmployment.EmpStat), thisEmployment.EmpStat)
            };

            _messageHandler.Handle(RuleName, learnRefNumber, null, parameters);
        }
    }
}