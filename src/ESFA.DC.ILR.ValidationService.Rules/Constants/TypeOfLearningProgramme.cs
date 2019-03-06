using System;

namespace ESFA.DC.ILR.ValidationService.Rules.Constants
{
    /// <summary>
    /// types of learning programme
    /// </summary>
    public static class TypeOfLearningProgramme
    {
        /// <summary>
        /// The advanced level apprenticeship
        /// </summary>
        public const int AdvancedLevelApprenticeship = 2;

        /// <summary>
        /// The intermediate level apprenticeship
        /// </summary>
        public const int IntermediateLevelApprenticeship = 3;

        /// <summary>
        /// The higher apprenticeship level 4
        /// </summary>
        public const int HigherApprenticeshipLevel4 = 20;

        /// <summary>
        /// The higher apprenticeship level 5
        /// </summary>
        public const int HigherApprenticeshipLevel5 = 21;

        /// <summary>
        /// The higher apprenticeship level 6
        /// </summary>
        public const int HigherApprenticeshipLevel6 = 22;

        /// <summary>
        /// The higher apprenticeship level 7 plus
        /// </summary>
        public const int HigherApprenticeshipLevel7Plus = 23;

        /// <summary>
        /// The traineeship
        /// </summary>
        public const int Traineeship = 24;

        /// <summary>
        /// The apprenticeship standard
        /// </summary>
        public const int ApprenticeshipStandard = 25;

        /// <summary>
        /// Gets the maximum traning duration.
        /// </summary>
        public static int MaximumTrainingDuration => -6; // 6 months

        /// <summary>
        /// Gets the maximum open duration for training.
        /// </summary>
        public static int MaximumOpenTrainingDuration => -8; // 8 months

        /// <summary>
        /// Gets the mininum viable training start date.
        /// </summary>
        public static DateTime MininumViableTrainingStartDate => new DateTime(2015, 08, 01);

        /// <summary>
        /// Gets as a set.
        /// </summary>
        public static int[] AsASet => new[]
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

        /// <summary>
        /// Determines whether [is viable apprenticeship] [for the specified start (date)].
        /// </summary>
        /// <param name="forThisStart">For this start.</param>
        /// <returns>
        ///   <c>true</c> if [is viable apprenticeship] [for the specified start (date)]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsViableApprenticeship(DateTime forThisStart) => forThisStart >= MininumViableTrainingStartDate;

        /// <summary>
        /// Within maxmimum training duration (6 months, 182 days(ish)).
        /// </summary>
        /// <param name="fromStart">From start.</param>
        /// <param name="toFinish">To finish.</param>
        /// <returns>
        ///   <c>true</c> if [within maxmimum training duration] [from start to finish]; otherwise, <c>false</c>.
        /// </returns>
        public static bool WithinMaxmimumTrainingDuration(DateTime fromStart, DateTime toFinish) =>
            (toFinish - fromStart) <= MonthsDiffernential(toFinish, MaximumTrainingDuration);

        /// <summary>
        /// Within (the) maximum open training duration (8 months, 243 days(ish)).
        /// </summary>
        /// <param name="fromStart">From start (the file preparation date).</param>
        /// <param name="toFinish">To finish (the start of the learning).</param>
        /// <returns>
        ///   <c>true</c> if [within maxmimum training duration] [from start to finish]; otherwise, <c>false</c>.
        /// </returns>
        public static bool WithinMaxmimumOpenTrainingDuration(DateTime fromStart, DateTime toFinish) =>
            (toFinish - fromStart) <= MonthsDiffernential(toFinish, MaximumOpenTrainingDuration);

        /// <summary>
        /// Months differnential.
        /// expect hte offsete to be a negative number...
        /// </summary>
        /// <param name="usingDate">The using date.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>the calculated timespan</returns>
        public static TimeSpan MonthsDiffernential(DateTime usingDate, int offset) =>
            usingDate - usingDate.AddMonths(offset);
    }
}
