namespace ESFA.DC.ILR.ValidationService.Rules.Constants
{
    /// <summary>
    /// type(s) of monitoring (learner, learning delivery and employment status)
    /// </summary>
    public static class Monitoring
    {
        /// <summary>
        /// learner funding and monitoring items
        /// </summary>
        public static class Learner
        {
            /// <summary>
            /// not achieved level 2 maths gcse by year 11
            /// </summary>
            public const string NotAchievedLevel2MathsGCSEByYear11 = "EDF1";

            /// <summary>
            /// not achieved level 2 english gcse by year 11
            /// </summary>
            public const string NotAchievedLevel2EnglishGCSEByYear11 = "EDF2";

            /// <summary>
            /// (GCSE) level 1 and lower grades
            /// Spec requires these to be in upper case when providing an ILR.
            /// </summary>
            public static readonly string[] Level1AndLowerGrades = new string[] { "D", "DD", "DE", "E", "EE", "EF", "F", "FF", "FG", "G", "GG", "N", "U", "3", "2", "1" };

            /// <summary>
            /// learner monitoring types
            /// </summary>
            public static class Types
            {
                /// <summary>
                /// high needs students
                /// </summary>
                public const string HighNeedsStudents = "HNS";

                /// <summary>
                /// education health careplan
                /// </summary>
                public const string EducationHealthCareplan = "EHC";

                /// <summary>
                /// disabled students allowance
                /// </summary>
                public const string DisabledStudentsAllowance = "DLA";

                /// <summary>
                /// learner support reason
                /// </summary>
                public const string LearnerSupportReason = "LSR";

                /// <summary>
                /// special educational needs
                /// </summary>
                public const string SpecialEducationalNeeds = "SEN";

                /// <summary>
                /// national learner monitoring
                /// </summary>
                public const string NationalLearnerMonitoring = "NLM";

                /// <summary>
                /// eligibility for 16 to 19 disadvantage funding
                /// </summary>
                public const string EligibilityFor16To19DisadvantageFunding = "EDF";

                /// <summary>
                /// gcse maths condition of funding
                /// </summary>
                public const string GCSEMathsConditionOfFunding = "MCF";

                /// <summary>
                /// gcse english condition of funding
                /// </summary>
                public const string GCSEEnglishConditionOfFunding = "ECF";

                /// <summary>
                /// free meals eligibility
                /// </summary>
                public const string FreeMealsEligibility = "FME";

                /// <summary>
                /// pupil premium funding
                /// </summary>
                public const string PupilPremiumFunding = "PPE";
            }
        }

        /// <summary>
        /// delivery funding and monitoring items
        /// </summary>
        public static class Delivery
        {
            /// <summary>
            /// olass offenders in custody
            /// </summary>
            public const string OLASSOffendersInCustody = "LDM034";

            /// <summary>
            /// released (from prison) on temporary licence
            /// </summary>
            public const string ReleasedOnTemporaryLicence = "LDM328";

            /// <summary>
            /// (DWP) mandated to skills training
            /// </summary>
            public const string MandationToSkillsTraining = "LDM318";

            /// <summary>
            /// steel industries redundancy training
            /// </summary>
            public const string SteelIndustriesRedundancyTraining = "LDM347";

            /// <summary>
            /// in receipt of low wages
            /// </summary>
            public const string InReceiptOfLowWages = "LDM363";

            /// <summary>
            /// fully funded learning aim
            /// </summary>
            public const string FullyFundedLearningAim = "FFI1";

            /// <summary>
            /// co-funded learning aim
            /// </summary>
            public const string CoFundedLearningAim = "FFI2";

            /// <summary>
            /// higher education funding council england
            /// </summary>
            public const string HigherEducationFundingCouncilEngland = "SOF1"; // HEFCE

            /// <summary>
            /// esfa adult funding
            /// </summary>
            public const string ESFAAdultFunding = "SOF105";

            /// <summary>
            /// The esfa 16 to 19 funding
            /// </summary>
            public const string ESFA16To19Funding = "SOF107";

