using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Desktop.Modules;
using ESFA.DC.ILR.ValidationService.Interface;
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

            var rulesetOrchestrationService = container.Resolve<IRuleSetOrchestrationService<IMessage>>();

            rulesetOrchestrationService.Should().NotBeNull();
        }
    }
}
