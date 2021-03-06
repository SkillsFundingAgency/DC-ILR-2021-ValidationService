﻿using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.ESMType
{
    public class ESMType_12Rule : IRule<ILearner>
    {
        public const string MessagePropertyName = "ESMType";
        public const string Name = RuleNameConstants.ESMType_12;
        private readonly IValidationErrorHandler _messageHandler;
        private readonly IDateTimeQueryService _dateTimeQueryService;

        public ESMType_12Rule(
            IValidationErrorHandler validationErrorHandler,
            IDateTimeQueryService dateTimeQueryService)
        {
            _messageHandler = validationErrorHandler;
            _dateTimeQueryService = dateTimeQueryService;
        }

        public static DateTime FirstViableDate => new DateTime(2013, 08, 01);

        public string RuleName => Name;

        public bool IsQualifyingEmployment(ILearnerEmploymentStatus employmentStatus) =>
            employmentStatus.EmpStat == EmploymentStatusEmpStats.NotEmployedNotSeekingOrNotAvailable
            || employmentStatus.EmpStat == EmploymentStatusEmpStats.NotEmployedSeekingAndAvailable;

        public bool HasDisqualifyingIndicator(IEmploymentStatusMonitoring monitor) =>
            monitor.ESMType.CaseInsensitiveEquals(Monitoring.EmploymentStatus.Types.SelfEmploymentIndicator)
            || monitor.ESMType.CaseInsensitiveEquals(Monitoring.EmploymentStatus.Types.EmploymentIntensityIndicator);

        public bool HasDisqualifyingIndicator(ILearnerEmploymentStatus employmentStatus) =>
            employmentStatus.EmploymentStatusMonitorings.NullSafeAny(HasDisqualifyingIndicator);

        public bool IsNotValid(ILearnerEmploymentStatus employmentStatus) =>
            employmentStatus != null
                && _dateTimeQueryService.IsDateBetween(employmentStatus.DateEmpStatApp, FirstViableDate, DateTime.MaxValue)
                && IsQualifyingEmployment(employmentStatus)
                && HasDisqualifyingIndicator(employmentStatus);

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
                _messageHandler.BuildErrorMessageParameter(MessagePropertyName, "Invalid"),
                _messageHandler.BuildErrorMessageParameter(nameof(thisEmployment.DateEmpStatApp), thisEmployment.DateEmpStatApp),
                _messageHandler.BuildErrorMessageParameter(nameof(thisEmployment.EmpStat), thisEmployment.EmpStat)
            };

            _messageHandler.Handle(RuleName, learnRefNumber, null, parameters);
        }
    }
}