namespace ESFA.DC.ILR.ValidationService.Rules.Constants
{
    /// <summary>
    /// type(s) of LARS common component
    /// </summary>
    public class TypeOfLARSCommonComponent
    {
        /// <summary>
        /// not applicable
        /// </summary>
        public const int NotApplicable = -2;

        /// <summary>
        /// unknown
        /// </summary>
        public const int Unknown = -1;

        /// <summary>
        /// Key Skills in Communication
        /// </summary>
        public const int KeySkillsInCommunication = 1;

        /// <summary>
        /// Key Skills in Application of Number
        /// </summary>
        public const int KeySkillsInApplicationNumber = 2;

        /// <summary>
        /// Key Skills in Information and Communication Technology
        /// </summary>
        public const int KeySkillsInInformationAndTechnology = 3;

        /// <summary>
        /// Key Skills in Working with Others
        /// </summary>
        public const int KeySkillsInWorkingWithOthers = 4;

        /// <summary>
        /// Key Skills in Improving Own Learning and Performance
        /// </summary>
        public const int KeySkillsInImprovingOwnLearning = 5;

        /// <summary>
        /// Key Skills in Problem Solving
        /// </summary>
        public const int KeySkillsInProblemSolving = 6;

        /// <summary>
        /// Functional Skills Mathematics
        /// </summary>
        public const int FunctionalSkillsMath = 10;

        /// <summary>
        /// Functional Skills English
        /// </summary>
        public const int FunctionalSkillsEnglish = 11;

        /// <summary>
        /// Functional Skills ICT
        /// </summary>
        public const int FunctionalSkillsICT = 12;

        /// <summary>
        /// British Sign Language
        /// </summary>
        public const int BritishSignLanguage = 20;

        /// <summary>
        /// Project/Extended Project
        /// </summary>
        public const int Project = 21;

        /// <summary>
        /// GCSE Mathematics
        /// </summary>
        public const int GCSEMath = 30;

        /// <summary>
        /// GCSE English
        /// </summary>
        public const int GCSEEnglish = 31;

        /// <summary>
        /// GCSE ICT
        /// </summary>
        public const int GCSEICT = 32;

        /// <summary>
        /// International GCSE Mathematics
        /// </summary>
        public const int InternationalGCSEMath = 33;

        /// <summary>
        /// International GCSE English
        /// </summary>
        public const int InternationalGCSEEnglish = 34;

        /// <summary>
        /// Stepping-stone English
        /// </summary>
        public const int SteppingEnglish = 35;

        /// <summary>
        /// Stepping-stone Maths
        /// </summary>
        public const int SteppingMaths = 36;

        /// <summary>
        /// Additional Units for Micro Business
        /// </summary>
        public const int AddUnitsForMicroBusiness = 40;

        public static readonly int?[] CommonComponents =
        {
            KeySkillsInCommunication,
            KeySkillsInApplicationNumber,
            KeySkillsInInformationAndTechnology,
            KeySkillsInWorkingWithOthers,
            KeySkillsInImprovingOwnLearning,
            KeySkillsInProblemSolving,
            FunctionalSkillsMath,
            FunctionalSkillsEnglish,
            FunctionalSkillsICT,
            BritishSignLanguage,
            Project,
            GCSEMath,
            GCSEEnglish,
            GCSEICT,
            InternationalGCSEMath,
            InternationalGCSEEnglish,
            SteppingEnglish,
            SteppingMaths,
            AddUnitsForMicroBusiness
        };
    }
}
