﻿using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived.Interface
{
    public interface IDerivedData_12Rule : IDerivedDataRule
    {
        bool IsAdultSkillsFundedOnBenefits(IReadOnlyCollection<ILearnerEmploymentStatus> employmentStatusMonitorings, ILearningDelivery learningDelivery);
    }
}