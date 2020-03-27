namespace ESFA.DC.ILR.ValidationService.Rules.Constants
{
    /// <summary>
    /// reasons for withdrawal
    /// </summary>
    public static class ReasonForWithdrawal
    {
        /// <summary>
        /// transferred to another provider
        /// </summary>
        public const int TransferredToAnotherProvider = 2;

        /// <summary>
        /// injury or illness
        /// </summary>
        public const int InjuryOrIllness = 3;

        /// <summary>
        /// transferred through intervention or consent
        /// </summary>
        public const int TransferredThroughInterventionOrConsent = 7;

        /// <summary>
        /// olass learner outside providers control
        /// </summary>
        public const int OLASSLearnerOutsideProvidersControl = 28;

        /// <summary>
        /// made redundant
        /// </summary>
        public const int MadeRedundant = 29;

        /// <summary>
        /// new learning aim with same provider
        /// </summary>
        public const int NewLearningAimWithSameProvider = 40;

        /// <summary>
        /// transferred meeting government strategy
        /// </summary>
        public const int TransferredMeetingGovernmentStrategy = 41;

        /// <summary>
        /// not allowed to continue, he only
        /// </summary>
        public const int NotAllowedToContinueHEOnly = 42;

        /// <summary>
        /// financial
        /// </summary>
        public const int Financial = 43;

        /// <summary>
        /// other personal
        /// </summary>
        public const int OtherPersonal = 44;

        /// <summary>
        /// written off, he only
        /// </summary>
        public const int WrittenOffHEOnly = 45;

        /// <summary>
        /// exclusion
        /// </summary>
        public const int Exclusion = 46;

        /// <summary>
        /// transferred due to merger
        /// </summary>
        public const int TransferredDueToMerger = 47;

        /// <summary>
        /// industrial placement learner withdrew
        /// </summary>
        public const int IndustrialPlacementLearnerWithdrew = 48;

        /// <summary>
        /// other
        /// </summary>
        public const int Other = 97;

        /// <summary>
        /// not known
        /// </summary>
        public const int NotKnown = 98;
    }
}
