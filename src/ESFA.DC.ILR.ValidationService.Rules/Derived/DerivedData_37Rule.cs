using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    public class DerivedData_37Rule : IDerivedData_37Rule
    {
        private readonly HashSet<int> _empStats = new HashSet<int>()
        {
            LearnerEmploymentStatusConstants.EmpStats.NotEmployedSeekingAndAvailable,
            LearnerEmploymentStatusConstants.EmpStats.NotEmployedNotSeekingOrNotAvailable
        };

        private readonly ILearnerEmploymentStatusQueryService _learnerEmploymentStatusQueryService;
        private readonly ILearnerEmploymentStatusMonitoringQueryService _learnerEmploymentStatusMonitoringQueryService;
        private readonly ILearningDeliveryFAMQueryService _learningDeliveryFAMQueryService;

        public DerivedData_37Rule(
            ILearnerEmploymentStatusQueryService learnerEmploymentStatusQueryService,
            ILearnerEmploymentStatusMonitoringQueryService learnerEmploymentStatusMonitoringQueryService,
            ILearningDeliveryFAMQueryService learningDeliveryFAMQueryService)
        {
            _learnerEmploymentStatusQueryService = learnerEmploymentStatusQueryService;
            _learnerEmploymentStatusMonitoringQueryService = learnerEmploymentStatusMonitoringQueryService;
            _learningDeliveryFAMQueryService = learningDeliveryFAMQueryService;
        }

        public bool Derive(int fundModel, DateTime learnStartDate, IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            if (FundModelCondition(fundModel) && learnerEmploymentStatuses != null)
            {
                var employmentStatuses = _learnerEmploymentStatusQueryService.LearnerEmploymentStatusesForDate(learnerEmploymentStatuses, learnStartDate);

                return employmentStatuses.Any(x => EmpStatusCondition(x, learningDeliveryFAMs));
            }

            return false;
        }

        public bool EmpStatusCondition(ILearnerEmploymentStatus employmentStatus, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return ValidEmpstat(employmentStatus.EmpStat)
                && EmpStatMonitoringCondition(employmentStatus, learningDeliveryFAMs);
        }

        public bool ValidEmpstat(int empStat) => _empStats.Contains(empStat);

        public bool EmpStatMonitoringCondition(ILearnerEmploymentStatus employmentStatus, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return _learnerEmploymentStatusMonitoringQueryService
                .HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus(employmentStatus, LearnerEmploymentStatusConstants.ESMTypes.BSI_BenefitStatusIndicator, LearnerEmploymentStatusConstants.ESMCodes.BSI_ReceiptOfOtherStateBenefits)
                || (_learnerEmploymentStatusMonitoringQueryService
                .HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus(employmentStatus, LearnerEmploymentStatusConstants.ESMTypes.BSI_BenefitStatusIndicator, LearnerEmploymentStatusConstants.ESMCodes.BSI_ReceiptOfUniversalCredit)
                && !_learningDeliveryFAMQueryService
                .HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, LearningDeliveryFAMCodeConstants.LDM_MandationtoSkillsTraining));
        }

        public bool FundModelCondition(int fundModel) => fundModel == FundModels.AdultSkills;
    }
}
