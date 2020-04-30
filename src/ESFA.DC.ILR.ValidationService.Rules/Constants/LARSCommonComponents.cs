namespace ESFA.DC.ILR.ValidationService.Rules.Constants
{
    public class LARSCommonComponents
    {
        public const int NotApplicable = -2;

        public const int Unknown = -1;

        public const int KeySkillsInCommunication = 1;

        public const int KeySkillsInApplicationNumber = 2;

        public const int KeySkillsInInformationAndTechnology = 3;

        public const int KeySkillsInWorkingWithOthers = 4;

        public const int KeySkillsInImprovingOwnLearning = 5;

        public const int KeySkillsInProblemSolving = 6;

        public const int FunctionalSkillsMath = 10;

        public const int FunctionalSkillsEnglish = 11;

        public const int FunctionalSkillsICT = 12;

        public const int BritishSignLanguage = 20;

        public const int Project = 21;

        public const int GCSEMath = 30;

        public const int GCSEEnglish = 31;

        public const int GCSEICT = 32;

        public const int InternationalGCSEMath = 33;

        public const int InternationalGCSEEnglish = 34;

        public const int SteppingEnglish = 35;

        public const int SteppingMaths = 36;

        public const int AddUnitsForMicroBusiness = 40;

        public static readonly int[] CommonComponents =
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

        public static class Apprenticeship
        {
            public const int CompetencyElement = 1;

            public const int KnowledgeElement = 2;

            public const int MainAimOrTechnicalCertificate = 3;

            public const int PrincipalLearning = 101;

            public const int DiplomaQualification = 102;

            public const int PersonalAndSocial = 201;

            public const int VocationalLearning = 202;

            public const int CombinationPersonalSocialAndVocational = 203;

            public const int Language = 205;

            public const int Subject = 206;
        }
    }
}
