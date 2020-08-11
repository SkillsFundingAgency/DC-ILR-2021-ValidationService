using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    public class DerivedData_28Rule : IDerivedData_28Rule
    {
        private readonly HashSet<int> _employmentStatusesTypes = new HashSet<int>
        {
            EmploymentStatusEmpStats.InPaidEmployment,
            EmploymentStatusEmpStats.NotEmployedSeekingAndAvailable,
            EmploymentStatusEmpStats.NotEmployedNotSeekingOrNotAvailable,
            EmploymentStatusEmpStats.NotKnownProvided
        };

        private readonly HashSet<string> _employmentStatuses = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            Monitoring.EmploymentStatus.EmployedForLessThan16HoursPW,
            Monitoring.EmploymentStatus.EmployedFor0To10HourPW,
            Monitoring.EmploymentStatus.EmployedFor11To20HoursPW
        };

        private readonly ILearnerEmploymentStatusQueryService _learnerEmploymentStatusQueryService;

        public DerivedData_28Rule(ILearnerEmploymentStatusQueryService learnerEmploymentStatusQueryService)
        {
            _learnerEmploymentStatusQueryService = learnerEmploymentStatusQueryService;
        }

        public bool InReceiptOfEmploymentSupport(IEmploymentStatusMonitoring employmentMonitoring)
        {
            var code = $"{employmentMonitoring.ESMType}{employmentMonitoring.ESMCode}";

            return code.CaseInsensitiveEquals(Monitoring.EmploymentStatus.InReceiptOfJobSeekersAllowance)
                || code.CaseInsensitiveEquals(Monitoring.EmploymentStatus.InReceiptOfEmploymentAndSupportAllowance);
        }

        public bool InReceiptOfEmploymentSupport(IReadOnlyCollection<IEmploymentStatusMonitoring> employmentMonitorings) =>
            employmentMonitorings.NullSafeAny(InReceiptOfEmploymentSupport);

        public bool HasValidEmploymentStatus(ILearnerEmploymentStatus candidate) =>
            _employmentStatusesTypes.Contains(candidate.EmpStat);

        public bool IsValidWithEmploymentSupport(ILearnerEmploymentStatus candidate)
        {
            return HasValidEmploymentStatus(candidate)
                && InReceiptOfEmploymentSupport(candidate.EmploymentStatusMonitorings);
        }

        public bool InReceiptOfCredits(IEmploymentStatusMonitoring employmentMonitoring)
        {
            var code = $"{employmentMonitoring.ESMType}{employmentMonitoring.ESMCode}";

            return code.CaseInsensitiveEquals(Monitoring.EmploymentStatus.InReceiptOfAnotherStateBenefit)
                || code.CaseInsensitiveEquals(Monitoring.EmploymentStatus.InReceiptOfUniversalCredit);
        }

        public bool InReceiptOfCredits(IReadOnlyCollection<IEmploymentStatusMonitoring> employmentMonitorings) =>
            employmentMonitorings.NullSafeAny(InReceiptOfCredits);

        public bool IsNotEmployed(ILearnerEmploymentStatus candidate) =>
            candidate.EmpStat == EmploymentStatusEmpStats.NotEmployedNotSeekingOrNotAvailable
                || candidate.EmpStat == EmploymentStatusEmpStats.NotEmployedSeekingAndAvailable;

        public bool IsNotEmployedWithBenefits(ILearnerEmploymentStatus candidate)
        {
            return IsNotEmployed(candidate)
                && InReceiptOfCredits(candidate.EmploymentStatusMonitorings);
        }

        public bool IsWorkingShortHours(IEmploymentStatusMonitoring monitor) =>
            _employmentStatuses.Contains($"{monitor.ESMType}{monitor.ESMCode}");

        public bool IsWorkingShortHours(IReadOnlyCollection<IEmploymentStatusMonitoring> employmentMonitorings) =>
            employmentMonitorings.NullSafeAny(IsWorkingShortHours);

        public bool IsEmployed(ILearnerEmploymentStatus candidate) =>
            candidate.EmpStat == EmploymentStatusEmpStats.InPaidEmployment;

        public bool IsEmployedWithSupport(ILearnerEmploymentStatus candidate)
        {
            return IsEmployed(candidate)
                && IsWorkingShortHours(candidate.EmploymentStatusMonitorings)
                && InReceiptOfCredits(candidate.EmploymentStatusMonitorings);
        }

        public bool IsAdultFundedUnemployedWithBenefits(ILearningDelivery thisDelivery, ILearner forThisCandidate)
        {
            /*
                if
                    // is adult skills
                    LearningDelivery.FundModel = 35
                    // and has valid employment status
                    and LearnerEmploymentStatus.EmpStat = 10, 11, 12 or 98
                    // and in receipt of support at the time of starting the learning aim
                    and (Monitoring.EmploymentStatus.ESMType = BSI and Monitoring.EmploymentStatus.ESMCode = 1 or 2)
                        (for the learner's Employment status on the LearningDelivery.LearnStartDate of the learning aim)
                or
                    // or is not employed, and in receipt of benefits
                    LearnerEmploymentStatus.EmpStat = 11 or 12
                    and (Monitoring.EmploymentStatus.ESMType = BSI and Monitoring.EmploymentStatus.ESMCode = 3 or 4)
                or
                    // or is employed with workng short hours and in receipt of support
                    LearnerEmploymentStatus.EmpStat = 10
                    and (Monitoring.EmploymentStatus.ESMType = EII and Monitoring.EmploymentStatus.ESMCode = 2, 5 or 6)
                    and (Monitoring.EmploymentStatus.ESMType = BSI and Monitoring.EmploymentStatus.ESMCode = 3 or 4)
                        set to Y,
                        otherwise set to N
             */

            var employment = _learnerEmploymentStatusQueryService.LearnerEmploymentStatusForDate(forThisCandidate.LearnerEmploymentStatuses, thisDelivery.LearnStartDate);

            return thisDelivery.FundModel == FundModels.AdultSkills
                && employment != null
                && (IsValidWithEmploymentSupport(employment)
                || IsNotEmployedWithBenefits(employment)
                || IsEmployedWithSupport(employment));
        }
    }
}
