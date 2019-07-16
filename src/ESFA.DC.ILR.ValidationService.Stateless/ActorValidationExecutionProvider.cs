using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.ValidationActor.Interfaces;
using ESFA.DC.ILR.ValidationService.ValidationActor.Interfaces.Models;
using ESFA.DC.ILR.ValidationService.ValidationActor.Model;
using ESFA.DC.ILR.ValidationService.ValidationDPActor.Interfaces;
using ESFA.DC.ILR.ValidationService.ValidationDPActor.Interfaces.Models;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;

namespace ESFA.DC.ILR.ValidationService.Stateless
{
    public class ActorValidationExecutionProvider : IValidationExecutionProvider
    {
        private const string _validationActorServiceName = "ValidationActorService";
        private const string _validationDPActorServiceName = "ValidationDPActorService";

        private readonly ILearnerPerActorProviderService _learnerPerActorProviderService;
        private readonly ILearnerDPPerActorProviderService _learnerDPPerActorProviderService;
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly IInternalDataCache _internalDataCache;
        private readonly IExternalDataCache _externalDataCache;
        private readonly IFileDataCache _fileDataCache;
        private readonly IValidationErrorCache _validationErrorCache;
        private readonly ILogger _logger;

        public ActorValidationExecutionProvider(
            ILearnerPerActorProviderService learnerPerActorProviderService,
            ILearnerDPPerActorProviderService learnerDPPerActorProviderService,
            IJsonSerializationService jsonSerializationService,
            IInternalDataCache internalDataCache,
            IExternalDataCache externalDataCache,
            IFileDataCache fileDataCache,
            IValidationErrorCache validationErrorCache,
            ILogger logger)
        {
            _learnerPerActorProviderService = learnerPerActorProviderService;
            _learnerDPPerActorProviderService = learnerDPPerActorProviderService;
            _jsonSerializationService = jsonSerializationService;
            _internalDataCache = internalDataCache;
            _externalDataCache = externalDataCache;
            _fileDataCache = fileDataCache;
            _validationErrorCache = validationErrorCache;
            _logger = logger;
        }

