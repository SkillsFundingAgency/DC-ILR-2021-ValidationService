using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Fabric;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Cache;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Data.Population.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Interface.Enum;
using ESFA.DC.ILR.ValidationService.ValidationActor.Interfaces;
using ESFA.DC.ILR.ValidationService.ValidationActor.Interfaces.Models;
using ESFA.DC.ILR.ValidationService.ValidationDPActor.Interfaces;
using ESFA.DC.ILR.ValidationService.ValidationDPActor.Interfaces.Models;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;

namespace ESFA.DC.ILR.ValidationService.Providers
{
    public class PreValidationOrchestrationSfService<U> : IPreValidationOrchestrationService<U>
    {
        private readonly IPopulationService _preValidationPopulationService;
        private readonly ILearnerPerActorProviderService _learnerPerActorProviderService;
        private readonly ILearnerDPPerActorProviderService _learnerDPPerActorProviderService;
        private readonly IJsonSerializationService _jsonSerializationService;
        private readonly IInternalDataCache _internalDataCache;
        private readonly IExternalDataCache _externalDataCache;
        private readonly IFileDataCache _fileDataCache;
        private readonly IValidationErrorCache<U> _validationErrorCache;
        private readonly IValidationOutputService _validationOutputService;
        private readonly IRuleSetOrchestrationService<IMessage, U> _ruleSetOrchestrationService;
        private readonly ILogger _logger;

        public PreValidationOrchestrationSfService(
            IPopulationService preValidationPopulationService,
            ILearnerPerActorProviderService learnerPerActorProviderService,
            ILearnerDPPerActorProviderService learnerDPPerActorProviderService,
            IJsonSerializationService jsonSerializationService,
            IInternalDataCache internalDataCache,
            IExternalDataCache externalDataCache,
            IFileDataCache fileDataCache,
            IValidationErrorCache<U> validationErrorCache,
            IValidationOutputService validationOutputService,
            IRuleSetOrchestrationService<IMessage, U> ruleSetOrchestrationService,
            ILogger logger)
        {
            _preValidationPopulationService = preValidationPopulationService;
            _learnerPerActorProviderService = learnerPerActorProviderService;
            _learnerDPPerActorProviderService = learnerDPPerActorProviderService;
            _jsonSerializationService = jsonSerializationService;
            _internalDataCache = internalDataCache;
            _externalDataCache = externalDataCache;
            _fileDataCache = fileDataCache;
            _validationErrorCache = validationErrorCache;
            _validationOutputService = validationOutputService;
            _ruleSetOrchestrationService = ruleSetOrchestrationService;
            _logger = logger;
        }

        public async Task ExecuteAsync(IValidationContext validationContext, CancellationToken cancellationToken)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                if (_validationErrorCache.ValidationErrors.Any())
                {
                    return;
                }

                cancellationToken.ThrowIfCancellationRequested();

                await _preValidationPopulationService.PopulateAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogDebug($"Population service completed in: {stopWatch.ElapsedMilliseconds}");

                // Set the filename
                _fileDataCache.FileName = validationContext.Filename;

                // File Validation
                await _ruleSetOrchestrationService.ExecuteAsync(validationContext.Tasks, cancellationToken).ConfigureAwait(false);

                cancellationToken.ThrowIfCancellationRequested();

                if (_validationErrorCache.ValidationErrors.Any(IsFail))
                {
                    _logger.LogDebug(
                        $"File schema catastrophic error, so will not execute learner validation actors, error count: {_validationErrorCache.ValidationErrors.Count}");
                    return;
                }

                await ExecuteValidationActors(validationContext, cancellationToken).ConfigureAwait(false);

