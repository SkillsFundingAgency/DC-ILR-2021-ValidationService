namespace ESFA.DC.ILR.ValidationService.Rules.Constants
{
    public static class AimTypes
    {
        public const int ProgrammeAim = 1;

        public const int ComponentAimInAProgramme = 3;

        public const int AimNotPartOfAProgramme = 4;

        public const int CoreAim16To19ExcludingApprenticeships = 5;

        public static int[] InAProgramme => new[]
        {
            ProgrammeAim,
            ComponentAimInAProgramme,
        };

        public static class Branches
        {
            public const string VocationalStudiesNotLeadingToARecognisedQualification = "ZVOC";

            public const string UnitsOfApprovedNQFProvision = "ZUXA";

            public const string NonExternallyCertificatedFEOtherProvision = "Z9OPE";
        }

        public static class References
        {
            public const string WorkPlacement0To49Hours = "Z0007834";

            public const string WorkPlacement50To99Hours = "Z0007835";

            public const string WorkPlacement100To199Hours = "Z0007836";

            public const string WorkPlacement200To499Hours = "Z0007837";

            public const string WorkPlacement500PlusHours = "Z0007838";

            public const string SupportedInternship16To19 = "Z0002347";

            public const string WorkExperience = "ZWRKX001";

            public const string IndustryPlacement = "ZWRKX002";

            public const string ESFLearnerStartandAssessment = "ZESF0001";

            public static string[] AsWorkPlacementCodes => new[]
            {
                WorkPlacement0To49Hours,
                WorkPlacement50To99Hours,
                WorkPlacement100To199Hours,
                WorkPlacement200To499Hours,
                WorkPlacement500PlusHours,
                SupportedInternship16To19,
                WorkExperience,
                IndustryPlacement,
            };
        }
    }
}
