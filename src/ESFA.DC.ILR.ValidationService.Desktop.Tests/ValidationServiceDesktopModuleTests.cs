using System.Collections.Generic;
using Autofac;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Desktop.Modules;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Rules.Modules;
using FluentAssertions;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Desktop.Tests
{
    public class ValidationServiceDesktopModuleTests
    {
        [Fact]
        public void MessageValidation_Resolve()
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterModule<ValidationServiceDesktopModule>();

            containerBuilder.RegisterCommonServiceStubs();

            var container = containerBuilder.Build();

            var rulesetOrchestrationService = container.Resolve<IRuleSetOrchestrationService<IMessage, IValidationError>>();

            rulesetOrchestrationService.Should().NotBeNull();
        }

        [Fact]
        public void Message_Resolve()
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterModule<MessageRuleSetModule>();

            containerBuilder.RegisterCommonServiceStubs();

            var container = containerBuilder.Build();

            var messageRules = container.Resolve<IEnumerable<IRule<IMessage>>>();

            messageRules.Should().NotBeNull();
        }
    }
}
