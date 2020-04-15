using System;

namespace ESFA.DC.ILR.ValidationService.Utility
{
    public static class It
    {
        public static bool IsNull<T>(T value)
            where T : class
        {
            return value == null;
        }
    }
}
