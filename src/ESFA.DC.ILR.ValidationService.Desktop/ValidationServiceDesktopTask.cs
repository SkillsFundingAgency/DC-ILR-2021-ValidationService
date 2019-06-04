using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.ILR.Desktop.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.ILR.ValidationService.Desktop
{
    public class ValidationServiceDesktopTask : IDesktopTask
    {
        private readonly ILifetimeScope _parentLifetimeScope;
        private readonly IValidationContextFactory<IDesktopContext> _validationContextFactory;

        public ValidationServiceDesktopTask(ILifetimeScope parentLifetimeScope, IValidationContextFactory<IDesktopContext> validationContextFactory)
        {
            _parentLifetimeScope = parentLifetimeScope;
            _validationContextFactory = validationContextFactory;
        }

        public async Task<IDesktopContext> ExecuteAsync(IDesktopContext desktopContext, CancellationToken cancellationToken)
        {
            var context = _validationContextFactory.Build(desktopContext);

            using (var childLifeTimeScope = _parentLifetimeScope.BeginLifetimeScope())
            {
                var logger = childLifeTimeScope.Resolve<ILogger>();

                try
                {
                    logger.LogDebug("Validation start");
                    var orchestrationService = childLifeTimeScope.Resolve<IPreValidationOrchestrationService>();

                    await orchestrationService.ExecuteAsync(context, cancellationToken);

                    logger.LogDebug("Validation complete");

                    return desktopContext;
                }
                catch (Exception ex)
                {
                    logger.LogError("Error while processing job", ex);
                    throw;
                }
            }
        }
    }
}