                _logger.LogDebug(
                    $"Actors results collated {_validationErrorCache.ValidationErrors.Count} validation errors");
            }
            catch (Exception ex)
            {
                _logger.LogError("Validation Critical Error", ex);
                throw;
            }
            finally
            {
                cancellationToken.ThrowIfCancellationRequested();
                await _validationOutputService.ProcessAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogDebug($"Validation final results persisted in {stopWatch.ElapsedMilliseconds}");
            }
        }

        private IValidationActor GetValidationActor()
        {
            return ActorProxy.Create<IValidationActor>(
                ActorId.CreateRandom(),
                new Uri($"{FabricRuntime.GetActivationContext().ApplicationName}/ValidationActorService"));
        }

        private IValidationDPActor GetValidationDPActor()
        {
            return ActorProxy.Create<IValidationDPActor>(
                ActorId.CreateRandom(),
                new Uri($"{FabricRuntime.GetActivationContext().ApplicationName}/ValidationDPActorService"));
        }

        private async Task DestroyValidationActorAsync(IValidationActor validationActor, CancellationToken cancellationToken)
        {
            try
            {
                ActorId actorId = validationActor.GetActorId();

                IActorService myActorServiceProxy = ActorServiceProxy.Create(
                    new Uri($"{FabricRuntime.GetActivationContext().ApplicationName}/ValidationActorService"),
                    actorId);

                await myActorServiceProxy.DeleteActorAsync(actorId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Problem deleting actor", ex);
            }
        }

        private async Task DestroyValidationDPActorAsync(IValidationDPActor validationDPActor, CancellationToken cancellationToken)
        {
            try
            {
                ActorId actorId = validationDPActor.GetActorId();

                IActorService myActorServiceProxy = ActorServiceProxy.Create(
                    new Uri($"{FabricRuntime.GetActivationContext().ApplicationName}/ValidationActorService"),
                    actorId);

                await myActorServiceProxy.DeleteActorAsync(actorId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Problem deleting actor", ex);
            }
        }

        private async Task ExecuteValidationActors(IValidationContext validationContext, CancellationToken cancellationToken)
        {
            // Get L/A and split the learners into separate lists
            IEnumerable<IMessage> learnerMessageShards = await _learnerPerActorProviderService.ProvideAsync();
            IEnumerable<IMessage> learnerDPMessageShards = await _learnerDPPerActorProviderService.ProvideAsync();

            List<IValidationActor> learnerValidationActors = new List<IValidationActor>();
            List<IValidationDPActor> learnerDPValidationActors = new List<IValidationDPActor>();
            List<Task<string>> actorTasks = new List<Task<string>>();
            List<Task> actorDestroys = new List<Task>();

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            _logger.LogDebug($"Validation will create {learnerMessageShards?.Count() ?? 0} actors");
            _logger.LogDebug($"DP Validation will create {learnerDPMessageShards?.Count() ?? 0} actors");

            string internalDataCacheAsString =
                _jsonSerializationService.Serialize(_internalDataCache);
            _logger.LogDebug($"_internalDataCache {internalDataCacheAsString.Length}");
            string fileDataCacheAsString =
                _jsonSerializationService.Serialize(_fileDataCache);
            _logger.LogDebug($"fileDataCacheAsString {fileDataCacheAsString.Length}");
            string externalDataCacheAsString =
                _jsonSerializationService.Serialize(_externalDataCache);
            _logger.LogDebug($"ExternalDataCache: {externalDataCacheAsString.Length}");
            string taskListAsString = _jsonSerializationService.Serialize(validationContext.Tasks);
            _logger.LogDebug($"taskListAsString {taskListAsString.Length}");

            if (learnerMessageShards != null)
            {
                foreach (IMessage messageShard in learnerMessageShards)
                {
                    _logger.LogDebug($"Validation Shard has {messageShard.Learners.Count} learners");

                    // create actors for each Shard.
                    IValidationActor actor = GetValidationActor();
                    learnerValidationActors.Add(actor);

                    // TODO:get reference data per each shard and send it to Actors
                    string ilrMessageAsString = _jsonSerializationService.Serialize(messageShard);

                    ValidationActorModel validationActorModel = new ValidationActorModel
                    {
                        JobId = validationContext.JobId,
                        Message = ilrMessageAsString,
                        InternalDataCache = internalDataCacheAsString,
                        ExternalDataCache = externalDataCacheAsString,
                        FileDataCache = fileDataCacheAsString,
                        TaskList = taskListAsString
                    };

                    actorTasks.Add(actor.Validate(validationActorModel, cancellationToken));
                }
            }

            if (learnerDPMessageShards != null)
            {
                foreach (IMessage messageShard in learnerDPMessageShards)
                {
                    _logger.LogDebug($"DP Validation Shard has {messageShard.LearnerDestinationAndProgressions.Count} learner DP records");

                    // create actors for each Shard.
                    IValidationDPActor actor = GetValidationDPActor();
                    learnerDPValidationActors.Add(actor);

                    // TODO:get reference data per each shard and send it to Actors
                    string ilrMessageAsString = _jsonSerializationService.Serialize(messageShard);

                    ValidationDPActorModel validationActorModel = new ValidationDPActorModel
                    {
                        JobId = validationContext.JobId,
                        Message = ilrMessageAsString,
                        InternalDataCache = internalDataCacheAsString,
                        ExternalDataCache = externalDataCacheAsString,
                        FileDataCache = fileDataCacheAsString,
                        TaskList = taskListAsString
                    };

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
                IEnumerable<U> errors = _jsonSerializationService.Deserialize<IEnumerable<U>>(actorTask.Result);

                foreach (U error in errors)
                {
                    _validationErrorCache.Add(error);
                }
            }

            _logger.LogDebug($"Destroying {actorTasks.Count} validation actors after {stopWatch.ElapsedMilliseconds}ms collation time");

            foreach (IValidationActor validationActor in learnerValidationActors)
            {
                actorDestroys.Add(DestroyValidationActorAsync(validationActor, cancellationToken));
            }

            foreach (IValidationDPActor validationDPActor in learnerDPValidationActors)
            {
                actorDestroys.Add(DestroyValidationDPActorAsync(validationDPActor, cancellationToken));
            }

            await Task.WhenAll(actorDestroys.ToArray()).ConfigureAwait(false);
        }

        private bool IsErrorOrFail(U item)
        {
            Severity severity = ((IValidationError)item).Severity ?? Severity.Error;
            return severity == Severity.Error || severity == Severity.Fail;
        }

        private bool IsFail(U item)
        {
            Severity severity = ((IValidationError)item).Severity ?? Severity.Error;
            return severity == Severity.Fail;
        }
    }
}
