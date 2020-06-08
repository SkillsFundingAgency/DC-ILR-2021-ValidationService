namespace ESFA.DC.ILR.ValidationService.Rules.Constants
{
    public static class LearnerEmploymentStatusConstants
    {
        public static class EmpStats
        {
            public const int InPaidEmployment = 10;
            public const int NotEmployedSeekingAndAvailable = 11;
            public const int NotEmployedNotSeekingOrNotAvailable = 12;
            public const int NotKnownProvided = 98;
        }

        public static class ESMTypes
        {
            public const string SEI_SelfEmploymentIndicator = "SEI";
            public const string EII_EmploymentIntensityIndicator = "EII";
            public const string LOU_LengthOfUnemployment = "LOU";
            public const string LOE_LengthOfEmployment = "LOE";
            public const string BSI_BenefitStatusIndicator = "BSI";
            public const string PEI_PreviousEducationIndicator = "PEI";
            public const string SEM_SmallEmployer = "SEM";
        }

        public static class ESMCodes
        {
            // Retired
            public const int Retired_EEI_EmployedMoreThan16Hours = 1;
            public const int Retired_EEI_EmployedLessThan16Hours = 2;
            public const int Retired_EEI_Employed16T019Hours = 3;
            public const int Retired_EEI_EmployedMoreThan20Hours = 4;

            public const int Retired_BSI_ReceiptOfESAWRAG = 2;
            public const int Retired_BSI_ReceiptOfOther = 3;

            //Active
            public const int SEI_SelfEmployed = 1;
            public const int EEI_Employed0To10Hours = 5;
            public const int EEI_Employed11To20Hours = 6;
            public const int EEI_Employed21To31Hours = 7;
            public const int EEI_EmployedMoreThan31Hours = 8;

            public const int LOU_UnemployedLessThan6Months = 1;
            public const int LOU_Unemployed6To11Months = 2;
            public const int LOU_Unemployed12To12Months = 3;
            public const int LOU_Unemployed24To35Months = 4;
            public const int LOU_UnemployedMoreThan36Months = 5;

            public const int LOE_EmployedUpTo3Months = 1;
            public const int LOE_Employed4To6Months = 2;
            public const int LOE_Employed7To12Months = 3;
            public const int LOE_EmployedMoreThan12Months = 4;

            public const int BSI_ReceiptOfJSA = 1;
            public const int BSI_ReceiptOfUniversalCredit = 4;
            public const int BSI_ReceiptOfEmploymentAndSupport = 5;
            public const int BSI_ReceiptOfOtherStateBenefits = 6;

            public const int PEI_InEducatonOrTrainingBeforeEnrollment = 1;

            public const int SEM_SmallEmployer = 1;
        }
    }
}
