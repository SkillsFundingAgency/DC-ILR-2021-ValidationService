using System;
using ESFA.DC.ILR.ValidationService.Interface;

namespace ESFA.DC.ILR.ValidationService.Data.Learner.Model
{
    public class LearnerReferenceData : ILearnerReferenceData
    {
        public long UKPRN { get; set; }
        public long? PrevUKPRN { get; set; }
        public long? PMUKPRN { get; set; }
        public long ULN { get; set; }
        public string LearnRefNumber { get; set; }
        public string PrevLearnRefNumber { get; set; }
        public string LearnAimRef { get; set; }
        public int? ProgTypeNullable { get; set; }
        public int? StdCodeNullable { get; set; }
        public int? FworkCodeNullable { get; set; }
        public int? PwayCodeNullable { get; set; }
        public DateTime LearnStartDate { get; set; }
        public DateTime? LearnActEndDate { get; set; }
        public int FundModel { get; set; }
    }
}
