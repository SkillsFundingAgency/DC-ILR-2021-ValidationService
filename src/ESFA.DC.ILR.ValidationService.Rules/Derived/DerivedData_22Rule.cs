using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Rules.Constants;
using ESFA.DC.ILR.ValidationService.Rules.Derived.Interface;

namespace ESFA.DC.ILR.ValidationService.Rules.Derived
{
    public class DerivedData_22Rule :
        IDerivedData_22Rule
    {
        public DateTime? GetLatestLearningStartForESFContract(
            ILearningDelivery candidate,
            IReadOnlyCollection<ILearningDelivery> usingSources)
        {
            var latest = usingSources
                .Where(x => IsCompletedQualifyingAim(x) && HasMatchingContractReference(x, candidate))
                .OrderByDescending(x => x.LearnStartDate)
                .FirstOrDefault();

            return latest?.LearnStartDate;
        }

        public bool IsCompletedQualifyingAim(ILearningDelivery delivery) =>
            delivery.LearnAimRef == AimTypes.References.ESFLearnerStartandAssessment
            && delivery.CompStatus == CompletionState.HasCompleted;

        public bool HasMatchingContractReference(ILearningDelivery source, ILearningDelivery candidate) =>
            !string.IsNullOrWhiteSpace(source.ConRefNumber) && source.ConRefNumber.CaseInsensitiveEquals(candidate.ConRefNumber);
    }
}
