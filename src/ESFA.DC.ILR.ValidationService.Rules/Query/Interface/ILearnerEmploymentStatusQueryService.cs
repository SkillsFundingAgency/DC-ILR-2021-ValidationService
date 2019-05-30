using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query.Interface
{
    public interface ILearnerEmploymentStatusQueryService : IQueryService
    {
        /// <summary>
        /// returns the latest Learner Employment Status that applies for a given Date
        /// </summary>
        /// <param name="learnerEmploymentStatuses">Learner Employment Statuses</param>
        /// <param name="date">Date to Compare</param>
        /// <returns>An Employment Status</returns>
        ILearnerEmploymentStatus LearnerEmploymentStatusForDate(IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses, DateTime date);

        bool EmpStatsNotExistBeforeDate(IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses, DateTime date);

        bool EmpStatsNotExistOnOrBeforeDate(IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses, DateTime date);
    }
}
