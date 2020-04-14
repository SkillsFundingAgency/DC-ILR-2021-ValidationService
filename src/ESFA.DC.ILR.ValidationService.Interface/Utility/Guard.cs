using System;
using System.Runtime.CompilerServices;

namespace ESFA.DC.ILR.ValidationService.Utility
{
    public static class Guard
    {
        public static void AsGuard<TExceptionType>(this bool failedEvaluation, [CallerMemberName] string callerName = null, string source = null)
            where TExceptionType : Exception
        {
            if (failedEvaluation)
            {
                throw GetException<TExceptionType>(source ?? $"an item in this routine ({callerName}) was invalid");
            }
        }

        private static Exception GetException<TExceptionType>(params string[] args)
            where TExceptionType : Exception
        {
            return (Exception)Activator.CreateInstance(typeof(TExceptionType), args);
        }
    }
}
