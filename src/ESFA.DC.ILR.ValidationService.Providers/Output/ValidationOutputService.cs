using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.FileService.Interface;
using ESFA.DC.ILR.IO.Model.Validation;
using ESFA.DC.ILR.Model.Interface;
using ESFA.DC.ILR.ValidationService.Data.Interface;
using ESFA.DC.ILR.ValidationService.Interface;
using ESFA.DC.ILR.ValidationService.Interface.Enum;
using ESFA.DC.ILR.ValidationService.IO.Model;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Serialization.Interfaces;

namespace ESFA.DC.ILR.ValidationService.Providers.Output
{
    public class ValidationOutputService : IValidationOutputService
    {
        private const string Error = "E";
        private const string Warning = "W";
        private const string Fail = "F";

        private readonly IValidationErrorCache<IValidationError> _validationErrorCache;
        private readonly ICache<IMessage> _messageCache;
        private readonly IFileService _fileService;
        private readonly IPreValidationContext _validationContext;
        private readonly IJsonSerializationService _serializationService;
        private readonly IValidationErrorsDataService _validationErrorsDataService;
        private readonly ILogger _logger;

        public ValidationOutputService(
            IValidationErrorCache<IValidationError> validationErrorCache,
            ICache<IMessage> messageCache,
            IFileService fileService,
            IPreValidationContext validationContext,
            IJsonSerializationService serializationService,
            IValidationErrorsDataService validationErrorsDataService,
            ILogger logger)
        {
            _validationErrorCache = validationErrorCache;
            _messageCache = messageCache;
            _fileService = fileService;
            _validationContext = validationContext;
            _serializationService = serializationService;
            _validationErrorsDataService = validationErrorsDataService;
            _logger = logger;
        }

        public async Task ProcessAsync(CancellationToken cancellationToken)
        {
            var existingValidationErrors = await GetExistingValidationErrors(cancellationToken);

            var validationErrors = _validationErrorCache
                .ValidationErrors
                .Select(ve => new ValidationError
                {
                    LearnerReferenceNumber = ve.LearnerReferenceNumber,
                    AimSequenceNumber = ve.AimSequenceNumber,
                    RuleName = ve.RuleName,
                    Severity = SeverityToString(ve.Severity),
                    ValidationErrorParameters = ve.ErrorMessageParameters?
                    .Select(emp => new ValidationErrorParameter
                    {
                        PropertyName = emp.PropertyName,
                        Value = emp.Value
                    }).ToList()
                }).ToList();

            validationErrors.AddRange(existingValidationErrors);

            var invalidLearnerRefNumbers = BuildInvalidLearnRefNumbers(validationErrors).ToList();
            var validLearnerRefNumbers = BuildValidLearnRefNumbers(_messageCache.Item, invalidLearnerRefNumbers).ToList();
            _logger.LogDebug($"ValidationOutputService invalid:{invalidLearnerRefNumbers.Count} valid:{validLearnerRefNumbers.Count}");

            var validationErrorMessageLookups = validationErrors
                .Select(ve => ve.RuleName)
                .Distinct()
                .Select(rn => new ValidationErrorMessageLookup
                {
                    RuleName = rn,
                    Message = _validationErrorsDataService.MessageforRuleName(rn)
                }).ToList();

            await SaveAsync(
                validLearnerRefNumbers,
                invalidLearnerRefNumbers,
                validationErrors,
                validationErrorMessageLookups,
                cancellationToken);
        }

        public IEnumerable<string> BuildInvalidLearnRefNumbers(IEnumerable<ValidationError> validationErrors)
        {
            return validationErrors
                .Where(x => !string.IsNullOrEmpty(x.LearnerReferenceNumber)
                    && x.Severity == Error)
                .Select(ve => ve.LearnerReferenceNumber)
                .Distinct();
        }

        public IEnumerable<string> BuildValidLearnRefNumbers(IMessage message, IEnumerable<string> invalidLearnRefNumbers)
        {
            var invalidLearnRefNumbersHashSet = invalidLearnRefNumbers != null ? new HashSet<string>(invalidLearnRefNumbers) : new HashSet<string>();

            return message?
                       .Learners?
                       .Select(l => l.LearnRefNumber)
                       .Where(lrn => !invalidLearnRefNumbersHashSet.Contains(lrn))
                       .Distinct()
                   ?? new List<string>();
        }

        public async Task SaveAsync(
            IEnumerable<string> validLearnerRefNumbers,
            IEnumerable<string> invalidLearnerRefNumbers,
            IEnumerable<ValidationError> validationErrors,
            IEnumerable<ValidationErrorMessageLookup> validationErrorMessageLookups,
            CancellationToken cancellationToken)
        {
            var validLearnRefNumbersKey = _validationContext.ValidLearnRefNumbersKey;
            var invalidLearnRefNumbersKey = _validationContext.InvalidLearnRefNumbersKey;
            var validationErrorsKey = _validationContext.ValidationErrorsKey;
            var validationErrorMessageLookupKey = _validationContext.ValidationErrorMessageLookupKey;

            var validationContext = _validationContext;
            validationContext.InvalidLearnRefNumbersCount = invalidLearnerRefNumbers.Count();
            validationContext.ValidLearnRefNumbersCount = validLearnerRefNumbers.Count();
            validationContext.ValidationTotalErrorCount = validationErrors.Count(x => x.Severity == Error);
            validationContext.ValidationTotalWarningCount = validationErrors.Count(x => x.Severity == Warning);

            await Task.WhenAll(
                OutputAsync(validLearnRefNumbersKey, _validationContext.Container, validLearnerRefNumbers, cancellationToken),
                OutputAsync(invalidLearnRefNumbersKey, _validationContext.Container, invalidLearnerRefNumbers, cancellationToken),
                OutputAsync(validationErrorsKey, _validationContext.Container, validationErrors, cancellationToken),
                OutputAsync(validationErrorMessageLookupKey, _validationContext.Container, validationErrorMessageLookups, cancellationToken));
        }

        public string SeverityToString(Severity? severity)
        {
            switch (severity)
            {
                case Severity.Error:
                    return Error;
                case Severity.Warning:
                    return Warning;
                case Severity.Fail:
                    return Fail;
                case null:
                    return null;
                default:
                    return null;
            }
        }

        private async Task OutputAsync<T>(string key, string container, IEnumerable<T> output, CancellationToken cancellationToken)
        {
            using (var fileStream = await _fileService.OpenWriteStreamAsync(key, container, cancellationToken))
            {
                _serializationService.Serialize(output, fileStream);
            }
        }

        private async Task<IEnumerable<ValidationError>> GetExistingValidationErrors(CancellationToken cancellationToken)
        {
            IEnumerable<ValidationError> validationErrors = new List<ValidationError>();

            try
            {
                using (var stream = await _fileService.OpenReadStreamAsync(_validationContext.ValidationErrorsKey, _validationContext.Container, cancellationToken))
                {
                    stream.Position = 0;

                    validationErrors = _serializationService.Deserialize<List<ValidationError>>(stream);
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Failed To get Existing Validation Errors, assume none available and carry on.", e);
            }

            return validationErrors;
        }
    }
}
