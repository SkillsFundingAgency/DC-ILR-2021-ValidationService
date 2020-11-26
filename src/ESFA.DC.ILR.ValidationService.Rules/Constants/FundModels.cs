namespace ESFA.DC.ILR.ValidationService.Rules.Constants
{
    public static class FundModels
    {
        public const int CommunityLearning = 10;

        public const int Age16To19ExcludingApprenticeships = 25;

        public const int AdultSkills = 35;

        public const int ApprenticeshipsFrom1May2017 = 36;

        public const int EuropeanSocialFund = 70;

        public const int OtherAdult = 81;

        public const int Other16To19 = 82;

        public const int NotFundedByESFA = 99;

        public static int[] TypeOfFundingCollection => new[]
        {
            CommunityLearning,
            Age16To19ExcludingApprenticeships,
            AdultSkills,
            ApprenticeshipsFrom1May2017,
            EuropeanSocialFund,
            OtherAdult,
            Other16To19
        };
    }
}
