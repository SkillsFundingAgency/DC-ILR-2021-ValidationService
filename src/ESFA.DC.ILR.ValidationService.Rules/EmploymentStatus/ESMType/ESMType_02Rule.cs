using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.ESMType
{
    public class ESMType_02Rule :
        IRule<ILearner>
    {
        public const string Name = RuleNameConstants.ESMType_02;

        private readonly IValidationErrorHandler _messageHandler;

        public ESMType_02Rule(
            IValidationErrorHandler validationErrorHandler)
        {
            _messageHandler = validationErrorHandler;
        }

        public string RuleName => Name;

        public DateTime LastInviableDate => new DateTime(2012, 07, 31);

        public bool IsQualifyingPeriod(ILearnerEmploymentStatus employmentStatus) =>
            employmentStatus.DateEmpStatApp > LastInviableDate;

        public bool IsQualifyingEmployment(ILearnerEmploymentStatus employmentStatus) =>
            employmentStatus.EmpStat == EmploymentStatusEmpStats.InPaidEmployment;

        public bool HasQualifyingIndicator(IEmploymentStatusMonitoring monitor) =>
            monitor.ESMType.CaseInsensitiveEquals(Monitoring.EmploymentStatus.Types.EmploymentIntensityIndicator);

        public bool HasQualifyingIndicator(ILearnerEmploymentStatus employmentStatus) =>
            employmentStatus.EmploymentStatusMonitorings.NullSafeAny(HasQualifyingIndicator);

        public bool IsNotValid(ILearnerEmploymentStatus employmentStatus) =>
            IsQualifyingPeriod(employmentStatus) && IsQualifyingEmployment(employmentStatus) && !HasQualifyingIndicator(employmentStatus);

        public void Validate(ILearner objectToValidate)
        {
            var learnRefNumber = objectToValidate.LearnRefNumber;

            objectToValidate.LearnerEmploymentStatuses
                .NullSafeWhere(IsNotValid)
                .ForEach(x => RaiseValidationMessage(learnRefNumber, x));
        }

        public void RaiseValidationMessage(string learnRefNumber, ILearnerEmploymentStatus thisEmployment)
        {
            var parameters = new List<IErrorMessageParameter>
            {
                _messageHandler.BuildErrorMessageParameter(nameof(thisEmployment.EmpStat), thisEmployment.EmpStat),
                _messageHandler.BuildErrorMessageParameter(nameof(thisEmployment.DateEmpStatApp), thisEmployment.DateEmpStatApp)
            };

            _messageHandler.Handle(RuleName, learnRefNumber, null, parameters);
        }
    }
}