using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    public class DerivedData_26Rule : IDerivedData_26Rule
    {
        private readonly ILearnerEmploymentStatusQueryService _learnerEmploymentStatusQueryService;

        public DerivedData_26Rule(ILearnerEmploymentStatusQueryService learnerEmploymentStatusQueryService)
        {
            _learnerEmploymentStatusQueryService = learnerEmploymentStatusQueryService;
        }

        public bool LearnerOnBenefitsAtStartOfCompletedZESF0001AimForContract(ILearner learner, string conRefNumber)
        {
            if (learner == null)
            {
                return false;
            }

            var completedZESF0001AimForContract = GetCompletedZESF0001AimForContract(learner.LearningDeliveries, conRefNumber);

            if (completedZESF0001AimForContract == null)
            {
                return false;
            }

            var learnerEmploymentStatusForLearnStartDate = _learnerEmploymentStatusQueryService.LearnerEmploymentStatusForDate(learner.LearnerEmploymentStatuses, completedZESF0001AimForContract.LearnStartDate);

            if (learnerEmploymentStatusForLearnStartDate == null)
            {
                return false;
            }

            return HasEmploymentStatusMonitoringForTypeBSI(learnerEmploymentStatusForLearnStartDate.EmploymentStatusMonitorings);
        }

        public ILearningDelivery GetCompletedZESF0001AimForContract(IEnumerable<ILearningDelivery> learningDeliveries, string conRefNumber)
        {
            return learningDeliveries?.FirstOrDefault(
                ld =>
                    ld.CompStatus == CompletionState.HasCompleted
                    && ld.LearnAimRef.CaseInsensitiveEquals(ValidationConstants.ZESF0001)
                    && ld.ConRefNumber.CaseInsensitiveEquals(conRefNumber));
        }

        public bool HasEmploymentStatusMonitoringForTypeBSI(IEnumerable<IEmploymentStatusMonitoring> employmentStatusMonitorings)
        {
            return employmentStatusMonitorings?.Any(esm =>
                       esm.ESMType.CaseInsensitiveEquals(Monitoring.EmploymentStatus.Types.BenefitStatusIndicator)) ??
                   false;
        }
    }
}
