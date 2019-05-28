using System;
using System.Collections.Generic;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using ESFA.DC.ILR.Model;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Cache;
using ESFA.DC.ILR.ValidationService.Data.Extensions;
using ESFA.DC.ILR.ValidationService.Data.External;
using ESFA.DC.ILR.ValidationService.Data.File;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Internal;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.ValidationActor.Interfaces;
using ESFA.DC.ILR.ValidationService.ValidationActor.Interfaces.Models;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using ExecutionContext = ESFA.DC.Logging.ExecutionContext;

namespace ESFA.DC.ILR.ValidationService.ValidationActor
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.None)]
    public class ValidationActor : Actor, IValidationActor
    {
        private readonly ILifetimeScope _parentLifeTimeScope;
        private readonly IExecutionContext _executionContext;
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly IValidationContextFactory<ValidationActorModel> _validationContextFactory;
        private readonly ActorId _actorId;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationActor"/> class.
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        /// <param name="parentLifeTimeScope">Autofac Parent Lifetime Scope</param>
        /// <param name="executionContext">The logger execution context.</param>
        /// <param name="jsonSerializationService">JSON serialiser.</param>
        public ValidationActor(ActorService actorService, ActorId actorId, ILifetimeScope parentLifeTimeScope, IExecutionContext executionContext, IJsonSerializationService jsonSerializationService, IValidationContextFactory<ValidationActorModel> validationContextFactory)
            : base(actorService, actorId)
        {
            _parentLifeTimeScope = parentLifeTimeScope;
            _executionContext = executionContext;
            _jsonSerializationService = jsonSerializationService;
            _validationContextFactory = validationContextFactory;
            _actorId = actorId;
        }

        public async Task<string> Validate(ValidationActorModel actorModel, CancellationToken cancellationToken)
        {
            IEnumerable<IValidationError> results = await RunValidation(actorModel, cancellationToken);
            actorModel = null;

            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true, true);

            return _jsonSerializationService.Serialize(results);
        }

        private async Task<IEnumerable<IValidationError>> RunValidation(ValidationActorModel actorModel, CancellationToken cancellationToken)
        {
            if (_executionContext is ExecutionContext executionContextObj)
            {
                executionContextObj.JobId = "-1";
                executionContextObj.TaskKey = _actorId.ToString();
            }

            var validationContext = _validationContextFactory.Build(actorModel);

            ILogger logger = _parentLifeTimeScope.Resolve<ILogger>();

            InternalDataCache internalDataCache;
            ExternalDataCache externalDataCacheGet;
            ExternalDataCache externalDataCache;
            FileDataCache fileDataCache;
            Message message;
            IEnumerable<IValidationError> errors;

            try
            {
                logger.LogDebug($"{nameof(ValidationActor)} {_actorId} {GC.GetGeneration(actorModel)} starting");

                internalDataCache = _jsonSerializationService.Deserialize<InternalDataCache>(actorModel.InternalDataCache);
                externalDataCacheGet = _jsonSerializationService.Deserialize<ExternalDataCache>(actorModel.ExternalDataCache);
                fileDataCache = _jsonSerializationService.Deserialize<FileDataCache>(actorModel.FileDataCache);
                message = _jsonSerializationService.Deserialize<Message>(actorModel.Message);

                externalDataCache = new ExternalDataCache
                {
                    LearningDeliveries = externalDataCacheGet.LearningDeliveries.ToCaseInsensitiveDictionary(),
                    EPAOrganisations = externalDataCacheGet.EPAOrganisations.ToCaseInsensitiveDictionary(),
                    ERNs = externalDataCacheGet.ERNs,
                    FCSContractAllocations = externalDataCacheGet.FCSContractAllocations,
                    Organisations = externalDataCacheGet.Organisations,
                    Postcodes = externalDataCacheGet.Postcodes.ToCaseInsensitiveHashSet(),
                    ONSPostcodes = externalDataCacheGet.ONSPostcodes,
                    Standards = externalDataCacheGet.Standards,
                    StandardValidities = externalDataCacheGet.StandardValidities,
                    ULNs = externalDataCacheGet.ULNs,
                    ValidationErrors = externalDataCacheGet.ValidationErrors.ToCaseInsensitiveDictionary(),
                    CampusIdentifiers = externalDataCacheGet.CampusIdentifiers
                };

                logger.LogDebug($"{nameof(ValidationActor)} {_actorId} {GC.GetGeneration(actorModel)} finished getting input data");

                cancellationToken.ThrowIfCancellationRequested();
            }
            catch (Exception ex)
            {
                ActorEventSource.Current.ActorMessage(this, "Exception-{0}", ex.ToString());
                logger.LogError($"Error while processing {nameof(ValidationActor)}", ex);
                throw;
            }

            using (var childLifeTimeScope = _parentLifeTimeScope.BeginLifetimeScope(c =>
            {
                c.RegisterInstance(new Cache<IMessage> { Item = message }).As<ICache<IMessage>>();
                c.RegisterInstance(internalDataCache).As<IInternalDataCache>();
                c.RegisterInstance(externalDataCache).As<IExternalDataCache>();
                c.RegisterInstance(fileDataCache).As<IFileDataCache>();
            }))
            {
                ExecutionContext executionContext = (ExecutionContext)childLifeTimeScope.Resolve<IExecutionContext>();
                executionContext.JobId = actorModel.JobId;
                executionContext.TaskKey = _actorId.ToString();
                ILogger jobLogger = childLifeTimeScope.Resolve<ILogger>();
                try
                {
                    jobLogger.LogDebug($"{nameof(ValidationActor)} {_actorId} {GC.GetGeneration(actorModel)} {executionContext.TaskKey} started learners: {message.Learners.Count}");
                    IRuleSetOrchestrationService<ILearner, IValidationError> preValidationOrchestrationService = childLifeTimeScope
                        .Resolve<IRuleSetOrchestrationService<ILearner, IValidationError>>();

                    errors = await preValidationOrchestrationService.ExecuteAsync(validationContext, cancellationToken);
                    jobLogger.LogDebug($"{nameof(ValidationActor)} {_actorId} {GC.GetGeneration(actorModel)} {executionContext.TaskKey} validation done");
                }
                catch (Exception ex)
                {
                    ActorEventSource.Current.ActorMessage(this, "Exception-{0}", ex.ToString());
                    jobLogger.LogError($"Error while processing {nameof(ValidationActor)}", ex);
                    throw;
                }
            }

            internalDataCache = null;
            externalDataCache = null;
            fileDataCache = null;
            message = null;

            return errors;
        }
    }
}
