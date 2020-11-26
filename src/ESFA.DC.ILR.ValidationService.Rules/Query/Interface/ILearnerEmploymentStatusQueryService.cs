using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query.Interface
{
    public interface ILearnerEmploymentStatusQueryService : IQueryService
    {
        ILearnerEmploymentStatus LearnerEmploymentStatusForDate(IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses, DateTime date);

        IEnumerable<ILearnerEmploymentStatus> LearnerEmploymentStatusesForDate(IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses, DateTime date);

        bool EmpStatsNotExistBeforeDate(IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses, DateTime date);

        bool EmpStatsNotExistOnOrBeforeDate(IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses, DateTime date);
    }
}
