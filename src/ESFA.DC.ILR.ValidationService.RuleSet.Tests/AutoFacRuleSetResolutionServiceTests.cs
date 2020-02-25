using System.Collections.Generic;
using System.Linq;
using Autofac;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.RuleSet.ErrorHandler;
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

            var enabledRulesProviderMock = new Mock<IEnabledRulesProvider>();

            enabledRulesProviderMock.Setup(x => x.Provide()).Returns(new List<string>());

            var container = builder.Build();

            using (var scope = container.BeginLifetimeScope())
            {
                var service = NewService(scope, enabledRulesProviderMock.Object);

                service.Resolve().Should().BeEmpty();
            }
        }

        [Fact]
        public void Resolve_Single()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<ValidationErrorCache>().As<IValidationErrorCache>();
            builder.RegisterType<RuleOne>().As<IRule<string>>();

            var enabledRulesProviderMock = new Mock<IEnabledRulesProvider>();

            enabledRulesProviderMock.Setup(x => x.Provide()).Returns(new List<string> { "RuleOne" });

            var container = builder.Build();

            using (var scope = container.BeginLifetimeScope())
            {
                var service = NewService(scope, enabledRulesProviderMock.Object);

                service.Resolve().First().Should().BeOfType<RuleOne>();
            }
        }

        [Fact]
        public void Resolve_Multiple()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<ValidationErrorCache>().As<IValidationErrorCache>();
            builder.RegisterType<RuleOne>().As<IRule<string>>();
            builder.RegisterType<RuleTwo>().As<IRule<string>>();

            var enabledRulesProviderMock = new Mock<IEnabledRulesProvider>();

            enabledRulesProviderMock.Setup(x => x.Provide()).Returns(new List<string> { "RuleOne", "RuleTwo" });

            var container = builder.Build();

            using (var scope = container.BeginLifetimeScope())
            {
                var service = NewService(scope, enabledRulesProviderMock.Object);

                var rules = service.Resolve().ToList();

                rules.Should().AllBeAssignableTo<IRule<string>>();
                rules[0].Should().BeOfType<RuleOne>();
                rules[1].Should().BeOfType<RuleTwo>();
            }
        }

        [Fact]
        public void Resolve_NewScopeNewRule()
        {
            var builder = new ContainerBuilder();

            var enabledRulesProviderMock = new Mock<IEnabledRulesProvider>();

            enabledRulesProviderMock.Setup(x => x.Provide()).Returns(new List<string> { "RuleOne", "RuleTwo" });

            builder.RegisterType<ValidationErrorCache>().As<IValidationErrorCache>().InstancePerLifetimeScope();
            builder.RegisterType<RuleOne>().As<IRule<string>>().InstancePerLifetimeScope();

            var container = builder.Build();

            IRule<string> ruleOne;
            IRule<string> ruleTwo;

            using (var scope = container.BeginLifetimeScope())
            {
                var service = NewService(scope, enabledRulesProviderMock.Object);

                ruleOne = service.Resolve().First();
            }

            using (var scope = container.BeginLifetimeScope())
            {
                var service = NewService(scope, enabledRulesProviderMock.Object);

                ruleTwo = service.Resolve().First();
            }

            ruleOne.Should().NotBeSameAs(ruleTwo);
        }

        [Fact]
        public void Resolve_DisabledValidationItems()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<ValidationErrorCache>().As<IValidationErrorCache>();
            builder.RegisterType<RuleOne>().As<IRule<string>>();
            builder.RegisterType<RuleTwo>().As<IRule<string>>();

            var enabledRulesProviderMock = new Mock<IEnabledRulesProvider>();

            enabledRulesProviderMock.Setup(x => x.Provide()).Returns(new List<string> { "RuleOne" });

            var container = builder.Build();

            using (var scope = container.BeginLifetimeScope())
            {
                var service = NewService(scope, enabledRulesProviderMock.Object);

                var rules = service.Resolve().ToList();

                rules.Count.Should().Be(1);
                rules.Should().AllBeAssignableTo<IRule<string>>();
                rules[0].Should().BeOfType<RuleOne>();
            }
        }

        private IRuleSetResolutionService<string> NewService(ILifetimeScope lifetimeScope, IEnabledRulesProvider enabledRulesProvider)
        {
            return new AutoFacRuleSetResolutionService<string>(lifetimeScope, enabledRulesProvider);
        }
    }
}