            /// <summary>
            /// local authority community learning funds
            /// </summary>
            public const string LocalAuthorityCommunityLearningFunds = "SOF108";

            /// <summary>
            /// The greater manchester combined authority
            /// </summary>
            public const string GreaterManchesterCombinedAuthority = "SOF110";

            /// <summary>
            /// The liverpool city region combined authority
            /// </summary>
            public const string LiverpoolCityRegionCombinedAuthority = "SOF111";

            /// <summary>
            /// The west midlands combined authority
            /// </summary>
            public const string WestMidlandsCombinedAuthority = "SOF112";

            /// <summary>
            /// The west of england combined authority
            /// </summary>
            public const string WestOfEnglandCombinedAuthority = "SOF113";

            /// <summary>
            /// The tees valley combined authority
            /// </summary>
            public const string TeesValleyCombinedAuthority = "SOF114";

            /// <summary>
            /// The cambridgeshire and peterborough combined authority
            /// </summary>
            public const string CambridgeshireAndPeterboroughCombinedAuthority = "SOF115";

            /// <summary>
            /// The greater london authority
            /// </summary>
            public const string GreaterLondonAuthority = "SOF116";

            /// <summary>
            /// financed by advanced learner loans
            /// </summary>
            public const string FinancedByAdvancedLearnerLoans = "ADL1";

            /// <summary>
            /// The apprenticeship funded through a contract for services with employer
            /// </summary>
            public const string ApprenticeshipFundedThroughAContractForServicesWithEmployer = "ACT1";

            /// <summary>
            /// funding and monitoring types
            /// </summary>
            public static class Types
            {
                /// <summary>
                /// source of funding
                /// </summary>
                public const string SourceOfFunding = "SOF";

                /// <summary>
                /// full or co funding
                /// </summary>
                public const string FullOrCoFunding = "FFI";

                /// <summary>
                /// eligibility for enhanced apprenticeship funding
                /// </summary>
                public const string EligibilityForEnhancedApprenticeshipFunding = "EEF";

                /// <summary>
                /// restart indicator
                /// </summary>
                public const string Restart = "RES";

                /// <summary>
                /// learning support funding
                /// </summary>
                public const string LearningSupportFunding = "LSF";

                /// <summary>
                /// advanced learner loans indicator
                /// </summary>
                public const string AdvancedLearnerLoan = "ADL";

                /// <summary>
                /// advanced learner loans bursary funding
                /// </summary>
                public const string AdvancedLearnerLoansBursaryFunding = "ALB";

                /// <summary>
                /// community learning provision type
                /// </summary>
                public const string CommunityLearningProvision = "ASL";

                /// <summary>
                /// family english, maths and language
                /// </summary>
                public const string FamilyEnglishMathsAndLanguage = "FLN";

                /// <summary>
                /// learning (delivery monitoring)
                /// </summary>
                public const string Learning = "LDM";

                /// <summary>
                /// national skills academy
                /// </summary>
                public const string NationalSkillsAcademy = "NSA";

                /// <summary>
                /// work programme participation
                /// </summary>
                public const string WorkProgrammeParticipation = "WPP";

                /// <summary>
                /// percentage of online delivery
                /// </summary>
                public const string PercentageOfOnlineDelivery = "POD";

                /// <summary>
                /// HE monitoring
                /// </summary>
                public const string HEMonitoring = "HEM";

                /// <summary>
                /// household situation
                /// </summary>
                public const string HouseholdSituation = "HHS";

                /// <summary>
                /// apprenticeship contract type
                /// </summary>
                public const string ApprenticeshipContract = "ACT";
            }
        }

        /// <summary>
        /// employment status monitoring items
        /// </summary>
        public static class EmploymentStatus
        {
            /// <summary>
            /// self employed
            /// </summary>
            public const string SelfEmployed = "SEI1";

            /// <summary>
            /// employed for 16 hours or more per week
            /// </summary>
            public const string EmployedFor16HoursOrMorePW = "EII1";

            /// <summary>
            /// employed for less than 16 hours per week
            /// </summary>
            public const string EmployedForLessThan16HoursPW = "EII2";

