using System.Collections.Generic;

namespace ESFA.DC.ILR.ValidationService.Rules.Constants
{
    public static class LARSNotionalNVQLevelV2Helper
    {
        private static readonly Dictionary<string, double> _notionalLevelV2Map = new Dictionary<string, double>
        {
            [LARSNotionalNVQLevelV2.EntryLevel] = TypeOfNotionalNVQLevelV2.EntryLevel,
            [LARSNotionalNVQLevelV2.Level1] = TypeOfNotionalNVQLevelV2.Level1,
            [LARSNotionalNVQLevelV2.Level2] = TypeOfNotionalNVQLevelV2.Level2,
            [LARSNotionalNVQLevelV2.Level3] = TypeOfNotionalNVQLevelV2.Level3,
            [LARSNotionalNVQLevelV2.HigherLevel] = TypeOfNotionalNVQLevelV2.HigherLevel,
            [LARSNotionalNVQLevelV2.Level1_2] = TypeOfNotionalNVQLevelV2.Level1_2,
            [LARSNotionalNVQLevelV2.Level4] = TypeOfNotionalNVQLevelV2.Level4,
            [LARSNotionalNVQLevelV2.Level5] = TypeOfNotionalNVQLevelV2.Level5,
            [LARSNotionalNVQLevelV2.Level6] = TypeOfNotionalNVQLevelV2.Level6,
            [LARSNotionalNVQLevelV2.Level7] = TypeOfNotionalNVQLevelV2.Level7,
            [LARSNotionalNVQLevelV2.Level8] = TypeOfNotionalNVQLevelV2.Level8,
            [LARSNotionalNVQLevelV2.MixedLevel] = TypeOfNotionalNVQLevelV2.MixedLevel,
            [LARSNotionalNVQLevelV2.NotKnown] = TypeOfNotionalNVQLevelV2.NotKnown
        };

        public static double AsNotionalNVQLevelV2(this string source)
        {
            return string.IsNullOrWhiteSpace(source) || !_notionalLevelV2Map.ContainsKey(source)
                ? TypeOfNotionalNVQLevelV2.OutOfScope
                : _notionalLevelV2Map[source];
        }
    }
}
