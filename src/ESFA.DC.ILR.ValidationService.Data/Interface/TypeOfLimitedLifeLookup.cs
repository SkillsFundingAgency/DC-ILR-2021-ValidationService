namespace ESFA.DC.ILR.ValidationService.Data.Interface
{
    /// <summary>
    /// lookup time restricted keys
    /// these are time restricted integer based value domains
    /// </summary>
    public enum TypeOfLimitedLifeLookup
    {
        /// <summary>
        /// employment outcome
        /// </summary>
        EmpOutcome,

        /// <summary>
        /// employment status monitor (typed code)
        /// </summary>
        ESMType,

        /// <summary>
        /// funding completion (code)
        /// </summary>
        FundComp,

        /// <summary>
        /// learning disability or difficulty category
        /// </summary>
        LLDDCat,

        /// <summary>
        /// major source (of) tuition fees
        /// </summary>
        MSTuFee,

        /// <summary>
        /// Outcome Type
        /// </summary>
        OutType,

        /// <summary>
        /// highest qualification on entry
        /// </summary>
        QualEnt3,

        /// <summary>
        /// term time accomodation
        /// </summary>
        TTAccom,

        /// <summary>
        /// contact preference
        /// </summary>
        ContPrefType,

        /// <summary>
        /// The learning delivery fam
        /// </summary>
        LearningDeliveryFAM,

        /// <summary>
        /// The learner fam
        /// </summary>
        LearnerFAM
    }
}