            /// <summary>
            /// employed for 16 to 19 hours per week
            /// </summary>
            public const string EmployedFor16To19HoursPW = "EII3";

            /// <summary>
            /// employed for 20 hours or more per week
            /// </summary>
            public const string EmployedFor20HoursOrMorePW = "EII4";

            /// <summary>
            /// employed for 0 to 10 hour per week
            /// </summary>
            public const string EmployedFor0To10HourPW = "EII5";

            /// <summary>
            /// employed for 11 to 20 hours per week
            /// </summary>
            public const string EmployedFor11To20HoursPW = "EII6";

            /// <summary>
            /// employed for 21 to 30 hours per week
            /// </summary>
            public const string EmployedFor21To30HoursPW = "EII7";

            /// <summary>
            /// employed for 31 plus hours per week
            /// </summary>
            public const string EmployedFor31PlusHoursPW = "EII8";

            /// <summary>
            /// unemployed for less than 6 months
            /// </summary>
            public const string UnemployedForLessThan6M = "LOU1";

            /// <summary>
            /// unemployed for 6 to 11 months
            /// </summary>
            public const string UnemployedFor6To11M = "LOU2";

            /// <summary>
            /// unemployed for 12 to 23 months
            /// </summary>
            public const string UnemployedFor12To23M = "LOU3";

            /// <summary>
            /// unemployed for 24 to 35 months
            /// </summary>
            public const string UnemployedFor24To35M = "LOU4";

            /// <summary>
            /// unemployed for 36 months and over
            /// </summary>
            public const string UnemployedFor36MPlus = "LOU5";

            /// <summary>
            /// employed for up to 3 months
            /// </summary>
            public const string EmployedForUpTo3M = "LOE1";

            /// <summary>
            /// employed for 4 to 6 months
            /// </summary>
            public const string EmployedFor4To6M = "LOE2";

            /// <summary>
            /// employed for 7 to 12 months
            /// </summary>
            public const string EmployedFor7To12M = "LOE3";

            /// <summary>
            /// employed for more than 12 months
            /// </summary>
            public const string EmployedForMoreThan12M = "LOE4";

            /// <summary>
            /// in receipt of job seekers allowance
            /// </summary>
            public const string InReceiptOfJobSeekersAllowance = "BSI1";

            /// <summary>
            /// in receipt of employment and support allowance
            /// </summary>
            public const string InReceiptOfEmploymentAndSupportAllowance = "BSI2";

            /// <summary>
            /// in receipt of another state benefit
            /// </summary>
            public const string InReceiptOfAnotherStateBenefit = "BSI3";

            /// <summary>
            /// in receipt of universal credit
            /// </summary>
            public const string InReceiptOfUniversalCredit = "BSI4";

            /// <summary>
            /// in fulltime education or training prior to enrolment
            /// </summary>
            public const string InFulltimeEducationOrTrainingPriorToEnrolment = "PEI1";

            /// <summary>
            /// small employer
            /// </summary>
            public const string SmallEmployer = "SEM1";

            // TODO: this should be removed now we have ESMTypedCodes in the lookup provider
            public static string[] AsASet => new string[]
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

            /// <summary>
            /// employment status monitoring types
            /// </summary>
            public static class Types
            {
                /// <summary>
                /// self employment indicator
                /// </summary>
                public const string SelfEmploymentIndicator = "SEI";

                /// <summary>
                /// employment intensity indicator
                /// </summary>
                public const string EmploymentIntensityIndicator = "EII";

                /// <summary>
                /// length of unemployment
                /// </summary>
                public const string LengthOfUnemployment = "LOU";

                /// <summary>
                /// length of employment
                /// </summary>
                public const string LengthOfEmployment = "LOE";

                /// <summary>
                /// benefit status indicator
                /// </summary>
                public const string BenefitStatusIndicator = "BSI";

                /// <summary>
                /// previous education indicator
                /// </summary>
                public const string PreviousEducationIndicator = "PEI";

                /// <summary>
                /// small employer
                /// </summary>
                public const string SmallEmployer = "SEM";
            }
        }
    }
}
