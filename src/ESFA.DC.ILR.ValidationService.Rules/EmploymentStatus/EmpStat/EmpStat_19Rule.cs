﻿using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpStat
{
    public class EmpStat_19Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<string> _esmTypeCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            Monitoring.EmploymentStatus.EmployedFor0To10HourPW,
            Monitoring.EmploymentStatus.EmployedFor11To20HoursPW
        };

        private readonly IDateTimeQueryService _dateTimeQueryService;
        private readonly ILearnerEmploymentStatusQueryService _learnerEmploymentStatusQueryService;

        public EmpStat_19Rule(
            IValidationErrorHandler validationErrorHandler,
            IDateTimeQueryService dateTimeQueryService,
            ILearnerEmploymentStatusQueryService learnerEmploymentStatusQueryService)
            : base(validationErrorHandler, RuleNameConstants.EmpStat_19)
        {
            _dateTimeQueryService = dateTimeQueryService;
            _learnerEmploymentStatusQueryService = learnerEmploymentStatusQueryService;
        }

        public static DateTime NewCodeMonitoringThresholdDate => new DateTime(2018, 08, 01);

        public ILearnerEmploymentStatus GetEmploymentStatusOn(DateTime thisDate, IReadOnlyCollection<ILearnerEmploymentStatus> usingEmployments) =>
            _learnerEmploymentStatusQueryService.LearnerEmploymentStatusForDate(usingEmployments, thisDate);

        public bool HasADisqualifyingMonitorStatus(IEmploymentStatusMonitoring monitor) =>
            monitor.ESMType.CaseInsensitiveEquals(Monitoring.EmploymentStatus.Types.EmploymentIntensityIndicator)
            && !_esmTypeCodes.Contains($"{monitor.ESMType}{monitor.ESMCode}");

        public void CheckEmploymentStatus(ILearnerEmploymentStatus thisEmployment, Action<IEmploymentStatusMonitoring> doThisAction)
        {
            if (thisEmployment != null && thisEmployment.EmpStat == EmploymentStatusEmpStats.InPaidEmployment)
            {
                thisEmployment.EmploymentStatusMonitorings.ForAny(HasADisqualifyingMonitorStatus, doThisAction);
            }
        }

        public bool IsRestrictionMatch(ILearningDelivery delivery) =>
            delivery.ProgTypeNullable == ProgTypes.Traineeship
            && delivery.AimType == AimTypes.ProgrammeAim
            && _dateTimeQueryService.IsDateBetween(delivery.LearnStartDate, NewCodeMonitoringThresholdDate, DateTime.MaxValue);

        public void Validate(ILearner objectToValidate)
        {
            var learnRefNumber = objectToValidate.LearnRefNumber;
            var employments = objectToValidate.LearnerEmploymentStatuses.ToReadOnlyCollection();

            objectToValidate.LearningDeliveries
                .NullSafeWhere(IsRestrictionMatch)
                .ForEach(x => CheckEmploymentStatus(GetEmploymentStatusOn(x.LearnStartDate, employments), y => RaiseValidationMessage(learnRefNumber, x, y)));
        }

        public void RaiseValidationMessage(string learnRefNumber, ILearningDelivery thisDelivery, IEmploymentStatusMonitoring thisMonitor)
        {
            HandleValidationError(learnRefNumber, thisDelivery.AimSeqNumber, BuildMessageParametersFor(thisMonitor));
        }

        public IEnumerable<IErrorMessageParameter> BuildMessageParametersFor(IEmploymentStatusMonitoring thisMonitor)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.ESMType, thisMonitor.ESMType),
                BuildErrorMessageParameter(PropertyNameConstants.ESMCode, thisMonitor.ESMCode)
            };
        }
    }
}
