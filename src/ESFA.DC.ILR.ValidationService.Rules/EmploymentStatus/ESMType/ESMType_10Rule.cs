using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Utility;
using System;
using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.ESMType
{
    public class ESMType_10Rule :
        IRule<ILearner>
    {
        public const string MessagePropertyName = "ESMType";

        public const string Name = RuleNameConstants.ESMType_10;

        private readonly IValidationErrorHandler _messageHandler;

        public ESMType_10Rule(
            IValidationErrorHandler validationErrorHandler)
        {
            It.IsNull(validationErrorHandler)
                .AsGuard<ArgumentNullException>(nameof(validationErrorHandler));

            _messageHandler = validationErrorHandler;
        }

        public string RuleName => Name;

        public bool IsQualifyingEmployment(ILearnerEmploymentStatus employmentStatus) =>
            employmentStatus.EmpStat == TypeOfEmploymentStatus.NotEmployedNotSeekingOrNotAvailable
            || employmentStatus.EmpStat == TypeOfEmploymentStatus.NotEmployedSeekingAndAvailable;

        public bool HasDisqualifyingIndicator(IEmploymentStatusMonitoring monitor) =>
            monitor.ESMType.CaseInsensitiveEquals(Monitoring.EmploymentStatus.Types.LengthOfEmployment);

        public bool HasDisqualifyingIndicator(ILearnerEmploymentStatus employmentStatus) =>
            employmentStatus.EmploymentStatusMonitorings.NullSafeAny(HasDisqualifyingIndicator);

        public bool IsNotValid(ILearnerEmploymentStatus employmentStatus) =>
            IsQualifyingEmployment(employmentStatus) && HasDisqualifyingIndicator(employmentStatus);

        public void Validate(ILearner objectToValidate)
        {
            It.IsNull(objectToValidate)
                .AsGuard<ArgumentNullException>(nameof(objectToValidate));

            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearnerEmploymentStatuses
                .NullSafeWhere(IsNotValid)
                .ForEach(x => RaiseValidationMessage(learnRefNumber, x));
        }

        public void RaiseValidationMessage(string learnRefNumber, ILearnerEmploymentStatus thisEmployment)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                _messageHandler.BuildErrorMessageParameter(MessagePropertyName, Monitoring.EmploymentStatus.Types.LengthOfEmployment),
                _messageHandler.BuildErrorMessageParameter(nameof(thisEmployment.DateEmpStatApp), thisEmployment.DateEmpStatApp),
                _messageHandler.BuildErrorMessageParameter(nameof(thisEmployment.EmpStat), thisEmployment.EmpStat)
            };

            _messageHandler.Handle(RuleName, learnRefNumber, null, parameters);
        }
    }
}