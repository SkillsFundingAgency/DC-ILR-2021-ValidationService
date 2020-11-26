using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    public class DerivedData_38Rule : IDerivedData_38Rule
    {
        private readonly HashSet<int> _empStatsLookupRange = new HashSet<int>()
        {
            LearnerEmploymentStatusConstants.EmpStats.InPaidEmployment,
            LearnerEmploymentStatusConstants.EmpStats.NotEmployedSeekingAndAvailable,
            LearnerEmploymentStatusConstants.EmpStats.NotEmployedNotSeekingOrNotAvailable,
            LearnerEmploymentStatusConstants.EmpStats.NotKnownProvided
        };

        private readonly HashSet<int> _empStatsConditionTwo = new HashSet<int>()
        {
            LearnerEmploymentStatusConstants.EmpStats.NotEmployedSeekingAndAvailable,
            LearnerEmploymentStatusConstants.EmpStats.NotEmployedNotSeekingOrNotAvailable
        };

        private readonly HashSet<int> _esmCodesConditionTwo = new HashSet<int>()
        {
            LearnerEmploymentStatusConstants.ESMCodes.BSI_ReceiptOfUniversalCredit,
            LearnerEmploymentStatusConstants.ESMCodes.BSI_ReceiptOfEmploymentAndSupport,
            LearnerEmploymentStatusConstants.ESMCodes.BSI_ReceiptOfOtherStateBenefits
        };

        private readonly HashSet<int> _esmEIICodesConditionThree = new HashSet<int>()
        {
            LearnerEmploymentStatusConstants.ESMCodes.Retired_EEI_EmployedLessThan16Hours,
            LearnerEmploymentStatusConstants.ESMCodes.EEI_Employed0To10Hours,
            LearnerEmploymentStatusConstants.ESMCodes.EEI_Employed11To20Hours
        };

        private readonly HashSet<int> _esmBSICodesConditionThree = new HashSet<int>()
        {
            LearnerEmploymentStatusConstants.ESMCodes.BSI_ReceiptOfUniversalCredit,
            LearnerEmploymentStatusConstants.ESMCodes.BSI_ReceiptOfEmploymentAndSupport,
            LearnerEmploymentStatusConstants.ESMCodes.BSI_ReceiptOfOtherStateBenefits
        };

        private readonly ILearnerEmploymentStatusQueryService _learnerEmploymentStatusQueryService;
        private readonly ILearnerEmploymentStatusMonitoringQueryService _learnerEmploymentStatusMonitoringQueryService;

        public DerivedData_38Rule(
            ILearnerEmploymentStatusQueryService learnerEmploymentStatusQueryService,
            ILearnerEmploymentStatusMonitoringQueryService learnerEmploymentStatusMonitoringQueryService)
        {
            _learnerEmploymentStatusQueryService = learnerEmploymentStatusQueryService;
            _learnerEmploymentStatusMonitoringQueryService = learnerEmploymentStatusMonitoringQueryService;
        }

        public bool Derive(int fundModel, DateTime learnStartDate, IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses)
        {
            if (FundModelCondition(fundModel) && learnerEmploymentStatuses != null)
            {
                var employmentStatuses = _learnerEmploymentStatusQueryService.LearnerEmploymentStatusesForDate(learnerEmploymentStatuses, learnStartDate);

                return employmentStatuses.Any(EmpStatusCondition);
            }

            return false;
        }

        public bool EmpStatusCondition(ILearnerEmploymentStatus employmentStatus)
        {
            return ValidEmpstat(employmentStatus.EmpStat)
                && (EmpStatMonitoringConditionOne(employmentStatus)
                || EmpStatMonitoringConditionTwo(employmentStatus)
                || EmpStatMonitoringConditionThree(employmentStatus));
        }

        public bool ValidEmpstat(int empStat) => _empStatsLookupRange.Contains(empStat);

        public bool EmpStatMonitoringConditionOne(ILearnerEmploymentStatus employmentStatus)
        {
             return _learnerEmploymentStatusMonitoringQueryService
                    .HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus(employmentStatus, LearnerEmploymentStatusConstants.ESMTypes.BSI_BenefitStatusIndicator, LearnerEmploymentStatusConstants.ESMCodes.BSI_ReceiptOfJSA);
        }

        public bool EmpStatMonitoringConditionTwo(ILearnerEmploymentStatus employmentStatus)
        {
            return _empStatsConditionTwo.Contains(employmentStatus.EmpStat)
                && _learnerEmploymentStatusMonitoringQueryService
                   .HasAnyEmploymentStatusMonitoringTypeAndCodesForEmploymentStatus(employmentStatus, LearnerEmploymentStatusConstants.ESMTypes.BSI_BenefitStatusIndicator, _esmCodesConditionTwo);
        }

        public bool EmpStatMonitoringConditionThree(ILearnerEmploymentStatus employmentStatus)
        {
            return employmentStatus.EmpStat == LearnerEmploymentStatusConstants.EmpStats.InPaidEmployment
                && _learnerEmploymentStatusMonitoringQueryService
                   .HasAnyEmploymentStatusMonitoringTypeAndCodesForEmploymentStatus(employmentStatus, LearnerEmploymentStatusConstants.ESMTypes.BSI_BenefitStatusIndicator, _esmBSICodesConditionThree)
                && _learnerEmploymentStatusMonitoringQueryService
                   .HasAnyEmploymentStatusMonitoringTypeAndCodesForEmploymentStatus(employmentStatus, LearnerEmploymentStatusConstants.ESMTypes.EII_EmploymentIntensityIndicator, _esmEIICodesConditionThree);
        }

        public bool FundModelCondition(int fundModel) => fundModel == FundModels.AdultSkills;
    }
}
