using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Abstract;
using ESFA.DC.ILR.ValidationService.Rules.Constants;

namespace ESFA.DC.ILR.ValidationService.Rules.CrossEntity
{
    public class R97Rule : AbstractRule, IRule<ILearner>
    {
        public R97Rule(IValidationErrorHandler validationErrorHandler)
            : base(validationErrorHandler, RuleNameConstants.R97)
        {
        }

        public void Validate(ILearner objectToValidate)
        {
            if (objectToValidate?.LearnerEmploymentStatuses == null)
            {
                return;
            }

            int record = 1;
            ILearnerEmploymentStatus previousLearnerEmploymentStatus = null;
            foreach (var learnerEmploymentStatus in
                objectToValidate.LearnerEmploymentStatuses
                .Where(e => e != null)
                .OrderBy(e => e.DateEmpStatApp))
            {
                if (record > 1)
                {
                    if (LearnerEmploymentStatusConditionMet(previousLearnerEmploymentStatus, learnerEmploymentStatus)
                        && LearnerEmploymentStatusMonitoringConditionMet(
                            previousLearnerEmploymentStatus.EmploymentStatusMonitorings,
                            learnerEmploymentStatus.EmploymentStatusMonitorings))
                    {
                        HandleValidationError(
                            learnRefNumber: objectToValidate.LearnRefNumber,
                            errorMessageParameters: BuildErrorMessageParameters(
                                learnerEmploymentStatus.EmpStat,
                                learnerEmploymentStatus.DateEmpStatApp,
                                learnerEmploymentStatus.EmpIdNullable,
                                learnerEmploymentStatus.EmploymentStatusMonitorings?.FirstOrDefault().ESMType,
                                learnerEmploymentStatus.EmploymentStatusMonitorings?.FirstOrDefault().ESMCode));
                    }
                }

                previousLearnerEmploymentStatus = learnerEmploymentStatus;
                record++;
            }
        }

        public bool LearnerEmploymentStatusMonitoringConditionMet(
            IReadOnlyCollection<IEmploymentStatusMonitoring> previousEmploymentStatusMonitorings,
            IReadOnlyCollection<IEmploymentStatusMonitoring> employmentStatusMonitorings)
        {
            if (previousEmploymentStatusMonitorings == null
                && employmentStatusMonitorings == null)
            {
                return true;
            }

            if (previousEmploymentStatusMonitorings == null
                || employmentStatusMonitorings == null)
            {
                return false;
            }

            var currentEmploymentStatuses = employmentStatusMonitorings.Where(p => p != null).Select(p => new { type = p.ESMType.ToLowerInvariant(), code = p.ESMCode });
            var previousEmploymentStatuses = previousEmploymentStatusMonitorings.Where(p => p != null).Select(p => new { type = p.ESMType.ToLowerInvariant(), code = p.ESMCode });

            return !(currentEmploymentStatuses.Except(previousEmploymentStatuses).Any()
                || previousEmploymentStatuses.Except(currentEmploymentStatuses).Any());
        }

        public bool LearnerEmploymentStatusConditionMet(
            ILearnerEmploymentStatus previousLearnerEmploymentStatus,
            ILearnerEmploymentStatus learnerEmploymentStatus) =>
            learnerEmploymentStatus.DateEmpStatApp != previousLearnerEmploymentStatus.DateEmpStatApp
                && learnerEmploymentStatus.EmpStat == previousLearnerEmploymentStatus.EmpStat
                && learnerEmploymentStatus.EmpIdNullable == previousLearnerEmploymentStatus.EmpIdNullable;

        public IEnumerable<IErrorMessageParameter> BuildErrorMessageParameters(
            int empStat,
            DateTime dateEmpStatApp,
            int? empId,
            string eSMType,
            int? eSMCode)
        {
            return new[]
            {
                BuildErrorMessageParameter(PropertyNameConstants.EmpStat, empStat),
                BuildErrorMessageParameter(PropertyNameConstants.DateEmpStatApp, dateEmpStatApp),
                BuildErrorMessageParameter(PropertyNameConstants.EmpId, empId),
                BuildErrorMessageParameter(PropertyNameConstants.ESMType, eSMType),
                BuildErrorMessageParameter(PropertyNameConstants.ESMCode, eSMCode)
            };
        }
    }
}
