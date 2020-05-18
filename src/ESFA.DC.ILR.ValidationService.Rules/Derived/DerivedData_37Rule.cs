using System;
using System.Collections.Generic;
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
            return FundModelCondition(fundModel) ? EmpStatusCondition(learnStartDate, learnerEmploymentStatuses, learningDeliveryFAMs) : false;
        }

        private bool EmpStatusCondition(DateTime learnStartDate, IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            var employmentStatus = _learnerEmploymentStatusQueryService.LearnerEmploymentStatusForDate(learnerEmploymentStatuses, learnStartDate);

            if (employmentStatus != null)
            {
                return _empStats.Contains(employmentStatus.EmpStat)
                    && EmpStatMonitoringCondition(employmentStatus, learningDeliveryFAMs);
            }

            return false;
        }

        private bool EmpStatMonitoringCondition(ILearnerEmploymentStatus employmentStatus, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs)
        {
            return _learnerEmploymentStatusMonitoringQueryService
                .HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus(employmentStatus, LearnerEmploymentStatusConstants.ESMTypes.BSI_BenefitStatusIndicator, LearnerEmploymentStatusConstants.ESMCodes.BSI_ReceiptOfEmploymentAndSupport)
                || (_learnerEmploymentStatusMonitoringQueryService
                .HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus(employmentStatus, LearnerEmploymentStatusConstants.ESMTypes.BSI_BenefitStatusIndicator, LearnerEmploymentStatusConstants.ESMCodes.BSI_ReceiptOfUniversalCredit)
                && !_learningDeliveryFAMQueryService
                .HasLearningDeliveryFAMCodeForType(learningDeliveryFAMs, LearningDeliveryFAMTypeConstants.LDM, LearningDeliveryFAMCodeConstants.LDM_MandationtoSkillsTraining));
        }

        private bool FundModelCondition(int fundModel) => fundModel == FundModels.AdultSkills;
    }
}
