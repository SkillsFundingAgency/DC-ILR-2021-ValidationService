﻿using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Providers.Utils;
using ESFA.DC.JobContextManager.Interface;
using ESFA.DC.JobContextManager.Model;
using ESFA.DC.JobContextManager.Model.Interface;
using ESFA.DC.Logging.Interfaces;
using ExecutionContext = ESFA.DC.Logging.ExecutionContext;

namespace ESFA.DC.ILR.ValidationService.Stateless.Handlers
{
    public class MessageHandler : IMessageHandler<JobContextMessage>
    {
        private readonly ILifetimeScope _parentLifeTimeScope;
        private readonly IValidationContextFactory<IJobContextMessage> _validationContextFactory;
        private readonly StatelessServiceContext _context;

        public MessageHandler(ILifetimeScope parentLifeTimeScope, IValidationContextFactory<IJobContextMessage> validationContextFactory, StatelessServiceContext context)
        {
            _parentLifeTimeScope = parentLifeTimeScope;
            _validationContextFactory = validationContextFactory;
            _context = context;
        }

        public async Task<bool> HandleAsync(JobContextMessage jobContextMessage, CancellationToken cancellationToken)
        {
            var validationContext = _validationContextFactory.Build(jobContextMessage);

            using (var childLifeTimeScope = _parentLifeTimeScope.BeginLifetimeScope())
            {
                var executionContext = (ExecutionContext)childLifeTimeScope.Resolve<IExecutionContext>();
                executionContext.JobId = jobContextMessage.JobId.ToString();
                var logger = childLifeTimeScope.Resolve<ILogger>();

                try
                {
                    logger.LogDebug("inside process message validate");

                    var preValidationOrchestrationService = childLifeTimeScope.Resolve<IPreValidationOrchestrationService>();

                    try
                    {
                        await preValidationOrchestrationService.ExecuteAsync(validationContext, cancellationToken);
                    }
                    catch (ValidationSeverityFailException ex)
                    {
                        logger.LogDebug(ex.Message);
                    }

                    logger.LogDebug("Validation complete");
                    ServiceEventSource.Current.ServiceMessage(_context, "Validation complete");
                    return true;
                }
                catch (OutOfMemoryException oom)
                {
                    Environment.FailFast("Validation Service Out Of Memory", oom);
                    throw;
                }
                catch (Exception ex)
                {
                    ServiceEventSource.Current.ServiceMessage(_context, "Exception-{0}", ex.ToString());
                    logger.LogError("Error while processing job", ex);
                    throw;
                }
            }
        }
    }
}
