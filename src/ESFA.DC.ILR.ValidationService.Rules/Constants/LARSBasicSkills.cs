namespace ESFA.DC.ILR.ValidationService.Rules.Constants
{
    public class LARSBasicSkills
    {
        public const int NotApplicable = -2;

        public const int Unknown = -1;

        public const int Certificate_AdultLiteracy = 1;

        public const int Certificate_AdultNumeracy = 2;

        public const int GCSE_EnglishLanguage = 11;

        public const int GCSE_Mathematics = 12;

        public const int KeySkill_Communication = 13;

        public const int KeySkill_ApplicationOfNumbers = 14;

        public const int OtherS4LNotLiteracyNumeracyOrESOL = 18;

        public const int FunctionalSkillsMathematics = 19;

        public const int FunctionalSkillsEnglish = 20;

        public const int UnitsOfTheCertificate_AdultNumeracy = 21;

        public const int UnitsOfTheCertificate_ESOLS4L = 22;

        public const int UnitsOfTheCertificate_AdultLiteracy = 23;

        public const int NonNQF_QCFS4LLiteracy = 24;

        public const int NonNQF_QCFS4LNumeracy = 25;

        public const int NonNQF_QCFS4LESOL = 26;

        public const int CertificateESOLS4L = 27;

        public const int CertificateESOLS4LSpeakListen = 28;

        public const int QCFBasicSkillsEnglishLanguage = 29;

        public const int QCFBasicSkillsMathematics = 30;

        public const int UnitQCFBasicSkillsEnglishLanguage = 31;

        public const int UnitQCFBasicSkillsMathematics = 32;

        public const int InternationalGCSEEnglishLanguage = 33;

        public const int InternationalGCSEMathematics = 34;

        public const int FreeStandingMathematicsQualification = 35;

        public const int QCFCertificateESOL = 36;

        public const int QCFESOLSpeakListen = 37;

        public const int QCFESOLReading = 38;

        public const int QCFESOLWriting = 39;

        public const int UnitESOLSpeakListen = 40;

        public const int UnitESOLReading = 41;

        public const int UnitESOLWriting = 42;

        public static readonly int[] AsEnglishAndMathsBasicSkills = new int[]
        {
            Certificate_AdultLiteracy,
            Certificate_AdultNumeracy,
            GCSE_EnglishLanguage,
            GCSE_Mathematics,
            KeySkill_Communication,
            KeySkill_ApplicationOfNumbers,
            FunctionalSkillsMathematics,
            FunctionalSkillsEnglish,
            UnitsOfTheCertificate_AdultNumeracy,
            UnitsOfTheCertificate_AdultLiteracy,
            NonNQF_QCFS4LLiteracy,
            NonNQF_QCFS4LNumeracy,
            QCFBasicSkillsEnglishLanguage,
            QCFBasicSkillsMathematics,
            UnitQCFBasicSkillsEnglishLanguage,
            UnitQCFBasicSkillsMathematics,
            InternationalGCSEEnglishLanguage,
            InternationalGCSEMathematics,
            FreeStandingMathematicsQualification
        };

        public static readonly int[] AsESOLBasicSkills = new int[]
        {
            QCFCertificateESOL,
            QCFESOLSpeakListen,
            QCFESOLReading,
            QCFESOLWriting,
            UnitESOLSpeakListen,
            UnitESOLReading,
            UnitESOLWriting
        };
    }
}
