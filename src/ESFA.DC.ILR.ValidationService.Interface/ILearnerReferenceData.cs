using System;

namespace ESFA.DC.ILR.ValidationService.Interface
{
    public interface ILearnerReferenceData
    {
        long UKPRN { get; }
        long? PrevUKPRN { get; }
        long? PMUKPRN { get; }
        long ULN { get; }
        string LearnRefNumber { get; }
        string PrevLearnRefNumber { get; }
        string LearnAimRef { get; }
        int? ProgTypeNullable { get; }
        int? StdCodeNullable { get; }
        int? FworkCodeNullable { get; }
        int? PwayCodeNullable { get; }
        DateTime LearnStartDate { get; }
        DateTime? LearnActEndDate { get; }
        int FundModel { get; }
    }
}