        public async Task ExecuteAsync(IValidationContext validationContext, IMessage message, CancellationToken cancellationToken)
        {
            // Get L/A and split the learners into separate lists
            var learnerMessageShards = _learnerPerActorProviderService.Provide(message).ToList();
            var learnerDPMessageShards = _learnerDPPerActorProviderService.Provide(message).ToList();

            List<IValidationActor> learnerValidationActors = new List<IValidationActor>();
            List<IValidationDPActor> learnerDPValidationActors = new List<IValidationDPActor>();
            List<Task<string>> actorTasks = new List<Task<string>>();
            List<Task> actorDestroys = new List<Task>();

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            _logger.LogDebug($"Validation will create {learnerMessageShards?.Count() ?? 0} actors");
            _logger.LogDebug($"DP Validation will create {learnerDPMessageShards?.Count() ?? 0} actors");

            string internalDataCacheAsString = _jsonSerializationService.Serialize(_internalDataCache);
            _logger.LogDebug($"_internalDataCache {internalDataCacheAsString.Length}");

            string fileDataCacheAsString = _jsonSerializationService.Serialize(_fileDataCache);
            _logger.LogDebug($"fileDataCacheAsString {fileDataCacheAsString.Length}");

            string externalDataCacheAsString = _jsonSerializationService.Serialize(_externalDataCache);
            _logger.LogDebug($"ExternalDataCache: {externalDataCacheAsString.Length}");

            if (learnerMessageShards != null)
            {
                foreach (IMessage messageShard in learnerMessageShards)
                {
                    _logger.LogDebug($"Validation Shard has {messageShard.Learners.Count} learners");

                    var actor = CreateValidationActor<IValidationActor, ValidationActorModel>(validationContext, messageShard, learnerValidationActors, internalDataCacheAsString, externalDataCacheAsString, fileDataCacheAsString, _validationActorServiceName, out var validationActorModel);

                    actorTasks.Add(actor.Validate(validationActorModel, cancellationToken));
                }
            }

            if (learnerDPMessageShards != null)
            {
                foreach (IMessage messageShard in learnerDPMessageShards)
                {
                    _logger.LogDebug($"Validation Shard has {messageShard.LearnerDestinationAndProgressions.Count} learnersDestinationAndProgressions");

                    var actor = CreateValidationActor<IValidationDPActor, ValidationDPActorModel>(validationContext, messageShard, learnerDPValidationActors, internalDataCacheAsString, externalDataCacheAsString, fileDataCacheAsString, _validationDPActorServiceName, out var validationActorModel);

                    actorTasks.Add(actor.Validate(validationActorModel, cancellationToken));
                }
            }

            _logger.LogDebug($"Starting {actorTasks.Count} validation actors after {stopWatch.ElapsedMilliseconds}ms prep time");
            stopWatch.Restart();

            await Task.WhenAll(actorTasks.ToArray()).ConfigureAwait(false);

            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogDebug($"Collating {actorTasks.Count} validation actors after {stopWatch.ElapsedMilliseconds}ms execution time");
            stopWatch.Restart();

            foreach (Task<string> actorTask in actorTasks)
            {
                IEnumerable<IValidationError> errors = _jsonSerializationService.Deserialize<IEnumerable<IValidationError>>(actorTask.Result);

                foreach (IValidationError error in errors)
                {
                    _validationErrorCache.Add(error);
                }
            }

            _logger.LogDebug($"Destroying {actorTasks.Count} validation actors after {stopWatch.ElapsedMilliseconds}ms collation time");

            foreach (IValidationActor validationActor in learnerValidationActors)
            {
                actorDestroys.Add(DestroyActorSync(validationActor, _validationActorServiceName, cancellationToken));
            }

            foreach (IValidationDPActor validationDPActor in learnerDPValidationActors)
            {
                actorDestroys.Add(DestroyActorSync(validationDPActor, _validationDPActorServiceName, cancellationToken));
            }

            await Task.WhenAll(actorDestroys.ToArray()).ConfigureAwait(false);
        }

        private T CreateValidationActor<T, S>(
            IValidationContext validationContext,
            IMessage messageShard,
            List<T> learnerValidationActors,
            string internalDataCacheAsString,
            string externalDataCacheAsString,
            string fileDataCacheAsString,
            string actorServiceName,
            out S validationActorModel)
            where T : IActor
            where S : ActorModel, new()
        {
            // create actors for each Shard.
            T actor = GetActor<T>(actorServiceName);
            learnerValidationActors.Add(actor);

            // TODO:get reference data per each shard and send it to Actors
            string ilrMessageAsString = _jsonSerializationService.Serialize(messageShard);

            validationActorModel = new S
            {
                JobId = validationContext.JobId,
                Message = ilrMessageAsString,
                InternalDataCache = internalDataCacheAsString,
                ExternalDataCache = externalDataCacheAsString,
                FileDataCache = fileDataCacheAsString
            };

            return actor;
        }

        private T GetActor<T>(string serviceName)
            where T : IActor
        {
            return ActorProxy.Create<T>(
                ActorId.CreateRandom(),
                new Uri($"{FabricRuntime.GetActivationContext().ApplicationName}/{serviceName}"));
        }

        private async Task DestroyActorSync<T>(T actor, string serviceName, CancellationToken cancellationToken)
            where T : IActor
        {
            try
            {
                ActorId actorId = actor.GetActorId();

                IActorService myActorServiceProxy = ActorServiceProxy.Create(
                    new Uri($"{FabricRuntime.GetActivationContext().ApplicationName}/{serviceName}"),
                    actorId);

                await myActorServiceProxy.DeleteActorAsync(actorId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Problem deleting actor", ex);
            }
        }
    }
}
