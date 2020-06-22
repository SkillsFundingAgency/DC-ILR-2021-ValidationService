using System;

namespace ESFA.DC.ILR.ValidationService.Rules.Constants
{
    public static class ProgTypes
    {
        public const int AdvancedLevelApprenticeship = 2;

        public const int IntermediateLevelApprenticeship = 3;

        public const int HigherApprenticeshipLevel4 = 20;

        public const int HigherApprenticeshipLevel5 = 21;

        public const int HigherApprenticeshipLevel6 = 22;

        public const int HigherApprenticeshipLevel7Plus = 23;

        public const int Traineeship = 24;

        public const int ApprenticeshipStandard = 25;

        public const int TLevel = 31;

        public const int TLevelTransition = 30;

        public const int MaximumTrainingDurationInMonths = -12;

        public const int MaximumOpenTrainingDurationInMonths = -8;

        public static DateTime MininumViableTrainingStartDate => new DateTime(2015, 08, 01);

        public static int[] TypeOfLearningProgrammesCollection => new[]
        {
            AdvancedLevelApprenticeship,
            IntermediateLevelApprenticeship,
            HigherApprenticeshipLevel4,
            HigherApprenticeshipLevel5,
            HigherApprenticeshipLevel6,
            HigherApprenticeshipLevel7Plus,
            Traineeship,
            ApprenticeshipStandard
        };

        public static bool IsViableApprenticeship(DateTime forThisStart) => forThisStart >= MininumViableTrainingStartDate;

        public static bool WithinMaxmimumTrainingDuration(DateTime fromStart, DateTime toFinish) =>
            (toFinish - fromStart) <= MonthsDiffernential(toFinish, MaximumTrainingDurationInMonths);

        public static bool WithinMaxmimumOpenTrainingDuration(DateTime fromStart, DateTime toFinish) =>
            (toFinish - fromStart) <= MonthsDiffernential(toFinish, MaximumOpenTrainingDurationInMonths);

        public static TimeSpan MonthsDiffernential(DateTime usingDate, int offset) =>
            ((usingDate.Year - 1) * 12 + usingDate.Month) >= Math.Abs(offset) ? usingDate - usingDate.AddMonths(offset) : TimeSpan.MinValue;
    }
}
