using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    public class DerivedData_25Rule :
        IDerivedData_25Rule
    {
        private readonly ILearnerEmploymentStatusQueryService _learnerEmploymentStatusQueryService;

        public DerivedData_25Rule(
            ILearnerEmploymentStatusQueryService learnerEmploymentStatusQueryService)
        {
            _learnerEmploymentStatusQueryService = learnerEmploymentStatusQueryService;
        }

        public int? GetLengthOfUnemployment(
            ILearner learner,
            string conRefNumber)
        {
            var delivery = learner?.LearningDeliveries
                    ?.OrderByDescending(x => x.LearnStartDate)
                    .FirstOrDefault(ld => ld.LearnAimRef.CaseInsensitiveEquals(AimTypes.References.ESFLearnerStartandAssessment)
                             && ld.CompStatus == CompletionState.HasCompleted
                             && ld.ConRefNumber.CaseInsensitiveEquals(conRefNumber));

            if (delivery == null)
            {
                return null;
            }

            var employmentStatusMonitoring = _learnerEmploymentStatusQueryService
                .LearnerEmploymentStatusForDate(learner.LearnerEmploymentStatuses, delivery.LearnStartDate)
                ?.EmploymentStatusMonitorings?.FirstOrDefault(esm => esm.ESMType.CaseInsensitiveEquals(Monitoring.EmploymentStatus.Types.LengthOfUnemployment));

            return employmentStatusMonitoring?.ESMCode;
        }
    }
}