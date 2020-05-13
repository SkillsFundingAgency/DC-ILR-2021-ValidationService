namespace ESFA.DC.ILR.ValidationService.Rules.Constants
{
    public static class Monitoring
    {
        public static class Learner
        {
            public const string NotAchievedLevel2MathsGCSEByYear11 = "EDF1";

            public const string NotAchievedLevel2EnglishGCSEByYear11 = "EDF2";

            public static readonly string[] Level1AndLowerGrades = new string[] { "D", "DD", "DE", "E", "EE", "EF", "F", "FF", "FG", "G", "GG", "N", "U", "3", "2", "1" };

            public static class Types
            {
                public const string HighNeedsStudents = "HNS";

                public const string EducationHealthCareplan = "EHC";

                public const string DisabledStudentsAllowance = "DLA";

                public const string LearnerSupportReason = "LSR";

                public const string SpecialEducationalNeeds = "SEN";

                public const string NationalLearnerMonitoring = "NLM";

                public const string EligibilityFor16To19DisadvantageFunding = "EDF";

                public const string GCSEMathsConditionOfFunding = "MCF";

                public const string GCSEEnglishConditionOfFunding = "ECF";

                public const string FreeMealsEligibility = "FME";

                public const string PupilPremiumFunding = "PPE";
            }
        }

        public static class Delivery
        {
            public const string OLASSOffendersInCustody = "LDM034";

            public const string ReleasedOnTemporaryLicence = "LDM328";

            public const string MandationToSkillsTraining = "LDM318";

            public const string SteelIndustriesRedundancyTraining = "LDM347";

            public const string AdultEducationBudgets = "LDM357";

            public const string InReceiptOfLowWages = "LDM363";

            public const string FullyFundedLearningAim = "FFI1";

            public const string CoFundedLearningAim = "FFI2";

            public const string HigherEducationFundingCouncilEngland = "SOF1";

            public const string ESFAAdultFunding = "SOF105";

            public const string ESFA16To19Funding = "SOF107";

            public const string LocalAuthorityCommunityLearningFunds = "SOF108";

            public const string GreaterManchesterCombinedAuthority = "SOF110";

            public const string LiverpoolCityRegionCombinedAuthority = "SOF111";

            public const string WestMidlandsCombinedAuthority = "SOF112";

            public const string WestOfEnglandCombinedAuthority = "SOF113";

            public const string TeesValleyCombinedAuthority = "SOF114";

            public const string CambridgeshireAndPeterboroughCombinedAuthority = "SOF115";

            public const string GreaterLondonAuthority = "SOF116";

            public const string FinancedByAdvancedLearnerLoans = "ADL1";

            public const string ApprenticeshipFundedThroughAContractForServicesWithEmployer = "ACT1";

            public const string ApprenticeshipFundedThroughAContractForServicesWithESFA = "ACT2";

            public const string PostcodeValidationExclusion = "DAM001";

            public const string DevolvedLevelTwoOrThree = "DAM023";

            public static class Types
            {
                public const string SourceOfFunding = "SOF";

                public const string FullOrCoFunding = "FFI";

                public const string EligibilityForEnhancedApprenticeshipFunding = "EEF";

                public const string Restart = "RES";

                public const string LearningSupportFunding = "LSF";

                public const string AdvancedLearnerLoan = "ADL";

                public const string AdvancedLearnerLoansBursaryFunding = "ALB";

                public const string CommunityLearningProvision = "ASL";

                public const string FamilyEnglishMathsAndLanguage = "FLN";

                public const string Learning = "LDM";

                public const string NationalSkillsAcademy = "NSA";

                public const string WorkProgrammeParticipation = "WPP";

                public const string PercentageOfOnlineDelivery = "POD";

                public const string HEMonitoring = "HEM";

                public const string HouseholdSituation = "HHS";

                public const string ApprenticeshipContract = "ACT";

                public const string DevolvedAreaMonitoring = "DAM";
            }
        }

        public static class EmploymentStatus
        {
            public const string SelfEmployed = "SEI1";

            public const string EmployedFor16HoursOrMorePW = "EII1";

            public const string EmployedForLessThan16HoursPW = "EII2";

            public const string EmployedFor16To19HoursPW = "EII3";

            public const string EmployedFor20HoursOrMorePW = "EII4";

            public const string EmployedFor0To10HourPW = "EII5";

            public const string EmployedFor11To20HoursPW = "EII6";

            public const string EmployedFor21To30HoursPW = "EII7";

            public const string EmployedFor31PlusHoursPW = "EII8";

            public const string UnemployedForLessThan6M = "LOU1";

            public const string UnemployedFor6To11M = "LOU2";

            public const string UnemployedFor12To23M = "LOU3";

            public const string UnemployedFor24To35M = "LOU4";

            public const string UnemployedFor36MPlus = "LOU5";

            public const string EmployedForUpTo3M = "LOE1";

            public const string EmployedFor4To6M = "LOE2";

            public const string EmployedFor7To12M = "LOE3";

            public const string EmployedForMoreThan12M = "LOE4";

            public const string InReceiptOfJobSeekersAllowance = "BSI1";

            public const string InReceiptOfEmploymentAndSupportAllowance = "BSI2";

            public const string InReceiptOfAnotherStateBenefit = "BSI3";

            public const string InReceiptOfUniversalCredit = "BSI4";

            public const string InFulltimeEducationOrTrainingPriorToEnrolment = "PEI1";

            public const string SmallEmployer = "SEM1";

            public static string[] StatusesCollection => new string[]
            {
                SelfEmployed,
                EmployedFor16HoursOrMorePW,
                EmployedForLessThan16HoursPW,
                EmployedFor16To19HoursPW,
                EmployedFor20HoursOrMorePW,
                EmployedFor0To10HourPW,
                EmployedFor11To20HoursPW,
                EmployedFor21To30HoursPW,
                EmployedFor31PlusHoursPW,
                UnemployedForLessThan6M,
                UnemployedFor6To11M,
                UnemployedFor12To23M,
                UnemployedFor24To35M,
                UnemployedFor36MPlus,
                EmployedForUpTo3M,
                EmployedFor4To6M,
                EmployedFor7To12M,
                EmployedForMoreThan12M,
                InReceiptOfJobSeekersAllowance,
                InReceiptOfEmploymentAndSupportAllowance,
                InReceiptOfAnotherStateBenefit,
                InReceiptOfUniversalCredit,
                InFulltimeEducationOrTrainingPriorToEnrolment,
                SmallEmployer
            };

            public static class Types
            {
                public const string SelfEmploymentIndicator = "SEI";

                public const string EmploymentIntensityIndicator = "EII";

                public const string LengthOfUnemployment = "LOU";

                public const string LengthOfEmployment = "LOE";

                public const string BenefitStatusIndicator = "BSI";

                public const string PreviousEducationIndicator = "PEI";

                public const string SmallEmployer = "SEM";
            }
        }
    }
}
