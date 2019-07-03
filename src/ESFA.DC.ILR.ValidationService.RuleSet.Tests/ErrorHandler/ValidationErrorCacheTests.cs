using System.Threading.Tasks;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.RuleSet.ErrorHandler;
using ESFA.DC.ILR.ValidationService.RuleSet.ErrorHandler.Model;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.RuleSet.Tests.ErrorHandler
{
    public class ValidationErrorCacheTests
    {
        [Fact]
        public void AddParallel()
        {
            var validationErrorCache = NewCache();

            Parallel.For(0, 1000, (i) =>
            {
                validationErrorCache.Add(new ValidationError()
                {
                    RuleName = i.ToString(),
                    LearnerReferenceNumber = i.ToString()
                });
            });

            for (int i = 0; i < 1000; i++)
            {
                validationErrorCache.ValidationErrors.Should().ContainSingle(eb => eb.RuleName == i.ToString());
            }
        }

        private ValidationErrorCache NewCache()
        {
            return new ValidationErrorCache();
        }
    }
}
