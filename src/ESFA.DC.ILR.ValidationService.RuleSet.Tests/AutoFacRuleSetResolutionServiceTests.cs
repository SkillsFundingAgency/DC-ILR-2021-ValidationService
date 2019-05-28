using System.Collections.Generic;
using System.Linq;
using Autofac;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.RuleSet.Tests.ErrorHandler;
using ESFA.DC.ILR.ValidationService.RuleSet.Tests.Rules;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.RuleSet.Tests
{
    public class AutoFacRuleSetResolutionServiceTests
    {
        [Fact]
        public void Resolve_None()
        {
            var builder = new ContainerBuilder();

            var validationContextMock = new Mock<IValidationContext>();

            var container = builder.Build();

            using (var scope = container.BeginLifetimeScope())
            {
                var service = NewService(scope);

                service.Resolve(validationContextMock.Object).Should().BeEmpty();
            }
        }

        [Fact]
        public void Resolve_Single()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<ValidationErrorCacheGenericTest<string>>().As<IValidationErrorCache<string>>();
            builder.RegisterType<RuleOne>().As<IRule<string>>();

            var validationContextMock = new Mock<IValidationContext>();

            var container = builder.Build();

            using (var scope = container.BeginLifetimeScope())
            {
                var service = NewService(scope);

                service.Resolve(validationContextMock.Object).First().Should().BeOfType<RuleOne>();
            }
        }

        [Fact]
        public void Resolve_Multiple()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<ValidationErrorCacheGenericTest<string>>().As<IValidationErrorCache<string>>();
            builder.RegisterType<RuleOne>().As<IRule<string>>();
            builder.RegisterType<RuleTwo>().As<IRule<string>>();

            var validationContextMock = new Mock<IValidationContext>();

            var container = builder.Build();

            using (var scope = container.BeginLifetimeScope())
            {
                var service = NewService(scope);

                var rules = service.Resolve(validationContextMock.Object).ToList();

                rules.Should().AllBeAssignableTo<IRule<string>>();
                rules[0].Should().BeOfType<RuleOne>();
                rules[1].Should().BeOfType<RuleTwo>();
            }
        }

        [Fact]
        public void Resolve_NewScopeNewRule()
        {
            var builder = new ContainerBuilder();

            var validationContextMock = new Mock<IValidationContext>();

            builder.RegisterType<ValidationErrorCacheGenericTest<string>>().As<IValidationErrorCache<string>>().InstancePerLifetimeScope();
            builder.RegisterType<RuleOne>().As<IRule<string>>().InstancePerLifetimeScope();

            var container = builder.Build();

            IRule<string> ruleOne;
            IRule<string> ruleTwo;

            using (var scope = container.BeginLifetimeScope())
            {
                var service = NewService(scope);

                ruleOne = service.Resolve(validationContextMock.Object).First();
            }

            using (var scope = container.BeginLifetimeScope())
            {
                var service = NewService(scope);

                ruleTwo = service.Resolve(validationContextMock.Object).First();
            }

            ruleOne.Should().NotBeSameAs(ruleTwo);
        }

        [Fact]
        public void Resolve_FilteredValidationItems()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<ValidationErrorCacheGenericTest<string>>().As<IValidationErrorCache<string>>();
            builder.RegisterType<RuleOne>().As<IRule<string>>();
            builder.RegisterType<RuleTwo>().As<IRule<string>>();

            var validationContextMock = new Mock<IValidationContext>();

            validationContextMock.SetupGet(x => x.IgnoredRules).Returns(new List<string> { "RuleTwo" });

            var container = builder.Build();

            using (var scope = container.BeginLifetimeScope())
            {
                var service = NewService(scope);

                var rules = service.Resolve(validationContextMock.Object).ToList();

                rules.Count.Should().Be(1);
                rules.Should().AllBeAssignableTo<IRule<string>>();
                rules[0].Should().BeOfType<RuleOne>();
            }
        }

        private IRuleSetResolutionService<string> NewService(ILifetimeScope lifetimeScope)
        {
            return new AutoFacRuleSetResolutionService<string>(lifetimeScope);
        }
    }
}
