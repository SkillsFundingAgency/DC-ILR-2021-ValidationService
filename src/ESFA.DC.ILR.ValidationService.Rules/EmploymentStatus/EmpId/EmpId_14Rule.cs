using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.EmploymentStatus.EmpId
{
    public class EmpId_14Rule : AbstractRule, IRule<ILearner>
    {
        private readonly HashSet<int> _progTypes = new HashSet<int>
        {
            ProgTypes.AdvancedLevelApprenticeship,
            ProgTypes.IntermediateLevelApprenticeship,
            ProgTypes.HigherApprenticeshipLevel4,
            ProgTypes.HigherApprenticeshipLevel5,
            ProgTypes.HigherApprenticeshipLevel6,
            ProgTypes.HigherApprenticeshipLevel7Plus,
            ProgTypes.ApprenticeshipStandard
        };

        private readonly HashSet<int> _empStats = new HashSet<int>
        {
            EmploymentStatusEmpStats.NotEmployedSeekingAndAvailable,
            EmploymentStatusEmpStats.NotEmployedNotSeekingOrNotAvailable
        };

        public EmpId_14Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.EmpId_14)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate.LearnerEmploymentStatuses == null)
            {
                return;
            }

            foreach (var learningDelivery in objectToValidate.LearningDeliveries)
            {
                if (ConditionMet(learningDelivery.ProgTypeNullable))
                {
                    var employmentStatus = GetLearnerEmploymentStatus(objectToValidate.LearnerEmploymentStatuses);
                    if (employmentStatus != null)
                    {
                        HandleValidationError(objectToValidate.LearnRefNumber, learningDelivery.AimSeqNumber, BuildErrorMessageParameters(learningDelivery.ProgTypeNullable, employmentStatus.EmpStat, employmentStatus.EmpIdNullable));
                    }
                }
            }
        }

        public bool ConditionMet(int? progType)
        {
            return progType.HasValue && _progTypes.Contains(progType.Value);
        }

        public ILearnerEmploymentStatus GetLearnerEmploymentStatus(IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses)
        {
            return learnerEmploymentStatuses.FirstOrDefault(x => x.EmpIdNullable.HasValue && _empStats.Contains(x.EmpStat));
        }

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(int? progType, int empStat, int? empId)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.ProgType, progType),
                BuildErrorMessageParameter(PropertyNameConstants.EmpStat, empStat),
                BuildErrorMessageParameter(PropertyNameConstants.EmpId, empId)
            };
        }
    }
}
