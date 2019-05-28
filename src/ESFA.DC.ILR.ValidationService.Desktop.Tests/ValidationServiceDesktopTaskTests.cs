using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Desktop.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using FluentAssertions;
using Moq;
using Xunit;

namespace ESFA.DC.ILR.ValidationService.Desktop.Tests
{
    public class ValidationServiceDesktopTaskTests
    {
        [Fact]
        public async Task ExecuteAsync()
        {
            var desktopContextMock = new Mock<IDesktopContext>();
            var cancellationToken = CancellationToken.None;

            var validationContextFactoryMock = new Mock<IValidationContextFactory<IDesktopContext>>();
            var validationContextMock = new Mock<IValidationContext>();

            validationContextFactoryMock.Setup(f => f.Build(desktopContextMock.Object)).Returns(validationContextMock.Object);
            
            var task = NewTask(validationContextFactoryMock.Object);
            
            var result = await task.ExecuteAsync(desktopContextMock.Object, cancellationToken);

            result.Should().Be(desktopContextMock.Object);
        }

        private ValidationServiceDesktopTask NewTask(IValidationContextFactory<IDesktopContext> validationContextFactory = null)
        {
            return new ValidationServiceDesktopTask(validationContextFactory);
        }
    }
}
