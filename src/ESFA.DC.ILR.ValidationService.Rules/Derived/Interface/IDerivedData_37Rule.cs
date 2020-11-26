using System;
using System.Collections.Generic;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived.Interface
{
    public interface IDerivedData_37Rule : IDerivedDataRule
    {
        bool Derive(int fundModel, DateTime learnStartDate, IEnumerable<ILearnerEmploymentStatus> learnerEmploymentStatuses, IEnumerable<ILearningDeliveryFAM> learningDeliveryFAMs);
    }
}
