namespace ESFA.DC.ILR.ValidationService.Rules.Constants
{
    public static class PriorAttainments
    {
        public const int Level1 = 1;

        public const int FullLevel2 = 2;

        public const int FullLevel3 = 3;

        public const int Level4Expired20130731 = 4;

        public const int Level5AndAboveExpired20130731 = 5;

        public const int OtherBelowLevel1 = 7;

        public const int EntryLevel = 9;

        public const int Level4 = 10;

        public const int Level5 = 11;

        public const int Level6 = 12;

        public const int Level7AndAbove = 13;

        public const int OtherLevelNotKnown = 97;

        public const int NotKnown = 98;

        public const int NoQualifications = 99;

        public static readonly int[] AsHigherLevelAchievements = new int[]
        {
            FullLevel2,
            FullLevel3,
            Level4Expired20130731,
            Level5AndAboveExpired20130731,
            Level4,
            Level5,
            Level6,
            Level7AndAbove
        };
    }
}
