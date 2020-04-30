using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpStat
{
    public class EmpStat_18Rule : AbstractRule, IRule<ILearner>
    {
        private readonly IDateTimeQueryService _dateTimeQueryService;
        private readonly ILearnerEmploymentStatusQueryService _learnerEmploymentStatusQueryService;

        public EmpStat_18Rule(
            IValidationErrorHandler validationErrorHandler,
            IDateTimeQueryService dateTimeQueryService,
            ILearnerEmploymentStatusQueryService learnerEmploymentStatusQueryService)
            : base(validationErrorHandler, RuleNameConstants.EmpStat_18)
        {
            _dateTimeQueryService = dateTimeQueryService;
            _learnerEmploymentStatusQueryService = learnerEmploymentStatusQueryService;
        }

        public static DateTime OldCodeMonitoringThresholdDate => new DateTime(2018, 07, 31);

        public void Validate(ILearner thisLearner)
        {
            var learnRefNumber = thisLearner.LearnRefNumber;
            var employments = thisLearner.LearnerEmploymentStatuses.ToReadOnlyCollection();

            thisLearner.LearningDeliveries
                .ForEach(x => RunChecks(x, GetEmploymentStatusOn(x.LearnStartDate, employments), y => RaiseValidationMessage(learnRefNumber, x, y)));
        }

        public void RunChecks(ILearningDelivery thisDelivery, ILearnerEmploymentStatus thisEmployment, Action<IEmploymentStatusMonitoring> raiseMessage)
        {
            if (IsQualifyingPrimaryLearningAim(thisDelivery)
                && HasQualifyingEmploymentStatus(thisEmployment))
            {
                CheckEmploymentMonitors(thisEmployment, raiseMessage);
            }
        }

        public ILearnerEmploymentStatus GetEmploymentStatusOn(DateTime thisDate, IReadOnlyCollection<ILearnerEmploymentStatus> fromEmployments) =>
            _learnerEmploymentStatusQueryService.LearnerEmploymentStatusForDate(fromEmployments, thisDate);

        public bool IsQualifyingPrimaryLearningAim(ILearningDelivery thisDelivery) =>
            thisDelivery != null
            && _dateTimeQueryService.IsDateBetween(thisDelivery.LearnStartDate, DateTime.MinValue, OldCodeMonitoringThresholdDate)
            && thisDelivery.ProgTypeNullable == TypeOfLearningProgramme.Traineeship
            && thisDelivery.AimType == AimTypes.ProgrammeAim;

        public bool HasQualifyingEmploymentStatus(ILearnerEmploymentStatus thisEmployment) =>
            thisEmployment != null
            && thisEmployment.EmpStat == TypeOfEmploymentStatus.InPaidEmployment;

        public void CheckEmploymentMonitors(ILearnerEmploymentStatus employment, Action<IEmploymentStatusMonitoring> raiseMessage) =>
            employment.EmploymentStatusMonitorings.ForAny(HasDisqualifyingMonitor, raiseMessage);

        public bool HasDisqualifyingMonitor(IEmploymentStatusMonitoring thisMonitor) =>
            thisMonitor.ESMType.CaseInsensitiveEquals(Monitoring.EmploymentStatus.Types.EmploymentIntensityIndicator)
            && !Monitoring.EmploymentStatus.EmployedForLessThan16HoursPW.CaseInsensitiveEquals($"{thisMonitor.ESMType}{thisMonitor.ESMCode}");

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
