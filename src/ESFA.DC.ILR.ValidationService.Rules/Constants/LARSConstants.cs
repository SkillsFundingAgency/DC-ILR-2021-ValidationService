using System.Runtime.CompilerServices;

namespace ESFA.DC.ILR.ValidationService.Rules.Constants
{
    public static class LARSConstants
    {
        public static class BasicSkills
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
            public const int EssentialDigitalSkill = 43;

            public static readonly int[] EnglishAndMathsList = new int[]
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

            public static readonly int[] ESOLList = new int[]
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

        public static class Categories
        {
            public const int WorkPreparationSFATraineeships = 2;
            public const int WorkPlacementSFAFunded = 4;
            public const int TradeUnionAims = 19;
            public const int LegalEntitlementLevel2 = 37;
            public const int OnlyForLegalEntitlementAtLevel3 = 38;
            public const int LicenseToPractice = 20;
            public const int McaGlaAim = 41;
            public const int Covid19SkillsOffer = 43;
        }

        public static class CommonComponents
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

            public static readonly int[] ComponentList =
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

        public static class FrameworkComponentTypes
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

        public static class LearnAimRefTypes
        {
            public const string GCEASLevel = "0001";
            public const string GCEALevel = "0002";
            public const string GCSE = "0003";
            public const string GCEA2Level = "1413";
            public const string GSCEVocational = "1422";
            public const string GCEAppliedALevel = "1430";
            public const string GCEAppliedALevelDoubleAward = "1431";
            public const string GCEAppliedASLevelDoubleAward = "1433";
            public const string GCEAppliedA2 = "1434";
            public const string GCEAppliedA2DoubleAward = "1435";
            public const string GCEALevelWithGCEAdvancedSubsidiary = "1453";
            public const string ShortCourseGCSE = "2999";
            public const string TLevelTechnicalQualification = "1468";
        }

        public static class NotionalNVQLevelV2Strings
        {
            public const string OutOfScope = "999";
            public const string Level1 = "1";
            public const string Level1_2 = "1.5";
            public const string Level2 = "2";
            public const string Level3 = "3";
            public const string Level4 = "4";
            public const string Level5 = "5";
            public const string Level6 = "6";
            public const string Level7 = "7";
            public const string Level8 = "8";
            public const string EntryLevel = "E";
            public const string HigherLevel = "H";
            public const string MixedLevel = "M";
            public const string NotKnown = "X";
        }

        public static class NotionalNVQLevelV2Doubles
        {
            public const double OutOfScope = 999;
            public const double EntryLevel = 0;
            public const double Level1 = 1;
            public const double Level2 = 2;
            public const double Level3 = 3;
            public const double HigherLevel = 9;
            public const double Level1_2 = 1.5;
            public const double Level4 = 4;
            public const double Level5 = 5;
            public const double Level6 = 6;
            public const double Level7 = 7;
            public const double Level8 = 8;
            public const double MixedLevel = 9;
            public const double NotKnown = 10;
        }

        public static class Validities
        {
            public const string EFA16To19 = "1619_EFA";
            public const string AdultSkills = "ADULT_SKILLS";
            public const string AdvancedLearnerLoan = "ADV_LEARN_LOAN";
            public const string Any = "ANY";
            public const string Apprenticeships = "APPRENTICESHIPS";
            public const string CommunityLearning = "COMM_LEARN";
            public const string EFAConFundEnglish = "EFACONFUNDENGLISH";
            public const string EFAConFundMaths = "EFACONFUNDMATHS";
            public const string EuropeanSocialFund = "ESF";
            public const string OLASSAdult = "OLASS_ADULT";
            public const string Unemployed = "UNEMPLOYED";
        }
    }
}