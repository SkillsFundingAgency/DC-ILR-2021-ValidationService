using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.RuleSet.ErrorHandler;
using ESFA.DC.ILR.ValidationService.RuleSet.Tests.Rules;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.RuleSet.Tests
{
    public class RuleSetOrchestrationServiceTests
    {
        [Fact]
        public async Task Execute_NoValidationItems()
        {
            var output = new List<string> { "1", "2", "3" };

            IValidationErrorCache validationErrorCache = new ValidationErrorCache();

            var ruleSetResolutionServiceMock = new Mock<IRuleSetResolutionService<string>>();
            ruleSetResolutionServiceMock.Setup(rs => rs.Resolve()).Returns(new List<IRule<string>>() { new RuleOne(validationErrorCache), new RuleTwo(validationErrorCache) });

            var cancellationToken = CancellationToken.None;

            var ruleSetExecutionService = new RuleSetExecutionService<string>();

            var service = NewService(ruleSetResolutionServiceMock.Object, validationErrorCache: validationErrorCache, ruleSetExecutionService: ruleSetExecutionService);

            (await service.ExecuteAsync(new List<string>(),  cancellationToken)).Should().BeEmpty();
        }

        [Fact]
        public async Task Execute()
        {
            IValidationErrorCache validationErrorCache = new ValidationErrorCache();

            var ruleSet = new List<IRule<string>> { new RuleOne(validationErrorCache), new RuleTwo(validationErrorCache) };

            var ruleSetResolutionServiceMock = new Mock<IRuleSetResolutionService<string>>();
            ruleSetResolutionServiceMock.Setup(rs => rs.Resolve()).Returns(ruleSet);

            const string one = "one";
            const string two = "two";
            var validationItems = new List<string> { one, two };

            var cancellationToken = CancellationToken.None;

            var ruleSetExecutionService = new RuleSetExecutionService<string>();

            var service = NewService(ruleSetResolutionServiceMock.Object, ruleSetExecutionService, validationErrorCache);

            (await service.ExecuteAsync(validationItems, cancellationToken)).Should().HaveCount(6);
        }

        private RuleSetOrchestrationService<T> NewService<T>(
            IRuleSetResolutionService<T> ruleSetResolutionService = null,
            IRuleSetExecutionService<T> ruleSetExecutionService = null,
            IValidationErrorCache validationErrorCache = null)
            where T : class
        {
            return new RuleSetOrchestrationService<T>(
                ruleSetResolutionService,
                ruleSetExecutionService,
                validationErrorCache);
        }
    }
}
