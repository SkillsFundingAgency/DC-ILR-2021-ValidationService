using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Query.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.ESMType
{
    public class ESMType_16Rule : AbstractRule, IRule<ILearner>
    {
        private readonly ILearnerEmploymentStatusMonitoringQueryService _learnerEmploymentStatusMonitoringQueryService;

        private readonly HashSet<int> _empStats = new HashSet<int>
        {
            EmploymentStatusEmpStats.NotEmployedSeekingAndAvailable,
            EmploymentStatusEmpStats.NotEmployedNotSeekingOrNotAvailable
        };

        public ESMType_16Rule(ILearnerEmploymentStatusMonitoringQueryService learnerEmploymentStatusMonitoringQueryService, IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.ESMType_16)
        {
            _learnerEmploymentStatusMonitoringQueryService = learnerEmploymentStatusMonitoringQueryService;
        }

        public void Validate(ILearner objectToValidate)
        {
            if (!LearningDeliveryConditionMet(objectToValidate.LearningDeliveries))
            {
                return;
            }

            var learnerEmploymentStatuses = GetLearnerEmploymentStatuses(objectToValidate.LearnerEmploymentStatuses);

            foreach (var learnerEmploymentStatus in learnerEmploymentStatuses)
            {
                if (ConditionMet(learnerEmploymentStatus))
                {
                    HandleValidationError(objectToValidate.LearnRefNumber, errorMessageParameters: BuildErrorMessageParameters(learnerEmploymentStatus.EmpStat));
                }
            }
        }

        public bool LearningDeliveryConditionMet(IEnumerable<ILearningDelivery> learningDeliveries)
        {
            return learningDeliveries?.Any(ld => ld.ProgTypeNullable != null && ld.ProgTypeNullable.Equals(ProgTypes.ApprenticeshipStandard)) ?? false;
        }

        public bool ConditionMet(ILearnerEmploymentStatus learnerEmploymentStatus)
        {
            return _learnerEmploymentStatusMonitoringQueryService
                       .HasAnyEmploymentStatusMonitoringTypeAndCodeForEmploymentStatus(learnerEmploymentStatus, Monitoring.EmploymentStatus.Types.SmallEmployer, 1);
        }

        public IEnumerable<ILearnerEmploymentStatus> GetLearnerEmploymentStatuses(IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses)
        {
            return learnerEmploymentStatuses?.Where(x => _empStats.Contains(x.EmpStat));
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int empStat)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, ProgTypes.ApprenticeshipStandard),
                BuildErrorMessageParameter(PropertyNameConstants.EmpStat, empStat),
                BuildErrorMessageParameter(PropertyNameConstants.ESMType, Monitoring.EmploymentStatus.Types.SmallEmployer),
                BuildErrorMessageParameter(PropertyNameConstants.ESMCode, 1),
            };
        }
    }
}
