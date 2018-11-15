﻿using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Query.Interface
{
    public interface ILearnerEmploymentStatusQueryService
    {
        IEnumerable<int> EmpStatsForDateEmpStatApp(IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses, DateTime dateEmpStatApp);

        bool EmpStatsNotExistBeforeLearnStartDate(IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses, DateTime dateLearnStartDate);

        bool EmpStatsNotExistOnOrBeforeLearnStartDate(IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses, DateTime dateLearnStartDate);
    }
}
