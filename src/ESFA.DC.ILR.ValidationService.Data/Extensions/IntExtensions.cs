using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ValidationService.Data.Extensions
{
    public static class IntExtensions
    {
        public static IReadOnlyCollection<int> SplitIntDigitsToList(this int number)
        {
            return Math.Abs(number)
                .ToString()
                .Select(x => Convert.ToInt32(x.ToString()))
                .ToList();
        }
    }
}