using System;
using System.Runtime.CompilerServices;

namespace ESFA.DC.ILR.ValidationService.Utility
{
    /// <summary>
    /// Contains methods to guard against invalid input
    /// </summary>
    public static class Guard
    {
        /// <summary>
        /// As guard.
        /// </summary>
        /// <typeparam name="TExceptionType">The type of the exception type.</typeparam>
        /// <param name="failedEvaluation">if set to <c>true</c> [failed evaluation].</param>
        /// <param name="callerName">Name of the caller.</param>
        /// <param name="source">The source.</param>
        public static void AsGuard<TExceptionType>(this bool failedEvaluation, [CallerMemberName] string callerName = null, string source = null)
            where TExceptionType : Exception
        {
            if (failedEvaluation)
            {
                throw GetException<TExceptionType>(source ?? $"an item in this routine ({callerName}) was invalid");
            }
        }

        /// <summary>
        /// Gets the exception.
        /// </summary>
        /// <typeparam name="TExceptionType">The type of the exception type.</typeparam>
        /// <param name="args">The arguments.</param>
        /// <returns><see cref="Exception"/></returns>
        private static Exception GetException<TExceptionType>(params string[] args)
            where TExceptionType : Exception
        {
            return (Exception)Activator.CreateInstance(typeof(TExceptionType), args);
        }
    }
}
