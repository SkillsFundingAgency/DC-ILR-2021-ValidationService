using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.Constants
{
    public static class LARSNotionalNVQLevelV2Helper
    {
        private static readonly Dictionary<string, double> _notionalLevelV2Map = new Dictionary<string, double>
        {
            [LARSConstants.NotionalNVQLevelV2Strings.EntryLevel] = LARSConstants.NotionalNVQLevelV2Doubles.EntryLevel,
            [LARSConstants.NotionalNVQLevelV2Strings.Level1] = LARSConstants.NotionalNVQLevelV2Doubles.Level1,
            [LARSConstants.NotionalNVQLevelV2Strings.Level2] = LARSConstants.NotionalNVQLevelV2Doubles.Level2,
            [LARSConstants.NotionalNVQLevelV2Strings.Level3] = LARSConstants.NotionalNVQLevelV2Doubles.Level3,
            [LARSConstants.NotionalNVQLevelV2Strings.HigherLevel] = LARSConstants.NotionalNVQLevelV2Doubles.HigherLevel,
            [LARSConstants.NotionalNVQLevelV2Strings.Level1_2] = LARSConstants.NotionalNVQLevelV2Doubles.Level1_2,
            [LARSConstants.NotionalNVQLevelV2Strings.Level4] = LARSConstants.NotionalNVQLevelV2Doubles.Level4,
            [LARSConstants.NotionalNVQLevelV2Strings.Level5] = LARSConstants.NotionalNVQLevelV2Doubles.Level5,
            [LARSConstants.NotionalNVQLevelV2Strings.Level6] = LARSConstants.NotionalNVQLevelV2Doubles.Level6,
            [LARSConstants.NotionalNVQLevelV2Strings.Level7] = LARSConstants.NotionalNVQLevelV2Doubles.Level7,
            [LARSConstants.NotionalNVQLevelV2Strings.Level8] = LARSConstants.NotionalNVQLevelV2Doubles.Level8,
            [LARSConstants.NotionalNVQLevelV2Strings.MixedLevel] = LARSConstants.NotionalNVQLevelV2Doubles.MixedLevel,
            [LARSConstants.NotionalNVQLevelV2Strings.NotKnown] = LARSConstants.NotionalNVQLevelV2Doubles.NotKnown
        };

        public static double AsNotionalNVQLevelV2(this string source)
        {
            return string.IsNullOrWhiteSpace(source) || !_notionalLevelV2Map.ContainsKey(source)
                ? LARSConstants.NotionalNVQLevelV2Doubles.OutOfScope
                : _notionalLevelV2Map[source];
        }
    }
}
